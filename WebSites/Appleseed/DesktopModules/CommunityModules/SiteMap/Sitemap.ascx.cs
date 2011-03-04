// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sitemap.ascx.cs" company="--">
//   Copyright © -- 2011. All Rights Reserved.
// </copyright>
// <summary>
//   The sitemaps.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Content.Web.Modules
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Appleseed.Content.Web.Modules.Sitemap;
    using Appleseed.Framework;
    using Appleseed.Framework.Security;
    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Site.Configuration;
    using Appleseed.Framework.Web.UI.WebControls;

    /// <summary>
    /// The sitemaps.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [History("jminond", "march 2005", "Changes for moving Tab to Page")]
    public partial class Sitemaps : PortalModuleControl, INavigation
    {
        #region Constants and Fields

        /// <summary>
        ///   Controls which tab is displayed. When 0 the current tab is displayed.
        /// </summary>
        protected int showTabID;

        /// <summary>
        /// The list.
        /// </summary>
        private readonly List<SitemapItem> list;

        /// <summary>
        /// The bind.
        /// </summary>
        private BindOption bind = BindOption.BindOptionTop;

        // MH: added 27/05/2003 by mario@hartmann.net
        /// <summary>
        /// The defined parent tab.
        /// </summary>
        private int definedParentTab = -1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Sitemaps" /> class.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public Sitemaps()
        {
            // Bind to Tab setting
            var bindToTab = new SettingItem<bool, CheckBox> { Value = false };
            this.BaseSettings.Add("BindToTab", bindToTab);

            var showTabIDSetting = new SettingItem<int, TextBox> { Required = true, Value = 0, MinValue = 0, MaxValue = int.MaxValue };
            this.BaseSettings.Add("ShowTabID", showTabIDSetting);

            var nodeIcon = new SettingItem<string, TextBox> { EnglishName = "Node Icon", Required = false, Order = 5, Value = "sm_node.gif" };
            this.BaseSettings.Add("NodeIcon", nodeIcon);

            var rootIcon = new SettingItem<string, TextBox> { EnglishName = "Root Icon", Required = false, Order = 6, Value = "sm_rootnode.gif" };
            this.BaseSettings.Add("RootIcon", rootIcon);

            var iconWidth = new SettingItem<int, TextBox> { Required = true, Value = 20, MinValue = 0, MaxValue = int.MaxValue };
            this.BaseSettings.Add("IconWidth", iconWidth);

            var iconHeight = new SettingItem<int, TextBox> { Required = true, Value = 20, MinValue = 0, MaxValue = int.MaxValue };
            this.BaseSettings.Add("IconHeight", iconHeight);

            // no view state needed
            base.EnableViewState = false;

            // init member variables
            this.list = new List<SitemapItem>();
            this.AutoBind = true;
            this.bind = BindOption.BindOptionNone;
        }

        #endregion

        // MH: end
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether control should bind when loads
        /// </summary>
        /// <value><c>true</c> if [auto bind]; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        [Category("Data")]
        [PersistenceMode(PersistenceMode.Attribute)]
        public bool AutoBind { get; set; }

        /// <summary>
        ///   Gets or sets how this control should bind to db data
        /// </summary>
        /// <value>The bind.</value>
        /// <remarks>
        /// </remarks>
        [Category("Data")]
        [PersistenceMode(PersistenceMode.Attribute)]
        public BindOption Bind
        {
            get
            {
                return this.bind;
            }

            set
            {
                this.bind = value;
            }
        }

        // MH: added 27/05/2003 by mario@hartmann.net

        /// <summary>
        ///   General Module Def GUID
        /// </summary>
        /// <remarks>
        /// </remarks>
        public override Guid GuidID
        {
            get
            {
                return new Guid("{429A98E3-7A07-4d9a-A578-3ED8DD158306}");
            }
        }

        /// <summary>
        ///   Gets or sets the ParentTabID when using BindOptionDefinedParent
        /// </summary>
        /// <value>The parent page ID.</value>
        /// <remarks>
        /// </remarks>
        [Category("Data")]
        [PersistenceMode(PersistenceMode.Attribute)]
        public int ParentPageID
        {
            get
            {
                return this.definedParentTab;
            }

            set
            {
                this.definedParentTab = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        /// <remarks></remarks>
        public override void DataBind()
        {
            this.MakeSitemap();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        /// <remarks>
        /// </remarks>
        protected override void CreateChildControls()
        {
            // if runtime render the sitemap, else show the placeholder
            if (HttpContext.Current != null)
            {
                var imgFolder = Path.WebPathCombine(Path.ApplicationRoot, "Design/Themes/Default/img/");

                // create the table renderer and set it's properties.
                var tableRenderer = new TableSitemapRenderer();
                if (this.Settings["RootIcon"].ToString().Length > 0)
                {
                    tableRenderer.ImageRootNodeUrl = imgFolder + this.Settings["RootIcon"].ToString();
                }
                else
                {
                    tableRenderer.ImageRootNodeUrl = this.Page.CurrentTheme.GetModuleImageSRC("sm_rootnode.gif");
                }

                // tableRenderer.ImageRootNodeUrl	= this.Page.CurrentTheme.GetImage("SiteMap_Root", imgFolder+"sm_rootnode.gif").ImageUrl;
                if (this.Settings["NodeIcon"].ToString().Length > 0)
                {
                    tableRenderer.ImageNodeUrl = imgFolder + this.Settings["NodeIcon"].ToString();
                }
                else
                {
                    tableRenderer.ImageRootNodeUrl = this.Page.CurrentTheme.GetModuleImageSRC("sm_node.gif");
                }

                // tableRenderer.ImageRootNodeUrl	= this.Page.CurrentTheme.GetImage("SiteMap_Node", imgFolder+"sm_node.gif").ImageUrl;
                tableRenderer.ImageSpacerUrl = this.Page.CurrentTheme.GetModuleImageSRC("sm_Spacer.gif");

                // tableRenderer.ImageSpacerUrl		= this.Page.CurrentTheme.GetImage("SiteMap_Spacer", imgFolder+"sm_Spacer.gif").ImageUrl;
                tableRenderer.ImageStraightLineUrl = this.Page.CurrentTheme.GetModuleImageSRC("sm_line_I.gif");

                // tableRenderer.ImageStraightLineUrl	= this.Page.CurrentTheme.GetImage("SiteMap_I", imgFolder+"sm_line_I.gif").ImageUrl;
                tableRenderer.ImageCrossedLineUrl = this.Page.CurrentTheme.GetModuleImageSRC("sm_line_T.gif");

                // tableRenderer.ImageCrossedLineUrl	= this.Page.CurrentTheme.GetImage("SiteMap_t", imgFolder+"sm_line_T.gif").ImageUrl;
                tableRenderer.ImageLastNodeLineUrl = this.Page.CurrentTheme.GetModuleImageSRC("sm_line_L.gif");

                // tableRenderer.ImageLastNodeLineUrl	= this.Page.CurrentTheme.GetImage("SiteMap_l", imgFolder+"sm_line_L.gif").ImageUrl;
                tableRenderer.ImagesHeight = Int32.Parse(this.Settings["IconHeight"].ToString());
                tableRenderer.ImagesWidth = Int32.Parse(this.Settings["IconWidth"].ToString());
                tableRenderer.TableWidth = new Unit(98, UnitType.Percentage);
                tableRenderer.CssStyle = "SiteMapItem";

                this.PlaceHolder1.Controls.Add(tableRenderer.Render(this.list));
            }
            else
            {
                var t = new Table();
                var r = new TableRow();
                var c = new TableCell { Text = "Placeholder for Sitemap" };
                t.BorderWidth = 1;
                r.Cells.Add(c);
                t.CellPadding = 0;
                t.CellSpacing = 0;
                t.Width = new Unit(98, UnitType.Percentage);
                t.Rows.Add(r);
                this.PlaceHolder1.Controls.Add(t);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        /// <remarks></remarks>
        protected override void OnInit(EventArgs e)
        {
            if (!this.Page.IsCssFileRegistered("Mod_SiteMap"))
            {
                this.Page.RegisterCssFile("Mod_SiteMap");
            }

            this.showTabID = Int32.Parse(this.Settings["ShowTabID"].ToString());

            this.Bind = bool.Parse(this.Settings["BindToTab"].ToString())
                            ? BindOption.BindOptionCurrentChilds
                            : BindOption.BindOptionNone;

            if (this.AutoBind)
            {
                this.DataBind();
            }

            base.OnInit(e);
        }

        /// <summary>
        /// This function creates from the PortalSettings structure a list with the tabs in the right order
        /// </summary>
        /// <remarks>
        /// </remarks>
        private void MakeSitemap()
        {
            var currentTabOnly = this.Bind == BindOption.BindOptionCurrentChilds;

            var portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];

            var level = 0;

            // Add Portal Root when showing all tabs
            if (!currentTabOnly)
            {
                this.list.Add(new SitemapItem(0, portalSettings.PortalName, HttpUrlBuilder.BuildUrl("~/"), 0));
                level++;
            }

            // Now loop all tabs to find the right ones to init the recursive functions
            foreach (var tab in portalSettings.DesktopPages)
            {
                // if showing all tabs, look for root tabs
                if (!currentTabOnly)
                {
                    if (tab.ParentPageID == 0)
                    {
                        this.RecurseSitemap(tab, level);
                    }
                }
                else
                {
                    // else find the current tab
                    var tabID = (this.showTabID > 0) ? this.showTabID : portalSettings.ActivePage.PageID;
                    if (tab.PageID == tabID)
                    {
                        this.RecurseSitemap(tab, level);
                    }
                }
            }
        }

        /// <summary>
        /// This is the recursive function to add all tabs with it's child tabs
        /// </summary>
        /// <param name="tab">
        /// The tab.
        /// </param>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <remarks>
        /// </remarks>
        private void RecurseSitemap(PageStripDetails tab, int level)
        {
            // only add tabs we have access to
            if (PortalSecurity.IsInRoles(tab.AuthorizedRoles))
            {
                // first add the tab, then add it's children
                this.list.Add(new SitemapItem(tab.PageID, tab.PageName, HttpUrlBuilder.BuildUrl(tab.PageID), level));

                foreach (PageStripDetails t in tab.Pages)
                {
                    this.RecurseSitemap(t, level + 1);
                }
            }
        }

        #endregion
    }
}