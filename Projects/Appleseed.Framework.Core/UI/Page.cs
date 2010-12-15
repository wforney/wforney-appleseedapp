using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Appleseed.Framework.Design;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Site.Data;
using Path = Appleseed.Framework.Settings.Path;
using Appleseed.Framework.Web.UI.WebControls;
using System.Web.Mvc;
using System.Configuration;

namespace Appleseed.Framework.Web.UI
{
    // TODO: this class needs a better write-up ;-)
    /// <summary>
    /// A template page useful for constructing custom edit pages for module settings.<br/>
    /// Encapsulates some common code including: moduleid,
    /// portalSettings and settings, referrer redirection, edit permission,
    /// cancel event, etc.
    /// This page is a base page.
    /// It is named USECURE becuse no check about security is made.
    /// Usencure page reminds you that you have to do your own security on it.
    /// </summary>
    [History("ozan@Appleseed.web.tr", "2005/06/01", "Added new overload for RegisterCSSFile")]
    [History("jminond", "2005/03/10", "Tab to page conversion")]
    [History("Jes1111", "2004/08/18", "Extensive changes - new way to build head element, support for multiple CSS stylesheets, etc.")]
    [History("jviladiu@portalServices.net", "2004/07/22", "Added Security Access.")]
    [History("John.Mandia@whitelightsolutions.com", "2003/10/11", "Added ability for each portal to have it's own custom icon instead of sharing one icon among many.")]
    [History("mario@hartmann.net", "2003/09/08", "Solpart Menu stylesheet support added.")]
    [History("Jes1111", "2003/03/04", "Smoothed out page event inheritance hierarchy - placed security checks and cache flushing")]
    public class Page : ViewPage
    {

        /// <summary>
        /// The default constructor initializes all fields to their default values.
        /// </summary>
        public Page()
            : base()
        {
            EnsureChildControls();

            //MVC

            HttpContextWrapper wrapper = new HttpContextWrapper(Context);

            ViewContext viewContext = new ViewContext();
            viewContext.HttpContext = wrapper;
            viewContext.ViewData = new ViewDataDictionary();


            //*/************************//
        }

        #region Standard Page Controls

        /// <summary>
        /// Standard update button
        /// </summary>
        protected LinkButton updateButton;

        /// <summary>
        /// Standard delete button
        /// </summary>
        protected LinkButton deleteButton;

        /// <summary>
        /// Standard cancel button
        /// </summary>
        protected LinkButton cancelButton;

        #endregion

        private ResourceSet userCultureSet = null;
        private string userCulture = "en-us";

        /// <summary>
        /// Gets the user culture.
        /// </summary>
        /// <value>The user culture.</value>
        public string UserCulture
        {
            get { return userCulture; }
        }

        /// <summary>
        /// Gets the user culture set.
        /// </summary>
        /// <value>The user culture set.</value>
        public ResourceSet UserCultureSet
        {
            get
            {
                // TODO: Leverage HttpContext.GetGlobalResourceObject(key, key); ???
                if (userCultureSet == null) {
                    userCulture = Thread.CurrentThread.CurrentCulture.Name;
                }
                return userCultureSet;
            }
        }

        #region Events

        /// <summary>
        /// Cancel Button click
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CancelBtn_Click(Object sender, EventArgs e)
        {
            OnCancel(e);
        }

        /// <summary>
        /// Update Button click
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UpdateBtn_Click(Object sender, EventArgs e)
        {
            OnUpdate(e);
        }

        /// <summary>
        /// Delete Button Click
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DeleteBtn_Click(Object sender, EventArgs e)
        {
            OnDelete(e);
        }

        const string LAYOUT_BASE_PAGE = "DesktopDefault.ascx";
        protected string MASTERPAGE_BASE_PAGE = "SiteMaster.master";

        protected bool IsMasterPageLayout { get; set; }

        /// <summary>
        /// Asigno masterpages
        /// </summary>
        /// <param name="e"></param>        
        protected override void OnPreInit(EventArgs e)
        {
            // TODO : Assign masters and themes here... :-)
            // this.Theme = "Default";

            if (portalSettings != null)
            {
            string masterLayoutPath = string.Concat(portalSettings.PortalLayoutPath, MASTERPAGE_BASE_PAGE);

                if (HttpContext.Current != null)
                {
                    if (File.Exists(HttpContext.Current.Server.MapPath(masterLayoutPath)) && Page.Master != null)
                    {
                    Page.MasterPageFile = masterLayoutPath;
                    IsMasterPageLayout = true;
                    }
                }
            }

            base.OnPreInit(e);
        }

        /// <summary>
        /// Handles the OnInit event at Page level<br/>
        /// Performs OnInit events that are common to all Pages<br/>
        /// Can be overridden
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            LoadSettings();

            Control myControl = null;

