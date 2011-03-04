// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterFull.ascx.cs" company="--">
//   Copyright © -- 2011. All Rights Reserved.
// </copyright>
// <summary>
//   The register full.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.DesktopModules.CoreModules.Register
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Web;
    using System.Web.Profile;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using Appleseed.Framework;
    using Appleseed.Framework.Providers.Geographic;
    using Appleseed.Framework.Security;
    using Appleseed.Framework.Web.UI.WebControls;

    using Resources;

    /// <summary>
    /// The register full.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public partial class RegisterFull : PortalModuleControl, IEditUserProfile
    {
        #region Constants and Fields

        /// <summary>
        ///   ddlDay control.
        /// </summary>
        protected DropDownList ddlDay;

        /// <summary>
        ///   ddlMonth control.
        /// </summary>
        protected DropDownList ddlMonth;

        /// <summary>
        ///   ddlYear control.
        /// </summary>
        protected DropDownList ddlYear;

        /// <summary>
        /// The default register date.
        /// </summary>
        private readonly DateTime defaultRegisterDate = new DateTime(DateTime.Today.Year, 1, 1);

        /// <summary>
        /// The redirect page.
        /// </summary>
        private string redirectPage;

        #endregion

        #region Properties

        /// <summary>
        /// Returns true when from control is on edit mode
        /// </summary>
        /// <remarks></remarks>
        public bool EditMode
        {
            get
            {
                return this.UserName.Length != 0;
            }
        }

        /// <summary>
        /// Stores the page where to redirect user on save
        /// </summary>
        /// <value>The redirect page.</value>
        /// <remarks></remarks>
        public string RedirectPage
        {
            get
            {
                if (this.redirectPage == null)
                {
                    return string.Format(
                        "{0}?TabID={1}&mID={2}&username={3}",
                        this.Request.Url.Segments[this.Request.Url.Segments.Length - 1],
                        this.PageID,
                        this.ModuleID,
                        this.tfEmail.Text);
                }

                return this.redirectPage;
            }

            set
            {
                this.redirectPage = value;
            }
        }

        /// <summary>
        ///   Gets or sets the birthday field.
        /// </summary>
        /// <value>The birthday field.</value>
        /// <remarks>
        /// </remarks>
        private DateTime BirthdayField
        {
            get
            {
                var day = Convert.ToInt32(this.ddlDay.SelectedValue);
                var month = Convert.ToInt32(this.ddlMonth.SelectedValue);
                var year = Convert.ToInt32(this.ddlYear.SelectedValue);

                // Si me ponen día 31 en un mes con 30 días, pongo 30.
                var daysInMonth = DateTime.DaysInMonth(year, month);
                if (day > daysInMonth)
                {
                    day = daysInMonth;
                }

                return new DateTime(year, month, day);
            }

            set
            {
                if (value > DateTime.MinValue)
                {
                    this.ddlDay.SelectedValue = value.Day.ToString();
                    this.ddlMonth.SelectedValue = value.Month.ToString();
                    this.ddlYear.SelectedValue = value.Year.ToString();
                }
            }
        }

        /// <summary>
        ///   Gets a value indicating whether [outer creation].
        /// </summary>
        /// <remarks>
        /// </remarks>
        private bool OuterCreation
        {
            get
            {
                return this.Request.Params["outer"] != null && (Convert.ToInt32(this.Request.Params["outer"]) == 1);
            }
        }

        /// <summary>
        ///   Gets the name of the user.
        /// </summary>
        /// <remarks>
        /// </remarks>
        private string UserName
        {
            get
            {
                var uid = string.Empty;

                if (this.Request.Params["userName"] != null)
                {
                    uid = this.Request.Params["userName"];
                }

                if (uid.Length == 0 && HttpContext.Current.Items["userName"] != null)
                {
                    uid = HttpContext.Current.Items["userName"].ToString();
                }

                return uid;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IEditUserProfile

        /// <summary>
        /// This procedure should persist the user data on db
        /// </summary>
        /// <returns>
        /// The user id
        /// </returns>
        /// <remarks>
        /// </remarks>
        public Guid SaveUserData()
        {
            if (!this.EditMode)
            {
                var result = Guid.Empty;

                var status = MembershipCreateStatus.Success;
                var user = Membership.Provider.CreateUser(
                    this.tfEmail.Text, 
                    this.tfPwd.Text, 
                    this.tfEmail.Text, 
                    "question", 
                    "answer", 
                    true, 
                    Guid.NewGuid(), 
                    out status);
                this.lblError.Text = string.Empty;

                switch (status)
                {
                    case MembershipCreateStatus.DuplicateEmail:
                    case MembershipCreateStatus.DuplicateUserName:
                        this.lblError.Text = Appleseed.USER_ALREADY_EXISTS;
                        break;
                    case MembershipCreateStatus.ProviderError:
                        break;
                    case MembershipCreateStatus.Success:
                        this.UpdateProfile();
                        result = (Guid)user.ProviderUserKey;

                        // if the user is registering himself (thus, is not yet authenticated) we will sign him on and send him to the home page.
                        if (!this.Context.User.Identity.IsAuthenticated)
                        {
                            PortalSecurity.SignOn(this.tfEmail.Text, this.tfPwd.Text, false, HttpUrlBuilder.BuildUrl());
                        }

                        break;

                        // for every other error message...
                    default:
                        this.lblError.Text = Appleseed.USER_SAVING_ERROR;
                        break;
                }

                return result;
            }
            
            this.UpdateProfile();
            return (Guid)Membership.GetUser(this.tfEmail.Text, false).ProviderUserKey;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        /// <remarks>
        /// </remarks>
        protected override void OnInit(EventArgs e)
        {
            this.recaptcha.PublicKey =
                Convert.ToString(this.PortalSettings.CustomSettings["SITESETTINGS_RECAPTCHA_PUBLIC_KEY"]);
            this.recaptcha.PrivateKey =
                Convert.ToString(this.PortalSettings.CustomSettings["SITESETTINGS_RECAPTCHA_PRIVATE_KEY"]);
            this.recaptcha.Language = this.PortalSettings.PortalContentLanguage.TwoLetterISOLanguageName;

            // captcha will only be displayed if the user is not authenticated.
            this.trCaptcha.Visible = !this.Context.User.Identity.IsAuthenticated;

            this.LoadBirthDateControls();
            this.trPwd.Visible = false;
            this.trPwdAgain.Visible = false;
            this.lnkChangePassword.Visible = false;
            this.panChangePwd.Visible = false;

            this.BindCountry();
            this.BindDateCombos();
            this.lblError.Text = string.Empty;
            this.lblSuceeded.Text = string.Empty;
            this.pnlSuceeded.Visible = false;
            this.pnlForm.Visible = true;

            this.btnSave.Visible = !this.Request.FilePath.Contains("UsersManage.aspx");

            if (this.Request.QueryString["email"] != null)
            {
                this.tfEmail.Text = this.Request.QueryString["email"];
            }

            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        /// <remarks>
        /// </remarks>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ViewState["responseWithPopup"] = null;

            if (!this.EditMode)
            {
                this.trPwd.Visible = true;
                this.trPwdAgain.Visible = true;
            }
            else
            {
                this.lnkChangePassword.Visible = true;
                this.panChangePwd.Visible = true;
            }

            if (this.Page.IsPostBack)
            {
                return;
            }

            if (this.EditMode && !this.OuterCreation)
            {
                this.lblTitle.Text = (string)this.GetGlobalResourceObject("Appleseed", "USER_MODIFICATION");
            }
            else
            {
                this.lblTitle.Text = (string)this.GetGlobalResourceObject("Appleseed", "USER_REGISTRY");

                if (this.OuterCreation)
                {
                    this.lblSendNotification.Visible = true;
                    this.chbSendNotification.Visible = true;
                    this.chbSendNotification.Checked = true;

                    // lblAssignCategory.Visible = true;
                    // ddlAssignCategory.Visible = true;
                    // BindCategory();
                }
            }

            if (this.EditMode)
            {
                // ProfileCommon profileCommon = Profile.GetProfile(UserName);
                // AppleseedProfileCommon profileCommon = (AppleseedProfileCommon)temp;
                var profileCommon = ProfileBase.Create(this.UserName);

                // profileCommon.Address;
                if ((DateTime)profileCommon.GetPropertyValue("BirthDate") == DateTime.MinValue)
                {
                    this.BirthdayField = this.defaultRegisterDate;
                }
                else
                {
                    this.BirthdayField = (DateTime)profileCommon.GetPropertyValue("BirthDate");
                }

                this.tfCompany.Text = (string)profileCommon.GetPropertyValue("Company");
                try
                {
                    this.ddlCountry.SelectedValue = (string)profileCommon.GetPropertyValue("CountryID");
                }
                catch (Exception exc)
                {
                    ErrorHandler.Publish(LogLevel.Error, Appleseed.PROFILE_COUNTRY_WRONG_ID, exc);
                }

                this.tfEmail.Text = profileCommon.UserName;
                this.tfEmail.Enabled = false;
                this.tfName.Text = (string)profileCommon.GetPropertyValue("Name");
                this.tfPhone.Text = (string)profileCommon.GetPropertyValue("Phone");
                this.chbReceiveNews.Checked = (bool)profileCommon.GetPropertyValue("SendNewsletter");
            }
            else
            {
                var firstOptionText = General.GetString("REGISTER_SELECT_COUNTRY", "Select Country", this);
                this.ddlCountry.Items.Insert(0, new ListItem(string.Concat("-- ", firstOptionText), string.Empty));
                this.BirthdayField = this.defaultRegisterDate;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnChangePwd control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        /// <remarks>
        /// </remarks>
        protected void btnChangePwd_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                try
                {
                    Membership.Provider.ChangePassword(this.tfEmail.Text, this.txtCurrentPwd.Text, this.txtNewPwd.Text);
                }
                catch (Exception ex)
                {
                    this.lblError.Text = ex.Message;
                }
            }
            else
            {
                this.ViewState["responseWithPopup"] = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        /// <remarks>
        /// </remarks>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                this.SaveUserData();
            }
        }

        /// <summary>
        /// Handles the ServerValidate event of the cvCaptcha control.
        /// </summary>
        /// <param name="source">
        /// The source of the event.
        /// </param>
        /// <param name="args">
        /// The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.
        /// </param>
        /// <remarks>
        /// </remarks>
        protected void cvCaptcha_ServerValidate(object source, ServerValidateEventArgs args)
        {
            this.recaptcha.Validate();
            args.IsValid = this.recaptcha.IsValid;
        }

        /// <summary>
        /// Handles the ServerValidate event of the cvCurrentPwdCorrect control.
        /// </summary>
        /// <param name="source">
        /// The source of the event.
        /// </param>
        /// <param name="args">
        /// The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.
        /// </param>
        /// <remarks>
        /// </remarks>
        protected void cvCurrentPwdCorrect_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = Membership.Provider.ValidateUser(this.tfEmail.Text, this.txtCurrentPwd.Text);
        }

        /// <summary>
        /// Binds the country.
        /// </summary>
        /// <remarks>
        /// </remarks>
        private void BindCountry()
        {
            var cs = GeographicProvider.Current.GetCountries(CountryFields.Name);
            this.ddlCountry.DataSource = cs;
            this.ddlCountry.DataBind();
        }

        /// <summary>
        /// Binds the date combos.
        /// </summary>
        /// <remarks>
        /// </remarks>
        private void BindDateCombos()
        {
            var days = new List<int>();
            for (var i = 1; i <= 31; i++)
            {
                days.Insert(i - 1, i);
            }

            this.ddlDay.DataSource = days;
            this.ddlDay.DataBind();

            var months = new Dictionary<int, string>();
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;
            for (var j = 1; j <= 12; j++)
            {
                if (DateTimeFormatInfo.CurrentInfo != null)
                {
                    months.Add(j, string.Format("{0} - {1}", j, textInfo.ToTitleCase(DateTimeFormatInfo.CurrentInfo.GetMonthName(j))));
                }
            }

            this.ddlMonth.DataSource = months;
            this.ddlMonth.DataValueField = "key";
            this.ddlMonth.DataTextField = "value";
            this.ddlMonth.DataBind();

            var years = new List<int>();
            for (var k = 1900; k <= DateTime.Today.Year; k++)
            {
                years.Insert(k - 1900, k);
            }

            this.ddlYear.DataSource = years;
            this.ddlYear.DataBind();
        }

        /// <summary>
        /// Loads the birth date controls.
        /// </summary>
        /// <remarks>
        /// </remarks>
        private void LoadBirthDateControls()
        {
            this.ddlDay = new DropDownList { ID = "ddlDay" };
            this.ddlMonth = new DropDownList { ID = "ddlMonth" };
            this.ddlYear = new DropDownList { ID = "ddlYear" };

            if (DateTimeFormatInfo.CurrentInfo != null)
            {
                var datePattern = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;

                if (datePattern.IndexOf("M") < datePattern.IndexOf("d"))
                {
                    this.plhBirthDate.Controls.Add(this.ddlMonth);
                    this.plhBirthDate.Controls.Add(this.ddlDay);
                }
                else
                {
                    this.plhBirthDate.Controls.Add(this.ddlDay);
                    this.plhBirthDate.Controls.Add(this.ddlMonth);
                }
            }

            this.plhBirthDate.Controls.Add(this.ddlYear);
        }

        // private void BindCategory()
        // {
        // using (DataAccessAdapter adapter = new DataAccessAdapter()) {

        // EntityCollection categories = new EntityCollection(new UserCategoryEntityFactory());
        // IRelationPredicateBucket bucket = new RelationPredicateBucket(UserCategoryFields.PortalId == PortalSettings.PortalID);
        // adapter.FetchEntityCollection(categories, bucket);

        // categories.AllowNew = true;
        // TravelAgency.AspNetMembership.EntityClasses.UserCategoryEntity allCategories = new TravelAgency.AspNetMembership.EntityClasses.UserCategoryEntity(-1);
        // allCategories.Name = "[ Ninguna ]";
        // categories.Insert(0, allCategories);

        // ddlAssignCategory.DataSource = categories;
        // ddlAssignCategory.DataTextField = "Name";
        // ddlAssignCategory.DataValueField = "Id";
        // ddlAssignCategory.DataBind();
        // }
        // }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="to">
        /// To.
        /// </param>
        /// <param name="from">
        /// From.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <remarks>
        /// </remarks>
        private void SendMail(string to, string from, string subject, string content)
        {
            // BulkMailService bMailService = new BulkMailService();
            // string[] toAddr = to.Split((';'));
            // string templateName="RegisterTemplate";
            // MailDTO mail = new MailDTO();
            // mail.ApplicationName = Membership.ApplicationName;
            // try {
            // if (BulkMailProviderManager.Provider.ExistsTemplate(templateName)) {
            // mail.Body = string.Empty;
            // } else {
            // mail.Body = content;
            // }
            // } catch (Exception ex) {
            // mail.Body = content;
            // } 

            // mail.CreatedBy = Membership.GetUser() != null ? Membership.GetUser().UserName : "anonymous";
            // mail.CreatedOn = DateTime.Now;
            // mail.Description = "Creación de Usuario";
            // mail.From = from;
            // mail.Priority = 0;
            // mail.Subject = subject;

            // List<MailParametersDTO> parameters = new List<MailParametersDTO>();
            // string path = Path.ApplicationFullPath;
            // if (Path.ApplicationRoot != null && Path.ApplicationRoot.Length > 0) {
            // path = path.Replace(Path.ApplicationRoot, string.Empty);
            // }

            // MailParametersDTO logo = new MailParametersDTO();
            // logo.Key = "ImgLogo";

            // IUserServices userServices = AbtourServicesFactory.Current.UserServices;
            // try {

            // Guid providerUserKey = PortalSettings.CurrentUser.Identity.ProviderUserKey;
            // Abtour.Providers.MembershipProvider.AbtourUser membershipUser = Membership.GetUser(providerUserKey) as Abtour.Providers.MembershipProvider.AbtourUser;
            // if (membershipUser != null) {
            // AbtourUserEntity abtourUser = userServices.LoadAbtourUser(membershipUser.Id, true);
            // // se esta asumiento que user siempre esta asociado a una agencia, por lo menos la agencia vacía.
            // if (!(abtourUser.Agency.Fields["LogoUrl"].IsNull || abtourUser.Agency.LogoUrl.Equals(string.Empty))) {
            // logo.Value = Path.ApplicationFullPath + "/" + abtourUser.Agency.LogoUrl;
            // }
            // MailParametersDTO bunner = new MailParametersDTO();
            // bunner.Key = "HTMLBanner";

            // if (abtourUser.Agency.Fields["HtmlBanner"].IsNull || abtourUser.Agency.HtmlBanner.Equals(string.Empty)) {
            // bunner.Value = string.Empty;
            // } else {
            // bunner.Value = "<span>" + abtourUser.Agency.HtmlBanner + "</span>";
            // }
            // parameters.Add(bunner);

            // } else {
            // logo.Value = path + this.PortalSettings.PortalFullPath + "/images/logo.gif";

            // MailParametersDTO cabezal = new MailParametersDTO();
            // cabezal.Key = "ImgCabezal";
            // cabezal.Value = path + this.PortalSettings.PortalFullPath + "/images/cabezal.jpg";
            // parameters.Add(cabezal);
            // }
            // } catch (Exception) {
            // Agency agt = AbtourServicesFactory.Current.AgencyServices.LoadAgency(0);
            // logo.Value = Path.ApplicationFullPath + "/" + agt.LogoUrl;
            // MailParametersDTO bunner = new MailParametersDTO();
            // bunner.Key = "HTMLBanner";
            // bunner.Value = "<span>" + agt.HtmlBanner + "</span>";
            // parameters.Add(bunner);
            // }

            // parameters.Add(logo);

            // MailParametersDTO websiteName = new MailParametersDTO();
            // websiteName.Key = "WebsiteName";
            // websiteName.Value = Path.ApplicationFullPath;
            // parameters.Add(websiteName);

            // mail.Template = templateName;

            // bMailService.SendMail(mail, to, templateName, true, parameters);
        }

        /// <summary>
        /// Sends the outer creation notification.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <remarks>
        /// </remarks>
        private void SendOuterCreationNotification(string email)
        {
            // TODO
            var msg = Appleseed.NEW_USER_CREATED + this.PortalSettings.PortalName;
            msg += string.Format("<br/>{0}: {1}", Appleseed.USER, email);
            var pwd = Membership.GetUser(email).GetPassword();
            pwd = String.IsNullOrEmpty(pwd) ? "<vacío>" : pwd;
            msg += string.Format("<br/>{0}: {1}", Appleseed.PASSWORD, pwd);
            var uName = this.UserName;
            if (string.IsNullOrEmpty(uName))
            {
                uName = Convert.ToString(this.PortalSettings.CustomSettings["SITESETTINGS_ON_REGISTER_SEND_FROM"]);
            }

            this.SendMail(email, uName, string.Format("Nuevo usuario en {0}!", this.PortalSettings.PortalName), msg);
        }

        /// <summary>
        /// Updates the profile.
        /// </summary>
        /// <remarks>
        /// </remarks>
        private void UpdateProfile()
        {
            var profile = ProfileBase.Create(this.tfEmail.Text);

            // Profile.GetProfile(tfEmail.Text);
            // profile.SetPropertyValue("Address", );
            profile.SetPropertyValue("BirthDate", this.BirthdayField);

            // if (EditMode && !OuterCreation) {
            // int catId = Convert.ToInt32(lblCategoryId.Text);
            // if (catId > 0) {
            // profile.SetPropertyValue("CategoryId", catId);
            // } else {
            // profile.SetPropertyValue("CategoryId", null);
            // }
            // } else {
            // if (OuterCreation) {
            // int categoryId = int.Parse(ddlAssignCategory.SelectedValue);
            // if (categoryId == -1) { //   [Ninguna] 
            // profile.SetPropertyValue("CategoryId", null);
            // } else {
            // profile.SetPropertyValue("CategoryId", categoryId);
            // }
            // }
            // }

            // profile.SetPropertyValue("City", );
            profile.SetPropertyValue("Company", this.tfCompany.Text);
            profile.SetPropertyValue("CountryID", this.ddlCountry.SelectedValue);
            profile.SetPropertyValue("Email", this.tfEmail.Text);

            // profile.SetPropertyValue("Fax", );
            profile.SetPropertyValue("Name", this.tfName.Text);
            profile.SetPropertyValue("Phone", this.tfPhone.Text);
            profile.SetPropertyValue("SendNewsletter", this.chbReceiveNews.Checked);

            // profile.SetPropertyValue("StateId", ); 
            // profile.SetPropertyValue("Zip", );
            try
            {
                profile.Save();
                this.pnlSuceeded.Visible = true;
                this.pnlForm.Visible = false;
                if (this.EditMode && !this.OuterCreation)
                {
                    this.lblSuceeded.Text = Appleseed.USER_UPDATED_SUCCESFULLY;
                }
                else
                {
                    if (this.OuterCreation)
                    {
                        this.lblSuceeded.Text = Appleseed.USER_UPDATED_SUCCESFULLY;
                        if (this.chbSendNotification.Checked)
                        {
                            this.SendOuterCreationNotification(this.tfEmail.Text);
                        }
                    }
                    else
                    {
                        this.lblSuceeded.Text = string.Format(
                            "{0}.{1}", Appleseed.USER_UPDATED_SUCCESFULLY, Appleseed.ENTER_THROUGH_HOMEPAGE);
                    }
                }
            }
            catch (Exception exc)
            {
                this.lblError.Text = Appleseed.USER_SAVING_ERROR;
                ErrorHandler.Publish(LogLevel.Error, "Error al salvar un perfil", exc);
            }
        }

        #endregion
    }
}