/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Original Coder: Indah Fuldner [indah@die-seitenweber.de]
 *                  modified by Mario Hartmann [mario@hartmann.net // http://mario.hartmann.net/]
 * Version: C#
 * Product name: Appleseed
 * Official site: http://www.Appleseedportal.net
 * Last updated Date: 04/JUN/2004
 * Derivate works, translation in other languages and binary distribution
 * of this code must retain this copyright notice and include the complete 
 * licence text that comes with product.
*/
using System;
using System.Collections;
using System.IO;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Web.UI;
using Appleseed.Framework.Web.UI.WebControls;
using Button=Appleseed.Framework.Web.UI.WebControls.Button;
using History=Appleseed.Framework.History;
using HyperLink=Appleseed.Framework.Web.UI.WebControls.HyperLink;
using Label=System.Web.UI.WebControls.Label;
using LinkButton=Appleseed.Framework.Web.UI.WebControls.LinkButton;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    [History("mario@hartmann.net", "2004/06/04", "Changed Flash movie control]")]
    [History("mario@hartmann.net", "2004/05/25", "Bug fixed:[ 877885 ] Flash Module - Cannot DELETE")]
    public partial class UploadFlash : AddEditItemPage
    {
        // Configuration
        private bool _uploadIsEnabled = true;
        private string _imageFolder = string.Empty;
        private string _returnPath = string.Empty;

        // Messages
        private string _noFileMessage = General.GetString("NO_FILE_MESSAGE");
        private string _uploadSuccessMessage = General.GetString("UPLOAD_SUCCESS_MESSAGE");
        private string _noImagesMessage = General.GetString("NO_IMAGE_MESSAGE");
        private string _noFolderSpecifiedMessage = General.GetString("NO_FOLDER_SPECIFIED_MESSAGE");

        protected Button closeButton;


        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Form the script that is to be registered at client side.
            string scriptString = "<script language=JavaScript>";
            scriptString += "function UpdateOpener(filename) {";
            scriptString += "opener.document.Form1." + Request.QueryString["FieldID"] + ".value = filename;";
            scriptString += "self.close(); return false; }";
            scriptString += "function closeWindow() {";
            scriptString += "opener.focus(); self.close(); return false;}";
            scriptString += "</script>";

            if (!ClientScript.IsClientScriptBlockRegistered("UpdateOpener"))
                ClientScript.RegisterClientScriptBlock(GetType(), "UpdateOpener", scriptString);

            uploadpanel.Visible = _uploadIsEnabled;

            this._imageFolder = ((SettingItem<string, TextBox>)this.ModuleSettings["FlashPath"]).FullPath;
            if (IOHelper.CreateDirectory(this.Server.MapPath(this._imageFolder)))
            {
                this.DisplayImages();
            }
            else
            {
                this.uploadmessage.Text = this._noFolderSpecifiedMessage;
            }

            Response.Write(_imageFolder);
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
                al.Add("623EC4DD-BA40-421c-887D-D774ED8EBF02");
                return al;
            }
        }

        /// <summary>
        /// Handles the Command event of the uploadButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        private void uploadButton_Command(object sender, CommandEventArgs e)
        {
            if (IOHelper.CreateDirectory(Server.MapPath(_imageFolder)))
            {
                if (uploadfile.PostedFile.FileName.Length != 0)
                {
                    try
                    {
                        string virtualPath = _imageFolder + "/" + Path.GetFileName(uploadfile.PostedFile.FileName);
                        string phyiscalPath = Server.MapPath(virtualPath);
                        uploadfile.PostedFile.SaveAs(phyiscalPath);
                        uploadmessage.Text = _uploadSuccessMessage;
                    }
                    catch (Exception exe)
                    {
                        uploadmessage.Text = (exe.Message);
                    }
                }
                else
                {
                    uploadmessage.Text = ("_noFileMessage");
                }
            }
            else
            {
                uploadmessage.Text = _noFolderSpecifiedMessage;
            }
            DisplayImages();
        }

        /// <summary>
        /// Handles the Command event of the Delete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        public void Delete_Command(object sender, CommandEventArgs e)
        {
            if (_imageFolder.Length != 0)
            {
                try
                {
                    File.Delete(Server.MapPath(_imageFolder) + @"\" + e.CommandArgument);
                }
                catch
                {
                }
                DisplayImages();
            }
        }

        /// <summary>
        /// ReturnFolderContentArray
        /// </summary>
        /// <returns></returns>
        private string[] ReturnFolderContentArray()
        {
            if (_imageFolder.Length != 0)
            {
                try
                {
                    string gallerytargetpath = Server.MapPath(_imageFolder);
                    string[] galleryfolderarray = IOHelper.GetFiles(gallerytargetpath, "*.swf");
                    return galleryfolderarray;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// DisplayImages
        /// </summary>
        public void DisplayImages()
        {
            this._returnPath = string.Format("~~/{0}", ((SettingItem<string, TextBox>)this.ModuleSettings["FlashPath"]).Value);
            string[] galleryfolderarray = ReturnFolderContentArray();
            flashTable.Controls.Clear();
            if (galleryfolderarray == null || galleryfolderarray.Length == 0)
            {
                gallerymessage.Text = _noImagesMessage;
            }
            else
            {
                string galleryfilename = (string.Empty);

                TableRow rowItem;
                TableCell cellItemImage;
                TableCell cellItemSelect;
                TableCell cellItemDelete;
                TableCell cellItemFileName;
                foreach (string galleryfolderarrayitem in galleryfolderarray)
                {
                    galleryfilename = galleryfolderarrayitem.ToString();
                    galleryfilename = galleryfilename.Substring(galleryfilename.LastIndexOf(@"\") + 1);

                    FlashMovie flashMovie = new FlashMovie();
                    flashMovie.MovieName = _imageFolder + "/" + galleryfilename;
                    flashMovie.MovieHeight = "150px";
                    flashMovie.MovieWidth = "150px";

                    Label filenameLbl = new Label();
                    filenameLbl.Text = galleryfilename;
                    HyperLink selectCmd = new HyperLink();
                    selectCmd.TextKey = "SELECT";
                    selectCmd.Text = "Select"; //by yiming
                    selectCmd.CssClass = "CommandButton";
                    selectCmd.NavigateUrl = "javascript:UpdateOpener('" + _returnPath + "/" + galleryfilename +
                                            "');self.close();";
                    LinkButton deleteCmd = new LinkButton();
                    deleteCmd.TextKey = "DELETE";
                    deleteCmd.Text = "Delete";
                    deleteCmd.CommandName = "DELETE";
                    deleteCmd.CssClass = "CommandButton";
                    deleteCmd.CommandArgument = galleryfilename;
                    deleteCmd.Command += new CommandEventHandler(Delete_Command);

                    rowItem = new TableRow();

                    cellItemImage = new TableCell();
                    cellItemSelect = new TableCell();
                    cellItemDelete = new TableCell();
                    cellItemFileName = new TableCell();
                    cellItemImage.Controls.Add(flashMovie);
                    cellItemFileName.Controls.Add(filenameLbl);
                    cellItemSelect.Controls.Add(selectCmd);
                    cellItemDelete.Controls.Add(deleteCmd);


                    rowItem.Controls.Add(cellItemImage);
                    rowItem.Controls.Add(cellItemFileName);
                    rowItem.Controls.Add(cellItemSelect);
                    rowItem.Controls.Add(cellItemDelete);

                    flashTable.Controls.Add(rowItem);
                    gallerymessage.Text = string.Empty;
                }
            }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Handles OnInit event
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.uploadButton.Command += new CommandEventHandler(this.uploadButton_Command);
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion 
    }
}