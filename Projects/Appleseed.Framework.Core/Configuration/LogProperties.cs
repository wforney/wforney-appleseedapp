using System.Web;
using Appleseed.Framework.Settings;

namespace Appleseed.Framework.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class LogCodeVersionProperty
    {
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            try
            {
                return Portal.CodeVersion.ToString();
            }
            catch
            {
                return "not available";
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LogUserNameProperty
    {
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            try
            {
                return HttpContext.Current.User.Identity.Name;
            }
            catch
            {
                return "not available";
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LogRewrittenUrlProperty
    {
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            try
            {
                return HttpContext.Current.Server.HtmlDecode(HttpContext.Current.Request.Url.ToString());
            }
            catch
            {
                return "not available";
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LogUserAgentProperty
    {
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            try
            {
                return HttpContext.Current.Request.UserAgent;
            }
            catch
            {
                return "not available";
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LogUserLanguagesProperty
    {
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            try
            {
                return string.Join(";", HttpContext.Current.Request.UserLanguages);
            }
            catch
            {
                return "not available";
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LogUserIpProperty
    {
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            try
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            catch
            {
                return "not available";
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PortalAliasProperty
    {
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            try
            {
                return Portal.UniqueID;
            }
            catch
            {
                return "not available";
            }
        }
    }
}