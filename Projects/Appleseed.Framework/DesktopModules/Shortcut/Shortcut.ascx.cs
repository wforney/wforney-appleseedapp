using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Settings.Cache;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// Shortcut control provide a quick way to duplicate
    /// a module content in different page of the portal
    /// </summary>
    [History("jminond", "march 2005", "Changes for moving Tab to Page")]
    [History("Mario Hartmann", "mario@hartmann.net", "1.1", "2003/10/08", "moved to seperate folder")]
    public class Shortcut : PortalModuleControl
    {
        /// <summary>
        /// 
        /// </summary>
        protected PlaceHolder PlaceHolderModule;

// removed Jes1111 - 2004/09/29 - to support new rendering method
//		/// <summary>
//		/// No theme for this module 
//		/// </summary>
//		public override bool ApplyTheme
//        {
//            get
//            {
//                return(false);
//            }
//        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Shortcut"/> class.
        /// </summary>
        public Shortcut()
        {
            // Obtain PortalSettings from Current Context
            if (HttpContext.Current != null && HttpContext.Current.Items["PortalSettings"] != null)
            {
                //Do not remove these checks!! It fails installing modules on startup
                PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

                int p = portalSettings.PortalID;

                // Get a list of modules of the current running portal
                SettingItem LinkedModule =
                    new SettingItem(
                        new CustomListDataType(new ModulesDB().GetModulesSinglePortal(p), "ModuleTitle", "ModuleID"));
                LinkedModule.Required = true;
                LinkedModule.Order = 0;
                LinkedModule.Value = "0";
                _baseSettings.Add("LinkedModule", LinkedModule);
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            /* Remove the IsPostBack check to allow contained controls to interpret the event
			 * to resolve issue #860424
			 * Author: Cemil Ayvaz
			 * Email : cemil_ayvaz@yahoo.com
			 * Date  : 2004-06-01
			 * 
			if(!IsPostBack)
			{
			*/
            string ControlPath = string.Empty;

            //Try to get info on linked control
            int LinkedModuleID = Int32.Parse(Settings["LinkedModule"].ToString());
            SqlDataReader dr = ModuleSettings.GetModuleDefinitionByID(LinkedModuleID);
            try
            {
                if (dr.Read())
                    ControlPath = Path.ApplicationRoot + "/" + dr["DesktopSrc"].ToString();
            }
            finally
            {
                dr.Close();
            }

            //Load control
            PortalModuleControl portalModule;
            try
            {
                if (ControlPath == null || ControlPath.Length == 0)
                {
                    PlaceHolderModule.Controls.Add(
                        new LiteralControl("Module '" + LinkedModuleID +
                                           "' not found!  Use Admin panel to add a linked control."));
                    return;
                }
                portalModule = (PortalModuleControl) Page.LoadControl(ControlPath);

                //Sets portal ID
                portalModule.PortalID = PortalID;

                //Update settings
                ModuleSettings m = new ModuleSettings();
                m.ModuleID = LinkedModuleID;
                m.PageID = ModuleConfiguration.PageID;
                m.PaneName = ModuleConfiguration.PaneName;
                m.ModuleTitle = ModuleConfiguration.ModuleTitle;
                m.AuthorizedEditRoles = string.Empty; //Readonly
                m.AuthorizedViewRoles = string.Empty; //Readonly
                m.AuthorizedAddRoles = string.Empty; //Readonly
                m.AuthorizedDeleteRoles = string.Empty; //Readonly
                m.AuthorizedPropertiesRoles = ModuleConfiguration.AuthorizedPropertiesRoles;
                m.CacheTime = ModuleConfiguration.CacheTime;
                m.ModuleOrder = ModuleConfiguration.ModuleOrder;
                m.ShowMobile = ModuleConfiguration.ShowMobile;
                m.DesktopSrc = ControlPath;
                m.MobileSrc = string.Empty; //Not supported yet
                // added bja@reedtek.com
                m.SupportCollapsable = ModuleConfiguration.SupportCollapsable;

                portalModule.ModuleConfiguration = m;

                portalModule.Settings["MODULESETTINGS_APPLY_THEME"] = Settings["MODULESETTINGS_APPLY_THEME"];
                portalModule.Settings["MODULESETTINGS_THEME"] = Settings["MODULESETTINGS_THEME"];

                // added so ShowTitle is independent of the Linked Module
                portalModule.Settings["MODULESETTINGS_SHOW_TITLE"] = Settings["MODULESETTINGS_SHOW_TITLE"];
                // added so that shortcut works for module "print this..." feature
                PlaceHolderModule.ID = "Shortcut";

                // added so AllowCollapsable -- bja@reedtek.com
                portalModule.Settings["AllowCollapsable"] = Settings["AllowCollapsable"];

                //Add control to the page
                PlaceHolderModule.Controls.Add(portalModule);
            }
            catch (Exception ex)
            {
                ErrorHandler.Publish(LogLevel.Error, "Shortcut: Unable to load control '" + ControlPath + "'!", ex);
                PlaceHolderModule.Controls.Add(
                    new LiteralControl("<br><span class=NormalRed>" + "Unable to load control '" + ControlPath + "'!" +
                                       "<br>"));
                PlaceHolderModule.Controls.Add(new LiteralControl(ex.Message));
                return; //The controls has failed!
            }

            //Set title
            portalModule.PropertiesUrl =
                HttpUrlBuilder.BuildUrl("~/DesktopModules/CoreModules/Admin/PropertyPage.aspx", PageID, "mID=" + ModuleID.ToString());
            portalModule.PropertiesText = "PROPERTIES";
            portalModule.AddUrl = string.Empty; //Readonly
            portalModule.AddText = string.Empty; //Readonly
            portalModule.EditUrl = string.Empty; //Readonly
            portalModule.EditText = string.Empty; //Readonly
            // jes1111
            portalModule.OriginalModuleID = ModuleID;
            CurrentCache.Remove(Key.ModuleSettings(LinkedModuleID));
        }

        #region General Implementaion

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{F9F9C3A4-6E16-43b4-B540-984DDB5F1CD2}"); }
        }

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// On init
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