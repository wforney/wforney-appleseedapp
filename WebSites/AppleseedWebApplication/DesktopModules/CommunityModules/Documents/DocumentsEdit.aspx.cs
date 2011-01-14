using System;
using System.Collections;
using System.Data.SqlClient;
using System.IO;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI;
using History=Appleseed.Framework.History;
using Path=Appleseed.Framework.Settings.Path;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// Update and edit documents.
    /// Update 14 nov 2002 - Bug on buttonclick events
    /// </summary>
    [History("Jes1111", "2003/03/04", "Cache flushing now handled by inherited page")]
    [History("jviladiu@portalServices.net", "2004/07/02", "Corrections for save documents in database")]
    public partial class DocumentsEdit : AddEditItemPage
    {
        #region Declarations

        /// <summary>
        /// 
        /// </summary>
        private string PathToSave;

        #endregion

        /// <summary>
        /// The Page_Load event on this Page is used to obtain the ModuleID
        /// and ItemID of the document to edit.
        /// It then uses the DocumentDB() data component
        /// to populate the page's edit controls with the document details.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // If the page is being requested the first time, determine if an
            // document itemID value is specified, and if so populate page
            // contents with the document details

            if (!Page.IsPostBack)
            {
                if (ModuleID > 0)
                    PathToSave = ((SettingItem) moduleSettings["DocumentPath"]).FullPath;

                if (ItemID > 0)
                {
                    // Obtain a single row of document information
                    DocumentDB documents = new DocumentDB();
                    SqlDataReader dr = documents.GetSingleDocument(ItemID, WorkFlowVersion.Staging);

                    try
                    {
                        // Load first row into Datareader
                        if (dr.Read())
                        {
                            NameField.Text = (string) dr["FileFriendlyName"];
                            PathField.Text = (string) dr["FileNameUrl"];
                            CategoryField.Text = (string) dr["Category"];
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
                        dr.Close();
                    }
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
                al.Add("F9645B82-CB45-4C4C-BB2D-72FA42FE2B75");
                return al;
            }
        }

        /// <summary>
        /// The UpdateBtn_Click event handler on this Page is used to either
        /// create or update an document.  It  uses the DocumentDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        [History("jviladiu@portalServices.net", "2004/07/02", "Corrections for save documents in database")]
        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);
            byte[] buffer = new byte[0];
            int size = 0;

            // Only Update if Input Data is Valid
            if (Page.IsValid)
            {
                // Create an instance of the Document DB component
                DocumentDB documents = new DocumentDB();

                // Determine whether a file was uploaded
                if (FileUpload.PostedFile.FileName != string.Empty)
                {
                    FileInfo fInfo = new FileInfo(FileUpload.PostedFile.FileName);
                    if (bool.Parse(moduleSettings["DOCUMENTS_DBSAVE"].ToString()))
                    {
                        Stream stream = FileUpload.PostedFile.InputStream;
                        buffer = new byte[FileUpload.PostedFile.ContentLength];
                        size = FileUpload.PostedFile.ContentLength;
                        try
                        {
                            stream.Read(buffer, 0, size);
                            PathField.Text = fInfo.Name;
                        }
                        finally
                        {
                            stream.Close(); //by manu
                        }
                    }
                    else
                    {
                        PathToSave = ((SettingItem) moduleSettings["DocumentPath"]).FullPath;
                        // jviladiu@portalServices.net (02/07/2004). Create the Directory if not exists.
                        if (!Directory.Exists(Server.MapPath(PathToSave)))
                            Directory.CreateDirectory(Server.MapPath(PathToSave));

                        string virtualPath = Path.WebPathCombine(PathToSave, fInfo.Name);
                        string physicalPath = Server.MapPath(virtualPath);

//						while(System.IO.File.Exists(physicalPath))
//						{
//							// Calculate virtualPath of the newly uploaded file
//							virtualPath = Appleseed.Framework.Settings.Path.WebPathCombine(PathToSave, Guid.NewGuid().ToString() + fInfo.Extension);
//
//							// Calculate physical path of the newly uploaded file
//							phyiscalPath = Server.MapPath(virtualPath);
//						}
                        while (File.Exists(physicalPath))
                        {
                            try
                            {
                                // Delete file before upload
                                File.Delete(physicalPath);
                            }
                            catch (Exception ex)
                            {
                                Message.Text =
                                    General.GetString("ERROR_FILE_DELETE", "Error while deleting file!<br>") +
                                    ex.Message;
                                return;
                            }
                        }

                        try
                        {
                            // Save file to uploads directory
                            FileUpload.PostedFile.SaveAs(physicalPath);

                            // Update PathFile with uploaded virtual file location
                            PathField.Text = virtualPath;
                        }
                        catch (Exception ex)
                        {
                            Message.Text = General.GetString("ERROR_FILE_NAME", "Invalid file name!<br>") + ex.Message;
                            return;
                        }
                    }
                }
                // Change for save contenType and document buffer
                // documents.UpdateDocument(ModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email, NameField.Text, PathField.Text, CategoryField.Text, new byte[0], 0, string.Empty );
                string contentType = PathField.Text.Substring(PathField.Text.LastIndexOf(".") + 1).ToLower();
                documents.UpdateDocument(ModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email, NameField.Text,
                                         PathField.Text, CategoryField.Text, buffer, size, contentType);

                RedirectBackToReferringPage();
            }
        }

        /// <summary>
        /// The DeleteBtn_Click event handler on this Page is used to delete an
        /// a document. It uses the Appleseed.DocumentsDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnDelete(EventArgs e)
        {
            base.OnDelete(e);

            // Only attempt to delete the item if it is an existing item
            // (new items will have "ItemID" of 0)
            //TODO: Ask confim before delete
            if (ItemID != 0)
            {
                DocumentDB documents = new DocumentDB();
                documents.DeleteDocument(ItemID, Server.MapPath(PathField.Text));
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
            this.Load += new EventHandler(this.Page_Load);

            //Translate
            CreatedLabel.Text = General.GetString("CREATED_BY");
            OnLabel.Text = General.GetString("ON");

            PageTitleLabel.Text = General.GetString("DOCUMENT_DETAILS");
            FileNameLabel.Text = General.GetString("FILE_NAME");
            CategoryLabel.Text = General.GetString("CATEGORY");
            UrlLabel.Text = General.GetString("URL");
            UploadLabel.Text = General.GetString("UPLOAD_FILE");
            OrLabel.Text = "---" + General.GetString("OR") + "---";

            RequiredFieldValidator1.Text = General.GetString("VALID_FILE_NAME");

            base.OnInit(e);
        }

        #endregion
    }
}