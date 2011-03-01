using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework.Settings;

namespace AppleseedWebApplication.Installer
{
    public partial class Update : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var s = new Appleseed.Framework.Core.Update.Services())
            {
                s.RunDBUpdate(Config.ConnectionString);
            }

            Response.Redirect("~/Default.aspx");
        }
    }
}