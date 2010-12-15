using System.Web;
using DUEMETRI.UI.WebControls.HWMenu;
using Appleseed.Framework.Security;
using Appleseed.Framework.Site.Configuration;
// mario@hartmann.net: 24/07/2003
// modified from MenuNavigation
// the navigation will not be effective and instead we navigate to the same page
// and transmit the PageID as a ItemID.
//
// thierry@tiptopweb.com.au: 17/09/2003
// replace Default.aspx by DesktopDefault.aspx as we are loosing the parameters
// when transfering from Default.aspx to DesktopDefault.aspx and not using the UrlBuilder

namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// ItemNavigation inherits from MenuNavigation
    /// and adds the functionality of the ShopNavigation with ALL types of binding.
    /// all subcategories are added as an ItemID property.
    /// </summary>
    public class ItemNavigation : MenuNavigation
    {
        /// <summary>
        /// Do databind.
        /// </summary>
        public override void DataBind()
        {
            // add the root!
            AddRootNode();

            base.DataBind();
        }


        /// <summary>
        /// Add the current tab as top menu item.
        /// </summary>
        private void AddRootNode()
        {
            PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
            PageSettings tabItemsRoot = portalSettings.ActivePage;

            using (MenuTreeNode mn = new MenuTreeNode(tabItemsRoot.PageName))
            {
                // change the link to stay on the same page and call a category product
                mn.Link = HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", tabItemsRoot.PageID);
                mn.Width = Width;
                Childs.Add(mn);
            }
        }


        /// <summary>
        /// Add a Menu Tree Node if user in in the list of Authorized roles.
        /// Thanks to abain for fixing authorization bug.
        /// </summary>
        /// <param name="tabIndex">Index of the tab</param>
        /// <param name="myTab">Tab to add to the MenuTreeNodes collection</param>
        protected override void AddMenuTreeNode(int tabIndex, PageStripDetails myTab)
        {
            if (PortalSecurity.IsInRoles(myTab.AuthorizedRoles))
            {
                // get index and id from this page and transmit them
                // Obtain PortalSettings from Current Context 
                PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
                int tabIDItemsRoot = portalSettings.ActivePage.PageID;

                MenuTreeNode mn = new MenuTreeNode(myTab.PageName);

                // change the link to stay on the same page and call a category product
                mn.Link = HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", tabIDItemsRoot, "ItemID=" + myTab.PageID);
                //fixed by manu
                mn.Width = Width;
                mn = RecourseMenu(tabIDItemsRoot, myTab.Pages, mn);
                Childs.Add(mn);
            }
        }


        /// <summary>
        /// modified to transmit the PageID and TabIndex for the item page
        /// </summary>
        /// <param name="tabIDItemsRoot">The tab ID items root.</param>
        /// <param name="t">The t.</param>
        /// <param name="mn">The mn.</param>
        /// <returns></returns>
        protected override MenuTreeNode RecourseMenu(int tabIDItemsRoot, PagesBox t, MenuTreeNode mn)
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
                            HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", tabIDItemsRoot, "ItemID=" + mySubTab.PageID);
                        //by manu
                        mnc.Width = mn.Width;
                        mnc = RecourseMenu(tabIDItemsRoot, mySubTab.Pages, mnc);
                        mn.Childs.Add(mnc);
                    }
                }
            }
            return mn;
        }
    }
}