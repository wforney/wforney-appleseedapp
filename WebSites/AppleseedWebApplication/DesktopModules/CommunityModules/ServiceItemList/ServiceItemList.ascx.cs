using System;
using System.Data;
using System.Data.SqlClient;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Services;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Users.Data;
using Appleseed.Framework.Web.UI;
using Appleseed.Framework.Web.UI.WebControls;
using History = Appleseed.Framework.History;
using Appleseed.Framework.Providers.AppleseedMembershipProvider;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// ServiceItemList
    /// Written by: Jakob hansen
    /// This module lists data from another Appleseed based portal or the same portal as where the module resides.
    /// All fields in result:
    /// ModuleName, Title, Description, ModuleID, ItemID, 
    /// CreatedByUser, CreatedDate, PageID, TabName, 
    /// ModuleGuidID, ModuleTitle
    /// </summary>
    [History("jminond", "2006/02/22", "converted to partial class")]
    [History("jminond", "2005/03/15", "Changes for moving Tab to Page")]
    public partial class ServiceItemList : PortalModuleControl
    {
        protected ServiceRequestInfo requestInfo;
        protected bool showImage, showModuleFriendlyName, showTitle, showDescription;
        protected bool showCreatedByUser, showCreatedDate, showLink, showTabName, showModuleTitle;
        protected string Target;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            requestInfo = new ServiceRequestInfo();
            requestInfo.Type = ServiceType.CommunityWebService;
            requestInfo.Url = Settings["URL"].ToString();
            requestInfo.PortalAlias = Settings["PortalAlias"].ToString();
            requestInfo.LocalMode = bool.Parse(Settings["LocalMode"].ToString().ToLower());
            /* Jakob says: later...
			requestInfo.UserName = Settings["UserName"].ToString(); 
			requestInfo.UserPassword = Settings["UserPassword"].ToString(); 
			*/

            requestInfo.ListType = ServiceListType.Item;
            requestInfo.ModuleType = Settings["ModuleType"].ToString();
            requestInfo.MaxHits = Int32.Parse(Settings["MaxHits"].ToString());
            requestInfo.ShowID = bool.Parse(Settings["ShowID"].ToString().ToLower());
            requestInfo.SearchString = Settings["SearchString"].ToString();
            requestInfo.SearchField = Settings["SearchField"].ToString();
            requestInfo.SortField = Settings["SortField"].ToString();
            requestInfo.SortDirection = Settings["SortDirection"].ToString();
            requestInfo.MobileOnly = bool.Parse(Settings["MobileOnly"].ToString().ToLower());
            requestInfo.IDList = Settings["IDList"].ToString();

            //requestInfo.IDListType = Settings["IDListType"].ToString();
            string par = Settings["IDListType"].ToString();
            if (par == ServiceListType.Item.ToString())
                requestInfo.IDListType = ServiceListType.Item;
            if (par == ServiceListType.Module.ToString())
                requestInfo.IDListType = ServiceListType.Module;
            if (par == ServiceListType.Tab.ToString())
                requestInfo.IDListType = ServiceListType.Tab;

            requestInfo.Tag = Int32.Parse(Settings["Tag"].ToString());

            //showImage = bool.Parse(Settings["ShowImage"].ToString().ToLower());
            showModuleFriendlyName = bool.Parse(Settings["ShowModuleFriendlyName"].ToString().ToLower());
            showTitle = bool.Parse(Settings["ShowSearchTitle"].ToString().ToLower());
            showDescription = bool.Parse(Settings["ShowDescription"].ToString().ToLower());
            showCreatedByUser = bool.Parse(Settings["ShowCreatedByUser"].ToString().ToLower());
            showCreatedDate = bool.Parse(Settings["ShowCreatedDate"].ToString().ToLower());
            showLink = bool.Parse(Settings["ShowLink"].ToString().ToLower());
            showTabName = bool.Parse(Settings["ShowTabName"].ToString().ToLower());
            showModuleTitle = bool.Parse(Settings["ShowModuleTitle"].ToString().ToLower());
            Target = "_" + Settings["Target"].ToString();

            GetItems();
        }


        /// <summary>
        /// Gets the items.
        /// </summary>
        private void GetItems()
        {
            string status = "Dialing...";
            try
            {
                int portalID = portalSettings.PortalID;
                Guid userID = Guid.Empty;

                UsersDB u = new UsersDB();
                AppleseedUser s = u.GetSingleUser(PortalSettings.CurrentUser.Identity.Email, portalSettings.PortalAlias);
                try
                {
                    userID = (Guid)s.ProviderUserKey;
                }
                finally
                {
                    //   s.Close(); //by Manu, fixed bug 807858
                }

                ServiceResponseInfo responseInfo;
                responseInfo =
                    ServiceHelper.CallService(portalID, userID, Path.ApplicationFullPath, ref requestInfo, (Page)Page);
                status = responseInfo.ServiceStatus;
                if (status != "OK")
                {
                    if (status.IndexOf("404") > 0)
                        lblStatus.Text = status + "<br>" + "URL: " + requestInfo.Url;
                    else
                        lblStatus.Text = "WARNING! Service status: " + status;
                }

                DataSet ds = FillPortalDS(ref responseInfo);
                DataGrid1.DataSource = ds;
                DataGrid1.DataBind();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "FATAL ERROR! Problem: " + ex.Message + ". Service status: " + status;
                return;
            }
        }


        /// <summary>
        /// Creates the portal DS.
        /// </summary>
        /// <param name="ds">The ds.</param>
        /// <returns></returns>
        private DataSet CreatePortalDS(DataSet ds)
        {
            ds.Tables.Add("ServiceItemList");
            //if (showImage)
            //	ds.Tables[0].Columns.Add("Image");
            if (showModuleFriendlyName)
                ds.Tables[0].Columns.Add("Module");
            if (showModuleTitle)
                ds.Tables[0].Columns.Add("Module Title");
            if (showTitle)
                ds.Tables[0].Columns.Add("Title");
            if (showDescription)
                ds.Tables[0].Columns.Add("Description");
            if (showCreatedByUser)
                ds.Tables[0].Columns.Add("User");
            if (showCreatedDate)
                ds.Tables[0].Columns.Add("Date");
            if (showLink)
                ds.Tables[0].Columns.Add("Link");
            if (showTabName)
                ds.Tables[0].Columns.Add("Tab");
            return ds;
        }


        /// <summary>
        /// Fills the portal DS.
        /// </summary>
        /// <param name="responseInfo">The response info.</param>
        /// <returns></returns>
        private DataSet FillPortalDS(ref ServiceResponseInfo responseInfo)
        {
            DataSet ds = new DataSet();
            try
            {
                ds = CreatePortalDS(ds);

                string strTmp, strLink, strBaseLink;
                string strModuleFriendlyName, strModuleID, strItemID;
                string strTabID, strModuleGuidID, strModuleTitle;
                string strLocate;
                DataRow dr;

                for (int row = 0; row < responseInfo.Items.Count; row++)
                {
                    dr = ds.Tables["ServiceItemList"].NewRow();

                    //ServiceResponseInfoItemExt item = (ServiceResponseInfoItemExt) responseInfo.Items[row];
                    ServiceResponseInfoItem item = (ServiceResponseInfoItem)responseInfo.Items[row];
                    strModuleFriendlyName = item.FriendlyName;
                    strModuleID = item.ModuleID.ToString();
                    strItemID = item.ItemID.ToString();
                    strTabID = item.PageID.ToString();
                    strModuleGuidID = item.GeneralModDefID.ToString().ToUpper();
                    strModuleTitle = item.ModuleTitle;
                    strLocate = "mID=" + strModuleID + "&ItemID=" + strItemID;

                    if (requestInfo.ShowID)
                    {
                        strModuleFriendlyName += " (ID=" + strModuleID + ")";
                        strModuleTitle += " (ID=" + strModuleID + ")";
                    }


                    if (requestInfo.LocalMode)
                    {
                        strBaseLink = Path.ApplicationRoot + "/";
                    }
                    else
                    {
                        strBaseLink = requestInfo.Url;
                    }

                    switch (strModuleGuidID)
                    {
                        case "2D86166C-4BDC-4A6F-A028-D17C2BB177C8": //Discussions
                            strLink = strBaseLink + "DesktopModules/Discussion/DiscussionView.aspx?" + strLocate;
                            break;
                        case "2502DB18-B580-4F90-8CB4-C15E6E531012": //Tasks
                            strLink = strBaseLink + "DesktopModules/Tasks/TasksView.aspx?" + strLocate;
                            break;
                        default:
                            strLink = strBaseLink + "DesktopDefault.aspx?tabID=" + strTabID;
                            break;
                    }
                    if (requestInfo.PortalAlias.Length != 0)
                        strLink += "&Alias=" + requestInfo.PortalAlias;


                    //if (showImage)
                    //{
                    //	dr["Image"] = "<a href='" + strLink + "'>" + strModuleGuidID + ".gif" + "</a>";
                    //}

                    if (showModuleFriendlyName)
                    {
                        dr["Module"] = strModuleFriendlyName;
                    }

                    if (showModuleTitle)
                    {
                        dr["Module Title"] = strModuleTitle;
                    }

                    if (showTitle)
                    {
                        if (strModuleGuidID == "0B113F51-FEA3-499A-98E7-7B83C192FDBB" || //Html Document
                            strModuleGuidID == "2B113F51-FEA3-499A-98E7-7B83C192FDBB") //Html WYSIWYG Edit (V2)
                        {
                            // We use the database field [rb.Modules].[ModuleTitle]:
                            strTmp = strModuleTitle;
                        }
                        else
                        {
                            if (item.Title == string.Empty)
                                strTmp = "missing";
                            else
                                strTmp = item.Title;
                        }
                        dr["Title"] = "<a href='" + strLink + "' Target='" + Target + "'>" + strTmp + "</a>";
                    }

                    if (showDescription)
                    {
                        if (item.Description == string.Empty)
                            strTmp = "missing";
                        else
                            strTmp = item.Description;

                        // Remove any html tags:
                        HTMLText html = SearchHelper.DeleteBeforeBody(Server.HtmlDecode(strTmp));
                        dr["Description"] = html.InnerText;
                    }

                    if (showCreatedByUser)
                    {
                        if (item.CreatedByUser == string.Empty)
                            strTmp = string.Empty;
                        else
                            strTmp = item.CreatedByUser;
                        // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                        if (strTmp == "unknown")
                        {
                            strTmp = General.GetString("UNKNOWN", "unknown");
                        }
                        dr["User"] = strTmp;
                    }

                    if (showCreatedDate)
                    {
                        try
                        {
                            strTmp = item.CreatedDate.ToShortDateString();
                        }
                        catch
                        {
                            strTmp = string.Empty;
                        }

                        // If date is an empty string the date "1/1/1900" is returned.
                        if (strTmp == "1/1/1900") strTmp = string.Empty;
                        dr["Date"] = strTmp;
                    }

                    if (showLink)
                    {
                        dr["Link"] = "<a href='" + strLink + "' Target='" + Target + "'>" + strLink + "</a>";
                    }

                    if (showTabName)
                    {
                        if (item.PageName == string.Empty)
                            strTmp = "missing";
                        else
                            strTmp = item.PageName;

                        if (requestInfo.ShowID && strTmp != "missing")
                            strTmp = strTmp + "(ID=" + item.PageID + ")";

                        if (requestInfo.PortalAlias.Length != 0)
                            dr["Tab"] = "<a href='" + strBaseLink + "DesktopDefault.aspx?tabID=" + strTabID + "&Alias=" +
                                        requestInfo.PortalAlias + "' Target='" + Target + "'>" + strTmp + "</a>";
                        else
                            dr["Tab"] = "<a href='" + strBaseLink + "DesktopDefault.aspx?tabID=" + strTabID +
                                        "' Target='" + Target + "'>" + strTmp + "</a>";
                    }

                    ds.Tables["ServiceItemList"].Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                lblStatus.Text = "Error when reading list. Problem: " + e.Message;
                return null;
            }
            return ds;
        }


        /// <summary>
        /// Consturctor where all the module settings are set as base property settings.
        /// if theye don't exist there defaults are set here.
        /// </summary>
        public ServiceItemList()
        {
            SettingItem setURL = new SettingItem(new UrlDataType());
            setURL.Order = 1;
            setURL.Required = true;
            setURL.Value = "http://www.Appleseedportal.net/";
            _baseSettings.Add("URL", setURL);

            SettingItem setPortalAlias = new SettingItem(new StringDataType());
            setPortalAlias.Order = 2;
            setPortalAlias.Required = false;
            setPortalAlias.Value = string.Empty;
            _baseSettings.Add("PortalAlias", setPortalAlias);

            SettingItem setLocalMode = new SettingItem(new BooleanDataType());
            setLocalMode.Order = 3;
            setLocalMode.Value = "false";
            _baseSettings.Add("LocalMode", setLocalMode);

            SettingItem setModuleType =
                new SettingItem(
                    new ListDataType(
                        "All;Announcements;Contacts;Discussion;Events;HtmlModule;Documents;Pictures;Articles;Tasks;FAQs;ComponentModule"));
            setModuleType.Order = 4;
            setModuleType.Required = true;
            setModuleType.Value = "All";
            _baseSettings.Add("ModuleType", setModuleType);

            SettingItem setMaxHits = new SettingItem(new IntegerDataType());
            setMaxHits.Order = 5;
            setMaxHits.Required = true;
            setMaxHits.Value = "20";
            setMaxHits.MinValue = 1;
            setMaxHits.MaxValue = 1000;
            _baseSettings.Add("MaxHits", setMaxHits);

            SettingItem setShowID = new SettingItem(new BooleanDataType());
            setShowID.Order = 6;
            setShowID.Value = "false";
            _baseSettings.Add("ShowID", setShowID);

            SettingItem setSearchString = new SettingItem(new StringDataType());
            setSearchString.Order = 7;
            setSearchString.Required = false;
            setSearchString.Value = "localization";
            _baseSettings.Add("SearchString", setSearchString);

            SettingItem setSearchField = new SettingItem(new StringDataType());
            setSearchField.Order = 8;
            setSearchField.Required = false;
            setSearchField.Value = string.Empty;
            _baseSettings.Add("SearchField", setSearchField);

            SettingItem setSortField =
                new SettingItem(new ListDataType("ModuleName;Title;CreatedByUser;CreatedDate;TabName"));
            setSortField.Order = 9;
            setSortField.Required = true;
            setSortField.Value = "ModuleName";
            _baseSettings.Add("SortField", setSortField);

            SettingItem setSortDirection = new SettingItem(new ListDataType("ASC;DESC"));
            setSortDirection.Order = 10;
            setSortDirection.Required = true;
            setSortDirection.Value = "ASC";
            _baseSettings.Add("SortDirection", setSortDirection);

            SettingItem setMobileOnly = new SettingItem(new BooleanDataType());
            setMobileOnly.Order = 11;
            setMobileOnly.Value = "false";
            _baseSettings.Add("MobileOnly", setMobileOnly);

            SettingItem setIDList = new SettingItem(new StringDataType());
            setIDList.Order = 12;
            setIDList.Required = false;
            setIDList.Value = string.Empty;
            _baseSettings.Add("IDList", setIDList);

            SettingItem setIDListType = new SettingItem(new ListDataType("Item;Module;Tab"));
            setIDListType.Order = 13;
            setIDListType.Required = true;
            setIDListType.Value = "Tab";
            _baseSettings.Add("IDListType", setIDListType);

            SettingItem setTag = new SettingItem(new IntegerDataType());
            setTag.Order = 14;
            setTag.Required = true;
            setTag.Value = "0";
            _baseSettings.Add("Tag", setTag);

            //SettingItem showImage = new SettingItem(new BooleanDataType());
            //showImage.Order = 15;
            //showImage.Value = "True";
            //this._baseSettings.Add("ShowImage", showImage);

            SettingItem setShowModuleFriendlyName = new SettingItem(new BooleanDataType());
            setShowModuleFriendlyName.Order = 16;
            setShowModuleFriendlyName.Value = "true";
            _baseSettings.Add("ShowModuleFriendlyName", setShowModuleFriendlyName);

            SettingItem setShowSearchTitle = new SettingItem(new BooleanDataType());
            setShowSearchTitle.Order = 17;
            setShowSearchTitle.Value = "true";
            _baseSettings.Add("ShowSearchTitle", setShowSearchTitle);

            SettingItem setShowDescription = new SettingItem(new BooleanDataType());
            setShowDescription.Order = 18;
            setShowDescription.Value = "true";
            _baseSettings.Add("ShowDescription", setShowDescription);

            SettingItem setShowCreatedByUser = new SettingItem(new BooleanDataType());
            setShowCreatedByUser.Order = 19;
            setShowCreatedByUser.Value = "true";
            _baseSettings.Add("ShowCreatedByUser", setShowCreatedByUser);

            SettingItem setShowCreatedDate = new SettingItem(new BooleanDataType());
            setShowCreatedDate.Order = 20;
            setShowCreatedDate.Value = "true";
            _baseSettings.Add("ShowCreatedDate", setShowCreatedDate);

            SettingItem setShowLink = new SettingItem(new BooleanDataType());
            setShowLink.Order = 21;
            setShowLink.Value = "false";
            _baseSettings.Add("ShowLink", setShowLink);

            SettingItem setShowTabName = new SettingItem(new BooleanDataType());
            setShowTabName.Order = 22;
            setShowTabName.Value = "true";
            _baseSettings.Add("ShowTabName", setShowTabName);

            SettingItem setShowModuleTitle = new SettingItem(new BooleanDataType());
            setShowModuleTitle.Order = 23;
            setShowModuleTitle.Value = "false";
            _baseSettings.Add("ShowModuleTitle", setShowModuleTitle);

            SettingItem setTarget = new SettingItem(new ListDataType("blank;parent;self;top"));
            setTarget.Order = 24;
            setTarget.Required = true;
            setTarget.Value = "blank";
            _baseSettings.Add("Target", setTarget);

            /* Jakob says: later...
			SettingItem setUserName = new SettingItem(new StringDataType());
			setUserName.Order = 25;
			setUserName.Required = false;
			setUserName.Value = string.Empty;
			this._baseSettings.Add("UserName", setUserName);

			SettingItem setUserPassword = new SettingItem(new StringDataType());
			setUserPassword.Order = 26;
			setUserPassword.Required = false;
			setUserPassword.Value = string.Empty;
			this._baseSettings.Add("UserPassword", setUserPassword);
			*/
        }


        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531052}"); }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises Init event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion
    }
}