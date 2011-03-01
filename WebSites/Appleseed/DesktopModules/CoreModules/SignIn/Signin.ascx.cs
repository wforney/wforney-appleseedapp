using System;
using System.Data.SqlClient;
using System.Text;
using Appleseed.Framework;
using Appleseed.Framework.Content.Security;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Users.Data;
using Appleseed.Framework.Web.UI.WebControls;

using System.Web.Security;
using System.Web.Profile;
using System.Data;
using System.Net.Mail;
using Appleseed.Framework.Providers.AppleseedMembershipProvider;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// The SignIn User Control enables clients to authenticate themselves using 
    /// the ASP.NET Forms based authentication system.
    ///
    /// When a client enters their username/password within the appropriate
    /// textboxes and clicks the "Login" button, the LoginBtn_Click event
    /// handler executes on the server and attempts to validate their
    /// credentials against a SQL database.
    ///
    /// If the password check succeeds, then the LoginBtn_Click event handler
    /// sets the customers username in an encrypted cookieID and redirects
    /// back to the portal home page.
    /// 
    /// If the password check fails, then an appropriate error message
    /// is displayed.
    /// </summary>
    public partial class Signin : PortalModuleControl
    {
        protected Localize LoginTitle;

        /// <summary>
        /// Handles the Click event of the LoginBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LoginBtn_Click(object sender, EventArgs e)
        {
            Session["PersistedUser"] = RememberCheckBox.Checked;
            if (PortalSecurity.SignOn(email.Text.Trim(), password.Text, RememberCheckBox.Checked) == null)
            {
                Message.Text = "Login failed";
                Message.TextKey = "LOGIN_FAILED";
            }
        }

        /// <summary>
        /// Handles the Click event of the RegisterBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RegisterBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect(HttpUrlBuilder.BuildUrl("~/DesktopModules/CoreModules/Register/Register.aspx"));
        }


        /// <summary>
        /// Handles the Click event of the SendPasswordBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SendPasswordBtn_Click(object sender, EventArgs e)
        {
            if (email.Text == string.Empty)
            {
                Message.Text = "Please enter you email address";
                Message.TextKey = "SIGNIN_ENTER_EMAIL_ADDR";
                return;
            }

            Membership.ApplicationName = portalSettings.PortalAlias;
            var membership = (AppleseedMembershipProvider)Membership.Provider;

            //Obtain single row of User information
            var memberUser = membership.GetUser(email.Text, false);
            
            if (memberUser == null)
            {
                Message.Text =
                    General.GetString("SIGNIN_PWD_MISSING_IN_DB",
                                      "The email you specified does not exists on our database", this);
                Message.TextKey = "SIGNIN_PWD_MISSING_IN_DB";
                return;
            }

            var userId = (Guid)memberUser.ProviderUserKey;
            // generate Token for user
            var token = membership.CreateResetPasswordToken(userId);

            string changePasswordUrl = string.Concat(
                Path.ApplicationFullPath,
                "DesktopModules/CoreModules/Admin/ChangePassword.aspx?usr=",
                userId.ToString("N"),
                "&tok=",
                token.ToString("N"));

            MailMessage mail = new MailMessage();
                
            //we check the PortalSettings in order to get if it has an sender registered 
            if (portalSettings.CustomSettings["SITESETTINGS_ON_REGISTER_SEND_FROM"] != null)
            {
                var sf = portalSettings.CustomSettings["SITESETTINGS_ON_REGISTER_SEND_FROM"];
                var mailFrom =  sf is SettingItem ? ((SettingItem)sf).Value : (string)sf;
                try
                {
                    mail.From = new MailAddress(mailFrom);
                }
                catch //if the address is not well formed, a warning is logged.
                {
                    LogHelper.Logger.Log(LogLevel.Warn, string.Format(
                        @"This is the current email address used as sender when someone want to retrieve his/her password: '{0}'. 
Is not well formed. Check the setting SITESETTINGS_ON_REGISTER_SEND_FROM of portal '{1}' in order to change this value (it's a portal setting).",
                        mailFrom, portalSettings.PortalAlias
                    ));
                }
            }
            //if there is not a correct email in the portalSettings, we use the default sender specified on the web.config file in the mailSettings tag.
                
            mail.To.Add(new MailAddress(email.Text));
            mail.Subject = portalSettings.PortalName + " - " + General.GetString("SIGNIN_PWD_LOST", "I lost my password", this);

            StringBuilder sb = new StringBuilder();

            sb.Append(memberUser.UserName);
            sb.Append(",");
            sb.Append("\r\n\r\n");
            sb.Append(General.GetString("SIGNIN_PWD_LOST_REQUEST_RECEIVED", "We received your request regarding the loss of your password.", this));
            sb.Append("\r\n");
            sb.Append(General.GetString("SIGNIN_SET_NEW_PWD_MSG", "You can set a new password for your account going to the following link:", this));
            sb.Append(" ");
            sb.Append(changePasswordUrl);
            sb.Append("\r\n\r\n");
            sb.Append(General.GetString("SIGNIN_THANK_YOU", "Thanks for your visit.", this));
            sb.Append(" ");
            sb.Append(portalSettings.PortalName);
            sb.Append("\r\n\r\n");
            sb.Append(
                General.GetString("SIGNIN_URL_WARNING",
                                    "NOTE: The address above may not show up on your screen as one line. This would prevent you from using the link to access the web page. If this happens, just use the 'cut' and 'paste' options to join the pieces of the URL.",
                                    this));

            mail.Body = sb.ToString();
            mail.IsBodyHtml = false;

            using (SmtpClient client = new SmtpClient())
            {
                try
                {
                    client.Send(mail);

                    Message.Text =
                        General.GetString("SIGNIN_PWD_WAS_SENT", "Your password was sent to the addess you provided",
                                            this);
                    Message.TextKey = "SIGNIN_PWD_WAS_SENT";
                }
                catch (Exception exception)
                {
                    Message.Text = General.GetString("SIGNIN_SMTP_SENDING_PWD_MAIL_ERROR", "We can't send you your password. There were problems while trying to do so.");
                    Message.TextKey = "SIGNIN_SMTP_SENDING_PWD_MAIL_ERROR";
                    LogHelper.Logger.Log(
                        LogLevel.Error, 
                        string.Format("Error while trying to send the password to '{0}'. Perhaps you should check your smtp server configuration in the web.config.", email.Text), 
                        exception);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Signin"/> class.
        /// </summary>
        public Signin()
        {
            SettingItem HideAutomatically = new SettingItem(new BooleanDataType());
            HideAutomatically.Value = "True";
            HideAutomatically.EnglishName = "Hide automatically";
            HideAutomatically.Order = 20;
            _baseSettings.Add("SIGNIN_AUTOMATICALLYHIDE", HideAutomatically);

            //1.2.8.1743b - 09/10/2003
            //New setting on Signin fo disable IE autocomplete by Mike Stone
            //If you uncheck this setting IE will not remember user name and passwords. 
            //Note that users who have memorized passwords will not be effected until their computer 
            //is reset, only new users and/or computers will honor this. 
            SettingItem AutoComplete = new SettingItem(new BooleanDataType());
            AutoComplete.Value = "True";
            AutoComplete.EnglishName = "Allow IE Autocomplete";
            AutoComplete.Description = "If Checked IE Will try to remember logins";
            AutoComplete.Order = 30;
            _baseSettings.Add("SIGNIN_ALLOW_AUTOCOMPLETE", AutoComplete);

            SettingItem RememberLogin = new SettingItem(new BooleanDataType());
            RememberLogin.Value = "True";
            RememberLogin.EnglishName = "Allow Remember Login";
            RememberLogin.Description = "If Checked allows to remember logins";
            RememberLogin.Order = 40;
            _baseSettings.Add("SIGNIN_ALLOW_REMEMBER_LOGIN", RememberLogin);

            SettingItem SendPassword = new SettingItem(new BooleanDataType());
            SendPassword.Value = "True";
            SendPassword.EnglishName = "Allow Send Password";
            SendPassword.Description = "If Checked allows user to ask to get password by email if he forgotten";
            SendPassword.Order = 50;
            _baseSettings.Add("SIGNIN_ALLOW_SEND_PASSWORD", SendPassword);
        }

        /// <summary>
        /// Overrides ModuleSetting to render this module type un-cacheable
        /// </summary>
        /// <value></value>
        public override bool Cacheable
        {
            get { return false; }
        }

        #region General Implementation

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{A0F1F62B-FDC7-4de5-BBAD-A5DAF31D960A}"); }
        }

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// On init
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            // use "View = Unauthenticated Users" instead
            //			//Hide control if not needed
            //			if (Request.IsAuthenticated)
            //				this.Visible = false;
            this.LoginBtn.Click += new EventHandler(this.LoginBtn_Click);
            this.SendPasswordBtn.Click += new EventHandler(this.SendPasswordBtn_Click);
            this.RegisterBtn.Click += new EventHandler(this.RegisterBtn_Click);
            this.Load += new EventHandler(this.Signin_Load);
            base.OnInit(e);
        }

        #endregion

        /// <summary>
        /// Handles the Load event of the Signin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Signin_Load(object sender, EventArgs e)
        {
            bool hide = true;
            bool autocomplete = false;
            if (this.ModuleID == 0)
                ((SettingItem)Settings["MODULESETTINGS_SHOW_TITLE"]).Value = "false";

            if (portalSettings.CustomSettings["SITESETTINGS_ALLOW_NEW_REGISTRATION"] != null)
                if (!bool.Parse(portalSettings.CustomSettings["SITESETTINGS_ALLOW_NEW_REGISTRATION"].ToString()))
                    RegisterBtn.Visible = false;

            if (Settings["SIGNIN_AUTOMATICALLYHIDE"] != null)
                hide = bool.Parse(Settings["SIGNIN_AUTOMATICALLYHIDE"].ToString());

            if (Settings["SIGNIN_ALLOW_AUTOCOMPLETE"] != null)
                autocomplete = bool.Parse(Settings["SIGNIN_ALLOW_AUTOCOMPLETE"].ToString());

            if (Settings["SIGNIN_ALLOW_REMEMBER_LOGIN"] != null)
                RememberCheckBox.Visible = bool.Parse(Settings["SIGNIN_ALLOW_REMEMBER_LOGIN"].ToString());

            if (Settings["SIGNIN_ALLOW_SEND_PASSWORD"] != null)
                SendPasswordBtn.Visible = bool.Parse(Settings["SIGNIN_ALLOW_SEND_PASSWORD"].ToString());

            if (hide && Request.IsAuthenticated)
            {
                this.Visible = false;
            }
            else if (!autocomplete)
            {
                //New setting on Signin fo disable IE autocomplete by Mike Stone
                password.Attributes.Add("autocomplete", "off");
            }
        }
    }
}