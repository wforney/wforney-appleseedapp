using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Web.UI;
using ImageButton=Appleseed.Framework.Web.UI.WebControls.ImageButton;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    [History("jminond", "2005/3/12", "Changes for moving Tab to Page")]
    [History("Mario Endara", "2004/6/1", "Added EsperantusKeys for Localization")]
    public partial class BlogView : ViewItemPage
    {
        protected Localize OnLabel;
        protected Localize CreatedLabel;
        protected HyperLink deleteLink;
        protected ImageButton btnDelete;
        public bool IsDeleteable = false;


        /// <summary>
        /// Author:					Joe Audette
        /// Created:				1/18/2004
        /// Last Modified:			2/8/2004
        /// 
        /// The Page_Load server event handler on this page is used
        /// to obtain the Blogs list, and to then display 
        /// the message contents.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && ModuleID > 0 && ItemID > 0)
            {
                if (Context.User.Identity.IsAuthenticated)
                {
                    char[] separator = {';'};
                    string[] deleteRoles = this.Module.AuthorizedDeleteRoles.Split(separator);
                    foreach (string role in deleteRoles)
                    {
                        if (role.Length > 0)
                        {
                            if (Context.User.IsInRole(role))
                            {
                                IsDeleteable = true;
                            }
                        }
                    }
                }
                lnkRSS.HRef = HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Blog/RSS.aspx", PageID, "&mID=" + ModuleID);
                imgRSS.Src = HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Blog/xml.gif");
                lblCopyright.Text = this.ModuleSettings["Copyright"].ToString();

                BindData();
            }
        }

        /// <summary>
        /// Set the module guids with free access to this page
        /// </summary>
        protected override List<string> AllowedModules
        {
            get
            {
                List<string> al = new List<string>();
                al.Add("55EF407B-C9D6-47e3-B627-EFA6A5EEF4B2");
                return al;
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            // Redirect back to the blog page
            RedirectBackToReferringPage();
        }

        protected void dlComments_ItemCommand(object source, DataListCommandEventArgs e)
        {
            if (e.CommandName == "DeleteComment")
            {
                BlogDB blogDB = new BlogDB();
                blogDB.DeleteBlogComment(int.Parse(e.CommandArgument.ToString()));
                Response.Clear();
                Response.Redirect(Request.Url.ToString());
            }
        }

        protected void btnPostComment_Click(object sender, EventArgs e)
        {
            if (IsValidComment())
            {
                if (chkRememberMe.Checked)
                {
                    SetCookies();
                }
                BlogDB blogDB = new BlogDB();
                blogDB.AddBlogComment(ModuleID, ItemID, txtName.Text,
                                      txtTitle.Text, txtURL.Text, txtComments.Text);
                Response.Redirect(Request.Url.ToString());
            }
        }

        private bool IsValidComment()
        {
            bool result = true;
            //TODO do we need validation?

            return result;
        }

        private void SetCookies()
        {
            HttpCookie blogUserCookie = new HttpCookie("blogUser", txtName.Text);
            HttpCookie blogUrlCookie = new HttpCookie("blogUrl", txtURL.Text);
            blogUserCookie.Expires = DateTime.Now.AddMonths(1);
            blogUrlCookie.Expires = DateTime.Now.AddMonths(1);
            Response.Cookies.Add(blogUserCookie);
            Response.Cookies.Add(blogUrlCookie);
        }

        /// <summary>
        /// The BindData method is used to obtain details of a message
        /// from the Blogs table, and update the page with
        /// the message content.
        /// </summary>
        private void BindData()
        {
            // Obtain the selected item from the Blogs table
            BlogDB blogDB = new BlogDB();
            SqlDataReader dataReader = blogDB.GetSingleBlog(ItemID);

            try
            {
                // Load first row from database
                if (dataReader.Read())
                {
                    // Update labels with message contents
                    Title.Text = (string) dataReader["Title"].ToString();
                    txtTitle.Text = "re: " + (string) dataReader["Title"].ToString();
                    StartDate.Text = ((DateTime) dataReader["StartDate"]).ToString("dddd MMMM d yyyy hh:mm tt");
                    Description.Text = Server.HtmlDecode((string) dataReader["Description"].ToString());
                }
            }
            finally
            {
                dataReader.Close();
            }
            dlComments.DataSource = blogDB.GetBlogComments(ModuleID, ItemID);
            dlComments.DataBind();

            if (Request.Params.Get("blogUser") != null)
            {
                txtName.Text = Request.Params.Get("blogUser");
            }
            if (Request.Params.Get("blogUrl") != null)
            {
                txtURL.Text = Request.Params.Get("blogUrl");
            }

            dlArchive.DataSource = blogDB.GetBlogMonthArchive(ModuleID);
            dlArchive.DataBind();

            dataReader = blogDB.GetBlogStats(ModuleID);
            try
            {
                if (dataReader.Read())
                {
                    lblEntryCount.Text = General.GetString("BLOG_ENTRIES", "Entries", null) +
                                         " (" + (string) dataReader["EntryCount"].ToString() + ")";
                    lblCommentCount.Text = General.GetString("BLOG_COMMENTS", "Comments", null) +
                                           " (" + (string) dataReader["CommentCount"].ToString() + ")";
                }
            }
            finally
            {
                dataReader.Close();
            }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();

            //  - jminond
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
            this.dlComments.ItemCommand += new DataListCommandEventHandler(this.dlComments_ItemCommand);
            this.btnPostComment.Click += new EventHandler(this.btnPostComment_Click);
            this.Load += new EventHandler(this.Page_Load);
            if (!this.IsCssFileRegistered("Mod_Blogs"))
                this.RegisterCssFile("Mod_Blogs");
        }

        #endregion
    }
}