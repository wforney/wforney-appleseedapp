// [START] Added for window mgmt. support (bja@reedtek.com)
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Appleseed.Framework.BLL.Utils;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Design;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Setup;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Site.Data;
using Path=Appleseed.Framework.Settings.Path;
using System.Web.Security;
using System.Web.Mvc;

namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// The PortalModuleControl class defines a custom 
    /// base class inherited by all
    /// desktop portal modules within the Portal.<br/>
    /// The PortalModuleControl class defines portal 
    /// specific properties that are used by the portal framework
    /// to correctly display portal modules.
    /// </summary>
    /// <remarks>This is the "all new" RC4 PortalModuleControl, which no longer has a separate DesktopModuleTitle.</remarks>
    [History( "john.mandia@whitelightsolutions.com", "2004/09/17", "Fixed path for help system" )]
    [History( "Jes1111", "2003/03/05", "Added ShowTitle setting - switches Title visibility on/off" )]
    [History( "Jes1111", "2003/04/24", "Added PortalAlias to cachekey" )]
    [History( "Jes1111", "2003/04/24", "Added Cacheable property" )]
    [History( "bja@reedtek.com", "2003/04/26", "Added support for win. mgmt min/max/close" )]
    [History( "david.verberckmoes@syntegra.com", "2003/06/02", "Showing LastModified date & user in a better way with themes" )]
    [History( "Jes1111", "2004/08/30", "All new version! No more DesktopModuleTitle." )]
    [History( "Mark, John and Jose", "2004/09/08", "Corrections in constructor for detect DesignMode" )]
    // history from old DesktopModuleTitle class
    [History( "Nicholas Smeaton", "2004/07/24", "Added support for arrow buttons to move modules" )]
    [History( "jviladiu@portalServices.net", "2004/07/13", "Corrections in workflow buttons" )]
    [History( "gman3001", "2004/04/08", "Added support for custom buttons in the title bar, and set all undefined title bar buttons to 'rb_mod_title_btn' css-class." )]
    [History( "Pekka Ylenius", "2004/11/28", "When '?' in ulr then '&' is needed not '?'" )]
    [History( "Hongwei Shen", "2005/09/8", "Fix the publishing problem and RevertToProduction button problem" )]
    [History( "Hongwei Shen", "2005/09/12", "Fix topic setting order problem(add module specific settings group base)" )]
    public class PortalModuleControl : ViewUserControl, ISearchable, IInstaller
    {
        #region Private field variables

        private ModuleSettings _moduleConfiguration;
        private int _canEdit = 0;
        private int _canAdd = 0;
        private int _canView = 0;
        private int _canDelete = 0;
        private int _canProperties = 0;
        private int _portalID = 0;
        private Hashtable _settings;
        private WorkFlowVersion _version = WorkFlowVersion.Production;
        private bool _supportsWorkflow = false;
        private bool _cacheable = true;
        private bool _supportsPrint = true;
        private bool _supportsBack = false;
        private bool _supportsEmail = false;
        //private bool			_supportsHelp = false;
        private bool _supportsArrows = true;
        private ViewControlManager _vcm = null;
        private PlaceHolder _header = new PlaceHolder();
        private PlaceHolder _footer = new PlaceHolder();
        private PlaceHolder _headerPlaceHolder = new PlaceHolder();

        //private PlaceHolder		_output = new PlaceHolder();

        // --  BJA Added Min/Max/Close Attributes [START]
        // Change wjanderson@reedtek.com
        // Date 25/4/2003 ( min/max./close buttons )
        // - Note : At some point you may wish to allow 
        // -        the selection of which buttons can
        // -        be displayed; right now it is all or nothing.
        // -        Also, you may wish to allow authorized close and min. 
        private int _canMin = 0;
        private int _canClose = 0;
        private bool _supportsCollapseable = false;

        /// <summary>
        /// 
        /// </summary>
        public Theme CurrentTheme;

        #endregion

        #region Standard Controls

        /// <summary>
        /// Standard content Delete button
        /// </summary>
        protected ImageButton DeleteBtn;

        /// <summary>
        /// Standard content Edit button
        /// </summary>
        protected ImageButton EditBtn;

        /// <summary>
        /// Standard content Update button
        /// </summary>
        protected LinkButton updateButton;

        #endregion

        #region Constructor

        /// <summary>
        /// Dafault contructor, initializes default settings
        /// </summary>
        public PortalModuleControl()
        {
            //MVC

            HttpContextWrapper wrapper = new HttpContextWrapper(Context);

            ViewContext viewContext = new ViewContext();
            viewContext.HttpContext = wrapper;
            viewContext.ViewData = new ViewDataDictionary();

            ViewContext = viewContext;

            //****************//

            int _groupOrderBase;
            SettingItemGroup _Group;

            // THEME MANAGEMENT
            _Group = SettingItemGroup.THEME_LAYOUT_SETTINGS;
            _groupOrderBase = ( int )SettingItemGroup.THEME_LAYOUT_SETTINGS;

            SettingItem ApplyTheme = new SettingItem( new BooleanDataType() );
            ApplyTheme.Order = _groupOrderBase + 10;
            ApplyTheme.Group = _Group;
            ApplyTheme.Value = "True";
            ApplyTheme.EnglishName = "Apply Theme";
            ApplyTheme.Description = "Check this box to apply theme to this module";
            _baseSettings.Add( "MODULESETTINGS_APPLY_THEME", ApplyTheme );

            ArrayList themeOptions = new ArrayList();
            themeOptions.Add(
                new SettingOption( ( int )ThemeList.Default, General.GetString( "MODULESETTINGS_THEME_DEFAULT" ) ) );
            themeOptions.Add( new SettingOption( ( int )ThemeList.Alt, General.GetString( "MODULESETTINGS_THEME_ALT" ) ) );
            SettingItem Theme = new SettingItem( new CustomListDataType( themeOptions, "Name", "Val" ) );
            Theme.Order = _groupOrderBase + 20;
            Theme.Group = _Group;
            Theme.Value = ( ( int )ThemeList.Default ).ToString();
            Theme.EnglishName = "Theme";
            Theme.Description = "Choose theme for this module";
            _baseSettings.Add( "MODULESETTINGS_THEME", Theme );

            if ( HttpContext.Current != null ) // null in DesignMode
            {
                // Added: Jes1111 - 2004-08-03
                PortalSettings _portalSettings;
                _portalSettings = ( PortalSettings )HttpContext.Current.Items[ "PortalSettings" ];
                // end addition: Jes1111

                if ( _portalSettings != null )
                {
                    //fix by The Bitland Prince
                    _portalID = _portalSettings.PortalID;

                    // added: Jes1111 2004-08-02 - custom module theme
                    if ( _portalSettings.CustomSettings.ContainsKey( "SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES" ) &&
                        _portalSettings.CustomSettings[ "SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES" ].ToString().Length != 0 &&
                        bool.Parse( _portalSettings.CustomSettings[ "SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES" ].ToString() )
                        )
                    {
                        ArrayList _tempList = new ArrayList( new ThemeManager( _portalSettings.PortalPath ).GetThemes() );
                        ArrayList _themeList = new ArrayList();
                        foreach ( ThemeItem _item in _tempList )
                        {
                            if ( _item.Name.ToLower().StartsWith( "module" ) )
                                _themeList.Add( _item );
                        }
                        ThemeItem _noCustomTheme = new ThemeItem();
                        _noCustomTheme.Name = string.Empty;
                        _themeList.Insert( 0, _noCustomTheme );
                        SettingItem ModuleTheme = new SettingItem( new CustomListDataType( _themeList, "Name", "Name" ) );
                        ModuleTheme.Order = _groupOrderBase + 25;
                        ModuleTheme.Group = _Group;
                        ModuleTheme.EnglishName = "Custom Theme";
                        ModuleTheme.Description = "Set a custom theme for this module only";
                        _baseSettings.Add( "MODULESETTINGS_MODULE_THEME", ModuleTheme );
                    }
                }
            }

            // switches title display on/off
            SettingItem ShowTitle = new SettingItem( new BooleanDataType() );
            ShowTitle.Order = _groupOrderBase + 30;
            ShowTitle.Group = _Group;
            ShowTitle.Value = "True";
            ShowTitle.EnglishName = "Show Title";
            ShowTitle.Description = "Switches title display on/off";
            _baseSettings.Add( "MODULESETTINGS_SHOW_TITLE", ShowTitle );

            // switches last modified summary on/off
            SettingItem ShowModifiedBy = new SettingItem( new BooleanDataType() );
            ShowModifiedBy.Order = _groupOrderBase + 40;
            ShowModifiedBy.Group = _Group;
            ShowModifiedBy.Value = "False";
            ShowModifiedBy.EnglishName = "Show Modified by";
            ShowModifiedBy.Description = "Switches 'Show Modified by' display on/off";
            _baseSettings.Add( "MODULESETTINGS_SHOW_MODIFIED_BY", ShowModifiedBy );

            // gman3001: added 10/26/2004
            //  - implement width, height, and content scrolling options for all modules 
            //  - implement auto-stretch option
            //Windows height
            SettingItem ControlHeight = new SettingItem( new IntegerDataType() );
            ControlHeight.Value = "0";
            ControlHeight.MinValue = 0;
            ControlHeight.MaxValue = 3000;
            ControlHeight.Required = true;
            ControlHeight.Order = _groupOrderBase + 50;
            ControlHeight.Group = _Group;
            ControlHeight.EnglishName = "Content Height";
            ControlHeight.Description = "Minimum height(in pixels) of the content area of this module. (0 for none)";
            _baseSettings.Add( "MODULESETTINGS_CONTENT_HEIGHT", ControlHeight );

            //Windows width
            SettingItem ControlWidth = new SettingItem( new IntegerDataType() );
            ControlWidth.Value = "0";
            ControlWidth.MinValue = 0;
            ControlWidth.MaxValue = 3000;
            ControlWidth.Required = true;
            ControlWidth.Order = _groupOrderBase + 60;
            ControlWidth.Group = _Group;
            ControlWidth.EnglishName = "Content Width";
            ControlWidth.Description = "Minimum width(in pixels) of the content area of this module. (0 for none)";
            _baseSettings.Add( "MODULESETTINGS_CONTENT_WIDTH", ControlWidth );

            //Content scrolling option
            SettingItem ScrollingSetting = new SettingItem( new BooleanDataType() );
            ScrollingSetting.Value = "false";
            ScrollingSetting.Order = _groupOrderBase + 70;
            ScrollingSetting.Group = _Group;
            ScrollingSetting.EnglishName = "Content Scrolling";
            ScrollingSetting.Description =
                "Set to enable/disable scrolling of Content based on height and width settings.";
            _baseSettings.Add( "MODULESETTINGS_CONTENT_SCROLLING", ScrollingSetting );

            //Module Stretching option
            SettingItem StretchSetting = new SettingItem( new BooleanDataType() );
            StretchSetting.Value = "true";
            StretchSetting.Order = _groupOrderBase + 80;
            StretchSetting.Group = _Group;
            StretchSetting.EnglishName = "Module Auto Stretch";
            StretchSetting.Description =
                "Set to enable/disable automatic stretching of the module's width to fill the empty area to the right of the module.";
            _baseSettings.Add( "MODULESETTINGS_WIDTH_STRETCHING", StretchSetting );
            // gman3001: END

            // BUTTONS
            _Group = SettingItemGroup.BUTTON_DISPLAY_SETTINGS;
            _groupOrderBase = ( int )SettingItemGroup.BUTTON_DISPLAY_SETTINGS;

            // Show print button in view mode?
            SettingItem PrintButton = new SettingItem( new BooleanDataType() );
            PrintButton.Value = "False";
            PrintButton.Order = _groupOrderBase + 20;
            PrintButton.Group = _Group;
            PrintButton.EnglishName = "Show Print Button";
            PrintButton.Description = "Show print button in view mode?";
            _baseSettings.Add( "MODULESETTINGS_SHOW_PRINT_BUTTION", PrintButton );

            // added: Jes1111 2004-08-29 - choice! Default is 'true' for backward compatibility
            // Show Title for print?
            SettingItem ShowTitlePrint = new SettingItem( new BooleanDataType() );
            ShowTitlePrint.Value = "True";
            ShowTitlePrint.Order = _groupOrderBase + 25;
            ShowTitlePrint.Group = _Group;
            ShowTitlePrint.EnglishName = "Show Title for Print";
            ShowTitlePrint.Description = "Show Title for this module in print popup?";
            _baseSettings.Add( "MODULESETTINGS_SHOW_TITLE_PRINT", ShowTitlePrint );

            // added: Jes1111 2004-08-02 - choices for Button display on module
            ArrayList buttonDisplayOptions = new ArrayList();
            buttonDisplayOptions.Add(
                new SettingOption( ( int )ModuleButton.RenderOptions.ImageOnly,
                                  General.GetString( "MODULESETTINGS_BUTTON_DISPLAY_IMAGE" ) ) );
            buttonDisplayOptions.Add(
                new SettingOption( ( int )ModuleButton.RenderOptions.TextOnly,
                                  General.GetString( "MODULESETTINGS_BUTTON_DISPLAY_TEXT" ) ) );
            buttonDisplayOptions.Add(
                new SettingOption( ( int )ModuleButton.RenderOptions.ImageAndTextCSS,
                                  General.GetString( "MODULESETTINGS_BUTTON_DISPLAY_BOTH" ) ) );
            buttonDisplayOptions.Add(
                new SettingOption( ( int )ModuleButton.RenderOptions.ImageOnlyCSS,
                                  General.GetString( "MODULESETTINGS_BUTTON_DISPLAY_IMAGECSS" ) ) );
            SettingItem ButtonDisplay = new SettingItem( new CustomListDataType( buttonDisplayOptions, "Name", "Val" ) );
            ButtonDisplay.Order = _groupOrderBase + 30;
            ButtonDisplay.Group = _Group;
            ButtonDisplay.Value = ( ( int )ModuleButton.RenderOptions.ImageOnly ).ToString();
            ButtonDisplay.EnglishName = "Display Buttons as:";
            ButtonDisplay.Description =
                "Choose how you want module buttons to be displayed. Note that settings other than 'Image only' may require Zen or special treatment in the Theme.";
            _baseSettings.Add( "MODULESETTINGS_BUTTON_DISPLAY", ButtonDisplay );

            // Jes1111 - not implemented yet			
            //			// Show email button in view mode?
            //			SettingItem EmailButton = new SettingItem(new BooleanDataType());
            //			EmailButton.Value = "False";
            //			EmailButton.Order = _groupOrderBase + 30;
            //			EmailButton.Group = _Group;
            //			this._baseSettings.Add("ShowEmailButton",EmailButton);

            // Show arrows buttons to move modules (admin only, property authorise)
            SettingItem ArrowButtons = new SettingItem( new BooleanDataType() );
            ArrowButtons.Value = "True";
            ArrowButtons.Order = _groupOrderBase + 40;
            ArrowButtons.Group = _Group;
            ArrowButtons.EnglishName = "Show Arrow Admin Buttons";
            ArrowButtons.Description = "Show Arrow Admin buttons?";
            _baseSettings.Add( "MODULESETTINGS_SHOW_ARROW_BUTTONS", ArrowButtons );

            // Show help button if exists
            SettingItem HelpButton = new SettingItem( new BooleanDataType() );
            HelpButton.Value = "True";
            HelpButton.Order = _groupOrderBase + 50;
            HelpButton.Group = _Group;
            HelpButton.EnglishName = "Show Help Button";
            HelpButton.Description = "Show help button in title if exists documentation for this module";
            _baseSettings.Add( "MODULESETTINGS_SHOW_HELP_BUTTON", HelpButton );

            // LANGUAGE/CULTURE MANAGEMENT
            _groupOrderBase = ( int )SettingItemGroup.CULTURE_SETTINGS;
            _Group = SettingItemGroup.CULTURE_SETTINGS;

            CultureInfo[] cultureList = Localization.LanguageSwitcher.GetLanguageList( true );

            SettingItem Culture = new SettingItem( new MultiSelectListDataType( cultureList, "DisplayName", "Name" ) );
            Culture.Value = string.Empty;
            Culture.Order = _groupOrderBase + 10;
            Culture.Group = _Group;
            Culture.EnglishName = "Culture";
            Culture.Description =
                "Please choose the culture. Invariant cultures shows always the module, if you choose one or more cultures only when culture is selected this module will shown.";
            _baseSettings.Add( "MODULESETTINGS_CULTURE", Culture );

            //Localized module title
            int counter = _groupOrderBase + 11;
            foreach ( CultureInfo c in cultureList )
            {
                //Ignore invariant
                if ( c != CultureInfo.InvariantCulture && !_baseSettings.ContainsKey( c.Name ) )
                {
                    SettingItem LocalizedTitle = new SettingItem( new StringDataType() );
                    LocalizedTitle.Order = counter;
                    LocalizedTitle.Group = _Group;
                    LocalizedTitle.EnglishName = "Title (" + c.Name + ")";
                    LocalizedTitle.Description = "Set title for " + c.EnglishName + " culture.";
                    _baseSettings.Add( "MODULESETTINGS_TITLE_" + c.Name, LocalizedTitle );
                    counter++;
                }
            }

            // SEARCH
            if ( Searchable )
            {
                _groupOrderBase = ( int )SettingItemGroup.MODULE_SPECIAL_SETTINGS;
                _Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;

                SettingItem topicName = new SettingItem( new StringDataType() );
                topicName.Required = false;
                topicName.Value = string.Empty;
                // modified by Hongwei Shen(hongwei.shen@gmail.com) 11/9/2005
                // goupbase and order is not specified
                topicName.Group = _Group;
                topicName.Order = _groupOrderBase; // always put in the first? 
                // end of modification
                topicName.EnglishName = "Topic";
                topicName.Description =
                    "Select a topic for this module. You may filter itmes by topic in Portal Search.";
                _baseSettings.Add( "TopicName", topicName );
            }

            //Default configuration
            _tabID = 0;

            _moduleConfiguration = new ModuleSettings();


            SettingItem Share = new SettingItem(new BooleanDataType());
            Share.Value = "False";
            Share.Order = _groupOrderBase + 51;
            Share.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            Share.EnglishName = "ShareModule";
            Share.Description = "Share Module";
            _baseSettings.Add("SHARE_MODULE", Share);
        }

        #endregion

        #region Module Configuration

        /// <summary>
        /// _baseSettings holds datatype information
        /// </summary>
        protected Hashtable _baseSettings = new Hashtable();

        /// <summary>
        /// Module custom settings
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public Hashtable Settings
        {
            get
            {
                if ( _settings == null )
                {
                    _settings = ModuleSettings.GetModuleSettings( ModuleID, _baseSettings );
                }
                return _settings;
            }
        }

        /// <summary>
        /// Module base settings defined by control creator
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public Hashtable BaseSettings
        {
            get { return _baseSettings; }
        }

        /// <summary>
        /// Override on derivates classes
        /// Method to initialize custom settings values (such as lists) 
        /// only when accessing the edition mode (and not in every class constructor)
        /// </summary>
        public virtual void InitializeCustomSettings()
        {
        }

        /// <summary>
        /// Override on derivates classes.
        /// Return the path of the add control if available.
        /// </summary>
        public virtual string AddModuleControl
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Override on derivates classes.
        /// Return the path of the edit control if available.
        /// </summary>
        public virtual string EditModuleControl
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// unique key for module caching
        /// </summary>
        public string ModuleCacheKey
        {
            get
            {
                if ( HttpContext.Current != null )
                {
                    // Change 8/April/2003 Jes1111
                    // changes to Language behaviour require addition of culture names to cache key
                    // Jes1111 2003/04/24 - Added PortalAlias to cachekey
                    PortalSettings portalSettings = ( PortalSettings )HttpContext.Current.Items[ "PortalSettings" ];
                    StringBuilder sb = new StringBuilder();
                    sb.Append( "rb_" );
                    sb.Append( portalSettings.PortalAlias );
                    sb.Append( "_mid" );
                    sb.Append( ModuleID.ToString() );
                    sb.Append( "[" );
                    sb.Append( portalSettings.PortalContentLanguage );
                    sb.Append( "+" );
                    sb.Append( portalSettings.PortalUILanguage );
                    sb.Append( "+" );
                    sb.Append( portalSettings.PortalDataFormattingCulture );
                    sb.Append( "]" );

                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The current ID of the module. Is unique for all portals.
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public int ModuleID
        {
            get
            {
                try
                {
                    return _moduleConfiguration.ModuleID;
                }
                catch
                {
                    return -1;
                }
            }
            set //made changeable by Manu, please be careful on changing it
            {
                _moduleConfiguration.ModuleID = value;
                _settings = null; //force cached settings to be reloaded
            }
        }

        // Jes1111
        private int _originalModuleID = -1;

        /// <summary>
        /// The ID of the orginal module (will be different to ModuleID when using shortcut module)
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public int OriginalModuleID
        {
            get
            {
                try
                {
                    if ( _originalModuleID == -1 )
                        return ModuleID;
                    else
                        return _originalModuleID;
                }
                catch
                {
                    return -1;
                }
            }
            set { _originalModuleID = value; }
        }

        /// <summary>
        /// Configuration
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public ModuleSettings ModuleConfiguration
        {
            get
            {
                if ( HttpContext.Current != null && _moduleConfiguration != null )
                    return _moduleConfiguration;
                else
                    return null;
            }
            set { _moduleConfiguration = value; }
        }

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public virtual Guid GuidID
        {
            get
            {
                //1.1.8.1324 - 24/01/2003
                throw new NotImplementedException( "You must implement a unique GUID for your module" );
            }
        }

        /// <summary>
        /// ClassName (Used for Get/Save: not implemented)
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public virtual string ClassName
        {
            get { return string.Empty; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles FlushCache event at Module level<br/>
        /// Performs FlushCache actions that are common to all Pages<br/>
        /// Can be overridden
        /// </summary>
        protected virtual void OnFlushCache()
        {
            if ( FlushCache != null )
                FlushCache( this, new EventArgs() ); //Invokes the delegates

            // remove module output from cache, if it's there
            if ( HttpContext.Current != null )
            {
                Context.Cache.Remove( ModuleCacheKey );
                Debug.WriteLine( "************* Remove " + ModuleCacheKey );
            }

            // any other code goes here
        }

        /// <summary>
        /// The Update event is defined using the event keyword.
        /// The type of Update is EventHandler.
        /// </summary>
        public event EventHandler Update;

        /// <summary>
        /// Handles OnUpdate event at Page level<br/>
        /// Performs OnUpdate actions that are common to all Pages<br/>
        /// Can be overridden
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUpdate( EventArgs e )
        {
            if ( Update != null )
                Update( this, e ); //Invokes the delegates

            //Flush cache
            OnFlushCache();

            // any other code goes here
            WorkFlowDB.SetLastModified( ModuleID, MailHelper.GetCurrentUserEmailAddress() );
        }

        /// <summary>
        /// On Delete
        /// </summary>
        protected virtual void OnDelete()
        {
            WorkFlowDB.SetLastModified( ModuleID, MailHelper.GetCurrentUserEmailAddress() );
        }

        // Change by Geert.Audenaert@Syntegra.Com
        // Date: 7/2/2003
        /// <summary>
        /// On Version Swap
        /// </summary>
        protected virtual void OnVersionSwap()
        {
        }

        // Change by Geert.Audenaert@Syntegra.Com
        // Date: 7/2/2003
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad( EventArgs e )
        {
            #region check workflow version

            // First Check if the version is specified
            string version = null;
            try
            {
                version = Page.Request.QueryString[ "wversion" + ModuleConfiguration.ModuleID.ToString() ];
            }
            catch ( NullReferenceException )
            {
                //string message = ex.Message;
            }

            if ( version != null )
            {
                WorkFlowVersion requestedVersion = version == "Staging"
                                                       ? WorkFlowVersion.Staging
                                                       : WorkFlowVersion.Production;
                if ( requestedVersion != Version )
                {
                    Version = requestedVersion;
                    OnVersionSwap();
                }
            }

            #endregion

            /* modified by Hongwei Shen (hongwei.shen@gmail.com) 8/9/2005
 * The publishing business is moved to the PublishButton click server event 
 * handler

			#region check if publish required

			// Now check if this module needs to published
			string publish = null;
			try
			{
				publish = Page.Request.QueryString["wpublish" + ModuleConfiguration.ModuleID.ToString()];
			}
			catch (NullReferenceException)
			{
				//string message = ex.Message;
			}

			if (publish == "doit")
			{
				// Check if the user has publish permissions on this
				if (! PortalSecurity.IsInRoles(ModuleConfiguration.AuthorizedPublishingRoles))
					PortalSecurity.AccessDeniedEdit();

				Publish();
			}

			#endregion
end of modification
*/

            #region set cacheable in ModuleConfiguration

            if ( ModuleConfiguration != null )
            {
                if ( Cacheable )
                    ModuleConfiguration.Cacheable = true;
                else
                    ModuleConfiguration.Cacheable = false;
            }

            #endregion

            SetupTheme();

            #region check for window management

            // bja@reedtek.com - does this configuration support window mgmt controls?
            // jes1111 - if (GlobalResources.SupportWindowMgmt && SupportCollapsable)
            if ( Config.WindowMgmtControls && SupportCollapsable )
                _vcm = new ViewControlManager( PageID, ModuleID, HttpContext.Current.Request.RawUrl );

            #endregion

            BuildButtonLists();

            MergeButtonLists();

            // Then call inherited member
            base.OnLoad( e );

            BuildControlHierarchy();
            _headerPlaceHolder.Controls.Add( _header );
            Controls.Add( _footer );
        }

        /// <summary>
        /// Raises OnInit event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit( EventArgs e )
        {
            Controls.AddAt( 0, _headerPlaceHolder );

            if ( DeleteBtn != null )
            {
                // Assign current permissions to Delete button
                if ( IsDeleteable == false )
                {
                    DeleteBtn.Visible = false;
                }
                else
                {
                    DeleteBtn.Visible = true;

                    if ( !( Page.ClientScript.IsClientScriptBlockRegistered( "confirmDelete" ) ) )
                    {
                        string[] s = { "CONFIRM_DELETE" };
                        Page.ClientScript.RegisterClientScriptBlock( GetType(), "confirmDelete",
                                                                    PortalSettings.GetStringResource(
                                                                        "CONFIRM_DELETE_SCRIPT",
                                                                        s ) );
                    }

                    if ( DeleteBtn.Attributes[ "onclick" ] != null )
                        DeleteBtn.Attributes[ "onclick" ] = "return confirmDelete();" + DeleteBtn.Attributes[ "onclick" ];
                    else
                        DeleteBtn.Attributes.Add( "onclick", "return confirmDelete();" );

                    DeleteBtn.Click += new ImageClickEventHandler( DeleteBtn_Click );
                    DeleteBtn.AlternateText = General.GetString( "DELETE" );
                    DeleteBtn.EnableViewState = false;
                }
            }

            if ( EditBtn != null )
            {
                // Assign current permissions to Edit button
                if ( IsEditable == false )
                {
                    EditBtn.Visible = false;
                }
                else
                {
                    EditBtn.Visible = true;
                    EditBtn.Click += new ImageClickEventHandler( EditBtn_Click );
                    EditBtn.AlternateText = General.GetString( "Edit" );
                    EditBtn.EnableViewState = false;
                }
            }

            if ( updateButton != null )
            {
                updateButton.Click += new EventHandler( UpdateBtn_Click );
                updateButton.Text = General.GetString( "UPDATE" );
                //updateButton.CssClass = "CommandButton"; // Jes1111 - set in .ascx
                updateButton.EnableViewState = false;
            }

            base.OnInit( e );
        }

        /// <summary>
        /// Update Button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateBtn_Click( Object sender, EventArgs e )
        {
            OnUpdate( e );
        }

        private void EditBtn_Click( Object sender, ImageClickEventArgs e )
        {
            OnEdit();
        }

        private void DeleteBtn_Click( Object sender, ImageClickEventArgs e )
        {
            OnDelete();
        }

        /// <summary>
        /// On Edit
        /// </summary>
        protected virtual void OnEdit()
        {
            WorkFlowDB.SetLastModified( ModuleID, MailHelper.GetCurrentUserEmailAddress() );
        }

        /// <summary>
        /// The FlushCache event is defined using the event keyword.
        /// The type of FlushCache is EventHandler.
        /// </summary>
        public event EventHandler FlushCache;

        #endregion

        #region Module Supports...

        /// <summary>
        /// Override on derivates class.
        /// Return true if the module is an Admin Module.
        /// </summary>
        public virtual bool AdminModule
        {
            get { return false; }
        }

        /// <summary>
        /// Override on derivates classes.
        /// Return true if the module is Searchable.
        /// </summary>
        public virtual bool Searchable
        {
            get { return false; }
        }

        // Jes1111
        /// <summary>
        /// Override on derived class.
        /// Return true if the module is Cacheable.
        /// </summary>
        public virtual bool Cacheable
        {
            get { return _cacheable; }
            set { _cacheable = value; }
        }


        /// <summary>
        /// Override on derived class.
        /// Return true if the module supports print in pop-up window.
        /// </summary>
        public bool SupportsPrint
        {
            get { return _supportsPrint; }
            set { _supportsPrint = value; }
        }

        /// <summary>
        /// This property indicates if the specified module supports can be
        /// collpasable (minimized/maximized/closed)
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool SupportCollapsable
        {
            get
            {
                if ( _moduleConfiguration == null )
                    return _supportsCollapseable;
                else
                    // jes1111 - return GlobalResources.SupportWindowMgmt && _moduleConfiguration.SupportCollapsable;
                    return Config.WindowMgmtControls && _moduleConfiguration.SupportCollapsable;
            }
            set { _supportsCollapseable = value; }
        } // end of SupportCollapsable

        /// <summary>
        /// This property indicates whether the module supports a Back button
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool SupportsBack
        {
            get { return _supportsBack; }
            set { _supportsBack = value; }
        }

        /// <summary>
        /// This property indicates if the module supports email
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool SupportsEmail
        {
            get { return _supportsEmail; }
            set { _supportsEmail = value; }
        }

        /// <summary>
        /// This property indicates if the specified module supports arrows to move modules
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool SupportsArrows
        {
            get
            {
                bool returnValue = _supportsArrows;

                if ( portalSettings.CustomSettings[ "SITESETTINGS_SHOW_MODULE_ARROWS" ] != null )
                    returnValue = returnValue &&
                                  bool.Parse( portalSettings.CustomSettings[ "SITESETTINGS_SHOW_MODULE_ARROWS" ].ToString() );

                if ( Settings[ "MODULESETTINGS_SHOW_ARROW_BUTTONS" ] != null )
                    returnValue = returnValue && bool.Parse( Settings[ "MODULESETTINGS_SHOW_ARROW_BUTTONS" ].ToString() );

                return returnValue;
            }
            set { _supportsArrows = value; }
        }

        /// <summary>
        /// This property indicates if the specified module workflow is enabled.
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool SupportsWorkflow
        {
            get
            {
                if ( _moduleConfiguration == null )
                    return _supportsWorkflow;
                else
                    return _supportsWorkflow && _moduleConfiguration.SupportWorkflow;
            }
            set { _supportsWorkflow = value; }
        }

        /// <summary>
        /// This property indicates if the specified module supports workflow.
        /// It returns the module property regardless of current module setting.
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool InnerSupportsWorkflow // changed Jes1111 (from 'internal')
        {
            get { return _supportsWorkflow; }
            set { _supportsWorkflow = value; }
        }

        /// <summary>
        /// This property indicates if the specified module supports help
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool SupportsHelp
        {
            get
            {
                if ( ( Settings[ "MODULESETTINGS_SHOW_HELP_BUTTON" ] == null ||
                     bool.Parse( Settings[ "MODULESETTINGS_SHOW_HELP_BUTTON" ].ToString() ) ) &&
                    ( ModuleConfiguration.DesktopSrc.Length != 0 ) )
                {
                    string aux = Path.ApplicationRoot + "/rb_documentation/Appleseed/" +
                                 ModuleConfiguration.DesktopSrc.Replace( ".", "_" ).ToString();
                    return Directory.Exists( HttpContext.Current.Server.MapPath( aux ) );
                }
                else
                    return false;
            }
        }

        #endregion

        #region Portal

        /// <summary>
        /// Stores current portal settings 
        /// </summary>
        public PortalSettings portalSettings
        {
            get
            {
                if ( Page != null )
                    return ( ( Page )Page ).portalSettings;
                else
                {
                    // Obtain PortalSettings from Current Context
                    if ( HttpContext.Current != null )
                    {
                        return ( PortalSettings )HttpContext.Current.Items[ "PortalSettings" ];
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// ID of portal in which module is instantiated
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public int PortalID
        {
            get { return _portalID; }
            set { _portalID = value; }
        }

        #endregion

        #region Page

        /// <summary>
        /// Returns the current page
        /// </summary>
        public new Page Page
        {
            get
            {
                if ( base.Page is Page )
                    return ( Page )base.Page;
                else
                    return null;
            }
        }

        #endregion

        #region Tab

        private int _tabID = 0;

        /// <summary>
        /// Stores current linked module ID if applicable
        /// </summary>
        public int PageID
        {
            get
            {
                if ( _tabID == 0 )
                {
                    Trace.Warn( "Request.Params['PageID'] = " + Request.Params[ "PageID" ] );
                    // Determine PageID if specified
                    if (HttpContext.Current != null && Request.Params["PageID"] != null) {
                        string pageIdString = Request.Params["PageID"];
                        _tabID = Int32.Parse(pageIdString.Split(',')[0]);
                    } else if (HttpContext.Current != null && Request.Params["TabID"] != null) {
                        string pageIdString = Request.Params["TabID"];
                        _tabID = Int32.Parse(pageIdString.Split(',')[0]);
                    }
                }
                return _tabID;
            }
        }

        /// <summary>
        /// Stores current tab settings 
        /// </summary>
        public Hashtable pageSettings
        {
            get
            {
                if ( Page != null )
                    return ( ( Page )Page ).pageSettings;
                else
                    return null;
            }
        }

        #endregion

        #region Title

        /// <summary>
        /// Return true if module has inner control of type title
        /// </summary>
        /// <remarks>Left here for backward compatibility until it proves redundant</remarks>
        protected bool HasTitle
        {
            get { return true; }
        }

        private DesktopModuleTitle _ModuleTitle;

        /// <summary>
        /// Inner Title control. Now only used for backward compatibility 
        /// </summary>
        public virtual DesktopModuleTitle ModuleTitle
        {
            get { return _ModuleTitle; }
            set { _ModuleTitle = value; }
        }

        /// <summary>
        /// Switch to turn on/off the display of Title text.
        /// </summary>
        /// <remarks>Note: won't turn off the display of Buttons like it used to! You can now have buttons displayed with no title text showing</remarks>
        public virtual bool ShowTitle
        {
            get
            {
                if ( HttpContext.Current != null ) // if it is not design time
                {
                    return ( bool.Parse( Settings[ "MODULESETTINGS_SHOW_TITLE" ].ToString() ) );
                }
                return false;
            }
            set
            {
                if ( HttpContext.Current != null ) // if it is not design time
                {
                    Settings[ "MODULESETTINGS_SHOW_TITLE" ] = value.ToString();
                }
            }
        }

        public bool ShareModule
        {
            get
            {
                if (HttpContext.Current != null) // if it is not design time
                {
                    return (bool.Parse(Settings["SHARE_MODULE"].ToString()));
                }
                return false;
            }
            set
            {
                if (HttpContext.Current != null) // if it is not design time
                {
                    Settings["SHARE_MODULE"] = value.ToString();
                }
            }
        }

        /// <summary>
        /// Switch to turn on/off the display of the module title text (not the buttons) in the print pop-up.
        /// </summary>
        public virtual bool ShowTitlePrint
        {
            get
            {
                if ( HttpContext.Current != null ) // if it is not design time
                {
                    return ( bool.Parse( Settings[ "MODULESETTINGS_SHOW_TITLE_PRINT" ].ToString() ) );
                }
                return false;
            }
            set
            {
                if ( HttpContext.Current != null ) // if it is not design time
                {
                    Settings[ "MODULESETTINGS_SHOW_TITLE_PRINT" ] = value.ToString();
                }
            }
        }

        private string titleText = string.Empty;

        /// <summary>
        /// The module title as it will be displayed on the page. Handles cultures automatically.
        /// </summary>
        public virtual string TitleText
        {
            get
            {
                if ( HttpContext.Current != null && titleText == string.Empty )
                // if it is not design time (and not overriden - Jes1111)
                {
                    if ( portalSettings.PortalContentLanguage != CultureInfo.InvariantCulture &&
                        Settings[ "MODULESETTINGS_TITLE_" + portalSettings.PortalContentLanguage.Name ] != null &&
                        Settings[ "MODULESETTINGS_TITLE_" + portalSettings.PortalContentLanguage.Name ].ToString().Length >
                        0 )
                    {
                        titleText =
                            Settings[ "MODULESETTINGS_TITLE_" + portalSettings.PortalContentLanguage.Name ].ToString();
                    }
                    else
                    {
                        if ( ModuleConfiguration != null )
                            titleText = ModuleConfiguration.ModuleTitle;
                        else
                            titleText = "TitleText Placeholder";
                    }
                }
                var title = string.Format("<span id=\"mTitle_{0}\" class=\"editTitle\">{1}</span>",
                            ModuleID.ToString(),
                            titleText);
                return title;
            }
            set { titleText = value; }
        }

        private string editText = "EDIT";
        private string editUrl;
        private string editTarget;

        /// <summary>
        /// Text for Edit Link
        /// </summary>
        public string EditText
        {
            get
            {
                if ( ModuleTitle != null && ModuleTitle.EditText.Length != 0 )
                    editText = ModuleTitle.EditText;
                return editText;
            }
            set { editText = value; }
        }

        /// <summary>
        /// Url for Edit Link
        /// </summary>
        public string EditUrl
        {
            get
            {
                if ( ModuleTitle != null && ModuleTitle.EditUrl.Length != 0 )
                    editUrl = ModuleTitle.EditUrl;
                return editUrl;
            }
            set { editUrl = value; }
        }

        /// <summary>
        /// Target frame/page for Edit Link
        /// </summary>
        public string EditTarget
        {
            get
            {
                if ( ModuleTitle != null && ModuleTitle.EditTarget.Length != 0 )
                    editUrl = ModuleTitle.EditTarget;
                return editTarget;
            }
            set { editTarget = value; }
        }

        private string addText = "ADD";
        private string addUrl;
        private string addTarget;

        /// <summary>
        /// Text for Add Link
        /// </summary>
        public string AddText
        {
            get
            {
                if ( ModuleTitle != null && ModuleTitle.AddText.Length != 0 )
                    addText = ModuleTitle.AddText;
                return addText;
            }
            set { addText = value; }
        }

        /// <summary>
        /// Url for Add Link
        /// </summary>
        /// <value>The add URL.</value>
        public string AddUrl
        {
            get
            {
                if ( ModuleTitle != null && ModuleTitle.AddUrl.Length != 0 )
                    addUrl = ModuleTitle.AddUrl;
                return addUrl;
            }
            set { addUrl = value; }
        }

        /// <summary>
        /// Target frame/page for Add Link
        /// </summary>
        public string AddTarget
        {
            get
            {
                if ( ModuleTitle != null && ModuleTitle.AddTarget.Length != 0 )
                    addTarget = ModuleTitle.AddTarget;
                return addTarget;
            }
            set { addTarget = value; }
        }

        private string propertiesText = "PROPERTIES";
        private string propertiesUrl = "~/DesktopModules/CoreModules/Admin/PropertyPage.aspx";
        private string propertiesTarget;

        /// <summary>
        /// Text for Properties Link
        /// </summary>
        public string PropertiesText
        {
            get
            {
                if ( ModuleTitle != null && ModuleTitle.PropertiesText.Length != 0 )
                    propertiesText = ModuleTitle.PropertiesText;
                return propertiesText;
            }
            set { propertiesText = value; }
        }

        /// <summary>
        /// Url for Properties Link
        /// </summary>
        public string PropertiesUrl
        {
            get
            {
                if ( ModuleTitle != null && ModuleTitle.PropertiesUrl.Length != 0 )
                    propertiesUrl = ModuleTitle.PropertiesUrl;
                return propertiesUrl;
            }
            set { propertiesUrl = value; }
        }

        /// <summary>
        /// Target frame/page for Properties Link
        /// </summary>
        public string PropertiesTarget
        {
            get
            {
                if ( ModuleTitle != null && ModuleTitle.PropertiesTarget.Length != 0 )
                    propertiesTarget = ModuleTitle.PropertiesTarget;
                return propertiesTarget;
            }
            set { propertiesTarget = value; }
        }

        private string securityText = "SECURITY";
        private string securityUrl = "~/DesktopModules/CoreModules/Admin/ModuleSettings.aspx";
        private string securityTarget;

        /// <summary>
        /// Text for Security Link
        /// </summary>
        public string SecurityText
        {
            get
            {
                if ( ModuleTitle != null && ModuleTitle.SecurityText.Length != 0 )
                    securityText = ModuleTitle.SecurityText;
                return securityText;
            }
            set { securityText = value; }
        }

        /// <summary>
        /// Url for Security Link
        /// </summary>
        public string SecurityUrl
        {
            get
            {
                if ( ModuleTitle != null && ModuleTitle.SecurityUrl.Length != 0 )
                    securityUrl = ModuleTitle.SecurityUrl;
                return securityUrl;
            }
            set { securityUrl = value; }
        }

        /// <summary>
        /// Target frame/page for Security Link
        /// </summary>
        public string SecurityTarget
        {
            get
            {
                if ( ModuleTitle != null && ModuleTitle.SecurityTarget.Length != 0 )
                    securityTarget = ModuleTitle.SecurityTarget;
                return securityTarget;
            }
            set { securityTarget = value; }
        }

        #endregion

        #region ModuleButtons

        private ModuleButton.RenderOptions buttonsRenderAs = ModuleButton.RenderOptions.ImageOnly;

        /// <summary>
        /// Determines how ModuleButtons are rendered: as TextOnly, TextAndImage or ImageOnly. ImageOnly is the 'classic' Appleseed style.
        /// </summary>
        public ModuleButton.RenderOptions ButtonsRenderAs
        {
            get
            {
                if ( HttpContext.Current != null ) // if it is not design time
                {
                    if ( Settings[ "MODULESETTINGS_BUTTON_DISPLAY" ] != null &&
                        Settings[ "MODULESETTINGS_BUTTON_DISPLAY" ].ToString().Length != 0 )
                        buttonsRenderAs =
                            ( ModuleButton.RenderOptions )int.Parse( Settings[ "MODULESETTINGS_BUTTON_DISPLAY" ].ToString() );
                }
                return buttonsRenderAs;
            }
            set { buttonsRenderAs = value; }
        }

        private ModuleButton propertiesButton;

        /// <summary>
        /// Module Properties button
        /// </summary>
        public ModuleButton PropertiesButton
        {
            get
            {
                if ( propertiesButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanProperties )
                    {
                        // create the button
                        propertiesButton = new ModuleButton();
                        propertiesButton.Group = ModuleButton.ButtonGroup.Admin;
                        propertiesButton.EnglishName = "Properties";
                        propertiesButton.TranslationKey = "PROPERTIES";
                        propertiesButton.Image = CurrentTheme.GetImage( "Buttons_Properties", "Properties.gif" );
                        if ( PropertiesUrl.IndexOf( "?" ) >= 0 )
                            //Do not change if  the querystring is present (shortcut patch)
                            //if ( this.ModuleID != OriginalModuleID ) // shortcut
                            propertiesButton.HRef = PropertiesUrl;
                        else
                            propertiesButton.HRef =
                                HttpUrlBuilder.BuildUrl( PropertiesUrl, PageID, "mID=" + ModuleID.ToString() );
                        propertiesButton.Target = PropertiesTarget;
                        propertiesButton.RenderAs = ButtonsRenderAs;
                    }
                }
                return propertiesButton;
            }
        }


        private ModuleButton securityButton;

        /// <summary>
        /// Module Security button
        /// </summary>
        public ModuleButton SecurityButton
        {
            get
            {
                if ( securityButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanSecurity )
                    {
                        // create the button
                        securityButton = new ModuleButton();
                        securityButton.Group = ModuleButton.ButtonGroup.Admin;
                        securityButton.EnglishName = "Security";
                        securityButton.TranslationKey = "SECURITY";
                        securityButton.Image = CurrentTheme.GetImage( "Buttons_Security", "Security.gif" );
                        if ( SecurityUrl.IndexOf( "?" ) >= 0 )
                            //Do not change if  the querystring is present (shortcut patch)
                            securityButton.HRef = SecurityUrl;
                        else
                            securityButton.HRef =
                                HttpUrlBuilder.BuildUrl( SecurityUrl, PageID, "mID=" + ModuleID.ToString() );
                        securityButton.Target = SecurityTarget;
                        securityButton.RenderAs = ButtonsRenderAs;
                    }
                }
                return securityButton;
            }
        }

        private ModuleButton deleteModuleButton;

        /// <summary>
        /// "Delete this Module" button
        /// </summary>
        public ModuleButton DeleteModuleButton
        {
            get
            {
                if ( deleteModuleButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanDeleteModule )
                    {
                        // create the button
                        deleteModuleButton = new ModuleButton();
                        deleteModuleButton.Group = ModuleButton.ButtonGroup.Admin;
                        deleteModuleButton.TranslationKey = "DELETEMODULE";
                        deleteModuleButton.EnglishName = "Delete this module";
                        deleteModuleButton.Image = CurrentTheme.GetImage( "Buttons_DeleteModule", "Delete.gif" );
                        deleteModuleButton.RenderAs = ButtonsRenderAs;

                        // TODO: This JavaScript Function Is used for different controls and should be in one place
                        // (it's also overweight considering that Javascript has a standard confirm() function - Jes1111)
                        if ( Page.Request.Browser.EcmaScriptVersion.Major >= 1 &&
                            !( Page.ClientScript.IsClientScriptBlockRegistered( this.Page.GetType(), "confirmDelete" ) ) )
                        {
                            string[] s = { "CONFIRM_DELETE" };
                            Page.ClientScript.RegisterClientScriptBlock( this.Page.GetType(), "confirmDelete",
                                                                        PortalSettings.GetStringResource(
                                                                            "CONFIRM_DELETE_SCRIPT",
                                                                            s ) );
                        }
                        if ( deleteModuleButton.Attributes[ "onclick" ] != null )
                            deleteModuleButton.Attributes[ "onclick" ] = "return confirmDelete();" +
                                                                       deleteModuleButton.Attributes[ "onclick" ];
                        else
                            deleteModuleButton.Attributes.Add( "onclick", "return confirmDelete();" );

                        deleteModuleButton.ServerClick += new EventHandler( DeleteModuleButton_Click );
                    }
                }
                return deleteModuleButton;
            }
        }

        private ModuleButton helpButton;

        /// <summary>
        /// Module button that will launch the module help in a pop-up window
        /// </summary>
        public ModuleButton HelpButton
        {
            get
            {
                if ( helpButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanHelp )
                    {
                        // build the HRef
                        string aux = ModuleConfiguration.DesktopSrc.Replace( ".", "_" ).ToString();
                        int fileNameStart = aux.LastIndexOf( "/" );
                        string fileName = aux.Substring( fileNameStart + 1 );
                        StringBuilder sb = new StringBuilder();
                        sb.Append( Path.ApplicationRoot );
                        sb.Append( @"/rb_documentation/Viewer.aspx?loc=Appleseed/" );
                        sb.Append( aux );
                        sb.Append( "&src=" );
                        sb.Append( fileName );

                        // create the button
                        helpButton = new ModuleButton();
                        helpButton.Group = ModuleButton.ButtonGroup.User;
                        helpButton.TranslationKey = "BTN_HELP";
                        helpButton.EnglishName = "Help";
                        helpButton.HRef = sb.ToString();
                        helpButton.PopUp = true;
                        helpButton.Target = "AppleseedHelp";
                        helpButton.PopUpOptions =
                            "toolbar=1,location=0,directories=0,status=0,menubar=1,scrollbars=1,resizable=1,width=600,height=400,screenX=15,screenY=15,top=15,left=15";
                        helpButton.Image = CurrentTheme.GetImage( "Buttons_Help", "Help.gif" );
                        helpButton.RenderAs = ButtonsRenderAs;
                    }
                }
                return helpButton;
            }
        }

        // Tiptopweb
        // Nicholas Smeaton: custom buttons from module developer enhancement added in Appleseed version 1.4.0.1767a - 03/07/2004
        private ModuleButton upButton;

        /// <summary>
        /// Module Up button
        /// </summary>
        public ModuleButton UpButton
        {
            get
            {
                if ( upButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanArrows && ModuleConfiguration.ModuleOrder != 1 )
                    {
                        // create the button
                        upButton = new ModuleButton();
                        upButton.Group = ModuleButton.ButtonGroup.Admin;
                        upButton.TranslationKey = "MOVE_UP";
                        upButton.EnglishName = "Move up";
                        upButton.Image = CurrentTheme.GetImage( "Buttons_Up", "Up.gif" );
                        upButton.Attributes.Add( "direction", "up" );
                        upButton.Attributes.Add( "pane", ModuleConfiguration.PaneName.ToLower() );
                        upButton.RenderAs = ButtonsRenderAs;
                        upButton.ServerClick += new EventHandler( UpDown_Click );
                    }
                }
                return upButton;
            }
        }

        private ModuleButton downButton;

        /// <summary>
        /// Module Down button
        /// </summary>
        public ModuleButton DownButton
        {
            get
            {
                if ( downButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanArrows )
                    {
                        ArrayList sourceList = GetModules( ModuleConfiguration.PaneName.ToLower() );
                        ModuleItem m = ( ModuleItem )sourceList[ sourceList.Count - 1 ];
                        if ( ModuleConfiguration.ModuleOrder != m.Order )
                        {
                            // create the button
                            downButton = new ModuleButton();
                            downButton.Group = ModuleButton.ButtonGroup.Admin;
                            downButton.TranslationKey = "MOVE_DOWN";
                            downButton.EnglishName = "Move down";
                            downButton.Image = CurrentTheme.GetImage( "Buttons_Down", "Down.gif" );
                            downButton.Attributes.Add( "direction", "down" );
                            downButton.Attributes.Add( "pane", ModuleConfiguration.PaneName.ToLower() );
                            downButton.RenderAs = ButtonsRenderAs;
                            downButton.ServerClick += new EventHandler( UpDown_Click );
                        }
                    }
                }
                return downButton;
            }
        }

        private ModuleButton leftButton;

        /// <summary>
        /// Module Left button
        /// </summary>
        public ModuleButton LeftButton
        {
            get
            {
                if ( leftButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanArrows && ModuleConfiguration.PaneName.ToLower() != "leftpane" )
                    {
                        string leftButtonTargetPane = "contentpane";
                        if ( ModuleConfiguration.PaneName.ToLower() == "contentpane" )
                            leftButtonTargetPane = "leftpane";
                        else if ( ModuleConfiguration.PaneName.ToLower() == "rightpane" )
                            leftButtonTargetPane = "contentpane";

                        // create the button
                        leftButton = new ModuleButton();
                        leftButton.Group = ModuleButton.ButtonGroup.Admin;
                        leftButton.TranslationKey = "MOVE_LEFT";
                        leftButton.EnglishName = "Move left";
                        leftButton.Image = CurrentTheme.GetImage( "Buttons_Left", "Left.gif" );
                        leftButton.Attributes.Add( "sourcepane", ModuleConfiguration.PaneName.ToLower() );
                        leftButton.Attributes.Add( "targetpane", leftButtonTargetPane );
                        leftButton.RenderAs = ButtonsRenderAs;
                        leftButton.ServerClick += new EventHandler( RightLeft_Click );
                    }
                }
                return leftButton;
            }
        }

        private ModuleButton rightButton;

        /// <summary>
        /// Module Right button
        /// </summary>
        public ModuleButton RightButton
        {
            get
            {
                if ( rightButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanArrows && ModuleConfiguration.PaneName.ToLower() != "rightpane" )
                    {
                        string rightButtonTargetPane = "contentpane";
                        if ( ModuleConfiguration.PaneName.ToLower() == "contentpane" )
                            rightButtonTargetPane = "rightpane";
                        else if ( ModuleConfiguration.PaneName.ToLower() == "leftpane" )
                            rightButtonTargetPane = "contentpane";

                        // create the button
                        rightButton = new ModuleButton();
                        rightButton.Group = ModuleButton.ButtonGroup.Admin;
                        rightButton.TranslationKey = "MOVE_RIGHT";
                        rightButton.EnglishName = "Move right";
                        rightButton.Image = CurrentTheme.GetImage( "Buttons_Right", "Right.gif" );
                        rightButton.Attributes.Add( "sourcepane", ModuleConfiguration.PaneName.ToLower() );
                        rightButton.Attributes.Add( "targetpane", rightButtonTargetPane );
                        rightButton.RenderAs = ButtonsRenderAs;
                        rightButton.ServerClick += new EventHandler( RightLeft_Click );
                    }
                }
                return rightButton;
            }
        }

        private ModuleButton readyToApproveButton;

        /// <summary>
        /// Module ReadyToApprove button
        /// </summary>
        public ModuleButton ReadyToApproveButton
        {
            get
            {
                if ( readyToApproveButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanRequestApproval )
                    {
                        // create the button
                        readyToApproveButton = new ModuleButton();
                        readyToApproveButton.Group = ModuleButton.ButtonGroup.Admin;
                        readyToApproveButton.TranslationKey = ReadyToApproveText;
                        readyToApproveButton.EnglishName = "Request approval";
                        readyToApproveButton.HRef =
                            HttpUrlBuilder.BuildUrl( "~/DesktopModules/Workflow/RequestModuleContentApproval.aspx",
                                                    PageID, "mID=" + ModuleID.ToString() );
                        readyToApproveButton.Image =
                            CurrentTheme.GetImage( "Buttons_ReadyToApprove", "ReadyToApprove.gif" );
                        readyToApproveButton.RenderAs = ButtonsRenderAs;
                    }
                }
                return readyToApproveButton;
            }
        }

        private ModuleButton revertButton;

        /// <summary>
        /// Module Revert button
        /// </summary>
        public ModuleButton RevertButton
        {
            get
            {
                if ( revertButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanRequestApproval )
                    {
                        // create the button
                        revertButton = new ModuleButton();
                        revertButton.Group = ModuleButton.ButtonGroup.Admin;
                        revertButton.TranslationKey = RevertText;
                        revertButton.EnglishName = "Revert";
                        revertButton.Image = CurrentTheme.GetImage( "Buttons_Revert", "Revert.gif" );
                        revertButton.ServerClick += new EventHandler( RevertToProductionContent );
                        revertButton.RenderAs = ButtonsRenderAs;
                    }
                }
                return revertButton;
            }
        }


        private ModuleButton approveButton;

        /// <summary>
        /// Module Approve button
        /// </summary>
        public ModuleButton ApproveButton
        {
            get
            {
                if ( approveButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanApproveReject )
                    {
                        // create the button
                        approveButton = new ModuleButton();
                        approveButton.Group = ModuleButton.ButtonGroup.Admin;
                        approveButton.TranslationKey = ApproveText;
                        approveButton.EnglishName = "Approve";
                        approveButton.HRef =
                            HttpUrlBuilder.BuildUrl( "~/DesktopModules/Workflow/ApproveModuleContent.aspx", PageID,
                                                    "mID=" + ModuleID.ToString() );
                        approveButton.Image = CurrentTheme.GetImage( "Buttons_Approve", "Approve.gif" );
                        approveButton.RenderAs = ButtonsRenderAs;
                    }
                }
                return approveButton;
            }
        }

        private ModuleButton rejectButton;

        /// <summary>
        /// Module Reject button
        /// </summary>
        public ModuleButton RejectButton
        {
            get
            {
                if ( rejectButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanApproveReject )
                    {
                        // create the button
                        rejectButton = new ModuleButton();
                        rejectButton.Group = ModuleButton.ButtonGroup.Admin;
                        rejectButton.TranslationKey = RejectText;
                        rejectButton.EnglishName = "Reject";
                        rejectButton.HRef =
                            HttpUrlBuilder.BuildUrl( "~/DesktopModules/Workflow/RejectModuleContent.aspx", PageID,
                                                    "mID=" + ModuleID.ToString() );
                        rejectButton.Image = CurrentTheme.GetImage( "Buttons_Reject", "Reject.gif" );
                        rejectButton.RenderAs = ButtonsRenderAs;
                    }
                }
                return rejectButton;
            }
        }

        private ModuleButton publishButton;

        /// <summary>
        /// Module Version button
        /// </summary>
        public ModuleButton PublishButton
        {
            get
            {
                if ( publishButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanPublish )
                    {
                        // create the button
                        publishButton = new ModuleButton();
                        publishButton.Group = ModuleButton.ButtonGroup.Admin;
                        publishButton.TranslationKey = PublishText;
                        publishButton.EnglishName = "Publish";
                        // modified by Hongwei Shen
                        // publishButton.HRef = GetPublishUrl();
                        publishButton.ServerClick += new EventHandler( publishButton_ServerClick );
                        // end of modification
                        publishButton.Image = CurrentTheme.GetImage( "Buttons_Publish", "Publish.gif" );
                        publishButton.RenderAs = ButtonsRenderAs;
                    }
                }
                return publishButton;
            }
        }

        private ModuleButton _versionButton;

        /// <summary>
        /// Module Version button
        /// </summary>
        public ModuleButton VersionButton
        {
            get
            {
                if ( _versionButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanVersion )
                    {
                        // create the button
                        _versionButton = new ModuleButton();
                        _versionButton.Group = ModuleButton.ButtonGroup.Admin;
                        if ( Version == WorkFlowVersion.Staging )
                        {
                            _versionButton.TranslationKey = ProductionVersionText;
                            _versionButton.EnglishName = "To production version";
                            _versionButton.Image =
                                CurrentTheme.GetImage( "Buttons_VersionToProduction", "VersionToProduction.gif" );
                        }
                        else
                        {
                            _versionButton.TranslationKey = StagingVersionText;
                            _versionButton.EnglishName = "To staging version";
                            _versionButton.Image =
                                CurrentTheme.GetImage( "Buttons_VersionToStaging", "VersionToStaging.gif" );
                        }
                        _versionButton.HRef = GetOtherVersionUrl();
                        _versionButton.Target = EditTarget;
                        _versionButton.RenderAs = ButtonsRenderAs;
                    }
                }
                return _versionButton;
            }
        }

        private ModuleButton editButton;

        /// <summary>
        /// Module edit button
        /// </summary>
        public ModuleButton EditButton
        {
            get
            {
                if ( editButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanEdit )
                    {
                        // create the button
                        editButton = new ModuleButton();
                        editButton.Group = ModuleButton.ButtonGroup.Admin;
                        editButton.TranslationKey = EditText;
                        editButton.EnglishName = "Edit";
                        if ( EditUrl.IndexOf( "?" ) >= 0 ) //Do not change if  the querystring is present
                            //if ( this.ModuleID != OriginalModuleID )
                            editButton.HRef = EditUrl;
                        else
                            editButton.HRef = HttpUrlBuilder.BuildUrl( EditUrl, PageID, "mID=" + ModuleID.ToString() );
                        editButton.Target = EditTarget;
                        editButton.Image = CurrentTheme.GetImage( "Buttons_Edit", "Edit.gif" );
                        editButton.RenderAs = ButtonsRenderAs;
                    }
                }
                return editButton;
            }
        }

        private ModuleButton addButton;

        /// <summary>
        /// Module Add button
        /// </summary>
        public ModuleButton AddButton
        {
            get
            {
                if ( addButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanAdd )
                    {
                        // create the button
                        addButton = new ModuleButton();
                        addButton.Group = ModuleButton.ButtonGroup.Admin;
                        addButton.TranslationKey = AddText;
                        addButton.EnglishName = "Add";
                        if ( AddUrl.IndexOf( "?" ) >= 0 ) //Do not change if  the querystring is present
                            //if ( this.ModuleID != OriginalModuleID )
                            AddButton.HRef = AddUrl;
                        else
                            AddButton.HRef = HttpUrlBuilder.BuildUrl( AddUrl, PageID, "mID=" + ModuleID.ToString() );
                        addButton.Target = AddTarget;
                        addButton.Image = CurrentTheme.GetImage( "Buttons_Add", "Add.gif" );
                        addButton.RenderAs = ButtonsRenderAs;
                    }
                }
                return addButton;
            }
        }

        private ModuleButton backButton;

        /// <summary>
        /// Module button that will return to previous tab
        /// </summary>
        public ModuleButton BackButton
        {
            get
            {
                if ( backButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanBack )
                    {
                        // create the button
                        backButton = new ModuleButton();
                        backButton.Group = ModuleButton.ButtonGroup.User;
                        backButton.TranslationKey = "BTN_BACK";
                        backButton.EnglishName = "Back to previous page";
                        backButton.HRef = Request.UrlReferrer.ToString();
                        backButton.Image = CurrentTheme.GetImage( "Buttons_Back", "Back.gif" );
                        backButton.RenderAs = ButtonsRenderAs;
                    }
                }
                return backButton;
            }
        }

        private ModuleButton printButton;

        /// <summary>
        /// Module button that will launch the module in a pop-up window suitable for printing
        /// </summary>
        public ModuleButton PrintButton
        {
            get
            {
                if ( printButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanPrint )
                    {
                        // build the HRef
                        StringBuilder _url = new StringBuilder();
                        _url.Append( Path.ApplicationRoot );
                        _url.Append( "/app_support/print.aspx?" );
                        _url.Append( Request.QueryString.ToString() );
                        if ( !( Request.QueryString.ToString().ToLower().IndexOf( "mid=" ) > 0 ) )
                        {
                            _url.Append( "&mID=" );
                            _url.Append( ModuleID.ToString() );
                        }
                        _url.Append( "&ModId=" );
                        _url.Append( OriginalModuleID.ToString() );

                        // create the button
                        printButton = new ModuleButton();
                        printButton.Group = ModuleButton.ButtonGroup.User;
                        printButton.Image = CurrentTheme.GetImage( "Buttons_Print", "Print.gif" );
                        printButton.TranslationKey = "BTN_PRINT";
                        printButton.EnglishName = "Print this";
                        printButton.HRef = _url.ToString();
                        printButton.PopUp = true;
                        printButton.Target = "AppleseedPrint";
                        printButton.PopUpOptions =
                            "toolbar=1,menubar=1,resizable=1,scrollbars=1,width=600,height=400,left=15,top=15";
                        printButton.RenderAs = ButtonsRenderAs;
                    }
                }
                return printButton;
            }
        }

        private ModuleButton emailButton;

        /// <summary>
        /// Module button that will launch a pop-up window to allow the module contents to be emailed
        /// </summary>
        /// <remarks>Not implemented yet.</remarks>
        public ModuleButton EmailButton
        {
            get
            {
                if ( emailButton == null && HttpContext.Current != null )
                {
                    // check authority
                    if ( CanEmail )
                    {
                        // not implemented
                        //					javaScript = "EmailWindow=window.open('" 
                        //						+ HttpUrlBuilder.BuildUrl("email.aspx","src=" + portalModule.ModuleCacheKey + "content") 
                        //						+ "','EmailWindow','toolbar=yes,location=no,directories=no,status=no,menubar=yes,scrollbars=yes,resizable=yes,width=640,height=480,left=15,top=15'); return false;";
                        //					EmailButton.Text = Esperantus.General.GetString("BTN_Email","Email this",null) + "...";
                        //					EmailButton.NavigateUrl = string.Empty;
                        //					EmailButton.CssClass = "rb_mod_btn";
                        //					EmailButton.Attributes.Add("onclick", javaScript);
                        //					EmailButton.ImageUrl = CurrentTheme.GetImage("Buttons_Email").ImageUrl;
                        //					ButtonList.Add(EmailButton);
                    }
                    emailButton = null;
                }
                return emailButton;
            }
        }

        private LinkButton minMaxButton = null;

        /// <summary>
        /// Module button to minimize/maximize module
        /// </summary>
        //public LinkButton MinMaxButton {
        //    get {
        //        if ( minMaxButton == null && HttpContext.Current != null ) {
        //            // check authority
        //            if ( _vcm != null && !UserDesktop.isClosed( _vcm.ModuleID ) && CanMinimized ) {
        //                // create the button based on current view
        //                if ( !UserDesktop.isMinimized( _vcm.ModuleID ) )
        //                    minMaxButton =
        //                        _vcm.create( WindowStateStrings.ButtonMinName, WindowStateStrings.ButtonMinLocalized );
        //                else {
        //                    minMaxButton =
        //                        _vcm.create( WindowStateStrings.ButtonMaxName, WindowStateStrings.ButtonMaxLocalized );
        //                    // we are minimized -- show the user a hint by changing the color [future]
        //                    // min_hint_ = true;
        //                }

        //                // set additional button properties
        //                minMaxButton.CssClass = "rb_mod_btn";
        //            }
        //        }
        //        return minMaxButton;
        //    }
        //}

        private LinkButton closeButton = null;

        /// <summary>
        /// Module button to close module
        /// </summary>
        //public LinkButton CloseButton {
        //    get {
        //        if ( closeButton == null && HttpContext.Current != null ) {
        //            // check authority
        //            // jes1111 - if (_vcm != null && !UserDesktop.isClosed(_vcm.ModuleID) && CanClose && GlobalResources.SupportWindowMgmtClose)
        //            if ( _vcm != null && !UserDesktop.isClosed( _vcm.ModuleID ) && CanClose && Config.WindowMgmtWantClose ) {
        //                // create the button
        //                closeButton =
        //                    _vcm.create( WindowStateStrings.ButtonCloseName, WindowStateStrings.ButtonClosedLocalized );

        //                // set attribute to confirm delete
        //                setDeleteAttributes( ref closeButton );

        //                // set additional button properties
        //                closeButton.CssClass = "rb_mod_btn";
        //            }
        //        }
        //        return closeButton;
        //    }
        //}

        #endregion

        #region Permissions

        /// <summary>
        /// View permission
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool IsViewable
        {
            get
            {
                if ( _moduleConfiguration == null || _moduleConfiguration.AuthorizedViewRoles == null )
                    return false;

                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if ( _canView == 0 )
                {
                    if ( PortalSecurity.IsInRoles( _moduleConfiguration.AuthorizedViewRoles ) )
                    {
                        _canView = 1;
                    }
                    else
                    {
                        _canView = 2;
                    }
                }
                return ( _canView == 1 );
            }
        }

        /// <summary>
        /// Add permission
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool IsAddable
        {
            get
            {
                if ( _moduleConfiguration == null || _moduleConfiguration.AuthorizedAddRoles == null )
                    return false;

                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if ( _canAdd == 0 )
                {
                    // Change by Geert.Audenaert@Syntegra.Com
                    // Date: 7/2/2003
                    if ( SupportsWorkflow && Version == WorkFlowVersion.Production )
                    {
                        _canAdd = 2;
                    }
                    else
                    {
                        // End Change Geert.Audenaert@Syntegra.Com
                        if ( PortalSecurity.IsInRoles( _moduleConfiguration.AuthorizedAddRoles ) )
                        {
                            _canAdd = 1;
                        }
                        else
                        {
                            _canAdd = 2;
                        }
                        // Change by Geert.Audenaert@Syntegra.Com
                    }
                    // Date: 7/2/2003
                    // End Change Geert.Audenaert@Syntegra.Com
                }
                return ( _canAdd == 1 );
            }
        }

        /// <summary>
        /// Edit permission
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool IsEditable
        {
            get
            {
                if ( _moduleConfiguration == null || _moduleConfiguration.AuthorizedEditRoles == null )
                    return false;

                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if ( _canEdit == 0 )
                {
                    // Change by Geert.Audenaert@Syntegra.Com
                    // Date: 7/2/2003
                    if ( SupportsWorkflow && Version == WorkFlowVersion.Production )
                    {
                        _canEdit = 2;
                    }
                    else
                    {
                        // End Change Geert.Audenaert@Syntegra.Com
                        //						if (portalSettings.AlwaysShowEditButton == true || PortalSecurity.IsInRoles(_moduleConfiguration.AuthorizedEditRoles))
                        if ( PortalSecurity.IsInRoles( _moduleConfiguration.AuthorizedEditRoles ) )
                        {
                            _canEdit = 1;
                        }
                        else
                        {
                            _canEdit = 2;
                        }
                        // Change by Geert.Audenaert@Syntegra.Com
                        // Date: 7/2/2003
                    }
                    // End Change Geert.Audenaert@Syntegra.Com
                }
                return ( _canEdit == 1 );
            }
        }

        /// <summary>
        /// Delete permission
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool IsDeleteable
        {
            get
            {
                if ( _moduleConfiguration == null || _moduleConfiguration.AuthorizedDeleteRoles == null )
                    return false;

                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if ( _canDelete == 0 )
                {
                    // Change by Geert.Audenaert@Syntegra.Com
                    // Date: 7/2/2003
                    if ( SupportsWorkflow && Version == WorkFlowVersion.Production )
                    {
                        _canDelete = 2;
                    }
                    else
                    {
                        // End Change Geert.Audenaert@Syntegra.Com
                        if ( PortalSecurity.IsInRoles( _moduleConfiguration.AuthorizedDeleteRoles ) )
                        {
                            _canDelete = 1;
                        }
                        else
                        {
                            _canDelete = 2;
                        }
                        // Change by Geert.Audenaert@Syntegra.Com
                        // Date: 7/2/2003
                    }
                    // End Change Geert.Audenaert@Syntegra.Com
                }
                return ( _canDelete == 1 );
            }
        }

        /// <summary>
        /// Edit properties permission
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool ArePropertiesEditable
        {
            get
            {
                if ( _moduleConfiguration == null || _moduleConfiguration.AuthorizedPropertiesRoles == null )
                    return false;

                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if ( _canProperties == 0 )
                {
                    if ( PortalSecurity.IsInRoles( _moduleConfiguration.AuthorizedPropertiesRoles ) )
                    {
                        _canProperties = 1;
                    }
                    else
                    {
                        _canProperties = 2;
                    }
                }
                return ( _canProperties == 1 );
            }
        }

        /// <summary>
        /// Minimize permission
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanMinimized
        {
            get
            {
                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if ( _canMin == 0 )
                {
                    if ( PortalSecurity.IsInRoles( _moduleConfiguration.AuthorizedViewRoles ) )
                    {
                        _canMin = 1;
                    }
                    else
                    {
                        _canMin = 2;
                    }
                }
                return ( _canMin == 1 );
            }
        } // end of CanMinimized

        /// <summary>
        /// Close permission
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanClose
        {
            get
            {
                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if ( _canClose == 0 )
                {
                    if ( PortalSecurity.IsInRoles( _moduleConfiguration.AuthorizedDeleteRoles ) )
                    {
                        _canClose = 1;
                    }
                    else
                    {
                        _canClose = 2;
                    }
                }
                return ( _canClose == 1 );
            }
        } // end of CanClose

        /// <summary>
        /// Print permission
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanPrint
        {
            get
            {
                if ( SupportsPrint && Settings[ "MODULESETTINGS_SHOW_PRINT_BUTTION" ] != null &&
                    bool.Parse( Settings[ "MODULESETTINGS_SHOW_PRINT_BUTTION" ].ToString() ) )
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Permission for DeleteModuleButton
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanDeleteModule
        {
            get
            {
                if ( PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedDeleteModuleRoles ) &&
                    portalSettings.ActivePage.PageID == ModuleConfiguration.PageID )
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Permission for HelpButton
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanHelp
        {
            get
            {
                if ( SupportsHelp &&
                    ( ( PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedEditRoles ) ) ||
                     ( PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedAddRoles ) ) ||
                     ( PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedDeleteRoles ) ) ||
                     ( PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedPropertiesRoles ) ) ||
                     ( PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedPublishingRoles ) ) ) )
                    return true;
                else
                    return false;
            }
        }


        /// <summary>
        /// Permission for BackButton
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanBack
        {
            get
            {
                if ( SupportsBack && ShowBack && Request.UrlReferrer != null )
                {
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Permission for EmailButton
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanEmail
        {
            get
            {
                if ( SupportsEmail && Settings[ "ShowEmailButton" ] != null &&
                    bool.Parse( Settings[ "ShowEmailButton" ].ToString() ) )
                {
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Permission for EditButton
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanEdit
        {
            get
            {
                if ( ModuleConfiguration == null || portalSettings.ActivePage.PageID != ModuleConfiguration.PageID )
                    return false;

                if ( ( SupportsWorkflow && Version == WorkFlowVersion.Staging ) || !SupportsWorkflow )
                {
                    if ( ( PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedEditRoles ) ) && ( EditUrl != null ) &&
                        ( EditUrl.Length != 0 ) &&
                        ( WorkflowStatus == WorkflowState.Original || WorkflowStatus == WorkflowState.Working ) )
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
        }

        public ArrayList GetUsersThatCanEdit()
        {
            string roles = ModuleConfiguration.AuthorizedEditRoles;
            return GetUsersInRoles(roles);
        }

        public ArrayList GetUsersThatCanView()
        {
            string roles = ModuleConfiguration.AuthorizedViewRoles;
            return GetUsersInRoles(roles);
        }

        public ArrayList GetUsersThatCanAdd()
        {
            string roles = ModuleConfiguration.AuthorizedAddRoles;
            return GetUsersInRoles(roles);
        }


        private static ArrayList GetUsersInRoles(string roles)
        {
            ArrayList result = new ArrayList(); ;
         
            HttpContext context = HttpContext.Current;

            if (roles != null) {
                foreach (string splitRole in roles.Split(new char[] { ';' })) {
                    if (!String.IsNullOrEmpty(splitRole)) {
                        if (splitRole != null && splitRole.Length != 0 && splitRole == "All Users") {
                            MembershipUserCollection collection = Membership.GetAllUsers();
                            foreach (MembershipUser user in collection) {
                                if(!result.Contains(user.Email)){
                                    result.Add(user.Email);
                                }
                            }
                        } else if (splitRole == "Authenticated Users" && context.Request.IsAuthenticated) {
                            if (!result.Contains(Membership.GetUser().Email)) {
                                result.Add(Membership.GetUser().Email);
                            }
                        } else if ((splitRole == "Unauthenticated Users") && (!context.Request.IsAuthenticated)) {
                            //TODO: no me queda claro que devolver en este caso
                            MembershipUserCollection collection = Membership.GetAllUsers();
                            foreach (MembershipUser user in collection) {
                                if (!result.Contains(user.Email)) {
                                    result.Add(user.Email);
                                }
                            }
                        } else {
                            string[] users= Roles.GetUsersInRole(splitRole);
                            foreach (string user in users) {
                                if (!result.Contains(user)) {
                                    result.Add(user);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Permission for AddButton
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanAdd
        {
            get
            {
                if ( ModuleConfiguration == null || portalSettings.ActivePage.PageID != ModuleConfiguration.PageID )
                    return false;

                if ( ( SupportsWorkflow && Version == WorkFlowVersion.Staging ) || !SupportsWorkflow )
                {
                    if ( ( PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedAddRoles ) ) && ( AddUrl != null ) &&
                        ( AddUrl.Length != 0 ) &&
                        ( WorkflowStatus == WorkflowState.Original || WorkflowStatus == WorkflowState.Working ) )
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Permission for Version Button
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanVersion
        {
            get
            {
                if ( ModuleConfiguration == null )
                    return false;

                if ( SupportsWorkflow &&
                    ( PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedAddRoles ) ||
                     PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedDeleteRoles ) ||
                     PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedEditRoles ) ||
                     PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedApproveRoles ) ||
                     PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedPublishingRoles ) ) &&
                    ( ProductionVersionText != null ) && ( StagingVersionText != null ) )
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Permission for Publish Button
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanPublish
        {
            get
            {
                if ( ModuleConfiguration == null )
                    return false;

                if ( SupportsWorkflow && PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedPublishingRoles ) &&
                    ( PublishText != null ) && Version == WorkFlowVersion.Staging &&
                    WorkflowStatus == WorkflowState.Approved )
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Permission for Approve/Reject Buttons
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanApproveReject
        {
            get
            {
                if ( ModuleConfiguration == null )
                    return false;

                if ( SupportsWorkflow && PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedApproveRoles ) &&
                    ( ApproveText != null ) && ( RejectText != null ) && Version == WorkFlowVersion.Staging &&
                    WorkflowStatus == WorkflowState.ApprovalRequested )
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Permission for ReadyToApprove and Revert Buttons
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanRequestApproval
        {
            get
            {
                if ( ModuleConfiguration == null )
                    return false;

                if ( SupportsWorkflow &&
                    ( PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedAddRoles ) ||
                     PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedDeleteRoles ) ||
                     PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedEditRoles ) ) && ( ReadyToApproveText != null ) &&
                    Version == WorkFlowVersion.Staging && WorkflowStatus == WorkflowState.Working )
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Permission for Arrow Buttons (Up/Down/Left/Right)
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanArrows
        {
            get
            {
                if ( ModuleConfiguration == null || ModuleID == 0 )
                    return false;

                if ( SupportsArrows && portalSettings.ActivePage.PageID == ModuleConfiguration.PageID &&
                    PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedMoveModuleRoles ) )
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Permission for Security Button
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanSecurity
        {
            get
            {
                if ( ModuleConfiguration == null )
                    return false;

                if ( PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedPropertiesRoles ) &&
                    portalSettings.ActivePage.PageID == ModuleConfiguration.PageID && SecurityUrl != null &&
                    SecurityUrl.Length != 0 )
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Permission for Properties Button
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool CanProperties
        {
            get
            {
                if ( ModuleConfiguration == null )
                    return false;

                if ( PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedPropertiesRoles ) &&
                    portalSettings.ActivePage.PageID == ModuleConfiguration.PageID && PropertiesUrl != null &&
                    PropertiesUrl.Length != 0 )
                    return true;
                else
                    return false;
            }
        }

        private bool _showBack = false;

        /// <summary>
        /// Can be set from module code to indicate whether module should display Back button
        /// </summary>
        public bool ShowBack
        {
            get { return _showBack; }
            set { _showBack = value; }
        }

        #endregion

        #region Workflow

        private string productionVersionText = "SWI_SWAPTOPRODUCTION";

        /// <summary>
        /// Text for version Link for swapping to production version
        /// </summary>
        public string ProductionVersionText
        {
            get { return productionVersionText; }
            set { productionVersionText = value; }
        }

        private string stagingVersionText = "SWI_SWAPTOSTAGING";

        /// <summary>
        /// Text for version Link for swapping to staging version
        /// </summary>
        public string StagingVersionText
        {
            get { return stagingVersionText; }
            set { stagingVersionText = value; }
        }

        private string publishText = "SWI_PUBLISH";

        /// <summary>
        /// Text for publish link
        /// </summary>
        public string PublishText
        {
            get { return publishText; }
            set { publishText = value; }
        }

        private string revertText = "SWI_REVERT";

        /// <summary>
        /// 
        /// </summary>
        public string RevertText
        {
            get { return revertText; }
            set { revertText = value; }
        }

        private string readyToApproveText = "SWI_READYTOAPPROVE";

        /// <summary>
        /// Text for request approval link
        /// </summary>
        public string ReadyToApproveText
        {
            get { return readyToApproveText; }
            set { readyToApproveText = value; }
        }

        private string approveText = "SWI_APPROVE";

        /// <summary>
        /// Text for approve link
        /// </summary>
        public string ApproveText
        {
            get { return approveText; }
            set { approveText = value; }
        }

        private string rejectText = "SWI_REJECT";

        /// <summary>
        /// Text for reject link
        /// </summary>
        public string RejectText
        {
            get { return rejectText; }
            set { rejectText = value; }
        }

        /// <summary>
        /// Publish staging to production
        /// </summary>
        protected virtual void Publish()
        {
            // Publish module
            WorkFlowDB.Publish( ModuleConfiguration.ModuleID );

            // Show the prod version
            Version = WorkFlowVersion.Production;
        }


        // Change by Geert.Audenaert@Syntegra.Com
        // Date: 27/2/2003
        /// <summary>
        /// This property indicates the staging content state
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public WorkflowState WorkflowStatus
        {
            get
            {
                if ( SupportsWorkflow )
                    return _moduleConfiguration.WorkflowStatus;
                else
                    return WorkflowState.Original;
            }
        }

        // End Change Geert.Audenaert@Syntegra.Com


        // Change by Geert.Audenaert@Syntegra.Com
        // Date: 6/2/2003
        /// <summary>
        /// This property indicates which content will be shown to the user
        /// production content or staging content
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public WorkFlowVersion Version
        {
            get
            {
                if ( !SupportsWorkflow )
                    return WorkFlowVersion.Staging;
                else
                    return _version;
            }
            set
            {
                if ( value == WorkFlowVersion.Staging )
                {
                    if (
                        !
                        ( PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedAddRoles ) ||
                         PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedDeleteRoles ) ||
                         PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedEditRoles ) ||
                         PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedPublishingRoles ) ||
                         PortalSecurity.IsInRoles( ModuleConfiguration.AuthorizedApproveRoles ) ) )
                        PortalSecurity.AccessDeniedEdit();
                }
                _version = value;
            }
        }

        /// <summary>
        /// Event handler for Workflow "revert"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RevertToProductionContent( object sender, EventArgs e )
        {
            // Revert
            WorkFlowDB.Revert( ModuleID );
            // Refresh current screen
            string querystring = "?";

            // Modified by Hongwei Shen(hongwei.shen@gmail.com) 8/9/2005
            // the key-value pairs are not separated by '&'.
            /* original code
 			foreach (string key in Page.Request.QueryString.Keys)
				querystring += key + "=" + Context.Server.UrlEncode(Page.Request.QueryString[key]);
            */
            // start of modification
            int i = 0;
            int totalKeys = Page.Request.QueryString.Keys.Count;
            foreach ( string key in Page.Request.QueryString.Keys )
            {
                querystring += key + "=" + Context.Server.UrlEncode( Page.Request.QueryString[ key ] );
                if ( i < totalKeys - 1 )
                    querystring += '&';
                i++;
            }
            // the call to stored-procedure rb_revert will reset
            // the WorkflowStatus to 0 (original) and we also need
            // to synchronize the module configuration to remove the
            // ReadyToApprove and Revert buttons.
            _moduleConfiguration.WorkflowStatus = 0;
            // end of modification			

            Context.Server.Transfer( Page.Request.Path + querystring );
        }

        /// <summary>
        /// This function constructs the NavigateUrl for the SwapVersions hyperlink
        /// </summary>
        /// <returns>string</returns>
        private string GetOtherVersionUrl()
        {
            string url = Page.Request.Path;
            string querystring;
            ArrayList qs = new ArrayList();

            foreach ( string var in Page.Request.QueryString.Keys )
            {
                //Added null check by manu
                if ( var != null && !( var.StartsWith( "wversion" ) || var.StartsWith( "wpublish" ) ) )
                {
                    qs.Add( var + "=" + Page.Server.UrlEncode( Page.Request.QueryString[ var ] ) );
                }
            }

            qs.Add( "wversion" + ModuleConfiguration.ModuleID.ToString() + "=" +
                   ( Version == WorkFlowVersion.Production
                        ? WorkFlowVersion.Staging.ToString()
                        : WorkFlowVersion.Production.ToString() ) );
            querystring = string.Join( "&", ( string[] )qs.ToArray( typeof( string ) ) );
            if ( querystring.Length != 0 )
                url += "?" + querystring;
            return url;
        }

        /// <summary>
        /// This function constructs the NavigateUrl for the Publish hyperlink
        /// </summary>
        /// <returns>string</returns>
        private string GetPublishUrl()
        {
            string url = Page.Request.Path;
            string querystring;
            ArrayList qs = new ArrayList();
            foreach ( string var in Page.Request.QueryString.Keys )
            {
                //Added null check by manu
                if ( var != null && !( var.StartsWith( "wversion" ) || var.StartsWith( "wpublish" ) ) )
                {
                    qs.Add( var + "=" + Page.Server.UrlEncode( Page.Request.QueryString[ var ] ) );
                }
            }
            // modified by Hongwei Shen (hongwei.shen@gmail.com) 8/9/2005
            // qs.Add("wpublish" + this.ModuleConfiguration.ModuleID.ToString() + "=doit"); 
            // end of modification
            querystring = string.Join( "&", ( string[] )qs.ToArray( typeof( string ) ) );
            if ( querystring.Length != 0 )
                url += "?" + querystring;
            return url;
        }

        #endregion

        #region publish button click event handler

        // added by Hongwei Shen to handle the publish button click
        // server event (hongwei.shen@gmail.com) 8/9/2005
        private void publishButton_ServerClick( Object sender, EventArgs e )
        {
            Publish();
            // redirect to the same page to pick up changes
            Page.Response.Redirect( GetPublishUrl() );
        }

        // end of addition

        #endregion

        #region Search

        /// <summary>
        /// Searchable module implementation
        /// </summary>
        /// <param name="portalID">The portal ID</param>
        /// <param name="userID">ID of the user is searching</param>
        /// <param name="searchString">The text to search</param>
        /// <param name="searchField">The fields where perfoming the search</param>
        /// <returns>The SELECT sql to perform a search on the current module</returns>
        public virtual string SearchSqlSelect( int portalID, int userID, string searchString, string searchField )
        {
            return string.Empty;
        }

        #endregion

        #region LastModified

        /// <summary>
        /// Returns the "Last Modified" string, or an empty string if option is not active.
        /// </summary>
        /// <returns></returns>
        public string GetLastModified()
        {
            // CHANGE by david.verberckmoes@syntegra.com on june, 2 2003
            if ( bool.Parse( ( ( SettingItem )portalSettings.CustomSettings[ "SITESETTINGS_SHOW_MODIFIED_BY" ] ).Value ) &&
                bool.Parse( ( ( SettingItem )Settings[ "MODULESETTINGS_SHOW_MODIFIED_BY" ] ).Value ) )
            {
                // Get stuff from database
                string Email = string.Empty;
                DateTime TimeStamp = DateTime.MinValue;
                WorkFlowDB.GetLastModified( ModuleID, Version, ref Email, ref TimeStamp );

                // Do some checking
                if ( Email == string.Empty )
                    return string.Empty;

                // Check if email address is valid
                EmailAddressList eal = new EmailAddressList();
                try
                {
                    eal.Add( Email );
                    Email = "<a href=\"mailto:" + Email + "\">" + Email + "</a>";
                }
                catch
                {
                }

                // Construct the rest of the html
                return
                    "<span class=\"LastModified\">" + General.GetString( "LMB_LAST_MODIFIED_BY" ) + "&#160;" + Email +
                    "&#160;" + General.GetString( "LMB_ON" ) + "&#160;" + TimeStamp.ToLongDateString() + " " +
                    TimeStamp.ToShortTimeString() + "</span>";
            }
            else
            {
                return string.Empty;
            }
            // END CHANGE by david.verberckmoes@syntegra.com on june, 2 2003
        }

        #endregion

        #region Arrow button functions

        /// <summary>
        /// function for module moving
        /// </summary>
        /// <param name="list"></param>
        private void OrderModules( ArrayList list )
        {
            int i = 1;

            // sort the arraylist
            list.Sort();

            // renumber the order
            foreach ( ModuleItem m in list )
            {
                // number the items 1, 3, 5, etc. to provide an empty order
                // number when moving items up and down in the list.
                m.Order = i;
                i += 2;
            }
        }

        /// <summary>
        /// The GetModules helper method is used to get the modules
        /// for a single pane within the tab
        /// </summary>
        /// <param name="pane"></param>
        /// <returns></returns>
        private ArrayList GetModules( string pane )
        {
            ArrayList paneModules = new ArrayList();

            // get the portal setting at the Tab level and not from this class as it is not refreshed
            foreach ( ModuleSettings _module in ( ( Page )Page ).portalSettings.ActivePage.Modules )
            {
                if ( portalSettings.ActivePage.PageID == _module.PageID && _module.PaneName.ToLower() == pane.ToLower() )
                {
                    ModuleItem m = new ModuleItem();
                    m.Title = _module.ModuleTitle;
                    m.ID = _module.ModuleID;
                    m.ModuleDefID = _module.ModuleDefID;
                    m.Order = _module.ModuleOrder;
                    m.PaneName = _module.PaneName; // tiptopweb
                    paneModules.Add( m );
                }
            }

            return paneModules;
        }

        /// <summary>
        /// function for module moving
        /// </summary>
        /// <param name="url"></param>
        /// <param name="moduleID"></param>
        /// <returns></returns>
        private string AppendModuleID( string url, int moduleID )
        {
            // tiptopweb, sometimes the home page does not have parameters 
            // so we test for both & and ?

            int selectedModIDPos = url.IndexOf( "&selectedmodid" );
            int selectedModIDPos2 = url.IndexOf( "?selectedmodid" );
            if ( selectedModIDPos >= 0 )
            {
                int selectedModIDEndPos = url.IndexOf( "&", selectedModIDPos + 1 );
                if ( selectedModIDEndPos >= 0 )
                {
                    return
                        url.Substring( 0, selectedModIDPos ) + "&selectedmodid=" + moduleID +
                        url.Substring( selectedModIDEndPos );
                }
                else
                {
                    return url.Substring( 0, selectedModIDPos ) + "&selectedmodid=" + moduleID;
                }
            }
            else if ( selectedModIDPos2 >= 0 )
            {
                int selectedModIDEndPos2 = url.IndexOf( "?", selectedModIDPos2 + 1 );
                if ( selectedModIDEndPos2 >= 0 )
                {
                    return
                        url.Substring( 0, selectedModIDPos2 ) + "?selectedmodid=" + moduleID +
                        url.Substring( selectedModIDEndPos2 );
                }
                else
                {
                    return url.Substring( 0, selectedModIDPos2 ) + "?selectedmodid=" + moduleID;
                }
            }
            else
            {
                if ( url.IndexOf( "?" ) >= 0 )
                {
                    return url + "&selectedmodid=" + moduleID;
                }
                else
                {
                    return url + "?selectedmodid=" + moduleID;
                }
            }
        }

        /// <summary>
        /// The RightLeft_Click server event handler on this page is
        /// used to move a portal module between layout panes on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightLeft_Click( Object sender, EventArgs e )
        {
            string sourcePane = ( ( ModuleButton )sender ).Attributes[ "sourcepane" ];
            string targetPane = ( ( ModuleButton )sender ).Attributes[ "targetpane" ];

            // get source arraylist
            ArrayList sourceList = GetModules( sourcePane );

            // add it to the database
            // tiptopweb : OriginalModuleID to have it work with shortcut module
            ModulesDB admin = new ModulesDB();
            admin.UpdateModuleOrder( OriginalModuleID, 99, targetPane );

            // reload the portalSettings from the database
            HttpContext.Current.Items[ "PortalSettings" ] = new PortalSettings( PageID, portalSettings.PortalAlias );
            ( ( Page )Page ).portalSettings = ( PortalSettings )Context.Items[ "PortalSettings" ];

            // reorder the modules in the source pane
            sourceList = GetModules( sourcePane );
            OrderModules( sourceList );

            // resave the order
            foreach ( ModuleItem item in sourceList )
                admin.UpdateModuleOrder( item.ID, item.Order, sourcePane );

            // reorder the modules in the target pane
            ArrayList targetList = GetModules( targetPane );
            OrderModules( targetList );

            // resave the order
            foreach ( ModuleItem item in targetList )
                admin.UpdateModuleOrder( item.ID, item.Order, targetPane );

            // Redirect to the same page to pick up changes
            Page.Response.Redirect( AppendModuleID( Page.Request.RawUrl, ModuleID ) );
        }

        /// <summary>
        /// The UpDown_Click server event handler on this page is
        /// used to move a portal module up or down on a tab's layout pane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpDown_Click( Object sender, EventArgs e )
        {
            int delta;

            //			string cmd = ((ModuleButton)sender).CommandName;
            //			string pane = ((ModuleButton)sender).CommandArgument;
            string cmd = ( ( ModuleButton )sender ).Attributes[ "direction" ];
            string pane = ( ( ModuleButton )sender ).Attributes[ "pane" ];


            ArrayList modules = GetModules( pane );

            // Determine the delta to apply in the order number for the module
            // within the list.  +3 moves down one item; -3 moves up one item
            if ( cmd == "down" )
                delta = 3;
            else
                delta = -3;

            foreach ( ModuleItem item in modules )
            {
                if ( item.ID == ModuleID )
                    item.Order += delta;
            }

            // reorder the modules in the content pane
            OrderModules( modules );

            // resave the order
            ModulesDB admin = new ModulesDB();
            foreach ( ModuleItem item in modules )
                admin.UpdateModuleOrder( item.ID, item.Order, pane );

            // Redirect to the same page to pick up changes
            Page.Response.Redirect( AppendModuleID( Page.Request.RawUrl, ModuleID ) );
        }

        // Nicholas Smeaton (24/07/2004) - Arrow button functions END

        #endregion

        #region Window Management functions

        // Added  - BJA [wjanderson@reedtek.com] [START]
        /// <summary>
        /// Set the close button attributes to prompt user before removing. 
        /// </summary>
        private void setDeleteAttributes( ref LinkButton delBtn )
        {
            // make sure javascript is valid and we have not already
            // added the function
            if ( Page.Request.Browser.EcmaScriptVersion.Major >= 1 &&
                !( Page.ClientScript.IsClientScriptBlockRegistered( "confirmDelete" ) ) )
            {
                string[] s = { "CONFIRM_DELETE" };
                Page.ClientScript.RegisterClientScriptBlock( GetType(), "confirmDelete",
                                                            PortalSettings.GetStringResource(
                                                                "CONFIRM_DELETE_SCRIPT",
                                                                s ) );
            }

            if ( delBtn.Attributes[ "onclick" ] != null )
                delBtn.Attributes[ "onclick" ] = "return confirmDelete();" + delBtn.Attributes[ "onclick" ];
            else
                delBtn.Attributes.Add( "onclick", "return confirmDelete();" );
        } // end of setDeleteAttributes
        // Added - BJA [wjanderson@reedtek.com] [END]

        #endregion

        #region Delete Module functions

        /// <summary>
        /// The DeleteModuleButton_Click server event handler on this page is
        /// used to delete a portal module
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteModuleButton_Click( Object sender, EventArgs e )
        {
            ModulesDB admin = new ModulesDB();

            //admin.DeleteModule(this.ModuleID);
            // TODO - add userEmail and useRecycler
            admin.DeleteModule( ModuleID );
            // Redirect to the same page to pick up changes
            Page.Response.Redirect( Page.Request.RawUrl );
        }

        #endregion

        #region Culture

        /// <summary>
        /// The module culture. If specified module should be showed
        /// only if current culture matches this setting.
        /// Colon separated list
        /// </summary>
        public virtual string Cultures
        {
            get
            {
                if ( HttpContext.Current != null )
                    return Settings[ "MODULESETTINGS_CULTURE" ].ToString();
                else
                    return Thread.CurrentThread.CurrentUICulture.Name;
            }
            set
            {
                if ( HttpContext.Current != null )
                    Settings[ "MODULESETTINGS_CULTURE" ] = value;
            }
        }

        #endregion

        #region Content

        // Jes1111
        private object _content = null;

        /// <summary>
        /// Will return module content as an object, if called.
        /// </summary>
        public object Content
        {
            get
            {
                if ( HttpContext.Current != null )
                {
                    if ( _content != null )
                    {
                        return _content;
                    }
                    else
                    {
                        try
                        {
                            _content = GetContent();
                            return _content;
                        }
                        catch
                        {
                            return "error"; // TODO: change this
                        }
                    }
                }
                else
                {
                    return "Module Content PlaceHolder";
                }
            }
            set { _content = value; }
        }

        /// <summary>
        /// Used by Content to fetch module content, by raising Init and Load events on the module.
        /// </summary>
        /// <returns></returns>
        public virtual object GetContent()
        {
            OnInit( null );
            OnLoad( null );
            return Content;
        }

        #endregion

        #region Module Content Sizing

        // Added by gman3001: 2004/10/26 to support specific module content sizing and scrolling capabilities
        /// <summary>
        /// Returns a module content sizing container tag with properties
        /// </summary>
        /// <paramref name="isBeginTag">Specifies whether to output the container's begin(true) or end(false) tag.</paramref>
        /// <returns>The literal control containing this tag</returns>
        private LiteralControl BuildModuleContentContainer( bool isBeginTag )
        {
            LiteralControl modContainer = new LiteralControl();
            int width = ( Settings[ "MODULESETTINGS_CONTENT_WIDTH" ] != null )
                            ? Int32.Parse( Settings[ "MODULESETTINGS_CONTENT_WIDTH" ].ToString() )
                            : 0;
            int height = ( Settings[ "MODULESETTINGS_CONTENT_HEIGHT" ] != null )
                             ? Int32.Parse( Settings[ "MODULESETTINGS_CONTENT_HEIGHT" ].ToString() )
                             : 0;
            bool scrolling = ( Settings[ "MODULESETTINGS_CONTENT_SCROLLING" ] != null )
                                 ? bool.Parse( Settings[ "MODULESETTINGS_CONTENT_SCROLLING" ].ToString() )
                                 : false;
            if ( isBeginTag )
            {
                string StartContentSizing = "<div class='modulePadding moduleScrollBars' id='modcont_" + ClientID + "' ";
                StartContentSizing += " style='POSITION: static; ";
                if ( !_isPrint && width > 0 )
                    StartContentSizing += "width: " + width.ToString() + "px; ";
                if ( !_isPrint && height > 0 )
                    StartContentSizing += "height: " + height.ToString() + "px; ";
                if ( !_isPrint && scrolling )
                    StartContentSizing += "overflow:auto;";
                StartContentSizing += "'>";
                modContainer.Text = StartContentSizing;
            }
            else
            {
                if ( Page.Request.Browser.EcmaScriptVersion.Major >= 1 && !_isPrint &&
                    ( width > 0 || height > 0 || ( width > 0 && scrolling ) || ( height > 0 && scrolling ) ) )
                {
                    // Register a client side script that will properly resize the content area of the module
                    // to compensate for different height and width settings, as well as, the browser's tendency
                    // to stretch the middle module width even when a specific width setting is specified.
                    if ( !Page.ClientScript.IsClientScriptBlockRegistered( "autoSizeModules" ) )
                    {
                        string src = Path.ApplicationRootPath( "aspnet_client/Appleseed_scripts/autoResizeModule.js" );
                        Page.ClientScript.RegisterClientScriptBlock( GetType(), "autoSizeModules",
                                                                    "<script language=javascript src='" + src +
                                                                    "'></script>" );
                        Page.ClientScript.RegisterStartupScript( GetType(), "initAutoSizeModules",
                                                                "<script defer language=javascript>if (document._portalmodules) document._portalmodules.GetModules(_portalModules); document._portalmodules.ProcessAll();</script>" );
                    }
                    Page.ClientScript.RegisterArrayDeclaration( "_portalModules", "'modcont_" + ClientID + "'" );
                }
                modContainer.Text = "</div>\r";
            }

            return modContainer;
        }

        // Added by gman3001: 2004/10/26 to support module width stretching/shrinking capability
        /// <summary>
        /// Updates the moduleControl literal control with proper width settings to render the 'module width stretching' option
        /// </summary>
        /// <paramref name="moduleControl">The literal control element to parse and modify.</paramref>
        /// <paramref name="isBeginTag">Specifies whether the moduleElement parameter is for the element's begin(true) or end(false) tag.</paramref>
        /// <returns></returns>
        private void ProcessModuleStrecthing( Control moduleControl, bool isBeginTag )
        {
            if ( moduleControl is LiteralControl && moduleControl != null )
            {
                LiteralControl moduleElement = ( LiteralControl )moduleControl;
                bool isStretched = ( Settings[ "MODULESETTINGS_WIDTH_STRETCHING" ] != null &&
                                    bool.Parse( Settings[ "MODULESETTINGS_WIDTH_STRETCHING" ].ToString() ) == true );
                string tmp = ( moduleElement.Text != null ) ? moduleElement.Text.Trim() : string.Empty;
                //Need to remove the current width setting of the main table in the module Start(Title/NoTitle)Content section of the theme,
                //if this is to be a stretched module then insert a width attribute into it,
                //if not, then surround this table with another table that has an empty cell after the cell that contains the module's HTML,
                //in order to use up any space in the window that the module has not been defined for.
                //if, no width specified for module then the module will be at least 50% width of area remaining, or expand to hold its contents.
                if ( isBeginTag )
                {
                    MatchCollection mc;
                    Regex r = new Regex( "<table[^>]*>" );
                    mc = r.Matches( tmp.ToLower() );
                    //Only concerned with first match
                    if ( mc.Count > 0 )
                    {
                        string TMatch = mc[ 0 ].Value;
                        int TIndx = mc[ 0 ].Index;

                        // jminond - variable not in use
                        //int TLength = mc[0].Value.Length;
                        //find a width attribute in this match(if exists remove it)
                        Regex r1 = new Regex( "width=((['\"][^'\"]*['\"])|([0-9]+))" );
                        mc = r1.Matches( TMatch );
                        if ( mc.Count > 0 )
                        {
                            int WIndx = mc[ 0 ].Index;
                            int WLength = mc[ 0 ].Value.Length;
                            tmp = tmp.Substring( 0, WIndx + TIndx ) + tmp.Substring( WIndx + TIndx + WLength );
                            TMatch = TMatch.Substring( 0, WIndx ) + TMatch.Substring( WIndx + WLength );
                        }
                        //find a style attribute in this match(if exists)
                        Regex r2 = new Regex( "style=['\"][^'\"]*['\"]" );
                        mc = r2.Matches( TMatch );
                        if ( mc.Count > 0 )
                        {
                            int SIndx = mc[ 0 ].Index;
                            // jminond- variable not in use
                            //int SLength = mc[0].Value.Length;

                            //Next find a width style property(if exists) and modify it
                            Regex r3 = new Regex( "width:[^;'\"]+[;'\"]" );
                            mc = r3.Matches( mc[ 0 ].Value );
                            if ( mc.Count > 0 )
                            {
                                int SwIndx = mc[ 0 ].Index;
                                int SwLength = mc[ 0 ].Value.Length - 1;
                                if ( isStretched )
                                    tmp = tmp.Substring( 0, SIndx + SwIndx + TIndx ) + "width:100%" +
                                          tmp.Substring( SIndx + SwIndx + TIndx + SwLength );
                                else
                                    tmp = tmp.Substring( 0, SIndx + SwIndx + TIndx ) +
                                          tmp.Substring( SIndx + SwIndx + TIndx + SwLength );
                            }
                            //Else, Add width style property to the existing style attribute
                            else if ( isStretched )
                                tmp = tmp.Substring( 0, SIndx + TIndx + 7 ) + "width:100%;" +
                                      tmp.Substring( SIndx + TIndx + 7 );
                        }
                        //Else, Add width style property to a new style attribute
                        else if ( isStretched )
                            tmp = tmp.Substring( 0, TIndx + 7 ) + "style='width:100%' " + tmp.Substring( TIndx + 7 );
                    }

                    if ( !isStretched )
                        tmp = "<table cellpadding=0 cellspacing=0 border=0><tr>\n<td>" + tmp;
                }
                else if ( !isStretched )
                    tmp += "</td><td></td>\n</tr></table>";
                moduleElement.Text = tmp;
            }
        }

        #endregion

        #region Theme/Rendering

        /// <summary>
        /// Before apply theme DesktopModule designer checks this
        /// property to be true.<br/>
        /// On inherited modules you can override this
        /// property to not apply theme on specific controls.
        /// </summary>
        /// <value><c>true</c> if [apply theme]; otherwise, <c>false</c>.</value>
        public virtual bool ApplyTheme
        {
            get
            {
                if ( HttpContext.Current != null ) // if it is not design time
                {
                    return ( bool.Parse( Settings[ "MODULESETTINGS_APPLY_THEME" ].ToString() ) );
                }
                return true;
            }
            set
            {
                if ( HttpContext.Current != null ) // if it is not design time
                {
                    Settings[ "MODULESETTINGS_APPLY_THEME" ] = value.ToString();
                }
            }
        }

        //Added by james for localization purpose
        /// <summary>
        /// Localises Theme types: 'Default' and 'Alt'
        /// </summary>
        public enum ThemeList : int
        {
            /// <summary>
            /// 
            /// </summary>
            Default = 0,
            /// <summary>
            /// 
            /// </summary>
            Alt = 1
        }

        /// <summary>
        /// used to hold the consolidated list of buttons for the module
        /// </summary>
        private ArrayList ButtonList = new ArrayList( 3 );

        /// <summary>
        /// User Buttons
        /// </summary>
        public ArrayList ButtonListUser = new ArrayList( 3 );

        /// <summary>
        /// Admin Buttons
        /// </summary>
        public ArrayList ButtonListAdmin = new ArrayList( 3 );

        /// <summary>
        /// Custom Buttons
        /// </summary>
        public ArrayList ButtonListCustom = new ArrayList( 3 );

        // switches used for building module hierarchy
        private bool _buildTitle = true;
        private bool _buildButtons = true;
       
        private bool _buildBody = true;
        private bool _beforeContent = true;
        private bool _isPrint = false;

        /// <summary>
        /// Makes the decisions about what needs to be built and calls the appropriate method
        /// </summary>
        protected virtual void BuildControlHierarchy()
        {
            if ( NamingContainer.ToString().EndsWith( "ASP.print_aspx" ) )
            {
                _isPrint = true;
                _buildButtons = false;
                if ( !ShowTitlePrint )
                    _buildTitle = false;
            }
            //else if ( SupportCollapsable && UserDesktop.isMinimized( ModuleID ) ) {
            //    _buildBody = false;
            //}
            else if ( !ShowTitle )
            {
                _buildTitle = false;
            }

            // added Jes1111: https://sourceforge.net/tracker/index.php?func=detail&aid=1034935&group_id=66837&atid=515929
            if ( ButtonList.Count == 0 )
                _buildButtons = false;

            // changed Jes1111 - 2004-09-29 - to correct shortcut behaviour
            if ( ModuleID != OriginalModuleID )
                BuildShortcut();
            else if ( !ApplyTheme || _isPrint )
                BuildNoTheme();
            else if ( CurrentTheme.Type.Equals( "zen" ) )
                ZenBuild();
            else if ( this.CurrentTheme.Type.Equals( "htm" ) )
                HtmBuild();
            else
                Build();

            // wrap the module in a &lt;div&gt; with the ModuleID and Module type name - needed for Zen support and useful for CSS styling and debugging ;-)
            // Added generic classname ModuleWrap useful for more generic CSS styling - Rob Siera 2004-12-30
            _header.Controls.AddAt( 0,
                                   new LiteralControl(
                                       string.Format( "<div id=\"mID{0}\" class=\"{1} ModuleWrap\">", ModuleID.ToString(),
                                                     GetType().Name ) ) );
            _footer.Controls.Add( new LiteralControl( "</div>" ) );



        }

        /// <summary>
        /// Builds the "with theme" versions of the module using html, with optional Title, Buttons and Body.
        /// </summary>
        protected virtual void HtmBuild() {

            string template = CurrentTheme.GetThemePart( "ModuleLayout" );

            #region Replace PlaceHolders

            template = template.Replace("{Title}", TitleText);

            if ( ShowTitle ) {
                template = template.Replace( "{TitleRowStyle}", "display:inline" );
            }
            else {
                template = template.Replace( "{TitleRowStyle}", "display:none" );
            }

            template = template.Replace( "{BodyBgColor}", CurrentTheme.GetThemePart( "DefaultBodyBgColor" ) );
            template = template.Replace( "{TitleBgColor}", CurrentTheme.GetThemePart( "DefaultTitleBgColor" ) );

            #endregion

            int iCtr = template.IndexOf( "{ControlPanel}" );
            int iBdy = template.IndexOf( "{Body}" );
            int iMby = template.IndexOf( "{ModifiedBy}" );


            if ( iCtr < iBdy ) {
                if ( iCtr != -1 ) {
                    // Both Ctrl & Body : ....Ctrl....Body.....
                    this._header.Controls.Add( new LiteralControl( template.Substring( 0, iCtr ) ) );
                    HtmRenderButtons( this._header );
                    this._header.Controls.Add( new LiteralControl( template.Substring( iCtr + 14, iBdy - ( iCtr + 14 ) ) ) );
                    //base.Render(output);
                    this._footer.Controls.Add( new LiteralControl( template.Substring( iBdy + 6 ) ) );
                }
                else {
                    if ( iBdy != -1 ) {
                        // Only Body: ...Body...
                        this._header.Controls.Add( new LiteralControl( template.Substring( 0, iBdy ) ) );
                        //base.Render(output);
                        this._footer.Controls.Add( new LiteralControl( template.Substring( iBdy + 6 ) ) );
                    }
                    else {
                        // No Ctrl No Body...
                        //base.Render(output);
                    }
                }
            }
            else {
                if ( iBdy != -1 ) {
                    // Both Ctrl & Body : ....Body....Ctrl.....
                    this._header.Controls.Add( new LiteralControl( template.Substring( 0, iBdy ) ) );
                    //base.Render(output);
                    this._footer.Controls.Add( new LiteralControl( template.Substring( iBdy + 6, iCtr - ( iBdy + 6 ) ) ) );
                    HtmRenderButtons( this._footer );
                    this._footer.Controls.Add( new LiteralControl( template.Substring( iCtr + 14 ) ) );
                }
                else {
                    if ( iCtr != -1 ) {
                        // Only Ctrl: ...Ctrl...
                        this._header.Controls.Add( new LiteralControl( template.Substring( 0, iCtr ) ) );
                        HtmRenderButtons( this._header );
                        this._footer.Controls.Add( new LiteralControl( template.Substring( iCtr + 14 ) ) );
                    }
                    else {
                        // No Ctrl No Body...
                        //base.Render(output);
                    }
                }
            }

        }

        private void HtmRenderButtons( PlaceHolder placeHolder )
        {
            if (ShareModule)
            {
                var publisherkeysetting = portalSettings.CustomSettings["SITESETTINGS_ADDTHIS_USERNAME"];
                  if (publisherkeysetting != null)
                  {
                      if (Convert.ToString(publisherkeysetting).Trim().Length > 0)
                      {
                          var culture = Thread.CurrentThread.CurrentUICulture.Name;
                          StringBuilder sb = new StringBuilder();
                          sb.Append("<script type=\"text/javascript\">var addthis_config = {\"data_track_clickback\":true, ");
                          sb.AppendFormat("ui_language:\"{0}\"", culture);
                          sb.Append("};</script>");
                          sb.Append("<div class=\"addthis_toolbox addthis_default_style\">");
                          sb.AppendFormat(" <a href=\"http://www.addthis.com/bookmark.php?v=250&amp;username={0}\"  class=\"addthis_button_compact\">{1}</a>", publisherkeysetting, General.GetString("SHARE", "Share"));
                          sb.Append(" <span class=\"addthis_separator\">|</span>");
                          sb.Append(" <a class=\"addthis_button_facebook\"></a>");
                          sb.Append(" <a class=\"addthis_button_twitter\"></a>");
                          sb.Append(" <a class=\"addthis_button_myspace\"></a>");
                          sb.Append("</div>");

                          placeHolder.Controls.Add(new LiteralControl(sb.ToString()));

                      }
                  }            
            }

            if ( _buildButtons )
            {
                foreach ( Control _button in this.ButtonList )
                {
                    placeHolder.Controls.Add( CurrentTheme.GetLiteralControl( "TitleBeforeButton" ) );
                    placeHolder.Controls.Add( _button );
                    placeHolder.Controls.Add( CurrentTheme.GetLiteralControl( "TitleAfterButton" ) );
                }
            }
        }

        /// <summary>
        /// Builds the shortcut.
        /// </summary>
        protected virtual void BuildShortcut()
        {
            // do nothing - just passes the target contents through. The theme will be applied
            // to the containing shortcut module.
        }

        /// <summary>
        /// Method builds "no theme" version of module. Now obeys ShowTitle and GetLastModified.
        /// </summary>
        protected virtual void BuildNoTheme()
        {
            Table t = new Table();
            t.Attributes.Add( "width", "100%" );
            t.CssClass = "TitleNoTheme";
            TableRow tr = new TableRow();
            t.Controls.Add( tr );
            TableCell tc;

            if ( _buildTitle )
            {
                tc = new TableCell();
                tc.Attributes.Add( "width", "100%" );
                tc.CssClass = "TitleNoTheme";
                tc.Controls.Add( new LiteralControl( TitleText ) );
                tr.Controls.Add( tc );
            }

            if ( _buildButtons )
            {
                foreach ( Control _button in ButtonList )
                {
                    tc = new TableCell();
                    tc.Controls.Add( _button ); //Add Button
                    tr.Controls.Add( tc );
                }
            }

            if ( _buildTitle || _buildButtons )
                _header.Controls.Add( t );

            if ( !_buildBody )
            {
                //for ( int i = 1 ; i < this.Controls.Count - 1 ; i++ ) // Jes1111 - was missing last control
                for ( int i = 1; i < Controls.Count; i++ )
                    Controls[ i ].Visible = false;
            }
            else
            {
                _footer.Controls.Add( new LiteralControl( GetLastModified() ) );
            }
        }

        /// <summary>
        /// Builds the "with theme" versions of the module, with optional Title, Buttons and Body.
        /// </summary>
        protected virtual void Build()
        {
            if ( !_buildTitle && !_buildButtons )
                _header.Controls.Add( CurrentTheme.GetLiteralControl( "ControlNoTitleStart" ) );
            else
            {
                _header.Controls.Add( CurrentTheme.GetLiteralControl( "ControlTitleStart" ) );

                _header.Controls.Add( CurrentTheme.GetLiteralControl( "TitleStart" ) );

                if ( _buildTitle )
                    _header.Controls.Add( new LiteralControl( TitleText ) );

                _header.Controls.Add( CurrentTheme.GetLiteralControl( "TitleMiddle" ) );

                if ( _buildButtons )
                {
                    foreach ( Control _button in ButtonList )
                    {
                        _header.Controls.Add( CurrentTheme.GetLiteralControl( "TitleBeforeButton" ) );
                        _header.Controls.Add( _button );
                        _header.Controls.Add( CurrentTheme.GetLiteralControl( "TitleAfterButton" ) );
                    }
                }

                _header.Controls.Add( CurrentTheme.GetLiteralControl( "TitleEnd" ) );
            }

            if ( !_buildBody )
            {
                for ( int i = 1; i < Controls.Count; i++ )
                    Controls[ i ].Visible = false;
            }
            else
            {
                _footer.Controls.Add( new LiteralControl( GetLastModified() ) );
            }

            // Added by gman3001: 2004/10/26 to support auto width sizing and scrollable module content
            // this must be the first footer control (besides custom ones such as GetLastModified)
            if ( _buildBody )
                _footer.Controls.Add( BuildModuleContentContainer( false ) );

            // changed Jes1111: https://sourceforge.net/tracker/index.php?func=detail&aid=1034935&group_id=66837&atid=515929
            if ( !_buildTitle && !_buildButtons )
                _footer.Controls.Add( CurrentTheme.GetLiteralControl( "ControlNoTitleEnd" ) );
            else
            {
                _header.Controls.Add( CurrentTheme.GetLiteralControl( "ControlTitleBeforeControl" ) );
                //Changed Rob Siera: Incorrect positioning of ControlTitleAfterControl
                //this._footer.Controls.AddAt(0, CurrentTheme.GetLiteralControl("ControlTitleAfterControl"));
                _footer.Controls.Add( CurrentTheme.GetLiteralControl( "ControlTitleAfterControl" ) );
                _footer.Controls.Add( CurrentTheme.GetLiteralControl( "ControlTitleEnd" ) );
            }

            // Added by gman3001: 2004/10/26 to support auto width sizing and scrollable module content
            // this must be the last header control
            if ( _buildBody )
                _header.Controls.Add( BuildModuleContentContainer( true ) );

            if ( !_isPrint && _header.Controls.Count > 0 && _footer.Controls.Count > 0 )
            {
                //Process the first header control as the module's outer most begin tag element
                ProcessModuleStrecthing( _header.Controls[ 0 ], true );
                //Process the last footer control as the module's outer most end tag element
                ProcessModuleStrecthing( _footer.Controls[ _footer.Controls.Count - 1 ], false );
            }
            // End add by gman3001
        }

        /// <summary>
        /// The Zen version of Build(). Parses XML Zen Module Layout.
        /// </summary>
        protected virtual void ZenBuild()
        {
            XmlTextReader _xtr = null;
            XmlTextReader _xtr2 = null;
            NameTable _nt = new NameTable();
            XmlNamespaceManager _nsm = new XmlNamespaceManager( _nt );
            _nsm.AddNamespace( string.Empty, "http://www.w3.org/1999/xhtml" );
            _nsm.AddNamespace( "if", "urn:MarinaTeq.Appleseed.Zen.Condition" );
            _nsm.AddNamespace( "loop", "urn:Marinateq.Appleseed.Zen.Looping" );
            _nsm.AddNamespace( "content", "urn:www.Appleseedportal.net" );
            XmlParserContext _context = new XmlParserContext( _nt, _nsm, string.Empty, XmlSpace.None );
            StringBuilder _fragText;
            LiteralControl _frag;
            string _loopFrag = string.Empty;

            try
            {
                _xtr = new XmlTextReader( CurrentTheme.GetThemePart( "ModuleLayout" ), XmlNodeType.Document, _context );

                while ( _xtr.Read() )
                {
                    _frag = new LiteralControl();
                    switch ( _xtr.Prefix )
                    {
                        case "if":
                            {
                                if ( _xtr.NodeType == XmlNodeType.Element && !ZenEvaluate( _xtr.LocalName ) )
                                    _xtr.Skip();
                                break;
                            }

                        case "loop":
                            {
                                if ( _xtr.NodeType == XmlNodeType.Element )
                                {
                                    switch ( _xtr.LocalName )
                                    {
                                        case "Buttons":
                                            {
                                                //   Menu btnMenu = new Menu();
                                                // btnMenu.Orientation = Orientation.Vertical;
                                                //btnMenu.StaticDisplayLevels = 1;
                                                //btnMenu.DisappearAfter = 500;
                                                //btnMenu.DynamicHorizontalOffset = 10;

                                                /*
                                                //btnMenu.StaticMenuStyle.CssClass = "CommandButton";
                                                btnMenu.StaticMenuItemStyle.CssClass = "CommandButton";
                                                
                                                btnMenu.DynamicMenuItemStyle.CssClass = "CommandButton";
                                                //btnMenu.DynamicHoverStyle.CssClass = "CommandButton";

                                                btnMenu.DynamicSelectedStyle.CssClass = "CommandButton";
                                                */

                                                //MenuItem rootNode = new MenuItem("Menu");
                                                //rootNode.ImageUrl = CurrentTheme.GetImage("Navlink", "icon/NavLink.gif").ImageUrl;
                                                //rootNode.ToolTip = "Module Control and Options Menu";
                                                //rootNode.Selected = true;

                                                _loopFrag = _xtr.ReadInnerXml();
                                                foreach ( Control c in ButtonList )
                                                {
                                                    //  ModuleButton mb = (ModuleButton)c;
                                                    /*
                                                    MenuItem MenuItem = new MenuItem(mb.EnglishName);
                                                    if(mb.Image.ImageUrl.Length > 0)
                                                        MenuItem.ImageUrl = mb.Image.ImageUrl;
                                                    else
                                                        MenuItem.ImageUrl = CurrentTheme.GetImage("Navlink", "icon/NavLink.gif").ImageUrl;

                                                    MenuItem.NavigateUrl = mb.HRef;
                                                    
                                                    MenuItem.ToolTip = mb.Title;
                                                    
                                                    MenuItem.Target = mb.Target;

                                                    rootNode.ChildItems.Add(MenuItem);
                                                     * */

                                                    #region OldCode

                                                    /* */
                                                    _xtr2 = new XmlTextReader( _loopFrag, XmlNodeType.Document, _context );
                                                    while ( _xtr2.Read() )
                                                    {
                                                        _frag = new LiteralControl();
                                                        switch ( _xtr2.Prefix )
                                                        {
                                                            case "content":
                                                                {
                                                                    switch ( _xtr2.LocalName )
                                                                    {
                                                                        case "Button":
                                                                            //																if ( this.CurrentTheme.Name.ToLower().Equals("zen-zero") && c is ModuleButton )
                                                                            //																	((ModuleButton)c).RenderAs = ModuleButton.RenderOptions.TextOnly;
                                                                            //																if ( _beforeContent )
                                                                            _header.Controls.Add( c );
                                                                            //																else
                                                                            //																	this._footer.Controls.Add(c);
                                                                            break;
                                                                        default:
                                                                            break;
                                                                    }
                                                                    break;
                                                                }
                                                            case "":
                                                            default:
                                                                {
                                                                    if ( _xtr2.NodeType == XmlNodeType.Element )
                                                                    {
                                                                        _fragText = new StringBuilder( "<" );
                                                                        _fragText.Append( _xtr2.LocalName );
                                                                        while ( _xtr2.MoveToNextAttribute() )
                                                                        {
                                                                            if ( _xtr2.LocalName != "xmlns" )
                                                                            {
                                                                                _fragText.Append( " " );
                                                                                _fragText.Append( _xtr.LocalName );
                                                                                _fragText.Append( "=\"" );
                                                                                _fragText.Append( _xtr.Value );
                                                                                _fragText.Append( "\"" );
                                                                            }
                                                                        }
                                                                        _fragText.Append( ">" );
                                                                        _frag.Text = _fragText.ToString();
                                                                        if ( _beforeContent )
                                                                            _header.Controls.Add( _frag );
                                                                        else
                                                                            _footer.Controls.Add( _frag );
                                                                    }
                                                                    else if ( _xtr2.NodeType == XmlNodeType.EndElement )
                                                                    {
                                                                        _frag.Text =
                                                                            string.Format( "</{0}>", _xtr2.LocalName );
                                                                        if ( _beforeContent )
                                                                            _header.Controls.Add( _frag );
                                                                        else
                                                                            _footer.Controls.Add( _frag );
                                                                    }
                                                                    break;
                                                                }
                                                        } // end switch
                                                    } // end while
                                                    /* */

                                                    #endregion
                                                } // end foreach

                                                // btnMenu.Items.Add(rootNode);

                                                // this._header.Controls.Add(btnMenu);
                                                break;
                                            }
                                        default:
                                            break;
                                    } // end inner switch
                                }
                                break;
                            }

                        case "content":
                            {
                                switch ( _xtr.LocalName )
                                {
                                    case "ModuleContent":
                                        _beforeContent = false;
                                        break;
                                    case "TitleText":
                                        _frag.Text = TitleText;
                                        if ( _beforeContent )
                                            _header.Controls.Add( _frag );
                                        else
                                            _footer.Controls.Add( _frag );
                                        break;
                                    case "ModifiedBy":
                                        _frag.Text = GetLastModified();
                                        if ( _beforeContent )
                                            _header.Controls.Add( _frag );
                                        else
                                            _footer.Controls.Add( _frag );
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            }

                        case "":
                        default:
                            {
                                if ( _xtr.NodeType == XmlNodeType.Element )
                                {
                                    _fragText = new StringBuilder( "<" );
                                    _fragText.Append( _xtr.LocalName );
                                    while ( _xtr.MoveToNextAttribute() )
                                    {
                                        _fragText.Append( " " );
                                        _fragText.Append( _xtr.LocalName );
                                        _fragText.Append( "=\"" );
                                        _fragText.Append( _xtr.Value );
                                        _fragText.Append( "\"" );
                                    }
                                    _fragText.Append( ">" );
                                    _frag.Text = _fragText.ToString();
                                    if ( _beforeContent )
                                        _header.Controls.Add( _frag );
                                    else
                                        _footer.Controls.Add( _frag );
                                }
                                else if ( _xtr.NodeType == XmlNodeType.EndElement )
                                {
                                    _frag.Text = string.Format( "</{0}>", _xtr.LocalName );
                                    if ( _beforeContent )
                                        _header.Controls.Add( _frag );
                                    else
                                        _footer.Controls.Add( _frag );
                                }
                                break;
                            }
                    }
                }
            }
            catch ( Exception ex )
            {
                ErrorHandler.Publish( LogLevel.Fatal, "Fatal error in ZenBuildControlHierarchy(): " + ex.Message );
                throw new Exception( "Fatal error in ZenBuildControlHierarchy(): " + ex.Message );
            }
            finally
            {
                if ( _xtr != null )
                    _xtr.Close();
            }
        }

        /// <summary>
        /// Supports ZenBuild(), evaluates 'if' commands
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        private bool ZenEvaluate( string condition )
        {
            bool _returnVal = false;

            switch ( condition )
            {
                case "Title":
                    if ( _buildTitle )
                        _returnVal = true;
                    break;
                case "Buttons":
                    //if ( _buildButtons && ButtonList.Count > 0 )
                    if ( _buildButtons )
                        _returnVal = true;
                    break;
                case "Body":
                case "Footer":
                    if ( !_buildBody )
                    {
                        _returnVal = false;
                        //for ( int i = 1 ; i < this.Controls.Count - 1 ; i++ ) // Jes1111 - was missing last control
                        for ( int i = 1; i < Controls.Count; i++ )
                            Controls[ i ].Visible = false;
                    }
                    else
                    {
                        _returnVal = true;
                    }
                    break;
                case "ShowModifiedBy":
                    if (
                        bool.Parse( ( ( SettingItem )portalSettings.CustomSettings[ "SITESETTINGS_SHOW_MODIFIED_BY" ] ).Value ) &&
                        bool.Parse( ( ( SettingItem )Settings[ "MODULESETTINGS_SHOW_MODIFIED_BY" ] ).Value ) )
                        _returnVal = true;
                    break;
                default:
                    _returnVal = false;
                    break;
            }
            return _returnVal;
        }

        #endregion

        #region Module Methods

        /// <summary>
        /// Sets the CurrentTheme - allowing custom Theme per module
        /// </summary>
        protected virtual void SetupTheme()
        {
            // changed: Jes1111 - 2004-08-05 - supports custom theme per module
            // (better to do this in OnLoad than in RenderChildren, which is too late)
            string themeName;
            if ( Int32.Parse( Settings[ "MODULESETTINGS_THEME" ].ToString() ) == ( int )ThemeList.Alt )
                themeName = "Alt";
            else
                themeName = "Default";
            // end: Jes1111



            // added: Jes1111 - 2004-08-05 - supports custom theme per module
            if ( portalSettings.CustomSettings.ContainsKey( "SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES" ) &&
                portalSettings.CustomSettings[ "SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES" ].ToString().Length != 0 &&
                bool.Parse( portalSettings.CustomSettings[ "SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES" ].ToString() ) &&
                Settings.ContainsKey( "MODULESETTINGS_MODULE_THEME" ) &&
                Settings[ "MODULESETTINGS_MODULE_THEME" ].ToString().Trim().Length > 0 )
            {
                // substitute custom theme for this module
                ThemeManager _tm = new ThemeManager( portalSettings.PortalPath );
                _tm.Load( Settings[ "MODULESETTINGS_MODULE_THEME" ].ToString() );
                CurrentTheme = _tm.CurrentTheme;
                // get CSS file, add ModuleID to each line and add resulting string to CssImportList
                try
                {
                    CssHelper cssHelper = new CssHelper();
                    string selectorPrefix = string.Concat( "#mID", ModuleID );
                    string cssFileName = Page.Server.MapPath( CurrentTheme.CssFile );
                    ( ( Page )Page ).RegisterCssImport( ModuleID.ToString(), cssHelper.ParseCss( cssFileName, selectorPrefix ) );
                }
                catch ( Exception ex )
                {
                    ErrorHandler.Publish( LogLevel.Error,
                                         "Failed to load custom theme '" + CurrentTheme.CssFile + "' for ModuleID " +
                                         ModuleID + ". Continuing with default tab theme. Message was: " + ex.Message );
                    CurrentTheme = portalSettings.GetCurrentTheme( themeName );
                }
            }
            else
            {
                // original behaviour unchanged
                CurrentTheme = portalSettings.GetCurrentTheme( themeName );
            }
            // end change: Jes1111
        }

        /// <summary>
        /// Merges the three public button lists into one.
        /// </summary>
        protected virtual void MergeButtonLists()
        {
            if ( CurrentTheme.Type != "zen" )
            {
                string _divider;
                try
                {
                    _divider = CurrentTheme.GetHTMLPart( "ButtonGroupsDivider" );
                }
                catch
                {
                    _divider =
                        string.Concat( "<img src='", CurrentTheme.GetImage( "Spacer", "Spacer.gif" ).ImageUrl,
                                      "' class='rb_mod_title_sep'/>" );
                }

                // merge the button lists
                if ( ButtonListUser.Count > 0 && ( ButtonListCustom.Count > 0 || ButtonListAdmin.Count > 0 ) )
                    ButtonListUser.Add( new LiteralControl( _divider ) );
                if ( ButtonListCustom.Count > 0 && ButtonListAdmin.Count > 0 )
                    ButtonListCustom.Add( new LiteralControl( _divider ) );
            }
            foreach ( Control btn in ButtonListUser )
                ButtonList.Add( btn );
            foreach ( Control btn in ButtonListAdmin )
                ButtonList.Add( btn );
            foreach ( Control btn in ButtonListCustom )
                ButtonList.Add( btn );
        }

        /// <summary>
        /// Builds the three public button lists
        /// </summary>
        protected virtual void BuildButtonLists()
        {
            // user buttons
            if ( BackButton != null ) ButtonListUser.Add( BackButton );
            if ( PrintButton != null ) ButtonListUser.Add( PrintButton );
            if ( HelpButton != null ) ButtonListUser.Add( HelpButton );

            // admin buttons
            if ( AddButton != null )
                ButtonListAdmin.Add( AddButton );
            if ( EditButton != null )
                ButtonListAdmin.Add( EditButton );
            if ( DeleteModuleButton != null )
                ButtonListAdmin.Add( DeleteModuleButton );
            if ( PropertiesButton != null )
                ButtonListAdmin.Add( PropertiesButton );
            if ( SecurityButton != null )
                ButtonListAdmin.Add( SecurityButton );
            if ( VersionButton != null )
                ButtonListAdmin.Add( VersionButton );
            if ( PublishButton != null )
                ButtonListAdmin.Add( PublishButton );
            if ( ApproveButton != null )
                ButtonListAdmin.Add( ApproveButton );
            if ( RejectButton != null )
                ButtonListAdmin.Add( RejectButton );
            if ( ReadyToApproveButton != null )
                ButtonListAdmin.Add( ReadyToApproveButton );
            if ( RevertButton != null )
                ButtonListAdmin.Add( RevertButton );
            if ( UpButton != null )
                ButtonListAdmin.Add( UpButton );
            if ( DownButton != null )
                ButtonListAdmin.Add( DownButton );
            if ( LeftButton != null )
                ButtonListAdmin.Add( LeftButton );
            if ( RightButton != null )
                ButtonListAdmin.Add( RightButton );

            // custom buttons
            // recover any CustomButtons set the 'old way'
            if ( ModuleTitle != null )
                foreach ( Control c in ModuleTitle.CustomButtons )
                    ButtonListCustom.Add( c );
            //if ( MinMaxButton != null )
            //    ButtonListCustom.Add( MinMaxButton );
            //if ( CloseButton != null )
            //    ButtonListCustom.Add( CloseButton );

            // set image url for standard buttons edit & delete
            if ( DeleteBtn != null )
                DeleteBtn.ImageUrl = CurrentTheme.GetImage( "Buttons_Delete", "Delete.gif" ).ImageUrl;
            if ( EditBtn != null )
                EditBtn.ImageUrl = CurrentTheme.GetImage( "Buttons_Edit", "Edit.gif" ).ImageUrl;
        }

        /// <summary>
        /// Save module data
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public virtual DataSet SaveData( DataSet ds )
        {
            return ( ds );
        }

        /// <summary>
        /// Load Data
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public virtual DataSet LoadData( DataSet ds )
        {
            return ( ds );
        }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver"></param>
        public virtual void Install( IDictionary stateSaver )
        {
        }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver"></param>
        public virtual void Uninstall( IDictionary stateSaver )
        {
        }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver">The state saver.</param>
        public virtual void Commit( IDictionary stateSaver )
        {
        }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver">The state saver.</param>
        public virtual void Rollback( IDictionary stateSaver )
        {
        }

        #endregion
    }
}