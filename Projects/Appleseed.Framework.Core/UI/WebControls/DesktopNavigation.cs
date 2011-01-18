using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework.Security;
using Appleseed.Framework.Site.Configuration;

namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// Represents a flat navigation bar. 
    /// One dimension. 
    /// Can render horizontally or vertically.
    /// </summary>
    public class DesktopNavigation : DataList, INavigation
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DesktopNavigation()
        {
            EnableViewState = false;
            RepeatDirection = RepeatDirection.Horizontal;
            Load += new EventHandler(LoadControl);
        }

        /// <summary>
        /// Loads the control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void LoadControl(object sender, EventArgs e)
        {
            if (AutoBind)
                DataBind();
        }

        private RepeatDirection rd = RepeatDirection.Horizontal;

        /// <summary>
        /// Gets or sets whether the <see cref="T:System.Web.UI.WebControls.DataList"></see> control displays vertically or horizontally.
        /// </summary>
        /// <value></value>
        /// <returns>One of the <see cref="T:System.Web.UI.WebControls.RepeatDirection"></see> values. The default is Vertical.</returns>
        /// <exception cref="T:System.ArgumentException">The specified value is not one of the <see cref="T:System.Web.UI.WebControls.RepeatDirection"></see> values. </exception>
        [
            DefaultValue(RepeatDirection.Horizontal)
            ]
        public override RepeatDirection RepeatDirection
        {
            get { return rd; }
            set { rd = value; }
        }

        /// <summary>
        /// Gives the me URL.
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public string giveMeUrl(string tab, int id)
        {
            if (!UseTabNameInUrl) return HttpUrlBuilder.BuildUrl(id);
            string auxtab = string.Empty;
            foreach (char c in tab)
                if (char.IsLetterOrDigit(c)) auxtab += c;
                else auxtab += "_";
            return HttpUrlBuilder.BuildUrl("~/" + auxtab + ".aspx", id);
        }

        #region INavigation implementation

        private bool _useTabNameInUrl = false;

        /// <summary>
        /// Indicates if control show the tabname in the url
        /// </summary>
        /// <value><c>true</c> if [use tab name in URL]; otherwise, <c>false</c>.</value>
        [Category("Data"), PersistenceMode(PersistenceMode.Attribute)]
        public bool UseTabNameInUrl
        {
            get { return _useTabNameInUrl; }
            set { _useTabNameInUrl = value; }
        }

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

        private object innerDataSource = null;

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
//						int tmpPageID = 0;
//
//						if(portalSettings.ActivePage.ParentPageID == 0)
//						{
//							tmpPageID = portalSettings.ActivePage.PageID;
//						}
//						else
//						{
//							tmpPageID = portalSettings.ActivePage.ParentPageID;
//						}
//						ArrayList parentTabs = GetTabs(tmpPageID, portalSettings.DesktopPages);
//						try
//						{
//							if (parentTabs.Count > 0)
//							{
//								PageStripDetails currentParentTab = (PageStripDetails) parentTabs[this.SelectedIndex];
//								this.SelectedIndex = -1;
//								authorizedTabs = GetTabs(portalSettings.ActivePage.PageID, currentParentTab.Pages);
//							}
//						}
//						catch
//						{}
//						break;
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
        /// Gets the selected tab.
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
            int index = -1;

            //MH:get the selected tab for this 
            int selectedPageID = GetSelectedTab(parentID, tabID, Tabs);


            // Populate Tab List with authorized tabs
            for (int i = 0; i < Tabs.Count; i++)
            {
                PageStripDetails tab = (PageStripDetails) Tabs[i];

                if (tab.ParentPageID == parentID) // Get selected row only
                {
                    if (PortalSecurity.IsInRoles(tab.AuthorizedRoles))
                    {
                        index = authorizedTabs.Add(tab);

                        //MH:if (tab.PageID == tabID)
                        //MH:added to support the selected menutab in each level
                        if (tab.PageID == selectedPageID)
                            SelectedIndex = index;
                    }
                }
            }
            return authorizedTabs;
        }

        /// <summary>
        /// DataSource
        /// </summary>
        public override object DataSource
        {
            get
            {
                if (innerDataSource == null)
                {
                    innerDataSource = GetInnerDataSource();
                }
                return innerDataSource;
            }
            set { innerDataSource = value; }
        }
    }
}