using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Data;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Users.Data;
using Appleseed.Framework.Web.UI;
using Appleseed.Framework.Web.UI.WebControls;
using Label=Appleseed.Framework.Web.UI.WebControls.Label;
using LinkButton=Appleseed.Framework.Web.UI.WebControls.LinkButton;
using System.Web.Security;
using Appleseed.Framework.Providers.AppleseedMembershipProvider;
using Appleseed.Framework.Providers.Geographic;


namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// Placeable Registration (Full) module
    /// </summary>
    [History("jminond", "march 2005", "Changes for moving Tab to Page")]
    [
        History("john.mandia@whitelightsolutions.com", "2005/02/12",
            "Adding handling for unique constraint violation and showed friendlier message.")]
    [History("bill@improvdesign.com", "2004/10/23", "Added email to admin on registration")]
    [
        History("john.mandia@whitelightsolutions.com", "2003/10/31",
            "Fixed bug where old state list would remain even though new country with no states had been selected.")]
    [History("Mario Hartmann", "mario@hartmann.net", "1.2", "2003/10/08", "moved to seperate folder")]
    [History("Jes1111", "2003/03/10", "Modified from original to be fully placeable module")]
    [History("Jes1111", "2003/03/10", "Updated to use Globalized controls")]
    //TODO still needs Globalized field validators (Compare and Regex)
    public class RegisterFull : PortalModuleControl, IEditUserProfile
    {
        #region Form controls

        /// <summary>
        /// 
        /// </summary>
        protected Label PageTitleLabel;

        /// <summary>
        /// 
        /// </summary>
        protected Label NameLabel;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox NameField;

        /// <summary>
        /// 
        /// </summary>
        protected RequiredFieldValidator RequiredName;

        /// <summary>
        /// 
        /// </summary>
        protected Label CompanyLabel;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox CompanyField;

        /// <summary>
        /// 
        /// </summary>
        protected Label AddressLabel;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox AddressField;

        /// <summary>
        /// 
        /// </summary>
        protected Label CityLabel;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox CityField;

        /// <summary>
        /// 
        /// </summary>
        protected Label ZipLabel;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox ZipField;

        /// <summary>
        /// 
        /// </summary>
        protected Label CountryLabel;

        /// <summary>
        /// 
        /// </summary>
        protected DropDownList CountryField;

        /// <summary>
        /// 
        /// </summary>
        protected Label StateLabel;

        /// <summary>
        /// 
        /// </summary>
        protected DropDownList StateField;

        /// <summary>
        /// 
        /// </summary>
        protected Label InLabel;

        /// <summary>
        /// 
        /// </summary>
        protected Label ThisCountryLabel;

        /// <summary>
        /// 
        /// </summary>
        protected Label PhoneLabel;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox PhoneField;

        /// <summary>
        /// 
        /// </summary>
        protected Label FaxLabel;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox FaxField;

        /// <summary>
        /// 
        /// </summary>
        protected Label SendNewsletterLabel;

        /// <summary>
        /// 
        /// </summary>
        protected CheckBox SendNewsletter;

        /// <summary>
        /// 
        /// </summary>
        protected Label AccountLabel;

        /// <summary>
        /// 
        /// </summary>
        protected Label EmailLabel;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox EmailField;

        /// <summary>
        /// 
        /// </summary>
        protected RegularExpressionValidator ValidEmail;

        /// <summary>
        /// 
        /// </summary>
        protected RequiredFieldValidator RequiredEmail;

        /// <summary>
        /// 
        /// </summary>
        protected Label PasswordLabel;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox PasswordField;

        /// <summary>
        /// 
        /// </summary>
        protected RequiredFieldValidator RequiredPassword;

        /// <summary>
        /// 
        /// </summary>
        protected Label ConfirmPasswordLabel;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox ConfirmPasswordField;

        /// <summary>
        /// 
        /// </summary>
        protected RequiredFieldValidator RequiredConfirm;

        /// <summary>
        /// 
        /// </summary>
        protected Label ConditionsLabel;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox FieldConditions;

        /// <summary>
        /// 
        /// </summary>
        protected CheckBox Accept;

        /// <summary>
        /// 
        /// </summary>
        protected CustomValidator CheckTermsValidator;

        /// <summary>
        /// 
        /// </summary>
        protected LinkButton RegisterBtn;

        /// <summary>
        /// 
        /// </summary>
        protected LinkButton cancelButton;

        /// <summary>
        /// 
        /// </summary>
        protected Label Message;

        /// <summary>
        /// 
        /// </summary>
        protected HtmlTableRow StateRow;

        /// <summary>
        /// 
        /// </summary>
        protected Label Label1;

        /// <summary>
        /// 
        /// </summary>
        protected HtmlTableRow EditPasswordRow;

        /// <summary>
        /// 
        /// </summary>
        protected HtmlTableRow ConditionsRow;

        /// <summary>
        /// 
        /// </summary>
        protected LinkButton SaveChangesBtn;

        /// <summary>
        /// 
        /// </summary>
        protected CompareValidator ComparePasswords;

        /// <summary>
        /// 
        /// </summary>
        protected CompareValidator CheckID;

        /// <summary>
        /// 
        /// </summary>
        protected Panel FullProfileInformation;

        #endregion

        #region Private Fields

        private string _redirectPage;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool EditMode
        {
            get { return (userName.Length != 0); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string RedirectPage
        {
            get
            {
                if (_redirectPage == null)
                {
                    // changed by Mario Endara <mario@softworks.com.uy> (2004/11/05)
                    // it's necessary the ModuleID in the URL to apply security checking in the target
                    return string.Format("{0}?TabID={1}&mID={2}&username={3}", Request.Url.Segments[Request.Url.Segments.Length - 1], PageID, ModuleID, EmailField.Text);
                }
                return _redirectPage;
            }
            set { _redirectPage = value; }
        }

        private string userName
        {
            get
            {
                string uid = string.Empty;

                if (Request.Params["userName"] != null)
                    uid = Request.Params["userName"];

                if (uid.Length == 0 && HttpContext.Current.Items["userName"] != null)
                    uid = HttpContext.Current.Items["userName"].ToString();

#if DEBUG
                // TODO: Remove this.
                if (uid.Length == 0)
                    HttpContext.Current.Response.Write("username is empty");
#endif

                return uid;
            }
        }

        private Guid originalUserID
        {
            get
            {
                if (ViewState["originalUserID"] != null)
                    return (Guid)ViewState["originalUserID"];
                else
                    return Guid.Empty;
            }
            set { ViewState["originalUserID"] = value; }
        }

        private string originalPassword
        {
            get
            {
                if (ViewState["originalPassword"] != null)
                    return (string)ViewState["originalPassword"];
                else
                    return string.Empty;
            }
            set { ViewState["originalPassword"] = value; }
        }

        private bool selfEdit
        {
            get
            {
                if (ViewState["selfEdit"] != null)
                    return (bool)ViewState["selfEdit"];
                else
                    return false;
            }
            set { ViewState["selfEdit"] = value; }
        }

        #endregion

        #region	Methods

        private void BindCountry()
        {
            CountryField.DataSource = GeographicProvider.Current.GetCountries(CountryFields.Name);
            CountryField.DataBind();
        }

        private void BindState()
        {
            StateRow.Visible = false;
            if (CountryField.SelectedItem != null)
            {

                Country selectedCountry = GeographicProvider.Current.GetCountry(CountryField.SelectedValue);

                //added next line to clear the list. 
                //The stateField seems to remember it's values even when you set the 
                //DataSource to null
                //Michel Barneveld Appleseed@MichelBarneveld.Com
                StateField.Items.Clear();
                StateField.DataSource = GeographicProvider.Current.GetCountryStates(selectedCountry.CountryID);
                StateField.DataBind();

                StateLabel.Text = selectedCountry.AdministrativeDivisionName;

                if (StateField.Items.Count > 0)
                {
                    StateRow.Visible = true;
                    ThisCountryLabel.Text = CountryField.SelectedItem.Text;
                }
                else
                {
                    StateRow.Visible = false;
                }


            }
        }

        private PortalSettings CurrentPortalSettings
        {
            get
            {
                return (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            }
        }

        /// <summary>
        /// Save user data
        /// </summary>
        /// <returns></returns>
        public Guid SaveUserData()
        {
            Guid returnID = Guid.Empty;

            if (PasswordField.Text.Length > 0 || ConfirmPasswordField.Text.Length > 0)
            {
                if (PasswordField.Text != ConfirmPasswordField.Text)
                    ComparePasswords.IsValid = false;
            }

            // Only attempt a login if all form fields on the page are valid
            if (Page.IsValid)
            {
                UsersDB accountSystem = new UsersDB();

                string CountryID = string.Empty;
                if (CountryField.SelectedItem != null)
                    CountryID = CountryField.SelectedItem.Value;

                int StateID = 0;
                if (StateField.SelectedItem != null)
                    StateID = Convert.ToInt32(StateField.SelectedItem.Value);

                try
                {
                    if (userName == string.Empty)
                    {
                        // Add New User to Portal User Database
                        returnID =
                            accountSystem.AddUser(NameField.Text, CompanyField.Text,
                                                  AddressField.Text, CityField.Text, ZipField.Text, CountryID, StateID,
                                                  PhoneField.Text, FaxField.Text,
                                                  PasswordField.Text, EmailField.Text, SendNewsletter.Checked, CurrentPortalSettings.PortalAlias);
                    }
                    else
                    {
                        // Update user
                        if (PasswordField.Text.Equals(ConfirmPasswordField.Text) && PasswordField.Text.Equals(string.Empty))
                        {

                            accountSystem.UpdateUser(originalUserID, NameField.Text, CompanyField.Text, AddressField.Text,
                                CityField.Text, ZipField.Text, CountryID, StateID, PhoneField.Text,
                                FaxField.Text, EmailField.Text, SendNewsletter.Checked);
                        }
                        else
                        {
                            accountSystem.UpdateUser(originalUserID, NameField.Text, CompanyField.Text, AddressField.Text,
                                CityField.Text, ZipField.Text, CountryID, StateID, PhoneField.Text, FaxField.Text,
                                PasswordField.Text, EmailField.Text, SendNewsletter.Checked, portalSettings.PortalAlias);
                        }
                        //If we are here no error occurred
                    }
                }
                catch (Exception ex)
                {
                    Message.Text = General.GetString("REGISTRATION_FAILED", "Registration failed", Message) + " - ";

                    if (ex is SqlException)
                    {
                        if ((((SqlException)ex).Number == 2627))
                        {
                            Message.Text =
                                General.GetString("REGISTRATION_FAILED_EXISTING_EMAIL_ADDRESS",
                                                  "Registration has failed. This email address has already been registered. Please use a different email address or use the 'Send Password' button on the login page.",
                                                  Message);
                        }
                    }

                    ErrorHandler.Publish(LogLevel.Error, "Error registering user", ex);
                }
            }
            return returnID;
        }


        /// <summary>
        /// Sends registration information to portal administrator.
        /// </summary>
        public void SendRegistrationNoticeToAdmin()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("New User Registration\n");
            sb.Append("---------------------\n");
            sb.Append("PORTAL         : " + portalSettings.PortalTitle + "\n");
            sb.Append("Name           : " + NameField.Text + "\n");
            sb.Append("Company        : " + CompanyField.Text + "\n");
            sb.Append("Address        : " + AddressField.Text + "\n");
            sb.Append("                 " + CityField.Text + ", ");
            if (StateField.SelectedItem != null)
                sb.Append(StateField.SelectedItem.Text + "  ");
            sb.Append(ZipField.Text + "\n");
            sb.Append("                 " + CountryField.SelectedItem.Text + "\n");
            sb.Append("                 " + PhoneField.Text + "\n");
            sb.Append("Fax            : " + FaxField.Text + "\n");
            sb.Append("Email          : " + EmailField.Text + "\n");
            sb.Append("Send Newsletter: " + SendNewsletter.Checked.ToString() + "\n");

            MailHelper.SendMailNoAttachment(
                portalSettings.CustomSettings["SITESETTINGS_ON_REGISTER_SEND_TO"].ToString(),
                portalSettings.CustomSettings["SITESETTINGS_ON_REGISTER_SEND_TO"].ToString(),
                "New User Registration for " + portalSettings.PortalAlias,
                sb.ToString(),
                string.Empty,
                string.Empty,
                Config.SmtpServer);
        }

        #endregion

        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RegisterBtn_Click(object sender, EventArgs e)
        {
            Guid returnID = SaveUserData();

            if (returnID != Guid.Empty)
            {
                if (portalSettings.CustomSettings["SITESETTINGS_ON_REGISTER_SEND_TO"].ToString().Length > 0)
                    SendRegistrationNoticeToAdmin();
                //Full signon
                PortalSecurity.SignOn(EmailField.Text, PasswordField.Text, false, RedirectPage);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveChangesBtn_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                Guid returnID = SaveUserData();

                if (returnID == Guid.Empty)
                {
                    if (selfEdit)
                    {
                        //All should be ok now
                        //Try logoff user
                        PortalSecurity.SignOut(string.Empty, true);

                        //Logon user again with new settings
                        string actualPassword;
                        if (PasswordField.Text.Length != 0)
                            actualPassword = PasswordField.Text;
                        else
                            actualPassword = originalPassword;

                        //Full signon
                        PortalSecurity.SignOn(EmailField.Text, actualPassword, false, RedirectPage);
                    }
                    else if (RedirectPage == string.Empty)
                    {
                        // Redirect browser back to home page
                        PortalSecurity.PortalHome();
                    }
                    else
                    {
                        Response.Redirect(RedirectPage);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                //Remove validation for Windows users
                if (HttpContext.Current != null && Context.User is WindowsPrincipal)
                {
                    ValidEmail.Visible = false;
                    EmailLabel.TextKey = "WINDOWS_USER_NAME";
                    EmailLabel.Text = "Windows User Name";
                }

                // TODO: Jonathan - need to bring in country functionality from esperantus or new somehow?
                BindCountry();

                // TODO: Fix this
                // More esperanuts country stuff...
                // CountryInfo country = CountryInfo.CurrentCountry;
                //if (country != null && CountryField.Items.FindByValue(country.Name) != null)
                //	CountryField.Items.FindByValue(country.Name).Selected = true;
                BindState();


                // Edit check
                if (EditMode) // Someone requested edit this record
                {
                    //True is use is editing himself, false if is edited by an admin
                    selfEdit = (userName == PortalSettings.CurrentUser.Identity.UserName);

                    // Removed by Mario Endara <mario@softworks.com.uy> (2004/11/04)
                    //					if (PortalSecurity.IsInRoles("Admins") || selfEdit)
                    if (PortalSecurity.HasEditPermissions(ModuleID) || PortalSecurity.HasAddPermissions(ModuleID) || selfEdit)
                    {
                        //We can edit

                        // Hide
                        RequiredPassword.Visible = false;
                        RequiredConfirm.Visible = false;
                        EditPasswordRow.Visible = true;
                        SaveChangesBtn.Visible = true;
                        RegisterBtn.Visible = false;

                        // Obtain a single row of event information
                        UsersDB accountSystem = new UsersDB();

                        AppleseedUser memberUser = accountSystem.GetSingleUser(userName, portalSettings.PortalAlias);

                        try
                        {
                            NameField.Text = memberUser.Name;
                            EmailField.Text = memberUser.Email;
                            CompanyField.Text = memberUser.Company;
                            AddressField.Text = memberUser.Address;
                            ZipField.Text = memberUser.Zip;
                            CityField.Text = memberUser.City;

                            CountryField.ClearSelection();
                            if (CountryField.Items.FindByValue(memberUser.CountryID) != null)
                                CountryField.Items.FindByValue(memberUser.CountryID).Selected = true;
                            BindState();
                            StateField.ClearSelection();
                            if (StateField.Items.Count > 0 &&
                                StateField.Items.FindByValue(memberUser.StateID.ToString()) != null)
                                StateField.Items.FindByValue(memberUser.StateID.ToString()).Selected = true;

                            FaxField.Text = memberUser.Fax;
                            PhoneField.Text = memberUser.Phone;
                            SendNewsletter.Checked = memberUser.SendNewsletter;

                            //stores original password for later check
                            originalPassword = memberUser.GetPassword();
                            originalUserID = memberUser.ProviderUserKey;
                        }
                        catch (System.ArgumentNullException error)
                        {
                            // user doesn't exist
                        }
                    }
                    else
                    {
                        //We do not have rights to do it!
                        PortalSecurity.AccessDeniedEdit();
                    }
                }
                else
                {
                    BindState();

                    //No edit
                    RequiredPassword.Visible = true;
                    RequiredConfirm.Visible = true;
                    EditPasswordRow.Visible = false;
                    SaveChangesBtn.Visible = false;
                    RegisterBtn.Visible = true;
                }

                string termsOfService = portalSettings.GetTermsOfService;

                //Verify if we have to show conditions
                if (termsOfService.Length != 0)
                {
                    //Shows conditions
                    FieldConditions.Text = termsOfService;
                    ConditionsRow.Visible = true;
                }
                else
                {
                    //Hides conditions
                    ConditionsRow.Visible = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CountryField_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cancelButton_Click(object sender, EventArgs e)
        {
            ((Page)Page).RedirectBackToReferringPage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void CheckTermsValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = Accept.Checked;
        }

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CountryField.SelectedIndexChanged += new EventHandler(this.CountryField_SelectedIndexChanged);
            this.CheckTermsValidator.ServerValidate +=
                new ServerValidateEventHandler(this.CheckTermsValidator_ServerValidate);
            this.RegisterBtn.Click += new EventHandler(this.RegisterBtn_Click);
            this.SaveChangesBtn.Click += new EventHandler(this.SaveChangesBtn_Click);
            this.cancelButton.Click += new EventHandler(this.cancelButton_Click);
            this.Load += new EventHandler(this.Page_Load);
        }

        #endregion

        public override Guid GuidID
        {
            get { return new Guid("{AE419DCC-B890-43ba-B77C-54955F182041}"); }
        }
    }
}