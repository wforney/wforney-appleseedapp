// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlashEdit.aspx.cs" company="--">
//   Copyright © -- 2011. All Rights Reserved.
// </copyright>
// <summary>
//   The flash edit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Content.Web.Modules
{
    using System;
    using System.Collections.Generic;

    using Appleseed.Framework.Site.Configuration;
    using Appleseed.Framework.Web.UI;

    /// <summary>
    /// The flash edit.
    /// </summary>
    public partial class FlashEdit : AddEditItemPage
    {
        #region Constants and Fields

        /// <summary>
        /// The show gallery.
        /// </summary>
        public string showGallery = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        ///   Set the module guids with free access to this page
        /// </summary>
        /// <value>The allowed modules.</value>
        protected override List<string> AllowedModules
        {
            get
            {
                var al = new List<string>();
                al.Add("623EC4DD-BA40-421c-887D-D774ED8EBF02");
                return al;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The CancelBtn_Click event handler on this Page is used to cancel
        ///   out of the page, and return the user back to the portal home page.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> instance containing the event data.
        /// </param>
        protected override void OnCancel(EventArgs e)
        {
            base.OnCancel(e);

            // Redirect back to the portal home page
            this.Response.Redirect((string)this.ViewState["UrlReferrer"]);
        }

        /// <summary>
        /// Handles OnInit event
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"></see> that contains the event data.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.           
            this.Load += this.Page_Load;
            base.OnInit(e);
        }

        /// <summary>
        /// The UpdateBtn_Click event handler on this Page is used to save
        ///   the settings to the ModuleSettings database table.  It  uses the
        ///   Appleseed.DesktopModulesDB() data component to encapsulate the data
        ///   access functionality.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> instance containing the event data.
        /// </param>
        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);

            // Update settings in the database
            Framework.Site.Configuration.ModuleSettings.UpdateModuleSetting(this.ModuleID, "src", this.Src.Text);
            Framework.Site.Configuration.ModuleSettings.UpdateModuleSetting(this.ModuleID, "height", this.Height.Text);
            Framework.Site.Configuration.ModuleSettings.UpdateModuleSetting(this.ModuleID, "width", this.Width.Text);
            Framework.Site.Configuration.ModuleSettings.UpdateModuleSetting(this.ModuleID, "backcolor", this.BackgroundCol.Text);

            // Redirect back to the portal home page
            this.Response.Redirect((string)this.ViewState["UrlReferrer"]);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> instance containing the event data.
        /// </param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Form the script that is to be registered at client side.
            var scriptString = "<script language=JavaScript>function newWindow(file,window) {";
            scriptString += "msgWindow=open(file,window,'resizable=yes,width=600,height=500,scrollbars=yes');";
            scriptString += "if (msgWindow.opener == null) msgWindow.opener = self;";
            scriptString += "}</script>";

            if (!this.ClientScript.IsClientScriptBlockRegistered("newWindow"))
            {
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "newWindow", scriptString);
            }

            this.showGalleryButton.NavigateUrl = "javascript:newWindow('UploadFlash.aspx?FieldID=Src&mID=" +
                                                 this.ModuleID + "','gallery')";

            if (this.Page.IsPostBack == false)
            {
                if (this.ModuleID > 0)
                {
                    // Get settings from the database
                    var settings = Framework.Site.Configuration.ModuleSettings.GetModuleSettings(this.ModuleID);

                    if (settings["src"] != null)
                    {
                        this.Src.Text = settings["src"].ToString();
                    }

                    if (settings["width"] != null)
                    {
                        this.Width.Text = settings["width"].ToString();
                    }

                    if (settings["height"] != null)
                    {
                        this.Height.Text = settings["height"].ToString();
                    }

                    if (settings["backcolor"] != null)
                    {
                        this.BackgroundCol.Text = settings["backcolor"].ToString();
                    }
                }

                // Store URL Referrer to return to portal
                this.ViewState["UrlReferrer"] = this.Request.UrlReferrer.ToString();
            }
        }

        #endregion
    }
}