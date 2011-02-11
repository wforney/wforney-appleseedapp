using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Content.Security;
using Appleseed.Framework.Security;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI;
using History=Appleseed.Framework.History;

namespace Appleseed.Content.Web.Modules.DiscussionEnhanced
{
    using System.Collections.Generic;

    [History("jminond", "2006/2/23", "Converted to partial class")]
    public partial class DiscussionEdit : AddEditItemPage
    {
        /// <summary>
        /// The Page_Load server event handler on this page is used
        /// to obtain the ModuleID and ItemID of the discussion list,
        /// and to then display the message contents.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            //Translations on the buttons, it doesn't appear there is a 
            //		tra:LinkButton style supported
            submitButton.Text = General.GetString("SUBMIT");
            cancelButton.Text = General.GetString("CANCEL");

            // Populate message contents if this is the first visit to the page
            if (Page.IsPostBack == false)
            {
                DiscussionDB discuss;
                SqlDataReader dr;

                switch (GetMode())
                {
                    case "REPLY":
                        if (PortalSecurity.HasAddPermissions(ModuleID) == false)
                            PortalSecurity.AccessDeniedEdit();

                        DiscussionEditInstructions.Text = General.GetString("DS_REPLYTHISMSG");

                        // Load fields for the item that we are replying to
                        discuss = new DiscussionDB();
                        dr = discuss.GetSingleMessage(ItemID);
                        try
                        {
                            if (dr.Read())
                            {
                                // Update labels with message contents
                                this.Title.Text = (string) dr["Title"];
                                Body.Text = (string) dr["Body"];
                                CreatedByUser.Text = (string) dr["CreatedByUser"];
                                CreatedDate.Text = string.Format("{0:d}", dr["CreatedDate"]);
                                TitleField.Text = string.Empty; // don't give users a default subject for their reply
                                // encourage them to title their response
                                // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                                if (CreatedByUser.Text == "unknown")
                                {
                                    CreatedByUser.Text = General.GetString("UNKNOWN", "unknown");
                                }
                            }
                        }
                        finally
                        {
                            dr.Close();
                        }
                        break;

                    case "ADD":
                        if (PortalSecurity.HasAddPermissions(ModuleID) == false)
                            PortalSecurity.AccessDeniedEdit();

                        // hide the 'previous message' controls
                        OriginalMessagePanel.Visible = false;
                        break;


                    case "EDIT":
                        {
                            string itemUserEmail = string.Empty;
                            // hide the 'parent message' controls
                            OriginalMessagePanel.Visible = false;
                            DiscussionEditInstructions.Text = General.GetString("EDIT");

                            // Bind the data to the control
                            // Obtain the selected item from the Discussion table
                            discuss = new DiscussionDB();
                            dr = discuss.GetSingleMessage(ItemID);

                            try
                            {
                                // Load first row from database
                                if (dr.Read())
                                {
                                    // Update edit fields with message contents
                                    TitleField.Text = (string) dr["Title"];
                                    BodyField.Text = (string) dr["Body"];
                                    itemUserEmail = (string) dr["CreatedByUser"];
                                    // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                                    if (itemUserEmail == "unknown")
                                    {
                                        itemUserEmail = General.GetString("UNKNOWN", "unknown");
                                    }
                                }
                            }
                            finally
                            {
                                dr.Close();
                            }

                            if (DiscussionPermissions.HasEditPermissions(ModuleID, itemUserEmail) == false)
                                PortalSecurity.AccessDeniedEdit();
                        }
                        break;

                        /* case "DELETE":
                        if (PortalSecurity.HasDeletePermissions(ModuleID) == false)
                            PortalSecurity.AccessDeniedEdit();
                        break;
                        */

                    default:
                        // invalid mode specified
                        PortalSecurity.AccessDeniedEdit();
                        break;
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
                al.Add("2D86166C-4BDC-4A6F-A028-D17C2BB177C8");
                return al;
            }
        }

        /// <summary>
        /// Determine Mode from the URL
        /// the mode can be Reply, Add, or Edit
        /// </summary>
        /// <returns></returns>
        private string GetMode()
        {
            if (HttpContext.Current != null && Request.Params["Mode"] != null)
                return Request.Params["Mode"];
            else
            {
                if (ItemID == 0)
                    return "ADD"; // its a new thread with no parent
                else
                    return string.Empty;
            }
        }

        //*******************************************************
        //
        // The SubmitBtn_Click server event handler on this page is used
        // to handle the scenario where a user clicks the "submit"
        // button after entering a response to a message post.
        //
        //*******************************************************

        /// <summary>
        /// Handles the Click event of the SubmitBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void SubmitBtn_Click(Object sender, EventArgs e)
        {
            base.OnUpdate(e);


            // Create new discussion database component
            DiscussionDB discuss = new DiscussionDB();

            // Add new message (updating the "ItemID" on the page)
            ItemID =
                discuss.AddMessage(ModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email,
                                   Server.HtmlEncode(TitleField.Text), BodyField.Text, GetMode());

            // The following code can be used if you want to create new threads
            // and responses in a separate browser window.  If this is the case
            // 1) uncomment the 2 ResponseWrite() lines below and comment the this.RedirectBackToReferringPage() call
            // 2) Make the 3 xxxTarget=_new changes commented in Discussion.ascx
            // 3) Switch the Response.Wrtie() and Redirect...() comments in CancelBtn_Click() in this file
            //
            // refresh the parent window thread and close the edit window
            // Response.Write("<script>window.opener.navigate(window.opener.document.location.href);</script>"); // update the parent window discussions
            // Response.Write("<script>window.close()</script>");  // close the edit window
            RedirectBackToReferringPage();
        }


        // *******************************************************
        //
        // The CancelBtn_Click server event handler on this page is used
        // to handle the scenario where a user clicks the "cancel"
        // button to discard a message post and toggle out of
        // edit mode.
        //
        // *******************************************************

        /// <summary>
        /// Handles the Click event of the CancelBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void CancelBtn_Click(Object sender, EventArgs e)
        {
            // Response.Write("<script>window.close()</script>");  // close the edit browser window
            RedirectBackToReferringPage(); // go back to the discussion module page
        }

        /// <summary>
        /// Load settings
        /// </summary>
        protected override void LoadSettings()
        {
            // Verify that the current user has proper permissions for this module

            // need to reanable this code as second level check in case users hack URLS to come to this page
            // if (PortalSecurity.HasEditPermissions(ModuleID) == false && PortalSecurity.IsInRoles("Admins") == false)
            //	PortalSecurity.AccessDeniedEdit();

            // base.LoadSettings();
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.submitButton.Click += new System.EventHandler(this.SubmitBtn_Click);
            this.cancelButton.Click += new System.EventHandler(this.CancelBtn_Click);
            this.Load += new System.EventHandler(this.Page_Load);

            // Added EsperantusKeys for Localization 
            // Mario Endara mario@softworks.com.uy june-1-2004 
            Requiredfieldvalidator1.ErrorMessage = General.GetString("ERROR_SUBJECT");

            base.OnInit(e);
        }

        #endregion
    }
}