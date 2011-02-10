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
    /// Default user control placed on top of each administrative page
    /// </summary>
    public partial  class DesktopPortalBanner : UserControl
    {
        /// <summary>
        /// Placeholder for current control
        /// </summary>
        protected PlaceHolder LayoutPlaceHolder;

		// jes1111 
		/// <summary>
		/// 
		/// </summary>
		public bool ShowTabs = true;

        #region Web Form Designer generated code
        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e"></param>
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();

            base.OnInit(e);
        }
        private void InitializeComponent() 
        {
			this.Load += new EventHandler(this.DesktopPortalBanner_Load);

		}
		#endregion

        private void DesktopPortalBanner_Load(object sender, EventArgs e)
        {
            
            string LayoutBasePage = "DesktopPortalBanner.ascx";
			
			// Obtain PortalSettings from Current Context
			PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
			
			// jes1111 
			portalSettings.ShowPages = ShowTabs;

			// [START] file path -- bja@reedtek.com
			//
			// Validate that the layout file is present. I have found
			// that sometimes they go away in different releases. So let's check
			//string filepath = portalSettings.PortalLayoutPath + LayoutBasePage;
			string filepath = Path.WebPathCombine(portalSettings.PortalLayoutPath, LayoutBasePage);

			// does it exsists
			if (File.Exists(Server.MapPath(filepath)))
				LayoutPlaceHolder.Controls.Add(Page.LoadControl(filepath));
			else 
			{
				// create an exception
				Exception ex = new Exception("Portal cannot find layout ('" + filepath + "')");
				// go log/handle it
				//ErrorHandler.HandleException(ex);
				Appleseed.Framework.ErrorHandler.Publish(Appleseed.Framework.LogLevel.Error, ex);
			}
			// [END] file path -- bja@reedtek.com
        }
    }
}
