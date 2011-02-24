// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DailyDilbert.ascx.cs" company="--">
//   Copyright © -- 2011. All Rights Reserved.
// </copyright>
// <summary>
//   DailyDilbert Module
//   Based on VB Module Written by SnowCovered.com
//   Modifications and conversion for C# IBS Portal (c)2002 by Christopher S Judd, CDP
//   email- dotNet@HorizonsLLC.com    web- www.HorizonsLLC.com/IBS
//   Modifications and conversion for Appleseed Jakob hansen
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.DesktopModules.CommunityModules.DailyDilbert
{
    using System;
    using System.Web.UI.WebControls;

    using Appleseed.Framework;
    using Appleseed.Framework.Web.UI.WebControls;

    /// <summary>
    /// DailyDilbert Module
    ///   Based on VB Module Written by SnowCovered.com
    ///   Modifications and conversion for C# IBS Portal (c)2002 by Christopher S Judd, CDP
    ///   email- dotNet@HorizonsLLC.com    web- www.HorizonsLLC.com/IBS
    ///   Modifications and conversion for Appleseed Jakob hansen
    /// </summary>
    public partial class DailyDilbertModule : PortalModuleControl
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DailyDilbertModule"/> class. 
        ///   Initializes a new instance of the <see cref="PortalModuleControl"/> class.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public DailyDilbertModule()
        {
            var setImagePercent = new SettingItem<int, TextBox> { Required = true, Value = 80, Order = 1, MinValue = 1, MaxValue = 100 };
            this.BaseSettings.Add("ImagePercent", setImagePercent);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get
            {
                return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531031}");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        /// <remarks>
        /// </remarks>
        protected override void OnInit(EventArgs e)
        {
            // Set the URl for the image
            this.imgDilbert.ImageUrl = string.Format(
                "{0}/DailyDilbertImage.aspx?mID={1}", this.TemplateSourceDirectory, this.ModuleID);
            this.imgDilbert.NavigateUrl = string.Format(
                "{0}/DailyDilbertImage.aspx?mID={1}", this.TemplateSourceDirectory, this.ModuleID);

            // ModuleTitle = new DesktopModuleTitle();
            // Controls.AddAt(0, ModuleTitle);
            base.OnInit(e);
        }

        #endregion
    }
}