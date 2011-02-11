using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Data;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Users.Data;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;
using Path = Appleseed.Framework.Settings.Path;

namespace Appleseed.Content.Web.Modules
{
	/// <summary>
	/// SimpleMenu Module
	/// created 30/04/2003 by Mario Hartmann 
	/// mario@hartmann.net // http://mario.hartmann.net
	/// </summary>
	public partial class SimpleMenu : PortalModuleControl 
	{

		/// <summary>
		/// Public constructor. Sets base settings for module.
		/// </summary>
		public SimpleMenu() 
		{
			SettingItem setParentPageID = new SettingItem(new IntegerDataType());
			setParentPageID.Required = true;
			setParentPageID.Value = "0";
			setParentPageID.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
			setParentPageID.EnglishName = "ParentTabId";
			setParentPageID.Description = "Sets the Id of then Parent tab for the menu (this tab may be hidden or inaccessible for the logged on user.)";
			setParentPageID.Order = 1;
			this._baseSettings.Add("sm_ParentPageID", setParentPageID);

			//localized by Pekka Ylenius
			ArrayList SetRepeatDirectionArrayList = new ArrayList();
			SetRepeatDirectionArrayList.Add( new SettingOption(0, 
			                                                  General.GetString("HORIZONTAL", "Horizontal"))); 
			SetRepeatDirectionArrayList.Add( new SettingOption(1, 
			                                                  General.GetString("VERTICAL", "Vertical"))); 

			SettingItem setMenuRepeatDirection = new SettingItem(new CustomListDataType(SetRepeatDirectionArrayList, "Name", "Val")); 

			setMenuRepeatDirection.Required = true; 
			setMenuRepeatDirection.Order = 2; 
			setMenuRepeatDirection.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS; 
			setMenuRepeatDirection.Description = "Sets the repeat direction for menu rendering."; 
			setMenuRepeatDirection.EnglishName = "Menu RepeatDirection";
			this._baseSettings.Add("sm_Menu_RepeatDirection", setMenuRepeatDirection); 

			// MenuLayouts
			Hashtable menuTypes = new Hashtable();
			foreach(string menuTypeControl in Directory.GetFiles(HttpContext.Current.Server.MapPath(Path.WebPathCombine(Path.ApplicationRoot, "/DesktopModules/CommunityModules/SimpleMenu/SimpleMenuTypes/")), "*.ascx"))
			{
				string menuTypeControlDisplayName = menuTypeControl.Substring(menuTypeControl.LastIndexOf("\\") + 1, menuTypeControl.LastIndexOf(".") - menuTypeControl.LastIndexOf("\\") - 1);
				string menuTypeControlName = menuTypeControl.Substring(menuTypeControl.LastIndexOf("\\") + 1);
				menuTypes.Add(menuTypeControlDisplayName, menuTypeControlName);
			}

			// Thumbnail Layout Setting
			SettingItem menuTypeSetting = new SettingItem(new CustomListDataType(menuTypes, "Key", "Value"));
			menuTypeSetting.Required = true;
			menuTypeSetting.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
			menuTypeSetting.Value = "StaticItemMenu.ascx";
			menuTypeSetting.Description = "Sets the type of menu this module use.";
			menuTypeSetting.EnglishName = "MenuType";
			menuTypeSetting.Order = 3;
			this._baseSettings.Add("sm_MenuType", menuTypeSetting);

			
			ArrayList SetBindingArrayList  = new ArrayList();
			SetBindingArrayList.Add(new SettingOption((int)BindOption.BindOptionNone,General.GetString("BIND_OPTION_NONE","BindOptionNone")));
			SetBindingArrayList.Add(new SettingOption((int)BindOption.BindOptionTop,General.GetString("BIND_OPTION_TOP","BindOptionTop")));
			SetBindingArrayList.Add(new SettingOption((int)BindOption.BindOptionChildren,General.GetString("BIND_OPTION_CHILDREN","BindOptionChildren")));
			SetBindingArrayList.Add(new SettingOption((int)BindOption.BindOptionCurrentChilds,General.GetString("BIND_OPTION_CURRENT_CHILDS","BindOptionCurrentChilds")));
			SetBindingArrayList.Add(new SettingOption((int)BindOption.BindOptionDefinedParent,General.GetString("BIND_OPTION_DEFINED_PARENT","BindOptionDefinedParent")));
			SetBindingArrayList.Add(new SettingOption((int)BindOption.BindOptionSiblings,General.GetString("BIND_OPTION_SIBLINGS","BindOptionSiblings")));
			SetBindingArrayList.Add(new SettingOption((int)BindOption.BindOptionSubtabSibling,General.GetString("BIND_OPTION_SUBTAB_SIBLING","BindOptionSubtabSibling")));
			SetBindingArrayList.Add(new SettingOption((int)BindOption.BindOptionTabSibling,General.GetString("BIND_OPTION_TABSIBLING","BindOptionTabSibling")));

			SettingItem setMenuBindingType = new SettingItem(new CustomListDataType(SetBindingArrayList, "Name", "Val"));
			setMenuBindingType.Required = true;
			setMenuBindingType.Order = 4;
			setMenuBindingType.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
			setMenuBindingType.EnglishName = "MenuBindingType";
			this._baseSettings.Add("sm_MenuBindingType", setMenuBindingType);

			//			SettingItem setHeaderText = new SettingItem(new StringDataType());
			//			setHeaderText.Required = false;
			//			setHeaderText.Value = string.Empty;
			//			setHeaderText.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
			//			setHeaderText.Description ="Sets a header text of the static menu (the special setting <CurrentTab> displays the current TabName).";
			//			setHeaderText.Order = 5;
			//			this._baseSettings.Add("sm_Menu_HeaderText", setHeaderText);
			//		
			//			SettingItem setFooterText = new SettingItem(new StringDataType());
			//			setFooterText.Required = false;
			//			setFooterText.Value = string.Empty;
			//			setFooterText.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
			//			setFooterText.Description ="Sets a footer text of the static menu.";
			//
			//			setFooterText.Order = 6;
			//			this._baseSettings.Add("sm_Menu_FooterText", setFooterText);
		
		}


