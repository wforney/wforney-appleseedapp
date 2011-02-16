// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleMenu.ascx.cs" company="--">
//   Copyright © -- 2011. All Rights Reserved.
// </copyright>
// <summary>
//   SimpleMenu Module
//   created 30/04/2003 by Mario Hartmann
//   mario@hartmann.net // http://mario.hartmann.net
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Content.Web.Modules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.UI.WebControls;

    using Appleseed.Framework;
    using Appleseed.Framework.DataTypes;
    using Appleseed.Framework.Web.UI.WebControls;

    using Path = Appleseed.Framework.Settings.Path;

    /// <summary>
    /// SimpleMenu Module
    ///   created 30/04/2003 by Mario Hartmann 
    ///   mario@hartmann.net // http://mario.hartmann.net
    /// </summary>
    public partial class SimpleMenu : PortalModuleControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "SimpleMenu" /> class.
        /// </summary>
        public SimpleMenu()
        {
            var setParentPageId = new SettingItem<int, TextBox>()
                {
                    Required = true, 
                    Value = 0, 
                    Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS, 
                    EnglishName = "ParentTabId", 
                    Description =
                        "Sets the Id of then Parent tab for the menu (this tab may be hidden or inaccessible for the logged on user.)", 
                    Order = 1
                };
            this.BaseSettings.Add("sm_ParentPageID", setParentPageId);

            // localized by Pekka Ylenius
            var setRepeatDirectionArrayList = new List<SettingOption> {
                    new SettingOption(0, General.GetString("HORIZONTAL", "Horizontal")), 
                    new SettingOption(1, General.GetString("VERTICAL", "Vertical"))
                };

            var setMenuRepeatDirection =
                new SettingItem<string, ListControl>(new CustomListDataType(setRepeatDirectionArrayList, "Name", "Val"))
                    {
                        Required = true, 
                        Order = 2, 
                        Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS, 
                        Description = "Sets the repeat direction for menu rendering.", 
                        EnglishName = "Menu RepeatDirection"
                    };

            this.BaseSettings.Add("sm_Menu_RepeatDirection", setMenuRepeatDirection);

            // MenuLayouts
            var menuTypes = new Hashtable();
            foreach (var menuTypeControl in
                Directory.GetFiles(
                    HttpContext.Current.Server.MapPath(
                        Path.WebPathCombine(
                            Path.ApplicationRoot, "/DesktopModules/CommunityModules/SimpleMenu/SimpleMenuTypes/")), 
                    "*.ascx"))
            {
                var menuTypeControlDisplayName = menuTypeControl.Substring(
                    menuTypeControl.LastIndexOf("\\") + 1, 
                    menuTypeControl.LastIndexOf(".") - menuTypeControl.LastIndexOf("\\") - 1);
                var menuTypeControlName = menuTypeControl.Substring(menuTypeControl.LastIndexOf("\\") + 1);
                menuTypes.Add(menuTypeControlDisplayName, menuTypeControlName);
            }

            // Thumbnail Layout Setting
            var menuTypeSetting = new SettingItem<string, ListControl>(
                new CustomListDataType(menuTypes, "Key", "Value"))
                {
                    Required = true, 
                    Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS, 
                    Value = "StaticItemMenu.ascx", 
                    Description = "Sets the type of menu this module use.", 
                    EnglishName = "MenuType", 
                    Order = 3
                };
            this.BaseSettings.Add("sm_MenuType", menuTypeSetting);

            var setBindingArrayList = new List<SettingOption> {
                    new SettingOption(
                        (int)BindOption.BindOptionNone, General.GetString("BIND_OPTION_NONE", "BindOptionNone")), 
                    new SettingOption(
                        (int)BindOption.BindOptionTop, General.GetString("BIND_OPTION_TOP", "BindOptionTop")), 
                    new SettingOption(
                        (int)BindOption.BindOptionChildren, 
                        General.GetString("BIND_OPTION_CHILDREN", "BindOptionChildren")), 
                    new SettingOption(
                        (int)BindOption.BindOptionCurrentChilds, 
                        General.GetString("BIND_OPTION_CURRENT_CHILDS", "BindOptionCurrentChilds")), 
                    new SettingOption(
                        (int)BindOption.BindOptionDefinedParent, 
                        General.GetString("BIND_OPTION_DEFINED_PARENT", "BindOptionDefinedParent")), 
                    new SettingOption(
                        (int)BindOption.BindOptionSiblings, 
                        General.GetString("BIND_OPTION_SIBLINGS", "BindOptionSiblings")), 
                    new SettingOption(
                        (int)BindOption.BindOptionSubtabSibling, 
                        General.GetString("BIND_OPTION_SUBTAB_SIBLING", "BindOptionSubtabSibling")), 
                    new SettingOption(
                        (int)BindOption.BindOptionTabSibling, 
                        General.GetString("BIND_OPTION_TABSIBLING", "BindOptionTabSibling"))
                };

            var setMenuBindingType =
                new SettingItem<string, ListControl>(new CustomListDataType(setBindingArrayList, "Name", "Val"))
                    {
                        Required = true, 
                        Order = 4, 
                        Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS, 
                        EnglishName = "MenuBindingType"
                    };
            this.BaseSettings.Add("sm_MenuBindingType", setMenuBindingType);

            // 			SettingItem setHeaderText = new SettingItem<string, TextBox>();
            // 			setHeaderText.Required = false;
            // 			setHeaderText.Value = string.Empty;
            // 			setHeaderText.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            // 			setHeaderText.Description ="Sets a header text of the static menu (the special setting <CurrentTab> displays the current TabName).";
            // 			setHeaderText.Order = 5;
            // 			this._baseSettings.Add("sm_Menu_HeaderText", setHeaderText);
            // 			SettingItem setFooterText = new SettingItem<string, TextBox>();
            // 			setFooterText.Required = false;
            // 			setFooterText.Value = string.Empty;
            // 			setFooterText.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            // 			setFooterText.Description ="Sets a footer text of the static menu.";
            // 			setFooterText.Order = 6;
            // 			this._baseSettings.Add("sm_Menu_FooterText", setFooterText);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get
            {
                return new Guid("{D3182CD6-DAFF-4E72-AD9E-0B28CB44F006}");
            }
        }

        /// <summary>
        ///   If the module is searchable you
        ///   must override the property to return true
        /// </summary>
        /// <value></value>
        public override bool Searchable
        {
            get
            {
                return false;
            }
        }

        #endregion

        // 		public override void Install(System.Collections.IDictionary stateSaver)
        // 		{
        // 			string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "install.sql");
        // 			ArrayList errors = Appleseed.Framework.Data.DBHelper.ExecuteScript(currentScriptName, true);
        // 			if (errors.Count > 0)
        // 			{
        // 				// Call rollback
        // 				throw new Exception("Error occurred:" + errors[0].ToString());
        // 			}
        // 		}
        // 		public override void Uninstall(System.Collections.IDictionary stateSaver)
        // 		{
        // 			string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "uninstall.sql");
        // 			ArrayList errors = Appleseed.Framework.Data.DBHelper.ExecuteScript(currentScriptName, true);
        // 			if (errors.Count > 0)
        // 			{
        // 				// Call rollback
        // 				throw new Exception("Error occurred:" + errors[0].ToString());
        // 			}
        // 		}
        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        /// <remarks></remarks>
        protected override void OnInit(EventArgs e)
        {
            var menuType = "SimpleMenu";
            if (this.Settings["sm_MenuType"] != null)
            {
                menuType = this.Settings["sm_MenuType"].ToString();
            }

            try
            {
                var theMenu =
                    (SimpleMenuType)
                    this.LoadControl(
                        string.Format("{0}/DesktopModules/CommunityModules/SimpleMenu/SimpleMenuTypes/{1}", Path.ApplicationRoot, menuType));
                theMenu.GlobalPortalSettings = this.PortalSettings;
                theMenu.ModuleSettings = this.Settings;
                theMenu.DataBind();
                this.PlaceHolder.Controls.Add(theMenu);
            }
            catch (Exception)
            {
                var tmpError = new Literal
                {
                    Text =
                        General.GetString("ERROR_MENUETYPE_LOAD", "The MenuType '{1}' cannot be loaded.", this).
                        Replace("{1}", menuType)
                };
                this.PlaceHolder.Controls.Add(tmpError);
            } 
            base.OnInit(e);
        }
        
        #endregion
    }
}