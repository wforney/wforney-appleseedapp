using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI;
using Appleseed.Framework.Web.UI.WebControls;
using History=Appleseed.Framework.History;
using LinkButton=Appleseed.Framework.Web.UI.WebControls.LinkButton;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    [History("Jes1111", "2003/03/04", "Cache flushing now handled by inherited page")]
    public partial class ArticlesEdit : AddEditItemPage
    {
        /// <summary>
        /// Html Text editor for the control
        /// </summary>
        protected IHtmlEditor DesktopText;

        /// <summary>
        /// Html Text editor for the control abstract
        /// </summary>
        protected IHtmlEditor AbstractText;

        /// <summary>
        /// The Page_Load event on this Page is used to obtain the ModuleID
        /// and ItemID of the Article to edit.
        /// It then uses the Appleseed.ArticlesDB() data component
        /// to populate the page's edit controls with the Article details.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Add the setting
            HtmlEditorDataType editor = new HtmlEditorDataType();
            editor.Value = moduleSettings["Editor"].ToString();
            DesktopText =
                editor.GetEditor(PlaceHolderHTMLEditor, ModuleID, bool.Parse(moduleSettings["ShowUpload"].ToString()),
                                 portalSettings);
            DesktopText.Width = new Unit(moduleSettings["Width"].ToString());
            DesktopText.Height = new Unit(moduleSettings["Height"].ToString());

            HtmlEditorDataType abstractEditor = new HtmlEditorDataType();
            if (moduleSettings["ARTICLES_RICHABSTRACT"] != null &&
                bool.Parse(moduleSettings["ARTICLES_RICHABSTRACT"].ToString()))
            {
                abstractEditor.Value = moduleSettings["Editor"].ToString();
                AbstractText =
                    abstractEditor.GetEditor(PlaceHolderAbstractHTMLEditor, ModuleID,
                                             bool.Parse(moduleSettings["ShowUpload"].ToString()), portalSettings);
            }
            else
            {
                abstractEditor.Value = "Plain Text";
                AbstractText =
                    abstractEditor.GetEditor(PlaceHolderAbstractHTMLEditor, ModuleID,
                                             bool.Parse(moduleSettings["ShowUpload"].ToString()), portalSettings);
            }
            AbstractText.Width = new Unit(moduleSettings["Width"].ToString());
            AbstractText.Height = new Unit("130px");

            // Construct the page
            // Added css Styles by Mario Endara <mario@softworks.com.uy> (2004/10/26)
            updateButton.CssClass = "CommandButton";
            PlaceHolderButtons.Controls.Add(updateButton);
            PlaceHolderButtons.Controls.Add(new LiteralControl("&#160;"));
            cancelButton.CssClass = "CommandButton";
            PlaceHolderButtons.Controls.Add(cancelButton);
            PlaceHolderButtons.Controls.Add(new LiteralControl("&#160;"));
            deleteButton.CssClass = "CommandButton";
            PlaceHolderButtons.Controls.Add(deleteButton);
            // If the page is being requested the first time, determine if an
            // Article itemID value is specified, and if so populate page
            // contents with the Article details
            if (Page.IsPostBack == false)
            {
                if (ItemID != 0)
                {
                    // Obtain a single row of Article information
                    ArticlesDB Articles = new ArticlesDB();
                    SqlDataReader dr = Articles.GetSingleArticle(ItemID, WorkFlowVersion.Staging);
                    try
                    {
                        // Load first row into Datareader
                        if (dr.Read())
                        {
                            StartField.Text = ((DateTime) dr["StartDate"]).ToShortDateString();
                            ExpireField.Text = ((DateTime) dr["ExpireDate"]).ToShortDateString();
                            TitleField.Text = (string) dr["Title"].ToString();
                            SubtitleField.Text = (string) dr["Subtitle"].ToString();
                            AbstractText.Text = (string) dr["Abstract"].ToString();
                            DesktopText.Text = Server.HtmlDecode(dr["Description"].ToString());
                            CreatedBy.Text = (string) dr["CreatedByUser"].ToString();
                            CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToString();
                            // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                            if (CreatedBy.Text == "unknown")
                            {
                                CreatedBy.Text = General.GetString("UNKNOWN", "unknown");
                            }
                        }
                    }
                    finally
                    {
                        dr.Close();
                    }
                }
                else
                {
                    //New article - set defaults
                    StartField.Text = DateTime.Now.ToShortDateString();
                    ExpireField.Text =
                        DateTime.Now.AddDays(int.Parse(moduleSettings["DefaultVisibleDays"].ToString())).
                            ToShortDateString();
                    CreatedBy.Text = PortalSettings.CurrentUser.Identity.Email;
                    CreatedDate.Text = DateTime.Now.ToString();
                }
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
                al.Add("87303CF7-76D0-49B1-A7E7-A5C8E26415BA"); //Articles
                al.Add("5B7B52D3-837C-4942-A85C-AAF4B5CC098F"); //ArticlesInline
                return al;
            }
        }

        /// <summary>
        /// Is used to either create or update an Article.
        /// It uses the Appleseed.ArticlesDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);

            // Only Update if Input Data is Valid
            if (Page.IsValid == true)
            {
                ArticlesDB Articles = new ArticlesDB();

                if (AbstractText.Text == string.Empty)
                {
                    AbstractText.Text = ((HTMLText) DesktopText.Text).GetAbstractText(500);
                }
                if (ItemID == 0)
                {
                    Articles.AddArticle(ModuleID, PortalSettings.CurrentUser.Identity.Email,
                                        ((HTMLText) TitleField.Text).InnerText,
                                        ((HTMLText) SubtitleField.Text).InnerText, AbstractText.Text,
                                        Server.HtmlEncode(DesktopText.Text), DateTime.Parse(StartField.Text),
                                        DateTime.Parse(ExpireField.Text), true, string.Empty);
                }
                else
                {
                    Articles.UpdateArticle(ModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email,
                                           ((HTMLText) TitleField.Text).InnerText,
                                           ((HTMLText) SubtitleField.Text).InnerText, AbstractText.Text,
                                           Server.HtmlEncode(DesktopText.Text), DateTime.Parse(StartField.Text),
                                           DateTime.Parse(ExpireField.Text), true, string.Empty);
                }
                RedirectBackToReferringPage();
            }
        }

        /// <summary>
        /// The DeleteBtn_Click event handler on this Page is used to delete an
        /// a Article.  It  uses the Appleseed.ArticlesDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDelete(EventArgs e)
        {
            base.OnDelete(e);
            // Only attempt to delete the item if it is an existing item
            // (new items will have "ItemID" of 0)
            if (ItemID != 0)
            {
                ArticlesDB Articles = new ArticlesDB();
                Articles.DeleteArticle(ItemID);
            }
            RedirectBackToReferringPage();
        }

        #region Web Form Designer generated code

        /// <summary>
        /// On Init
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            //Controls must be created here
            updateButton = new LinkButton();
            cancelButton = new LinkButton();
            deleteButton = new LinkButton();

            updateButton.Click += new EventHandler(updateButton_Click);
            deleteButton.Click += new EventHandler(deleteButton_Click);

            InitializeComponent();
            base.OnInit(e);
        }

        void deleteButton_Click(object sender, EventArgs e)
        {
            OnDelete(e);
        }

        void updateButton_Click(object sender, EventArgs e)
        {
            OnUpdate(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            
        }

        #endregion
    }
}