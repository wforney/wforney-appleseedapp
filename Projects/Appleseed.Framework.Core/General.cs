using System;
using System.Web;

namespace Appleseed.Framework
{
    /// <summary>
    /// Static helper methods for one line calls
    /// </summary>
    /// <remarks>
    /// <list type="string">
    /// <item>GetString</item>
    /// </list>
    /// </remarks>
    public static class General
    {
        #region Get Strings

        /// <summary>
        /// Get a resource string value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            return GetString(key, "");
        }

        /// <summary>
        /// Get a resource string value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetString(string key, string defaultValue, object o)
        {
            // TODO: What are objects passed arond for?
            return GetString(key, defaultValue);
        }

        /// <summary>
        /// Get a resource string value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetString(string key, string defaultValue)
        {
            if (HttpContext.Current == null)
            {
                Exception ne = new Exception("HttpContext.Current not an object");
                ErrorHandler.Publish(LogLevel.Warn, "Problem with Global Resources - could not get key: " + key, ne);
                return "<span class='error'>Could not get key: " + key + "</span>";
            }

            try
            {
                // TODO: Should we be using cached reourceset per language?
#if DEBUG
                HttpContext.Current.Trace.Warn("GetString(" + key + ")");
#endif
                // userCulture = Thread.CurrentThread.CurrentCulture.Name;

                object str = HttpContext.GetGlobalResourceObject("Appleseed", key);
                // string str = ((Appleseed.Framework.Web.UI.Page)System.Web.UI.Page).UserCultureSet.GetString(key);
                string ret = "";

                if (str != null)
                {
                    ret = str.ToString();
#if DEBUG
                    if (ret.Length > 0)
                        HttpContext.Current.Trace.Warn("We got localized  version");
                    else
                        HttpContext.Current.Trace.Warn("Localized return empty, use default");
#endif
                }

                if (ret.Length == 0)
                    return defaultValue;

                HttpContext.Current.Trace.Warn("GetString  = " + ret);
                return ret;
            }
            catch (Exception ex)
            {
                ErrorHandler.Publish(LogLevel.Warn, "Problem with Global Resources - could not get key: " + key, ex);
                return defaultValue;
            }
        }

        #endregion
    }
}