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
    using System.Web.UI.WebControls;

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
                int portalID = this.PortalSettings.PortalID;
                Guid userID = Guid.Empty;

                UsersDB u = new UsersDB();
                AppleseedUser s = u.GetSingleUser(PortalSettings.CurrentUser.Identity.Email, this.PortalSettings.PortalAlias);
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
            var setURL = new SettingItem<Uri, TextBox>(new UrlDataType());
            setURL.Order = 1;
            setURL.Required = true;
            setURL.Value = new Uri("http://www.Appleseedportal.net/");
            this.BaseSettings.Add("URL", setURL);

            var setPortalAlias = new SettingItem<string, TextBox>();
            setPortalAlias.Order = 2;
            setPortalAlias.Required = false;
            setPortalAlias.Value = string.Empty;
            this.BaseSettings.Add("PortalAlias", setPortalAlias);

            var setLocalMode = new SettingItem<bool, CheckBox>();
            setLocalMode.Order = 3;
            setLocalMode.Value = false;
            this.BaseSettings.Add("LocalMode", setLocalMode);

            var setModuleType =
                new SettingItem<string, ListControl>(
                    new ListDataType<string, ListControl>(
                        "All;Announcements;Contacts;Discussion;Events;HtmlModule;Documents;Pictures;Articles;Tasks;FAQs;ComponentModule"));
            setModuleType.Order = 4;
            setModuleType.Required = true;
            setModuleType.Value = "All";
            this.BaseSettings.Add("ModuleType", setModuleType);

            var setMaxHits = new SettingItem<int, TextBox>();
            setMaxHits.Order = 5;
            setMaxHits.Required = true;
            setMaxHits.Value = 20;
            setMaxHits.MinValue = 1;
            setMaxHits.MaxValue = 1000;
            this.BaseSettings.Add("MaxHits", setMaxHits);

            var setShowID = new SettingItem<bool, CheckBox>();
            setShowID.Order = 6;
            setShowID.Value = false;
            this.BaseSettings.Add("ShowID", setShowID);

            var setSearchString = new SettingItem<string, TextBox>();
            setSearchString.Order = 7;
            setSearchString.Required = false;
            setSearchString.Value = "localization";
            this.BaseSettings.Add("SearchString", setSearchString);

            var setSearchField = new SettingItem<string, TextBox>();
            setSearchField.Order = 8;
            setSearchField.Required = false;
            setSearchField.Value = string.Empty;
            this.BaseSettings.Add("SearchField", setSearchField);

            var setSortField =
                new SettingItem<string, ListControl>(new ListDataType<string, ListControl>("ModuleName;Title;CreatedByUser;CreatedDate;TabName"));
            setSortField.Order = 9;
            setSortField.Required = true;
            setSortField.Value = "ModuleName";
            this.BaseSettings.Add("SortField", setSortField);

            var setSortDirection = new SettingItem<string, ListControl>(new ListDataType<string, ListControl>("ASC;DESC"));
            setSortDirection.Order = 10;
            setSortDirection.Required = true;
            setSortDirection.Value = "ASC";
            this.BaseSettings.Add("SortDirection", setSortDirection);

            var setMobileOnly = new SettingItem<bool, CheckBox>();
            setMobileOnly.Order = 11;
            setMobileOnly.Value = false;
            this.BaseSettings.Add("MobileOnly", setMobileOnly);

            var setIDList = new SettingItem<string, TextBox>();
            setIDList.Order = 12;
            setIDList.Required = false;
            setIDList.Value = string.Empty;
            this.BaseSettings.Add("IDList", setIDList);

            var setIDListType = new SettingItem<string, ListControl>(new ListDataType<string, ListControl>("Item;Module;Tab"));
            setIDListType.Order = 13;
            setIDListType.Required = true;
            setIDListType.Value = "Tab";
            this.BaseSettings.Add("IDListType", setIDListType);

            var setTag = new SettingItem<int, TextBox>();
            setTag.Order = 14;
            setTag.Required = true;
            setTag.Value = 0;
            this.BaseSettings.Add("Tag", setTag);

            //var showImage = new SettingItem<bool, CheckBox>();
            //showImage.Order = 15;
            //showImage.Value = true;
            //this._baseSettings.Add("ShowImage", showImage);

            var setShowModuleFriendlyName = new SettingItem<bool, CheckBox>();
            setShowModuleFriendlyName.Order = 16;
            setShowModuleFriendlyName.Value = true;
            this.BaseSettings.Add("ShowModuleFriendlyName", setShowModuleFriendlyName);

            var setShowSearchTitle = new SettingItem<bool, CheckBox>();
            setShowSearchTitle.Order = 17;
            setShowSearchTitle.Value = true;
            this.BaseSettings.Add("ShowSearchTitle", setShowSearchTitle);

            var setShowDescription = new SettingItem<bool, CheckBox>();
            setShowDescription.Order = 18;
            setShowDescription.Value = true;
            this.BaseSettings.Add("ShowDescription", setShowDescription);

            var setShowCreatedByUser = new SettingItem<bool, CheckBox>();
            setShowCreatedByUser.Order = 19;
            setShowCreatedByUser.Value = true;
            this.BaseSettings.Add("ShowCreatedByUser", setShowCreatedByUser);

            var setShowCreatedDate = new SettingItem<bool, CheckBox>();
            setShowCreatedDate.Order = 20;
            setShowCreatedDate.Value = true;
            this.BaseSettings.Add("ShowCreatedDate", setShowCreatedDate);

            var setShowLink = new SettingItem<bool, CheckBox>();
            setShowLink.Order = 21;
            setShowLink.Value = false;
            this.BaseSettings.Add("ShowLink", setShowLink);

            var setShowTabName = new SettingItem<bool, CheckBox>();
            setShowTabName.Order = 22;
            setShowTabName.Value = true;
            this.BaseSettings.Add("ShowTabName", setShowTabName);

            var setShowModuleTitle = new SettingItem<bool, CheckBox>();
            setShowModuleTitle.Order = 23;
            setShowModuleTitle.Value = false;
            this.BaseSettings.Add("ShowModuleTitle", setShowModuleTitle);

            var setTarget = new SettingItem<string, ListControl>(new ListDataType<string, ListControl>("blank;parent;self;top"));
            setTarget.Order = 24;
            setTarget.Required = true;
            setTarget.Value = "blank";
            this.BaseSettings.Add("Target", setTarget);

            /* Jakob says: later...
            var setUserName = new SettingItem<string, TextBox>();
            setUserName.Order = 25;
            setUserName.Required = false;
            setUserName.Value = string.Empty;
            this._baseSettings.Add("UserName", setUserName);

            var setUserPassword = new SettingItem<string, TextBox>();
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