using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using DUEMETRI.UI.WebControls.HWMenu;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
// Tiptopweb: 27 Jan 2003
// modified from MenuNavigation to replace the Category module:
// the navigation will not be effective and instead we navigate to the same page
// and transmit the PageID as a CatID to the Product list module.
// jviladiu@portalServices.net 21/07/2004: Clean code & added localization for "Shop home"

namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// Menu navigation inherits from Menu Webcontrol
    /// and adds the 'glue' to link to tabs tree.
    /// Bugfix #656794 'Menu rendering adds all tabs' by abain
    /// </summary>
    public class ShopNavigation : Menu, INavigation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShopNavigation"/> class.
        /// </summary>
        public ShopNavigation()
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
        private bool _autoBind = false;
        //MH: added 29/04/2003 by mario@hartmann.net
        private int _definedParentTab = -1;
        //MH: end

        /// <summary>
        /// Indicates if control should bind when loads
        /// </summary>
        /// <value><c>true</c> if [auto bind]; otherwise, <c>false</c>.</value>
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
        /// <value>The bind.</value>
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
        /// <value>The parent page ID.</value>
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
        /// Thanks to abain for cleaning up the code
        /// </summary>
        public override void DataBind()
        {
            bool currentTabOnly = (Bind == BindOption.BindOptionCurrentChilds);

            // Obtain PortalSettings from Current Context 
            PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

            // Build list of tabs to be shown to user 
            ArrayList authorizedTabs = new ArrayList();
            int addedTabs = 0;

            for (int i = 0; i < portalSettings.DesktopPages.Count; i++)
            {
                PageStripDetails tab = (PageStripDetails) portalSettings.DesktopPages[i];

                if (PortalSecurity.IsInRoles(tab.AuthorizedRoles))
                    authorizedTabs.Add(tab);

                addedTabs++;
            }

            //Menu 

            // add the shop home!
            AddShopHomeNode();

            if (!currentTabOnly)
            {
                for (int i = 0; i < authorizedTabs.Count; i++)
                {
                    PageStripDetails myTab = (PageStripDetails) authorizedTabs[i];
                    AddMenuTreeNode(i, myTab);
                }
            }
            else
            {
                if (authorizedTabs.Count >= 0)
                {
                    PageStripDetails myTab = PortalSettings.GetRootPage(portalSettings.ActivePage, authorizedTabs);

                    if (myTab.Pages.Count > 0)
                    {
                        for (int i = 0; i < myTab.Pages.Count; i++)
                        {
                            PageStripDetails mySubTab = (PageStripDetails) myTab.Pages[i];
                            AddMenuTreeNode(0, mySubTab);
                        }
                    }
                }
            }

            base.DataBind();
        }

        /// <summary>
        /// Adds the shop home node.
        /// </summary>
        private void AddShopHomeNode()
        {
            PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
            int tabIDShop = portalSettings.ActivePage.PageID;

            MenuTreeNode mn = new MenuTreeNode(General.GetString("PRODUCT_HOME", "Shop Home"));
            // change the link to stay on the same page and call a category product

            mn.Link = HttpUrlBuilder.BuildUrl(tabIDShop);
            mn.Width = Width;
            Childs.Add(mn);
        }

        /// <summary>
        /// Add a Menu Tree Node if user in in the list of Authorized roles.
        /// Thanks to abain for fixing authorization bug.
        /// </summary>
        /// <param name="tabIndex">Index of the tab</param>
        /// <param name="myTab">Tab to add to the MenuTreeNodes collection</param>
        private void AddMenuTreeNode(int tabIndex, PageStripDetails myTab)
        {
            if (PortalSecurity.IsInRoles(myTab.AuthorizedRoles))
            {
                // get index and id from this page and transmit them
                // Obtain PortalSettings from Current Context 
                PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
                int tabIDShop = portalSettings.ActivePage.PageID;

                MenuTreeNode mn = new MenuTreeNode(myTab.PageName);
                // change the link to stay on the same page and call a category product

                mn.Link =
                    HttpUrlBuilder.BuildUrl("~/" + HttpUrlBuilder.DefaultPage, tabIDShop, "ItemID=" + myTab.PageID);
                mn.Width = Width;
                mn = RecourseMenu(tabIDShop, myTab.Pages, mn);
                Childs.Add(mn);
            }
        }

        // modified to transmit the PageID and TabIndex for the shop page
        /// <summary>
        /// Recourses the menu.
        /// </summary>
        /// <param name="tabIDShop">The tab ID shop.</param>
        /// <param name="t">The t.</param>
        /// <param name="mn">The mn.</param>
        /// <returns></returns>
        private MenuTreeNode RecourseMenu(int tabIDShop, PagesBox t, MenuTreeNode mn)
        {
            if (t.Count > 0)
            {
                for (int c = 0; c < t.Count; c++)
                {
                    PageStripDetails mySubTab = (PageStripDetails) t[c];
                    if (PortalSecurity.IsInRoles(mySubTab.AuthorizedRoles))
                    {
                        MenuTreeNode mnc = new MenuTreeNode(mySubTab.PageName);
                        // change PageID into ItemID for the product module on the same page
                        mnc.Link =
                            HttpUrlBuilder.BuildUrl("~/" + HttpUrlBuilder.DefaultPage, tabIDShop,
                                                    "ItemID=" + mySubTab.PageID);
                        mnc.Width = mn.Width;
                        mnc = RecourseMenu(tabIDShop, mySubTab.Pages, mnc);
                        mn.Childs.Add(mnc);
                    }
                }
            }
            return mn;
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