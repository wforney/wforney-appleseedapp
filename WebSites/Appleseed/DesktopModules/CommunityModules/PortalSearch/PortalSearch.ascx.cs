using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Users.Data;
using Appleseed.Framework.Web.UI.WebControls;
using BoundColumn = Appleseed.Framework.Web.UI.WebControls.BoundColumn;
using History = Appleseed.Framework.History;
using System.Web;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// Portal Search module
    /// Written by: Manu and Jakob hansen
    /// All fields in search result:
    /// ModuleName, Title, Abstract, ModuleID, ItemID, 
    /// CreatedByUser, CreatedDate, TabID, TabName, 
    /// ModuleGuidID, ModuleTitle
    /// </summary>
    [History("Mario Hartmann", "mario@hartmann.net", "2.1", "2003/10/08", "moved to separate folder")]
    [
        History("john.mandia@whitelightsolutions.com", "2003/05/21",
            "Changed Search Result links to use build url and fixed discussion bug")]
    [History("Jes1111", "2003/04/24", "Added !Cacheable property")]
    public partial class PortalSearch : PortalModuleControl
    {
        protected string sortOrder;
        protected int maxHits;
        protected bool showImage, showModuleName, showTitle, showAbstract;
        protected bool showCreatedByUser, showCreatedDate, showLink, showTabName, showTestInfo;
        protected bool showModuleTitle;
        protected Guid testUserID;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            lblError.Visible = false;

            sortOrder = Settings["SortOrder"].ToString();
            maxHits = Int32.Parse(Settings["MaxHits"].ToString());

            //showImage = "True" == Settings["ShowImage"].ToString();
            showModuleName = "True" == Settings["ShowModuleName"].ToString();
            showTitle = "True" == Settings["ShowSearchTitle"].ToString();
            showAbstract = "True" == Settings["ShowAbstract"].ToString();
            showCreatedByUser = "True" == Settings["ShowCreatedByUser"].ToString();
            showCreatedDate = "True" == Settings["ShowCreatedDate"].ToString();
            showLink = "True" == Settings["ShowLink"].ToString();
            showTabName = "True" == Settings["ShowTabName"].ToString();
            showTestInfo = "True" == Settings["ShowTestInfo"].ToString();
            showModuleTitle = "True" == Settings["ShowModuleTitle"].ToString();

            ddSearchField.Visible = "True" == Settings["showddField"].ToString();
            lblField.Visible = "True" == Settings["showddField"].ToString();
            ddTopics.Visible = "True" == Settings["showddTopic"].ToString();
            lblTopic.Visible = "True" == Settings["showddTopic"].ToString();
            ddSearchModule.Visible = "True" == Settings["showddModule"].ToString();
            lblModule.Visible = "True" == Settings["showddModule"].ToString();

            var testUserIdGuid = Settings["TestUserID"].ToString();
            if (!Guid.TryParse(testUserIdGuid, out testUserID))
            {
                testUserID = Guid.Empty;
            }

            // Jes1111
            if (Cacheable)
            {
                base.Cacheable = true;
                ModuleConfiguration.Cacheable = true;
            }
            else
            {
                base.Cacheable = false;
                ModuleConfiguration.Cacheable = false;
            }

            if (!IsPostBack)
            {
                // TODO do a better databind
                SearchHelper.AddToDropDownList(PortalID, ref ddSearchModule);

                ddTopics.DataSource = SearchHelper.GetTopicList(PortalID);
                ddTopics.DataBind();

                //Added by Rob Siera 19 aug 2004 - Search for the provided Default_Topic property. Select it if present.
                if (ddTopics.Items.FindByValue(Settings["defaultTopic"].ToString()) != null)
                {
                    ddTopics.SelectedValue = Settings["defaultTopic"].ToString();
                }
                //End addition Rob Siera

                ListItem tmpItem;
                tmpItem = new ListItem(General.GetString("PORTALSEARCH_ALL", "All", null), string.Empty);
                ddSearchField.Items.Add(tmpItem);
                tmpItem = new ListItem(General.GetString("PORTALSEARCH_TITLE", "Title", null), "Title");
                ddSearchField.Items.Add(tmpItem);
            }
        }


        /// <summary>
        /// Handles the Click event of the Search control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Search_Click(object sender, EventArgs e)
        {
            try
            {
                Guid userID = Guid.Empty;
                if (testUserID != Guid.Empty)
                    userID = testUserID;
                else
                {
                    UsersDB u = new UsersDB();
                    PortalSettings portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
                    System.Web.Security.MembershipUser s = u.GetSingleUser(PortalSettings.CurrentUser.Identity.Email, portalSettings.PortalAlias);
                    try
                    {
                        userID = (Guid)s.ProviderUserKey;
                    }
                    finally
                    {
                        //    s.Close(); //by Manu, fixed bug 807858
                    }
                }

                //Get topic
                string topicName = ddTopics.SelectedItem.Value;
                //All = no filter
                if (topicName == General.GetString("PORTALSEARCH_ALL", "All", null))
                    topicName = string.Empty;

                if (txtSearchString.Text.Length <= 2)
                    throw new Exception(
                        General.GetString("PORTALSEARCH_TONARROW", "Search string to narrow to be searched", null));

                SqlDataReader r =
                    SearchHelper.SearchPortal(PortalID, userID, ddSearchModule.SelectedItem.Value, txtSearchString.Text,
                                              ddSearchField.SelectedItem.Value, sortOrder, string.Empty, topicName,
                                              string.Empty);

                int hits;
                DataSet ds = FillPortalDS(PortalID, userID, r, out hits);

                DataView myDataView = new DataView();
                myDataView = ds.Tables[0].DefaultView;

                if (sortOrder.Equals("Title")) sortOrder = "cleanTitle";

                for (int i = 0; i < myDataView.Table.Columns.Count; i++)
                {
                    DataGrid1.Columns.Add(new BoundColumn());
                    DataGrid1.Columns[i].HeaderText = myDataView.Table.Columns[i].Caption;
                    ((BoundColumn)DataGrid1.Columns[i]).DataField = myDataView.Table.Columns[i].ColumnName;
                    if (myDataView.Table.Columns[i].ColumnName.Equals("cleanTitle"))
                        DataGrid1.Columns[i].Visible = false;
                    if (sortOrder.Equals(myDataView.Table.Columns[i].ColumnName))
                        myDataView.Sort = sortOrder + " ASC";
                }
                ;

                DataGrid1.DataSource = myDataView;
                DataGrid1.DataBind();
                if (hits == 1)
                    lblHits.Text = "1 " + General.GetString("PORTALSEARCH_HIT", "hit", null);
                else
                    lblHits.Text = Convert.ToString(hits) + " " + General.GetString("PORTALSEARCH_HITS", "hit", null);
            }
            catch (Exception ex)
            {
                lblError.Text = "We are sorry, an error occurred processing your request.";
                lblError.Visible = true;
                ErrorHandler.Publish(LogLevel.Error, ex);
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
            ds.Tables.Add("PortalSearch");
            //if (showImage)
            //	ds.Tables[0].Columns.Add("Image");
            if (showModuleName)
            {
                ds.Tables[0].Columns.Add("Module");
                ds.Tables[0].Columns["Module"].Caption = General.GetString("PORTALSEARCH_MODULE", "Module", null);
            }
            ;
            if (showModuleTitle)
            {
                ds.Tables[0].Columns.Add("Module Title");
                ds.Tables[0].Columns["Module Title"].Caption =
                    General.GetString("PORTALSEARCH_MODULETITLE", "Module Title", null);
            }
            ;
            if (showTitle)
            {
                ds.Tables[0].Columns.Add("Title");
                ds.Tables[0].Columns["Title"].Caption = General.GetString("PORTALSEARCH_TITLE", "Title", null);
                ds.Tables[0].Columns.Add("cleanTitle");
            }
            ;
            if (showAbstract)
            {
                ds.Tables[0].Columns.Add("Abstract");
                ds.Tables[0].Columns["Abstract"].Caption = General.GetString("ABSTRACT", "Abstract", null);
            }
            ;
            if (showCreatedByUser)
            {
                ds.Tables[0].Columns.Add("User");
                ds.Tables[0].Columns["User"].Caption = General.GetString("PORTALSEARCH_USER", "User", null);
            }
            ;
            if (showCreatedDate)
            {
                ds.Tables[0].Columns.Add("Date");
                ds.Tables[0].Columns["Date"].Caption = General.GetString("DATE", "Date", null);
            }
            ;
            if (showLink)
            {
                ds.Tables[0].Columns.Add("Link");
                ds.Tables[0].Columns["Link"].Caption = General.GetString("PORTALSEARCH_LINK", "Link", null);
            }
            ;
            if (showTabName)
            {
                ds.Tables[0].Columns.Add("Tab");
                ds.Tables[0].Columns["Tab"].Caption = General.GetString("PORTALSEARCH_TAB", "Tab", null);
            }
            ;
            if (showTestInfo)
            {
                ds.Tables[0].Columns.Add("TestInfo");
                ds.Tables[0].Columns["TestInfo"].Caption = General.GetString("PORTALSEARCH_TESTINFO", "TestInfo", null);
            }
            ;
            return ds;
        }


        /// <summary>
        /// Fills the portal DS.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="portalSearchResult">The portal search result.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        private DataSet FillPortalDS(int portalID, Guid userID, SqlDataReader portalSearchResult, out int hits)
        {
            hits = 0;
            DataSet ds = new DataSet();
            try
            {
                ds = CreatePortalDS(ds);

                string strTmp, strLink, strModuleName;
                string strModuleID, strItemID, strLocate;
                string strTabID, strTabName;
                string strModuleGuidID, strModuleTitle;
                DataRow dr;

                try
                {
                    while (hits <= maxHits && portalSearchResult.Read())
                    {
                        dr = ds.Tables["PortalSearch"].NewRow();

                        strModuleName = portalSearchResult.GetString(0);
                        strModuleID = portalSearchResult.GetInt32(3).ToString();
                        strItemID = portalSearchResult.GetInt32(4).ToString();
                        strLocate = "mID=" + strModuleID + "&ItemID=" + strItemID;
                        strTabID = portalSearchResult.GetInt32(7).ToString();
                        strTabName = portalSearchResult.GetString(8).ToString();
                        strModuleGuidID = portalSearchResult.GetGuid(9).ToString().ToUpper();
                        strModuleTitle = portalSearchResult.GetString(10);
                        //strLink = Appleseed.Framework.Settings.Path.ApplicationRoot;

                        // john.mandia@whitelightsolutions.com
                        // Changed the way links were created so that it utilises BuildUrl.
                        switch (strModuleGuidID)
                        {
                            case "2D86166C-4BDC-4A6F-A028-D17C2BB177C8": //Discussions
                                // Mark McFarlane 
                                // added support for a new page that lets you view an entire thread 
                                // URL requires tabID = 0
                                strLink =
                                    HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Discussion/DiscussionViewThread.aspx", 0,
                                                            strLocate);
                                break;
                            case "2502DB18-B580-4F90-8CB4-C15E6E531012": //Tasks
                                strLink =
                                    HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Tasks/TasksView.aspx",
                                                            Convert.ToInt32(strTabID), strLocate);
                                break;
                            case "87303CF7-76D0-49B1-A7E7-A5C8E26415BA": //Articles
                                // Rob Siera
                                // Added support to link to the article itself, instead op the page of with article module
                                strLink =
                                    HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Articles/ArticlesView.aspx", 0, strLocate);
                                break;
                            case "EC24FABD-FB16-4978-8C81-1ADD39792377": //Products
                                // Manu
                                int tabID =
                                    PortalSettings.GetRootPage(Convert.ToInt32(strTabID), this.PortalSettings.DesktopPages).
                                        PageID;
                                strLink =
                                    HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", tabID,
                                                            "mID=" + strModuleID + "&ItemID=" + strTabID);
                                break;

                            case "875254B7-2471-491F-BAF8-4AFC261CC224": //EnhancedHtml
                                // José Viladiu
                                // Added support to link to the specific page
                                strLink =
                                    HttpUrlBuilder.BuildUrl("~/DesktopDefault.aspx", Convert.ToInt32(strTabID),
                                                            strLocate);
                                break;
                            default:
                                strLink = HttpUrlBuilder.BuildUrl(Convert.ToInt32(strTabID));
                                // "/DesktopDefault.aspx?tabID=" + ;
                                break;
                        }

                        //if (showImage)
                        //{
                        //	dr["Image"] = "<a href='" + strLink + "'>" + strModuleGuidID + ".gif" + "</a>";
                        //}

                        if (showModuleName)
                        {
                            dr["Module"] = strModuleName;
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
                                if (portalSearchResult.IsDBNull(1))
                                    strTmp = General.GetString("PORTALSEARCH_MISSING", "Missing", null);
                                else
                                    strTmp = portalSearchResult.GetString(1);
                            }
                            dr["Title"] = "<a href='" + strLink + "'>" + strTmp + "</a>";
                            dr["cleanTitle"] = strTmp;
                        }

                        if (showAbstract)
                        {
                            if (portalSearchResult.IsDBNull(2))
                                strTmp = General.GetString("PORTALSEARCH_MISSING", "Missing", null);
                            else
                                strTmp = portalSearchResult.GetString(2);

                            // Remove any html tags:  
                            //HTMLText html = SearchHelper.DeleteBeforeBody(Server.HtmlDecode(strTmp));
                            // security script: Regex.Replace(strTmp, @"</?(?i:script|color|font|align|strong|font|div|class|modulepadding|modulescrollbars|body|embed|object|frameset|frame|iframe|meta|link|style)(.|\n)*?>", string.Empty);
                            dr["Abstract"] = Regex.Replace(Server.HtmlDecode(strTmp), @"<(.|\n)*?>", string.Empty);
                        }

                        if (showCreatedByUser)
                        {
                            if (portalSearchResult.IsDBNull(5))
                                strTmp = General.GetString("PORTALSEARCH_MISSING", "Missing", null);
                            else
                                strTmp = portalSearchResult.GetString(5);
                            // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                            if (strTmp == "unknown")
                            {
                                strTmp = General.GetString("UNKNOWN", "unknown");
                            }
                            dr["User"] = strTmp;
                        }

                        if (showCreatedDate)
                        {
                            if (portalSearchResult.IsDBNull(6))
                                strTmp = General.GetString("PORTALSEARCH_MISSING", "Missing", null);
                            else
                            {
                                try
                                {
                                    strTmp = portalSearchResult.GetDateTime(6).ToShortDateString();
                                }
                                catch
                                {
                                    strTmp = string.Empty;
                                }
                            }

                            // If GetDateTime(6) is an empty string the date "1/1/1900" is returned.
                            if (strTmp == "1/1/1900") strTmp = string.Empty;
                            dr["Date"] = strTmp;
                        }

                        if (showLink)
                        {
                            dr["Link"] = "<a href='" + strLink + "'>" + strLink + "</a>";
                        }

                        if (showTabName)
                        {
                            if (portalSearchResult.IsDBNull(8))
                                strTmp = General.GetString("PORTALSEARCH_MISSING", "Missing", null);
                            else
                                strTmp = portalSearchResult.GetString(8);
                            dr["Tab"] = "<a href='" + HttpUrlBuilder.BuildUrl(Convert.ToInt32(strTabID)) + "'>" + strTmp +
                                        "</a>";
                        }

                        if (showTestInfo)
                        {
                            dr["TestInfo"] = "ModuleGuidID=" + strModuleGuidID + "<br>" +
                                             "ModuleID=" + strModuleID + ", ItemID=" + strItemID + "<br>" +
                                             "PortalID=" + portalID.ToString() + ", UserID=" + userID.ToString() +
                                             "<br>" +
                                             "TabID=" + strTabID + ", TabName=" + strTabName;
                        }

                        ds.Tables["PortalSearch"].Rows.Add(dr);
                        hits++;
                    }
                }
                finally
                {
                    portalSearchResult.Close();
                }
            }
            catch (Exception e)
            {
                lblError.Text = "We are sorry, an error occurred processing your request.";
                lblError.Visible = true;
                ErrorHandler.Publish(LogLevel.Error, e);
                return null;
            }
            return ds;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PortalSearch"/> class.
        /// </summary>
        public PortalSearch()
        {
            var setSortOrder =
                new SettingItem<string, ListControl>(
                    new ListDataType<string, ListControl>("ModuleName;Title;CreatedByUser;CreatedDate;TabName"))
                    {
                        Required = true, Value = "ModuleName", Order = 1 
                    };
            this.BaseSettings.Add("SortOrder", setSortOrder);

            //var showImage = new SettingItem<bool, CheckBox>();
            //showImage.Order = 2;
            //showImage.Value = "True";
            //this._baseSettings.Add("ShowImage", showImage);

            var showModuleName1 = new SettingItem<bool, CheckBox>() { Order = 3, Value = true };
            this.BaseSettings.Add("ShowModuleName", showModuleName1);

            var showSearchTitle = new SettingItem<bool, CheckBox>() { Order = 4, Value = true };
            this.BaseSettings.Add("ShowSearchTitle", showSearchTitle);

            var showAbstract1 = new SettingItem<bool, CheckBox>() { Order = 5, Value = true };
            this.BaseSettings.Add("ShowAbstract", showAbstract1);

            var showCreatedByUser1 = new SettingItem<bool, CheckBox>() { Order = 6, Value = true };
            this.BaseSettings.Add("ShowCreatedByUser", showCreatedByUser1);

            var showCreatedDate1 = new SettingItem<bool, CheckBox>() { Order = 7, Value = true };
            this.BaseSettings.Add("ShowCreatedDate", showCreatedDate1);

            var showLink1 = new SettingItem<bool, CheckBox>() { Order = 8, Value = false };
            this.BaseSettings.Add("ShowLink", showLink1);

            var showTabName1 = new SettingItem<bool, CheckBox>() { Order = 9, Value = true };
            this.BaseSettings.Add("ShowTabName", showTabName1);

            var showTestInfo1 = new SettingItem<bool, CheckBox>() { Order = 10, Value = false };
            this.BaseSettings.Add("ShowTestInfo", showTestInfo1);

            var maxHits1 = new SettingItem<int, TextBox>()
                {
                    Required = true, Order = 11, Value = 100 
                };
            //maxHits.MinValue = 1;
            //maxHits.MaxValue = 1000;
            this.BaseSettings.Add("MaxHits", maxHits1);

            var showModuleTitle1 = new SettingItem<bool, CheckBox>() { Order = 12, Value = false };
            this.BaseSettings.Add("ShowModuleTitle", showModuleTitle1);

            var testUserId = new SettingItem<int, TextBox>()
                {
                    Required = true, Order = 13, Value = -1 
                };
            this.BaseSettings.Add("TestUserID", testUserId);

            var showddModule = new SettingItem<bool, CheckBox>()
                {
                    Value = true,
                    Order = 14,
                    EnglishName = "Show Module list",
                    Description = "Show the module drop down list."
                };
            this.BaseSettings.Add("showddModule", showddModule);

            var showddTopic = new SettingItem<bool, CheckBox>()
                {
                    Value = true,
                    Order = 15,
                    EnglishName = "Show Topics list",
                    Description = "Show the topics drop down list."
                };
            this.BaseSettings.Add("showddTopic", showddTopic);

            //Added by Rob Siera - 19 aug 2004 - Provide default Topic to search for
            var defaultTopic = new SettingItem<string, TextBox>()
                {
                    Value = "All",
                    Order = 16,
                    EnglishName = "Default Topic",
                    Description = "Set the default Topic to search."
                };
            this.BaseSettings.Add("defaultTopic", defaultTopic);
            //End addition Rob Siera

            var showddField = new SettingItem<bool, CheckBox>()
                {
                    Value = true,
                    Order = 17,
                    EnglishName = "Show Field list",
                    Description = "Show the field drop down list."
                };
            this.BaseSettings.Add("showddField", showddField);
        }

        // Jes1111
        /// <summary>
        /// Overrides ModuleSetting to render this module type un-cacheable
        /// </summary>
        /// <value></value>
        public override bool Cacheable
        {
            get { return false; }
        }


        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531030}"); }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises Init event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.btnSearch.Click += new EventHandler(this.Search_Click);
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion
    }
}