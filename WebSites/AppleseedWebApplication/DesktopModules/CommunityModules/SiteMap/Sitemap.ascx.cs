using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Content.Web.Modules.Sitemap;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI.WebControls;
using History=Appleseed.Framework.History;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    ///	Summary description for Sitemap.
    /// </summary>
    [History("jminond", "march 2005", "Changes for moving Tab to Page")]
    public partial class Sitemaps : PortalModuleControl, INavigation
    {
        /// <summary>
        /// 
        /// </summary>
        private SitemapItems list;

        /// <summary>
        /// Controls which tab is displayed. When 0 the current tab is displyed.
        /// </summary>
        protected int showTabID;

        /// <summary>
        /// Sitemap Constructor
        /// </summary>
        public Sitemaps()
        {
            //Bind to Tab setting
            SettingItem BindToTab = new SettingItem(new BooleanDataType());
            BindToTab.Value = "false";
            _baseSettings.Add("BindToTab", BindToTab);

            SettingItem showTabID = new SettingItem(new IntegerDataType());
            showTabID.Required = true;
            showTabID.Value = "0";
            showTabID.MinValue = 0;
            showTabID.MaxValue = int.MaxValue;
            _baseSettings.Add("ShowTabID", showTabID);

            SettingItem NodeIcon = new SettingItem(new StringDataType());
            NodeIcon.EnglishName = "Node Icon";
            NodeIcon.Required = false;
            NodeIcon.Order = 5;
            NodeIcon.Value = "sm_node.gif";
            _baseSettings.Add("NodeIcon", NodeIcon);

            SettingItem RootIcon = new SettingItem(new StringDataType());
            RootIcon.EnglishName = "Root Icon";
            RootIcon.Required = false;
            RootIcon.Order = 6;
            RootIcon.Value = "sm_rootnode.gif";
            _baseSettings.Add("RootIcon", RootIcon);

            SettingItem IconWidth = new SettingItem(new IntegerDataType());
            IconWidth.Required = true;
            IconWidth.Value = "20";
            IconWidth.MinValue = 0;
            IconWidth.MaxValue = int.MaxValue;
            _baseSettings.Add("IconWidth", IconWidth);

            SettingItem IconHeight = new SettingItem(new IntegerDataType());
            IconHeight.Required = true;
            IconHeight.Value = "20";
            IconHeight.MinValue = 0;
            IconHeight.MaxValue = int.MaxValue;
            _baseSettings.Add("IconHeight", IconHeight);

            // no viewstate needed
            EnableViewState = false;

            //init member variables
            list = new SitemapItems();
            _autoBind = true;
            _bind = BindOption.BindOptionNone;
        }

        #region Databinding region

        /// <summary>
        /// Do databind.
        /// </summary>
        public override void DataBind()
        {
            MakeSitemap();
        }

        #endregion

        #region INavigation implementation 

        private BindOption _bind = BindOption.BindOptionTop;
        private bool _autoBind = false;
        //MH: added 27/05/2003 by mario@hartmann.net
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

        //MH: added 27/05/2003 by mario@hartmann.net
        /// <summary>
        /// Defines the ParentTabID when using BindOptionDefinedParent
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
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            // if runtime render the sitemap, else show the placeholder
            if (HttpContext.Current != null)
            {
                string imgFolder = Path.WebPathCombine(Path.ApplicationRoot, "Design/Themes/Default/img/");
                // create the table renderer and set it's properties.
                TableSitemapRenderer tableRenderer = new TableSitemapRenderer();
                if (Settings["RootIcon"].ToString().Length > 0)
                    tableRenderer.ImageRootNodeUrl = imgFolder + Settings["RootIcon"].ToString();
                else
                    tableRenderer.ImageRootNodeUrl = Page.CurrentTheme.GetModuleImageSRC("sm_rootnode.gif");
                //tableRenderer.ImageRootNodeUrl	= this.Page.CurrentTheme.GetImage("SiteMap_Root", imgFolder+"sm_rootnode.gif").ImageUrl;

                if (Settings["NodeIcon"].ToString().Length > 0)
                    tableRenderer.ImageNodeUrl = imgFolder + Settings["NodeIcon"].ToString();
                else
                    tableRenderer.ImageRootNodeUrl = Page.CurrentTheme.GetModuleImageSRC("sm_node.gif");
                //tableRenderer.ImageRootNodeUrl	= this.Page.CurrentTheme.GetImage("SiteMap_Node", imgFolder+"sm_node.gif").ImageUrl;

                tableRenderer.ImageSpacerUrl = Page.CurrentTheme.GetModuleImageSRC("sm_Spacer.gif");
                //tableRenderer.ImageSpacerUrl		= this.Page.CurrentTheme.GetImage("SiteMap_Spacer", imgFolder+"sm_Spacer.gif").ImageUrl;
                tableRenderer.ImageStraightLineUrl = Page.CurrentTheme.GetModuleImageSRC("sm_line_I.gif");
                //tableRenderer.ImageStraightLineUrl	= this.Page.CurrentTheme.GetImage("SiteMap_I", imgFolder+"sm_line_I.gif").ImageUrl;
                tableRenderer.ImageCrossedLineUrl = Page.CurrentTheme.GetModuleImageSRC("sm_line_T.gif");
                //tableRenderer.ImageCrossedLineUrl	= this.Page.CurrentTheme.GetImage("SiteMap_t", imgFolder+"sm_line_T.gif").ImageUrl;
                tableRenderer.ImageLastNodeLineUrl = Page.CurrentTheme.GetModuleImageSRC("sm_line_L.gif");
                //tableRenderer.ImageLastNodeLineUrl	= this.Page.CurrentTheme.GetImage("SiteMap_l", imgFolder+"sm_line_L.gif").ImageUrl;
                tableRenderer.ImagesHeight = Int32.Parse(Settings["IconHeight"].ToString());
                ;
                tableRenderer.ImagesWidth = Int32.Parse(Settings["IconWidth"].ToString());
                ;
                tableRenderer.TableWidth = new Unit(98, UnitType.Percentage);
                tableRenderer.CssStyle = "SiteMapItem";

                PlaceHolder1.Controls.Add(tableRenderer.Render(list));
            }
            else
            {
                Table t = new Table();
                TableRow r = new TableRow();
                TableCell c = new TableCell();
                c.Text = "Placeholder for Sitemap";
                t.BorderWidth = 1;
                r.Cells.Add(c);
                t.CellPadding = 0;
                t.CellSpacing = 0;
                t.Width = new Unit(98, UnitType.Percentage);
                t.Rows.Add(r);
                PlaceHolder1.Controls.Add(t);
            }
        }

        #region Sitemap creation region

        /// <summary>
        /// This function creates from the PortalSettings structure a list with the tabs in the right order
        /// </summary>
        private void MakeSitemap()
        {
            bool currentTabOnly = (Bind == BindOption.BindOptionCurrentChilds);

            PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

            int level = 0;

            // Add Portal Root when showing all tabs
            if (!currentTabOnly)
            {
                list.Add(new SitemapItem(0, portalSettings.PortalName, HttpUrlBuilder.BuildUrl("~/"), 0));
                level++;
            }

            // Now loop all tabs to find the right ones to init the recursive functions
            for (int i = 0; i < portalSettings.DesktopPages.Count; ++i)
            {
                PageStripDetails tab = (PageStripDetails) portalSettings.DesktopPages[i];

                //if showing all tabs, look for root tabs
                if (!currentTabOnly)
                {
                    if (tab.ParentPageID == 0)
                    {
                        RecurseSitemap(tab, level);
                    }
                }
                else
                {
                    //else find the current tab
                    int tabID = (showTabID > 0) ? showTabID : portalSettings.ActivePage.PageID;
                    if (tab.PageID == tabID)
                    {
                        RecurseSitemap(tab, level);
                    }
                }
            }
        }

        /// <summary>
        /// This is the recursive function to add all tabs with it's child tabs
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="level">The level.</param>
        private void RecurseSitemap(PageStripDetails tab, int level)
        {
            //only add tabs we have access to
            if (PortalSecurity.IsInRoles(tab.AuthorizedRoles))
            {
                //first add the tab, then add it's children
                list.Add(new SitemapItem(tab.PageID, tab.PageName, HttpUrlBuilder.BuildUrl(tab.PageID), level));

                for (int i = 0; i < tab.Pages.Count; ++i)
                {
                    RecurseSitemap(tab.Pages[i], level + 1);
                }
            }
        }

        #endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            showTabID = Int32.Parse(Settings["ShowTabID"].ToString());

            if (bool.Parse(Settings["BindToTab"].ToString()))
            {
                Bind = BindOption.BindOptionCurrentChilds;
            }
            else
            {
                Bind = BindOption.BindOptionNone;
            }

            if (AutoBind)
            {
                DataBind();
            }
        }

        /// <summary>
        /// General Module Def GUID
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{429A98E3-7A07-4d9a-A578-3ED8DD158306}"); }
        }

        #region Install/Uninstall

        /// <summary>
        /// Install
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            //nothing to do yet, just call base
            base.Install(stateSaver);
        }

        /// <summary>
        /// Uninstall
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Uninstall(IDictionary stateSaver)
        {
            //nothing to do yet, just call base
            base.Uninstall(stateSaver);
        }

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// Module OnInit
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);

            if (!this.Page.IsCssFileRegistered("Mod_SiteMap"))
                this.Page.RegisterCssFile("Mod_SiteMap");

            base.OnInit(e);
        }

        #endregion
    }
}