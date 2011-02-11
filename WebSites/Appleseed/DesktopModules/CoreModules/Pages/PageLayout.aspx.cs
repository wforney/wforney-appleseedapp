using System;
using System.Collections;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Settings.Cache;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Users.Data;
using Appleseed.Framework.Web.UI;
using Appleseed.Framework.Web.UI.WebControls;
using History=Appleseed.Framework.History;
using ImageButton=System.Web.UI.WebControls.ImageButton;
using Appleseed.Framework.Providers.AppleseedRoleProvider;
using System.Collections.Generic;
using Appleseed.Framework.Providers.AppleseedSiteMapProvider;
using Appleseed.Framework.Core.Model;

namespace Appleseed.Admin
{
    /// <summary>
    /// Edit page for page layouts
    /// </summary>
    public partial class PageLayout : EditItemPage
    {
        #region Controls

        /// <summary>
        /// list of sorted modules in the left regioni of a page
        /// </summary>
        protected ArrayList leftList;

        /// <summary>
        /// list of sorted module in the main content pane
        /// </summary>
        protected ArrayList contentList;

        /// <summary>
        /// list of sorted modules in the right region of the page
        /// </summary>
        protected ArrayList rightList;

        /// <summary>
        /// list of sorted modules in the top region of the page
        /// </summary>
        protected ArrayList topList;

        /// <summary>
        /// list of sorted modules in the bottom region of the page
        /// </summary>
        protected ArrayList bottomList;

        #endregion

        /// <summary>
        /// The Page_Load server event handler on this page is used
        /// to populate a tab's layout settings on the page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnLoad( EventArgs e ) {
            base.OnLoad( e );

            //Confirm delete
            if ( !( ClientScript.IsClientScriptBlockRegistered( "confirmDelete" ) ) ) {
                string[] s = { "CONFIRM_DELETE" };
                ClientScript.RegisterClientScriptBlock( this.GetType(), "confirmDelete",
                                                       PortalSettings.GetStringResource(
                                                           "CONFIRM_DELETE_SCRIPT", s ) );
            }

            TopDeleteBtn.Attributes.Add("OnClick", "return confirmDelete()");
            LeftDeleteBtn.Attributes.Add("OnClick", "return confirmDelete()");
            RightDeleteBtn.Attributes.Add("OnClick", "return confirmDelete()");
            ContentDeleteBtn.Attributes.Add("OnClick", "return confirmDelete()");
            BottomDeleteBtn.Attributes.Add("OnClick", "return confirmDelete()");

            // If first visit to the page, update all entries
            if ( !Page.IsPostBack ) {
                msgError.Visible = false;

                // Set images for buttons from current theme
                TopUpBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Up", "Up.gif").ImageUrl;
                TopRightBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Bottom", "Right.gif").ImageUrl;
                TopDownBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Down", "Down.gif").ImageUrl;
                TopEditBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl;
                TopDeleteBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl;

                LeftUpBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Up", "Up.gif").ImageUrl;
                LeftRightBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Right", "Right.gif").ImageUrl;
                LeftDownBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Down", "Down.gif").ImageUrl;
                LeftEditBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl;
                LeftDeleteBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl;

                ContentUpBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Up", "Up.gif").ImageUrl;
                ContentTopBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Top", "Left.gif").ImageUrl;
                ContentLeftBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Left", "Left.gif").ImageUrl;
                ContentRightBtn.ImageUrl = CurrentTheme.GetImage( "Buttons_Right", "Right.gif" ).ImageUrl;
                ContentDownBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Down", "Down.gif").ImageUrl;
                ContentBottomBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Bottom", "Right.gif").ImageUrl;
                ContentEditBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl;
                ContentDeleteBtn.ImageUrl = CurrentTheme.GetImage( "Buttons_Delete", "Delete.gif" ).ImageUrl;

                RightUpBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Up", "Up.gif").ImageUrl;
                RightLeftBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Left", "Left.gif").ImageUrl;
                RightDownBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Down", "Down.gif").ImageUrl;
                RightEditBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl;
                RightDeleteBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl;

                BottomUpBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Up", "Up.gif").ImageUrl;
                BottomLeftBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Top", "Left.gif").ImageUrl;
                BottomDownBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Down", "Down.gif").ImageUrl;
                BottomEditBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl;
                BottomDeleteBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl;

                BindData();

                SetSecurityAccess();

                // 2/27/2003 Start - Ender Malkoc
                // After up or down button when the page is refreshed, select the previously selected
                // tab from the list.
                if ( Request.Params[ "selectedmodid" ] != null ) {
                    try {
                        int modIndex = Int32.Parse( Request.Params[ "selectedmodid" ] );
                        SelectModule(topPane, GetModules("TopPane"), modIndex);
                        SelectModule(leftPane, GetModules("LeftPane"), modIndex);
                        SelectModule(contentPane, GetModules("ContentPane"), modIndex);
                        SelectModule(rightPane, GetModules("RightPane"), modIndex);
                        SelectModule(bottomPane, GetModules("BottomPane"), modIndex);
                    }
                    catch ( Exception ex ) {
                        ErrorHandler.Publish( LogLevel.Error,
                                             "After up or down button when the page is refreshed, select the previously selected tab from the list.",
                                             ex );
                    }
                }
                // 2/27/2003 end - Ender Malkoc
            }
            // Binds custom settings to table
            EditTable.DataSource = new SortedList( pageSettings );
            EditTable.DataBind();
        }