            if (cancelButton != null || (myControl = GetControl("cancelButton")) != null) {
                if (cancelButton == null) {
                    cancelButton = (System.Web.UI.WebControls.LinkButton)myControl;
                }

                cancelButton.Click += new EventHandler(CancelBtn_Click);
                cancelButton.Text = General.GetString("CANCEL", "Cancel");
                cancelButton.CausesValidation = false;
                cancelButton.EnableViewState = false;
            }
            
            if (updateButton != null || (myControl = GetControl("updateButton")) != null) {
                if (updateButton == null) {
                    updateButton = (System.Web.UI.WebControls.LinkButton)myControl;
                }

                updateButton.Click += new EventHandler(UpdateBtn_Click);
                updateButton.Text = General.GetString("APPLY", "Apply", updateButton);
                updateButton.EnableViewState = false;
            }
            
            if (deleteButton != null || (myControl = GetControl("deleteButton")) != null) {
                if (deleteButton == null) {
                    deleteButton = (System.Web.UI.WebControls.LinkButton)myControl;
                }

                deleteButton.Click += new EventHandler(DeleteBtn_Click);
                deleteButton.Text = General.GetString("DELETE", "Delete", deleteButton);
                deleteButton.EnableViewState = false;

                // Assign current permissions to Delete button
                if (PortalSecurity.HasDeletePermissions(ModuleID) == false) {
                    deleteButton.Visible = false;
                } else {
                    if (!(ClientScript.IsClientScriptBlockRegistered("confirmDelete"))) {
                        string[] s = { "CONFIRM_DELETE" };
                        ClientScript.RegisterClientScriptBlock(GetType(), "confirmDelete",
                                                               PortalSettings.GetStringResource(
                                                                   "CONFIRM_DELETE_SCRIPT",
                                                                   s));
                    }
                    deleteButton.Attributes.Add("OnClick", "return confirmDelete()");
                }
            }
            ModuleGuidInCookie();

