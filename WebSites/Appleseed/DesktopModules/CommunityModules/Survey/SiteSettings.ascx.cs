using System;
using System.Collections;
using System.Web.UI.WebControls;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI.WebControls;
using Localize = Appleseed.Framework.Web.UI.WebControls.Localize;
using Label = Appleseed.Framework.Web.UI.WebControls.Label;
using Appleseed.Framework.Site.Data;

namespace Appleseed.Content.Web.Modules
{
    using Appleseed.Framework;

    public class SiteSettings : PortalModuleControl 
    {
        protected SettingsTable EditTable;
        protected TextBox siteName;
        protected Localize site_title;
        protected Localize site_path;
        protected Label sitePath;

        /// <summary>
        /// Admin Module
        /// </summary>
        public override bool AdminModule
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The Page_Load server event handler on this user control is used
        /// to populate the current site settings from the config system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, EventArgs e) 
        {
            // If this is the first visit to the page, populate the site data
            if (Page.IsPostBack == false) 
            {
                //We flush cache for enable correct localization of items
                PortalSettings.FlushBaseSettingsCache(this.PortalSettings.PortalPath);

                siteName.Text = this.PortalSettings.PortalName;
                sitePath.Text = this.PortalSettings.PortalPath;
            }
            EditTable.DataSource = new SortedList(this.PortalSettings.CustomSettings);
            EditTable.DataBind();
        }
                
        /// <summary>
        /// Is used to update the Site Settings within the Portal Config System
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUpdate(EventArgs e) 
        {
            // Flush the cache for recovery the changes. jviladiu@portalServices.net (30/07/2004)
            PortalSettings.FlushBaseSettingsCache(this.PortalSettings.PortalPath);
            //Call base
            base.OnUpdate(e);

            // Only Update if Input Data is Valid
            if (Page.IsValid == true) 
            {
                //Update main settings and Tab info in the database
                new PortalsDB().UpdatePortalInfo(this.PortalSettings.PortalID, siteName.Text, sitePath.Text, false);

                // Update custom settings in the database
                EditTable.UpdateControls();

                // Redirect to this site to refresh
                Response.Redirect(Request.RawUrl);
            }
        }

        private void EditTable_UpdateControl(object sender, SettingsTableEventArgs e)
        {
            PortalSettings.UpdatePortalSetting(this.PortalSettings.PortalID, ((SettingItem<string, TextBox>)e.CurrentItem).EditControl.ID, ((SettingItem<string, TextBox>)e.CurrentItem).Value);        
        }

        public override Guid GuidID 
        {
            get
            {
                return new Guid("{EBBB01B1-FBB5-4E79-8FC4-59BCA1D0554E}");
            }
        }

        #region Web Form Designer generated code
        /// <summary>
        /// Raises OnInit Event
        /// </summary>
        /// <param name="e"></param>
        override protected void OnInit(EventArgs e)
        {
            if ( !this.Page.IsCssFileRegistered("TabControl") )
                this.Page.RegisterCssFile("TabControl");

            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent() 
        {
            this.EditTable.UpdateControl += new UpdateControlEventHandler(this.EditTable_UpdateControl);
            this.Load += new EventHandler(this.Page_Load);

        }
        #endregion
    }
}