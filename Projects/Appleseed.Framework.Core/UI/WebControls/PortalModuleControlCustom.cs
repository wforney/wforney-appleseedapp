using System.Collections;
using System.Web;
using Appleseed.Framework.Settings.Cache;
using Appleseed.Framework.Site.Configuration;
using System;

namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// A PortalModuleControl that supports CustomUserSettings for authenticated users. (Users can specify
    /// settings for the module instance that will apply only to them when they interact with the module).
    /// </summary>
    public class PortalModuleControlCustom : PortalModuleControl
    {
        // provide a custom Hashtable that will store user-specific settings for this instance
        // of the module.  
        protected Hashtable _customUserSettings;

        /// <summary>
        /// Gets a value indicating whether this instance has customizeable settings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has customizeable settings; otherwise, <c>false</c>.
        /// </value>
        public bool HasCustomizeableSettings
        {
            get { return CustomizedUserSettings.Count > 0; }
        }


        /// <summary>
        /// Gets the customized user settings.
        /// </summary>
        /// <value>The customized user settings.</value>
        public Hashtable CustomizedUserSettings
        {
            get
            {
                if (_customUserSettings != null)
                {
                    return _customUserSettings;
                }
                else
                {
                    Hashtable tempSettings = new Hashtable();

                    SettingItem _default;

                    //refresh this module's settings on every call in case they logged off, so it will
                    //retrieve the 'default' settings from the database.
                    //Invalidate cache
                    CurrentCache.Remove(Key.ModuleSettings(ModuleID));
                    //this._baseSettings = ModuleSettings.GetModuleSettings(this.ModuleID, this._baseSettings);

                    foreach (string str in Settings.Keys)
                    {
                        _default = (SettingItem) Settings[str];
                        if (_default.Group == SettingItemGroup.CUSTOM_USER_SETTINGS) //It's one we want to customize
                        {
                            tempSettings.Add(str, _default); //insert the 'default' value
                        }
                    }

                    //Now, replace the default settings with the custom settings for this user from the database.
                    return
                        ModuleSettingsCustom.GetModuleUserSettings(ModuleConfiguration.ModuleID,
                                                                   (Guid)PortalSettings.CurrentUser.Identity.ProviderUserKey,
                                                                   tempSettings);
                }
            }
        }

        #region "Customize" Button Implementation

        private ModuleButton customizeButton;

        /// <summary>
        /// Module Properties button
        /// </summary>
        /// <value>The customize button.</value>
        public ModuleButton CustomizeButton
        {
            get
            {
                if (customizeButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (HasCustomizeableSettings && PortalSettings.CurrentUser.Identity.IsOnline)
                    {
                        // create the button
                        customizeButton = new ModuleButton();
                        customizeButton.Group = ModuleButton.ButtonGroup.Admin;
                        customizeButton.EnglishName = "Customize";
                        customizeButton.TranslationKey = "CUSTOMIZE";
                        customizeButton.Image = CurrentTheme.GetImage("Buttons_Properties", "Properties.gif");
                        if (PropertiesUrl.IndexOf("?") >= 0)
                            //Do not change if  the querystring is present (shortcut patch)
                            //if ( this.ModuleID != this.OriginalModuleID ) // shortcut
                            customizeButton.HRef = PropertiesUrl;
                        else
                            customizeButton.HRef =
                                HttpUrlBuilder.BuildUrl("~/DesktopModules/CoreModules/Admin/CustomPropertyPage.aspx", PageID,
                                                        "mID=" + ModuleID.ToString());
                        customizeButton.Target = PropertiesTarget;
                        customizeButton.RenderAs = ButtonsRenderAs;
                    }
                }
                return customizeButton;
            }
        }

        /// <summary>
        /// Builds the three public button lists
        /// </summary>
        protected override void BuildButtonLists()
        {
            if (CustomizeButton != null)
                ButtonListAdmin.Add(CustomizeButton);

            base.BuildButtonLists();
        }

        #endregion
    }
}