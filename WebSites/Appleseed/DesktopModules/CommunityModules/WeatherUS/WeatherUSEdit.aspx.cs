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
    /// WeatherUSEdit Module - Edit page part
    /// Writen by: Jason Schaitel, Jason_Schaitel@hotmail.com
    /// Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
    /// </summary>
    public partial class WeatherUSEdit : EditItemPage
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
                if (ModuleID > 0)
                {
                    this.Zip.Text = this.ModuleSettings["Zip"] != null ? this.ModuleSettings["Zip"].ToString() : "10001";

                    if (this.ModuleSettings["Option"] != null)
                        Option.SelectedIndex = int.Parse(this.ModuleSettings["Option"].ToString());
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
                al.Add("2502DB18-B580-4F90-8CB4-C15E6E531006");
                return al;
            }
        }

        /// <summary>
        /// The UpdateBtn_Click event handler on this Page is used to either
        /// create or update an task. It uses the Appleseed.TasksDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);

            //only Update if the entered data is Valid
            if (Page.IsValid == true)
            {
                // Update settings in the database
                Framework.Site.Configuration.ModuleSettings.UpdateModuleSetting(ModuleID, "Zip", Zip.Text);
                Framework.Site.Configuration.ModuleSettings.UpdateModuleSetting(ModuleID, "Option", Option.SelectedIndex.ToString());

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