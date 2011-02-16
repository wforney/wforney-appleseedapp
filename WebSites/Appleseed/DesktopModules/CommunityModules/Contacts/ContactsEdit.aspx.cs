using System;
using System.Collections;
using System.Data.SqlClient;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI;
using History=Appleseed.Framework.History;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// Page for editing contacts
    /// </summary>
    [History("Jes1111", "2003/03/04", "Cache flushing now handled by inherited page")]
    public partial class ContactsEdit : AddEditItemPage
    {
        #region Declarations

        #endregion

        /// <summary>
        /// The Page_Load event on this Page is used to obtain the ModuleID
        /// and ItemID of the contact to edit.
        /// It then uses the Appleseed.ContactsDB() data component
        /// to populate the page's edit controls with the contact details.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // If the page is being requested the first time, determine if an
            // contact itemID value is specified, and if so populate page
            // contents with the contact details
            if (Page.IsPostBack == false)
            {
                if (ItemID != 0)
                {
                    // Obtain a single row of contact information
                    ContactsDB contacts = new ContactsDB();
                    SqlDataReader dr = contacts.GetSingleContact(ItemID, WorkFlowVersion.Staging);

                    try
                    {
                        // Read first row from database
                        if (dr.Read())
                        {
                            NameField.Text = (dr["Name"] == DBNull.Value) ? string.Empty : (string) (dr["Name"]);
                            RoleField.Text = (dr["Role"] == DBNull.Value) ? string.Empty : (string) (dr["Role"]);
                            EmailField.Text = (dr["Email"] == DBNull.Value) ? string.Empty : (string) (dr["Email"]);
                            Contact1Field.Text = (dr["Contact1"] == DBNull.Value)
                                                     ? string.Empty
                                                     : (string) (dr["Contact1"]);
                            Contact2Field.Text = (dr["Contact2"] == DBNull.Value)
                                                     ? string.Empty
                                                     : (string) (dr["Contact2"]);
                            FaxField.Text = (dr["Fax"] == DBNull.Value) ? string.Empty : (string) (dr["Fax"]);
                            AddressField.Text = (dr["Address"] == DBNull.Value)
                                                    ? string.Empty
                                                    : (string) (dr["Address"]);
                            CreatedBy.Text = (dr["CreatedByUser"] == DBNull.Value)
                                                 ? string.Empty
                                                 : (string) (dr["CreatedByUser"]);
                            CreatedDate.Text = (dr["CreatedDate"] == DBNull.Value)
                                                   ? DateTime.Now.ToShortDateString()
                                                   : ((DateTime) dr["CreatedDate"]).ToShortDateString();
                            // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                            if (CreatedBy.Text == "unknown")
                            {
                                CreatedBy.Text = General.GetString("UNKNOWN", "unknown");
                            }
                        }
                    }
                    finally
                    {
                        // Close datareader
                        dr.Close();
                    }
                }
                else
                {
                    this.DeleteButton.Visible = false; // Cannot delete an unexsistent item
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
                al.Add("2502DB18-B580-4F90-8CB4-C15E6E5339EF");
                return al;
            }
        }

        /// <summary>
        /// The UpdateBtn_Click event handler on this Page is used to either
        /// create or update a contact.  It  uses the Appleseed.ContactsDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);

            // Only Update if Entered data is Valid
            if (Page.IsValid == true)
            {
                // Create an instance of the ContactsDB component
                ContactsDB contacts = new ContactsDB();

                if (ItemID == 0)
                {
                    // Add the contact within the contacts table
                    contacts.AddContact(ModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email, NameField.Text,
                                        RoleField.Text, EmailField.Text, Contact1Field.Text, Contact2Field.Text,
                                        FaxField.Text, AddressField.Text);
                }
                else
                {
                    // Update the contact within the contacts table
                    contacts.UpdateContact(ModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email, NameField.Text,
                                           RoleField.Text, EmailField.Text, Contact1Field.Text, Contact2Field.Text,
                                           FaxField.Text, AddressField.Text);
                }

                // Redirect back to the portal home page
                RedirectBackToReferringPage();
            }
        }

        /// <summary>
        /// The DeleteBtn_Click event handler on this Page is used to delete an
        /// a contact.  It  uses the Appleseed.ContactsDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnDelete(EventArgs e)
        {
            base.OnDelete(e);

            // Only attempt to delete the item if it is an existing item
            // (new items will have "ItemID" of 0)

            if (ItemID != 0)
            {
                ContactsDB contacts = new ContactsDB();
                contacts.DeleteContact(ItemID);
            }

            // Redirect back to the portal home page
            RedirectBackToReferringPage();
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            //Translate
            RequiredName.ErrorMessage = General.GetString("CONTACTS_VALID_NAME");

            this.Load += new EventHandler(this.Page_Load);

            // Change by Geert.Audenaert@Syntegra.Com
            // Date: 10/2/2003
            base.UpdateButton = this.UpdateButton;
            base.DeleteButton = this.DeleteButton;
            base.CancelButton = this.CancelButton;
            // End Change Geert.Audenaert@Syntegra.Com

            base.OnInit(e);
        }

        #endregion
    }
}