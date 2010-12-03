using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using DUEMETRI.UI.WebControls.HWMenu;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Settings.Cache;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Site.Data;

namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// Menu navigation inherits from Menu Webcontrol
    /// and adds the 'glue' to link to tabs tree.
    /// Bugfix #656794 'Menu rendering adds all tabs' by abain
    /// </summary>
    [History("jperry", "2003/05/01", "Code changed to more closely resemble DesktopNavigation")]
    [History("jperry", "2003/05/02", "Support for new binding options.")]
    [History("MH", "2003/05/23", "Added bind option 'BindOptionDefinedParent' and 'ParentPageID'.")]
    [
        History("jviladiu@portalServices.net", "2004/08/26",
            "Add AutoShopDetect support and set url's for categories of products")]
    public class MenuNavigation : Menu, INavigation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuNavigation"/> class.
        /// </summary>
        public MenuNavigation()
        {
            EnableViewState = false;
            Load += new EventHandler(LoadControl);
        }

        /// <summary>
        /// Loads the control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LoadControl(object sender, EventArgs e)
        {
            if (AutoBind)
                DataBind();
        }

        #region INavigation implementation

        private BindOption _bind = BindOption.BindOptionTop;
        //MH: added 29/04/2003 by mario@hartmann.net
        private int _definedParentTab = -1;
        //MH: end

        private bool _useTabNameInUrl = false;

        /// <summary>
        /// Indicates if control should show the tabname in the url
        /// </summary>
        /// <value><c>true</c> if [use tab name in URL]; otherwise, <c>false</c>.</value>
        [Category("Data"), PersistenceMode(PersistenceMode.Attribute)]
        public bool UseTabNameInUrl
        {
            get { return _useTabNameInUrl; }
            set { _useTabNameInUrl = value; }
        }

        private bool _autoShopDetect = false;

        /// <summary>
        /// Indicates if control should detect products module when loads
        /// </summary>
        /// <value><c>true</c> if [auto shop detect]; otherwise, <c>false</c>.</value>
        [Category("Data"), PersistenceMode(PersistenceMode.Attribute)]
        public bool AutoShopDetect
        {
            get { return _autoShopDetect; }
            set { _autoShopDetect = value; }
        }

        private bool _autoBind = false;

        /// <summary>
        /// Indicates if control should bind when loads
        /// </summary>
        /// <value></value>
        [
            Category("Data"),
                PersistenceMode(PersistenceMode.Attribute)
            ]
        public bool AutoBind
        {
            get { return _autoBind; }
            set { _autoBind = value; }
        }

        /// <summary>
        /// Describes how this control should bind to db data
        /// </summary>
        /// <value></value>
        [
            Category("Data"),
                PersistenceMode(PersistenceMode.Attribute)
            ]
        public BindOption Bind
        {
            get { return _bind; }
            set
            {
                if (_bind != value)
                {
                    _bind = value;
                }
            }
        }

        //MH: added 23/05/2003 by mario@hartmann.net
        /// <summary>
        /// defines the parentPageID when using BindOptionDefinedParent
        /// </summary>
        /// <value></value>
        [
            Category("Data"),
                PersistenceMode(PersistenceMode.Attribute)
            ]
        public int ParentPageID
        {
            get { return _definedParentTab; }
            set
            {
                if (_definedParentTab != value)
                {
                    _definedParentTab = value;
                }
            }
        }

        //MH: end

        #endregion

        /// <summary>
        /// Do databind.
        /// Thanks to abain for cleaning up the code and fixing the bugs
        /// </summary>
        public override void DataBind()
        {
            base.DataBind();


            // Obtain PortalSettings from Current Context 
            PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];


            // Build list of tabs to be shown to user 
            ArrayList authorizedTabs = new ArrayList();

            authorizedTabs = (ArrayList) GetInnerDataSource();

            for (int i = 0; i < authorizedTabs.Count; i++)
            {
                PageStripDetails myTab = (PageStripDetails) authorizedTabs[i];
 
                AddMenuTreeNode(0, myTab);
            }
        }

        /// <summary>
        /// Gives the me URL.
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        private string giveMeUrl(string tab, int id)
        {
            if (!UseTabNameInUrl) return HttpUrlBuilder.BuildUrl(id);
            string auxtab = string.Empty;
            foreach (char c in tab)
                if (char.IsLetterOrDigit(c)) auxtab += c;
                else auxtab += "_";
            return HttpUrlBuilder.BuildUrl("~/" + auxtab + ".aspx", id);
        }

        /// <summary>
        /// Shops the desktop navigation.
        /// </summary>
        /// <param name="myTab">My tab.</param>
        private void ShopDesktopNavigation(PageStripDetails myTab)
        {
            if (PortalSecurity.IsInRoles(myTab.AuthorizedRoles))
            {
                MenuTreeNode mn = new MenuTreeNode(myTab.PageName);

                mn.Link =
                    HttpUrlBuilder.BuildUrl("~/" + HttpUrlBuilder.DefaultPage, myTab.ParentPageID,
                                            "ItemID=" + myTab.PageID.ToString());
                mn.Height = Height;
                mn.Width = Width;
                mn = RecourseMenuShop(0, myTab.Pages, mn, myTab.ParentPageID);
                Childs.Add(mn);
            }
        }

        /// <summary>
        /// Recourses the menu shop.
        /// </summary>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="t">The t.</param>
        /// <param name="mn">The mn.</param>
        /// <param name="idShop">The id shop.</param>
        /// <returns></returns>
        protected virtual MenuTreeNode RecourseMenuShop(int tabIndex, PagesBox t, MenuTreeNode mn, int idShop)
        {
            if (t.Count > 0)
            {
                for (int c = 0; c < t.Count; c++)
                {
                    PageStripDetails mySubTab = t[c];

                    if (PortalSecurity.IsInRoles(mySubTab.AuthorizedRoles))
                    {
                        MenuTreeNode mnc = new MenuTreeNode(mySubTab.PageName);

                        mnc.Link =
                            HttpUrlBuilder.BuildUrl("~/" + HttpUrlBuilder.DefaultPage, idShop,
                                                    "ItemID=" + mySubTab.PageID.ToString());
                        mnc.Width = mn.Width;
                        mnc = RecourseMenuShop(tabIndex, mySubTab.Pages, mnc, idShop);
                        mn.Childs.Add(mnc);
                    }
                }
            }
            return mn;
        }

        /// <summary>
        /// Add a Menu Tree Node if user in in the list of Authorized roles.
        /// Thanks to abain for fixing authorization bug.
        /// </summary>
        /// <param name="tabIndex">Index of the tab</param>
        /// <param name="myTab">Tab to add to the MenuTreeNodes collection</param>
        protected virtual void AddMenuTreeNode(int tabIndex, PageStripDetails myTab) //MH:
        {
            if (PortalSecurity.IsInRoles(myTab.AuthorizedRoles))
            {
                MenuTreeNode mn = new MenuTreeNode(myTab.PageName);

                mn.Link = giveMeUrl(myTab.PageName, myTab.PageID);
                mn.Height = Height;
                mn.Width = Width;
                mn = RecourseMenu(tabIndex, myTab.Pages, mn);
                Childs.Add(mn);
            }
        }

        /// <summary>
        /// Recourses the menu.
        /// </summary>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="t">The t.</param>
        /// <param name="mn">The mn.</param>
        /// <returns></returns>
        protected virtual MenuTreeNode RecourseMenu(int tabIndex, PagesBox t, MenuTreeNode mn) //mh:
        {
            if (t.Count > 0)
            {
                for (int c = 0; c < t.Count; c++)
                {
                    PageStripDetails mySubTab = t[c];
                    if (PortalSecurity.IsInRoles(mySubTab.AuthorizedRoles))
                    {
                        MenuTreeNode mnc = new MenuTreeNode(mySubTab.PageName);

                        mnc.Link = giveMeUrl(mySubTab.PageName, mySubTab.PageID);
                        mnc.Width = mn.Width;

                        mnc = RecourseMenu(tabIndex, mySubTab.Pages, mnc);
                        
                        mn.Childs.Add(mnc);
                    }
                }
            }
            return mn;
        }

        /// <summary>
        /// Populates ArrayList of tabs based on binding option selected.
        /// </summary>
        /// <returns></returns>
        protected object GetInnerDataSource()
        {
            ArrayList authorizedTabs = new ArrayList();

            if (HttpContext.Current != null)
            {
                // Obtain PortalSettings from Current Context
                PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

                switch (Bind)
                {
                    case BindOption.BindOptionTop:
                        {
                            authorizedTabs = GetTabs(0, portalSettings.ActivePage.PageID, portalSettings.DesktopPages);
                            break;
                        }

                    case BindOption.BindOptionCurrentChilds:
                        {
                            int currentTabRoot =
                                PortalSettings.GetRootPage(portalSettings.ActivePage, portalSettings.DesktopPages).
                                    PageID;
                            authorizedTabs =
                                GetTabs(currentTabRoot, portalSettings.ActivePage.PageID, portalSettings.DesktopPages);
                            break;
                        }

                    case BindOption.BindOptionSubtabSibling:
                        {
                            int currentTabRoot;
                            if (portalSettings.ActivePage.ParentPageID == 0)
                                currentTabRoot = portalSettings.ActivePage.PageID;
                            else
                                currentTabRoot = portalSettings.ActivePage.ParentPageID;

                            authorizedTabs =
                                GetTabs(currentTabRoot, portalSettings.ActivePage.PageID, portalSettings.DesktopPages);
                            break;
                        }

                    case BindOption.BindOptionChildren:
                        {
                            authorizedTabs =
                                GetTabs(portalSettings.ActivePage.PageID, portalSettings.ActivePage.PageID,
                                        portalSettings.DesktopPages);
                            break;
                        }

                    case BindOption.BindOptionSiblings:
                        {
                            authorizedTabs =
                                GetTabs(portalSettings.ActivePage.ParentPageID, portalSettings.ActivePage.PageID,
                                        portalSettings.DesktopPages);
                            break;
                        }

                        //MH: added 19/09/2003 by mario@hartmann.net
                    case BindOption.BindOptionTabSibling:
                        {
                            authorizedTabs =
                                GetTabs(portalSettings.ActivePage.PageID, portalSettings.ActivePage.PageID,
                                        portalSettings.DesktopPages);

                            if (authorizedTabs.Count == 0)
                                authorizedTabs =
                                    GetTabs(portalSettings.ActivePage.ParentPageID, portalSettings.ActivePage.PageID,
                                            portalSettings.DesktopPages);

                            break;
                        }

                        //MH: added 29/04/2003 by mario@hartmann.net
                    case BindOption.BindOptionDefinedParent:
                        if (ParentPageID != -1)
                            authorizedTabs =
                                GetTabs(ParentPageID, portalSettings.ActivePage.PageID, portalSettings.DesktopPages);
                        break;
                        //MH: end
                    default:
                        {
                            break;
                        }
                }
            }
            return authorizedTabs;
        }

        /// <summary>
        /// Seems to be unused - Jes1111
        /// </summary>
        /// <param name="parentPageID">The parent page ID.</param>
        /// <param name="activePageID">The active page ID.</param>
        /// <param name="allTabs">All tabs.</param>
        /// <returns></returns>
        private int GetSelectedTab(int parentPageID, int activePageID, IList allTabs)
        {
            for (int i = 0; i < allTabs.Count; i++)
            {
                PageStripDetails tmpTab = (PageStripDetails) allTabs[i];
                if (tmpTab.PageID == activePageID)
                {
                    int selectedPageID = activePageID;
                    if (tmpTab.ParentPageID != parentPageID)
                    {
                        selectedPageID = GetSelectedTab(parentPageID, tmpTab.ParentPageID, allTabs);
                        return selectedPageID;
                    }
                    else
                    {
                        return selectedPageID;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the tabs.
        /// </summary>
        /// <param name="parentID">The parent ID.</param>
        /// <param name="tabID">The tab ID.</param>
        /// <param name="Tabs">The tabs.</param>
        /// <returns></returns>
        private ArrayList GetTabs(int parentID, int tabID, IList Tabs)
        {
            ArrayList authorizedTabs = new ArrayList();
            //int index = -1;

            //MH:get the selected tab for this 
            //int selectedPageID = GetSelectedTab (parentID, tabID,Tabs);

            // Populate Tab List with authorized tabs
            for (int i = 0; i < Tabs.Count; i++)
            {
                PageStripDetails tab = (PageStripDetails) Tabs[i];

                if (tab.ParentPageID == parentID) // Get selected row only
                {
                    if (PortalSecurity.IsInRoles(tab.AuthorizedRoles))
                    {
                        //index = authorizedTabs.Add(tab);
                        authorizedTabs.Add(tab);
                    }
                }
            }
            return authorizedTabs;
        }

        /// <summary>
        /// Gets the client script path.
        /// </summary>
        /// <returns></returns>
        protected override string GetClientScriptPath()
        {
            return string.Concat(Path.ApplicationRoot, "/aspnet_client/DUEMETRI_UI_WebControls_HWMenu/1_0_0_0/");
        }
    }
}