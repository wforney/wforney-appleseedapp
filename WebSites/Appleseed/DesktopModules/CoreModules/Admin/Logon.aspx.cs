using System;
using Appleseed.Framework;
using Appleseed.Framework.Security;
using Appleseed.Framework.Web.UI;

namespace Appleseed.Admin
{
    /// <summary>
    /// Single click logon, useful for email and newsletters
    /// </summary>
    public partial class LogonPage : Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            string _user = string.Empty;
            string _password = string.Empty;
            string _alias = string.Empty;
            int _pageId = 0;

            // Get Login User from querystring
            if (Request.Params["usr"] != null) {
                _user = Request.Params["usr"];
                // Get Login Password from querystring
                if (Request.Params["pwd"] != null) {
                    _password = Request.Params["pwd"];
                }

                // Get portalaias
                if (Request.Params["alias"] != null) {
                    _alias = HttpUrlBuilder.BuildUrl("~/Default.aspx", 0, string.Empty, Request.Params["alias"]);
                }
                if (Request.Params["pageId"] != null) {
                    try {
                        _pageId = int.Parse(Request.Params["pageId"].ToString());
                        _alias = HttpUrlBuilder.BuildUrl(_pageId);
                    } catch {
                        PortalSecurity.AccessDenied();
                    }
                }
                //try to validate logon
                if (PortalSecurity.SignOn(_user, _password, true, _alias) == null) {
                    // Login failed
                    PortalSecurity.AccessDenied();
                }

            } else {
                //if user has logged on
                if (Request.IsAuthenticated) {
                    // Redirect user back to the Portal Home Page
                    PortalSecurity.PortalHome();
                } else {
                    //User not provided, display logon
                    string controlStr = "~/DesktopModules/CoreModules/SignIn/Signin.ascx";
                    if (portalSettings.CustomSettings.ContainsKey("SITESETTINGS_LOGIN_TYPE")) {
                        controlStr = Convert.ToString(portalSettings.CustomSettings["SITESETTINGS_LOGIN_TYPE"]);
                    }
                    try {
                        signIn.Controls.Add(LoadControl(controlStr));
                    } catch (Exception exc) {
                        ErrorHandler.Publish(LogLevel.Error, exc);
                        signIn.Controls.Add(LoadControl("~/DesktopModules/CoreModules/SignIn/Signin.ascx"));
                    }

                }
            }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }
        #endregion
    }
}