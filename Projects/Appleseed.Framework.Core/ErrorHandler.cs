using System;
using System.Collections;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Appleseed.Framework.Exceptions;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Settings.Cache;

namespace Appleseed.Framework
{
    /// <summary>
    /// This class in combination with the Web.Config file handles all the Errors that are not caught programatically
    /// 99% of the time Errors will be caught by Appleseed's HttpUrlModule, this class will be called, errors will be 
    /// logged depending on what was specified by the Web.Config file, after that the error cascades up and is caught
    /// by the customErrors settings in Web.Config. Here you can specify errors and which pages to redirect to.
    /// Visitors will be directed to dynamic aspx pages for General Errors and 404 Errors (Specified aspx page does not exist)
    /// These pages are dynamic and will keep the theme you selected for your portal. It also makes use of Appleseed's
    /// multi-language support. If these dynamic pages themselves have an error (e.g the Database has crashed 
    /// so it can't retrieve the theme or translations, then there is code in these pages to catch errors at the
    /// Page Level and redirect to a static html page (one for general errors and one for 404 errors). 
    /// These pages will have no theme at all, just text (So that they will work across multiple themes) and the 
    /// text will be in English (No Translation - Although multiple versions of the html pages could be created to
    /// handle this. Please specify if it is urgent.
    /// 
    /// Thanks go to  Joan M for the Original Code.
    /// Modified and extended by John Mandia.
    /// Major re-write by Jes1111 - 17/June/2005 - see http://support.Appleseedportal.net/confluence/display/DOX/New+Exception+Handling+and+Logging+features
    /// </summary>
    [
        History("JohnMandia", "john.mandia@whitelightsolutions.com", "1.2", "2003/04/09",
            "Updated LogToFile code to allow users to specify logfile location and specify frequency of the log files daily monthly yearly or all. Also created the LogHelper file with useful functions."
            )]
    [
        History("Manu", "manu-dea@hotmail dot it", "1.3", "2004/05/16",
            "Commented out obsolete code or marked as obsolete. Will be removed in future versions.")]
    public class ErrorHandler
    {
        //		const string strTOE = "Time of Error: ";
        //		const string strSrvName = "SERVER_NAME";
        //		const string strSrc = "Source: ";
        //		const string strErrMsg = "Error Message: ";
        //		const string strTgtSite = "Target Site: ";
        //		const string strStkTrace = "Stack Trace: ";

