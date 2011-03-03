using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using Appleseed.Context;
using Appleseed.Framework;
using Appleseed.Framework.BLL.Utils;
using Appleseed.Framework.Exceptions;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Scheduler;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using History = Appleseed.Framework.History;
using Path = System.IO.Path;
using Reader = Appleseed.Context.Reader;
using System.Web.Profile;
using System.Web.Mvc;

using System.Web.Routing;

using System.Configuration;
using MvcContrib.Routing;
using MvcContrib.UI.InputBuilder;

namespace Appleseed
{


    public class Global : HttpApplication
    {


        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            if (!Request.Path.ToLower().Contains("images.ashx"))
            {
                AppleseedApplication_BeginRequest(sender, e);
            }
        }

        protected void AppleseedApplication_BeginRequest(Object sender, EventArgs e)
        {
            Reader contextReader = new Reader(new WebContextReader());
            HttpContext context = contextReader.Current;


            string currentURL = context.Request.Path.ToLower();


#if DEBUG
                        if (currentURL.Contains("trace.axd"))
                            return;
#endif

            context.Trace.Warn("Application_BeginRequest :: " + currentURL);
            if (Portal.PageID > 0)
            {
                string physicalPath = context.Server.MapPath(currentURL.Substring(currentURL.LastIndexOf("/") + 1));

                if (!File.Exists(physicalPath)) // Rewrites the path
                    context.RewritePath("~/default.aspx?" + context.Request.ServerVariables["QUERY_STRING"]);
            }
            else
            {
                string pname = currentURL.Substring(currentURL.LastIndexOf("/") + 1);

                // if the request was not caused by an MS Ajax Client script invoking a WS.
                if (!currentURL.ToLower().EndsWith(".asmx/js"))
                {
                    if (!String.IsNullOrEmpty(pname) && pname.Length > 5)
                    {
                        pname = pname.Substring(0, (pname.Length - 5));
                    }
                    if (Regex.IsMatch(pname, @"^\d+$"))
                        context.RewritePath("~/default.aspx?pageid=" + pname +
                                            context.Request.ServerVariables["QUERY_STRING"]);
                }
            }

            #region 1st Check
            // 1st Check: is it a dangerously malformed request?
            //Important patch http://support.microsoft.com/?kbid=887459
            if (context.Request.Path.IndexOf('\\') >= 0 ||
                Path.GetFullPath(context.Request.PhysicalPath) != context.Request.PhysicalPath)
                throw new AppleseedRedirect(LogLevel.Warn, HttpStatusCode.NotFound, "Malformed request", null);
            #endregion

            #region 2nd Check: is the AllPortals Lock switched on?

            // 2nd Check: is the AllPortals Lock switched on?
            // let the user through if client IP address is in LockExceptions list, otherwise throw...
            if (Config.LockAllPortals)
            {
                string _rawUrl = context.Request.RawUrl.ToLower(CultureInfo.InvariantCulture);
                string _lockRedirect = Config.LockRedirect;
                if (!_rawUrl.EndsWith(_lockRedirect))
                {
                    // construct IPList
                    string[] lockKeyHolders = Config.LockKeyHolders.Split(new char[] { ';' });
                    IPList ipList = new IPList();
                    foreach (string lockKeyHolder in lockKeyHolders)
                    {
                        if (lockKeyHolder.IndexOf("-") > -1)
                            ipList.AddRange(lockKeyHolder.Substring(0, lockKeyHolder.IndexOf("-")),
                                            lockKeyHolder.Substring(lockKeyHolder.IndexOf("-") + 1));
                        else
                            ipList.Add(lockKeyHolder);
                    }
                    // check if requestor's IP address is in allowed list
                    if (!ipList.CheckNumber(context.Request.UserHostAddress))
                        throw new PortalsLockedException();
                }
            }

            #endregion

            #region 3rd Check: is database/code version correct?

            // 3rd Check: is database/code version correct?
            Uri _requestUri = context.Request.Url;
            string _requestPath = _requestUri.AbsolutePath.ToLower(CultureInfo.InvariantCulture);
            string _databaseUpdateRedirect = Config.DatabaseUpdateRedirect;
            if (_databaseUpdateRedirect.StartsWith("~/"))
                _databaseUpdateRedirect = _databaseUpdateRedirect.TrimStart(new char[] { '~' });
            if (_requestPath.EndsWith(_databaseUpdateRedirect.ToLower(CultureInfo.InvariantCulture)))
                return; // this is DB Update page... so skip creation of PortalSettings

            string _installRedirect = Config.InstallerRedirect;
            if (_installRedirect.StartsWith("~/"))
                _installRedirect = _installRedirect.TrimStart(new char[] { '~', '/' });
            _installRedirect = _installRedirect.ToLower(CultureInfo.InvariantCulture);
            if (_requestPath.EndsWith(_installRedirect) ||
                _requestPath.Contains(_installRedirect.Split(new char[] { '/' })[0]))
                return; // this is Install page... so skip creation of PortalSettings

            string _smartErrorRedirect = Config.SmartErrorRedirect;
            if (_smartErrorRedirect.StartsWith("~/"))
                _smartErrorRedirect = _smartErrorRedirect.TrimStart(new char[] { '~' });
            if (_requestPath.EndsWith(_smartErrorRedirect.ToLower(CultureInfo.InvariantCulture)))
                return; // this is SmartError page... so skip creation of PortalSettings


            int versionDelta = Database.DatabaseVersion.CompareTo(Portal.CodeVersion);
            // if DB and code versions do not match


            if (versionDelta != 0)
            {
                // ...and this is not DB Update page
                string errorMessage = "Database version: " + Database.DatabaseVersion.ToString() + " Code version: " +
                                        Portal.CodeVersion.ToString();
                if (versionDelta < 0) // DB Version is behind Code Version
                {
                  
                    ErrorHandler.Publish(LogLevel.Warn, errorMessage);
                    Response.Redirect(Framework.Settings.Path.ApplicationRoot + _databaseUpdateRedirect, true);
                  
                }
                else // DB version is ahead of Code Version
                {
                    ErrorHandler.Publish(LogLevel.Warn, errorMessage);

                }
            }

            #endregion

            #region Comments
            // ************ 'calculate' response to this request ************
            //
            // Test 1 - try requested Alias and requested PageID
            // Test 2 - try requested Alias and PageID 0
            // Test 3 - try default Alias and requested PageID
            // Test 4 - try default Alias and PageID 0
            //
            // The UrlToleranceLevel determines how many times the test is allowed to fail before the request is considered
            // to be "an error" and is therefore redirected:
            //
            // UrlToleranceLevel 1 
            //		- requested Alias must be valid - if invalid, InvalidAliasRedirect page on default portal will be shown
            //		- if requested PageID is found, it is shown
            //		- if requested PageID is not found, InvalidPageIdRedirect page is shown
            // 
            // UrlToleranceLevel 2 
            //		- requested Alias must be valid - if invalid, InvalidAliasRedirect page on default portal will be shown
            //		- if requested PageID is found, it is shown
            //		- if requested PageID is not found, PageID 0 (Home page) is shown
            //
            // UrlToleranceLevel 3 - <<<<<< not working?
            //		- if requested Alias is invalid, default Alias will be used
            //		- if requested PageID is found, it is shown
            //		- if requested PageID is not found, InvalidPageIdRedirect page is shown
            // 
            // UrlToleranceLevel 4 - 
            //		- if requested Alias is invalid, default Alias will be used
            //		- if requested PageID is found, it is shown
            //		- if requested PageID is not found, PageID 0 (Home page) is shown

            #endregion

            PortalSettings portalSettings = null;

            int pageID = Portal.PageID; // Get PageID from QueryString
            string portalAlias = Portal.UniqueID; // Get requested alias from querystring, cookies or hostname
            string defaultAlias = Config.DefaultPortal; // get default portal from config

            // load arrays with values to test
            string[] testAlias = new string[4] { portalAlias, portalAlias, defaultAlias, defaultAlias };
            int[] testPageID = new int[4] { pageID, pageID, pageID, pageID };

            int testsAllowed = Config.UrlToleranceLevel;
            int testsToRun = testsAllowed > 2 ? 4 : 2;
            // if requested alias is default alias, limit UrlToleranceLevel to max value of 2 and limit tests to 2
            if (portalAlias == defaultAlias)
            {
                testsAllowed = testsAllowed % 2;
                testsToRun = 2;
            }

            int testsCounter = 1;
            while (testsCounter <= testsToRun)
            {
                //try with current values from arrays
                try
                {
                    portalSettings = new PortalSettings(testPageID[testsCounter - 1], testAlias[testsCounter - 1]);
                }
                catch (DatabaseUnreachableException dexc)
                {
                    //If no database, must update
                    ErrorHandler.Publish(LogLevel.Error, dexc);
                    Response.Redirect(Config.DatabaseUpdateRedirect);
                }

                // test returned result
                if (portalSettings.PortalAlias != null)
                    break; // successful hit
                else
                    testsCounter++; // increment the test counter and continue
            }

            if (portalSettings.PortalAlias == null)
            {
                // critical error - neither requested alias nor default alias could be found in DB
                throw new AppleseedRedirect(
                    Config.NoPortalErrorRedirect,
                    LogLevel.Fatal,
                    Config.NoPortalErrorResponse,
                    "Unable to load any portal - redirecting request to ErrorNoPortal page.",
                    null);
            }

            Membership.Provider.ApplicationName = portalSettings.PortalAlias;
            ProfileManager.Provider.ApplicationName = portalSettings.PortalAlias;
            Roles.ApplicationName = portalSettings.PortalAlias;

            if (testsCounter <= testsAllowed) // success
            {
                // Portal Settings has passed the test so add it to Context
                context.Items.Add("PortalSettings", portalSettings);
                context.Items.Add("PortalID", portalSettings.PortalID); // jes1111
            }
            else // need to redirect
            {
                if (portalSettings.PortalAlias != portalAlias) // we didn't get the portal we asked for
                {
                    throw new AppleseedRedirect(
                        Config.InvalidAliasRedirect,
                        LogLevel.Info,
                        HttpStatusCode.NotFound,
                        "Invalid Alias specified in request URL - redirecting (404) to InvalidAliasRedirect page.",
                        null);
                }

                if (portalSettings.ActivePage.PageID != pageID) // we didn't get the page we asked for
                {
                    throw new AppleseedRedirect(
                        Config.InvalidPageIdRedirect,
                        LogLevel.Info,
                        HttpStatusCode.NotFound,
                        "Invalid PageID specified in request URL - redirecting (404) to InvalidPageIdRedirect page.",
                        null);
                }
            }


            context.Response.Cookies["PortalAlias"].Path = "/";
            context.Response.Cookies["PortalAlias"].Value = portalSettings.PortalAlias;


            //Try to get alias from cookie to determine if alias has been changed
            bool refreshSite = false;
            if (context.Request.Cookies["PortalAlias"] != null &&
                context.Request.Cookies["PortalAlias"].Value.ToLower() != Portal.UniqueID)
                refreshSite = true; //Portal has changed since last page request
            // if switching portals then clean parameters [TipTopWeb]
            // Must be the last instruction in this method 

            // 5/7/2006 Ed Daniel
            // Added hack for Http 302 by extending condition below to check for more than 3 cookies
            if (refreshSite && context.Request.Cookies.Keys.Count > 3)
            {
                // Signout and force the browser to refresh only once to avoid any dead-lock
                if (context.Request.Cookies["refreshed"] == null
                    || (context.Request.Cookies["refreshed"] != null
                        && context.Response.Cookies["refreshed"].Value == "false"))
                {
                    string rawUrl = context.Request.RawUrl;

                    context.Response.Cookies["refreshed"].Value = "true";
                    context.Response.Cookies["refreshed"].Path = "/";
                    context.Response.Cookies["refreshed"].Expires = DateTime.Now.AddMinutes(1);

					ErrorHandler.Publish(
                        LogLevel.Warn,
                        string.Format("Deslogueo al usuario en el global asax line 306. Valores -> refreshsite:{0}, context.Request.Cookies.Keys.count: {1}, rawurl: {2}", refreshSite, context.Request.Cookies.Keys.Count, rawUrl));
                    // sign-out, if refreshed param on the command line we will not call it again
                    PortalSecurity.SignOut(rawUrl, false);
                }
            }

            // invalidate cookie, so the page can be refreshed when needed
            if (context.Request.Cookies["refreshed"] != null && context.Request.Cookies.Keys.Count > 3)
            {
                context.Response.Cookies["refreshed"].Path = "/";
                context.Response.Cookies["refreshed"].Value = "false";
                context.Response.Cookies["refreshed"].Expires = DateTime.Now.AddMinutes(1);
            }

            

            //This is done in order to allow the sitemap to reference a page that is outside this website.
            var targetPage = Request.Params["sitemapTargetPage"];
            if (!string.IsNullOrEmpty(targetPage))
            {
                int pageId;
                if (int.TryParse(targetPage, out pageId))
                {
                    var url = HttpUrlBuilder.BuildUrl(pageId);
                    Response.Redirect(url);
                }
            }
        }


