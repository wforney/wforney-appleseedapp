using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// WeatherDEEdit Module - Editpage part
    /// adapted from original version by: Mario Hartmann, Mario@Hartmann.net
    ///
    /// Original WeatherUSEdit Module
    /// Writen by: Jason Schaitel, Jason_Schaitel@hotmail.com
    /// Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
    /// </summary>
    public partial class WeatherDEEdit : EditItemPage
    {
        protected TextBox Textbox2;


        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Construct the page
            // Added css Styles by Mario Endara <mario@softworks.com.uy> (2004/10/26)
            this.UpdateButton.CssClass = "CommandButton";
            PlaceHolderButtons.Controls.Add(this.UpdateButton);
            PlaceHolderButtons.Controls.Add(new LiteralControl("&#160;"));
            this.CancelButton.CssClass = "CommandButton";
            PlaceHolderButtons.Controls.Add(this.CancelButton);

            if (Page.IsPostBack == false)
            {
                WeatherZip.Text = "88045";
                WeatherCityIndex.Text = "0";
                WeatherSetting.SelectedIndex = 0;

                if (ModuleID > 0)
                {
                    if (this.ModuleSettings["WeatherZip"] != null)
                        WeatherZip.Text = this.ModuleSettings["WeatherZip"].ToString();

                    if (this.ModuleSettings["WeatherCityIndex"] != null)
                        WeatherCityIndex.Text = this.ModuleSettings["WeatherCityIndex"].ToString();

                    if (this.ModuleSettings["WeatherSetting"] != null)
                        WeatherSetting.SelectedIndex =
                            int.Parse(this.ModuleSettings["WeatherSetting"].ToString());

                    if (this.ModuleSettings["WeatherDesign"] != null)
                    {
                        for (int i = 0; i < WeatherDesign.Items.Count; i++)
                        {
                            if (WeatherDesign.Items[i].Value == this.ModuleSettings["WeatherDesign"].ToString())
                            {
                                WeatherDesign.SelectedIndex = i;
                            }
                        }
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
                al.Add("D3182CD6-DAFF-4E72-AD9E-0B28CB44F000");
                return al;
            }
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);

            //only Update if the entered data is Valid
            if (Page.IsValid == true)
            {
                // Update settings in the database
                Framework.Site.Configuration.ModuleSettings.UpdateModuleSetting(ModuleID, "WeatherZip", WeatherZip.Text);
                Framework.Site.Configuration.ModuleSettings.UpdateModuleSetting(ModuleID, "WeatherCityIndex", WeatherCityIndex.Text);
                Framework.Site.Configuration.ModuleSettings.UpdateModuleSetting(ModuleID, "WeatherSetting",
                                                   WeatherSetting.Items[WeatherSetting.SelectedIndex].Value);
                Framework.Site.Configuration.ModuleSettings.UpdateModuleSetting(ModuleID, "WeatherDesign",
                                                   WeatherDesign.Items[WeatherDesign.SelectedIndex].Value);
                RedirectBackToReferringPage();
            }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            //Controls must be created here
            this.UpdateButton = new LinkButton();
            this.CancelButton = new LinkButton();

            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion
    }
}