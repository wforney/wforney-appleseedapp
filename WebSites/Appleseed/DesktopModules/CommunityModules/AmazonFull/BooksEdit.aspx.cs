using System;
using System.Data.SqlClient;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AmazonBooksEdit : EditItemPage
    {
        private void Page_Load(Object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                if (ItemID != 0)
                {
                    BooksDB bookDB = new BooksDB();
                    SqlDataReader dr = bookDB.GetSinglerb_BookList(ItemID);

                    // Load first row into DataReader
                    while (dr.Read())
                    {
                        if (dr["ISBN"] != DBNull.Value)
                            ISBNField.Text = dr["ISBN"].ToString();
                        if (dr["Caption"] != DBNull.Value)
                            CaptionTextBox.Text = dr["Caption"].ToString();
                        CreatedBy.Text = dr["CreatedByUser"].ToString();
                        CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToShortDateString();
                    }

                    // Close the datareader
                    dr.Close();
                }
                else
                {
                    CreatedDate.Text = DateTime.Now.ToShortDateString();
                    topDeleteButton.Visible = false; // Cannot delete an unexsistent item
                    bottomDeleteButton.Visible = false; // Cannot delete an unexsistent item
                }
            }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            if (!(this.Page.ClientScript.IsClientScriptBlockRegistered("confirmDelete")))
            {
                string[] s = {"CONFIRM_DELETE"};
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "confirmDelete",
                                                                 PortalSettings.GetStringResource(
                                                                     "CONFIRM_DELETE_SCRIPT",
                                                                     s));
            }

            if (topDeleteButton.Attributes["onclick"] != null)
                topDeleteButton.Attributes["onclick"] = "return confirmDelete();" +
                                                        topDeleteButton.Attributes["onclick"];
            else
                topDeleteButton.Attributes.Add("onclick", "return confirmDelete();");

            if (bottomDeleteButton.Attributes["onclick"] != null)
                bottomDeleteButton.Attributes["onclick"] = "return confirmDelete();" +
                                                           bottomDeleteButton.Attributes["onclick"];
            else
                bottomDeleteButton.Attributes.Add("onclick", "return confirmDelete();");

            this.topUpdateButton.Click += new EventHandler(this.UpdateButton_Click);
            this.topCancelButton.Click += new EventHandler(this.CancelButton_Click);
            this.topDeleteButton.Click += new EventHandler(this.DeleteButton_Click);
            this.bottomUpdateButton.Click += new EventHandler(this.UpdateButton_Click);
            this.bottomCancelButton.Click += new EventHandler(this.CancelButton_Click);
            this.bottomDeleteButton.Click += new EventHandler(this.DeleteButton_Click);
            this.Load += new EventHandler(this.Page_Load);

            base.OnInit(e);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUpdate(EventArgs e)
        {
            // Only Update if the Entered Data is Valid
            if (Page.IsValid == true)
            {
                Appleseed.Framework.Content.Data.BooksDB bookDB = new Appleseed.Framework.Content.Data.BooksDB();

                if (ItemID == 0)
                {
                    // Add the book within the books table
                    bookDB.Addrb_BookList(ModuleID, PortalSettings.CurrentUser.Identity.Email, ISBNField.Text,
                                          CaptionTextBox.Text);
                }
                else
                {
                    // Update the book
                    bookDB.Updaterb_BookList(ItemID, PortalSettings.CurrentUser.Identity.Email, ISBNField.Text,
                                             CaptionTextBox.Text);
                }

                // Redirect back to the portal home page
                this.RedirectBackToReferringPage();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDelete(EventArgs e)
        {
            if (ItemID != 0)
            {
                Appleseed.Framework.Content.Data.BooksDB bookDB = new Appleseed.Framework.Content.Data.BooksDB();
                bookDB.Deleterb_BookList(ItemID);
            }

            this.RedirectBackToReferringPage();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            this.OnUpdate(e);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.OnCancel(e);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            this.OnDelete(e);
        }
    }
}
