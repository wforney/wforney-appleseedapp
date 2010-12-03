using System.Web;
using System.Web.UI.WebControls;
using Appleseed.Framework.Site.Configuration;

namespace Appleseed.Framework.Web.UI.WebControls
{
    public class HeaderTitle : Label
    {
        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        override public void DataBind()
        {
            if(HttpContext.Current != null)
            {
                // Obtain PortalSettings from Current Context
            	PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

                // Dynamically Populate the Portal Site Name
                this.Text = portalSettings.PortalName;
            }

            base.DataBind();
        }
    }
}