        protected void Application_Error(object sender, EventArgs e)
        {
            ErrorHandler.ProcessUnhandledException();
        }

        protected void Application_Start()
        {
            HttpContext context = HttpContext.Current;

            // moved from PortalSettings
            FileVersionInfo f = FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(Appleseed.Framework.Settings.Portal)).Location);
            HttpContext.Current.Application.Lock();
            HttpContext.Current.Application["CodeVersion"] = f.FilePrivatePart;
            HttpContext.Current.Application.UnLock();

            ErrorHandler.Publish(LogLevel.Info, "Application Started: code version " + Portal.CodeVersion.ToString());

            if (Config.CheckForFilePermission)
            {
                try
                {
                    string myNewDir = Path.Combine(Framework.Settings.Path.ApplicationPhysicalPath, "_createnewdir");

                    if (!Directory.Exists(myNewDir))
                        Directory.CreateDirectory(myNewDir);

                    if (Directory.Exists(myNewDir))
                        Directory.Delete(myNewDir);
                }
                catch (Exception ex)
                {
                    throw new AppleseedException(LogLevel.Fatal, HttpStatusCode.ServiceUnavailable,
                                               "ASPNET Account does not have rights to the filesystem", ex); // Jes1111
                }
            }


            //Start scheduler
            if (Config.SchedulerEnable)
            {
                PortalSettings.Scheduler = CachedScheduler.GetScheduler(
                    context.Server.MapPath(Framework.Settings.Path.ApplicationRoot),
                    Config.SqlConnectionString,
                    Config.SchedulerPeriod,
                    Config.SchedulerCacheSize);
                PortalSettings.Scheduler.Start();
            }

            // Start proxy
            if (Config.UseProxyServerForServerWebRequests)
            {
                WebRequest.DefaultWebProxy = PortalSettings.GetProxy();
            }


            #region MVC


            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);

            InputBuilder.BootStrap();

            #endregion
        }

        public void Application_OnEnd()
        {
            ErrorHandler.Publish(LogLevel.Info, "Application Ended");
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*\.aspx(/.*)?" });
            routes.IgnoreRoute("{*allashx}", new { allashx = @".*\.ashx(/.*)?" });

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("");

            //routes.MapRoute(
            //    "Default",                                              // Route name
            //    "{controller}/{action}/{id}",                           // URL with parameters
            //    new { controller = "Home", action = "Index", id = "" } // Parameter defaults



            //);
        }
    }
}

