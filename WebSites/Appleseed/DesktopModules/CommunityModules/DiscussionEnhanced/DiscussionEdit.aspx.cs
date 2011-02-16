// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscussionEdit.aspx.cs" company="--">
//   Copyright © -- 2011. All Rights Reserved.
// </copyright>
// <summary>
//   The discussion edit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Content.Web.Modules.DiscussionEnhanced
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Web;

    using Appleseed.Framework;
    using Appleseed.Framework.Content.Data;
    using Appleseed.Framework.Content.Security;
    using Appleseed.Framework.Security;
    using Appleseed.Framework.Site.Configuration;
    using Appleseed.Framework.Web.UI;

    /// <summary>
    /// The discussion edit.
    /// </summary>
    [History("jminond", "2006/2/23", "Converted to partial class")]
    public partial class DiscussionEdit : AddEditItemPage
    {
        #region Properties

        /// <summary>
        ///   Set the module guids with free access to this page
        /// </summary>
        /// <value>The allowed modules.</value>
        protected override List<string> AllowedModules
        {
            get
            {
                var al = new List<string> { "2D86166C-4BDC-4A6F-A028-D17C2BB177C8" };
                return al;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load settings
        /// </summary>
        /// <remarks></remarks>
        protected override void LoadSettings()
        {
            // Verify that the current user has proper permissions for this module

            // need to re-enable this code as second level check in case users hack URLS to come to this page
            // if (PortalSecurity.HasEditPermissions(ModuleID) == false && PortalSecurity.IsInRoles("Admins") == false)
            // 	PortalSecurity.AccessDeniedEdit();

            // base.LoadSettings();
        }

        /// <summary>
        /// Handles OnInit event
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"></see> that contains the event data.
        /// </param>
        /// <remarks>
        /// </remarks>
        protected override void OnInit(EventArgs e)
        {
            this.submitButton.Click += this.SubmitBtn_Click;
            this.CancelButton.Click += this.CancelBtn_Click;
            this.Load += this.Page_Load;

            // Added EsperantusKeys for Localization 
            // Mario Endara mario@softworks.com.uy june-1-2004 
            this.Requiredfieldvalidator1.ErrorMessage = General.GetString("ERROR_SUBJECT");

            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Click event of the CancelBtn control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> instance containing the event data.
        /// </param>
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            // Response.Write("<script>window.close()</script>");  // close the edit browser window
            this.RedirectBackToReferringPage(); // go back to the discussion module page
        }

        /// <summary>
        /// Determine Mode from the URL
        ///   the mode can be Reply, Add, or Edit
        /// </summary>
        /// <returns>
        /// The get mode.
        /// </returns>
        private string GetMode()
        {
            if (HttpContext.Current != null && this.Request.Params["Mode"] != null)
            {
                return this.Request.Params["Mode"];
            }

            // true then its a new thread with no parent
            return this.ItemID == 0 ? "ADD" : string.Empty;
        }

        /// <summary>
        /// The Page_Load server event handler on this page is used
        ///   to obtain the ModuleID and ItemID of the discussion list,
        ///   and to then display the message contents.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> instance containing the event data.
        /// </param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Translations on the buttons, it doesn't appear there is a 
            // 		tra:LinkButton style supported
            this.submitButton.Text = General.GetString("SUBMIT");
            this.CancelButton.Text = General.GetString("CANCEL");

            // Populate message contents if this is the first visit to the page
            if (!this.Page.IsPostBack)
            {
                DiscussionDB discuss;
                SqlDataReader dr;

                switch (this.GetMode())
                {
                    case "REPLY":
                        if (PortalSecurity.HasAddPermissions(this.ModuleID) == false)
                        {
                            PortalSecurity.AccessDeniedEdit();
                        }

                        this.DiscussionEditInstructions.Text = General.GetString("DS_REPLYTHISMSG");

                        // Load fields for the item that we are replying to
                        discuss = new DiscussionDB();
                        dr = discuss.GetSingleMessage(this.ItemID);
                        try
                        {
                            if (dr.Read())
                            {
                                // Update labels with message contents
                                this.Title.Text = (string)dr["Title"];
                                this.Body.Text = (string)dr["Body"];
                                this.CreatedByUser.Text = (string)dr["CreatedByUser"];
                                this.CreatedDate.Text = string.Format("{0:d}", dr["CreatedDate"]);
                                this.TitleField.Text = string.Empty;

                                // don't give users a default subject for their reply

                                // encourage them to title their response
                                // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                                if (this.CreatedByUser.Text == "unknown")
                                {
                                    this.CreatedByUser.Text = General.GetString("UNKNOWN", "unknown");
                                }
                            }
                        }
                        finally
                        {
                            dr.Close();
                        }

                        break;

                    case "ADD":
                        if (PortalSecurity.HasAddPermissions(this.ModuleID) == false)
                        {
                            PortalSecurity.AccessDeniedEdit();
                        }

                        // hide the 'previous message' controls
                        this.OriginalMessagePanel.Visible = false;
                        break;

                    case "EDIT":
                        {
                            var itemUserEmail = string.Empty;

                            // hide the 'parent message' controls
                            this.OriginalMessagePanel.Visible = false;
                            this.DiscussionEditInstructions.Text = General.GetString("EDIT");

                            // Bind the data to the control
                            // Obtain the selected item from the Discussion table
                            discuss = new DiscussionDB();
                            dr = discuss.GetSingleMessage(this.ItemID);

                            try
                            {
                                // Load first row from database
                                if (dr.Read())
                                {
                                    // Update edit fields with message contents
                                    this.TitleField.Text = (string)dr["Title"];
                                    this.BodyField.Text = (string)dr["Body"];
                                    itemUserEmail = (string)dr["CreatedByUser"];

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

                            if (DiscussionPermissions.HasEditPermissions(this.ModuleID, itemUserEmail) == false)
                            {
                                PortalSecurity.AccessDeniedEdit();
                            }
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
        /// Handles the Click event of the SubmitBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void SubmitBtn_Click(object sender, EventArgs e)
        {
            this.OnUpdate(e);

            // Create new discussion database component
            var discuss = new DiscussionDB();

            // Add new message (updating the "ItemID" on the page)
            this.ItemID = discuss.AddMessage(
                this.ModuleID, 
                this.ItemID, 
                PortalSettings.CurrentUser.Identity.Email, 
                this.Server.HtmlEncode(this.TitleField.Text), 
                this.BodyField.Text, 
                this.GetMode());

            // The following code can be used if you want to create new threads
            // and responses in a separate browser window.  If this is the case
            // 1) uncomment the 2 ResponseWrite() lines below and comment the this.RedirectBackToReferringPage() call
            // 2) Make the 3 xxxTarget=_new changes commented in Discussion.ascx
            // 3) Switch the Response.Wrtie() and Redirect...() comments in CancelBtn_Click() in this file
            // refresh the parent window thread and close the edit window
            // Response.Write("<script>window.opener.navigate(window.opener.document.location.href);</script>"); // update the parent window discussions
            // Response.Write("<script>window.close()</script>");  // close the edit window
            this.RedirectBackToReferringPage();
        }

        #endregion
    }
}