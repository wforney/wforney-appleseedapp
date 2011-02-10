/*
 * This code is released under Duemetri Public License (DPL) Version 1.2.
 * Original Coder: Indah Fuldner [indah@die-seitenweber.de]
 * modified by Mario Hartmann [mario@hartmann.net // http://mario.hartmann.net/]
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
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public partial class FlashEdit : AddEditItemPage
    {
        public string showGallery = string.Empty;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Form the script that is to be registered at client side.
            string scriptString = "<script language=JavaScript>function newWindow(file,window) {";
            scriptString += "msgWindow=open(file,window,'resizable=yes,width=600,height=500,scrollbars=yes');";
            scriptString += "if (msgWindow.opener == null) msgWindow.opener = self;";
            scriptString += "}</script>";

            if (!ClientScript.IsClientScriptBlockRegistered("newWindow"))
                ClientScript.RegisterClientScriptBlock(GetType(), "newWindow", scriptString);

            showGalleryButton.NavigateUrl = "javascript:newWindow('UploadFlash.aspx?FieldID=Src&mID=" + ModuleID +
                                            "','gallery')";

            if (Page.IsPostBack == false)
            {
                if (ModuleID > 0)
                {
                    Hashtable settings;

                    // Get settings from the database
                    settings = ModuleSettings.GetModuleSettings(ModuleID);

                    if (settings["src"] != null)
                        Src.Text = settings["src"].ToString();
                    if (settings["width"] != null)
                        Width.Text = settings["width"].ToString();
                    if (settings["height"] != null)
                        Height.Text = settings["height"].ToString();
                    if (settings["backcolor"] != null)
                        BackgroundCol.Text = settings["backcolor"].ToString();
                }

                // Store URL Referrer to return to portal
                ViewState["UrlReferrer"] = Request.UrlReferrer.ToString();
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
                al.Add("623EC4DD-BA40-421c-887D-D774ED8EBF02");
                return al;
            }
        }

        /// <summary>
        /// The UpdateBtn_Click event handler on this Page is used to save
        /// the settings to the ModuleSettings database table.  It  uses the
        /// Appleseed.DesktopModulesDB() data component to encapsulate the data
        /// access functionality.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);

            // Update settings in the database
            ModuleSettings.UpdateModuleSetting(ModuleID, "src", Src.Text);
            ModuleSettings.UpdateModuleSetting(ModuleID, "height", Height.Text);
            ModuleSettings.UpdateModuleSetting(ModuleID, "width", Width.Text);
            ModuleSettings.UpdateModuleSetting(ModuleID, "backcolor", BackgroundCol.Text);

            // Redirect back to the portal home page
            Response.Redirect((string) ViewState["UrlReferrer"]);
        }

        /// <summary>
        /// The CancelBtn_Click event handler on this Page is used to cancel
        /// out of the page, and return the user back to the portal home page.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnCancel(EventArgs e)
        {
            base.OnCancel(e);
            // Redirect back to the portal home page
            Response.Redirect((string) ViewState["UrlReferrer"]);
        }

        /// <summary>
        /// Handles OnInit event
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.           
            Load += new EventHandler(Page_Load);
            base.OnInit(e);
        }

        #region Web Form Designer generated code

        #endregion
    }
}