		/// <summary>
		/// The Page_Load event handler on this User Control
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e)
		{
			string menuType = "SimpleMenu";
			if (Settings["sm_MenuType"] != null)
				menuType = (Settings["sm_MenuType"].ToString());

			try
			{
                SimpleMenuType theMenu = (SimpleMenuType)this.LoadControl(Path.ApplicationRoot + "/DesktopModules/CommunityModules/SimpleMenu/SimpleMenuTypes/" + menuType);
				theMenu.GlobalPortalSettings = this.portalSettings;
				theMenu.ModuleSettings	 = this.Settings;
				theMenu.DataBind();
				PlaceHolder.Controls.Add(theMenu);   
			}
			catch (Exception)
			{
				Literal tmpError = new Literal ();
				tmpError.Text=General.GetString("ERROR_MENUETYPE_LOAD", "The MenuType '{1}' cannot be loaded.",this).Replace("{1}",menuType);
				PlaceHolder.Controls.Add (tmpError);
			}
		}


		#region General module Implementation
        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
		public override Guid GuidID 
		{
			get
			{
				return new Guid("{D3182CD6-DAFF-4E72-AD9E-0B28CB44F006}");
			}
		}


		#region  Search Implementation
        /// <summary>
        /// If the module is searchable you
        /// must override the property to return true
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

		# region Install / Uninstall Implementation
		//		public override void Install(System.Collections.IDictionary stateSaver)
		//		{
		//			string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "install.sql");
		//			ArrayList errors = Appleseed.Framework.Data.DBHelper.ExecuteScript(currentScriptName, true);
		//			if (errors.Count > 0)
		//			{
		//				// Call rollback
		//				throw new Exception("Error occurred:" + errors[0].ToString());
		//			}
		//		}
		//
		//		public override void Uninstall(System.Collections.IDictionary stateSaver)
		//		{
		//			string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "uninstall.sql");
		//			ArrayList errors = Appleseed.Framework.Data.DBHelper.ExecuteScript(currentScriptName, true);
		//			if (errors.Count > 0)
		//			{
		//				// Call rollback
		//				throw new Exception("Error occurred:" + errors[0].ToString());
		//			}
		//		}

		# endregion
		
		#endregion


		#region Web Form Designer generated code
        /// <summary>
        /// Raises OnInit event.
        /// </summary>
        /// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
            this.Load += new EventHandler(this.Page_Load);
			base.OnInit(e);
		}
		#endregion

	}
}