            if (!Page.ClientScript.IsStartupScriptRegistered("jQuery"))
            {
                this.Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "jQuery", "http://ajax.microsoft.com/ajax/jquery/jquery-1.4.2.min.js");
                this.Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "jQueryUI", "http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js");
                this.Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "jQueryValidate", "http://ajax.microsoft.com/ajax/jquery.validate/1.7/jquery.validate.min.js");
                this.Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "bgiFrame", "http://jquery-ui.googlecode.com/svn/tags/latest/external/jquery.bgiframe-2.1.1.js");
                this.Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "jQueryLang", "http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.1/i18n/jquery-ui-i18n.min.js");

                this.Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "DND", Appleseed.Framework.Settings.Path.ApplicationFullPath + "/aspnet_client/js/DragNDrop.js");
            }

            if (!Page.ClientScript.IsStartupScriptRegistered("BROWSER_VERSION_WARNING"))
            {
                this.Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "BROWSER_VERSION_WARNING", Appleseed.Framework.Settings.Path.ApplicationFullPath + "/aspnet_client/js/browser_upgrade_notification.js");
            }

            //AddThis
            if (!Page.ClientScript.IsStartupScriptRegistered("ADD_THIS"))
            {

                var addThisUsernameSetting = portalSettings.CustomSettings["SITESETTINGS_ADDTHIS_USERNAME"];
                if (addThisUsernameSetting != null)
                {
                    if (Convert.ToString(addThisUsernameSetting).Trim().Length > 0)
                    {
                        //     string publisherkey = Convert.ToString(publisherkeysetting);
                        //     Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ADD_THIS",
                        //"http://w.sharethis.com/button/sharethis.js#publisher=" + publisherkey.ToString() + "&amp;type=website&amp;post_services=facebook%2Ctwitter%2Cblogger&amp;button=false");

                        // }

                        string addThisUsername = Convert.ToString(addThisUsernameSetting);
                        Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ADD_THIS",
                   "http://s7.addthis.com/js/250/addthis_widget.js#username=" + addThisUsername.ToString());


                    }
                }

                //<style type="text/css">
                //body {font-family:helvetica,sans-serif;font-size:12px;}
                //a.stbar.chicklet img {border:0;height:16px;width:16px;margin-right:3px;vertical-align:middle;}
                //a.stbar.chicklet {height:16px;line-height:16px;}
                //</style>"



            }

            base.OnInit(e);
        }


        Control GetControl(string name)
        {
            Control control = Page.FindControl(name);
            if (control == null) {
                try {
                    Control container = null;
                    MasterPage master = Page.Master;
                    while (control == null && master != null) {
                        container = master.FindControl("Content");
                        if (container != null) {
                            control = container.FindControl(name);
                        }
                        master = master.Master;
                    }
                } catch (Exception exc) {
                    ErrorHandler.Publish(LogLevel.Warn, string.Format("Error while trying to get the '{0}' control in Appleseed.Framework.Web.UI.Page.GetControl(controlName).", name), exc);
                }
            }
            return control;
        }
       

        /// <summary>
        /// Handles OnLoad event at Page level<br/>
        /// Performs OnLoad actions that are common to all Pages.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            // add CurrentTheme CSS
            RegisterCssFile(CurrentTheme.Name, CurrentTheme.CssFile);
            InsertGLAnalyticsScript();

            if (Request.Cookies["Appleseed_" + portalSettings.PortalAlias] != null) {
                if (!Config.ForceExpire) {
                    //jminond - option to kill cookie after certain time always
                    int minuteAdd = Config.CookieExpire;
                    PortalSecurity.ExtendCookie(portalSettings, minuteAdd);
                }
            }

            // Stores referring URL in viewstate
            if (!Page.IsPostBack) {
                if (Request.UrlReferrer != null)
                    UrlReferrer = Request.UrlReferrer.ToString();
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// The Add event is defined using the event keyword.
        /// The type of Add is EventHandler.
        /// </summary>
        public event EventHandler Add;

        /// <summary>
        /// EventHanlder for the click event on the add button.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void OnAdd(object source, EventArgs e)
        {
            OnAdd(e);
        }

        /// <summary>
        /// Handles OnAdd event at Page level<br/>
        /// Performs OnAdd actions that are common to all Pages<br/>
        /// Can be overridden
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnAdd(EventArgs e)
        {
            if (Add != null)
                Add(this, e); //Invokes the delegates

            //Flush cache
            OnFlushCache();

            // Verify that the current user has access to edit this module
            if (PortalSecurity.HasAddPermissions(ModuleID) == false)
                PortalSecurity.AccessDeniedEdit();

            // any other code goes here
        }

        /// <summary>
        /// The Update event is defined using the event keyword.
        /// The type of Update is EventHandler.
        /// </summary>
        public event EventHandler Update;

        /// <summary>
        /// EventHanlder for the click event on the update button.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected virtual void OnUpdate(object source, EventArgs e)
        {
            OnUpdate(e);
        }

        /// <summary>
        /// Handles OnUpdate event at Page level<br/>
        /// Performs OnUpdate actions that are common to all Pages<br/>
        /// Can be overridden
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnUpdate(EventArgs e)
        {
            if (Update != null)
                Update(this, e); //Invokes the delegates

            //Flush cache
            OnFlushCache();

            // Verify that the current user has access to edit this module
            // June 23, 2003: Mark McFarlane made change to check for both Add AND Edit permissions
            // Since UI.Page.EditPage and UI.Page.AddPage both inherit from this UI.Page class
            if (PortalSecurity.HasEditPermissions(ModuleID) == false &&
                PortalSecurity.HasAddPermissions(ModuleID) == false)
                PortalSecurity.AccessDeniedEdit();

            // any other code goes here
        }

        /// <summary>
        /// The FlushCache event is defined using the event keyword.
        /// The type of FlushCache is EventHandler.
        /// </summary>
        public event EventHandler FlushCache;

        /// <summary>
        /// Handles FlushCache event at Page level<br/>
        /// Performs FlushCache actions that are common to all Pages<br/>
        /// Can be overridden
        /// </summary>
        protected virtual void OnFlushCache()
        {
            if (FlushCache != null)
                FlushCache(this, new EventArgs()); //Invokes the delegates

            // remove module output from cache, if it's there
            StringBuilder sb = new StringBuilder();
            sb.Append("rb_");
            sb.Append(portalSettings.PortalAlias.ToLower());
            sb.Append("_mid");
            sb.Append(ModuleID.ToString());
            sb.Append("[");
            sb.Append(portalSettings.PortalContentLanguage);
            sb.Append("+");
            sb.Append(portalSettings.PortalUILanguage);
            sb.Append("+");
            sb.Append(portalSettings.PortalDataFormattingCulture);
            sb.Append("]");

            if (Context.Cache[sb.ToString()] != null) {
                Context.Cache.Remove(sb.ToString());
                Debug.WriteLine("************* Remove " + sb.ToString());
            }

            // any other code goes here
        }

        /// <summary>
        /// The Delete event is defined using the event keyword.
        /// The type of Delete is EventHandler.
        /// </summary>
        public event EventHandler Delete;

        /// <summary>
        /// EventHanlder for the click event on the delete button.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void OnDelete(object source, EventArgs e)
        {
            OnDelete(e);
        }

        /// <summary>
        /// Handles OnDelete event at Page level<br/>
        /// Performs OnDelete actions that are common to all Pages<br/>
        /// Can be overridden
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDelete(EventArgs e)
        {
            if (Delete != null)
                Delete(this, e); //Invokes the delegates

            //Flush cache
            OnFlushCache();

            // Verify that the current user has access to delete in this module
            if (PortalSecurity.HasDeletePermissions(ModuleID) == false)
                PortalSecurity.AccessDeniedEdit();

            // any other code goes here
        }

        /// <summary>
        /// The Cancel event is defined using the event keyword.
        /// The type of Cancel is EventHandler.
        /// </summary>
        public event EventHandler Cancel;

        /// <summary>
        /// Called when [cancel].
        /// </summary>
        protected virtual void OnCancel()
        {
            OnCancel(new EventArgs());
        }

        /// <summary>
        /// EventHanlder for the click event on the cancel button.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void OnCancel(object source, EventArgs e)
        {
            OnCancel(e);
        }

        /// <summary>
        /// Handles OnCancel Event at Page level<br/>
        /// Performs OnCancel actions that are common to all Pages<br/>
        /// Can be overridden
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnCancel(EventArgs e)
        {
            if (Cancel != null)
                Cancel(this, e); //Invokes the delegates

            // any other code goes here

            RedirectBackToReferringPage();
        }

        #endregion

        #region Properties (Portal)

        private PortalSettings _portalSettings;

        /// <summary>
        /// Stores current portal settings
        /// </summary>
        /// <value>The portal settings.</value>
        public PortalSettings portalSettings
        {
            get
            {
                if (_portalSettings == null) {
                    // Obtain PortalSettings from Current Context
                    if (HttpContext.Current != null)
                        _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
                }
                return _portalSettings;
            }
            set { _portalSettings = value; }
        }

        #endregion

        #region Properties (Page)

        private bool setTitle = false;

        /// <summary>
        /// Page Title
        /// </summary>
        /// <value>The page title.</value>
        public string PageTitle
        {
            get
            {
                if (!setTitle && (HttpContext.Current != null)) {
                    // see if we have a value somewhere to put.
                    string tabTitle;

                    if (portalSettings.ActivePage.CustomSettings["TabTitle"].ToString().Length != 0)
                        tabTitle = portalSettings.ActivePage.CustomSettings["TabTitle"].ToString();
                    else if (portalSettings.CustomSettings["SITESETTINGS_PAGE_TITLE"].ToString().Length != 0)
                        tabTitle = portalSettings.CustomSettings["SITESETTINGS_PAGE_TITLE"].ToString();
                    else
                        tabTitle = portalSettings.PortalTitle;

                    if (tabTitle.Length > 0) {
                        Title = tabTitle;
                        setTitle = true;
                    }
                }
                return Title;
            }
            set
            {
                Title = value;
                setTitle = true;
            }
        }

        private string pageMetaKeyWords = null;

        /// <summary>
        /// "keywords" meta element
        /// </summary>
        /// <value>The page meta key words.</value>
        public string PageMetaKeyWords
        {
            get
            {
                // Try saved viewstate value
                // tabMetaKeyWords = (string) ViewState["PageMetaKeyWords"];
                if (pageMetaKeyWords == null) {
                    if (HttpContext.Current != null &&
                        portalSettings.ActivePage.CustomSettings["TabMetaKeyWords"].ToString().Length != 0)
                        pageMetaKeyWords = portalSettings.ActivePage.CustomSettings["TabMetaKeyWords"].ToString();
                    else if (HttpContext.Current != null &&
                             portalSettings.CustomSettings["SITESETTINGS_PAGE_META_KEYWORDS"].ToString().Length != 0)
                        pageMetaKeyWords =
                            portalSettings.CustomSettings["SITESETTINGS_PAGE_META_KEYWORDS"].ToString();
                    else
                        pageMetaKeyWords = string.Empty;

                    // ViewState["PageMetaKeyWords"] = tabMetaKeyWords;
                }
                return pageMetaKeyWords;
            }
        }

        private string pageMetaDescription = null;

        /// <summary>
        /// "description" meta element
        /// </summary>
        /// <value>The page meta description.</value>
        public string PageMetaDescription
        {
            get
            {
                // Try saved viewstate value
                // tabMetaDescription = (string) ViewState["PageMetaDescription"];
                if (pageMetaDescription == null) {
                    if (HttpContext.Current != null &&
                        portalSettings.ActivePage.CustomSettings["TabMetaDescription"].ToString().Length != 0)
                        pageMetaDescription = portalSettings.ActivePage.CustomSettings["TabMetaDescription"].ToString();
                    else if (HttpContext.Current != null &&
                             portalSettings.CustomSettings["SITESETTINGS_PAGE_META_DESCRIPTION"].ToString().Length != 0)
                        pageMetaDescription =
                            portalSettings.CustomSettings["SITESETTINGS_PAGE_META_DESCRIPTION"].ToString();
                    else
                        pageMetaDescription = string.Empty;

                    // ViewState["PageMetaDescription"] = tabMetaDescription;
                }
                return pageMetaDescription;
            }
        }

        private string pageMetaEncoding = null;

        /// <summary>
        /// "encoding" meta element
        /// </summary>
        /// <value>The page meta encoding.</value>
        public string PageMetaEncoding
        {
            get
            {
                // Try saved viewstate value
                // tabMetaEncoding = (string) ViewState["PageMetaEncoding"];
                if (pageMetaEncoding == null) {
                    if (HttpContext.Current != null &&
                        portalSettings.ActivePage.CustomSettings["TabMetaEncoding"].ToString().Length != 0)
                        pageMetaEncoding = portalSettings.ActivePage.CustomSettings["TabMetaEncoding"].ToString();
                    else if (HttpContext.Current != null &&
                             portalSettings.CustomSettings["SITESETTINGS_PAGE_META_ENCODING"].ToString().Length != 0)
                        pageMetaEncoding =
                            portalSettings.CustomSettings["SITESETTINGS_PAGE_META_ENCODING"].ToString();
                    else
                        pageMetaEncoding = string.Empty;

                    // ViewState["PageMetaEncoding"] = tabMetaEncoding;
                }
                return pageMetaEncoding;
            }
        }

        private string pageMetaOther = null;

        /// <summary>
        /// Gets the page meta other.
        /// </summary>
        /// <value>The page meta other.</value>
        public string PageMetaOther
        {
            get
            {
                // Try saved viewstate value
                // tabMetaOther = (string) ViewState["PageMetaOther"];
                if (pageMetaOther == null) {
                    if (HttpContext.Current != null &&
                        portalSettings.ActivePage.CustomSettings["TabMetaOther"].ToString().Length != 0)
                        pageMetaOther = portalSettings.ActivePage.CustomSettings["TabMetaOther"].ToString();
                    else if (HttpContext.Current != null &&
                             portalSettings.CustomSettings["SITESETTINGS_PAGE_META_OTHERS"].ToString().Length != 0)
                        pageMetaOther = portalSettings.CustomSettings["SITESETTINGS_PAGE_META_OTHERS"].ToString();
                    else
                        pageMetaOther = string.Empty;

                    // ViewState["PageMetaOther"] = tabMetaOther;
                }
                return pageMetaOther;
            }
        }


        // Jes1111
        /// <summary>
        /// List of CSS files to be applied to this page
        /// </summary>
        private Hashtable cssFileList = new Hashtable(3);

        /// <summary>
        /// Determines whether [is CSS file registered] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if [is CSS file registered] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCssFileRegistered(string key)
        {
            if (cssFileList.ContainsKey(key.ToLower()))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Registers CSS file given path.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="file">The file.</param>
        public void RegisterCssFile(string key, string file)
        {
            cssFileList.Add(key.ToLower(), file);
        }

        /// <summary>
        /// Registers CSS file in which current theme folder or Default theme folder
        /// </summary>
        /// <param name="key">CSS file name</param>
        public void RegisterCssFile(string key)
        {
            string path = CurrentTheme.WebPath + "/" + key + ".css";
            string filePath = CurrentTheme.Path + "/" + key + ".css";
            if (!File.Exists(filePath)) {
                //jes 11111 - path=ThemeManager.WebPath+"/Default/"+key+".css";
                //filePath=ThemeManager.Path+"/Default/"+key+".css";
                path = ThemeManager.WebPath + "/" + "Default" + "/" + key + ".css";
                filePath = ThemeManager.Path + "/Default/" + key + ".css";
                if (!File.Exists(filePath)) {
                    return;
                }
            }
            cssFileList.Add(key.ToLower(), path);
        }

        /// <summary>
        /// Clears registered css files
        /// </summary>
        public void ClearCssFileList()
        {
            cssFileList.Clear();
        }

        /// <summary>
        /// Register the correct css module file searching in this order in current theme/mod,
        /// default theme/mod and in module folder.
        /// </summary>
        /// <param name="folderModuleName">The name of module directory</param>
        /// <param name="file">The Css file</param>
        public void RegisterCssModule(string folderModuleName, string file)
        {
            if (!IsCssFileRegistered(file)) {
                string cssFile = currentTheme.Module_CssFile(file);
                if (cssFile.Equals(string.Empty)) {
                    cssFile = Path.WebPathCombine(Path.ApplicationRoot, "DesktopModules", folderModuleName, file);
                    if (!File.Exists(HttpContext.Current.Server.MapPath(cssFile))) cssFile = string.Empty;
                }
                if (!cssFile.Equals(string.Empty))
                    RegisterCssFile(file, cssFile);
            }
        }

        // Jes1111
        /// <summary>
        /// List of CSS blocks to be applied to this page.
        /// Strings added to this list will injected into a &lt;style&gt;
        /// block in the page head.
        /// </summary>
        private Hashtable cssImportList = new Hashtable(3);

        /// <summary>
        /// Determines whether [is CSS import registered] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if [is CSS import registered] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCssImportRegistered(string key)
        {
            if (cssImportList.ContainsKey(key.ToLower()))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Registers the CSS import.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="import">The import.</param>
        public void RegisterCssImport(string key, string import)
        {
            cssImportList.Add(key.ToLower(), import);
        }

        private string docType;

        /// <summary>
        /// page DOCTYPE
        /// </summary>
        /// <value>The type of the doc.</value>
        public string DocType
        {
            get
            {
                if (docType == null) {
                    if (portalSettings.CustomSettings.ContainsKey("SITESETTINGS_DOCTYPE") &&
                        portalSettings.CustomSettings["SITESETTINGS_DOCTYPE"].ToString().Trim().Length > 0)
                        docType = portalSettings.CustomSettings["SITESETTINGS_DOCTYPE"].ToString();
                    else
                        docType = string.Empty;
                }
                return docType;
            }
        }

        /// <summary>
        /// Holds a list of javascript function calls which will be output to the body tag as a semicolon-delimited list in the 'onload' attribute
        /// </summary>
        private Hashtable bodyOnLoadList = new Hashtable(3);

        /// <summary>
        /// Determines whether [is body on load registered] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if [is body on load registered] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBodyOnLoadRegistered(string key)
        {
            if (bodyOnLoadList.ContainsKey(key.ToLower()))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Registers the body on load.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="functionCall">The function call.</param>
        public void RegisterBodyOnLoad(string key, string functionCall)
        {
            bodyOnLoadList.Add(key.ToLower(), functionCall);
        }

        private Hashtable clientScripts = new Hashtable(3);

        /// <summary>
        /// Determines whether [is client script registered] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if [is client script registered] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsClientScriptRegistered(string key)
        {
            if (clientScripts.ContainsKey(key.ToLower()))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Registers the client script.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="filePath">The file path.</param>
        public void RegisterClientScript(string key, string filePath)
        {
            clientScripts.Add(key.ToLower(), filePath);
        }

        // Jes1111
        /// <summary>
        /// Stores any additional Meta entries requested by modules or other code.
        /// </summary>
        private Hashtable additionalMetaElements = new Hashtable(3);

        /// <summary>
        /// Determines whether [is additional meta element registered] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if [is additional meta element registered] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdditionalMetaElementRegistered(string key)
        {
            if (additionalMetaElements.ContainsKey(key.ToLower()))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Registers the additional meta element.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="element">The element.</param>
        public void RegisterAdditionalMetaElement(string key, string element)
        {
            additionalMetaElements.Add(key.ToLower(), element);
        }

        private string bodyOtherAttributes = string.Empty;

        /// <summary>
        /// Referring URL
        /// </summary>
        /// <value>The URL referrer.</value>
        protected string UrlReferrer
        {
            get
            {
                if (ViewState["UrlReferrer"] != null)
                    return (string)ViewState["UrlReferrer"];
                else
                    return HttpUrlBuilder.BuildUrl();
            }
            set { ViewState["UrlReferrer"] = value; }
        }

        #endregion

        #region Properties (Pages)

        private int _tabID = 0;
        private Hashtable _Page;

        /// <summary>
        /// Stores current linked module ID if applicable
        /// </summary>
        /// <value>The page ID.</value>
        public int PageID
        {
            get
            {
                if (_tabID == 0) {
                    // Determine PageID if specified
                    if (HttpContext.Current != null && Request.Params["PageID"] != null)
                        _tabID = Int32.Parse(Request.Params["PageID"]);
                    else if (HttpContext.Current != null && Request.Params["TabID"] != null)
                        _tabID = Int32.Parse(Request.Params["TabID"]);
                }
                return _tabID;
            }
        }

        /// <summary>
        /// Stores current tab settings
        /// </summary>
        /// <value>The page settings.</value>
        public Hashtable pageSettings
        {
            get
            {
                if (_Page == null) {
                    if (PageID > 0) {
                        // thierry@tiptopweb.com.au : custom page layout, cannot be static
                        //_Page = Page.GetPageCustomSettings(PageID);
                        _Page = portalSettings.ActivePage.GetPageCustomSettings(PageID);
                    } else {
                        // Or provides an empty hashtable
                        _Page = new Hashtable();
                    }
                }
                return _Page;
            }
        }

        private Theme currentTheme;

        /// <summary>
        /// Current page theme
        /// </summary>
        /// <value>The current theme.</value>
        public Theme CurrentTheme
        {
            get
            {
                if (currentTheme == null)
                    currentTheme = portalSettings.GetCurrentTheme();
                return currentTheme;
            }
        }

        #endregion

        #region Properties (Modules)

        private int _moduleID = 0;

        /// <summary>
        /// Stores current linked module ID if applicable
        /// </summary>
        /// <value>The module ID.</value>
        public int ModuleID
        {
            get
            {
                if (_moduleID == 0) {
                    // Determine ModuleID if specified
                    if (HttpContext.Current != null && Request.Params["Mid"] != null)
                        _moduleID = Int32.Parse(Request.Params["Mid"]);
                }
                return _moduleID;
            }
        }

        private ModuleSettings _module;

        /// <summary>
        /// Stores current module if applicable
        /// </summary>
        /// <value>The module.</value>
        public ModuleSettings Module
        {
            get
            {
                if (_module == null) {
                    if (ModuleID > 0) {
                        // Obtain selected module data
                        foreach (ModuleSettings _mod in portalSettings.ActivePage.Modules) {
                            if (_mod.ModuleID == ModuleID) {
                                _module = _mod;
                                return _module;
                            }
                        }
                    } else {
                        // Return null
                        return null;
                    }
                }
                return _module;
            }
        }

        private Hashtable _moduleSettings;

        /// <summary>
        /// Stores current module settings
        /// </summary>
        /// <value>The module settings.</value>
        public Hashtable moduleSettings
        {
            get
            {
                if (_moduleSettings == null) {
                    if (ModuleID > 0)
                        // Get settings from the database
                        _moduleSettings = ModuleSettings.GetModuleSettings(ModuleID, this);
                    else
                        // Or provides an empty hashtable
                        _moduleSettings = new Hashtable();
                }
                return _moduleSettings;
            }
        }

        #endregion

        #region Properties (Items)

        private int _itemID = 0;

        /// <summary>
        /// Stores current item id
        /// </summary>
        /// <value>The item ID.</value>
        public int ItemID
        {
            get
            {
                if (_itemID == 0) {
                    // Determine ItemID if specified
                    if (HttpContext.Current != null && Request.Params["ItemID"] != null)
                        _itemID = Int32.Parse(Request.Params["ItemID"]);
                }
                return _itemID;
            }
            set { _itemID = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load settings
        /// </summary>
        protected virtual void LoadSettings()
        {
        }

        /// <summary>
        /// Redirect back to the referring page
        /// </summary>
        public void RedirectBackToReferringPage()
        {
            // Response.Redirect throws a ThreadAbortException to make it work,
            // which is handled by the ASP.NET runtime.
            // By catching an Exception (not a specialized exception, just the
            // base exception class), you end up catching the ThreadAbortException which is
            // always thrown by the Response.Redirect method. Normally, the ASP.NET runtime
            // catches this exception and handles it itself, hence your page never really
            // realized an exception occurred. So by catching this exception, you stop the
            // normal order of events that happen when redirecting.
            try {
                Response.Redirect(UrlReferrer);
            } catch (ThreadAbortException) {
            } //Do nothing it is normal
        }

        /// <summary>
        /// Overrides Render() and writes out &lt;html&gt;, &lt;head&gt; and &lt;body&gt; elements along with page contents.
        /// </summary>
        /// <param name="writer">the HtmlTextWriter connected to the output stream</param>
        protected override void Render(HtmlTextWriter writer)
        {
            BuildDocType();
            BuildHead();
            BuildBody();
            base.Render(writer);
        }

        /// <summary>
        /// Builds the DOCTYPE statement when requested by the Render() override.
        /// </summary>
        protected virtual void BuildDocType()
        {
            if (string.IsNullOrEmpty(this.DocType) && (CurrentTheme.Type == "zen" || Request.Url.PathAndQuery.IndexOf("Viewer") > 0)) {
                //this.DocType = Server.HtmlDecode( Config.DefaultDOCTYPE );
            }
        }

        /// <summary>
        /// Builds the HTML &lt;body&gt; element, adding meta tags, stylesheets and client scripts
        /// </summary>
        protected virtual void BuildHead()
        {
            this.Title = PageTitle;
            this.Header.Controls.Add(new LiteralControl("<meta name=\"generator\" content=\"Appleseed Portal - see http://www.Appleseedportal.net\"/>\n"));

            if (PageMetaKeyWords.Length != 0) {
                this.Header.Controls.Add(new LiteralControl(string.Format("<meta name=\"keywords\" content=\"{0}\"/>\n", PageMetaKeyWords)));
            }

            if (PageMetaDescription.Length != 0) {
                this.Header.Controls.Add(new LiteralControl(string.Format("<meta name=\"description\" content=\"{0}\"/>\n", PageMetaDescription)));
            }

            if (PageMetaEncoding.Length != 0) {
                this.Header.Controls.Add(new LiteralControl(PageMetaEncoding + "\n"));
            }

            if (PageMetaOther.Length != 0) {
                this.Header.Controls.Add(new LiteralControl(PageMetaOther + "\n"));
            }

            // additional metas (added by code)
            foreach (string _metaElement in additionalMetaElements.Values) {
                this.Header.Controls.Add(new LiteralControl(_metaElement + "\n"));
            }

            // ADD THE CSS <LINK> ELEMENT(S)
            foreach (string _cssFile in cssFileList.Values) {
                this.Header.Controls.Add(new LiteralControl(string.Format("<link rel=\"stylesheet\" href=\"{0}\" type=\"text/css\"/>\n", _cssFile)));
            }

            this.Header.Controls.Add(new LiteralControl(string.Format("<link rel=\"SHORTCUT ICON\" href=\"{0}/portalicon.ico\"/>\n",
                             Path.WebPathCombine(Path.ApplicationRoot, portalSettings.PortalPath))));

            if (cssImportList.Count > 0) {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("<style type=\"text/css\">");
                sb.AppendLine("<!--");
                foreach (string _cssBlock in cssImportList.Values) {
                    sb.AppendLine(_cssBlock);
                }
                sb.AppendLine("-->");
                sb.AppendLine("</style>");

                this.Header.Controls.Add(new LiteralControl(sb.ToString() + "\n"));
            }

            // ADD CLIENTSCRIPTS 
            foreach (string _script in clientScripts.Values) {
                this.Header.Controls.Add(new LiteralControl(string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>\n", _script)));
            }
        }

        /// <summary>
        /// Builds the HTML &lt;head&gt; element, adding body's onload event listeners
        /// </summary>
        protected virtual void BuildBody()
        {
            HtmlGenericControl body = null;

            foreach (Control c in Controls) {
                if (c is HtmlGenericControl) {
                    HtmlGenericControl myControl = (HtmlGenericControl)c;

                    if (myControl.TagName.ToLower() == "body") {
                        body = myControl;
                        break;
                    }
                }
            }

            // output onload attribute
            if (this.bodyOnLoadList.Count > 0) {
                StringBuilder sb = new StringBuilder();

                foreach (string _functionCall in this.bodyOnLoadList.Values) {
                    sb.Append(_functionCall);
                }

                body.Attributes["onload"] = sb.ToString();
            }
        }



        /// <summary>
        /// Inserts the GoogleAnalytics code if necessary
        /// </summary>
        private void InsertGLAnalyticsScript()
        {
            bool include = true;
            try {
                include = ConfigurationManager.AppSettings["INCLUDE_GOOGLEANALYTICS_SETTINGS"] == null ||
                            Convert.ToBoolean(ConfigurationManager.AppSettings["INCLUDE_GOOGLEANALYTICS_SETTINGS"]);
            } catch (Exception e) {
            }
            if (include) {
                if (this.portalSettings.CustomSettings["SITESETTINGS_GOOGLEANALYTICS"] != null && !this.portalSettings.CustomSettings["SITESETTINGS_GOOGLEANALYTICS"].ToString().Equals(string.Empty)) {
                    StringBuilder script = new StringBuilder();
                    script.AppendFormat("<script type=\"text/javascript\">");
                    script.AppendFormat("var gaJsHost = ((\"https:\" == document.location.protocol) ? \"https://ssl.\" : \"http://www.\");");
                    script.AppendFormat("document.write(unescape(\"%3Cscript src='\" + gaJsHost + \"google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E\"));");
                    script.AppendFormat("</script>");

                    script.AppendFormat("<script type=\"text/javascript\">");
                    script.AppendFormat("try {{ var pageTracker = _gat._getTracker(\"{0}\");", portalSettings.CustomSettings["SITESETTINGS_GOOGLEANALYTICS"].ToString());
                    script.AppendFormat("pageTracker._trackPageview();");
                    script.AppendFormat("}} catch (err) {{ }}</script>");

                    ClientScript.RegisterStartupScript(this.GetType(), "SITESETTINGS_GOOGLEANALYTICS", script.ToString(), false);
                }
            }
        }

        #endregion

        #region Security access

        /// <summary>
        /// This array is override for edit and view pages
        /// with the guids allowed to access.
        /// jviladiu@portalServices.net (2004/07/22)
        /// </summary>
        /// <value>The allowed modules.</value>
        protected virtual ArrayList AllowedModules
        {
            get { return null; }
        }


        /// <summary>
        /// Every guid module in page is set in cookie.
        /// This method is override in edit &amp; view controls for read the cookie
        /// and pass or denied access to edit or view module.
        /// jviladiu@portalServices.net (2004/07/22)
        /// </summary>
        protected virtual void ModuleGuidInCookie()
        {
            HttpCookie cookie;
            DateTime time;
            TimeSpan span;
            string guidsInUse = string.Empty;
            Guid guid;

            ModulesDB mdb = new ModulesDB();

            if (portalSettings.ActivePage.Modules.Count > 0) {
                foreach (ModuleSettings ms in portalSettings.ActivePage.Modules) {
                    guid = mdb.GetModuleGuid(ms.ModuleID);
                    if (guid != Guid.Empty) guidsInUse += guid.ToString().ToUpper() + "@";
                }
            }
            cookie = new HttpCookie("AppleseedSecurity", guidsInUse);
            time = DateTime.Now;
            span = new TimeSpan(0, 2, 0, 0, 0); // 120 minutes to expire
            cookie.Expires = time.Add(span);
            Response.AppendCookie(cookie);
        }

        #endregion

    }
}