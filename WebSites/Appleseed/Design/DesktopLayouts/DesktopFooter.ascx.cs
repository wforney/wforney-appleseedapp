using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework.Site.Configuration;
using Path = Appleseed.Framework.Settings.Path;

namespace Appleseed.Content.Web.Modules
{
	/// <summary>
	///		Default user control placed at the bottom of each administrative page.
	/// </summary>
	public partial  class DesktopFooter : UserControl
	{
        /// <summary>
        /// Placeholder for current control
        /// </summary>
        protected PlaceHolder LayoutPlaceHolder;

		private void DesktopFooter_Load(object sender, EventArgs e)
		{
            string LayoutBasePage = "DesktopFooter.ascx";
			
            // Obtain PortalSettings from Current Context
			PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
			
			string footerPage = Path.WebPathCombine(portalSettings.PortalLayoutPath, LayoutBasePage);
			if(File.Exists(Server.MapPath(footerPage)))
				LayoutPlaceHolder.Controls.Add(Page.LoadControl(footerPage));
//			try
//			{
//				//LayoutPlaceHolder.Controls.Add(Page.LoadControl(portalSettings.PortalLayoutPath + LayoutBasePage));
//				LayoutPlaceHolder.Controls.Add(Page.LoadControl(portalSettings.PortalLayoutPath + LayoutBasePage));
//			}
//			catch
//			{
//				//No footer available
//			}
		}

		#region Web Form Designer generated code
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.Load += new EventHandler(this.DesktopFooter_Load);

        }
		#endregion
	}
}
