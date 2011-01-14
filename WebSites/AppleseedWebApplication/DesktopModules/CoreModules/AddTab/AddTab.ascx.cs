using System;
using System.Data;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings.Cache;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Web.UI.WebControls;
using Appleseed.Framework.Providers.AppleseedSiteMapProvider;
using History=Appleseed.Framework.History;
using Label=System.Web.UI.WebControls.Label;
using LinkButton=System.Web.UI.WebControls.LinkButton;
using Localize=Appleseed.Framework.Web.UI.WebControls.Localize;

namespace Appleseed.Content.Web.Modules.AddModule
{
    /// <summary>
    /// This module has been built by John Mandia (www.whitelightsolutions.com)
    /// It allows administrators to give permission to selected roles to add modules to pages
    /// </summary>
    [History("jminond", "12/5/2005", "Added DropDownList to combos")]
    [History("jminond", "march 2005", "Changes for moving Tab to Page")]
    public partial class AddPage : PortalModuleControl
    {
        #region Controls

        /// <summary>
        /// 
        /// </summary>
        protected Localize tabParentLabel;

        /// <summary>
        /// 
        /// </summary>
        protected Localize tabVisibleLabel;

        /// <summary>
        /// 
        /// </summary>
        protected DropDownList PermissionDropDown;

        /// <summary>
        /// 
        /// </summary>
        protected Localize tabTitleLabel;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox TabTitleTextBox;

        /// <summary>
        /// 
        /// </summary>
        protected LinkButton AddTabButton;

        /// <summary>
        /// 
        /// </summary>
        protected Label lblErrorNotAllowed;

        /// <summary>
        /// 
        /// </summary>
        protected DropDownList parentTabDropDown;

        /// <summary>
        /// 
        /// </summary>
        protected Localize lbl_ShowMobile;

        /// <summary>
        /// 
        /// </summary>
        protected CheckBox cb_ShowMobile;

        /// <summary>
        /// 
        /// </summary>
        protected Localize lbl_MobileTabName;

        /// <summary>
        /// 
        /// </summary>
        protected TextBox tb_MobileTabName;

        /// <summary>
        /// 
        /// </summary>
        protected Localize Literal1;

        /// <summary>
        /// 
        /// </summary>
        protected RadioButtonList rbl_JumpToTab;

        /// <summary>
        /// 
        /// </summary>
        protected Localize moduleError;

        #endregion

        #region Page Load

        /// <summary>
        /// The Page_Load event handler on this User Control is used to
        /// load all the modules that are currently on this tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // If first visit to the page, update all entries
            if (Page.IsPostBack == false)
            {
                BindData();
                TabTitleTextBox.Text = General.GetString("TAB_NAME", "New Tab Name");
            }
        }

        #endregion

        /// <summary>
        /// Bind data and load miscellanous data
        /// </summary>
        public void LoadAddPageModule()
        {
            BindData();
            TabTitleTextBox.Text = General.GetString("TAB_NAME", "New Tab Name");
            AddTabButton.Click += new EventHandler(AddTabButton_Click);
        }

        #region Methods

        /// <summary>
        /// The BindData helper method is used to update the tab's
        /// layout panes with the current configuration information
        /// </summary>
        private void BindData()
        {
            // Populate the "ParentTab" Data
            DataTable dt_Pages = new PagesDB().GetPagesFlatTable(portalSettings.PortalID);
            DataColumn[] keys = new DataColumn[2];
            keys[0] = dt_Pages.Columns["PageID"];
            dt_Pages.PrimaryKey = keys;

            parentTabDropDown.DataSource = dt_Pages;
            //parentTabDropDown.DataValueField = "PageID";
            //parentTabDropDown.DataTextField = "PageOrder";
            parentTabDropDown.DataBind();
            parentTabDropDown.Items.Insert(1, "Root");

            //dt_Pages = null;

            // Preselects current tab as parent
            // Changes for Grischa Brockhaus copied by Mike Stone 7/1/2005
            if (parentTabDropDown.SelectedIndex <= 0)
            {
                int currentTab = portalSettings.ActivePage.PageID;
                parentTabDropDown.SelectedValue = (currentTab.ToString());
            }
            //	parentTabDropDown.Items.FindByValue(currentTab .ToString()).Selected = true; 

            // Translate
            //parentTabDropDown.Item(0).Text =General.GetString("ROOT_LEVEL", "Root Level", parentTabDropDown);
        }

        #endregion

        #region Events

        /// <summary>
        /// The AddTabButton_Click server event handler 
        /// on this page is used to add a new portal module 
        /// into the tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTabButton_Click(Object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Hide error message in case there was a previous error.
                moduleError.Visible = false;

