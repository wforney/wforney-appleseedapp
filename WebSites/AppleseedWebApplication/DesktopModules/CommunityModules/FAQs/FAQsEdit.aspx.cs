//Namespaces added for editor and config settings
//Chris Farrell, 10/27/03, chris@cftechconsulting.com
using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// IBS Portal FAQ module - Edit page part
    /// (c)2002 by Christopher S Judd, CDP &amp; Horizons, LLC
    /// Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
    /// </summary>
    public partial class FAQsEdit : EditItemPage
    {
        #region Declarations

        private int itemID = -1;

        /// <summary>
        /// 
        /// </summary>
        protected IHtmlEditor DesktopText;

        #endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            //Editor placeholder setup
            HtmlEditorDataType h = new HtmlEditorDataType();
            h.Value = moduleSettings["Editor"].ToString();
            DesktopText =
                h.GetEditor(PlaceHolderHTMLEditor, ModuleID, bool.Parse(moduleSettings["ShowUpload"].ToString()),
                            portalSettings);

            DesktopText.Width = new Unit(moduleSettings["Width"].ToString());
            DesktopText.Height = new Unit(moduleSettings["Height"].ToString());


            //  Determine itemID of FAQ to Update
            if (Request.Params["itemID"] != null)
            {
                itemID = Int32.Parse(Request.Params["itemID"]);
            }

            //	populate with FAQ Details  
            if (Page.IsPostBack == false)
            {
                if (itemID != -1)
                {
                    //  get a single row of FAQ info
                    FAQsDB questions = new FAQsDB();
                    SqlDataReader dr = questions.GetSingleFAQ(itemID);

                    try
                    {
                        //  Read database
                        dr.Read();
                        Question.Text = (string) dr["Question"];
                        //Answer.Text = (string) dr["Answer"];
                        DesktopText.Text = (string) dr["Answer"];
                        CreatedBy.Text = (string) dr["CreatedByUser"];
                        CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToShortDateString();
                        // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                        if (CreatedBy.Text == "unknown" || CreatedBy.Text == string.Empty)
                        {
                            CreatedBy.Text = General.GetString("UNKNOWN", "unknown");
                        }
                    }
                    finally
                    {
                        dr.Close();
                    }
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
                al.Add("2502DB18-B580-4F90-8CB4-C15E6E531000");
                return al;
            }
        }

        /// <summary>
        /// Handles OnUpdate event
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);

            // Don't Allow empty data
            if (Question.Text == string.Empty || DesktopText.Text == string.Empty)
                return;

            //  Update only if entered data is valid
            if (Page.IsValid == true)
            {
                FAQsDB questions = new FAQsDB();

                if (itemID == -1)
                {
                    //  Add the question within the questions table
                    questions.AddFAQ(ModuleID, itemID, PortalSettings.CurrentUser.Identity.Email, Question.Text,
                                     DesktopText.Text);
                }
                else
                {
                    //  Update the question within the questions table
                    questions.UpdateFAQ(ModuleID, itemID, PortalSettings.CurrentUser.Identity.Email, Question.Text,
                                        DesktopText.Text);
                }

                RedirectBackToReferringPage();
            }
        }


        /// <summary>
        /// Handles OnDelete
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnDelete(EventArgs e)
        {
            base.OnDelete(e);

            //  Only attempt to delete the item if it is an existing item
            //  (new items will have "itemID" of -1)
            if (itemID != -1)
            {
                FAQsDB questions = new FAQsDB();
                questions.DeleteFAQ(itemID);
            }
            RedirectBackToReferringPage();
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.updateButton.Click += new EventHandler(updateButton_Click);
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }


        /// <summary>
        /// Handles the Click event of the updateButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void updateButton_Click(object sender, EventArgs e)
        {
            OnUpdate(e);
        }

        #endregion
    }
}