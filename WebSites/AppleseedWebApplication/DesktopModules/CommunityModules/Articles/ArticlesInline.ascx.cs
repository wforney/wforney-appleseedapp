// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArticlesInline.ascx.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   ArticlesInline
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Content.Web.Modules
{
    using System;
    using System.Web.UI.WebControls;

    using Appleseed.Framework;
    using Appleseed.Framework.Content.Data;

    /// <summary>
    /// Articles Inline
    /// </summary>
    public partial class ArticlesInline : Articles
    {
        #region Properties

        /// <summary>
        ///   Module Guid
        /// </summary>
        public override Guid GuidID
        {
            get
            {
                return new Guid("{5B7B52D3-837C-4942-A85C-AAF4B5CC098F}");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The BindData method is used to obtain details of a message
        ///   from the Articles table, and update the page with
        ///   the message content.
        /// </summary>
        /// <param name="itemId">
        /// The Item ID.
        /// </param>
        private void BindData(int itemId)
        {
            if (this.IsEditable)
            {
                this.editLinkDetail.NavigateUrl =
                    HttpUrlBuilder.BuildUrl(
                        "~/DesktopModules/CommunityModules/Articles/ArticlesEdit.aspx", 
                        string.Format("ItemID={0}&mid={1}", itemId, this.ModuleID));
                this.editLinkDetail.Visible = true;
            }
            else
            {
                this.editLinkDetail.Visible = false;
            }

            // Obtain the selected item from the Articles table
            var article = new ArticlesDB();
            var dr = article.GetSingleArticle(itemId, this.Version);

            try
            {
                // Load first row from database
                if (dr.Read())
                {
                    // Update labels with message contents
                    this.TitleDetail.Text = dr["Title"].ToString();

                    // Chris@cftechconsulting.com  5/24/04 added subtitle to ArticlesView.
                    if (dr["Subtitle"].ToString().Length > 0)
                    {
                        this.SubtitleDetail.Text = dr["Subtitle"].ToString();
                    }

                    this.StartDateDetail.Text = ((DateTime)dr["StartDate"]).ToShortDateString();
                    this.StartDateDetail.Visible = bool.Parse(this.Settings["ShowDate"].ToString());
                    this.Description.Text = this.Server.HtmlDecode(dr["Description"].ToString());
                    this.CreatedDate.Text = ((DateTime)dr["CreatedDate"]).ToShortDateString();
                    this.CreatedBy.Text = dr["CreatedByUser"].ToString();

                    // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                    if (this.CreatedBy.Text == "unknown")
                    {
                        this.CreatedBy.Text = General.GetString("UNKNOWN", "unknown");
                    }

                    // Chris Farrell, chris@cftechconsulting.com, 5/24/2004
                    if (!bool.Parse(this.Settings["MODULESETTINGS_SHOW_MODIFIED_BY"].ToString()))
                    {
                        this.CreatedLabel.Visible = false;
                        this.CreatedDate.Visible = false;
                        this.OnLabel.Visible = false;
                        this.CreatedBy.Visible = false;
                    }
                }
            }
            finally
            {
                // close the data reader
                dr.Close();
            }
        }

        /// <summary>
        /// Handles the Click event of the goback control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void GobackClick(object sender, EventArgs e)
        {
            this.ArticleDetail.Visible = false;
            MyDataList.Visible = true;
        }

        /// <summary>
        /// Handles the ItemCommand event of the MyDataList control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataListCommandEventArgs"/> instance containing the event data.</param>
        private void MyDataListItemCommand(object source, DataListCommandEventArgs e)
        {
            if (e.CommandName == "View")
            {
                var itemId = int.Parse(e.CommandArgument.ToString());

                if (itemId > 0)
                {
                    this.BindData(itemId);
                    this.ArticleDetail.Visible = true;
                    MyDataList.Visible = false;
                }
            }
        }

        #endregion
    }
}