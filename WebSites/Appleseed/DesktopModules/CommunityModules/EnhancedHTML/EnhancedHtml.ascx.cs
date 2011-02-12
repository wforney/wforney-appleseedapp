// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnhancedHtml.ascx.cs" company="--">
//   Copyright © -- 2011. All Rights Reserved.
// </copyright>
// <summary>
//   Appleseed EnhancedHtml Module
//   Written by: José Viladiu, jviladiu@portalServices.net
//   This module show a list of pages with navigation control
//   and language control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Content.Web.Modules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Appleseed.Framework;
    using Appleseed.Framework.Content.Data;
    using Appleseed.Framework.Data;
    using Appleseed.Framework.DataTypes;
    using Appleseed.Framework.Helpers;
    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Settings.Cache;
    using Appleseed.Framework.Site.Configuration;
    using Appleseed.Framework.Web.UI.WebControls;

    /// <summary>
    /// Appleseed EnhancedHtml Module
    ///   Written by: José Viladiu, jviladiu@portalServices.net
    ///   This module show a list of pages with navigation control
    ///   and language control.
    /// </summary>
    [History("jminond", "march 2005", "Changes for moving Tab to Page")]
    [History("Hongwei Shen", "September 2005", "fix the module specific setting grouping problem")]
    public partial class EnhancedHtml : PortalModuleControl
    {
        #region Constants and Fields

        /// <summary>
        /// The token module.
        /// </summary>
        private const string TokenModule = "#MODULE#";

        /// <summary>
        /// The token portal module.
        /// </summary>
        private const string TokenPortalModule = "#PORTALMODULE#";

        /// <summary>
        /// The eh page id.
        /// </summary>
        private string ehPageId;

        /// <summary>
        /// The current menu separator.
        /// </summary>
        private string currentMenuSeparator;

        /// <summary>
        /// The current mode url.
        /// </summary>
        private string currentModeUrl;

        /// <summary>
        /// The current url.
        /// </summary>
        private string currentUrl;

        /// <summary>
        /// The mode id.
        /// </summary>
        private string modeId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalModuleControl"/> class.
        /// </summary>
        /// <remarks></remarks>
        public EnhancedHtml()
        {
            SettingItemGroup _Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;

            // modified by Hongwei Shen(hongwei.shen@gmail.com) 12/9/2005
            // _groupOrderBase = 0;
            int groupOrderBase = (int)_Group;

            // end of modification
            HtmlEditorDataType.HtmlEditorSettings(this._baseSettings, _Group);

            var showTitlePage = new SettingItem<bool, CheckBox>()
                {
                    Value = false,
                    Order = groupOrderBase + 20,
                    Group = _Group,
                    EnglishName = "Show Title Page?",
                    Description = "Mark this if you like see the Title Page"
                };
            this._baseSettings.Add("ENHANCEDHTML_SHOWTITLEPAGE", showTitlePage);

            var showUpMenu = new SettingItem<bool, CheckBox>()
                {
                    Value = false,
                    Order = groupOrderBase + 25,
                    Group = _Group,
                    EnglishName = "Show Index Menu?",
                    Description = "Mark this if you would like to see an index menu with the titles of all pages"
                };
            this._baseSettings.Add("ENHANCEDHTML_SHOWUPMENU", showUpMenu);

            var alignUpMenu = new List<SettingOption>
                {
                    new SettingOption(1, General.GetString("LEFT", "Left")),
                    new SettingOption(2, General.GetString("CENTER", "Center")),
                    new SettingOption(3, General.GetString("RIGHT", "Right"))
                };

            var labelAlignUpMenu =
                new SettingItem<string, ListControl>(new CustomListDataType(alignUpMenu, "Name", "Val"))
                    {
                        Description = "Select here the align for index menu",
                        EnglishName = "Align Index Menu",
                        Value = "1",
                        Order = groupOrderBase + 30
                    };
            this._baseSettings.Add("ENHANCEDHTML_ALIGNUPMENU", labelAlignUpMenu);

            var showDownMenu = new SettingItem<bool, CheckBox>()
                {
                    Value = true,
                    Order = groupOrderBase + 40,
                    Group = _Group,
                    EnglishName = "Show Navigation Menu?",
                    Description = "Mark this if you like see a navigation menu with previous and next page"
                };
            this._baseSettings.Add("ENHANCEDHTML_SHOWDOWNMENU", showDownMenu);

            var alignDownMenu = new List<SettingOption>
                {
                    new SettingOption(1, General.GetString("LEFT", "Left")), 
                    new SettingOption(2, General.GetString("CENTER", "Center")), 
                    new SettingOption(3, General.GetString("RIGHT", "Right"))
                };

            var labelAlignDownMenu =
                new SettingItem<string, ListControl>(new CustomListDataType(alignDownMenu, "Name", "Val"))
                    {
                        Description = "Select here the align for index menu",
                        EnglishName = "Align Navigation Menu",
                        Value = "3",
                        Order = groupOrderBase + 50
                    };
            this._baseSettings.Add("ENHANCEDHTML_ALIGNDOWNMENU", labelAlignDownMenu);

            var addInvariant = new SettingItem<bool, CheckBox>()
                {
                    Value = true,
                    Order = groupOrderBase + 60,
                    Group = _Group,
                    EnglishName = "Add Invariant Culture?",
                    Description =
                        "Mark this if you like see pages with invariant culture after pages with actual culture code"
                };
            this._baseSettings.Add("ENHANCEDHTML_ADDINVARIANTCULTURE", addInvariant);

            var showMultiMode = new SettingItem<bool, CheckBox>()
                {
                    Value = true,
                    Order = groupOrderBase + 70,
                    Group = _Group,
                    EnglishName = "Show Multi-Mode icon?",
                    Description = "Mark this if you like see icon multimode page"
                };
            this._baseSettings.Add("ENHANCEDHTML_SHOWMULTIMODE", showMultiMode);

            var getContentsFromPortals = new SettingItem<bool, CheckBox>()
                {
                    Value = false,
                    Order = groupOrderBase + 80,
                    Group = _Group,
                    EnglishName = "Get contents from others Portals?",
                    Description = "Mark this if you like get contents from modules in others portals in the same database"
                };
            this._baseSettings.Add("ENHANCEDHTML_GET_CONTENTS_FROM_PORTALS", getContentsFromPortals);

            this.SupportsWorkflow = true;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get
            {
                return new Guid("{875254B7-2471-491f-BAF8-4AFC261CC224}");
            }
        }

        /// <summary>
        ///   Searchable module
        /// </summary>
        /// <value></value>
        public override bool Searchable
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Installs the specified state saver.
        /// </summary>
        /// <param name="stateSaver">The state saver.</param>
        /// <remarks></remarks>
        public override void Install(IDictionary stateSaver)
        {
            var currentScriptName = System.IO.Path.Combine(
                this.Server.MapPath(this.TemplateSourceDirectory), "install.sql");
            var errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                throw new Exception("Error occurred:" + errors[0]);
            }
        }

        /// <summary>
        /// Searchable module implementation
        /// </summary>
        /// <param name="portalId">
        /// The portal ID
        /// </param>
        /// <param name="userId">
        /// ID of the user is searching
        /// </param>
        /// <param name="searchString">
        /// The text to search
        /// </param>
        /// <param name="searchField">
        /// The fields where performing the search
        /// </param>
        /// <returns>
        /// The SELECT SQL to perform a search on the current module
        /// </returns>
        public override string SearchSqlSelect(int portalId, int userId, string searchString, string searchField)
        {
            // For better performance is necessary to add in 
            // method FillPortalDS from PortalSearch.ascx.cs the
            // next case:
            // case "875254B7-2471-491F-BAF8-4AFC261CC224":  //EnhancedHtml
            // strLink = HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", Convert.ToInt32(strTabID), strLocate);
            // break;
            var dbTable = "rb_EnhancedHtml";
            if (this.Version == WorkFlowVersion.Staging)
            {
                dbTable += "_st";
            }

            var s = new SearchDefinition(dbTable, "Title", "DesktopHtml", "CreatedByUser", "CreatedDate", searchField);
            var retorno = s.SearchSqlSelect(portalId, userId, searchString);
            if (HttpContext.Current != null && HttpContext.Current.Items["PortalSettings"] != null)
            {
                var pS = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
                retorno += string.Format(
                    " AND ((itm.CultureCode = '{0}') OR (itm.CultureCode = '{1}'))",
                    pS.PortalUILanguage.LCID,
                    CultureInfo.InvariantCulture.LCID);
            }

            return retorno;
        }

        /// <summary>
        /// Uninstalls the specified state saver.
        /// </summary>
        /// <param name="stateSaver">The state saver.</param>
        /// <remarks></remarks>
        public override void Uninstall(IDictionary stateSaver)
        {
            var currentScriptName = System.IO.Path.Combine(
                this.Server.MapPath(this.TemplateSourceDirectory), "uninstall.sql");
            var errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                throw new Exception(string.Format("Error occurred:{0}", errors[0]));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        /// <remarks></remarks>
        protected override void OnInit(EventArgs e)
        {
            this.Load += this.Page_Load;

            if (!this.Page.IsCssFileRegistered("Mod_EnhancedHtml"))
            {
                this.Page.RegisterCssFile("Mod_EnhancedHtml");
            }

            this.EditUrl = "~/DesktopModules/CommunityModules/EnhancedHtml/EnhancedHtmlEdit.aspx";
            base.OnInit(e);
        }

        /// <summary>
        /// The Page_Load event handler on this User Control is
        ///   used to render blocks of HTML or text to the page.
        ///   The text/HTML to render is stored in the EnhancedHtml
        ///   database table.  This method uses the Appleseed.EnhancedHtmlDB()
        ///   data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> instance containing the event data.
        /// </param>
        private void Page_Load(object sender, EventArgs e)
        {
            var addInvariantCulture = bool.Parse(this.Settings["ENHANCEDHTML_ADDINVARIANTCULTURE"].ToString());
            var showTitle = bool.Parse(this.Settings["ENHANCEDHTML_SHOWTITLEPAGE"].ToString());
            this.estableceParametros();
            var paginas = this.giveMePages(addInvariantCulture);
            if (paginas.Rows.Count == 0)
            {
                return;
            }

            if (this.modeId == "1")
            {
                

                var ehMultiPageMode = General.GetString("ENHANCEDHTML_MULTIPAGEMODE", "Multi Page Mode");

                foreach (DataRow fila in paginas.Rows)
                {
                    this.Content = fila["DesktopHtml"];
                    var i = int.Parse((string)fila["ItemID"]);
                    var aux = "<p/><table width='100%' border='0' cellpadding='0' cellspacing='0'><tr>";

                    if (showTitle)
                    {
                        aux += "<br /><td width='100%' align='left'><span class='EnhancedHtmlTitlePage'>" +
                               fila["Title"] + "</span><hr></td>";
                    }

                    // Not show navigation in Print Page
                    if (paginas.Rows.Count > 1 && !this.NamingContainer.ToString().Equals("ASP.print_aspx"))
                    {
                        aux += "<td align='right'><a href=" + this.dameUrl("&ModeID=0&EhPageID=" + fila["ItemID"]) + ">";
                        aux += "<img src='" + Path.ApplicationRoot + "/DesktopModules/EnhancedHTML/multipage.gif' alt='" +
                               ehMultiPageMode + "' border='0' /></a></td>";
                    }

                    aux += "</tr></table>";

                    this.HtmlHolder.Controls.Add(new LiteralControl(aux));
                    this.HtmlHolder.Controls.Add(this.toShow(this.Content.ToString()));
                }
            }
            else
            {
                

                var showMultiMode = bool.Parse(this.Settings["ENHANCEDHTML_SHOWMULTIMODE"].ToString()) &&
                                    paginas.Rows.Count > 1;
                var showUpmenu = bool.Parse(this.Settings["ENHANCEDHTML_SHOWUPMENU"].ToString()) &&
                                 paginas.Rows.Count > 1;
                var showDownmenu = bool.Parse(this.Settings["ENHANCEDHTML_SHOWDOWNMENU"].ToString()) &&
                                   paginas.Rows.Count > 1;

                // Not show navigation in Print Page
                if (this.NamingContainer.ToString().Equals("ASP.print_aspx"))
                {
                    showMultiMode = false;
                    showUpmenu = false;
                    showDownmenu = false;
                }

                var alignUpmenu = this.dameAlineacion(this.Settings["ENHANCEDHTML_ALIGNUPMENU"].ToString());
                var alignDownmenu = this.dameAlineacion(this.Settings["ENHANCEDHTML_ALIGNDOWNMENU"].ToString());
                var upmenu = string.Empty;
                var downmenu = string.Empty;
                var titulo = string.Empty;
                var buffer = string.Empty;
                var referencia = string.Empty;
                var prevRef = string.Empty;
                var nextRef = string.Empty;
                int i;
                var totalPages = 0;
                var actualPage = -1;
                var primera = this.ehPageId == null;
                var first = true;
                string aux;

                var ehPage = General.GetString("ENHANCEDHTML_PAGE", "Page");
                var ehOf = General.GetString("ENHANCEDHTML_OF", "of");
                var ehSinglePageMode = General.GetString("ENHANCEDHTML_SINGLEPAGEMODE", "Single Page Mode");

                foreach (DataRow fila in paginas.Rows)
                {
                    i = int.Parse((string)fila["ItemID"]);
                    totalPages++;
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        upmenu += this.currentMenuSeparator;
                    }

                    if (primera)
                    {
                        primera = false;
                        this.ehPageId = i.ToString();
                    }

                    if (i == int.Parse(this.ehPageId))
                    {
                        actualPage = totalPages;
                        buffer = (string)fila["DesktopHtml"];
                        titulo = (string)fila["Title"];

                        // fix by Rob Siera to prevent css problem with ContentPane A: classes being inherited (and not overwritten)
                        upmenu += "<span class='EnhancedHtmlLink'>" + titulo + "</span>";
                        if (referencia.Length != 0)
                        {
                            prevRef = referencia;
                        }
                    }
                    else
                    {
                        referencia = "<a href='" + this.dameUrl("&ModeID=0&EhPageID=" + i) + "' " +
                                     "class='EnhancedHtmlLink'>" + fila["Title"] + "</a>";
                        upmenu += referencia;
                        if (totalPages - 1 == actualPage)
                        {
                            nextRef = referencia;
                        }
                    }
                }

                if (prevRef.Length != 0)
                {
                    downmenu += prevRef + this.currentMenuSeparator;
                }

                downmenu += ehPage + " " + actualPage + " " + ehOf + " " + totalPages;
                if (nextRef.Length != 0)
                {
                    downmenu += this.currentMenuSeparator + nextRef;
                }

                aux = "<br><table width='100%' border='0' cellpadding='0' cellspacing='0'><tr>";
                if (showTitle)
                {
                    aux += "<td align='left'><span class='EnhancedHtmlTitlePage'>" + titulo + "</span></td>";
                    if (showMultiMode)
                    {
                        aux += "<td align='right'><a href=" + this.dameUrl("&ModeID=1") + ">";
                        aux += "<img src='" + Path.ApplicationRoot +
                               "/DesktopModules/EnhancedHTML/singlepage.gif' alt='" + ehSinglePageMode +
                               "' border='0' /></a></td>";
                    }

                    aux += "</tr><tr><td colspan=2><hr></td></tr><tr>";
                }

                if (showUpmenu && totalPages != 0)
                {
                    aux += "<td class='EnhancedHtmlIndexMenu' align='" + alignUpmenu + "'>" + upmenu + "</td>";
                }

                if (!showTitle && showMultiMode)
                {
                    aux += "<td align='right'><a href=" + this.dameUrl("&ModeID=1") + ">";
                    aux += "<img src='" + Path.ApplicationRoot + "/DesktopModules/EnhancedHTML/singlepage.gif' alt='" +
                           ehSinglePageMode + "' border='0' /></a></td>";
                }

                if (!aux.Equals("<br><table width='100%' border='0' cellpadding='0' cellspacing='0'><tr>"))
                {
                    // If table is empty not add to the control
                    aux += "</tr></table><br>";
                    this.HtmlHolder.Controls.Add(new LiteralControl(aux));
                }

                this.HtmlHolder.Controls.Add(this.toShow(buffer));
                if (showDownmenu && totalPages != 0)
                {
                    this.HtmlHolder.Controls.Add(
                        new LiteralControl(
                            "<hr><table width='100%' border='0' cellpadding='0' cellspacing='0'><tr><td align='" +
                            alignDownmenu + "'>" + downmenu + "</td></tr></table>"));
                }

                
            }
        }

        /// <summary>
        /// Adds the page row.
        /// </summary>
        /// <param name="tabla">
        /// The tabla.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        private void addPageRow(DataTable tabla, string item, string title, string content)
        {
            var fila = tabla.NewRow();
            fila["ItemID"] = item;
            fila["Title"] = title;
            fila["DesktopHtml"] = this.Server.HtmlDecode(content);
            tabla.Rows.Add(fila);
        }

        /// <summary>
        /// Dames the alineacion.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <returns>
        /// The dame alineacion.
        /// </returns>
        private string dameAlineacion(string n)
        {
            if (n.Equals("1"))
            {
                return "left";
            }

            if (n.Equals("2"))
            {
                return "center";
            }

            if (n.Equals("3"))
            {
                return "right";
            }

            return "left";
        }

        /// <summary>
        /// Dames the URL.
        /// </summary>
        /// <param name="custom">
        /// The custom.
        /// </param>
        /// <returns>
        /// The dame url.
        /// </returns>
        private string dameUrl(string custom)
        {
            // Sugerence by Mario Endara mario@softworks.com.uy 21-jun-2004
            // for compatibility with handler splitter
            return HttpUrlBuilder.BuildUrl(
                "~/" + HttpUrlBuilder.DefaultPage, this.PageID, this.ModuleID, null, custom, string.Empty, string.Empty);
        }

        /// <summary>
        /// Estableces the parametros.
        /// </summary>
        private void estableceParametros()
        {
            HttpCookie cookie1;
            DateTime time1;
            TimeSpan span1;
            int num1;
            string moduleCookie;
            var moduleInUrl = false;

            if (this.Page.Request.Params["mID"] != null)
            {
                moduleInUrl = int.Parse(this.Page.Request.Params["mID"]) == this.ModuleID;
            }

            this.currentMenuSeparator = "<span class='EnhancedHTMLSeparator'> | </span>";

            this.currentUrl = HttpUrlBuilder.BuildUrl(this.Page.Request.Path, this.PageID, this.ModuleID);
            this.currentUrl = this.currentUrl.Replace("//", "/");

            if (moduleInUrl)
            {
                if (this.Page.Request.Params["EhPageID"] != null)
                {
                    this.ehPageId = this.Page.Request.Params["EhPageID"];
                }
                else if (this.Page.Request.Params["ItemID"] != null)
                {
                    this.ehPageId = this.Page.Request.Params["ItemID"];
                }
            }

            this.currentModeUrl = this.currentUrl;
            moduleCookie = "EnhancedHtml:" + this.ModuleID;
            this.modeId = "0";
            if (this.Page.Request.Params["ModeID"] != null)
            {
                this.modeId = this.Page.Request.Params["ModeID"];
                cookie1 = new HttpCookie(moduleCookie);
                cookie1.Value = this.modeId;
                time1 = DateTime.Now;
                span1 = new TimeSpan(90, 0, 0, 0);
                cookie1.Expires = time1.Add(span1);
                base.Response.AppendCookie(cookie1);
            }
            else
            {
                if (base.Request.Cookies[moduleCookie] != null)
                {
                    this.modeId = this.Request.Cookies[moduleCookie].Value;
                }
            }

            num1 = this.currentModeUrl.IndexOf("ModeID=");
            if (num1 > 0)
            {
                this.currentModeUrl = this.currentModeUrl.Substring(0, num1 - 1);
                this.currentUrl = this.currentModeUrl;
            }

            num1 = this.currentUrl.IndexOf("EhPageID=");
            if (num1 > 0)
            {
                this.currentUrl = this.currentUrl.Substring(0, num1 - 1);
            }

            if (this.modeId == null)
            {
                this.modeId = "0";
            }
        }

        /// <summary>
        /// Gives the me pages.
        /// </summary>
        /// <param name="addInvariantCulture">
        /// if set to <c>true</c> [add invariant culture].
        /// </param>
        /// <returns>
        /// </returns>
        private DataTable giveMePages(bool addInvariantCulture)
        {
            var selected = false;
            var selectedPage = -1;
            if (!(this.ehPageId == null))
            {
                selectedPage = int.Parse(this.ehPageId);
            }

            var tabla = new DataTable("LocalizedPages");

            tabla.Columns.Add(new DataColumn("ItemID", typeof(string)));
            tabla.Columns.Add(new DataColumn("Title", typeof(string)));
            tabla.Columns.Add(new DataColumn("DesktopHtml", typeof(string)));

            var ehdb = new EnhancedHtmlDB();

            using (
                var dr = ehdb.GetLocalizedPages(this.ModuleID, this.portalSettings.PortalUILanguage.LCID, this.Version))
            {
                while (dr.Read())
                {
                    this.addPageRow(tabla, dr["ItemID"].ToString(), (string)dr["Title"], (string)dr["DesktopHtml"]);
                    if (int.Parse(dr["ItemID"].ToString()) == selectedPage)
                    {
                        selected = true;
                    }
                }

                if (tabla.Rows.Count == 0)
                {
                    if (this.portalSettings.PortalUILanguage.Parent.LCID != CultureInfo.InvariantCulture.LCID)
                    {
                        using (
                            var dr1 = ehdb.GetLocalizedPages(
                                this.ModuleID, this.portalSettings.PortalUILanguage.Parent.LCID, this.Version))
                        {
                            while (dr1.Read())
                            {
                                this.addPageRow(
                                    tabla, dr1["ItemID"].ToString(), (string)dr1["Title"], (string)dr1["DesktopHtml"]);
                                if (int.Parse(dr1["ItemID"].ToString()) == selectedPage)
                                {
                                    selected = true;
                                }
                            }
                        }
                    }
                }

                if (addInvariantCulture || tabla.Rows.Count == 0)
                {
                    using (
                        var dr2 = ehdb.GetLocalizedPages(this.ModuleID, CultureInfo.InvariantCulture.LCID, this.Version)
                        )
                    {
                        while (dr2.Read())
                        {
                            this.addPageRow(
                                tabla, dr2["ItemID"].ToString(), (string)dr2["Title"], (string)dr2["DesktopHtml"]);
                            if (int.Parse(dr2["ItemID"].ToString()) == selectedPage)
                            {
                                selected = true;
                            }
                        }
                    }
                }
            }

            if (!selected)
            {
                this.ehPageId = null;
            }

            return tabla;
        }

        /// <summary>
        /// Toes the show.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// </returns>
        private Control toShow(string text)
        {
            var module = 0;
            if (text.StartsWith(TokenModule))
            {
                module = int.Parse(text.Substring(TokenModule.Length));
            }
            else if (text.StartsWith(TokenPortalModule))
            {
                module = int.Parse(text.Substring(TokenPortalModule.Length));
            }
            else
            {
                return new LiteralControl(text);
            }

            PortalModuleControl portalModule;
            var controlPath = string.Empty;
            using (var dr = ModuleSettings.GetModuleDefinitionByID(module))
            {
                if (dr.Read())
                {
                    controlPath = string.Format("{0}/{1}", Path.ApplicationRoot, dr["DesktopSrc"]);
                }
            }

            try
            {
                if (string.IsNullOrEmpty(controlPath))
                {
                    return new LiteralControl(string.Format("Module '{0}' not found! ", module));
                }

                portalModule = (PortalModuleControl)this.Page.LoadControl(controlPath);

                // Sets portal ID
                portalModule.PortalID = this.PortalID;

                var m = new ModuleSettings
                    {
                        ModuleID = module, 
                        PageID = this.ModuleConfiguration.PageID, 
                        PaneName = this.ModuleConfiguration.PaneName, 
                        ModuleTitle = this.ModuleConfiguration.ModuleTitle, 
                        AuthorizedEditRoles = string.Empty, 
                        AuthorizedViewRoles = string.Empty, 
                        AuthorizedAddRoles = string.Empty, 
                        AuthorizedDeleteRoles = string.Empty, 
                        AuthorizedPropertiesRoles = string.Empty, 
                        CacheTime = this.ModuleConfiguration.CacheTime, 
                        ModuleOrder = this.ModuleConfiguration.ModuleOrder, 
                        ShowMobile = this.ModuleConfiguration.ShowMobile, 
                        DesktopSrc = controlPath
                    };

                portalModule.ModuleConfiguration = m;

                portalModule.Settings["MODULESETTINGS_APPLY_THEME"].Value = false;
                portalModule.Settings["MODULESETTINGS_SHOW_TITLE"].Value = false;
            }
            catch (Exception ex)
            {
                ErrorHandler.Publish(
                    LogLevel.Warn, string.Format("Shortcut: Unable to load control '{0}'!", controlPath), ex);
                return
                    new LiteralControl(
                        string.Format("<br><span class=NormalRed>Unable to load control '{0}'!<br>", controlPath));
            }

            portalModule.PropertiesUrl = string.Empty;
            portalModule.AddUrl = string.Empty; // Readonly
            portalModule.AddText = string.Empty; // Readonly
            portalModule.EditUrl = string.Empty; // Readonly
            portalModule.EditText = string.Empty; // Readonly
            portalModule.OriginalModuleID = this.ModuleID;

            CurrentCache.Remove(Key.ModuleSettings(module));
            return portalModule;
        }

        #endregion
    }
}