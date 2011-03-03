using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Core.Model;

namespace Appleseed.Framework.Web.UI.WebControls
{
    public class DesktopPanes : DesktopPanesBase
    {

        /// <summary>
        /// This method determines the tab index of the currently
        /// requested portal view, and then dynamically populate the left,
        /// center and right hand sections of the portal tab.
        /// </summary>
        protected override void InitializeDataSource()
        {
            DataSource = ModelServices.GetCurrentPageModules();  
        }
    }
}