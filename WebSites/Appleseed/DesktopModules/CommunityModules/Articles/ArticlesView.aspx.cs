using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Security;
using Appleseed.Framework.Web.UI;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    public partial class ArticlesView : ViewItemPage
    {

        protected bool IsEditable
        {
            get
            {
                if (PortalSecurity.HasEditPermissions(ModuleID) == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The Page_Load server event handler on this page is used
        /// to obtain the Articles list, and to then display
        /// the message contents.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Populate message contents if this is the first visit to the page
            if (!Page.IsPostBack && ModuleID > 0 && ItemID > 0)
            {
                StartDate.Visible = bool.Parse(this.ModuleSettings["ShowDate"].ToString());
                BindData();
            }

            if (IsEditable)
            {
                editLink.NavigateUrl =
                    HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Articles/ArticlesEdit.aspx",
                                            "ItemID=" + ItemID + "&mid=" + ModuleID);
                editLink.Visible = true;
            }
            else
                editLink.Visible = false;
        }

        /// <summary>
        /// Set the module guids with free access to this page
        /// </summary>
        protected override List<string> AllowedModules
        {
            get
            {
                List<string> al = new List<string>();
                al.Add("87303CF7-76D0-49B1-A7E7-A5C8E26415BA"); //Articles
                al.Add("5B7B52D3-837C-4942-A85C-AAF4B5CC098F"); //ArticlesInline
                al.Add("2502DB18-B580-4F90-8CB4-C15E6E531030"); // Access from portalSearch
                al.Add("2502DB18-B580-4F90-8CB4-C15E6E531052"); // Access from serviceItemList				
                return al;
            }
        }

        /// <summary>
        /// The BindData method is used to obtain details of a message
        /// from the Articles table, and update the page with
        /// the message content.
        /// </summary>
        private void BindData()
        {
            WorkFlowVersion Version = Request.QueryString["wversion"] == "Staging"
                                          ? WorkFlowVersion.Staging
                                          : WorkFlowVersion.Production;

            // Obtain the selected item from the Articles table
            ArticlesDB Article = new ArticlesDB();
            SqlDataReader dr = Article.GetSingleArticle(ItemID, Version);

            try
            {
                // Load first row from database
                if (dr.Read())
                {
                    // Update labels with message contents
                    Title.Text = dr["Title"].ToString();

                    //Chris@cftechconsulting.com  5/24/04 added subtitle to ArticlesView.
                    if (dr["Subtitle"].ToString().Length > 0)
                    {
                        Subtitle.Text = "(" + dr["Subtitle"].ToString() + ")";
                    }
                    StartDate.Text = ((DateTime) dr["StartDate"]).ToShortDateString();
                    Description.Text = Server.HtmlDecode(dr["Description"].ToString());
                    CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToShortDateString();
                    CreatedBy.Text = dr["CreatedByUser"].ToString();
                    // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                    if (CreatedBy.Text == "unknown")
                    {
                        CreatedBy.Text = General.GetString("UNKNOWN", "unknown");
                    }

                    //Chris Farrell, chris@cftechconsulting.com, 5/24/2004
                    if (!bool.Parse(this.ModuleSettings["MODULESETTINGS_SHOW_MODIFIED_BY"].ToString()))
                    {
                        CreatedLabel.Visible = false;
                        CreatedDate.Visible = false;
                        OnLabel.Visible = false;
                        CreatedBy.Visible = false;
                    }
                }
            }
            finally
            {
                // close the datareader
                dr.Close();
            }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            InitializeComponent();

            // - jminond
            // View item pages in general have no need for viewstate
            // Especailly big texts.
            this.Page.EnableViewState = false;

            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
        }

        #endregion
    }
}