// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Signin.ascx.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   The SignIn User Control enables clients to authenticate themselves using
//   the ASP.NET Forms based authentication system.
//   When a client enters their username/password within the appropriate
//   textboxes and clicks the "Login" button, the LoginBtn_Click event
//   handler executes on the server and attempts to validate their
//   credentials against a SQL database.
//   If the password check succeeds, then the LoginBtn_Click event handler
//   sets the customers username in an encrypted cookieID and redirects
//   back to the portal home page.
//   If the password check fails, then an appropriate error message
//   is displayed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Content.Web.Modules
{
    using System;
    using System.Net.Mail;
    using System.Text;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using Appleseed.Framework;
    using Appleseed.Framework.Content.Security;
    using Appleseed.Framework.DataTypes;
    using Appleseed.Framework.Helpers;
    using Appleseed.Framework.Security;
    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Users.Data;
    using Appleseed.Framework.Web.UI.WebControls;

    using Resources;

    using Localize = Appleseed.Framework.Web.UI.WebControls.Localize;
    using MailMessage = System.Net.Mail.MailMessage;

    /// <summary>
    /// The SignIn User Control enables clients to authenticate themselves using 
    ///   the ASP.NET Forms based authentication system.
    ///   When a client enters their username/password within the appropriate
    ///   textboxes and clicks the "Login" button, the LoginBtn_Click event
    ///   handler executes on the server and attempts to validate their
    ///   credentials against a SQL database.
    ///   If the password check succeeds, then the LoginBtn_Click event handler
    ///   sets the customers username in an encrypted cookieID and redirects
    ///   back to the portal home page.
    ///   If the password check fails, then an appropriate error message
    ///   is displayed.
    /// </summary>
    public partial class Signin : PortalModuleControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The login title.
        /// </summary>
        protected Localize LoginTitle;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Signin" /> class.
        /// </summary>
        public Signin()
        {
            var hideAutomatically = new SettingItem<bool, CheckBox>(new BooleanDataType())
                {
                    Value = true,
                    EnglishName = "Hide automatically",
                    Order = 20
                };
            this._baseSettings.Add("SIGNIN_AUTOMATICALLYHIDE", hideAutomatically);

            // 1.2.8.1743b - 09/10/2003
            // New setting on Sign-in for disable IE auto-complete by Mike Stone
            // If you uncheck this setting IE will not remember user name and passwords. 
            // Note that users who have memorized passwords will not be effected until their computer 
            // is reset, only new users and/or computers will honor this. 
            var autoComplete = new SettingItem<bool, CheckBox>(new BooleanDataType())
                {
                    Value = true,
                    EnglishName = "Allow IE Auto-complete",
                    Description = "If Checked IE Will try to remember logins",
                    Order = 30
                };
            this._baseSettings.Add("SIGNIN_ALLOW_AUTOCOMPLETE", autoComplete);

            var rememberLogin = new SettingItem<bool, CheckBox>(new BooleanDataType())
                {
                    Value = true,
                    EnglishName = "Allow Remember Login",
                    Description = "If Checked allows to remember logins",
                    Order = 40
                };
            this._baseSettings.Add("SIGNIN_ALLOW_REMEMBER_LOGIN", rememberLogin);

            var sendPassword = new SettingItem<bool, CheckBox>(new BooleanDataType())
                {
                    Value = true,
                    EnglishName = "Allow Send Password",
                    Description = "If Checked allows user to ask to get password by email if he forgotten",
                    Order = 50
                };
            this._baseSettings.Add("SIGNIN_ALLOW_SEND_PASSWORD", sendPassword);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Overrides ModuleSetting to render this module type un-cacheable
        /// </summary>
        /// <value></value>
        public override bool Cacheable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get
            {
                return new Guid("{A0F1F62B-FDC7-4de5-BBAD-A5DAF31D960A}");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            // use "View = Unauthenticated Users" instead
            // //Hide control if not needed
            // if (Request.IsAuthenticated)
            // this.Visible = false;
            this.LoginBtn.Click += this.LoginBtnClick;
            this.SendPasswordBtn.Click += this.SendPasswordBtnClick;
            this.RegisterBtn.Click += this.RegisterBtnClick;

            base.OnInit(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var hide = true;
            var autocomplete = false;
            if (this.ModuleID == 0)
            {
                ((SettingItem<bool, CheckBox>)this.Settings["MODULESETTINGS_SHOW_TITLE"]).Value = false;
            }

            if (this.portalSettings.CustomSettings["SITESETTINGS_ALLOW_NEW_REGISTRATION"] != null)
            {
                if (!bool.Parse(this.portalSettings.CustomSettings["SITESETTINGS_ALLOW_NEW_REGISTRATION"].ToString()))
                {
                    this.RegisterBtn.Visible = false;
                }
            }

            if (this.Settings["SIGNIN_AUTOMATICALLYHIDE"] != null)
            {
                hide = bool.Parse(this.Settings["SIGNIN_AUTOMATICALLYHIDE"].ToString());
            }

            if (this.Settings["SIGNIN_ALLOW_AUTOCOMPLETE"] != null)
            {
                autocomplete = bool.Parse(this.Settings["SIGNIN_ALLOW_AUTOCOMPLETE"].ToString());
            }

            if (this.Settings["SIGNIN_ALLOW_REMEMBER_LOGIN"] != null)
            {
                this.RememberCheckBox.Visible = bool.Parse(this.Settings["SIGNIN_ALLOW_REMEMBER_LOGIN"].ToString());
            }

            if (this.Settings["SIGNIN_ALLOW_SEND_PASSWORD"] != null)
            {
                this.SendPasswordBtn.Visible = bool.Parse(this.Settings["SIGNIN_ALLOW_SEND_PASSWORD"].ToString());
            }

            if (hide && this.Request.IsAuthenticated)
            {
                this.Visible = false;
            }
            else if (!autocomplete)
            {
                // New setting on Sign-in for disable IE auto-complete by Mike Stone
                this.password.Attributes.Add("autocomplete", "off");
            }
        }

        /// <summary>
        /// Handles the Click event of the LoginBtn control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void LoginBtnClick(object sender, EventArgs e)
        {
            this.Session["PersistedUser"] = this.RememberCheckBox.Checked;
            if (PortalSecurity.SignOn(this.email.Text.Trim(), this.password.Text, this.RememberCheckBox.Checked) != null)
            {
                return;
            }

            this.Message.Text = Appleseed.Signin_LoginBtnClick_Login_failed;
            this.Message.TextKey = "LOGIN_FAILED";
        }

        /// <summary>
        /// Handles the Click event of the RegisterBtn control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void RegisterBtnClick(object sender, EventArgs e)
        {
            this.Response.Redirect(HttpUrlBuilder.BuildUrl("~/DesktopModules/CoreModules/Register/Register.aspx"));
        }

        /// <summary>
        /// Handles the Click event of the SendPasswordBtn control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void SendPasswordBtnClick(object sender, EventArgs e)
        {
            if (this.email.Text == string.Empty)
            {
                this.Message.Text = Appleseed.Signin_SendPasswordBtnClick_Please_enter_you_email_address;
                this.Message.TextKey = "SIGNIN_ENTER_EMAIL_ADDR";
                return;
            }

            // generate random password
            var randomPassword = RandomPassword.Generate(8, 10);

            var crypthelp = new CryptoHelper();
            var usersDb = new UsersDB();

            // Obtain single row of User information
            MembershipUser memberUser = usersDb.GetSingleUser(this.email.Text, this.portalSettings.PortalAlias);

            // ProfileCommon profile = usersDB.GetSingleUserProfile( email.Text, portalSettings.PortalID );
            // if (true)
            // {
            string pswrd;
            var appName = this.portalSettings.PortalName;
            var encrypted = Config.EncryptPassword;
            var name = memberUser.Email;
            if (encrypted)
            {
                pswrd = randomPassword;
                crypthelp.ResetPassword(name, randomPassword);
            }
            else
            {
                pswrd = memberUser.GetPassword();
            }

            var loginUrl = string.Format(
                "{0}DesktopModules/Admin/Logon.aspx?Usr={1}&Pwd={2}&Alias={3}",
                Path.ApplicationFullPath,
                name,
                pswrd,
                this.portalSettings.PortalAlias);
            var mail = new MailMessage();

            // Geert.Audenaert@Syntegra.Com
            // Date 19 March 2003
            // We have to use a correct sender address, 
            // because most SMTP servers reject it otherwise
            // jes1111 - mail.From = ConfigurationSettings.AppSettings["EmailFrom"].ToString();
            if (this.portalSettings.CustomSettings["SITESETTINGS_ON_REGISTER_SEND_FROM"] != null)
            {
                var sf = this.portalSettings.CustomSettings["SITESETTINGS_ON_REGISTER_SEND_FROM"];
                mail.From = new MailAddress(sf is SettingItem<string, TextBox> ? ((SettingItem<string, TextBox>)sf).Value : (string)sf);
            }
            else
            {
                mail.From = new MailAddress(Config.EmailFrom);
            }

            mail.To.Add(this.email.Text);
            mail.Subject = string.Format(
                "{0} - {1}", appName, General.GetString("SIGNIN_SEND_PWD", "Send me password", this));

            var sb = new StringBuilder();

            sb.Append(name);
            sb.Append(",");
            sb.Append("\r\n\r\n");
            sb.Append(General.GetString("SIGNIN_PWD_REQUESTED", "This is the password you requested", this));
            sb.Append(" ");
            sb.Append(pswrd);
            sb.Append("\r\n\r\n");
            sb.Append(General.GetString("SIGNIN_THANK_YOU", "Thanks for your visit.", this));
            sb.Append(" ");
            sb.Append(appName);
            sb.Append("\r\n\r\n");
            sb.Append(General.GetString("SIGNIN_YOU_CAN_LOGIN_FROM", "You can login from", this));
            sb.Append(":");
            sb.Append("\r\n");
            sb.Append(Path.ApplicationFullPath);
            sb.Append("\r\n\r\n");
            sb.Append(General.GetString("SIGNIN_USE_DIRECT_URL", "Or using direct url", this));
            sb.Append("\r\n");
            sb.Append(loginUrl);
            sb.Append("\r\n\r\n");
            sb.Append(
                General.GetString(
                    "SIGNIN_URL_WARNING",
                    "NOTE: The address above may not show up on your screen as one line. This would prevent you from using the link to access the web page. If this happens, just use the 'cut' and 'paste' options to join the pieces of the URL.",
                    this));

            mail.Body = sb.ToString();
            mail.IsBodyHtml = false;

            var smtpClient = new SmtpClient(Config.SmtpServer);
            smtpClient.SendAsync(mail, null);

            this.Message.Text = General.GetString(
                "SIGNIN_PWD_WAS_SENT", "Your password was sent to the address you provided", this);
            this.Message.TextKey = "SIGNIN_PWD_WAS_SENT";

            // }
            // else
            // {
            //     this.Message.Text = General.GetString(
            //         "SIGNIN_PWD_MISSING_IN_DB", "The email you specified does not exists on our database", this);
            //     this.Message.TextKey = "SIGNIN_PWD_MISSING_IN_DB";
            // }
        }

        #endregion
    }
}