using System;
using Appleseed.Framework.Security;
using Appleseed.Framework.Web.UI;

namespace Appleseed.Admin
{
    /// <summary>
    /// The Logoff page is responsible for signing out a user 
    /// from the cookie authentication, and then redirecting 
    /// the user back to the portal home page.
    /// This page is executed when the user	clicks 
    /// the Logoff button at the top of the page.
    /// </summary>
    public partial class Logoff : Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Signout
            PortalSecurity.SignOut();
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion
    }
}