        /// <summary>
        /// Called only by Application_Error in global.asax.cs to deal with unhandled exceptions.
        /// </summary>
        public static void ProcessUnhandledException()
        {
            try
            {
                Exception e = HttpContext.Current.Server.GetLastError();

                string _myGuid;
                string _auxMessage;
                string _redirectUrl = Config.SmartErrorRedirect; // default value
                LogLevel _logLevel = LogLevel.Fatal; // default value
                HttpStatusCode _httpStatusCode = HttpStatusCode.InternalServerError; // default value
                string myCacheKey = string.Empty;
                StringBuilder sb;

                if (HttpContext.Current.Request != null &&
                    HttpContext.Current.Request.Url.AbsolutePath.EndsWith(Config.SmartErrorRedirect.Substring(2)))
                {
                    HttpContext.Current.Response.Write("Sorry - a critical error has occurred - unable to continue");
                    HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    HttpContext.Current.Response.End();
                }


                try
                {
                    if (e is DatabaseUnreachableException || e is SqlException)
                    {
                        _logLevel = LogLevel.Fatal;
                        _redirectUrl = Config.DatabaseErrorRedirect;
                        _httpStatusCode = Config.DatabaseErrorResponse;
                    }
                    else if (e is DatabaseVersionException) // db version is behind code version
                    {
                        _logLevel = LogLevel.Fatal;
                        _httpStatusCode = Config.DatabaseUpdateResponse;
                        _redirectUrl = Config.DatabaseUpdateRedirect;

                    }
                    else if (e is CodeVersionException) // code version is behind db version
                    {
                        _logLevel = LogLevel.Fatal;
                        _httpStatusCode = Config.CodeUpdateResponse;
                        _redirectUrl = Config.CodeUpdateRedirect;
                    }
                    else if (e is PortalsLockedException) // AllPortals lock is "on"
                    {
                        _logLevel = ((PortalsLockedException)e).Level;
                        _auxMessage = "Attempt to access locked portal by non-keyholder.";
                        _httpStatusCode = ((PortalsLockedException)e).StatusCode;
                        _redirectUrl = Config.LockRedirect;
                        e = null;
                    }
                    else if (e is AppleseedRedirect)
                    {
                        _logLevel = ((AppleseedRedirect)e).Level;
                        _httpStatusCode = ((AppleseedRedirect)e).StatusCode;
                        _redirectUrl = ((AppleseedRedirect)e).RedirectUrl;
                    }
                    else if (e is AppleseedException)
                    {
                        _logLevel = ((AppleseedException)e).Level;
                        _httpStatusCode = ((AppleseedException)e).StatusCode;
                    }
                    else if (e is HttpException)
                    {
                        _logLevel = LogLevel.Fatal;
                        _httpStatusCode = (HttpStatusCode)((HttpException)e).GetHttpCode();
                    }
                    else
                    {
                        _logLevel = LogLevel.Fatal; // default value
                        _httpStatusCode = HttpStatusCode.InternalServerError; // default value
                    }

                    // create unique id
                    _myGuid = Guid.NewGuid().ToString("N");
                    _auxMessage = string.Format("errorGUID: {0}", _myGuid);
                    _auxMessage += string.Format("\nUrl: {0}", HttpContext.Current.Request.Url);
                    _auxMessage += string.Format("\nUrlReferer: {0}", HttpContext.Current.Request.UrlReferrer);
                    _auxMessage += string.Format("\nUser: {0}", HttpContext.Current.User != null ? HttpContext.Current.User.Identity.Name.ToString() : "unauthenticated");
                    _auxMessage += string.Format("\nStackTrace: {0}", e.StackTrace);

                    // log it
                    StringWriter sw = new StringWriter();
                    PublishToLog(_logLevel, _auxMessage, e, sw);

                    // bundle the info
                    ArrayList storedError = new ArrayList(3);
                    storedError.Add(_logLevel);
                    storedError.Add(_myGuid);
                    storedError.Add(sw);
                    // cache it
                    sb = new StringBuilder(Portal.UniqueID);
                    sb.Append("_rb_error_");
                    sb.Append(_myGuid);
                    myCacheKey = sb.ToString();
                    CurrentCache.Insert(myCacheKey, storedError);
                }
                catch
                {
                    try
                    {
                        HttpContext.Current.Response.WriteFile(Config.CriticalErrorRedirect);
                        HttpContext.Current.Response.StatusCode = (int)Config.CriticalErrorResponse;
                    }
                    catch
                    {
                        HttpContext.Current.Response.Write("Sorry - a critical error has occurred - unable to continue");
                        HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    }
                }
                finally
                {
                    if (_redirectUrl.StartsWith("http://"))
                    {
                        HttpContext.Current.Response.Redirect(_redirectUrl, true);
                    }
                    else if (_redirectUrl.StartsWith("~/") && _redirectUrl.IndexOf(".aspx") > 0)
                    {
                        // append params to redirect url
                        if (!_redirectUrl.StartsWith(@"http://"))
                        {
                            sb = new StringBuilder();
                            if (_redirectUrl.IndexOf("?") != -1)
                            {
                                sb.Append(_redirectUrl.Substring(0, _redirectUrl.IndexOf("?") + 1));
                                sb.Append(((int)_httpStatusCode).ToString());
                                sb.Append("&eid=");
                                sb.Append(myCacheKey);
                                sb.Append("&");
                                sb.Append(_redirectUrl.Substring(_redirectUrl.IndexOf("?") + 1));
                                _redirectUrl = sb.ToString();
                            }
                            else
                            {
                                sb.Append(_redirectUrl);
                                sb.Append("?");
                                sb.Append(((int)_httpStatusCode).ToString());
                                sb.Append("&eid=");
                                sb.Append(myCacheKey);
                                _redirectUrl = sb.ToString();
                            }
                        }
                        HttpContext.Current.Response.Redirect(_redirectUrl, true);
                    }
                    else if (_redirectUrl.StartsWith("~/") && _redirectUrl.IndexOf(".htm") > 0)
                    {
                        HttpContext.Current.Response.WriteFile(_redirectUrl);
                        HttpContext.Current.Response.StatusCode = (int)_httpStatusCode;
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        HttpContext.Current.Response.End();
                    }
                    else
                    {
                        HttpContext.Current.Response.Write("Sorry - a critical error has occurred - unable to continue");
                        HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        HttpContext.Current.Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                Publish(LogLevel.Fatal, "Unexpected error in ErrorHandler", ex);
            }
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        [Obsolete("use one of the Publish() overloads")]
        public static void HandleException()
        {
            Exception e = HttpContext.Current.Server.GetLastError();

            if (e == null)
                return;

            e = e.GetBaseException();

            if (e != null)
                HandleException(e);
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="e">The e.</param>
        [Obsolete("use one of the Publish() overloads")]
        public static void HandleException(Exception e)
        {
            //InnerHandleException(FormatExceptionDescription(e), e);
            Publish(LogLevel.Error, e);
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="e">The e.</param>
        [Obsolete("use one of the Publish() overloads")]
        public static void HandleException(string message, Exception e)
        {
            //InnerHandleException(message + Environment.NewLine + FormatExceptionDescription(e), e);
            Publish(LogLevel.Error, message, e);
        }

        /// <summary>
        /// Publish an exception.
        /// </summary>
        /// <param name="logLevel">LogLevel enum</param>
        /// <param name="auxMessage">Text message to be shown in log entry</param>
        public static void Publish(LogLevel logLevel, string auxMessage)
        {
            PublishToLog(logLevel, auxMessage, null);
        }

        /// <summary>
        /// Publish an exception.
        /// </summary>
        /// <param name="logLevel">LogLevel enum</param>
        /// <param name="e">Exception object (can be null)</param>
        public static void Publish(LogLevel logLevel, Exception e)
        {
            PublishToLog(logLevel, string.Empty, e);
        }

        /// <summary>
        /// Publish an exception.
        /// </summary>
        /// <param name="logLevel">LogLevel enum</param>
        /// <param name="auxMessage">Text message to be shown in log entry</param>
        /// <param name="e">Exception object (can be null)</param>
        public static void Publish(LogLevel logLevel, string auxMessage, Exception e)
        {
            PublishToLog(logLevel, auxMessage, e);
        }

        /// <summary>
        /// Publishes the exception.
        /// </summary>
        /// <param name="_logLevel">Appleseed.Framework.Configuration.LogLevel enumerator</param>
        /// <param name="_auxMessage">Text message to be shown in log entry</param>
        /// <param name="e">Exception object (can be null)</param>
        private static void PublishToLog(LogLevel _logLevel, string _auxMessage, Exception e)
        {
            // log it
            LogHelper.Logger.Log(_logLevel, _auxMessage, e);
        }

        /// <summary>
        /// Publishes the exception.
        /// </summary>
        /// <param name="_logLevel">Appleseed.Framework.Configuration.LogLevel enumerator</param>
        /// <param name="_auxMessage">Text message to be shown in log entry</param>
        /// <param name="e">Exception object (can be null)</param>
        /// <param name="sw">A StringWriter object which will be filled with a formatted verion of the log entry</param>
        private static void PublishToLog(LogLevel _logLevel, string _auxMessage, Exception e, StringWriter sw)
        {
            // log it
            LogHelper.Logger.Log(_logLevel, _auxMessage, e, sw);
        }
    }
}