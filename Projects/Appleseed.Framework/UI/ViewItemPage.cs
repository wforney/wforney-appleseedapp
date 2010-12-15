using System;
using Appleseed.Framework.Security;

namespace Appleseed.Framework.Web.UI
{
    [
        History("jviladiu@portalServices.net", "2004/07/22",
            "Added Security Access. Now inherits from Appleseed.Framework.UI.SecurePage")]
    [History("jviladiu@portalServices.net", "2004/07/22", "Clean Methods that only call to base")]
    public class ViewItemPage : SecurePage
    {
        /// <summary>
        /// Handles OnInit event
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        /// <summary>
        /// Handles OnCancel event
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnCancel(EventArgs e)
        {
            base.OnCancel(e);
        }

        /// <summary>
        /// Handles OnUpdate event
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnUpdate(EventArgs e)
        {
            // Verify that the current user has access to add in this module
            if (PortalSecurity.HasPropertiesPermissions(ModuleID) == false)
                // Removed by Mario Endara <mario@softworks.com.uy> (2004/11/04)
                //				&& PortalSecurity.IsInRoles("Admins") == false)
                PortalSecurity.AccessDeniedEdit();
            base.OnUpdate(e);
        }

        /// <summary>
        /// Handles OnDelete
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnDelete(EventArgs e)
        {
            base.OnDelete(e);
        }

        /// <summary>
        /// Load settings
        /// </summary>
        protected override void LoadSettings()
        {
            // Verify that the current user has access to view this module
            if (PortalSecurity.HasViewPermissions(ModuleID) == false)
                // Removed by Mario Endara <mario@softworks.com.uy> (2004/11/04)
                //				&& PortalSecurity.IsInRoles("Admins") == false)
                PortalSecurity.AccessDenied();
            base.LoadSettings();
        }
    }
}