        /// <summary>
        /// This method override the security cookie for allow
        /// to access property pages of selected module in tab.
        /// jviladiu@portalServices.net (2004/07/23)
        /// </summary>
        private void SetSecurityAccess()
        {
            HttpCookie cookie;
            DateTime time;
            TimeSpan span;
            string guidsInUse = string.Empty;
            Guid guid;

            ModulesDB mdb = new ModulesDB();

            foreach (ListItem li in topPane.Items)
            {
                guid = mdb.GetModuleGuid(int.Parse(li.Value));
                if (guid != Guid.Empty) guidsInUse += guid.ToString().ToUpper() + "@";
            }

            foreach (ListItem li in leftPane.Items)
            {
                guid = mdb.GetModuleGuid(int.Parse(li.Value));
                if (guid != Guid.Empty) guidsInUse += guid.ToString().ToUpper() + "@";
            }

            foreach (ListItem li in contentPane.Items)
            {
                guid = mdb.GetModuleGuid(int.Parse(li.Value));
                if (guid != Guid.Empty) guidsInUse += guid.ToString().ToUpper() + "@";
            }

            foreach (ListItem li in rightPane.Items)
            {
                guid = mdb.GetModuleGuid(int.Parse(li.Value));
                if (guid != Guid.Empty) guidsInUse += guid.ToString().ToUpper() + "@";
            }

            foreach (ListItem li in bottomPane.Items)
            {
                guid = mdb.GetModuleGuid(int.Parse(li.Value));
                if (guid != Guid.Empty) guidsInUse += guid.ToString().ToUpper() + "@";
            }

            cookie = new HttpCookie("AppleseedSecurity", guidsInUse);
            time = DateTime.Now;
            span = new TimeSpan(0, 2, 0, 0, 0); // 120 minutes to expire
            cookie.Expires = time.Add(span);
            base.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// Given the moduleID of a module, this function selects the right tab in the provided list control
        /// </summary>
        /// <param name="listBox">Listbox that contains the list of modules</param>
        /// <param name="modules">ArrayList containing the Module Items</param>
        /// <param name="moduleID">moduleID of the module that needs to be selected</param>
        private void SelectModule(ListBox listBox, ArrayList modules, int moduleID)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (((ModuleItem) modules[i]).ID == moduleID)
                {
                    if (listBox.SelectedItem != null) listBox.SelectedItem.Selected = false;
                    listBox.Items[i].Selected = true;
                    return;
                }
            }
            return;
        }

        /// <summary>
        /// Appends the module ID.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="moduleID">The module ID.</param>
        /// <returns></returns>
        private string AppendModuleID(string url, int moduleID)
        {
            int selectedModIDPos = url.IndexOf("&selectedmodid");
            if (selectedModIDPos >= 0)
            {
                int selectedModIDEndPos = url.IndexOf("&", selectedModIDPos + 1);
                if (selectedModIDEndPos >= 0)
                {
                    return
                        url.Substring(0, selectedModIDPos) + "&selectedmodid=" + moduleID +
                        url.Substring(selectedModIDEndPos);
                }
                else
                {
                    return url.Substring(0, selectedModIDPos) + "&selectedmodid=" + moduleID;
                }
            }
            else
            {
                return url + "&selectedmodid=" + moduleID;
            }
        }

        /// <summary>
        /// The AddModuleToPane_Click server event handler
        /// on this page is used to add a new portal module
        /// into the tab
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        [History("bja@reedtek.com", "2003/05/16", "Added extra parameter for collpasable window")]
        [History("john.mandia@whitelightsolutions.com", "2003/05/24", "Added extra parameter for ShowEverywhere")]
        protected void AddModuleToPane_Click(Object sender, EventArgs e)
        {
            // All new modules go to the end of the contentpane
            ModuleItem m = new ModuleItem();
            m.Title = moduleTitle.Text;
            m.ModuleDefID = Int32.Parse(moduleType.SelectedItem.Value);
            m.Order = 999;

            // save to database
            ModulesDB _mod = new ModulesDB();
            // Change by Geert.Audenaert@Syntegra.Com
            // Date: 6/2/2003
            // Original:             m.ID = _mod.AddModule(tabID, m.Order, "ContentPane", m.Title, m.ModuleDefID, 0, "Admins", "All Users", "Admins", "Admins", "Admins", false);
            // Changed by Mario Endara <mario@softworks.com.uy> (2004/11/09)
            // The new module inherits security from Pages module (current ModuleID) 
            // so who can edit the tab properties/content can edit the module properties/content (except view that remains =)
            m.ID =
                _mod.AddModule(PageID, m.Order, paneLocation.SelectedItem.Value.ToString(), m.Title, m.ModuleDefID, 0,
                               PortalSecurity.GetEditPermissions(ModuleID),
                               viewPermissions.SelectedItem.Value.ToString(),
                               PortalSecurity.GetAddPermissions(ModuleID), PortalSecurity.GetDeletePermissions(ModuleID),
                               PortalSecurity.GetPropertiesPermissions(ModuleID),
                               PortalSecurity.GetMoveModulePermissions(ModuleID),
                               PortalSecurity.GetDeleteModulePermissions(ModuleID), false,
                               PortalSecurity.GetPublishPermissions(ModuleID), false, false, false);
            // End Change Geert.Audenaert@Syntegra.Com

            // reload the portalSettings from the database
            Context.Items["PortalSettings"] = new PortalSettings(PageID, portalSettings.PortalAlias);
            portalSettings = (PortalSettings) Context.Items["PortalSettings"];

            // reorder the modules in the content pane
            ArrayList modules = GetModules("ContentPane");
            OrderModules(modules);

            // resave the order
            foreach (ModuleItem item in modules)
            {
                _mod.UpdateModuleOrder(item.ID, item.Order, "ContentPane");
            }

            // Redirect to the same page to pick up changes
            Response.Redirect(AppendModuleID(Request.RawUrl, m.ID));
        }

        /// <summary>
        /// The UpDown_Click server event handler on this page is
        /// used to move a portal module up or down on a tab's layout pane
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void UpDown_Click(Object sender, ImageClickEventArgs e)
        {
            string cmd = ((ImageButton) sender).CommandName;
            string pane = ((ImageButton) sender).CommandArgument;
            ListBox _listbox = (ListBox) Page.FindControl(pane);
            if (_listbox == null) {
                _listbox = (ListBox)Page.Master.FindControl("Content").FindControl(pane);
            }

            ArrayList modules = GetModules(pane);

            if (_listbox.SelectedIndex != -1)
            {
                int delta;
                int selection = -1;

                // Determine the delta to apply in the order number for the module
                // within the list.  +3 moves down one item; -3 moves up one item
                if (cmd == "down")
                {
                    delta = 3;
                    if (_listbox.SelectedIndex < _listbox.Items.Count - 1)
                        selection = _listbox.SelectedIndex + 1;
                }
                else
                {
                    delta = -3;
                    if (_listbox.SelectedIndex > 0)
                        selection = _listbox.SelectedIndex - 1;
                }

                ModuleItem m;
                m = (ModuleItem) modules[_listbox.SelectedIndex];

                if (PortalSecurity.IsInRoles(PortalSecurity.GetMoveModulePermissions(m.ID)))
                {
                    m.Order += delta;

                    // reorder the modules in the content pane
                    OrderModules(modules);

                    // resave the order
                    ModulesDB admin = new ModulesDB();
                    foreach (ModuleItem item in modules)
                    {
                        admin.UpdateModuleOrder(item.ID, item.Order, pane);
                    }

                    // Redirect to the same page to pick up changes
                    Response.Redirect(AppendModuleID(Request.RawUrl, m.ID));
                }
                else
                {
                    msgError.Visible = true;
                    return;
                }
            }
        }

        /// <summary>
        /// The RightLeft_Click server event handler on this page is
        /// used to move a portal module between layout panes on
        /// the tab page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void RightLeft_Click(Object sender, ImageClickEventArgs e)
        {
            string sourcePane = ((ImageButton) sender).Attributes["sourcepane"];
            string targetPane = ((ImageButton) sender).Attributes["targetpane"];
            ListBox sourceBox = (ListBox) Page.FindControl(sourcePane);
            if (sourceBox == null) {
                sourceBox = (ListBox)Page.Master.FindControl("Content").FindControl(sourcePane);
            }
            ListBox targetBox = (ListBox) Page.FindControl(targetPane);
            if (targetBox == null) {
                targetBox = (ListBox)Page.Master.FindControl("Content").FindControl(targetPane);
            }

            if (sourceBox.SelectedIndex != -1)
            {
                // get source arraylist
                ArrayList sourceList = GetModules(sourcePane);

                // get a reference to the module to move
                // and assign a high order number to send it to the end of the target list
                ModuleItem m = (ModuleItem) sourceList[sourceBox.SelectedIndex];

                if (PortalSecurity.IsInRoles(PortalSecurity.GetMoveModulePermissions(m.ID)))
                {
                    // add it to the database
                    ModulesDB admin = new ModulesDB();
                    admin.UpdateModuleOrder(m.ID, 99, targetPane);

                    // delete it from the source list
                    sourceList.RemoveAt(sourceBox.SelectedIndex);

                    // reload the portalSettings from the database
                    HttpContext.Current.Items["PortalSettings"] = new PortalSettings(PageID, portalSettings.PortalAlias);
                    portalSettings = (PortalSettings) Context.Items["PortalSettings"];

                    // reorder the modules in the source pane
                    sourceList = GetModules(sourcePane);
                    OrderModules(sourceList);

                    // resave the order
                    foreach (ModuleItem item in sourceList)
                        admin.UpdateModuleOrder(item.ID, item.Order, sourcePane);

                    // reorder the modules in the target pane
                    ArrayList targetList = GetModules(targetPane);
                    OrderModules(targetList);

                    // resave the order
                    foreach (ModuleItem item in targetList)
                        admin.UpdateModuleOrder(item.ID, item.Order, targetPane);

                    // Redirect to the same page to pick up changes
                    Response.Redirect(AppendModuleID(Request.RawUrl, m.ID));
                }
                else
                {
                    msgError.Visible = true;
                }
            }
        }

        /// <summary>
        /// The OnUpdate on this page is used to save
        /// the current tab settings to the database and
        /// then redirect back to the main admin page.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnUpdate(EventArgs e)
        {
            // Only Update if Input Data is Valid
            if (Page.IsValid == true)
            {
                try
                {
                    SavePageData();

                    // Flush all tab navigation cache keys. Very important for recovery the changes
                    // made in all languages and not get a error if user change the tab parent.
                    // jviladiu@portalServices.net (05/10/2004)
                    CurrentCache.RemoveAll("_PageNavigationSettings_");
                    // Clear AppleseedSiteMapCache
                    AppleseedSiteMapProvider.ClearAllAppleseedSiteMapCaches();
                    

                    // redirect back to the admin page
                    // int adminIndex = portalSettings.DesktopPages.Count-1;        
                    // 3_aug_2004 Cory Isakson use returntabid from QueryString
                    // Updated 6_Aug_2004 by Cory Isakson to accomodate addtional Page Management
                    string retPage = Request.QueryString["returnPageID"];
                    string returnPage;

                    if (retPage != null) // user is returned to the calling tab.
                    {
                        returnPage = HttpUrlBuilder.BuildUrl(int.Parse(retPage));
                    }
                    else // user is returned to updated tab
                    {
                        returnPage = HttpUrlBuilder.BuildUrl(PageID);
                    }
                    Response.Redirect(returnPage);
                }
                catch
                {
                    lblErrorNotAllowed.Visible = true;
                }
            }
        }

        /// <summary>
        /// The PageSettings_Change server event handler on this page is
        /// invoked any time the tab name or access security settings
        /// change.  The event handler in turn calls the "SavePageData"
        /// helper method to ensure that these changes are persisted
        /// to the portal configuration file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void PageSettings_Change(Object sender, EventArgs e)
        {
            // Ensure that settings are saved
            SavePageData();
        }

        /// <summary>
        /// The SavePageData helper method is used to persist the
        /// current tab settings to the database.
        /// </summary>
        private void SavePageData()
        {
            // Construct Authorized User Roles string
            string authorizedRoles = string.Empty;

            foreach ( ListItem item in authRoles.Items ) {
                if ( item.Selected == true ) {
                    authorizedRoles = authorizedRoles + item.Text + ";";
                }
            }

            // update Page info in the database
            new PagesDB().UpdatePage(portalSettings.PortalID, PageID, Int32.Parse(parentPage.SelectedItem.Value),
                                     tabName.Text, portalSettings.ActivePage.PageOrder, authorizedRoles,
                                     mobilePageName.Text, showMobile.Checked);

            // Update custom settings in the database
            EditTable.UpdateControls();
        }

        /// <summary>
        /// The EditBtn_Click server event handler on this page is
        /// used to edit an individual portal module's settings
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void EditBtn_Click(Object sender, ImageClickEventArgs e)
        {
            string pane = ((ImageButton) sender).CommandArgument;
            ListBox _listbox = (ListBox) Page.FindControl(pane);
            if (_listbox == null) {
                _listbox = (ListBox)Page.Master.FindControl("Content").FindControl(pane);
            }

            if (_listbox.SelectedIndex != -1)
            {
                int mid = Int32.Parse(_listbox.SelectedItem.Value);

                // Add role control to edit module settings by Mario Endara <mario@softworks.com.uy> (2004/11/09)
                if (PortalSecurity.IsInRoles(PortalSecurity.GetPropertiesPermissions(mid)))
                {
                    // Redirect to module settings page
                    Response.Redirect(HttpUrlBuilder.BuildUrl("~/DesktopModules/CoreModules/Admin/ModuleSettings.aspx", PageID, mid));
                }
                else
                {
                    msgError.Visible = true;
                    return;
                }
            }
        }

        /// <summary>
        /// The DeleteBtn_Click server event handler on this page is
        /// used to delete a portal module from the page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void DeleteBtn_Click(Object sender, ImageClickEventArgs e)
        {
            string pane = ((ImageButton) sender).CommandArgument;
            ListBox _listbox = (ListBox) Page.FindControl(pane);
            if (_listbox == null) {
                _listbox = (ListBox)Page.Master.FindControl("Content").FindControl(pane);
            }

            ArrayList modules = GetModules(pane);

            if (_listbox.SelectedIndex != -1)
            {
                ModuleItem m = (ModuleItem) modules[_listbox.SelectedIndex];
                if (m.ID > -1)
                {
                    // jviladiu@portalServices.net (20/08/2004) Add role control for delete module
                    if (PortalSecurity.IsInRoles(PortalSecurity.GetDeleteModulePermissions(m.ID)))
                    {
                        // must delete from database too
                        ModulesDB moddb = new ModulesDB();
                        // TODO add userEmail and useRecycler
                        moddb.DeleteModule(m.ID);
                    }
                    else
                    {
                        msgError.Visible = true;
                        return;
                    }
                }
            }

            // Redirect to the same page to pick up changes
            Response.Redirect(Request.RawUrl);
        }

        /// <summary>
        /// The BindData helper method is used to update the tab's
        /// layout panes with the current configuration information
        /// </summary>
        private void BindData() {
            PageSettings page = portalSettings.ActivePage;

            // Populate Page Names, etc.
            tabName.Text = page.PageName;
            mobilePageName.Text = page.MobilePageName;
            showMobile.Checked = page.ShowMobile;

            // Populate the "ParentPage" Data
            PagesDB t = new PagesDB();
            IList<PageItem> items = t.GetPagesParent( portalSettings.PortalID, PageID );
            parentPage.DataSource = items;
            parentPage.DataBind();

            if ( parentPage.Items.FindByValue( page.ParentPageID.ToString() ) != null ) {
                //parentPage.Items.FindByValue( tab.ParentPageID.ToString() ).Selected = true;

                parentPage.SelectedValue = page.ParentPageID.ToString();
            }

            // Translate
            if ( parentPage.Items.FindByText( " ROOT_LEVEL" ) != null )
                parentPage.Items.FindByText( " ROOT_LEVEL" ).Text =
                    General.GetString( "ROOT_LEVEL", "Root Level", parentPage );

            // Populate checkbox list with all security roles for this portal
            // and "check" the ones already configured for this tab
            UsersDB users = new UsersDB();
            IList<AppleseedRole> roles = users.GetPortalRoles( portalSettings.PortalAlias );

            // Clear existing items in checkboxlist
            authRoles.Items.Clear();

            foreach ( AppleseedRole role in roles ) {
                ListItem item = new ListItem();
                item.Text = role.Name;
                item.Value = role.Id.ToString();

                if ( ( page.AuthorizedRoles.LastIndexOf( item.Text ) ) > -1 )
                    item.Selected = true;

                authRoles.Items.Add( item );
            }

            // Populate the "Add Module" Data
            ModulesDB m = new ModulesDB();
            SortedList<string, string> modules = new SortedList<string, string>();
            SqlDataReader drCurrentModuleDefinitions = m.GetCurrentModuleDefinitions(portalSettings.PortalID);
            if (PortalSecurity.IsInRoles("Admins") == true ||
                       !(bool.Parse(drCurrentModuleDefinitions["Admin"].ToString()))) {
                
                try {
                    while (drCurrentModuleDefinitions.Read()) {
                        if (!modules.ContainsKey(drCurrentModuleDefinitions["FriendlyName"].ToString())) {
                            modules.Add(
                                //moduleType.Items.Add(
                                //    new ListItem(drCurrentModuleDefinitions["FriendlyName"].ToString(),
                                //                 drCurrentModuleDefinitions["ModuleDefID"].ToString()));
                                drCurrentModuleDefinitions["FriendlyName"].ToString(),
                                drCurrentModuleDefinitions["ModuleDefID"].ToString()
                            );
                        }

                    }

                } finally {
                    drCurrentModuleDefinitions.Close();
                }
            }
            //Dictionary<string, string> actions = ModelServices.GetMVCActionModules();
            //foreach (string key in actions.Keys) {
            //    modules.Add(key, actions[key]);
            //}

            moduleType.DataSource = modules;
            moduleType.DataBind();

            // Populate Top Pane Module Data
            topList = GetModules("TopPane");
            topPane.DataBind();

            // Populate Left Hand Pane Module Data
            leftList = GetModules("LeftPane");
            leftPane.DataBind();

            // Populate Content Pane Module Data
            contentList = GetModules( "ContentPane" );
            contentPane.DataBind();

            // Populate Right Hand Module Data
            rightList = GetModules("RightPane");
            rightPane.DataBind();

            // Populate Bottom Module Data
            bottomList = GetModules("BottomPane");
            bottomPane.DataBind();

        }

        /// <summary>
        /// The GetModules helper method is used to get the modules
        /// for a single pane within the tab
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <returns></returns>
        private ArrayList GetModules(string pane)
        {
            ArrayList paneModules = new ArrayList();

            foreach (ModuleSettings _module in portalSettings.ActivePage.Modules)
            {
                if ((_module.PaneName).ToLower() == pane.ToLower() && portalSettings.ActivePage.PageID == _module.PageID
                    )
                {
                    ModuleItem m = new ModuleItem();
                    m.Title = _module.ModuleTitle;
                    m.ID = _module.ModuleID;
                    m.ModuleDefID = _module.ModuleDefID;
                    m.Order = _module.ModuleOrder;
                    paneModules.Add(m);
                }
            }

            return paneModules;
        }

        /// <summary>
        /// The OrderModules helper method is used to reset the display
        /// order for modules within a pane
        /// </summary>
        /// <param name="list">The list.</param>
        private void OrderModules(ArrayList list)
        {
            int i = 1;

            // sort the arraylist
            list.Sort();

            // renumber the order
            foreach (ModuleItem m in list)
            {
                // number the items 1, 3, 5, etc. to provide an empty order
                // number when moving items up and down in the list.
                m.Order = i;
                i += 2;
            }
        }

        /// <summary>
        /// Handles the UpdateControl event of the EditTable control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:Appleseed.Framework.Web.UI.WebControls.SettingsTableEventArgs"/> instance containing the event data.</param>
        protected void EditTable_UpdateControl(object sender, SettingsTableEventArgs e)
        {
            PageSettings.UpdatePageSettings(PageID, e.CurrentItem.EditControl.ID, e.CurrentItem.Value);
        }
    }
}
