using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Web.UI;
using History=Appleseed.Framework.History;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// Author:					Joe Audette
    /// Created:				1/18/2004
    /// Last Modified:			2/18/2004 (Jakob Hansen did localizing)
    /// </summary>
    [History("jminond", "2004/04/5", "Changes for moving Tab to Page")]
    public partial class ArchiveView : ViewItemPage
    {
        #region Declarations

        /// <summary>
        /// 
        /// </summary>
        protected DataList myDataList;

        /// <summary>
        /// 
        /// </summary>
        protected HtmlAnchor lnkRSS;

        /// <summary>
        /// 
        /// </summary>
        protected HtmlImage imgRSS;

        /// <summary>
        /// 
        /// </summary>
        protected Label lblEntryCount;

        /// <summary>
        /// 
        /// </summary>
        protected Label lblCommentCount;

        /// <summary>
        /// 
        /// </summary>
        protected Repeater dlArchive;

        /// <summary>
        /// 
        /// </summary>
        protected Label lblHeader;

        /// <summary>
        /// 
        /// </summary>
        protected Localize BlogPageLabel;

        /// <summary>
        /// 
        /// </summary>
        protected Localize SyndicationLabel;

        /// <summary>
        /// 
        /// </summary>
        protected Localize StatisticsLabel;

        /// <summary>
        /// 
        /// </summary>
        protected Localize ArchivesLabel;

        /// <summary>
        /// 
        /// </summary>
        protected Label lblCopyright;

        /// <summary>
        /// 
        /// </summary>
        protected string Feedback;

        /// <summary>
        /// 
        /// </summary>
        protected string sortField = "Title";

        /// <summary>
        /// 
        /// </summary>
        protected string sortOrder = "ASC";

        #endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Added EsperantusKeys for Localization 
            // Mario Endara mario@softworks.com.uy june-1-2004 
            //Feedback = General.GetString ("BLOG_FEEDBACK");
            Feedback = (String) HttpContext.GetGlobalResourceObject("Appleseed", "BLOG_FEEDBACK");

            if (!IsPostBack)
            {
                BindArchive();
            }
        }

        /// <summary>
        /// Binds the archive.
        /// </summary>
        private void BindArchive()
        {
            lnkRSS.HRef = HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Blog/RSS.aspx", PageID, "&mID=" + ModuleID);
            imgRSS.Src = HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Blog/xml.gif");
            lblCopyright.Text = this.ModuleSettings["Copyright"].ToString();

            BlogDB blogDB = new BlogDB();
            int month = -1;
            int year = -1;
            try
            {
                month = int.Parse(Request.Params.Get("month"));
                year = int.Parse(Request.Params.Get("year"));
            }
            catch
            {
            }

            if ((month > -1) && (year > -1))
            {
                lblHeader.Text = (String) HttpContext.GetGlobalResourceObject("Appleseed", "BLOG_POSTSFROM") +
                                 " " + DateTime.Parse(month.ToString() + "/1/" + year.ToString()).ToString("MMMM, yyyy");
                myDataList.DataSource = blogDB.GetBlogEntriesByMonth(month, year, ModuleID);
            }
            else
            {
                myDataList.DataSource = blogDB.GetBlogs(ModuleID);
            }
            myDataList.DataBind();

            dlArchive.DataSource = blogDB.GetBlogMonthArchive(ModuleID);
            dlArchive.DataBind();

            SqlDataReader dataReader = blogDB.GetBlogStats(ModuleID);
            try
            {
                if (dataReader.Read())
                {
                    lblEntryCount.Text = (String) HttpContext.GetGlobalResourceObject("Appleseed", "BLOG_ENTRIES") +
                                         " (" + dataReader["EntryCount"].ToString() + ")";
                    lblCommentCount.Text = (String) HttpContext.GetGlobalResourceObject("Appleseed", "BLOG_COMMENTS") +
                                           " (" + dataReader["CommentCount"].ToString() + ")";
                }
            }
            finally
            {
                dataReader.Close();
            }
        }

        #region Sorting

        /// <summary>
        /// BTNs the sort archive by title.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnSortArchiveByTitle(object sender, EventArgs e)
        {
            sortField = "Title";
            BindArchive();
        }

        /// <summary>
        /// BTNs the sort archive by date.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnSortArchiveByDate(object sender, EventArgs e)
        {
            sortField = "Date";
            BindArchive();
        }

        /// <summary>
        /// BTNs the sort archive by comments.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnSortArchiveByComments(object sender, EventArgs e)
        {
            sortField = "Comments";
            BindArchive();
        }

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// Handles OnInit event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            if (!this.IsCssFileRegistered("Mod_Blogs"))
                this.RegisterCssFile("Mod_Blogs");
        }

        #endregion
    }
}