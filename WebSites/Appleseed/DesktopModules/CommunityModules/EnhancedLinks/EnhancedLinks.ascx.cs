namespace Appleseed.Content.Web.Modules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;

    using Appleseed.Framework;
    using Appleseed.Framework.Content.Data;
    using Appleseed.Framework.Data;
    using Appleseed.Framework.DataTypes;
    using Appleseed.Framework.Helpers;
    using Appleseed.Framework.Web.UI.WebControls;

    /// <summary>
    /// Appleseed EnhancedLinks Module
    /// Written by: José Viladiu, jviladiu@portalServices.net
    /// </summary>
    public partial class EnhancedLinks : PortalModuleControl
    {
        /// <summary>
        /// The Page_Load event handler on this User Control is used to
        /// obtain a DataReader of link information from the EnhancedLinks
        /// table, and then data bind the results to a templated DataList
        /// server control.  It uses the Appleseed.EnhancedLinkDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            string iconContainer = Settings["ENHANCEDLINKS_ICONPATH"].ToString();
            string defaultImage = Settings["ENHANCEDLINKS_DEFAULTIMAGE"].ToString();
            bool dropDownList = Settings["ENHANCEDLINKS_SWITCHERTYPES"].ToString().Equals("1");
            bool doExpandAll = bool.Parse(Settings["ENHANCEDLINKS_EXPANDALL"].ToString());
            int maxColumns = int.Parse(Settings["ENHANCEDLINKS_MAXCOLUMS"].ToString());
            int actualColumn = 1;

            if (dropDownList)
            {
                if (!Page.IsPostBack)
                {
                    results.Visible = false;
                    cboLinks.Visible = true;
                    cboLinks.DataSource = mountDataList();
                    cboLinks.DataTextField = "TextField";
                    cboLinks.DataValueField = "ValueField";
                    cboLinks.SelectedIndex = 0;
                    cboLinks.DataBind();
                }
                return;
            }
            int itemCount = 0;
            EnhancedLinkDB enhancedLinks = new EnhancedLinkDB();
            StringBuilder listStr = new StringBuilder();
            listStr.Append("<table border=0 cellspacing=5 cellpadding=0>");
            SqlDataReader dr = enhancedLinks.GetEnhancedLinks(ModuleID, Version);

            listStr.Append("<tr>");

            while (dr.Read())
            {
                string imageStr = (string) dr["ImageURL"];
                string linkStr = (string) dr["Url"];
                string titleStr = (string) dr["Title"];
                string descStr = (string) dr["Description"];
                string targetStr = (string) dr["Target"];
                bool isSeparator = linkStr.Equals("SEPARATOR");

                if (isSeparator)
                {
                    listStr.Append("<td>&nbsp;</td></tr><tr>");
                    actualColumn = 1;
                }
                if (IsEditable)
                {
                    listStr.Append("<td><a href=\"" + GetLinkUrl(dr["ItemID"].ToString(), linkStr, true) +
                                   "\"><img src='" + this.CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl +
                                   "' border=0></a>&nbsp;</td>");
                }
                string auxImage;
                if ((imageStr != null) && (imageStr.Length > 0))
                {
                    auxImage = this.PortalSettings.PortalFullPath + "/" + iconContainer + "/" + imageStr;
                }
                else if ((defaultImage != null) && (defaultImage.Length > 0))
                {
                    auxImage = this.PortalSettings.PortalFullPath + "/" + iconContainer + "/" + defaultImage;
                }
                else
                {
                    auxImage = this.CurrentTheme.GetImage("NavLink", "navlink.gif").ImageUrl;
                }
                string altStr = string.Empty;
                if ((descStr != null) && (descStr.Length > 0))
                {
                    altStr = " title='" + descStr + "'";
                }
                targetStr = " target='" + targetStr + "'";
                listStr.Append("<td valign=center align=center><img src='" + auxImage + "' border=0></td>");
                if (isSeparator)
                {
                    listStr.Append("<td valign=center><a class='EnhancedTitle'>" + titleStr +
                                   "</a></td></tr><tr><td colspan=40><hr></td></tr><tr>");
                }
                else
                {
                    listStr.Append("<td valign=center><a class='EnhancedLink' href='" + GetLinkUrl(null, linkStr, false) +
                                   "'" + targetStr + altStr + ">" + titleStr + "</a>");
                    if ((doExpandAll) && (descStr != null) && (descStr.Length > 0))
                    {
                        listStr.Append("<br>" + "<a class='EnhancedDescription'>" + descStr + "</a>");
                    }
                }
                if (!isSeparator)
                {
                    listStr.Append("</td>");
                    if (actualColumn < maxColumns)
                    {
                        listStr.Append("<td>&nbsp;</td>");
                        actualColumn++;
                    }
                    else
                    {
                        listStr.Append("</tr>");
                        actualColumn = 1;
                    }
                }
                itemCount++;
            }
            dr.Close();

            listStr.Append("</tr>");
            listStr.Append("</table>\r\n");

            if (itemCount > 0)
            {
                results.Visible = true;
                results.Controls.Add(ParseControl(listStr.ToString()));
            }
        }

        /// <summary>
        /// Determines whether the specified value is numeric.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is numeric; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsNumeric(object value)
        {
            try
            {
                double d = Double.Parse(value.ToString(), NumberStyles.Any);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Mounts the data list.
        /// </summary>
        /// <returns></returns>
        private ICollection mountDataList()
        {
            EnhancedLinkDB enhancedLinks = new EnhancedLinkDB();
            SqlDataReader dr = enhancedLinks.GetEnhancedLinks(ModuleID, Version);
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("TextField", typeof (string)));
            dt.Columns.Add(new DataColumn("ValueField", typeof (string)));
            if (IsEditable)
            {
                dt.Rows.Add(CreateRow(General.GetString("ENHANCEDLINKS_EDITLINK", "Edit Link", null), string.Empty, dt));
            }
            else
            {
                dt.Rows.Add(CreateRow(ModuleTitle.Title, string.Empty, dt));
            }
            while (dr.Read())
            {
                string linkStr = (string) dr["Url"];
                if (!linkStr.Equals("SEPARATOR"))
                {
                    dt.Rows.Add(CreateRow((string) dr["Title"], dr["ItemID"].ToString(), dt));
                }
            }
            DataView dv = new DataView(dt);
            return dv;
        }

        /// <summary>
        /// Creates the row.
        /// </summary>
        /// <param name="Text">The text.</param>
        /// <param name="Value">The value.</param>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        private DataRow CreateRow(string Text, string Value, DataTable dt)
        {
            DataRow dr = dt.NewRow();
            dr[0] = Text;
            dr[1] = Value;
            return dr;
        }

        /// <summary>
        /// Gets the link URL.
        /// </summary>
        /// <param name="itemID">The item ID.</param>
        /// <param name="url">The URL.</param>
        /// <param name="editionMode">if set to <c>true</c> [edition mode].</param>
        /// <returns></returns>
        private string GetLinkUrl(object itemID, object url, bool editionMode)
        {
            if (editionMode)
            {
                if (IsEditable)
                {
                    return
                        HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/EnhancedLinks/EnhancedLinksEdit.aspx",
                                                "ItemID=" + itemID.ToString() + "&mID=" + ModuleID.ToString());
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                string linkStr = url.ToString();
                if (linkStr.IndexOf("://") < 0)
                {
                    if (IsNumeric(linkStr))
                    {
                        return HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx?tabid=" + linkStr);
                    }
                    else
                    {
                        if (linkStr.IndexOf("~/") < 0)
                        {
                            linkStr = "~/" + linkStr;
                        }
                        return HttpUrlBuilder.BuildUrl(linkStr);
                    }
                }
                return linkStr;
            }
        }

        #region General Module Implementation

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EnhancedLinks"/> class.
        /// </summary>
        public EnhancedLinks()
        {
            SupportsWorkflow = true;

            // Modified by Hongwei Shen(hongwei.shen@gmail.com) to group the settings
            // 13/9/2005
            SettingItemGroup group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            int groupBase = (int) group;
            // end of modification

            SettingItem<string, Panel> iconPath = null;
            if (this.PortalSettings != null)
            {
                iconPath =
                    new SettingItem<string, Panel>(
                        new FolderDataType(
                            HttpContext.Current.Server.MapPath(this.PortalSettings.PortalFullPath), "IconContainer"))
                        {
                            Value = "IconContainer", 
                            Order = groupBase + 15, 
                            Group = group, 
                            EnglishName = "Container for Icons", 
                            Description = "Portal directory for upload used icons"
                        };

                // Modified by Hongwei Shen
                // IconPath.Order = 5;
                // IconPath.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
                // end of modification
            }

            this.BaseSettings.Add("ENHANCEDLINKS_ICONPATH", iconPath);

            var styleLink = new List<SettingOption>
                {
                    new SettingOption(1, General.GetString("ENHANCEDLINKS_DROPDOWNLIST", "DropDownList", null)), 
                    new SettingOption(2, General.GetString("ENHANCEDLINKS_LINKS", "Links", null))
                };

            var maxColums = new SettingItem<int, TextBox>()
                {
                    Value = 1, 
                    EnglishName = "Max Colums", 
                    Description = "Maximun number of colums", 
                    Group = group, 
                    Order = groupBase + 20
                };

            // Modified by Hongwei Shen
            // MaxColums.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            // MaxColums.Order = 10;
            // end of modification
            this.BaseSettings.Add("ENHANCEDLINKS_MAXCOLUMS", maxColums);

            var labelStyleLink = new SettingItem<string, ListControl>(new CustomListDataType(styleLink, "Name", "Val"))
                {
                    Description = "Select here how your module should look like",
                    EnglishName = "Style Links",
                    Value = "2",
                    Group = group,
                    Order = groupBase + 25
                };

            // Modified by Hongwei Shen
            // abelStyleLink.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            // labelStyleLink.Order = 15;
            // end of modification
            this.BaseSettings.Add("ENHANCEDLINKS_SWITCHERTYPES", labelStyleLink);

            var imageDefault = new SettingItem<string, TextBox>()
                {
                    Value = "navLink.gif",
                    EnglishName = "Default Image for link",
                    Description = "Select here a image for links with no special setting image",
                    Group = group,
                    Order = groupBase + 30
                };

            // Modified by Hongwei Shen
            // ImageDefault.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            // ImageDefault.Order = 20;
            // end of modification
            this.BaseSettings.Add("ENHANCEDLINKS_DEFAULTIMAGE", imageDefault);

            var expandAll = new SettingItem<bool, CheckBox>()
                {
                    Value = false,
                    EnglishName = "Show Description",
                    Description = "Mark this if you like to see the description down the link",
                    Group = group,
                    Order = groupBase + 35
                };

            // Modified by Hongwei Shen
            // ExpandAll.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            // ExpandAll.Order = 25;
            // end of modification
            this.BaseSettings.Add("ENHANCEDLINKS_EXPANDALL", expandAll);
        }

        /// <summary>
        /// Searchable module
        /// </summary>
        /// <value></value>
        public override bool Searchable
        {
            get { return true; }
        }

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{96BC3CE0-0409-4ab1-A0C2-67D6C4D68193}"); }
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
            SearchDefinition s =
                new SearchDefinition("rb_EnhancedLinks", "Title", "Description", "CreatedByUser", "CreatedDate",
                                     searchField);

            s.ArrSearchFields.Add("itm.Url");
            s.ArrSearchFields.Add("itm.ImageUrl");

            return s.SearchSqlSelect(portalID, userID, searchString, true);
        }

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// Raises Init event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.cboLinks.SelectedIndexChanged += new EventHandler(this.cmdGo);
            this.Load += new EventHandler(this.Page_Load);
            if (!this.Page.IsCssFileRegistered("Mod_EnhancedLinks"))
                this.Page.RegisterCssFile("Mod_EnhancedLinks");
            this.AddUrl = "~/DesktopModules/CommunityModules/EnhancedLinks/EnhancedLinksEdit.aspx";
            base.OnInit(e);
        }

        /// <summary>
        /// CMDs the go.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void cmdGo(object sender, EventArgs e)
        {
            string auxItem = cboLinks.SelectedItem.Value;
            cboLinks.SelectedIndex = 0;
            if (IsEditable)
            {
                Response.Redirect(GetLinkUrl(auxItem, null, true), true);
            }
            else
            {
                string strURL = string.Empty;
                EnhancedLinkDB enhancedLinks = new EnhancedLinkDB();
                SqlDataReader dr = enhancedLinks.GetSingleEnhancedLink(Int32.Parse(auxItem), Version);
                if (dr.Read())
                {
                    strURL = dr["Url"].ToString();
                }
                dr.Close();
                Response.Redirect(GetLinkUrl(null, strURL, false), true);
            }
        }

        #endregion

        # region Install / Uninstall Implementation

        public override void Install(IDictionary stateSaver)
        {
            string currentScriptName = Server.MapPath(this.TemplateSourceDirectory + "/Install.sql");
            List<string> errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                // Call rollback
                throw new Exception("Error occurred:" + errors[0].ToString());
            }
        }

        public override void Uninstall(IDictionary stateSaver)
        {
            string currentScriptName = Server.MapPath(this.TemplateSourceDirectory + "/Uninstall.sql");
            List<string> errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                // Call rollback
                throw new Exception("Error occurred:" + errors[0].ToString());
            }
        }

        #endregion
    }
}