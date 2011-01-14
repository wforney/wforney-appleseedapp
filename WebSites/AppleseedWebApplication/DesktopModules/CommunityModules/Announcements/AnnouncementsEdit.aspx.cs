using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI;
using Appleseed.Framework.Web.UI.WebControls;
using History=Appleseed.Framework.History;
using LinkButton=Appleseed.Framework.Web.UI.WebControls.LinkButton;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// Announcmeent Edit
    /// </summary>
    [History("Jes1111", "2003/03/04", "Cache flushing now handled by inherited page")]
    public partial class AnnouncementsEdit : AddEditItemPage
    {
        #region Declarations

        /// <summary>
        /// 
        /// </summary>
        protected IHtmlEditor DesktopText;

        #endregion

        /// <summary>
        /// The Page_Load event on this Page is used to obtain the ModuleID
        /// and ItemID of the announcement to edit.
        /// It then uses the Appleseed.AnnouncementsDB() data component
        /// to populate the page's edit controls with the annoucement details.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // If the page is being requested the first time, determine if an
            // announcement itemID value is specified, and if so populate page
            // contents with the announcement details

            //Indah Fuldner
            HtmlEditorDataType h = new HtmlEditorDataType();
            h.Value = moduleSettings["Editor"].ToString();
            DesktopText =
                h.GetEditor(PlaceHolderHTMLEditor, ModuleID, bool.Parse(moduleSettings["ShowUpload"].ToString()),
                            portalSettings);

            DesktopText.Width = new Unit(moduleSettings["Width"].ToString());
            DesktopText.Height = new Unit(moduleSettings["Height"].ToString());
            //End Indah Fuldner

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

            if (Page.IsPostBack == false)
            {
                if (ItemID != 0)
                {
                    // Obtain a single row of announcement information
                    AnnouncementsDB announcementDB = new AnnouncementsDB();
                    SqlDataReader dr = announcementDB.GetSingleAnnouncement(ItemID, WorkFlowVersion.Staging);

                    try
                    {
                        // Load first row into DataReader
                        if (dr.Read())
                        {
                            TitleField.Text = (string) dr["Title"];
                            MoreLinkField.Text = (string) dr["MoreLink"];
                            MobileMoreField.Text = (string) dr["MobileMoreLink"];
                            DesktopText.Text = (string) dr["Description"];
                            ExpireField.Text = ((DateTime) dr["ExpireDate"]).ToShortDateString();
                            CreatedBy.Text = (string) dr["CreatedByUser"];
                            CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToShortDateString();
                            // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                            if (CreatedBy.Text == "unknown")
                            {
                                CreatedBy.Text = General.GetString("UNKNOWN", "unknown");
                            }
                        }
                    }
                    finally
                    {
                        // Close the datareader
                        dr.Close();
                    }
                }
                else
                {
                    ExpireField.Text =
                        DateTime.Now.AddDays(Int32.Parse(moduleSettings["DelayExpire"].ToString())).ToShortDateString();
                    deleteButton.Visible = false; // Cannot delete an unexsistent item
                }
            }
        }

        /// <summary>
        /// Set the module guids with free access to this page
        /// </summary>
        /// <value>The allowed modules.</value>
        protected override List<string> AllowedModules
        {
            get
            {
                List<string> al = new List<string>();
                al.Add("CE55A821-2449-4903-BA1A-EC16DB93F8DB");
                return al;
            }
        }

        /// <summary>
        /// The UpdateBtn_Click event handler on this Page is used to either
        /// create or update an announcement.  It  uses the Appleseed.AnnouncementsDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);

            // Only Update if the Entered Data is Valid
            if (Page.IsValid == true)
            {
                // Create an instance of the Announcement DB component
                AnnouncementsDB announcementDB = new AnnouncementsDB();

                if (ItemID == 0)
                {
                    // Add the announcement within the Announcements table
                    announcementDB.AddAnnouncement(ModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email,
                                                   TitleField.Text, DateTime.Parse(ExpireField.Text), DesktopText.Text,
                                                   MoreLinkField.Text, MobileMoreField.Text);
                }
                else
                {
                    // Update the announcement within the Announcements table
                    announcementDB.UpdateAnnouncement(ModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email,
                                                      TitleField.Text, DateTime.Parse(ExpireField.Text),
                                                      DesktopText.Text, MoreLinkField.Text, MobileMoreField.Text);
                }

                // Redirect back to the portal home page
                RedirectBackToReferringPage();
            }
        }

        /// <summary>
        /// The DeleteBtn_Click event handler on this Page is used to delete an
        /// an announcement.  It  uses the Appleseed.AnnouncementsDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        protected override void OnDelete(EventArgs e)
        {
            base.OnDelete(e);
            // Only attempt to delete the item if it is an existing item
            // (new items will have "ItemID" of 0)
            if (ItemID != 0)
            {
                AnnouncementsDB announcementDB = new AnnouncementsDB();
                announcementDB.DeleteAnnouncement(ItemID);
            }

            // Redirect back to the portal home page
            RedirectBackToReferringPage();
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            //Controls must be created here
            updateButton = new LinkButton();
            cancelButton = new LinkButton();
            deleteButton = new LinkButton();

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
        }

        #endregion
    }
}