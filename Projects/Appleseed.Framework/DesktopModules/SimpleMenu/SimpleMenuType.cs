using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

using Appleseed.Framework;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
	/// <summary>
	/// SimpleMenuType
	/// </summary>
	public class SimpleMenuType :UserControl
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SimpleMenuType"/> class.
        /// </summary>
		public SimpleMenuType()
		{
			//
			// TODO: Add constructor logic here
			//
		}


		private Hashtable _settings;
        /// <summary>
        /// Gets or sets the module settings.
        /// </summary>
        /// <value>The module settings.</value>
		public Hashtable ModuleSettings
		{
			get
			{
				return _settings;
			}
			set
			{
				_settings = value ;
			}
		}
		

		private PortalSettings _portalSettings;
        /// <summary>
        /// Gets or sets the global portal settings.
        /// </summary>
        /// <value>The global portal settings.</value>
		public PortalSettings GlobalPortalSettings
		{
			get
			{
				return _portalSettings;
			}
			set
			{
				_portalSettings = value ;
			}
		}
		

		private BindOption menuBindOption = BindOption.BindOptionNone;
        /// <summary>
        /// Gets the menu bind option.
        /// </summary>
        /// <value>The menu bind option.</value>
		public BindOption MenuBindOption
		{
		get
		{
			if (ModuleSettings["sm_MenuBindingType"] != null )
			menuBindOption = (BindOption) int.Parse("0" +  ModuleSettings["sm_MenuBindingType"].ToString());
			return menuBindOption;
		}
}



		private RepeatDirection menuRepeatDirection;
        /// <summary>
        /// Gets the menu repeat direction.
        /// </summary>
        /// <value>The menu repeat direction.</value>
		public RepeatDirection MenuRepeatDirection
		{
			get
			{
				if (ModuleSettings["sm_Menu_RepeatDirection"] != null && ModuleSettings["sm_Menu_RepeatDirection"].ToString() == "0" ) 
					menuRepeatDirection = RepeatDirection.Horizontal; 
				else
					menuRepeatDirection = RepeatDirection.Vertical;

				return menuRepeatDirection;

			}
		}
	


		private int parentTabID = 0;
        /// <summary>
        /// Gets the parent page ID.
        /// </summary>
        /// <value>The parent page ID.</value>
		public int ParentPageID
		{
			get
			{
				if (ModuleSettings["sm_ParentPageID"] != null)
					parentTabID = int.Parse(ModuleSettings["sm_ParentPageID"].ToString());
			
				return parentTabID;

			}
		}

	}
}
