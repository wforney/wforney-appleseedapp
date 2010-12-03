using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using HyperLink=Appleseed.Framework.Web.UI.WebControls.HyperLink;
using LinkButton=Appleseed.Framework.Web.UI.WebControls.LinkButton;
using Localize=Appleseed.Framework.Web.UI.WebControls.Localize;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// ArticlesInline
    /// </summary>
    public partial class ArticlesInline : Articles
    {

        #region General Implementation

        /// <summary>
        /// Guid
        /// </summary>
        public override Guid GuidID
        {
            get { return new Guid("{5B7B52D3-837C-4942-A85C-AAF4B5CC098F}"); }
        }

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// Raises Init event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();

            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.myDataList.ItemCommand += new DataListCommandEventHandler(this.myDataList_ItemCommand);
            this.goBackTop.Click += new EventHandler(this.goback_Click);
            this.goBackBottom.Click += new EventHandler(this.goback_Click);
        }

        #endregion

        private void myDataList_ItemCommand(object source, DataListCommandEventArgs e)
        {
            if (e.CommandName == "View")
            {
                int ItemID = int.Parse(e.CommandArgument.ToString());

                if (ItemID > 0)
                {
                    BindData(ItemID);
                    ArticleDetail.Visible = true;
                    myDataList.Visible = false;
                }
            }
        }

        /// <summary>
        /// The BindData method is used to obtain details of a message
        /// from the Articles table, and update the page with
        /// the message content.
        /// </summary>
        private void BindData(int ItemID)
        {
            if (IsEditable)
            {
                editLinkDetail.NavigateUrl =
                    HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Articles/ArticlesEdit.aspx",
                                            "ItemID=" + ItemID + "&mid=" + ModuleID);
                editLinkDetail.Visible = true;
            }
            else
                editLinkDetail.Visible = false;


            // Obtain the selected item from the Articles table
            ArticlesDB Article = new ArticlesDB();
            SqlDataReader dr = Article.GetSingleArticle(ItemID, Version);

            try
            {
                // Load first row from database
                if (dr.Read())
                {
                    // Update labels with message contents
                    TitleDetail.Text = dr["Title"].ToString();

                    //Chris@cftechconsulting.com  5/24/04 added subtitle to ArticlesView.
                    if (dr["Subtitle"].ToString().Length > 0)
                    {
                        SubtitleDetail.Text = dr["Subtitle"].ToString();
                    }
                    StartDateDetail.Text = ((DateTime) dr["StartDate"]).ToShortDateString();
                    StartDateDetail.Visible = bool.Parse(Settings["ShowDate"].ToString());
                    Description.Text = Server.HtmlDecode(dr["Description"].ToString());
                    CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToShortDateString();
                    CreatedBy.Text = dr["CreatedByUser"].ToString();
                    // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                    if (CreatedBy.Text == "unknown")
                    {
                        CreatedBy.Text = General.GetString("UNKNOWN", "unknown");
                    }

                    //Chris Farrell, chris@cftechconsulting.com, 5/24/2004
                    if (!bool.Parse(Settings["MODULESETTINGS_SHOW_MODIFIED_BY"].ToString()))
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

        private void goback_Click(object sender, EventArgs e)
        {
            ArticleDetail.Visible = false;
            myDataList.Visible = true;
        }
    }
}