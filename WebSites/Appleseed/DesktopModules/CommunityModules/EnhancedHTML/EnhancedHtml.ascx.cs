using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.UI;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Data;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Settings.Cache;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI.WebControls;
using History=Appleseed.Framework.History;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// Appleseed EnhancedHtml Module
    /// Written by: José Viladiu, jviladiu@portalServices.net
    /// This module show a list of pages with navigation control
    /// and language control.
    /// </summary>
    [History("jminond", "march 2005", "Changes for moving Tab to Page")]
    [History("Hongwei Shen", "September 2005", "fix the module specific setting grouping problem")]
    public partial class EnhancedHtml : PortalModuleControl
    {
        #region private variables

        private string EhPageID;
        private string modeID;
        private string currentModeURL;
        private string currentURL;
        private string currentMenuSeparator;
        private readonly string tokenModule = "#MODULE#";
        private readonly string tokenPortalModule = "#PORTALMODULE#";

        #endregion

        #region page load

        /// <summary>
        /// The Page_Load event handler on this User Control is
        /// used to render blocks of HTML or text to the page.
        /// The text/HTML to render is stored in the EnhancedHtml
        /// database table.  This method uses the Appleseed.EnhancedHtmlDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            bool addInvariantCulture = bool.Parse(Settings["ENHANCEDHTML_ADDINVARIANTCULTURE"].ToString());
            bool showTitle = bool.Parse(Settings["ENHANCEDHTML_SHOWTITLEPAGE"].ToString());
            estableceParametros();
            DataTable paginas = giveMePages(addInvariantCulture);
            if (paginas.Rows.Count == 0) return;

            if (modeID == "1")
            {
                #region render all pages

                string ehMultiPageMode = General.GetString("ENHANCEDHTML_MULTIPAGEMODE", "Multi Page Mode");

                foreach (DataRow fila in paginas.Rows)
                {
                    Content = fila["DesktopHtml"];
                    int i = int.Parse((string) fila["ItemID"]);
                    string aux = "<p/><table width='100%' border='0' cellpadding='0' cellspacing='0'><tr>";

                    if (showTitle)
                    {
                        aux += "<br /><td width='100%' align='left'><span class='EnhancedHtmlTitlePage'>" +
                               fila["Title"] + "</span><hr></td>";
                    }
                    // Not show navigation in Print Page
                    if (paginas.Rows.Count > 1 && !NamingContainer.ToString().Equals("ASP.print_aspx"))
                    {
                        aux += "<td align='right'><a href=" + dameUrl("&ModeID=0&EhPageID=" + fila["ItemID"]) + ">";
                        aux += "<img src='" + Path.ApplicationRoot + "/DesktopModules/EnhancedHTML/multipage.gif' alt='" +
                               ehMultiPageMode + "' border='0' /></a></td>";
                    }
                    aux += "</tr></table>";

                    HtmlHolder.Controls.Add(new LiteralControl(aux));
                    HtmlHolder.Controls.Add(toShow(Content.ToString()));
                }

                #endregion
            }
            else
            {
                #region render selected page

                bool showMultiMode = bool.Parse(Settings["ENHANCEDHTML_SHOWMULTIMODE"].ToString()) &&
                                     paginas.Rows.Count > 1;
                bool showUpmenu = bool.Parse(Settings["ENHANCEDHTML_SHOWUPMENU"].ToString()) && paginas.Rows.Count > 1;
                bool showDownmenu = bool.Parse(Settings["ENHANCEDHTML_SHOWDOWNMENU"].ToString()) &&
                                    paginas.Rows.Count > 1;

                // Not show navigation in Print Page
                if (NamingContainer.ToString().Equals("ASP.print_aspx"))
                {
                    showMultiMode = false;
                    showUpmenu = false;
                    showDownmenu = false;
                }
                string alignUpmenu = dameAlineacion(Settings["ENHANCEDHTML_ALIGNUPMENU"].ToString());
                string alignDownmenu = dameAlineacion(Settings["ENHANCEDHTML_ALIGNDOWNMENU"].ToString());
                string upmenu = string.Empty;
                string downmenu = string.Empty;
                string titulo = string.Empty;
                string buffer = string.Empty;
                string referencia = string.Empty;
                string prevRef = string.Empty;
                string nextRef = string.Empty;
                int i;
                int totalPages = 0;
                int actualPage = -1;
                bool primera = (EhPageID == null);
                bool first = true;
                string aux;

                string ehPage = General.GetString("ENHANCEDHTML_PAGE", "Page");
                string ehOf = General.GetString("ENHANCEDHTML_OF", "of");
                string ehSinglePageMode = General.GetString("ENHANCEDHTML_SINGLEPAGEMODE", "Single Page Mode");

                foreach (DataRow fila in paginas.Rows)
                {
                    i = int.Parse((string) fila["ItemID"]);
                    totalPages++;
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        upmenu += currentMenuSeparator;
                    }
                    if (primera)
                    {
                        primera = false;
                        EhPageID = i.ToString();
                    }
                    if (i == int.Parse(EhPageID))
                    {
                        actualPage = totalPages;
                        buffer = (string) fila["DesktopHtml"];
                        titulo = (string) fila["Title"];
                        //fix by Rob Siera to prevent css problem with ContentPane A: classes being inherited (and not overwritten)
                        upmenu += "<span class='EnhancedHtmlLink'>" + titulo + "</span>";
                        if (referencia.Length != 0)
                        {
                            prevRef = referencia;
                        }
                    }
                    else
                    {
                        referencia = "<a href='" + dameUrl("&ModeID=0&EhPageID=" + i.ToString()) + "' " +
                                     "class='EnhancedHtmlLink'>" + fila["Title"] + "</a>";
                        upmenu += referencia;
                        if (totalPages - 1 == actualPage)
                        {
                            nextRef = referencia;
                        }
                    }
                }
                if (prevRef.Length != 0) downmenu += prevRef + currentMenuSeparator;
                downmenu += ehPage + " " + actualPage.ToString() + " " + ehOf + " " + totalPages.ToString();
                if (nextRef.Length != 0) downmenu += currentMenuSeparator + nextRef;

                aux = "<br><table width='100%' border='0' cellpadding='0' cellspacing='0'><tr>";
                if (showTitle)
                {
                    aux += "<td align='left'><span class='EnhancedHtmlTitlePage'>" + titulo + "</span></td>";
                    if (showMultiMode)
                    {
                        aux += "<td align='right'><a href=" + dameUrl("&ModeID=1") + ">";
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
                    aux += "<td align='right'><a href=" + dameUrl("&ModeID=1") + ">";
                    aux += "<img src='" + Path.ApplicationRoot + "/DesktopModules/EnhancedHTML/singlepage.gif' alt='" +
                           ehSinglePageMode + "' border='0' /></a></td>";
                }
                if (!aux.Equals("<br><table width='100%' border='0' cellpadding='0' cellspacing='0'><tr>"))
                    // If table is empty not add to the control
                {
                    aux += "</tr></table><br>";
                    HtmlHolder.Controls.Add(new LiteralControl(aux));
                }
                HtmlHolder.Controls.Add(toShow(buffer));
                if (showDownmenu && totalPages != 0)
                {
                    HtmlHolder.Controls.Add(
                        new LiteralControl(
                            "<hr><table width='100%' border='0' cellpadding='0' cellspacing='0'><tr><td align='" +
                            alignDownmenu + "'>" + downmenu + "</td></tr></table>"));
                }

                #endregion
            }
        }

        #endregion

        #region getContents to show

        /// <summary>
        /// Toes the show.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private Control toShow(string text)
        {
            int module = 0;
            if (text.StartsWith(tokenModule))
            {
                module = int.Parse(text.Substring(tokenModule.Length));
            }
            else if (text.StartsWith(tokenPortalModule))
            {
                module = int.Parse(text.Substring(tokenPortalModule.Length));
            }
            else return new LiteralControl(text.ToString());

            PortalModuleControl portalModule;
            string ControlPath = string.Empty;
            using (SqlDataReader dr = ModuleSettings.GetModuleDefinitionByID(module))
            {
                if (dr.Read())
                    ControlPath = Path.ApplicationRoot + "/" + dr["DesktopSrc"].ToString();
            }
            try
            {
                if (ControlPath == null || ControlPath.Length == 0)
                {
                    return new LiteralControl("Module '" + module + "' not found! ");
                }
                portalModule = (PortalModuleControl) Page.LoadControl(ControlPath);

                //Sets portal ID
                portalModule.PortalID = PortalID;

                ModuleSettings m = new ModuleSettings();
                m.ModuleID = module;
                m.PageID = ModuleConfiguration.PageID;
                m.PaneName = ModuleConfiguration.PaneName;
                m.ModuleTitle = ModuleConfiguration.ModuleTitle;
                m.AuthorizedEditRoles = string.Empty;
                m.AuthorizedViewRoles = string.Empty;
                m.AuthorizedAddRoles = string.Empty;
                m.AuthorizedDeleteRoles = string.Empty;
                m.AuthorizedPropertiesRoles = string.Empty;
                m.CacheTime = ModuleConfiguration.CacheTime;
                m.ModuleOrder = ModuleConfiguration.ModuleOrder;
                m.ShowMobile = ModuleConfiguration.ShowMobile;
                m.DesktopSrc = ControlPath;

                portalModule.ModuleConfiguration = m;

                portalModule.Settings["MODULESETTINGS_APPLY_THEME"] = false;
                portalModule.Settings["MODULESETTINGS_SHOW_TITLE"] = false;
            }
            catch (Exception ex)
            {
                ErrorHandler.Publish(LogLevel.Warn, "Shortcut: Unable to load control '" + ControlPath + "'!", ex);
                return
                    new LiteralControl("<br><span class=NormalRed>" + "Unable to load control '" + ControlPath + "'!" +
                                       "<br>");
            }

            portalModule.PropertiesUrl = string.Empty;
            portalModule.AddUrl = string.Empty; //Readonly
            portalModule.AddText = string.Empty; //Readonly
            portalModule.EditUrl = string.Empty; //Readonly
            portalModule.EditText = string.Empty; //Readonly
            portalModule.OriginalModuleID = ModuleID;

            CurrentCache.Remove(Key.ModuleSettings(module));
            return portalModule;
        }

        #endregion

        #region select pages to show

        /// <summary>
        /// Adds the page row.
        /// </summary>
        /// <param name="tabla">The tabla.</param>
        /// <param name="item">The item.</param>
        /// <param name="title">The title.</param>
        /// <param name="content">The content.</param>
        private void addPageRow(DataTable tabla, string item, string title, string content)
        {
            DataRow fila = tabla.NewRow();
            fila["ItemID"] = item;
            fila["Title"] = title;
            fila["DesktopHtml"] = Server.HtmlDecode(content);
            tabla.Rows.Add(fila);
        }

        /// <summary>
        /// Gives the me pages.
        /// </summary>
        /// <param name="addInvariantCulture">if set to <c>true</c> [add invariant culture].</param>
        /// <returns></returns>
        private DataTable giveMePages(bool addInvariantCulture)
        {
            bool selected = false;
            int selectedPage = -1;
            if (!(EhPageID == null)) selectedPage = int.Parse(EhPageID);

            DataTable tabla = new DataTable("LocalizedPages");

            tabla.Columns.Add(new DataColumn("ItemID", typeof (string)));
            tabla.Columns.Add(new DataColumn("Title", typeof (string)));
            tabla.Columns.Add(new DataColumn("DesktopHtml", typeof (string)));

            EnhancedHtmlDB ehdb = new EnhancedHtmlDB();

            using (SqlDataReader dr = ehdb.GetLocalizedPages(ModuleID, portalSettings.PortalUILanguage.LCID, Version))
            {
                while (dr.Read())
                {
                    addPageRow(tabla, dr["ItemID"].ToString(), (string) dr["Title"], (string) dr["DesktopHtml"]);
                    if (int.Parse(dr["ItemID"].ToString()) == selectedPage) selected = true;
                }

                if (tabla.Rows.Count == 0)
                {
                    if (portalSettings.PortalUILanguage.Parent.LCID != CultureInfo.InvariantCulture.LCID)
                    {
                        using (
                            SqlDataReader dr1 =
                                ehdb.GetLocalizedPages(ModuleID, portalSettings.PortalUILanguage.Parent.LCID, Version))
                        {
                            while (dr1.Read())
                            {
                                addPageRow(tabla, dr1["ItemID"].ToString(), (string) dr1["Title"],
                                           (string) dr1["DesktopHtml"]);
                                if (int.Parse(dr1["ItemID"].ToString()) == selectedPage) selected = true;
                            }
                        }
                    }
                }

                if (addInvariantCulture || tabla.Rows.Count == 0)
                {
                    using (
                        SqlDataReader dr2 = ehdb.GetLocalizedPages(ModuleID, CultureInfo.InvariantCulture.LCID, Version)
                        )
                    {
                        while (dr2.Read())
                        {
                            addPageRow(tabla, dr2["ItemID"].ToString(), (string) dr2["Title"],
                                       (string) dr2["DesktopHtml"]);
                            if (int.Parse(dr2["ItemID"].ToString()) == selectedPage) selected = true;
                        }
                    }
                }
            }

            if (!selected) EhPageID = null;
            return tabla;
        }

        #endregion

        #region get/set initial parameters		

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
            bool moduleInUrl = false;

            if (Page.Request.Params["mID"] != null)
            {
                moduleInUrl = int.Parse(Page.Request.Params["mID"]) == ModuleID;
            }

            currentMenuSeparator = "<span class='EnhancedHTMLSeparator'> | </span>";

            currentURL = HttpUrlBuilder.BuildUrl(Page.Request.Path, PageID, ModuleID);
            currentURL = currentURL.Replace("//", "/");

            if (moduleInUrl)
            {
                if (Page.Request.Params["EhPageID"] != null)
                {
                    EhPageID = Page.Request.Params["EhPageID"].ToString();
                }
                else if (Page.Request.Params["ItemID"] != null)
                {
                    EhPageID = Page.Request.Params["ItemID"].ToString();
                }
            }
            currentModeURL = currentURL;
            moduleCookie = "EnhancedHtml:" + ModuleID.ToString();
            modeID = "0";
            if (Page.Request.Params["ModeID"] != null)
            {
                modeID = Page.Request.Params["ModeID"].ToString();
                cookie1 = new HttpCookie(moduleCookie);
                cookie1.Value = modeID;
                time1 = DateTime.Now;
                span1 = new TimeSpan(90, 0, 0, 0);
                cookie1.Expires = time1.Add(span1);
                base.Response.AppendCookie(cookie1);
            }
            else
            {
                if (base.Request.Cookies[moduleCookie] != null)
                {
                    modeID = Request.Cookies[moduleCookie].Value;
                }
            }
            num1 = currentModeURL.IndexOf("ModeID=");
            if (num1 > 0)
            {
                currentModeURL = currentModeURL.Substring(0, (num1 - 1));
                currentURL = currentModeURL;
            }
            num1 = currentURL.IndexOf("EhPageID=");
            if (num1 > 0)
            {
                currentURL = currentURL.Substring(0, (num1 - 1));
            }
            if (modeID == null)
            {
                modeID = "0";
            }
        }

        #endregion

        #region auxiliary functions

        /// <summary>
        /// Dames the alineacion.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        private string dameAlineacion(string n)
        {
            if (n.Equals("1")) return "left";
            if (n.Equals("2")) return "center";
            if (n.Equals("3")) return "right";
            return "left";
        }

        /// <summary>
        /// Dames the URL.
        /// </summary>
        /// <param name="custom">The custom.</param>
        /// <returns></returns>
        private string dameUrl(string custom)
        {
            // Sugerence by Mario Endara mario@softworks.com.uy 21-jun-2004
            // for compatibility with handler splitter
            return
                HttpUrlBuilder.BuildUrl("~/" + HttpUrlBuilder.DefaultPage, PageID, ModuleID, null, custom, string.Empty,
                                        string.Empty);
        }

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EnhancedHtml"/> class.
        /// </summary>
        public EnhancedHtml()
        {
            int _groupOrderBase;
            SettingItemGroup _Group;

            #region Module Settings

            _Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            // modified by Hongwei Shen(hongwei.shen@gmail.com) 12/9/2005
            //_groupOrderBase = 0;
            _groupOrderBase = (int) _Group;
            // end of modification

            HtmlEditorDataType.HtmlEditorSettings(_baseSettings, _Group);

            SettingItem ShowTitlePage = new SettingItem(new BooleanDataType());
            ShowTitlePage.Value = "false";
            ShowTitlePage.Order = _groupOrderBase + 20;
            ShowTitlePage.Group = _Group;
            ShowTitlePage.EnglishName = "Show Title Page?";
            ShowTitlePage.Description = "Mark this if you like see the Title Page";
            _baseSettings.Add("ENHANCEDHTML_SHOWTITLEPAGE", ShowTitlePage);

            SettingItem ShowUpMenu = new SettingItem(new BooleanDataType());
            ShowUpMenu.Value = "false";
            ShowUpMenu.Order = _groupOrderBase + 25;
            ShowUpMenu.Group = _Group;
            ShowUpMenu.EnglishName = "Show Index Menu?";
            ShowUpMenu.Description = "Mark this if you like see a index menu whith the titles of all pages";
            _baseSettings.Add("ENHANCEDHTML_SHOWUPMENU", ShowUpMenu);

            ArrayList alignUpMenu = new ArrayList();
            alignUpMenu.Add(new SettingOption(1, General.GetString("LEFT", "Left")));
            alignUpMenu.Add(new SettingOption(2, General.GetString("CENTER", "Center")));
            alignUpMenu.Add(new SettingOption(3, General.GetString("RIGHT", "Right")));

            SettingItem labelAlignUpMenu = new SettingItem(new CustomListDataType(alignUpMenu, "Name", "Val"));
            labelAlignUpMenu.Description = "Select here the align for index menu";
            labelAlignUpMenu.EnglishName = "Align Index Menu";
            labelAlignUpMenu.Value = "1";
            labelAlignUpMenu.Order = _groupOrderBase + 30;
            _baseSettings.Add("ENHANCEDHTML_ALIGNUPMENU", labelAlignUpMenu);

            SettingItem ShowDownMenu = new SettingItem(new BooleanDataType());
            ShowDownMenu.Value = "true";
            ShowDownMenu.Order = _groupOrderBase + 40;
            ShowDownMenu.Group = _Group;
            ShowDownMenu.EnglishName = "Show Navigation Menu?";
            ShowDownMenu.Description = "Mark this if you like see a navigation menu with previous and next page";
            _baseSettings.Add("ENHANCEDHTML_SHOWDOWNMENU", ShowDownMenu);

            ArrayList alignDownMenu = new ArrayList();
            alignDownMenu.Add(new SettingOption(1, General.GetString("LEFT", "Left")));
            alignDownMenu.Add(new SettingOption(2, General.GetString("CENTER", "Center")));
            alignDownMenu.Add(new SettingOption(3, General.GetString("RIGHT", "Right")));

            SettingItem labelAlignDownMenu = new SettingItem(new CustomListDataType(alignDownMenu, "Name", "Val"));
            labelAlignDownMenu.Description = "Select here the align for index menu";
            labelAlignDownMenu.EnglishName = "Align Navigation Menu";
            labelAlignDownMenu.Value = "3";
            labelAlignDownMenu.Order = _groupOrderBase + 50;
            _baseSettings.Add("ENHANCEDHTML_ALIGNDOWNMENU", labelAlignDownMenu);

            SettingItem AddInvariant = new SettingItem(new BooleanDataType());
            AddInvariant.Value = "true";
            AddInvariant.Order = _groupOrderBase + 60;
            AddInvariant.Group = _Group;
            AddInvariant.EnglishName = "Add Invariant Culture?";
            AddInvariant.Description =
                "Mark this if you like see pages with invariant culture after pages with actual culture code";
            _baseSettings.Add("ENHANCEDHTML_ADDINVARIANTCULTURE", AddInvariant);

            SettingItem ShowMultiMode = new SettingItem(new BooleanDataType());
            ShowMultiMode.Value = "true";
            ShowMultiMode.Order = _groupOrderBase + 70;
            ShowMultiMode.Group = _Group;
            ShowMultiMode.EnglishName = "Show Multi-Mode icon?";
            ShowMultiMode.Description = "Mark this if you like see icon multimode page";
            _baseSettings.Add("ENHANCEDHTML_SHOWMULTIMODE", ShowMultiMode);

            SettingItem GetContentsFromPortals = new SettingItem(new BooleanDataType());
            GetContentsFromPortals.Value = "false";
            GetContentsFromPortals.Order = _groupOrderBase + 80;
            GetContentsFromPortals.Group = _Group;
            GetContentsFromPortals.EnglishName = "Get contents from others Portals?";
            GetContentsFromPortals.Description =
                "Mark this if you like get contents from modules in others portals in the same database";
            _baseSettings.Add("ENHANCEDHTML_GET_CONTENTS_FROM_PORTALS", GetContentsFromPortals);

            #endregion

            SupportsWorkflow = true;
        }

        #endregion

        #region Searchable module implementation

        /// <summary>
        /// Searchable module
        /// </summary>
        /// <value></value>
        public override bool Searchable
        {
            get { return true; }
        }

        /// <summary>
        /// Searchable module implementation
        /// </summary>
        /// <param name="portalID">The portal ID</param>
        /// <param name="userID">ID of the user is searching</param>
        /// <param name="searchString">The text to search</param>
        /// <param name="searchField">The fields where perfoming the search</param>
        /// <returns>
        /// The SELECT sql to perform a search on the current module
        /// </returns>
        public override string SearchSqlSelect(int portalID, int userID, string searchString, string searchField)
        {
            // For better performance is necessary to add in 
            // method FillPortalDS from PortalSearch.ascx.cs the
            // next case:
            //    case "875254B7-2471-491F-BAF8-4AFC261CC224":  //EnhancedHtml
            //       strLink = HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", Convert.ToInt32(strTabID), strLocate);
            //       break;

            string dbTable = "rb_EnhancedHtml";
            if (Version == WorkFlowVersion.Staging)
                dbTable += "_st";
            SearchDefinition s =
                new SearchDefinition(dbTable, "Title", "DesktopHtml", "CreatedByUser", "CreatedDate", searchField);
            string retorno = s.SearchSqlSelect(portalID, userID, searchString);
            if (HttpContext.Current != null && HttpContext.Current.Items["PortalSettings"] != null)
            {
                PortalSettings pS = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
                retorno += " AND ((itm.CultureCode = '" + pS.PortalUILanguage.LCID.ToString() +
                           "') OR (itm.CultureCode = '" + CultureInfo.InvariantCulture.LCID.ToString() + "'))";
            }
            return retorno;
        }

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// OnInit
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);

            if (!this.Page.IsCssFileRegistered("Mod_EnhancedHtml"))
                this.Page.RegisterCssFile("Mod_EnhancedHtml");

            this.EditUrl = "~/DesktopModules/CommunityModules/EnhancedHtml/EnhancedHtmlEdit.aspx";
            base.OnInit(e);
        }

        #endregion

        #region Install / Uninstall Implementation

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{875254B7-2471-491f-BAF8-4AFC261CC224}"); }
        }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "install.sql");
            ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                throw new Exception("Error occurred:" + errors[0].ToString());
            }
        }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Uninstall(IDictionary stateSaver)
        {
            string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "uninstall.sql");
            ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                throw new Exception("Error occurred:" + errors[0].ToString());
            }
        }

        #endregion
    }
}