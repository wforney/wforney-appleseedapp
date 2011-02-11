using System;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// DailyDilbert Module
    /// Based on VB Module Written by SnowCovered.com
    /// Modifications and conversion for C# IBS Portal (c)2002 by Christopher S Judd, CDP
    /// email- dotNet@HorizonsLLC.com    web- www.HorizonsLLC.com/IBS
    /// Modifications and conversion for Appleseed Jakob hansen
    /// </summary>
    public partial class DailyDilbert : PortalModuleControl
    {
        /// <summary>
        /// The Page_Load event handler on this User Control is used to
        /// call the web service function that get the Dilbert Image
        /// then sets the image for the page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Set the URl for the image
            imgDilbert.ImageUrl = TemplateSourceDirectory + "/DailyDilbertImage.aspx?mID=" + ModuleID.ToString();
            imgDilbert.NavigateUrl = TemplateSourceDirectory + "/DailyDilbertImage.aspx?mID=" + ModuleID.ToString();
        }

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531031}"); }
        }

        /// <summary>
        /// Public constructor. Sets base settings for module.
        /// </summary>
        public DailyDilbert()
        {
            SettingItem setImagePercent = new SettingItem(new IntegerDataType());
            setImagePercent.Required = true;
            setImagePercent.Value = "80";
            setImagePercent.Order = 1;
            setImagePercent.MinValue = 1;
            setImagePercent.MaxValue = 100;
            _baseSettings.Add("ImagePercent", setImagePercent);
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInit event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
//			ModuleTitle = new DesktopModuleTitle();
//			Controls.AddAt(0, ModuleTitle);
            base.OnInit(e);
        }

        #endregion
    }
}