                // This allows the user to pick what type of people can view the module being added.
                // If Authorised Roles is selected from the dropdown then every role that has view permission for the
                // Add Role module will be added to the view permissions of the module being added.
                string viewPermissionRoles = PermissionDropDown.SelectedValue.ToString();
                if (viewPermissionRoles == "Authorised Roles")
                {
                    viewPermissionRoles = PortalSecurity.GetViewPermissions(ModuleID);
                }

                try
                {
                    // New tabs go to the end of the list
                    PageItem t = new PageItem();
                    t.Name = TabTitleTextBox.Text;
                    t.ID = -1;
                    t.Order = 990000;

                    // Get Parent Tab Id Convert only once used many times
                    int parentTabID = int.Parse(parentTabDropDown.SelectedValue);


                    // write tab to database
                    PagesDB tabs = new PagesDB();
                    //t.ID = tabs.AddTab(portalSettings.PortalID, t.Name, viewPermissionRoles, t.Order);

                    // Changed to use new method in TabsDB.cs now all parms are possible 
                    // By Mike Stone (mstone@kaskaskia.edu) - 30/12/2004
                    t.ID =
                        tabs.AddPage(portalSettings.PortalID, parentTabID, t.Name, t.Order, viewPermissionRoles,
                                     cb_ShowMobile.Checked, tb_MobileTabName.Text);

                    //TODO.. the only way to update a parent id is throught update :S
                    // Changed to AddTab method now supports the parm
                    // Mike Stone - 30/12/2004
                    //tabs.UpdateTab(portalSettings.PortalID, t.ID, parentTabID, t.Name, t.Order, viewPermissionRoles, t.Name, false);

                    //Invalidate cache
                    // Changed to access form directly 
                    // mike stone - 30/12/2004
                    //   Cache.Remove(Appleseed.Framework.Settings.Cache.Key.TabSettings(parentTabID));
                    // Copied to here 29/12/2004 by Mike Stone
                    CurrentCache.RemoveAll("_TabNavigationSettings_");
                    //Debug.WriteLine("************* Remove " + Key.TabSettings(parentTabID));
                    
                    //Clear SiteMaps Cache
                    AppleseedSiteMapProvider.ClearAllAppleseedSiteMapCaches();

                    //Jump to Page option
                    string returnTab = string.Empty;
                    if (rbl_JumpToTab.SelectedValue.ToString() == "Yes")
                    {
                        // Redirect to New Page/Tab - Mike Stone 30/12/2004
                        // modified by Hongwei Shen 9/25/2005
                        // returnTab = HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", t.ID, "SelectedTabID=" + t.ID.ToString());
                        string newPage = "~/" + t.Name.Trim().Replace(" ", "_") + ".aspx";
                        returnTab = HttpUrlBuilder.BuildUrl(newPage, t.ID);
                    }
                    else
                    {
                        // Do NOT Redirect to New Form - Mike Stone 30/12/2004
                        // I guess every .aspx page needs to have a module tied to it. 
                        // or you will get an error about edit access denied.

                        // Modified by Hongwei Shen 9/25/2005 to fix: QueryString["tabID"] maybe null.
                        // returnTab = HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", int.Parse(Request.QueryString["tabID"]), "SelectedTabID=" + t.ID.ToString());
                        returnTab =
                            HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", PageID, "SelectedTabID=" + t.ID.ToString());
                    }
                    Response.Redirect(returnTab);
                }
                catch (Exception ex)
                {
                    moduleError.Visible = true;
                    ErrorHandler.Publish(LogLevel.Error,
                                         "There was an error with the Add Tab Module while trying to add a new tab.", ex);
                    return;
                }
                // Reload page to pick up changes
                Response.Redirect(Request.RawUrl, false);
            }
        }

        #endregion

        #region General Implementation

        /// <summary>
        /// Gets the GUID for this module.
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{A1E37A0F-4EE9-4b83-9482-43466FC21E08}"); }
        }

        /// <summary>
        /// Marks This Module To Be An Admin Module
        /// </summary>
        public override bool AdminModule
        {
            get { return true; }
        }

        /// <summary>
        /// Public constructor. Sets base settings for module.
        /// </summary>
        public AddPage()
        {
        }

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.PermissionDropDown.Items.Add("All Users");
            this.PermissionDropDown.Items.Add("Authenticated Users");
            this.PermissionDropDown.Items.Add("Unauthenticated Users");
            this.PermissionDropDown.Items.Add("Authorised Roles");
            this.PermissionDropDown.SelectedIndex = 1;

            this.AddTabButton.Click += new EventHandler(this.AddTabButton_Click);
            this.Load += new EventHandler(this.Page_Load);


            // Call base init procedure
            base.OnInit(e);
        }
        #endregion
    }
}
