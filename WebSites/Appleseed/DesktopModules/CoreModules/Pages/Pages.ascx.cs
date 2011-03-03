using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.UI;
using Appleseed.Framework;
using Appleseed.Framework.Data;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Settings.Cache;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Web.UI.WebControls;
using Appleseed.Framework.Providers.AppleseedSiteMapProvider;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Pages : PortalModuleControl
    {
        /// <summary>
        /// 
        /// </summary>
        protected ArrayList portalPages { get; set; }

        /// <summary>
        /// Admin Module
        /// </summary>
        public override bool AdminModule
        {
            get { return true; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Set the ImageUrl for controls from current Theme
            upBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Up", "Up.gif").ImageUrl;
            downBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Down", "Down.gif").ImageUrl;
            DeleteBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl;
            EditBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl;

            // If this is the first visit to the page, bind the tab data to the page listbox
            portalPages = new PagesDB().GetPagesFlat(portalSettings.PortalID);
            if (!Page.IsPostBack) {

                tabList.DataSource = portalPages;
                tabList.DataBind();

                // 2/27/2003 Start - Ender Malkoc
                // After up or down button when the page is refreshed, 
                // select the previously selected tab from the list.
                if (Request.Params["selectedtabID"] != null) {
                    try {
                        int tabIndex = Int32.Parse(Request.Params["selectedtabID"]);
                        SelectPage(tabIndex);
                    } catch {
                    }
                }
                // 2/27/2003 End - Ender Malkoc
            }
        }

        /// <summary>
        /// This is where you add module settings. These settings
        /// are used to control the behavior of the module
        /// </summary>
        public Pages()
        {
            // EHN: Add new version control for tabs module. 
            //      Mike Stone - 19/12/2004
            SettingItem PageVersion = new SettingItem(new BooleanDataType());
            PageVersion.Value = "True";
            PageVersion.EnglishName = "Use Old Version?";
            PageVersion.Description =
                "If Checked the module acts has it always did. If not it uses the new short form which allows security to be set so the new tab will not be seen by all users.";
            PageVersion.Order = 10;
            _baseSettings.Add("TAB_VERSION", PageVersion);
        }

        /// <summary>
        /// The UpDown_Click server event handler on this page is
        /// used to move a portal module up or down on a tab's layout pane
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void UpDown_Click(object sender, ImageClickEventArgs e)
        {
            string cmd = ((ImageButton)sender).CommandName;

            if (tabList.SelectedIndex > -1) {
                int delta;

                // Determine the delta to apply in the order number for the module
                // within the list.  +3 moves down one item; -3 moves up one item
                if (cmd == "down") {
                    delta = 3;
                } else {
                    delta = -3;
                }

                PageItem t;
                t = (PageItem)portalPages[tabList.SelectedIndex];
                t.Order += delta;
                OrderPages();
                Response.Redirect(HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", PageID, "selectedtabID=" + t.ID));
            }
        }

        /// <summary>
        /// The DeleteBtn_Click server event handler is used to delete
        /// the selected tab from the portal
        /// </summary>
        protected void DeleteBtn_Click(object sender, ImageClickEventArgs e)
        {
            //base.OnDelete();

            if (tabList.SelectedIndex > -1) {
                try {
                    // must delete from database too
                    PageItem t = (PageItem)portalPages[tabList.SelectedIndex];
                    PagesDB tabs = new PagesDB();
                    tabs.DeletePage(t.ID);

                    portalPages.RemoveAt(tabList.SelectedIndex);

                    if (tabList.SelectedIndex > 0)
                        t = (PageItem)portalPages[tabList.SelectedIndex - 1];

                    OrderPages();

                    Response.Redirect(HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", PageID, "SelectedPageID=" + t.ID));
                } catch (SqlException) {
                    Controls.Add(
                        new LiteralControl("<br><span class='Error'>" +
                                           General.GetString("TAB_DELETE_FAILED", "Failed to delete Page", this) +
                                           "<br>"));
                }
            }
        }

        protected void EditBtn_Click(object sender, ImageClickEventArgs e)
        {
            // Redirect to edit page of currently selected tab
            if (tabList.SelectedIndex > -1) {
                // Redirect to module settings page
                PageItem t = (PageItem)portalPages[tabList.SelectedIndex];

                // added mID by Mario Endara <mario@softworks.com.uy> to support security check (2004/11/09)
                Response.Redirect(
                    string.Concat("~/DesktopModules/CoreModules/Pages/PageLayout.aspx?PageID=", t.ID, "&mID=", ModuleID.ToString(),
                                  "&Alias=", portalSettings.PortalAlias, "&returntabid=", Page.PageID));
            }
        }

        /// <summary>
        /// The AddPage_Click server event handler is used
        /// to add a new tab for this portal
        /// </summary>
        /// <param name="Sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void AddPage_Click(Object Sender, EventArgs e)
        {
            if (Settings["TAB_VERSION"] != null) {
                if (Settings["TAB_VERSION"].ToString() == "True") // Use Old Version
                {
                    // New tabs go to the end of the list
                    PageItem t = new PageItem();
                    t.Name = General.GetString("TAB_NAME", "New Page Name"); //Just in case it comes to be empty
                    t.ID = -1;
                    t.Order = 990000;
                    portalPages.Add(t);

                    // write tab to database
                    PagesDB tabs = new PagesDB();
                    t.ID = tabs.AddPage(portalSettings.PortalID, t.Name, t.Order);

                    // Reset the order numbers for the tabs within the list  
                    OrderPages();

                    //Clear SiteMaps Cache
                    AppleseedSiteMapProvider.ClearAllAppleseedSiteMapCaches();

                    // Redirect to edit page
                    // 3_aug_2004 Cory Isakson added returntabid so that PageLayout could return to the tab it was called from.
                    // added mID by Mario Endara <mario@softworks.com.uy> to support security check (2004/11/09)
                    Response.Redirect(
                        HttpUrlBuilder.BuildUrl("~/DesktopModules/CoreModules/Pages/PageLayout.aspx", t.ID,
                                                "mID=" + ModuleID.ToString() + "&returntabid=" + Page.PageID));
                } else {
                    // Redirect to New Form - Mike Stone 19/12/2004
                    Response.Redirect(
                        HttpUrlBuilder.BuildUrl("~/DesktopModules/CoreModules/Pages/AddPage.aspx",
                                                "mID=" + ModuleID.ToString() + "&returntabid=" + Page.PageID));
                }
            }
        }

        /// <summary>
        /// The OrderPages helper method is used to reset
        /// the display order for tabs within the portal
        /// </summary>
        private void OrderPages()
        {
            int i = 1;

            portalPages.Sort();

            foreach (PageItem t in portalPages) {
                // number the items 1, 3, 5, etc. to provide an empty order
                // number when moving items up and down in the list.
                t.Order = i;
                i += 2;

                // rewrite tab to database
                PagesDB tabs = new PagesDB();
                // 12/16/2002 Start - Cory Isakson 
                tabs.UpdatePageOrder(t.ID, t.Order);
                // 12/16/2002 End - Cory Isakson 
            }
            //gbs: Invalidate cache, fix for bug RBM-220
            CurrentCache.RemoveAll("_PageNavigationSettings_");
        }

        /// <summary>
        /// Given the tabID of a tab, this function selects the right tab in the provided list control
        /// </summary>
        /// <param name="tabID">tabID of the tab that needs to be selected</param>
        private void SelectPage(int tabID)
        {
            for (int i = 0; i < tabList.Items.Count; i++) {
                if (((PageItem)portalPages[i]).ID == tabID) {
                    if (tabList.SelectedItem != null) tabList.SelectedItem.Selected = false;
                    tabList.Items[i].Selected = true;
                    return;
                }
            }
            return;
        }

        /// <summary>
        /// GuidID
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{1C575D94-70FC-4A83-80C3-2087F726CBB3}"); }
        }

        # region Install / Uninstall Implementation

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            string currentScriptName = Server.MapPath(TemplateSourceDirectory + "/Install.sql");
            ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0) {
                // Call rollback
                throw new Exception("Error occurred:" + errors[0].ToString());
            }
        }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Uninstall(IDictionary stateSaver)
        {
            //Cannot be uninstalled
            throw new Exception("This is an essential module that can be unistalled");
        }

        #endregion

    }
}