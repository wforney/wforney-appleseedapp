// [START] Added for window mgmt. support (bja@reedtek.com)

namespace Appleseed.Framework.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml;

    using Appleseed.Framework.DataTypes;
    using Appleseed.Framework.Design;
    using Appleseed.Framework.Helpers;
    using Appleseed.Framework.Security;
    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Setup;
    using Appleseed.Framework.Site.Configuration;
    using Appleseed.Framework.Site.Data;

    using Page = Appleseed.Framework.Web.UI.Page;
    using Path = Appleseed.Framework.Settings.Path;

    /// <summary>
    /// The PortalModuleControl class defines a custom 
    ///     base class inherited by all
    ///     desktop portal modules within the Portal.<br/>
    ///     The PortalModuleControl class defines portal 
    ///     specific properties that are used by the portal framework
    ///     to correctly display portal modules.
    /// </summary>
    /// <remarks>
    /// This is the "all new" RC4 PortalModuleControl, which no longer has a separate DesktopModuleTitle.
    /// </remarks>
    [History("john.mandia@whitelightsolutions.com", "2004/09/17", "Fixed path for help system")]
    [History("Jes1111", "2003/03/05", "Added ShowTitle setting - switches Title visibility on/off")]
    [History("Jes1111", "2003/04/24", "Added PortalAlias to cachekey")]
    [History("Jes1111", "2003/04/24", "Added Cacheable property")]
    [History("bja@reedtek.com", "2003/04/26", "Added support for win. mgmt min/max/close")]
    [History("david.verberckmoes@syntegra.com", "2003/06/02", 
        "Showing LastModified date & user in a better way with themes")]
    [History("Jes1111", "2004/08/30", "All new version! No more DesktopModuleTitle.")]
    [History("Mark, John and Jose", "2004/09/08", "Corrections in constructor for detect DesignMode")]
    // history from old DesktopModuleTitle class
    [History("Nicholas Smeaton", "2004/07/24", "Added support for arrow buttons to move modules")]
    [History("jviladiu@portalServices.net", "2004/07/13", "Corrections in workflow buttons")]
    [History("gman3001", "2004/04/08", 
        "Added support for custom buttons in the title bar, and set all undefined title bar buttons to 'rb_mod_title_btn' css-class."
        )]
    [History("Pekka Ylenius", "2004/11/28", "When '?' in ulr then '&' is needed not '?'")]
    [History("Hongwei Shen", "2005/09/8", "Fix the publishing problem and RevertToProduction button problem")]
    [History("Hongwei Shen", "2005/09/12", "Fix topic setting order problem(add module specific settings group base)")]
    public class PortalModuleControl : ViewUserControl, ISearchable, IInstaller
    {
        #region Private field variables

        /// <summary>
        /// The _module configuration.
        /// </summary>
        private ModuleSettings _moduleConfiguration;

        /// <summary>
        /// The _can edit.
        /// </summary>
        private int _canEdit;

        /// <summary>
        /// The _can add.
        /// </summary>
        private int _canAdd;

        /// <summary>
        /// The _can view.
        /// </summary>
        private int _canView;

        /// <summary>
        /// The _can delete.
        /// </summary>
        private int _canDelete;

        /// <summary>
        /// The _can properties.
        /// </summary>
        private int _canProperties;

        /// <summary>
        /// The _settings.
        /// </summary>
        private Hashtable _settings;

        /// <summary>
        /// The _version.
        /// </summary>
        private WorkFlowVersion _version = WorkFlowVersion.Production;

        /// <summary>
        /// The _supports workflow.
        /// </summary>
        private bool _supportsWorkflow;

        /// <summary>
        /// The _cacheable.
        /// </summary>
        private bool _cacheable = true;

        /// <summary>
        /// The _supports print.
        /// </summary>
        private bool _supportsPrint = true;

        // private bool			_supportsHelp = false;
        /// <summary>
        /// The _supports arrows.
        /// </summary>
        private bool _supportsArrows = true;

        /// <summary>
        /// The _vcm.
        /// </summary>
        private ViewControlManager _vcm;

        /// <summary>
        /// The _header.
        /// </summary>
        private readonly PlaceHolder _header = new PlaceHolder();

        /// <summary>
        /// The _footer.
        /// </summary>
        private readonly PlaceHolder _footer = new PlaceHolder();

        /// <summary>
        /// The _header place holder.
        /// </summary>
        private readonly PlaceHolder _headerPlaceHolder = new PlaceHolder();

        // private PlaceHolder		_output = new PlaceHolder();

        // --  BJA Added Min/Max/Close Attributes [START]
        // Change wjanderson@reedtek.com
        // Date 25/4/2003 ( min/max./close buttons )
        // - Note : At some point you may wish to allow 
        // -        the selection of which buttons can
        // -        be displayed; right now it is all or nothing.
        // -        Also, you may wish to allow authorized close and min. 
        /// <summary>
        /// The _can min.
        /// </summary>
        private int _canMin;

        /// <summary>
        /// The _can close.
        /// </summary>
        private int _canClose;

        /// <summary>
        /// The _supports collapseable.
        /// </summary>
        private bool _supportsCollapseable;

        /// <summary>
        /// The current theme.
        /// </summary>
        public Theme CurrentTheme;

        #endregion

        #region Standard Controls

        /// <summary>
        ///     Standard content Delete button
        /// </summary>
        protected ImageButton DeleteBtn;

        /// <summary>
        ///     Standard content Edit button
        /// </summary>
        protected ImageButton EditBtn;

        /// <summary>
        ///     Standard content Update button
        /// </summary>
        protected LinkButton updateButton;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalModuleControl"/> class. 
        ///     Dafault contructor, initializes default settings
        /// </summary>
        public PortalModuleControl()
        {
            // MVC
            var wrapper = new HttpContextWrapper(this.Context);

            var viewContext = new ViewContext();
            viewContext.HttpContext = wrapper;
            viewContext.ViewData = new ViewDataDictionary();

            this.ViewContext = viewContext;

            // ****************//
            int _groupOrderBase;
            SettingItemGroup _Group;

            // THEME MANAGEMENT
            _Group = SettingItemGroup.THEME_LAYOUT_SETTINGS;
            _groupOrderBase = (int)SettingItemGroup.THEME_LAYOUT_SETTINGS;

            var ApplyTheme = new SettingItem(new BooleanDataType());
            ApplyTheme.Order = _groupOrderBase + 10;
            ApplyTheme.Group = _Group;
            ApplyTheme.Value = "True";
            ApplyTheme.EnglishName = "Apply Theme";
            ApplyTheme.Description = "Check this box to apply theme to this module";
            this._baseSettings.Add("MODULESETTINGS_APPLY_THEME", ApplyTheme);

            var themeOptions = new ArrayList();
            themeOptions.Add(
                new SettingOption((int)ThemeList.Default, General.GetString("MODULESETTINGS_THEME_DEFAULT")));
            themeOptions.Add(new SettingOption((int)ThemeList.Alt, General.GetString("MODULESETTINGS_THEME_ALT")));
            var Theme = new SettingItem(new CustomListDataType(themeOptions, "Name", "Val"));
            Theme.Order = _groupOrderBase + 20;
            Theme.Group = _Group;
            Theme.Value = ((int)ThemeList.Default).ToString();
            Theme.EnglishName = "Theme";
            Theme.Description = "Choose theme for this module";
            this._baseSettings.Add("MODULESETTINGS_THEME", Theme);

            if (HttpContext.Current != null)
            {
                // null in DesignMode
                // Added: Jes1111 - 2004-08-03
                PortalSettings _portalSettings;
                _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];

                // end addition: Jes1111

                if (_portalSettings != null)
                {
                    // fix by The Bitland Prince
                    this.PortalID = _portalSettings.PortalID;
                    // added: Jes1111 2004-08-02 - custom module theme
                    if (_portalSettings.CustomSettings.ContainsKey("SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES") &&
                        _portalSettings.CustomSettings["SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES"].ToString().Length != 0 &&
                        bool.Parse(_portalSettings.CustomSettings["SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES"].ToString()))
                    {
                        var _tempList = new ArrayList(new ThemeManager(_portalSettings.PortalPath).GetThemes());
                        var _themeList = new ArrayList();
                        foreach (ThemeItem _item in _tempList)
                        {
                            if (_item.Name.ToLower().StartsWith("module"))
                            {
                                _themeList.Add(_item);
                            }
                        }

                        var _noCustomTheme = new ThemeItem();
                        _noCustomTheme.Name = string.Empty;
                        _themeList.Insert(0, _noCustomTheme);
                        var ModuleTheme = new SettingItem(new CustomListDataType(_themeList, "Name", "Name"));
                        ModuleTheme.Order = _groupOrderBase + 25;
                        ModuleTheme.Group = _Group;
                        ModuleTheme.EnglishName = "Custom Theme";
                        ModuleTheme.Description = "Set a custom theme for this module only";
                        this._baseSettings.Add("MODULESETTINGS_MODULE_THEME", ModuleTheme);
                    }
                }
            }

            // switches title display on/off
            var ShowTitle = new SettingItem(new BooleanDataType());
            ShowTitle.Order = _groupOrderBase + 30;
            ShowTitle.Group = _Group;
            ShowTitle.Value = "True";
            ShowTitle.EnglishName = "Show Title";
            ShowTitle.Description = "Switches title display on/off";
            this._baseSettings.Add("MODULESETTINGS_SHOW_TITLE", ShowTitle);

            // switches last modified summary on/off
            var ShowModifiedBy = new SettingItem(new BooleanDataType());
            ShowModifiedBy.Order = _groupOrderBase + 40;
            ShowModifiedBy.Group = _Group;
            ShowModifiedBy.Value = "False";
            ShowModifiedBy.EnglishName = "Show Modified by";
            ShowModifiedBy.Description = "Switches 'Show Modified by' display on/off";
            this._baseSettings.Add("MODULESETTINGS_SHOW_MODIFIED_BY", ShowModifiedBy);

            // gman3001: added 10/26/2004
            // - implement width, height, and content scrolling options for all modules 
            // - implement auto-stretch option
            // Windows height
            var ControlHeight = new SettingItem(new IntegerDataType());
            ControlHeight.Value = "0";
            ControlHeight.MinValue = 0;
            ControlHeight.MaxValue = 3000;
            ControlHeight.Required = true;
            ControlHeight.Order = _groupOrderBase + 50;
            ControlHeight.Group = _Group;
            ControlHeight.EnglishName = "Content Height";
            ControlHeight.Description = "Minimum height(in pixels) of the content area of this module. (0 for none)";
            this._baseSettings.Add("MODULESETTINGS_CONTENT_HEIGHT", ControlHeight);

            // Windows width
            var ControlWidth = new SettingItem(new IntegerDataType());
            ControlWidth.Value = "0";
            ControlWidth.MinValue = 0;
            ControlWidth.MaxValue = 3000;
            ControlWidth.Required = true;
            ControlWidth.Order = _groupOrderBase + 60;
            ControlWidth.Group = _Group;
            ControlWidth.EnglishName = "Content Width";
            ControlWidth.Description = "Minimum width(in pixels) of the content area of this module. (0 for none)";
            this._baseSettings.Add("MODULESETTINGS_CONTENT_WIDTH", ControlWidth);

            // Content scrolling option
            var ScrollingSetting = new SettingItem(new BooleanDataType());
            ScrollingSetting.Value = "false";
            ScrollingSetting.Order = _groupOrderBase + 70;
            ScrollingSetting.Group = _Group;
            ScrollingSetting.EnglishName = "Content Scrolling";
            ScrollingSetting.Description =
                "Set to enable/disable scrolling of Content based on height and width settings.";
            this._baseSettings.Add("MODULESETTINGS_CONTENT_SCROLLING", ScrollingSetting);

            // Module Stretching option
            var StretchSetting = new SettingItem(new BooleanDataType());
            StretchSetting.Value = "true";
            StretchSetting.Order = _groupOrderBase + 80;
            StretchSetting.Group = _Group;
            StretchSetting.EnglishName = "Module Auto Stretch";
            StretchSetting.Description =
                "Set to enable/disable automatic stretching of the module's width to fill the empty area to the right of the module.";
            this._baseSettings.Add("MODULESETTINGS_WIDTH_STRETCHING", StretchSetting);

            // gman3001: END

            // BUTTONS
            _Group = SettingItemGroup.BUTTON_DISPLAY_SETTINGS;
            _groupOrderBase = (int)SettingItemGroup.BUTTON_DISPLAY_SETTINGS;

            // Show print button in view mode?
            var PrintButton = new SettingItem(new BooleanDataType());
            PrintButton.Value = "False";
            PrintButton.Order = _groupOrderBase + 20;
            PrintButton.Group = _Group;
            PrintButton.EnglishName = "Show Print Button";
            PrintButton.Description = "Show print button in view mode?";
            this._baseSettings.Add("MODULESETTINGS_SHOW_PRINT_BUTTION", PrintButton);

            // added: Jes1111 2004-08-29 - choice! Default is 'true' for backward compatibility
            // Show Title for print?
            var ShowTitlePrint = new SettingItem(new BooleanDataType());
            ShowTitlePrint.Value = "True";
            ShowTitlePrint.Order = _groupOrderBase + 25;
            ShowTitlePrint.Group = _Group;
            ShowTitlePrint.EnglishName = "Show Title for Print";
            ShowTitlePrint.Description = "Show Title for this module in print popup?";
            this._baseSettings.Add("MODULESETTINGS_SHOW_TITLE_PRINT", ShowTitlePrint);

            // added: Jes1111 2004-08-02 - choices for Button display on module
            var buttonDisplayOptions = new ArrayList();
            buttonDisplayOptions.Add(
                new SettingOption(
                    (int)ModuleButton.RenderOptions.ImageOnly, General.GetString("MODULESETTINGS_BUTTON_DISPLAY_IMAGE")));
            buttonDisplayOptions.Add(
                new SettingOption(
                    (int)ModuleButton.RenderOptions.TextOnly, General.GetString("MODULESETTINGS_BUTTON_DISPLAY_TEXT")));
            buttonDisplayOptions.Add(
                new SettingOption(
                    (int)ModuleButton.RenderOptions.ImageAndTextCSS, 
                    General.GetString("MODULESETTINGS_BUTTON_DISPLAY_BOTH")));
            buttonDisplayOptions.Add(
                new SettingOption(
                    (int)ModuleButton.RenderOptions.ImageOnlyCSS, 
                    General.GetString("MODULESETTINGS_BUTTON_DISPLAY_IMAGECSS")));
            var ButtonDisplay = new SettingItem(new CustomListDataType(buttonDisplayOptions, "Name", "Val"));
            ButtonDisplay.Order = _groupOrderBase + 30;
            ButtonDisplay.Group = _Group;
            ButtonDisplay.Value = ((int)ModuleButton.RenderOptions.ImageOnly).ToString();
            ButtonDisplay.EnglishName = "Display Buttons as:";
            ButtonDisplay.Description =
                "Choose how you want module buttons to be displayed. Note that settings other than 'Image only' may require Zen or special treatment in the Theme.";
            this._baseSettings.Add("MODULESETTINGS_BUTTON_DISPLAY", ButtonDisplay);

            // Jes1111 - not implemented yet			
            // 			// Show email button in view mode?
            // 			SettingItem EmailButton = new SettingItem(new BooleanDataType());
            // 			EmailButton.Value = "False";
            // 			EmailButton.Order = _groupOrderBase + 30;
            // 			EmailButton.Group = _Group;
            // 			this._baseSettings.Add("ShowEmailButton",EmailButton);

            // Show arrows buttons to move modules (admin only, property authorise)
            var ArrowButtons = new SettingItem(new BooleanDataType());
            ArrowButtons.Value = "True";
            ArrowButtons.Order = _groupOrderBase + 40;
            ArrowButtons.Group = _Group;
            ArrowButtons.EnglishName = "Show Arrow Admin Buttons";
            ArrowButtons.Description = "Show Arrow Admin buttons?";
            this._baseSettings.Add("MODULESETTINGS_SHOW_ARROW_BUTTONS", ArrowButtons);

            // Show help button if exists
            var HelpButton = new SettingItem(new BooleanDataType());
            HelpButton.Value = "True";
            HelpButton.Order = _groupOrderBase + 50;
            HelpButton.Group = _Group;
            HelpButton.EnglishName = "Show Help Button";
            HelpButton.Description = "Show help button in title if exists documentation for this module";
            this._baseSettings.Add("MODULESETTINGS_SHOW_HELP_BUTTON", HelpButton);

            // LANGUAGE/CULTURE MANAGEMENT
            _groupOrderBase = (int)SettingItemGroup.CULTURE_SETTINGS;
            _Group = SettingItemGroup.CULTURE_SETTINGS;

            var cultureList = Localization.LanguageSwitcher.GetLanguageList(true);

            var Culture = new SettingItem(new MultiSelectListDataType(cultureList, "DisplayName", "Name"));
            Culture.Value = string.Empty;
            Culture.Order = _groupOrderBase + 10;
            Culture.Group = _Group;
            Culture.EnglishName = "Culture";
            Culture.Description =
                "Please choose the culture. Invariant cultures shows always the module, if you choose one or more cultures only when culture is selected this module will shown.";
            this._baseSettings.Add("MODULESETTINGS_CULTURE", Culture);

            // Localized module title
            var counter = _groupOrderBase + 11;
            foreach (var c in cultureList)
            {
                // Ignore invariant
                if (c != CultureInfo.InvariantCulture && !this._baseSettings.ContainsKey(c.Name))
                {
                    var LocalizedTitle = new SettingItem(new StringDataType());
                    LocalizedTitle.Order = counter;
                    LocalizedTitle.Group = _Group;
                    LocalizedTitle.EnglishName = "Title (" + c.Name + ")";
                    LocalizedTitle.Description = "Set title for " + c.EnglishName + " culture.";
                    this._baseSettings.Add("MODULESETTINGS_TITLE_" + c.Name, LocalizedTitle);
                    counter++;
                }
            }

            // SEARCH
            if (this.Searchable)
            {
                _groupOrderBase = (int)SettingItemGroup.MODULE_SPECIAL_SETTINGS;
                _Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;

                var topicName = new SettingItem(new StringDataType());
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
                this._baseSettings.Add("TopicName", topicName);
            }

            // Default configuration
            this._tabID = 0;

            this._moduleConfiguration = new ModuleSettings();

            var Share = new SettingItem(new BooleanDataType());
            Share.Value = "False";
            Share.Order = _groupOrderBase + 51;
            Share.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            Share.EnglishName = "ShareModule";
            Share.Description = "Share Module";
            this._baseSettings.Add("SHARE_MODULE", Share);
        }

        #endregion

        #region Module Configuration

        /// <summary>
        ///     _baseSettings holds datatype information
        /// </summary>
        protected Hashtable _baseSettings = new Hashtable();

        /// <summary>
        ///     Module custom settings
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Hashtable Settings
        {
            get
            {
                if (this._settings == null)
                {
                    this._settings = ModuleSettings.GetModuleSettings(this.ModuleID, this._baseSettings);
                }

                return this._settings;
            }
        }

        /// <summary>
        ///     Module base settings defined by control creator
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Hashtable BaseSettings
        {
            get
            {
                return this._baseSettings;
            }
        }

        /// <summary>
        /// Override on derivates classes
        ///     Method to initialize custom settings values (such as lists) 
        ///     only when accessing the edition mode (and not in every class constructor)
        /// </summary>
        public virtual void InitializeCustomSettings()
        {
        }

        /// <summary>
        ///     Override on derivates classes.
        ///     Return the path of the add control if available.
        /// </summary>
        public virtual string AddModuleControl
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///     Override on derivates classes.
        ///     Return the path of the edit control if available.
        /// </summary>
        public virtual string EditModuleControl
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///     unique key for module caching
        /// </summary>
        public string ModuleCacheKey
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    // Change 8/April/2003 Jes1111
                    // changes to Language behaviour require addition of culture names to cache key
                    // Jes1111 2003/04/24 - Added PortalAlias to cachekey
                    var portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
                    var sb = new StringBuilder();
                    sb.Append("rb_");
                    sb.Append(portalSettings.PortalAlias);
                    sb.Append("_mid");
                    sb.Append(this.ModuleID.ToString());
                    sb.Append("[");
                    sb.Append(portalSettings.PortalContentLanguage);
                    sb.Append("+");
                    sb.Append(portalSettings.PortalUILanguage);
                    sb.Append("+");
                    sb.Append(portalSettings.PortalDataFormattingCulture);
                    sb.Append("]");

                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///     The current ID of the module. Is unique for all portals.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ModuleID
        {
            get
            {
                try
                {
                    return this._moduleConfiguration.ModuleID;
                }
                catch
                {
                    return -1;
                }
            }

            set
            {
                // made changeable by Manu, please be careful on changing it
                this._moduleConfiguration.ModuleID = value;
                this._settings = null; // force cached settings to be reloaded
            }
        }

        // Jes1111
        /// <summary>
        /// The _original module id.
        /// </summary>
        private int _originalModuleID = -1;

        /// <summary>
        ///     The ID of the orginal module (will be different to ModuleID when using shortcut module)
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int OriginalModuleID
        {
            get
            {
                try
                {
                    if (this._originalModuleID == -1)
                    {
                        return this.ModuleID;
                    }
                    else
                    {
                        return this._originalModuleID;
                    }
                }
                catch
                {
                    return -1;
                }
            }

            set
            {
                this._originalModuleID = value;
            }
        }

        /// <summary>
        ///     Configuration
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ModuleSettings ModuleConfiguration
        {
            get
            {
                if (HttpContext.Current != null && this._moduleConfiguration != null)
                {
                    return this._moduleConfiguration;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                this._moduleConfiguration = value;
            }
        }

        /// <summary>
        ///     GUID of module (mandatory)
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Guid GuidID
        {
            get
            {
                // 1.1.8.1324 - 24/01/2003
                throw new NotImplementedException("You must implement a unique GUID for your module");
            }
        }

        /// <summary>
        ///     ClassName (Used for Get/Save: not implemented)
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string ClassName
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles FlushCache event at Module level<br/>
        ///     Performs FlushCache actions that are common to all Pages<br/>
        ///     Can be overridden
        /// </summary>
        protected virtual void OnFlushCache()
        {
            if (this.FlushCache != null)
            {
                this.FlushCache(this, new EventArgs()); // Invokes the delegates
            }

            // remove module output from cache, if it's there
            if (HttpContext.Current != null)
            {
                this.Context.Cache.Remove(this.ModuleCacheKey);
                Debug.WriteLine("************* Remove " + this.ModuleCacheKey);
            }

            // any other code goes here
        }

        /// <summary>
        ///     The Update event is defined using the event keyword.
        ///     The type of Update is EventHandler.
        /// </summary>
        public event EventHandler Update;

        /// <summary>
        /// Handles OnUpdate event at Page level<br/>
        ///     Performs OnUpdate actions that are common to all Pages<br/>
        ///     Can be overridden
        /// </summary>
        /// <param name="e">
        /// </param>
        protected virtual void OnUpdate(EventArgs e)
        {
            if (this.Update != null)
            {
                this.Update(this, e); // Invokes the delegates
            }

            // Flush cache
            this.OnFlushCache();

            // any other code goes here
            WorkFlowDB.SetLastModified(this.ModuleID, MailHelper.GetCurrentUserEmailAddress());
        }

        /// <summary>
        /// On Delete
        /// </summary>
        protected virtual void OnDelete()
        {
            WorkFlowDB.SetLastModified(this.ModuleID, MailHelper.GetCurrentUserEmailAddress());
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
        /// The on load.
        /// </summary>
        /// <param name="e">
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            

            // First Check if the version is specified
            string version = null;
            try
            {
                version = this.Page.Request.QueryString["wversion" + this.ModuleConfiguration.ModuleID];
            }
            catch (NullReferenceException)
            {
                // string message = ex.Message;
            }

            if (version != null)
            {
                var requestedVersion = version == "Staging" ? WorkFlowVersion.Staging : WorkFlowVersion.Production;
                if (requestedVersion != this.Version)
                {
                    this.Version = requestedVersion;
                    this.OnVersionSwap();
                }
            }

            

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

            if (this.ModuleConfiguration != null)
            {
                if (this.Cacheable)
                {
                    this.ModuleConfiguration.Cacheable = true;
                }
                else
                {
                    this.ModuleConfiguration.Cacheable = false;
                }
            }

            #endregion

            this.SetupTheme();

            #region check for window management

            // bja@reedtek.com - does this configuration support window mgmt controls?
            // jes1111 - if (GlobalResources.SupportWindowMgmt && SupportCollapsable)
            if (Config.WindowMgmtControls && this.SupportCollapsable)
            {
                this._vcm = new ViewControlManager(this.PageID, this.ModuleID, HttpContext.Current.Request.RawUrl);
            }

            #endregion

            this.BuildButtonLists();

            this.MergeButtonLists();

            // Then call inherited member
            base.OnLoad(e);

            this.BuildControlHierarchy();
            this._headerPlaceHolder.Controls.Add(this._header);
            this.Controls.Add(this._footer);
        }

        /// <summary>
        /// Raises OnInit event.
        /// </summary>
        /// <param name="e">
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            this.Controls.AddAt(0, this._headerPlaceHolder);

            if (this.DeleteBtn != null)
            {
                // Assign current permissions to Delete button
                if (this.IsDeleteable == false)
                {
                    this.DeleteBtn.Visible = false;
                }
                else
                {
                    this.DeleteBtn.Visible = true;

                    if (!this.Page.ClientScript.IsClientScriptBlockRegistered("confirmDelete"))
                    {
                        string[] s = { "CONFIRM_DELETE" };
                        this.Page.ClientScript.RegisterClientScriptBlock(
                            this.GetType(), 
                            "confirmDelete", 
                            PortalSettings.GetStringResource("CONFIRM_DELETE_SCRIPT", s));
                    }

                    if (this.DeleteBtn.Attributes["onclick"] != null)
                    {
                        this.DeleteBtn.Attributes["onclick"] = "return confirmDelete();" +
                                                               this.DeleteBtn.Attributes["onclick"];
                    }
                    else
                    {
                        this.DeleteBtn.Attributes.Add("onclick", "return confirmDelete();");
                    }

                    this.DeleteBtn.Click += this.DeleteBtn_Click;
                    this.DeleteBtn.AlternateText = General.GetString("DELETE");
                    this.DeleteBtn.EnableViewState = false;
                }
            }

            if (this.EditBtn != null)
            {
                // Assign current permissions to Edit button
                if (this.IsEditable == false)
                {
                    this.EditBtn.Visible = false;
                }
                else
                {
                    this.EditBtn.Visible = true;
                    this.EditBtn.Click += this.EditBtn_Click;
                    this.EditBtn.AlternateText = General.GetString("Edit");
                    this.EditBtn.EnableViewState = false;
                }
            }

            if (this.updateButton != null)
            {
                this.updateButton.Click += this.UpdateBtn_Click;
                this.updateButton.Text = General.GetString("UPDATE");

                // updateButton.CssClass = "CommandButton"; // Jes1111 - set in .ascx
                this.updateButton.EnableViewState = false;
            }

            base.OnInit(e);
        }

        /// <summary>
        /// Update Button click
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            this.OnUpdate(e);
        }

        /// <summary>
        /// The edit btn_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void EditBtn_Click(object sender, ImageClickEventArgs e)
        {
            this.OnEdit();
        }

        /// <summary>
        /// The delete btn_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DeleteBtn_Click(object sender, ImageClickEventArgs e)
        {
            this.OnDelete();
        }

        /// <summary>
        /// On Edit
        /// </summary>
        protected virtual void OnEdit()
        {
            WorkFlowDB.SetLastModified(this.ModuleID, MailHelper.GetCurrentUserEmailAddress());
        }

        /// <summary>
        ///     The FlushCache event is defined using the event keyword.
        ///     The type of FlushCache is EventHandler.
        /// </summary>
        public event EventHandler FlushCache;

        #endregion

        #region Module Supports...

        /// <summary>
        ///     Override on derivates class.
        ///     Return true if the module is an Admin Module.
        /// </summary>
        public virtual bool AdminModule
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Override on derivates classes.
        ///     Return true if the module is Searchable.
        /// </summary>
        public virtual bool Searchable
        {
            get
            {
                return false;
            }
        }

        // Jes1111
        /// <summary>
        ///     Override on derived class.
        ///     Return true if the module is Cacheable.
        /// </summary>
        public virtual bool Cacheable
        {
            get
            {
                return this._cacheable;
            }

            set
            {
                this._cacheable = value;
            }
        }

        /// <summary>
        ///     Override on derived class.
        ///     Return true if the module supports print in pop-up window.
        /// </summary>
        public bool SupportsPrint
        {
            get
            {
                return this._supportsPrint;
            }

            set
            {
                this._supportsPrint = value;
            }
        }

        /// <summary>
        ///     This property indicates if the specified module supports can be
        ///     collpasable (minimized/maximized/closed)
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SupportCollapsable
        {
            get
            {
                if (this._moduleConfiguration == null)
                {
                    return this._supportsCollapseable;
                }
                else
                {
                    // jes1111 - return GlobalResources.SupportWindowMgmt && _moduleConfiguration.SupportCollapsable;
                    return Config.WindowMgmtControls && this._moduleConfiguration.SupportCollapsable;
                }
            }

            set
            {
                this._supportsCollapseable = value;
            }
        }

        // end of SupportCollapsable

        /// <summary>
        ///     This property indicates whether the module supports a Back button
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SupportsBack { get; set; }

        /// <summary>
        ///     This property indicates if the module supports email
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SupportsEmail { get; set; }

        /// <summary>
        ///     This property indicates if the specified module supports arrows to move modules
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SupportsArrows
        {
            get
            {
                var returnValue = this._supportsArrows;

                if (this.portalSettings.CustomSettings["SITESETTINGS_SHOW_MODULE_ARROWS"] != null)
                {
                    returnValue = returnValue &&
                                  bool.Parse(
                                      this.portalSettings.CustomSettings["SITESETTINGS_SHOW_MODULE_ARROWS"].ToString());
                }

                if (this.Settings["MODULESETTINGS_SHOW_ARROW_BUTTONS"] != null)
                {
                    returnValue = returnValue &&
                                  bool.Parse(this.Settings["MODULESETTINGS_SHOW_ARROW_BUTTONS"].ToString());
                }

                return returnValue;
            }

            set
            {
                this._supportsArrows = value;
            }
        }

        /// <summary>
        ///     This property indicates if the specified module workflow is enabled.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SupportsWorkflow
        {
            get
            {
                if (this._moduleConfiguration == null)
                {
                    return this._supportsWorkflow;
                }
                else
                {
                    return this._supportsWorkflow && this._moduleConfiguration.SupportWorkflow;
                }
            }

            set
            {
                this._supportsWorkflow = value;
            }
        }

        /// <summary>
        ///     This property indicates if the specified module supports workflow.
        ///     It returns the module property regardless of current module setting.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InnerSupportsWorkflow
        {
            // changed Jes1111 (from 'internal')
            get
            {
                return this._supportsWorkflow;
            }

            set
            {
                this._supportsWorkflow = value;
            }
        }

        /// <summary>
        ///     This property indicates if the specified module supports help
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SupportsHelp
        {
            get
            {
                if ((this.Settings["MODULESETTINGS_SHOW_HELP_BUTTON"] == null ||
                     bool.Parse(this.Settings["MODULESETTINGS_SHOW_HELP_BUTTON"].ToString())) &&
                    (this.ModuleConfiguration.DesktopSrc.Length != 0))
                {
                    var aux = Path.ApplicationRoot + "/rb_documentation/Appleseed/" +
                              this.ModuleConfiguration.DesktopSrc.Replace(".", "_");
                    return Directory.Exists(HttpContext.Current.Server.MapPath(aux));
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region Portal

        /// <summary>
        ///     Stores current portal settings
        /// </summary>
        public PortalSettings portalSettings
        {
            get
            {
                if (this.Page != null)
                {
                    return this.Page.portalSettings;
                }
                else
                {
                    // Obtain PortalSettings from Current Context
                    if (HttpContext.Current != null)
                    {
                        return (PortalSettings)HttpContext.Current.Items["PortalSettings"];
                    }

                    return null;
                }
            }
        }

        /// <summary>
        ///     ID of portal in which module is instantiated
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PortalID { get; set; }

        #endregion

        #region Page

        /// <summary>
        ///     Returns the current page
        /// </summary>
        public new Page Page
        {
            get
            {
                if (base.Page is Page)
                {
                    return (Page)base.Page;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Tab

        /// <summary>
        /// The _tab id.
        /// </summary>
        private int _tabID;

        /// <summary>
        ///     Stores current linked module ID if applicable
        /// </summary>
        public int PageID
        {
            get
            {
                if (this._tabID == 0)
                {
                    this.Trace.Warn("Request.Params['PageID'] = " + this.Request.Params["PageID"]);

                    // Determine PageID if specified
                    if (HttpContext.Current != null && this.Request.Params["PageID"] != null)
                    {
                        var pageIdString = this.Request.Params["PageID"];
                        this._tabID = Int32.Parse(pageIdString.Split(',')[0]);
                    }
                    else if (HttpContext.Current != null && this.Request.Params["TabID"] != null)
                    {
                        var pageIdString = this.Request.Params["TabID"];
                        this._tabID = Int32.Parse(pageIdString.Split(',')[0]);
                    }
                }

                return this._tabID;
            }
        }

        /// <summary>
        ///     Stores current tab settings
        /// </summary>
        public Hashtable pageSettings
        {
            get
            {
                if (this.Page != null)
                {
                    return this.Page.pageSettings;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Title

        /// <summary>
        ///     Return true if module has inner control of type title
        /// </summary>
        /// <remarks>
        ///     Left here for backward compatibility until it proves redundant
        /// </remarks>
        protected bool HasTitle
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Inner Title control. Now only used for backward compatibility
        /// </summary>
        public virtual DesktopModuleTitle ModuleTitle { get; set; }

        /// <summary>
        ///     Switch to turn on/off the display of Title text.
        /// </summary>
        /// <remarks>
        ///     Note: won't turn off the display of Buttons like it used to! You can now have buttons displayed with no title text showing
        /// </remarks>
        public virtual bool ShowTitle
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    // if it is not design time
                    return bool.Parse(this.Settings["MODULESETTINGS_SHOW_TITLE"].ToString());
                }

                return false;
            }

            set
            {
                if (HttpContext.Current != null)
                {
                    // if it is not design time
                    this.Settings["MODULESETTINGS_SHOW_TITLE"] = value.ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShareModule.
        /// </summary>
        public bool ShareModule
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    // if it is not design time
                    return bool.Parse(this.Settings["SHARE_MODULE"].ToString());
                }

                return false;
            }

            set
            {
                if (HttpContext.Current != null)
                {
                    // if it is not design time
                    this.Settings["SHARE_MODULE"] = value.ToString();
                }
            }
        }

        /// <summary>
        ///     Switch to turn on/off the display of the module title text (not the buttons) in the print pop-up.
        /// </summary>
        public virtual bool ShowTitlePrint
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    // if it is not design time
                    return bool.Parse(this.Settings["MODULESETTINGS_SHOW_TITLE_PRINT"].ToString());
                }

                return false;
            }

            set
            {
                if (HttpContext.Current != null)
                {
                    // if it is not design time
                    this.Settings["MODULESETTINGS_SHOW_TITLE_PRINT"] = value.ToString();
                }
            }
        }

        /// <summary>
        /// The title text.
        /// </summary>
        private string titleText = string.Empty;

        /// <summary>
        ///     The module title as it will be displayed on the page. Handles cultures automatically.
        /// </summary>
        public virtual string TitleText
        {
            get
            {
                if (HttpContext.Current != null && this.titleText == string.Empty)
                {
                    // if it is not design time (and not overriden - Jes1111)
                    if (this.portalSettings.PortalContentLanguage != CultureInfo.InvariantCulture &&
                        this.Settings["MODULESETTINGS_TITLE_" + this.portalSettings.PortalContentLanguage.Name] != null &&
                        this.Settings["MODULESETTINGS_TITLE_" + this.portalSettings.PortalContentLanguage.Name].ToString
                            ().Length > 0)
                    {
                        this.titleText =
                            this.Settings["MODULESETTINGS_TITLE_" + this.portalSettings.PortalContentLanguage.Name].
                                ToString();
                    }
                    else
                    {
                        if (this.ModuleConfiguration != null)
                        {
                            this.titleText = this.ModuleConfiguration.ModuleTitle;
                        }
                        else
                        {
                            this.titleText = "TitleText Placeholder";
                        }
                    }
                }

                var title = string.Format(
                    "<span id=\"mTitle_{0}\" class=\"editTitle\">{1}</span>", this.ModuleID, this.titleText);
                return title;
            }

            set
            {
                this.titleText = value;
            }
        }

        /// <summary>
        /// The edit text.
        /// </summary>
        private string editText = "EDIT";

        /// <summary>
        /// The edit url.
        /// </summary>
        private string editUrl;

        /// <summary>
        /// The edit target.
        /// </summary>
        private string editTarget;

        /// <summary>
        ///     Text for Edit Link
        /// </summary>
        public string EditText
        {
            get
            {
                if (this.ModuleTitle != null && this.ModuleTitle.EditText.Length != 0)
                {
                    this.editText = this.ModuleTitle.EditText;
                }

                return this.editText;
            }

            set
            {
                this.editText = value;
            }
        }

        /// <summary>
        ///     Url for Edit Link
        /// </summary>
        public string EditUrl
        {
            get
            {
                if (this.ModuleTitle != null && this.ModuleTitle.EditUrl.Length != 0)
                {
                    this.editUrl = this.ModuleTitle.EditUrl;
                }

                return this.editUrl;
            }

            set
            {
                this.editUrl = value;
            }
        }

        /// <summary>
        ///     Target frame/page for Edit Link
        /// </summary>
        public string EditTarget
        {
            get
            {
                if (this.ModuleTitle != null && this.ModuleTitle.EditTarget.Length != 0)
                {
                    this.editUrl = this.ModuleTitle.EditTarget;
                }

                return this.editTarget;
            }

            set
            {
                this.editTarget = value;
            }
        }

        /// <summary>
        /// The add text.
        /// </summary>
        private string addText = "ADD";

        /// <summary>
        /// The add url.
        /// </summary>
        private string addUrl;

        /// <summary>
        /// The add target.
        /// </summary>
        private string addTarget;

        /// <summary>
        ///     Text for Add Link
        /// </summary>
        public string AddText
        {
            get
            {
                if (this.ModuleTitle != null && this.ModuleTitle.AddText.Length != 0)
                {
                    this.addText = this.ModuleTitle.AddText;
                }

                return this.addText;
            }

            set
            {
                this.addText = value;
            }
        }

        /// <summary>
        ///     Url for Add Link
        /// </summary>
        /// <value>The add URL.</value>
        public string AddUrl
        {
            get
            {
                if (this.ModuleTitle != null && this.ModuleTitle.AddUrl.Length != 0)
                {
                    this.addUrl = this.ModuleTitle.AddUrl;
                }

                return this.addUrl;
            }

            set
            {
                this.addUrl = value;
            }
        }

        /// <summary>
        ///     Target frame/page for Add Link
        /// </summary>
        public string AddTarget
        {
            get
            {
                if (this.ModuleTitle != null && this.ModuleTitle.AddTarget.Length != 0)
                {
                    this.addTarget = this.ModuleTitle.AddTarget;
                }

                return this.addTarget;
            }

            set
            {
                this.addTarget = value;
            }
        }

        /// <summary>
        /// The properties text.
        /// </summary>
        private string propertiesText = "PROPERTIES";

        /// <summary>
        /// The properties url.
        /// </summary>
        private string propertiesUrl = "~/DesktopModules/CoreModules/Admin/PropertyPage.aspx";

        /// <summary>
        /// The properties target.
        /// </summary>
        private string propertiesTarget;

        /// <summary>
        ///     Text for Properties Link
        /// </summary>
        public string PropertiesText
        {
            get
            {
                if (this.ModuleTitle != null && this.ModuleTitle.PropertiesText.Length != 0)
                {
                    this.propertiesText = this.ModuleTitle.PropertiesText;
                }

                return this.propertiesText;
            }

            set
            {
                this.propertiesText = value;
            }
        }

        /// <summary>
        ///     Url for Properties Link
        /// </summary>
        public string PropertiesUrl
        {
            get
            {
                if (this.ModuleTitle != null && this.ModuleTitle.PropertiesUrl.Length != 0)
                {
                    this.propertiesUrl = this.ModuleTitle.PropertiesUrl;
                }

                return this.propertiesUrl;
            }

            set
            {
                this.propertiesUrl = value;
            }
        }

        /// <summary>
        ///     Target frame/page for Properties Link
        /// </summary>
        public string PropertiesTarget
        {
            get
            {
                if (this.ModuleTitle != null && this.ModuleTitle.PropertiesTarget.Length != 0)
                {
                    this.propertiesTarget = this.ModuleTitle.PropertiesTarget;
                }

                return this.propertiesTarget;
            }

            set
            {
                this.propertiesTarget = value;
            }
        }

        /// <summary>
        /// The security text.
        /// </summary>
        private string securityText = "SECURITY";

        /// <summary>
        /// The security url.
        /// </summary>
        private string securityUrl = "~/DesktopModules/CoreModules/Admin/ModuleSettings.aspx";

        /// <summary>
        /// The security target.
        /// </summary>
        private string securityTarget;

        /// <summary>
        ///     Text for Security Link
        /// </summary>
        public string SecurityText
        {
            get
            {
                if (this.ModuleTitle != null && this.ModuleTitle.SecurityText.Length != 0)
                {
                    this.securityText = this.ModuleTitle.SecurityText;
                }

                return this.securityText;
            }

            set
            {
                this.securityText = value;
            }
        }

        /// <summary>
        ///     Url for Security Link
        /// </summary>
        public string SecurityUrl
        {
            get
            {
                if (this.ModuleTitle != null && this.ModuleTitle.SecurityUrl.Length != 0)
                {
                    this.securityUrl = this.ModuleTitle.SecurityUrl;
                }

                return this.securityUrl;
            }

            set
            {
                this.securityUrl = value;
            }
        }

        /// <summary>
        ///     Target frame/page for Security Link
        /// </summary>
        public string SecurityTarget
        {
            get
            {
                if (this.ModuleTitle != null && this.ModuleTitle.SecurityTarget.Length != 0)
                {
                    this.securityTarget = this.ModuleTitle.SecurityTarget;
                }

                return this.securityTarget;
            }

            set
            {
                this.securityTarget = value;
            }
        }

        #endregion

        #region ModuleButtons

        /// <summary>
        /// The buttons render as.
        /// </summary>
        private ModuleButton.RenderOptions buttonsRenderAs = ModuleButton.RenderOptions.ImageOnly;

        /// <summary>
        ///     Determines how ModuleButtons are rendered: as TextOnly, TextAndImage or ImageOnly. ImageOnly is the 'classic' Appleseed style.
        /// </summary>
        public ModuleButton.RenderOptions ButtonsRenderAs
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    // if it is not design time
                    if (this.Settings["MODULESETTINGS_BUTTON_DISPLAY"] != null &&
                        this.Settings["MODULESETTINGS_BUTTON_DISPLAY"].ToString().Length != 0)
                    {
                        this.buttonsRenderAs =
                            (ModuleButton.RenderOptions)
                            int.Parse(this.Settings["MODULESETTINGS_BUTTON_DISPLAY"].ToString());
                    }
                }

                return this.buttonsRenderAs;
            }

            set
            {
                this.buttonsRenderAs = value;
            }
        }

        /// <summary>
        /// The properties button.
        /// </summary>
        private ModuleButton propertiesButton;

        /// <summary>
        ///     Module Properties button
        /// </summary>
        public ModuleButton PropertiesButton
        {
            get
            {
                if (this.propertiesButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanProperties)
                    {
                        // create the button
                        this.propertiesButton = new ModuleButton();
                        this.propertiesButton.Group = ModuleButton.ButtonGroup.Admin;
                        this.propertiesButton.EnglishName = "Properties";
                        this.propertiesButton.TranslationKey = "PROPERTIES";
                        this.propertiesButton.Image = this.CurrentTheme.GetImage("Buttons_Properties", "Properties.gif");
                        if (this.PropertiesUrl.IndexOf("?") >= 0)
                        {
                            // Do not change if  the querystring is present (shortcut patch)
                            // if ( this.ModuleID != OriginalModuleID ) // shortcut
                            this.propertiesButton.HRef = this.PropertiesUrl;
                        }
                        else
                        {
                            this.propertiesButton.HRef = HttpUrlBuilder.BuildUrl(
                                this.PropertiesUrl, this.PageID, "mID=" + this.ModuleID);
                        }

                        this.propertiesButton.Target = this.PropertiesTarget;
                        this.propertiesButton.RenderAs = this.ButtonsRenderAs;
                    }
                }

                return this.propertiesButton;
            }
        }

        /// <summary>
        /// The security button.
        /// </summary>
        private ModuleButton securityButton;

        /// <summary>
        ///     Module Security button
        /// </summary>
        public ModuleButton SecurityButton
        {
            get
            {
                if (this.securityButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanSecurity)
                    {
                        // create the button
                        this.securityButton = new ModuleButton();
                        this.securityButton.Group = ModuleButton.ButtonGroup.Admin;
                        this.securityButton.EnglishName = "Security";
                        this.securityButton.TranslationKey = "SECURITY";
                        this.securityButton.Image = this.CurrentTheme.GetImage("Buttons_Security", "Security.gif");
                        if (this.SecurityUrl.IndexOf("?") >= 0)
                        {
                            // Do not change if  the querystring is present (shortcut patch)
                            this.securityButton.HRef = this.SecurityUrl;
                        }
                        else
                        {
                            this.securityButton.HRef = HttpUrlBuilder.BuildUrl(
                                this.SecurityUrl, this.PageID, "mID=" + this.ModuleID);
                        }

                        this.securityButton.Target = this.SecurityTarget;
                        this.securityButton.RenderAs = this.ButtonsRenderAs;
                    }
                }

                return this.securityButton;
            }
        }

        /// <summary>
        /// The delete module button.
        /// </summary>
        private ModuleButton deleteModuleButton;

        /// <summary>
        ///     "Delete this Module" button
        /// </summary>
        public ModuleButton DeleteModuleButton
        {
            get
            {
                if (this.deleteModuleButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanDeleteModule)
                    {
                        // create the button
                        this.deleteModuleButton = new ModuleButton();
                        this.deleteModuleButton.Group = ModuleButton.ButtonGroup.Admin;
                        this.deleteModuleButton.TranslationKey = "DELETEMODULE";
                        this.deleteModuleButton.EnglishName = "Delete this module";
                        this.deleteModuleButton.Image = this.CurrentTheme.GetImage("Buttons_DeleteModule", "Delete.gif");
                        this.deleteModuleButton.RenderAs = this.ButtonsRenderAs;

                        // TODO: This JavaScript Function Is used for different controls and should be in one place
                        // (it's also overweight considering that Javascript has a standard confirm() function - Jes1111)
                        if (this.Page.Request.Browser.EcmaScriptVersion.Major >= 1 &&
                            !this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "confirmDelete"))
                        {
                            string[] s = { "CONFIRM_DELETE" };
                            this.Page.ClientScript.RegisterClientScriptBlock(
                                this.Page.GetType(), 
                                "confirmDelete", 
                                PortalSettings.GetStringResource("CONFIRM_DELETE_SCRIPT", s));
                        }

                        if (this.deleteModuleButton.Attributes["onclick"] != null)
                        {
                            this.deleteModuleButton.Attributes["onclick"] = "return confirmDelete();" +
                                                                            this.deleteModuleButton.Attributes["onclick"
                                                                                ];
                        }
                        else
                        {
                            this.deleteModuleButton.Attributes.Add("onclick", "return confirmDelete();");
                        }

                        this.deleteModuleButton.ServerClick += this.DeleteModuleButton_Click;
                    }
                }

                return this.deleteModuleButton;
            }
        }

        /// <summary>
        /// The help button.
        /// </summary>
        private ModuleButton helpButton;

        /// <summary>
        ///     Module button that will launch the module help in a pop-up window
        /// </summary>
        public ModuleButton HelpButton
        {
            get
            {
                if (this.helpButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanHelp)
                    {
                        // build the HRef
                        var aux = this.ModuleConfiguration.DesktopSrc.Replace(".", "_");
                        var fileNameStart = aux.LastIndexOf("/");
                        var fileName = aux.Substring(fileNameStart + 1);
                        var sb = new StringBuilder();
                        sb.Append(Path.ApplicationRoot);
                        sb.Append(@"/rb_documentation/Viewer.aspx?loc=Appleseed/");
                        sb.Append(aux);
                        sb.Append("&src=");
                        sb.Append(fileName);

                        // create the button
                        this.helpButton = new ModuleButton();
                        this.helpButton.Group = ModuleButton.ButtonGroup.User;
                        this.helpButton.TranslationKey = "BTN_HELP";
                        this.helpButton.EnglishName = "Help";
                        this.helpButton.HRef = sb.ToString();
                        this.helpButton.PopUp = true;
                        this.helpButton.Target = "AppleseedHelp";
                        this.helpButton.PopUpOptions =
                            "toolbar=1,location=0,directories=0,status=0,menubar=1,scrollbars=1,resizable=1,width=600,height=400,screenX=15,screenY=15,top=15,left=15";
                        this.helpButton.Image = this.CurrentTheme.GetImage("Buttons_Help", "Help.gif");
                        this.helpButton.RenderAs = this.ButtonsRenderAs;
                    }
                }

                return this.helpButton;
            }
        }

        // Tiptopweb
        // Nicholas Smeaton: custom buttons from module developer enhancement added in Appleseed version 1.4.0.1767a - 03/07/2004
        /// <summary>
        /// The up button.
        /// </summary>
        private ModuleButton upButton;

        /// <summary>
        ///     Module Up button
        /// </summary>
        public ModuleButton UpButton
        {
            get
            {
                if (this.upButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanArrows && this.ModuleConfiguration.ModuleOrder != 1)
                    {
                        // create the button
                        this.upButton = new ModuleButton();
                        this.upButton.Group = ModuleButton.ButtonGroup.Admin;
                        this.upButton.TranslationKey = "MOVE_UP";
                        this.upButton.EnglishName = "Move up";
                        this.upButton.Image = this.CurrentTheme.GetImage("Buttons_Up", "Up.gif");
                        this.upButton.Attributes.Add("direction", "up");
                        this.upButton.Attributes.Add("pane", this.ModuleConfiguration.PaneName.ToLower());
                        this.upButton.RenderAs = this.ButtonsRenderAs;
                        this.upButton.ServerClick += this.UpDown_Click;
                    }
                }

                return this.upButton;
            }
        }

        /// <summary>
        /// The down button.
        /// </summary>
        private ModuleButton downButton;

        /// <summary>
        ///     Module Down button
        /// </summary>
        public ModuleButton DownButton
        {
            get
            {
                if (this.downButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanArrows)
                    {
                        var sourceList = this.GetModules(this.ModuleConfiguration.PaneName.ToLower());
                        var m = (ModuleItem)sourceList[sourceList.Count - 1];
                        if (this.ModuleConfiguration.ModuleOrder != m.Order)
                        {
                            // create the button
                            this.downButton = new ModuleButton();
                            this.downButton.Group = ModuleButton.ButtonGroup.Admin;
                            this.downButton.TranslationKey = "MOVE_DOWN";
                            this.downButton.EnglishName = "Move down";
                            this.downButton.Image = this.CurrentTheme.GetImage("Buttons_Down", "Down.gif");
                            this.downButton.Attributes.Add("direction", "down");
                            this.downButton.Attributes.Add("pane", this.ModuleConfiguration.PaneName.ToLower());
                            this.downButton.RenderAs = this.ButtonsRenderAs;
                            this.downButton.ServerClick += this.UpDown_Click;
                        }
                    }
                }

                return this.downButton;
            }
        }

        /// <summary>
        /// The left button.
        /// </summary>
        private ModuleButton leftButton;

        /// <summary>
        ///     Module Left button
        /// </summary>
        public ModuleButton LeftButton
        {
            get
            {
                if (this.leftButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanArrows && this.ModuleConfiguration.PaneName.ToLower() != "leftpane")
                    {
                        var leftButtonTargetPane = "contentpane";
                        if (this.ModuleConfiguration.PaneName.ToLower() == "contentpane")
                        {
                            leftButtonTargetPane = "leftpane";
                        }
                        else if (this.ModuleConfiguration.PaneName.ToLower() == "rightpane")
                        {
                            leftButtonTargetPane = "contentpane";
                        }

                        // create the button
                        this.leftButton = new ModuleButton();
                        this.leftButton.Group = ModuleButton.ButtonGroup.Admin;
                        this.leftButton.TranslationKey = "MOVE_LEFT";
                        this.leftButton.EnglishName = "Move left";
                        this.leftButton.Image = this.CurrentTheme.GetImage("Buttons_Left", "Left.gif");
                        this.leftButton.Attributes.Add("sourcepane", this.ModuleConfiguration.PaneName.ToLower());
                        this.leftButton.Attributes.Add("targetpane", leftButtonTargetPane);
                        this.leftButton.RenderAs = this.ButtonsRenderAs;
                        this.leftButton.ServerClick += this.RightLeft_Click;
                    }
                }

                return this.leftButton;
            }
        }

        /// <summary>
        /// The right button.
        /// </summary>
        private ModuleButton rightButton;

        /// <summary>
        ///     Module Right button
        /// </summary>
        public ModuleButton RightButton
        {
            get
            {
                if (this.rightButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanArrows && this.ModuleConfiguration.PaneName.ToLower() != "rightpane")
                    {
                        var rightButtonTargetPane = "contentpane";
                        if (this.ModuleConfiguration.PaneName.ToLower() == "contentpane")
                        {
                            rightButtonTargetPane = "rightpane";
                        }
                        else if (this.ModuleConfiguration.PaneName.ToLower() == "leftpane")
                        {
                            rightButtonTargetPane = "contentpane";
                        }

                        // create the button
                        this.rightButton = new ModuleButton();
                        this.rightButton.Group = ModuleButton.ButtonGroup.Admin;
                        this.rightButton.TranslationKey = "MOVE_RIGHT";
                        this.rightButton.EnglishName = "Move right";
                        this.rightButton.Image = this.CurrentTheme.GetImage("Buttons_Right", "Right.gif");
                        this.rightButton.Attributes.Add("sourcepane", this.ModuleConfiguration.PaneName.ToLower());
                        this.rightButton.Attributes.Add("targetpane", rightButtonTargetPane);
                        this.rightButton.RenderAs = this.ButtonsRenderAs;
                        this.rightButton.ServerClick += this.RightLeft_Click;
                    }
                }

                return this.rightButton;
            }
        }

        /// <summary>
        /// The ready to approve button.
        /// </summary>
        private ModuleButton readyToApproveButton;

        /// <summary>
        ///     Module ReadyToApprove button
        /// </summary>
        public ModuleButton ReadyToApproveButton
        {
            get
            {
                if (this.readyToApproveButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanRequestApproval)
                    {
                        // create the button
                        this.readyToApproveButton = new ModuleButton();
                        this.readyToApproveButton.Group = ModuleButton.ButtonGroup.Admin;
                        this.readyToApproveButton.TranslationKey = this.ReadyToApproveText;
                        this.readyToApproveButton.EnglishName = "Request approval";
                        this.readyToApproveButton.HRef =
                            HttpUrlBuilder.BuildUrl(
                                "~/DesktopModules/Workflow/RequestModuleContentApproval.aspx", 
                                this.PageID, 
                                "mID=" + this.ModuleID);
                        this.readyToApproveButton.Image = this.CurrentTheme.GetImage(
                            "Buttons_ReadyToApprove", "ReadyToApprove.gif");
                        this.readyToApproveButton.RenderAs = this.ButtonsRenderAs;
                    }
                }

                return this.readyToApproveButton;
            }
        }

        /// <summary>
        /// The revert button.
        /// </summary>
        private ModuleButton revertButton;

        /// <summary>
        ///     Module Revert button
        /// </summary>
        public ModuleButton RevertButton
        {
            get
            {
                if (this.revertButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanRequestApproval)
                    {
                        // create the button
                        this.revertButton = new ModuleButton();
                        this.revertButton.Group = ModuleButton.ButtonGroup.Admin;
                        this.revertButton.TranslationKey = this.RevertText;
                        this.revertButton.EnglishName = "Revert";
                        this.revertButton.Image = this.CurrentTheme.GetImage("Buttons_Revert", "Revert.gif");
                        this.revertButton.ServerClick += this.RevertToProductionContent;
                        this.revertButton.RenderAs = this.ButtonsRenderAs;
                    }
                }

                return this.revertButton;
            }
        }

        /// <summary>
        /// The approve button.
        /// </summary>
        private ModuleButton approveButton;

        /// <summary>
        ///     Module Approve button
        /// </summary>
        public ModuleButton ApproveButton
        {
            get
            {
                if (this.approveButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanApproveReject)
                    {
                        // create the button
                        this.approveButton = new ModuleButton();
                        this.approveButton.Group = ModuleButton.ButtonGroup.Admin;
                        this.approveButton.TranslationKey = this.ApproveText;
                        this.approveButton.EnglishName = "Approve";
                        this.approveButton.HRef =
                            HttpUrlBuilder.BuildUrl(
                                "~/DesktopModules/Workflow/ApproveModuleContent.aspx", 
                                this.PageID, 
                                "mID=" + this.ModuleID);
                        this.approveButton.Image = this.CurrentTheme.GetImage("Buttons_Approve", "Approve.gif");
                        this.approveButton.RenderAs = this.ButtonsRenderAs;
                    }
                }

                return this.approveButton;
            }
        }

        /// <summary>
        /// The reject button.
        /// </summary>
        private ModuleButton rejectButton;

        /// <summary>
        ///     Module Reject button
        /// </summary>
        public ModuleButton RejectButton
        {
            get
            {
                if (this.rejectButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanApproveReject)
                    {
                        // create the button
                        this.rejectButton = new ModuleButton();
                        this.rejectButton.Group = ModuleButton.ButtonGroup.Admin;
                        this.rejectButton.TranslationKey = this.RejectText;
                        this.rejectButton.EnglishName = "Reject";
                        this.rejectButton.HRef =
                            HttpUrlBuilder.BuildUrl(
                                "~/DesktopModules/Workflow/RejectModuleContent.aspx", 
                                this.PageID, 
                                "mID=" + this.ModuleID);
                        this.rejectButton.Image = this.CurrentTheme.GetImage("Buttons_Reject", "Reject.gif");
                        this.rejectButton.RenderAs = this.ButtonsRenderAs;
                    }
                }

                return this.rejectButton;
            }
        }

        /// <summary>
        /// The publish button.
        /// </summary>
        private ModuleButton publishButton;

        /// <summary>
        ///     Module Version button
        /// </summary>
        public ModuleButton PublishButton
        {
            get
            {
                if (this.publishButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanPublish)
                    {
                        // create the button
                        this.publishButton = new ModuleButton();
                        this.publishButton.Group = ModuleButton.ButtonGroup.Admin;
                        this.publishButton.TranslationKey = this.PublishText;
                        this.publishButton.EnglishName = "Publish";

                        // modified by Hongwei Shen
                        // publishButton.HRef = GetPublishUrl();
                        this.publishButton.ServerClick += this.publishButton_ServerClick;

                        // end of modification
                        this.publishButton.Image = this.CurrentTheme.GetImage("Buttons_Publish", "Publish.gif");
                        this.publishButton.RenderAs = this.ButtonsRenderAs;
                    }
                }

                return this.publishButton;
            }
        }

        /// <summary>
        /// The _version button.
        /// </summary>
        private ModuleButton _versionButton;

        /// <summary>
        ///     Module Version button
        /// </summary>
        public ModuleButton VersionButton
        {
            get
            {
                if (this._versionButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanVersion)
                    {
                        // create the button
                        this._versionButton = new ModuleButton();
                        this._versionButton.Group = ModuleButton.ButtonGroup.Admin;
                        if (this.Version == WorkFlowVersion.Staging)
                        {
                            this._versionButton.TranslationKey = this.ProductionVersionText;
                            this._versionButton.EnglishName = "To production version";
                            this._versionButton.Image = this.CurrentTheme.GetImage(
                                "Buttons_VersionToProduction", "VersionToProduction.gif");
                        }
                        else
                        {
                            this._versionButton.TranslationKey = this.StagingVersionText;
                            this._versionButton.EnglishName = "To staging version";
                            this._versionButton.Image = this.CurrentTheme.GetImage(
                                "Buttons_VersionToStaging", "VersionToStaging.gif");
                        }

                        this._versionButton.HRef = this.GetOtherVersionUrl();
                        this._versionButton.Target = this.EditTarget;
                        this._versionButton.RenderAs = this.ButtonsRenderAs;
                    }
                }

                return this._versionButton;
            }
        }

        /// <summary>
        /// The edit button.
        /// </summary>
        private ModuleButton editButton;

        /// <summary>
        ///     Module edit button
        /// </summary>
        public ModuleButton EditButton
        {
            get
            {
                if (this.editButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanEdit)
                    {
                        // create the button
                        this.editButton = new ModuleButton();
                        this.editButton.Group = ModuleButton.ButtonGroup.Admin;
                        this.editButton.TranslationKey = this.EditText;
                        this.editButton.EnglishName = "Edit";
                        if (this.EditUrl.IndexOf("?") >= 0)
                        {
                            // Do not change if  the querystring is present
                            // if ( this.ModuleID != OriginalModuleID )
                            this.editButton.HRef = this.EditUrl;
                        }
                        else
                        {
                            this.editButton.HRef = HttpUrlBuilder.BuildUrl(
                                this.EditUrl, this.PageID, "mID=" + this.ModuleID);
                        }

                        this.editButton.Target = this.EditTarget;
                        this.editButton.Image = this.CurrentTheme.GetImage("Buttons_Edit", "Edit.gif");
                        this.editButton.RenderAs = this.ButtonsRenderAs;
                    }
                }

                return this.editButton;
            }
        }

        /// <summary>
        /// The add button.
        /// </summary>
        private ModuleButton addButton;

        /// <summary>
        ///     Module Add button
        /// </summary>
        public ModuleButton AddButton
        {
            get
            {
                if (this.addButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanAdd)
                    {
                        // create the button
                        this.addButton = new ModuleButton();
                        this.addButton.Group = ModuleButton.ButtonGroup.Admin;
                        this.addButton.TranslationKey = this.AddText;
                        this.addButton.EnglishName = "Add";
                        if (this.AddUrl.IndexOf("?") >= 0)
                        {
                            // Do not change if  the querystring is present
                            // if ( this.ModuleID != OriginalModuleID )
                            this.AddButton.HRef = this.AddUrl;
                        }
                        else
                        {
                            this.AddButton.HRef = HttpUrlBuilder.BuildUrl(
                                this.AddUrl, this.PageID, "mID=" + this.ModuleID);
                        }

                        this.addButton.Target = this.AddTarget;
                        this.addButton.Image = this.CurrentTheme.GetImage("Buttons_Add", "Add.gif");
                        this.addButton.RenderAs = this.ButtonsRenderAs;
                    }
                }

                return this.addButton;
            }
        }

        /// <summary>
        /// The back button.
        /// </summary>
        private ModuleButton backButton;

        /// <summary>
        ///     Module button that will return to previous tab
        /// </summary>
        public ModuleButton BackButton
        {
            get
            {
                if (this.backButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanBack)
                    {
                        // create the button
                        this.backButton = new ModuleButton();
                        this.backButton.Group = ModuleButton.ButtonGroup.User;
                        this.backButton.TranslationKey = "BTN_BACK";
                        this.backButton.EnglishName = "Back to previous page";
                        this.backButton.HRef = this.Request.UrlReferrer.ToString();
                        this.backButton.Image = this.CurrentTheme.GetImage("Buttons_Back", "Back.gif");
                        this.backButton.RenderAs = this.ButtonsRenderAs;
                    }
                }

                return this.backButton;
            }
        }

        /// <summary>
        /// The print button.
        /// </summary>
        private ModuleButton printButton;

        /// <summary>
        ///     Module button that will launch the module in a pop-up window suitable for printing
        /// </summary>
        public ModuleButton PrintButton
        {
            get
            {
                if (this.printButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanPrint)
                    {
                        // build the HRef
                        var _url = new StringBuilder();
                        _url.Append(Path.ApplicationRoot);
                        _url.Append("/app_support/print.aspx?");
                        _url.Append(this.Request.QueryString.ToString());
                        if (!(this.Request.QueryString.ToString().ToLower().IndexOf("mid=") > 0))
                        {
                            _url.Append("&mID=");
                            _url.Append(this.ModuleID.ToString());
                        }

                        _url.Append("&ModId=");
                        _url.Append(this.OriginalModuleID.ToString());

                        // create the button
                        this.printButton = new ModuleButton();
                        this.printButton.Group = ModuleButton.ButtonGroup.User;
                        this.printButton.Image = this.CurrentTheme.GetImage("Buttons_Print", "Print.gif");
                        this.printButton.TranslationKey = "BTN_PRINT";
                        this.printButton.EnglishName = "Print this";
                        this.printButton.HRef = _url.ToString();
                        this.printButton.PopUp = true;
                        this.printButton.Target = "AppleseedPrint";
                        this.printButton.PopUpOptions =
                            "toolbar=1,menubar=1,resizable=1,scrollbars=1,width=600,height=400,left=15,top=15";
                        this.printButton.RenderAs = this.ButtonsRenderAs;
                    }
                }

                return this.printButton;
            }
        }

        /// <summary>
        /// The email button.
        /// </summary>
        private ModuleButton emailButton;

        /// <summary>
        ///     Module button that will launch a pop-up window to allow the module contents to be emailed
        /// </summary>
        /// <remarks>
        ///     Not implemented yet.
        /// </remarks>
        public ModuleButton EmailButton
        {
            get
            {
                if (this.emailButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (this.CanEmail)
                    {
                        // not implemented
                        // 					javaScript = "EmailWindow=window.open('" 
                        // 						+ HttpUrlBuilder.BuildUrl("email.aspx","src=" + portalModule.ModuleCacheKey + "content") 
                        // 						+ "','EmailWindow','toolbar=yes,location=no,directories=no,status=no,menubar=yes,scrollbars=yes,resizable=yes,width=640,height=480,left=15,top=15'); return false;";
                        // 					EmailButton.Text = Esperantus.General.GetString("BTN_Email","Email this",null) + "...";
                        // 					EmailButton.NavigateUrl = string.Empty;
                        // 					EmailButton.CssClass = "rb_mod_btn";
                        // 					EmailButton.Attributes.Add("onclick", javaScript);
                        // 					EmailButton.ImageUrl = CurrentTheme.GetImage("Buttons_Email").ImageUrl;
                        // 					ButtonList.Add(EmailButton);
                    }

                    this.emailButton = null;
                }

                return this.emailButton;
            }
        }

        /// <summary>
        /// The min max button.
        /// </summary>
        private LinkButton minMaxButton;

        /*
        /// <summary>
        /// Module button to minimize/maximize module
        /// </summary>
        public LinkButton MinMaxButton
        {
            get
            {
                if (minMaxButton == null && HttpContext.Current != null)
                {
                    // check authority
                    if (_vcm != null && !UserDesktop.isClosed(_vcm.ModuleID) && CanMinimized)
                    {
                        // create the button based on current view
                        if (!UserDesktop.isMinimized(_vcm.ModuleID))
                            minMaxButton =
                                _vcm.create(WindowStateStrings.ButtonMinName, WindowStateStrings.ButtonMinLocalized);
                        else
                        {
                            minMaxButton =
                                _vcm.create(WindowStateStrings.ButtonMaxName, WindowStateStrings.ButtonMaxLocalized);
                            // we are minimized -- show the user a hint by changing the color [future]
                            // min_hint_ = true;
                        }

                        // set additional button properties
                        minMaxButton.CssClass = "rb_mod_btn";
                    }
                }
                return minMaxButton;
            }
        }
        */

        /// <summary>
        /// The close button.
        /// </summary>
        private LinkButton closeButton;

        /*      
        /// <summary>
        /// Module button to close module
        /// </summary>
        public LinkButton CloseButton
        {
            get
            {
                if (closeButton == null && HttpContext.Current != null)
                {
                    // check authority
                    // jes1111 - if (_vcm != null && !UserDesktop.isClosed(_vcm.ModuleID) && CanClose && GlobalResources.SupportWindowMgmtClose)
                    if (_vcm != null && !UserDesktop.isClosed(_vcm.ModuleID) && CanClose && Config.WindowMgmtWantClose)
                    {
                        // create the button
                        closeButton =
                            _vcm.create(WindowStateStrings.ButtonCloseName, WindowStateStrings.ButtonClosedLocalized);

                        // set attribute to confirm delete
                        setDeleteAttributes(ref closeButton);

                        // set additional button properties
                        closeButton.CssClass = "rb_mod_btn";
                    }
                }
                return closeButton;
            }
        }
        */
        #endregion

        #region Permissions

        /// <summary>
        ///     View permission
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsViewable
        {
            get
            {
                if (this._moduleConfiguration == null || this._moduleConfiguration.AuthorizedViewRoles == null)
                {
                    return false;
                }

                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if (this._canView == 0)
                {
                    if (PortalSecurity.IsInRoles(this._moduleConfiguration.AuthorizedViewRoles))
                    {
                        this._canView = 1;
                    }
                    else
                    {
                        this._canView = 2;
                    }
                }

                return this._canView == 1;
            }
        }

        /// <summary>
        ///     Add permission
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAddable
        {
            get
            {
                if (this._moduleConfiguration == null || this._moduleConfiguration.AuthorizedAddRoles == null)
                {
                    return false;
                }

                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if (this._canAdd == 0)
                {
                    // Change by Geert.Audenaert@Syntegra.Com
                    // Date: 7/2/2003
                    if (this.SupportsWorkflow && this.Version == WorkFlowVersion.Production)
                    {
                        this._canAdd = 2;
                    }
                    else
                    {
                        // End Change Geert.Audenaert@Syntegra.Com
                        if (PortalSecurity.IsInRoles(this._moduleConfiguration.AuthorizedAddRoles))
                        {
                            this._canAdd = 1;
                        }
                        else
                        {
                            this._canAdd = 2;
                        }

                        // Change by Geert.Audenaert@Syntegra.Com
                    }

                    // Date: 7/2/2003
                    // End Change Geert.Audenaert@Syntegra.Com
                }

                return this._canAdd == 1;
            }
        }

        /// <summary>
        ///     Edit permission
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEditable
        {
            get
            {
                if (this._moduleConfiguration == null || this._moduleConfiguration.AuthorizedEditRoles == null)
                {
                    return false;
                }

                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if (this._canEdit == 0)
                {
                    // Change by Geert.Audenaert@Syntegra.Com
                    // Date: 7/2/2003
                    if (this.SupportsWorkflow && this.Version == WorkFlowVersion.Production)
                    {
                        this._canEdit = 2;
                    }
                    else
                    {
                        // End Change Geert.Audenaert@Syntegra.Com
                        // 						if (portalSettings.AlwaysShowEditButton == true || PortalSecurity.IsInRoles(_moduleConfiguration.AuthorizedEditRoles))
                        if (PortalSecurity.IsInRoles(this._moduleConfiguration.AuthorizedEditRoles))
                        {
                            this._canEdit = 1;
                        }
                        else
                        {
                            this._canEdit = 2;
                        }

                        // Change by Geert.Audenaert@Syntegra.Com
                        // Date: 7/2/2003
                    }

                    // End Change Geert.Audenaert@Syntegra.Com
                }

                return this._canEdit == 1;
            }
        }

        /// <summary>
        ///     Delete permission
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDeleteable
        {
            get
            {
                if (this._moduleConfiguration == null || this._moduleConfiguration.AuthorizedDeleteRoles == null)
                {
                    return false;
                }

                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if (this._canDelete == 0)
                {
                    // Change by Geert.Audenaert@Syntegra.Com
                    // Date: 7/2/2003
                    if (this.SupportsWorkflow && this.Version == WorkFlowVersion.Production)
                    {
                        this._canDelete = 2;
                    }
                    else
                    {
                        // End Change Geert.Audenaert@Syntegra.Com
                        if (PortalSecurity.IsInRoles(this._moduleConfiguration.AuthorizedDeleteRoles))
                        {
                            this._canDelete = 1;
                        }
                        else
                        {
                            this._canDelete = 2;
                        }

                        // Change by Geert.Audenaert@Syntegra.Com
                        // Date: 7/2/2003
                    }

                    // End Change Geert.Audenaert@Syntegra.Com
                }

                return this._canDelete == 1;
            }
        }

        /// <summary>
        ///     Edit properties permission
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ArePropertiesEditable
        {
            get
            {
                if (this._moduleConfiguration == null || this._moduleConfiguration.AuthorizedPropertiesRoles == null)
                {
                    return false;
                }

                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if (this._canProperties == 0)
                {
                    if (PortalSecurity.IsInRoles(this._moduleConfiguration.AuthorizedPropertiesRoles))
                    {
                        this._canProperties = 1;
                    }
                    else
                    {
                        this._canProperties = 2;
                    }
                }

                return this._canProperties == 1;
            }
        }

        /// <summary>
        ///     Minimize permission
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanMinimized
        {
            get
            {
                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if (this._canMin == 0)
                {
                    if (PortalSecurity.IsInRoles(this._moduleConfiguration.AuthorizedViewRoles))
                    {
                        this._canMin = 1;
                    }
                    else
                    {
                        this._canMin = 2;
                    }
                }

                return this._canMin == 1;
            }
        }

        // end of CanMinimized

        /// <summary>
        ///     Close permission
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanClose
        {
            get
            {
                // Perform tri-state switch check to avoid having to perform a security
                // role lookup on every property access (instead caching the result)
                if (this._canClose == 0)
                {
                    if (PortalSecurity.IsInRoles(this._moduleConfiguration.AuthorizedDeleteRoles))
                    {
                        this._canClose = 1;
                    }
                    else
                    {
                        this._canClose = 2;
                    }
                }

                return this._canClose == 1;
            }
        }

        // end of CanClose

        /// <summary>
        ///     Print permission
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanPrint
        {
            get
            {
                if (this.SupportsPrint && this.Settings["MODULESETTINGS_SHOW_PRINT_BUTTION"] != null &&
                    bool.Parse(this.Settings["MODULESETTINGS_SHOW_PRINT_BUTTION"].ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Permission for DeleteModuleButton
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanDeleteModule
        {
            get
            {
                if (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedDeleteModuleRoles) &&
                    this.portalSettings.ActivePage.PageID == this.ModuleConfiguration.PageID)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Permission for HelpButton
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanHelp
        {
            get
            {
                if (this.SupportsHelp &&
                    (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedEditRoles) ||
                     (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedAddRoles)) ||
                     PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedDeleteRoles) ||
                     PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedPropertiesRoles) ||
                     PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedPublishingRoles)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Permission for BackButton
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanBack
        {
            get
            {
                if (this.SupportsBack && this.ShowBack && this.Request.UrlReferrer != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Permission for EmailButton
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanEmail
        {
            get
            {
                if (this.SupportsEmail && this.Settings["ShowEmailButton"] != null &&
                    bool.Parse(this.Settings["ShowEmailButton"].ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Permission for EditButton
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanEdit
        {
            get
            {
                if (this.ModuleConfiguration == null ||
                    this.portalSettings.ActivePage.PageID != this.ModuleConfiguration.PageID)
                {
                    return false;
                }

                if ((this.SupportsWorkflow && this.Version == WorkFlowVersion.Staging) || !this.SupportsWorkflow)
                {
                    if (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedEditRoles) &&
                        (this.EditUrl != null) && (this.EditUrl.Length != 0) &&
                        (this.WorkflowStatus == WorkflowState.Original || this.WorkflowStatus == WorkflowState.Working))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The get users that can edit.
        /// </summary>
        /// <returns>
        /// </returns>
        public ArrayList GetUsersThatCanEdit()
        {
            var roles = this.ModuleConfiguration.AuthorizedEditRoles;
            return GetUsersInRoles(roles);
        }

        /// <summary>
        /// The get users that can view.
        /// </summary>
        /// <returns>
        /// </returns>
        public ArrayList GetUsersThatCanView()
        {
            var roles = this.ModuleConfiguration.AuthorizedViewRoles;
            return GetUsersInRoles(roles);
        }

        /// <summary>
        /// The get users that can add.
        /// </summary>
        /// <returns>
        /// </returns>
        public ArrayList GetUsersThatCanAdd()
        {
            var roles = this.ModuleConfiguration.AuthorizedAddRoles;
            return GetUsersInRoles(roles);
        }

        /// <summary>
        /// The get users in roles.
        /// </summary>
        /// <param name="roles">
        /// The roles.
        /// </param>
        /// <returns>
        /// </returns>
        private static ArrayList GetUsersInRoles(string roles)
        {
            var result = new ArrayList();
            

            var context = HttpContext.Current;

            if (roles != null)
            {
                foreach (var splitRole in roles.Split(new[] { ';' }))
                {
                    if (!String.IsNullOrEmpty(splitRole))
                    {
                        if (splitRole != null && splitRole.Length != 0 && splitRole == "All Users")
                        {
                            var collection = Membership.GetAllUsers();
                            foreach (MembershipUser user in collection)
                            {
                                if (!result.Contains(user.Email))
                                {
                                    result.Add(user.Email);
                                }
                            }
                        }
                        else if (splitRole == "Authenticated Users" && context.Request.IsAuthenticated)
                        {
                            if (!result.Contains(Membership.GetUser().Email))
                            {
                                result.Add(Membership.GetUser().Email);
                            }
                        }
                        else if ((splitRole == "Unauthenticated Users") && (!context.Request.IsAuthenticated))
                        {
                            // TODO: no me queda claro que devolver en este caso
                            var collection = Membership.GetAllUsers();
                            foreach (MembershipUser user in collection)
                            {
                                if (!result.Contains(user.Email))
                                {
                                    result.Add(user.Email);
                                }
                            }
                        }
                        else
                        {
                            var users = Roles.GetUsersInRole(splitRole);
                            foreach (var user in users)
                            {
                                if (!result.Contains(user))
                                {
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
        ///     Permission for AddButton
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanAdd
        {
            get
            {
                if (this.ModuleConfiguration == null ||
                    this.portalSettings.ActivePage.PageID != this.ModuleConfiguration.PageID)
                {
                    return false;
                }

                if ((this.SupportsWorkflow && this.Version == WorkFlowVersion.Staging) || !this.SupportsWorkflow)
                {
                    if (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedAddRoles) && (this.AddUrl != null) &&
                        (this.AddUrl.Length != 0) &&
                        (this.WorkflowStatus == WorkflowState.Original || this.WorkflowStatus == WorkflowState.Working))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Permission for Version Button
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanVersion
        {
            get
            {
                if (this.ModuleConfiguration == null)
                {
                    return false;
                }

                if (this.SupportsWorkflow &&
                    (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedAddRoles) ||
                     PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedDeleteRoles) ||
                     PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedEditRoles) ||
                     PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedApproveRoles) ||
                     PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedPublishingRoles)) &&
                    (this.ProductionVersionText != null) && (this.StagingVersionText != null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Permission for Publish Button
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanPublish
        {
            get
            {
                if (this.ModuleConfiguration == null)
                {
                    return false;
                }

                if (this.SupportsWorkflow &&
                    PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedPublishingRoles) &&
                    (this.PublishText != null) && this.Version == WorkFlowVersion.Staging &&
                    this.WorkflowStatus == WorkflowState.Approved)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Permission for Approve/Reject Buttons
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanApproveReject
        {
            get
            {
                if (this.ModuleConfiguration == null)
                {
                    return false;
                }

                if (this.SupportsWorkflow && PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedApproveRoles) &&
                    (this.ApproveText != null) && (this.RejectText != null) && this.Version == WorkFlowVersion.Staging &&
                    this.WorkflowStatus == WorkflowState.ApprovalRequested)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Permission for ReadyToApprove and Revert Buttons
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanRequestApproval
        {
            get
            {
                if (this.ModuleConfiguration == null)
                {
                    return false;
                }

                if (this.SupportsWorkflow &&
                    (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedAddRoles) ||
                     PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedDeleteRoles) ||
                     PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedEditRoles)) &&
                    (this.ReadyToApproveText != null) && this.Version == WorkFlowVersion.Staging &&
                    this.WorkflowStatus == WorkflowState.Working)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Permission for Arrow Buttons (Up/Down/Left/Right)
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanArrows
        {
            get
            {
                if (this.ModuleConfiguration == null || this.ModuleID == 0)
                {
                    return false;
                }

                if (this.SupportsArrows && this.portalSettings.ActivePage.PageID == this.ModuleConfiguration.PageID &&
                    PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedMoveModuleRoles))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Permission for Security Button
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanSecurity
        {
            get
            {
                if (this.ModuleConfiguration == null)
                {
                    return false;
                }

                if (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedPropertiesRoles) &&
                    this.portalSettings.ActivePage.PageID == this.ModuleConfiguration.PageID && this.SecurityUrl != null &&
                    this.SecurityUrl.Length != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Permission for Properties Button
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanProperties
        {
            get
            {
                if (this.ModuleConfiguration == null)
                {
                    return false;
                }

                if (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedPropertiesRoles) &&
                    this.portalSettings.ActivePage.PageID == this.ModuleConfiguration.PageID &&
                    this.PropertiesUrl != null && this.PropertiesUrl.Length != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Can be set from module code to indicate whether module should display Back button
        /// </summary>
        public bool ShowBack { get; set; }

        #endregion

        #region Workflow

        /// <summary>
        /// The production version text.
        /// </summary>
        private string productionVersionText = "SWI_SWAPTOPRODUCTION";

        /// <summary>
        ///     Text for version Link for swapping to production version
        /// </summary>
        public string ProductionVersionText
        {
            get
            {
                return this.productionVersionText;
            }

            set
            {
                this.productionVersionText = value;
            }
        }

        /// <summary>
        /// The staging version text.
        /// </summary>
        private string stagingVersionText = "SWI_SWAPTOSTAGING";

        /// <summary>
        ///     Text for version Link for swapping to staging version
        /// </summary>
        public string StagingVersionText
        {
            get
            {
                return this.stagingVersionText;
            }

            set
            {
                this.stagingVersionText = value;
            }
        }

        /// <summary>
        /// The publish text.
        /// </summary>
        private string publishText = "SWI_PUBLISH";

        /// <summary>
        ///     Text for publish link
        /// </summary>
        public string PublishText
        {
            get
            {
                return this.publishText;
            }

            set
            {
                this.publishText = value;
            }
        }

        /// <summary>
        /// The revert text.
        /// </summary>
        private string revertText = "SWI_REVERT";

        /// <summary>
        /// The revert text.
        /// </summary>
        public string RevertText
        {
            get
            {
                return this.revertText;
            }

            set
            {
                this.revertText = value;
            }
        }

        /// <summary>
        /// The ready to approve text.
        /// </summary>
        private string readyToApproveText = "SWI_READYTOAPPROVE";

        /// <summary>
        ///     Text for request approval link
        /// </summary>
        public string ReadyToApproveText
        {
            get
            {
                return this.readyToApproveText;
            }

            set
            {
                this.readyToApproveText = value;
            }
        }

        /// <summary>
        /// The approve text.
        /// </summary>
        private string approveText = "SWI_APPROVE";

        /// <summary>
        ///     Text for approve link
        /// </summary>
        public string ApproveText
        {
            get
            {
                return this.approveText;
            }

            set
            {
                this.approveText = value;
            }
        }

        /// <summary>
        /// The reject text.
        /// </summary>
        private string rejectText = "SWI_REJECT";

        /// <summary>
        ///     Text for reject link
        /// </summary>
        public string RejectText
        {
            get
            {
                return this.rejectText;
            }

            set
            {
                this.rejectText = value;
            }
        }

        /// <summary>
        /// Publish staging to production
        /// </summary>
        protected virtual void Publish()
        {
            // Publish module
            WorkFlowDB.Publish(this.ModuleConfiguration.ModuleID);

            // Show the prod version
            this.Version = WorkFlowVersion.Production;
        }

        // Change by Geert.Audenaert@Syntegra.Com
        // Date: 27/2/2003
        /// <summary>
        ///     This property indicates the staging content state
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WorkflowState WorkflowStatus
        {
            get
            {
                if (this.SupportsWorkflow)
                {
                    return this._moduleConfiguration.WorkflowStatus;
                }
                else
                {
                    return WorkflowState.Original;
                }
            }
        }

        // End Change Geert.Audenaert@Syntegra.Com

        // Change by Geert.Audenaert@Syntegra.Com
        // Date: 6/2/2003
        /// <summary>
        ///     This property indicates which content will be shown to the user
        ///     production content or staging content
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WorkFlowVersion Version
        {
            get
            {
                if (!this.SupportsWorkflow)
                {
                    return WorkFlowVersion.Staging;
                }
                else
                {
                    return this._version;
                }
            }

            set
            {
                if (value == WorkFlowVersion.Staging)
                {
                    if (!
                        (PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedAddRoles) ||
                         PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedDeleteRoles) ||
                         PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedEditRoles) ||
                         PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedPublishingRoles) ||
                         PortalSecurity.IsInRoles(this.ModuleConfiguration.AuthorizedApproveRoles)))
                    {
                        PortalSecurity.AccessDeniedEdit();
                    }
                }

                this._version = value;
            }
        }

        /// <summary>
        /// Event handler for Workflow "revert"
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void RevertToProductionContent(object sender, EventArgs e)
        {
            // Revert
            WorkFlowDB.Revert(this.ModuleID);

            // Refresh current screen
            var querystring = "?";

            // Modified by Hongwei Shen(hongwei.shen@gmail.com) 8/9/2005
            // the key-value pairs are not separated by '&'.
            /* original code
            foreach (string key in Page.Request.QueryString.Keys)
                querystring += key + "=" + Context.Server.UrlEncode(Page.Request.QueryString[key]);
            */
            // start of modification
            var i = 0;
            var totalKeys = this.Page.Request.QueryString.Keys.Count;
            foreach (string key in this.Page.Request.QueryString.Keys)
            {
                querystring += key + "=" + this.Context.Server.UrlEncode(this.Page.Request.QueryString[key]);
                if (i < totalKeys - 1)
                {
                    querystring += '&';
                }

                i++;
            }

            // the call to stored-procedure rb_revert will reset
            // the WorkflowStatus to 0 (original) and we also need
            // to synchronize the module configuration to remove the
            // ReadyToApprove and Revert buttons.
            this._moduleConfiguration.WorkflowStatus = 0;

            // end of modification			
            this.Context.Server.Transfer(this.Page.Request.Path + querystring);
        }

        /// <summary>
        /// This function constructs the NavigateUrl for the SwapVersions hyperlink
        /// </summary>
        /// <returns>
        /// string
        /// </returns>
        private string GetOtherVersionUrl()
        {
            var url = this.Page.Request.Path;
            string querystring;
            var qs = new ArrayList();

            foreach (string var in this.Page.Request.QueryString.Keys)
            {
                // Added null check by manu
                if (var != null && !(var.StartsWith("wversion") || var.StartsWith("wpublish")))
                {
                    qs.Add(var + "=" + this.Page.Server.UrlEncode(this.Page.Request.QueryString[var]));
                }
            }

            qs.Add(
                "wversion" + this.ModuleConfiguration.ModuleID + "=" +
                (this.Version == WorkFlowVersion.Production
                     ? WorkFlowVersion.Staging.ToString()
                     : WorkFlowVersion.Production.ToString()));
            querystring = string.Join("&", (string[])qs.ToArray(typeof(string)));
            if (querystring.Length != 0)
            {
                url += "?" + querystring;
            }

            return url;
        }

        /// <summary>
        /// This function constructs the NavigateUrl for the Publish hyperlink
        /// </summary>
        /// <returns>
        /// string
        /// </returns>
        private string GetPublishUrl()
        {
            var url = this.Page.Request.Path;
            var qs = new ArrayList();
            foreach (string var in
                this.Page.Request.QueryString.Keys.Cast<string>().Where(var => var != null && !(var.StartsWith("wversion") || var.StartsWith("wpublish"))))
            {
                qs.Add(var + "=" + this.Page.Server.UrlEncode(this.Page.Request.QueryString[var]));
            }

            // modified by Hongwei Shen (hongwei.shen@gmail.com) 8/9/2005
            // qs.Add("wpublish" + this.ModuleConfiguration.ModuleID.ToString() + "=doit"); 
            // end of modification
            string querystring = string.Join("&", (string[])qs.ToArray(typeof(string)));
            if (querystring.Length != 0)
            {
                url += "?" + querystring;
            }

            return url;
        }

        #endregion

        #region publish button click event handler

        // added by Hongwei Shen to handle the publish button click
        // server event (hongwei.shen@gmail.com) 8/9/2005
        /// <summary>
        /// Handles the ServerClick event of the publishButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void publishButton_ServerClick(object sender, EventArgs e)
        {
            this.Publish();

            // redirect to the same page to pick up changes
            this.Page.Response.Redirect(this.GetPublishUrl());
        }

        // end of addition
        #endregion

        #region Search

        /// <summary>
        /// Searchable module implementation
        /// </summary>
        /// <param name="portalID">
        /// The portal ID
        /// </param>
        /// <param name="userID">
        /// ID of the user is searching
        /// </param>
        /// <param name="searchString">
        /// The text to search
        /// </param>
        /// <param name="searchField">
        /// The fields where perfoming the search
        /// </param>
        /// <returns>
        /// The SELECT sql to perform a search on the current module
        /// </returns>
        public virtual string SearchSqlSelect(int portalID, int userID, string searchString, string searchField)
        {
            return string.Empty;
        }

        #endregion

        #region LastModified

        /// <summary>
        /// Returns the "Last Modified" string, or an empty string if option is not active.
        /// </summary>
        /// <returns>
        /// The get last modified.
        /// </returns>
        public string GetLastModified()
        {
            // CHANGE by david.verberckmoes@syntegra.com on june, 2 2003
            if (bool.Parse(((SettingItem)this.portalSettings.CustomSettings["SITESETTINGS_SHOW_MODIFIED_BY"]).Value) &&
                bool.Parse(((SettingItem)this.Settings["MODULESETTINGS_SHOW_MODIFIED_BY"]).Value))
            {
                // Get stuff from database
                var Email = string.Empty;
                var TimeStamp = DateTime.MinValue;
                WorkFlowDB.GetLastModified(this.ModuleID, this.Version, ref Email, ref TimeStamp);

                // Do some checking
                if (Email == string.Empty)
                {
                    return string.Empty;
                }

                // Check if email address is valid
                var eal = new EmailAddressList();
                try
                {
                    eal.Add(Email);
                    Email = string.Format("<a href=\"mailto:{0}\">{1}</a>", Email, Email);
                }
                catch
                {
                }

                // Construct the rest of the html
                return string.Format("<span class=\"LastModified\">{0}&#160;{1}&#160;{2}&#160;{3} {4}</span>", General.GetString("LMB_LAST_MODIFIED_BY"), Email, General.GetString("LMB_ON"), TimeStamp.ToLongDateString(), TimeStamp.ToShortTimeString());
            }
            
            return string.Empty;

            // END CHANGE by david.verberckmoes@syntegra.com on june, 2 2003
        }

        #endregion

        #region Arrow button functions

        /// <summary>
        /// function for module moving
        /// </summary>
        /// <param name="list">The list.</param>
        private static void OrderModules(ArrayList list)
        {
            var i = 1;

            // sort the arraylist
            list.Sort();

            // renumber the order
            foreach (ModuleItem m in list)
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
        /// <param name="pane">The pane.</param>
        /// <returns></returns>
        private ArrayList GetModules(string pane)
        {
            var paneModules = new ArrayList();

            // get the portal setting at the Tab level and not from this class as it is not refreshed
            foreach (var m in from ModuleSettings module in this.Page.portalSettings.ActivePage.Modules
                              where this.portalSettings.ActivePage.PageID == module.PageID && module.PaneName.ToLower() == pane.ToLower()
                              select new ModuleItem
                                  {
                                      Title = module.ModuleTitle, ID = module.ModuleID, ModuleDefID = module.ModuleDefID, Order = module.ModuleOrder, PaneName = module.PaneName // tiptopweb
                                  })
            {
                paneModules.Add(m);
            }

            return paneModules;
        }

        /// <summary>
        /// function for module moving
        /// </summary>
        /// <param name="url">
        /// </param>
        /// <param name="moduleId">
        /// </param>
        /// <returns>
        /// The append module id.
        /// </returns>
        private static string AppendModuleId(string url, int moduleId)
        {
            // tiptopweb, sometimes the home page does not have parameters 
            // so we test for both & and ?
            var selectedModIdPos = url.IndexOf("&selectedmodid");
            var selectedModIdPos2 = url.IndexOf("?selectedmodid");
            if (selectedModIdPos >= 0)
            {
                var selectedModIdEndPos = url.IndexOf("&", selectedModIdPos + 1);
                return selectedModIdEndPos >= 0 ? string.Format("{0}&selectedmodid={1}{2}", url.Substring(0, selectedModIdPos), moduleId, url.Substring(selectedModIdEndPos)) : string.Format("{0}&selectedmodid={1}", url.Substring(0, selectedModIdPos), moduleId);
            }

            if (selectedModIdPos2 >= 0)
            {
                var selectedModIdEndPos2 = url.IndexOf("?", selectedModIdPos2 + 1);
                return selectedModIdEndPos2 >= 0
                           ? string.Format("{0}?selectedmodid={1}{2}", url.Substring(0, selectedModIdPos2), moduleId, url.Substring(selectedModIdEndPos2))
                           : string.Format("{0}?selectedmodid={1}", url.Substring(0, selectedModIdPos2), moduleId);
            }

            return url.IndexOf("?") >= 0 ? string.Format("{0}&selectedmodid={1}", url, moduleId) : string.Format("{0}?selectedmodid={1}", url, moduleId);
        }

        /// <summary>
        /// The RightLeft_Click server event handler on this page is
        ///     used to move a portal module between layout panes on
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void RightLeft_Click(object sender, EventArgs e)
        {
            var sourcePane = ((ModuleButton)sender).Attributes["sourcepane"];
            var targetPane = ((ModuleButton)sender).Attributes["targetpane"];

            // get source arraylist
            var sourceList = this.GetModules(sourcePane);

            // add it to the database
            // tiptopweb : OriginalModuleID to have it work with shortcut module
            var admin = new ModulesDB();
            admin.UpdateModuleOrder(this.OriginalModuleID, 99, targetPane);

            // reload the portalSettings from the database
            HttpContext.Current.Items["PortalSettings"] = new PortalSettings(
                this.PageID, this.portalSettings.PortalAlias);
            this.Page.portalSettings = (PortalSettings)this.Context.Items["PortalSettings"];

            // reorder the modules in the source pane
            sourceList = this.GetModules(sourcePane);
            OrderModules(sourceList);

            // resave the order
            foreach (ModuleItem item in sourceList)
            {
                admin.UpdateModuleOrder(item.ID, item.Order, sourcePane);
            }

            // reorder the modules in the target pane
            var targetList = this.GetModules(targetPane);
            OrderModules(targetList);

            // resave the order
            foreach (ModuleItem item in targetList)
            {
                admin.UpdateModuleOrder(item.ID, item.Order, targetPane);
            }

            // Redirect to the same page to pick up changes
            this.Page.Response.Redirect(AppendModuleId(this.Page.Request.RawUrl, this.ModuleID));
        }

        /// <summary>
        /// The UpDown_Click server event handler on this page is
        /// used to move a portal module up or down on a tab's layout pane
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UpDown_Click(object sender, EventArgs e)
        {
            // 			string cmd = ((ModuleButton)sender).CommandName;
            // 			string pane = ((ModuleButton)sender).CommandArgument;
            var cmd = ((ModuleButton)sender).Attributes["direction"];
            var pane = ((ModuleButton)sender).Attributes["pane"];

            var modules = this.GetModules(pane);

            // Determine the delta to apply in the order number for the module
            // within the list.  +3 moves down one item; -3 moves up one item
            int delta = cmd == "down" ? 3 : -3;

            foreach (ModuleItem item in modules.Cast<ModuleItem>().Where(item => item.ID == this.ModuleID))
            {
                item.Order += delta;
            }

            // reorder the modules in the content pane
            OrderModules(modules);

            // resave the order
            var admin = new ModulesDB();
            foreach (ModuleItem item in modules)
            {
                admin.UpdateModuleOrder(item.ID, item.Order, pane);
            }

            // Redirect to the same page to pick up changes
            this.Page.Response.Redirect(AppendModuleId(this.Page.Request.RawUrl, this.ModuleID));
        }

        // Nicholas Smeaton (24/07/2004) - Arrow button functions END
        #endregion

        #region Window Management functions

        /*
        // Added  - BJA [wjanderson@reedtek.com] [START]
        /// <summary>
        /// Set the close button attributes to prompt user before removing.
        /// </summary>
        /// <param name="delBtn">
        /// The del Btn.
        /// </param>
        private void SetDeleteAttributes(ref LinkButton delBtn)
        {
            // make sure javascript is valid and we have not already
            // added the function
            if (this.Page.Request.Browser.EcmaScriptVersion.Major >= 1 &&
                !this.Page.ClientScript.IsClientScriptBlockRegistered("confirmDelete"))
            {
                string[] s = { "CONFIRM_DELETE" };
                this.Page.ClientScript.RegisterClientScriptBlock(
                    this.GetType(), "confirmDelete", PortalSettings.GetStringResource("CONFIRM_DELETE_SCRIPT", s));
            }

            if (delBtn.Attributes["onclick"] != null)
            {
                delBtn.Attributes["onclick"] = "return confirmDelete();" + delBtn.Attributes["onclick"];
            }
            else
            {
                delBtn.Attributes.Add("onclick", "return confirmDelete();");
            }
        }
*/

        // end of setDeleteAttributes
        // Added - BJA [wjanderson@reedtek.com] [END]
        #endregion

        #region Delete Module functions

        /// <summary>
        /// Handles the Click event of the DeleteModuleButton control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void DeleteModuleButton_Click(object sender, EventArgs e)
        {
            var admin = new ModulesDB();

            // admin.DeleteModule(this.ModuleID);
            // TODO - add userEmail and useRecycler
            admin.DeleteModule(this.ModuleID);

            // Redirect to the same page to pick up changes
            this.Page.Response.Redirect(this.Page.Request.RawUrl);
        }

        #endregion

        #region Culture

        /// <summary>
        ///     The module culture. If specified module should be showed
        ///     only if current culture matches this setting.
        ///     Colon separated list
        /// </summary>
        public virtual string Cultures
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return this.Settings["MODULESETTINGS_CULTURE"].ToString();
                }
                else
                {
                    return Thread.CurrentThread.CurrentUICulture.Name;
                }
            }

            set
            {
                if (HttpContext.Current != null)
                {
                    this.Settings["MODULESETTINGS_CULTURE"] = value;
                }
            }
        }

        #endregion

        #region Content

        // Jes1111
        /// <summary>
        /// The content.
        /// </summary>
        private object content;

        /// <summary>
        ///     Will return module content as an object, if called.
        /// </summary>
        public object Content
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return "Module Content PlaceHolder";
                }
                
                if (this.content != null)
                {
                    return this.content;
                }
                else
                {
                    try
                    {
                        this.content = this.GetContent();
                        return this.content;
                    }
                    catch
                    {
                        return "error"; // TODO: change this
                    }
                }
            }

            set
            {
                this.content = value;
            }
        }

        /// <summary>
        /// Used by Content to fetch module content, by raising Init and Load events on the module.
        /// </summary>
        /// <returns>
        /// The get content.
        /// </returns>
        public virtual object GetContent()
        {
            this.OnInit(new EventArgs());
            this.OnLoad(new EventArgs());
            return this.Content;
        }

        #endregion

        #region Module Content Sizing

        // Added by gman3001: 2004/10/26 to support specific module content sizing and scrolling capabilities
        /// <summary>
        /// Returns a module content sizing container tag with properties
        /// </summary>
        /// <param name="isBeginTag">
        /// The is Begin Tag.
        /// </param>
        /// <paramref name="isBeginTag">Specifies whether to output the container's begin(true) or end(false) tag.</paramref>
        /// <returns>
        /// The literal control containing this tag
        /// </returns>
        private LiteralControl BuildModuleContentContainer(bool isBeginTag)
        {
            var modContainer = new LiteralControl();
            var width = (this.Settings["MODULESETTINGS_CONTENT_WIDTH"] != null)
                            ? Int32.Parse(this.Settings["MODULESETTINGS_CONTENT_WIDTH"].ToString())
                            : 0;
            var height = (this.Settings["MODULESETTINGS_CONTENT_HEIGHT"] != null)
                             ? Int32.Parse(this.Settings["MODULESETTINGS_CONTENT_HEIGHT"].ToString())
                             : 0;
            var scrolling = (this.Settings["MODULESETTINGS_CONTENT_SCROLLING"] != null)
                                ? bool.Parse(this.Settings["MODULESETTINGS_CONTENT_SCROLLING"].ToString())
                                : false;
            if (isBeginTag)
            {
                var startContentSizing = "<div class='modulePadding moduleScrollBars' id='modcont_" + this.ClientID +
                                         "' ";
                startContentSizing += " style='POSITION: static; ";
                if (!this.isPrint && width > 0)
                {
                    startContentSizing += "width: " + width + "px; ";
                }

                if (!this.isPrint && height > 0)
                {
                    startContentSizing += "height: " + height + "px; ";
                }

                if (!this.isPrint && scrolling)
                {
                    startContentSizing += "overflow:auto;";
                }

                startContentSizing += "'>";
                modContainer.Text = startContentSizing;
            }
            else
            {
                if (this.Page.Request.Browser.EcmaScriptVersion.Major >= 1 && !this.isPrint &&
                    (width > 0 || height > 0 || (width > 0 && scrolling) || (height > 0 && scrolling)))
                {
                    // Register a client side script that will properly resize the content area of the module
                    // to compensate for different height and width settings, as well as, the browser's tendency
                    // to stretch the middle module width even when a specific width setting is specified.
                    if (!this.Page.ClientScript.IsClientScriptBlockRegistered("autoSizeModules"))
                    {
                        var src = Path.ApplicationRootPath("aspnet_client/Appleseed_scripts/autoResizeModule.js");
                        this.Page.ClientScript.RegisterClientScriptBlock(
                            this.GetType(), "autoSizeModules", "<script language=javascript src='" + src + "'></script>");
                        this.Page.ClientScript.RegisterStartupScript(
                            this.GetType(), 
                            "initAutoSizeModules", 
                            "<script defer language=javascript>if (document._portalmodules) document._portalmodules.GetModules(_portalModules); document._portalmodules.ProcessAll();</script>");
                    }

                    this.Page.ClientScript.RegisterArrayDeclaration("_portalModules", "'modcont_" + this.ClientID + "'");
                }

                modContainer.Text = "</div>\r";
            }

            return modContainer;
        }

        // Added by gman3001: 2004/10/26 to support module width stretching/shrinking capability
        /// <summary>
        /// Updates the moduleControl literal control with proper width settings to render the 'module width stretching' option
        /// </summary>
        /// <param name="moduleControl">
        /// The module Control.
        /// </param>
        /// <param name="isBeginTag">
        /// The is Begin Tag.
        /// </param>
        /// <paramref name="moduleControl">The literal control element to parse and modify.</paramref>
        /// <paramref name="isBeginTag">Specifies whether the moduleElement parameter is for the element's begin(true) or end(false) tag.</paramref>
        private void ProcessModuleStrecthing(Control moduleControl, bool isBeginTag)
        {
            if (moduleControl is LiteralControl && moduleControl != null)
            {
                var moduleElement = (LiteralControl)moduleControl;
                var isStretched = this.Settings["MODULESETTINGS_WIDTH_STRETCHING"] != null &&
                                  bool.Parse(this.Settings["MODULESETTINGS_WIDTH_STRETCHING"].ToString());
                var tmp = (moduleElement.Text != null) ? moduleElement.Text.Trim() : string.Empty;

                // Need to remove the current width setting of the main table in the module Start(Title/NoTitle)Content section of the theme,
                // if this is to be a stretched module then insert a width attribute into it,
                // if not, then surround this table with another table that has an empty cell after the cell that contains the module's HTML,
                // in order to use up any space in the window that the module has not been defined for.
                // if, no width specified for module then the module will be at least 50% width of area remaining, or expand to hold its contents.
                if (isBeginTag)
                {
                    MatchCollection mc;
                    var r = new Regex("<table[^>]*>");
                    mc = r.Matches(tmp.ToLower());

                    // Only concerned with first match
                    if (mc.Count > 0)
                    {
                        var TMatch = mc[0].Value;
                        var TIndx = mc[0].Index;

                        // jminond - variable not in use
                        // int TLength = mc[0].Value.Length;
                        // find a width attribute in this match(if exists remove it)
                        var r1 = new Regex("width=((['\"][^'\"]*['\"])|([0-9]+))");
                        mc = r1.Matches(TMatch);
                        if (mc.Count > 0)
                        {
                            var WIndx = mc[0].Index;
                            var WLength = mc[0].Value.Length;
                            tmp = tmp.Substring(0, WIndx + TIndx) + tmp.Substring(WIndx + TIndx + WLength);
                            TMatch = TMatch.Substring(0, WIndx) + TMatch.Substring(WIndx + WLength);
                        }

                        // find a style attribute in this match(if exists)
                        var r2 = new Regex("style=['\"][^'\"]*['\"]");
                        mc = r2.Matches(TMatch);
                        if (mc.Count > 0)
                        {
                            var SIndx = mc[0].Index;

                            // jminond- variable not in use
                            // int SLength = mc[0].Value.Length;

                            // Next find a width style property(if exists) and modify it
                            var r3 = new Regex("width:[^;'\"]+[;'\"]");
                            mc = r3.Matches(mc[0].Value);
                            if (mc.Count > 0)
                            {
                                var SwIndx = mc[0].Index;
                                var SwLength = mc[0].Value.Length - 1;
                                if (isStretched)
                                {
                                    tmp = tmp.Substring(0, SIndx + SwIndx + TIndx) + "width:100%" +
                                          tmp.Substring(SIndx + SwIndx + TIndx + SwLength);
                                }
                                else
                                {
                                    tmp = tmp.Substring(0, SIndx + SwIndx + TIndx) +
                                          tmp.Substring(SIndx + SwIndx + TIndx + SwLength);
                                }
                            }
                                
                                // Else, Add width style property to the existing style attribute
                            else if (isStretched)
                            {
                                tmp = tmp.Substring(0, SIndx + TIndx + 7) + "width:100%;" +
                                      tmp.Substring(SIndx + TIndx + 7);
                            }
                        }
                            
                            // Else, Add width style property to a new style attribute
                        else if (isStretched)
                        {
                            tmp = tmp.Substring(0, TIndx + 7) + "style='width:100%' " + tmp.Substring(TIndx + 7);
                        }
                    }

                    if (!isStretched)
                    {
                        tmp = "<table cellpadding=0 cellspacing=0 border=0><tr>\n<td>" + tmp;
                    }
                }
                else if (!isStretched)
                {
                    tmp += "</td><td></td>\n</tr></table>";
                }

                moduleElement.Text = tmp;
            }
        }

        #endregion

        #region Theme/Rendering

        /// <summary>
        ///     Before apply theme DesktopModule designer checks this
        ///     property to be true.<br />
        ///     On inherited modules you can override this
        ///     property to not apply theme on specific controls.
        /// </summary>
        /// <value><c>true</c> if [apply theme]; otherwise, <c>false</c>.</value>
        public virtual bool ApplyTheme
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    // if it is not design time
                    return bool.Parse(this.Settings["MODULESETTINGS_APPLY_THEME"].ToString());
                }

                return true;
            }

            set
            {
                if (HttpContext.Current != null)
                {
                    // if it is not design time
                    this.Settings["MODULESETTINGS_APPLY_THEME"] = value.ToString();
                }
            }
        }

        // Added by james for localization purpose
        /// <summary>
        /// Localises Theme types: 'Default' and 'Alt'
        /// </summary>
        public enum ThemeList
        {
            /// <summary>
            /// The default.
            /// </summary>
            Default = 0, 

            /// <summary>
            /// The alternate.
            /// </summary>
            Alt = 1
        }

        /// <summary>
        ///     used to hold the consolidated list of buttons for the module
        /// </summary>
        private readonly ArrayList buttonList = new ArrayList(3);

        /// <summary>
        ///     User Buttons
        /// </summary>
        public ArrayList ButtonListUser = new ArrayList(3);

        /// <summary>
        ///     Admin Buttons
        /// </summary>
        public ArrayList ButtonListAdmin = new ArrayList(3);

        /// <summary>
        ///     Custom Buttons
        /// </summary>
        public ArrayList ButtonListCustom = new ArrayList(3);

        // switches used for building module hierarchy
        /// <summary>
        /// The _build title.
        /// </summary>
        private bool buildTitle = true;

        /// <summary>
        /// The _build buttons.
        /// </summary>
        private bool buildButtons = true;

        /// <summary>
        /// The _build body.
        /// </summary>
        private bool _buildBody = true;

        /// <summary>
        /// The _before content.
        /// </summary>
        private bool beforeContent = true;

        /// <summary>
        /// The _is print.
        /// </summary>
        private bool isPrint;

        /// <summary>
        /// Makes the decisions about what needs to be built and calls the appropriate method
        /// </summary>
        protected virtual void BuildControlHierarchy()
        {
            if (this.NamingContainer.ToString().EndsWith("ASP.print_aspx"))
            {
                this.isPrint = true;
                this.buildButtons = false;
                if (!this.ShowTitlePrint)
                {
                    this.buildTitle = false;
                }
            }
                // else if ( SupportCollapsable && UserDesktop.isMinimized( ModuleID ) ) {
                // _buildBody = false;
                // }
            else if (!this.ShowTitle)
            {
                this.buildTitle = false;
            }

            // added Jes1111: https://sourceforge.net/tracker/index.php?func=detail&aid=1034935&group_id=66837&atid=515929
            if (this.buttonList.Count == 0)
            {
                this.buildButtons = false;
            }

            // changed Jes1111 - 2004-09-29 - to correct shortcut behaviour
            if (this.ModuleID != this.OriginalModuleID)
            {
                this.BuildShortcut();
            }
            else if (!this.ApplyTheme || this.isPrint)
            {
                this.BuildNoTheme();
            }
            else if (this.CurrentTheme.Type.Equals("zen"))
            {
                this.ZenBuild();
            }
            else if (this.CurrentTheme.Type.Equals("htm"))
            {
                this.HtmBuild();
            }
            else
            {
                this.Build();
            }

            // wrap the module in a &lt;div&gt; with the ModuleID and Module type name - needed for Zen support and useful for CSS styling and debugging ;-)
            // Added generic classname ModuleWrap useful for more generic CSS styling - Rob Siera 2004-12-30
            this._header.Controls.AddAt(
                0, 
                new LiteralControl(
                    string.Format("<div id=\"mID{0}\" class=\"{1} ModuleWrap\">", this.ModuleID, this.GetType().Name)));
            this._footer.Controls.Add(new LiteralControl("</div>"));
        }

        /// <summary>
        /// Builds the "with theme" versions of the module using html, with optional Title, Buttons and Body.
        /// </summary>
        protected virtual void HtmBuild()
        {
            var template = this.CurrentTheme.GetThemePart("ModuleLayout");
            
            template = template.Replace("{Title}", this.TitleText);
            template = template.Replace("{TitleRowStyle}", this.ShowTitle ? "display:inline" : "display:none");
            template = template.Replace("{BodyBgColor}", this.CurrentTheme.GetThemePart("DefaultBodyBgColor"));
            template = template.Replace("{TitleBgColor}", this.CurrentTheme.GetThemePart("DefaultTitleBgColor"));
            
            var iCtr = template.IndexOf("{ControlPanel}");
            var iBdy = template.IndexOf("{Body}");
            var iMby = template.IndexOf("{ModifiedBy}");

            if (iCtr < iBdy)
            {
                if (iCtr != -1)
                {
                    // Both Ctrl & Body : ....Ctrl....Body.....
                    this._header.Controls.Add(new LiteralControl(template.Substring(0, iCtr)));
                    this.HtmRenderButtons(this._header);
                    this._header.Controls.Add(new LiteralControl(template.Substring(iCtr + 14, iBdy - (iCtr + 14))));

                    // base.Render(output);
                    this._footer.Controls.Add(new LiteralControl(template.Substring(iBdy + 6)));
                }
                else
                {
                    if (iBdy != -1)
                    {
                        // Only Body: ...Body...
                        this._header.Controls.Add(new LiteralControl(template.Substring(0, iBdy)));

                        // base.Render(output);
                        this._footer.Controls.Add(new LiteralControl(template.Substring(iBdy + 6)));
                    }
                    else
                    {
                        // No Ctrl No Body...
                        // base.Render(output);
                    }
                }
            }
            else
            {
                if (iBdy != -1)
                {
                    // Both Ctrl & Body : ....Body....Ctrl.....
                    this._header.Controls.Add(new LiteralControl(template.Substring(0, iBdy)));

                    // base.Render(output);
                    this._footer.Controls.Add(new LiteralControl(template.Substring(iBdy + 6, iCtr - (iBdy + 6))));
                    this.HtmRenderButtons(this._footer);
                    this._footer.Controls.Add(new LiteralControl(template.Substring(iCtr + 14)));
                }
                else
                {
                    if (iCtr != -1)
                    {
                        // Only Ctrl: ...Ctrl...
                        this._header.Controls.Add(new LiteralControl(template.Substring(0, iCtr)));
                        this.HtmRenderButtons(this._header);
                        this._footer.Controls.Add(new LiteralControl(template.Substring(iCtr + 14)));
                    }
                    else
                    {
                        // No Ctrl No Body...
                        // base.Render(output);
                    }
                }
            }
        }

        /// <summary>
        /// The htm render buttons.
        /// </summary>
        /// <param name="placeHolder">
        /// The place holder.
        /// </param>
        private void HtmRenderButtons(PlaceHolder placeHolder)
        {
            if (this.ShareModule)
            {
                var publisherkeysetting = this.portalSettings.CustomSettings["SITESETTINGS_ADDTHIS_USERNAME"];
                if (publisherkeysetting != null)
                {
                    if (Convert.ToString(publisherkeysetting).Trim().Length > 0)
                    {
                        var culture = Thread.CurrentThread.CurrentUICulture.Name;
                        var sb = new StringBuilder();
                        sb.Append(
                            "<script type=\"text/javascript\">var addthis_config = {\"data_track_clickback\":true, ");
                        sb.AppendFormat("ui_language:\"{0}\"", culture);
                        sb.Append("};</script>");
                        sb.Append("<div class=\"addthis_toolbox addthis_default_style\">");
                        sb.AppendFormat(
                            " <a href=\"http://www.addthis.com/bookmark.php?v=250&amp;username={0}\"  class=\"addthis_button_compact\">{1}</a>", 
                            publisherkeysetting, 
                            General.GetString("SHARE", "Share"));
                        sb.Append(" <span class=\"addthis_separator\">|</span>");
                        sb.Append(" <a class=\"addthis_button_facebook\"></a>");
                        sb.Append(" <a class=\"addthis_button_twitter\"></a>");
                        sb.Append(" <a class=\"addthis_button_myspace\"></a>");
                        sb.Append("</div>");

                        placeHolder.Controls.Add(new LiteralControl(sb.ToString()));
                    }
                }
            }

            if (this.buildButtons)
            {
                foreach (Control button in this.buttonList)
                {
                    placeHolder.Controls.Add(this.CurrentTheme.GetLiteralControl("TitleBeforeButton"));
                    placeHolder.Controls.Add(button);
                    placeHolder.Controls.Add(this.CurrentTheme.GetLiteralControl("TitleAfterButton"));
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
            var t = new Table();
            t.Attributes.Add("width", "100%");
            t.CssClass = "TitleNoTheme";
            var tr = new TableRow();
            t.Controls.Add(tr);
            TableCell tc;

            if (this.buildTitle)
            {
                tc = new TableCell();
                tc.Attributes.Add("width", "100%");
                tc.CssClass = "TitleNoTheme";
                tc.Controls.Add(new LiteralControl(this.TitleText));
                tr.Controls.Add(tc);
            }

            if (this.buildButtons)
            {
                foreach (Control button in this.buttonList)
                {
                    tc = new TableCell();
                    tc.Controls.Add(button); // Add Button
                    tr.Controls.Add(tc);
                }
            }

            if (this.buildTitle || this.buildButtons)
            {
                this._header.Controls.Add(t);
            }

            if (!this._buildBody)
            {
                // for ( int i = 1 ; i < this.Controls.Count - 1 ; i++ ) // Jes1111 - was missing last control
                for (var i = 1; i < this.Controls.Count; i++)
                {
                    this.Controls[i].Visible = false;
                }
            }
            else
            {
                this._footer.Controls.Add(new LiteralControl(this.GetLastModified()));
            }
        }

        /// <summary>
        /// Builds the "with theme" versions of the module, with optional Title, Buttons and Body.
        /// </summary>
        protected virtual void Build()
        {
            if (!this.buildTitle && !this.buildButtons)
            {
                this._header.Controls.Add(this.CurrentTheme.GetLiteralControl("ControlNoTitleStart"));
            }
            else
            {
                this._header.Controls.Add(this.CurrentTheme.GetLiteralControl("ControlTitleStart"));

                this._header.Controls.Add(this.CurrentTheme.GetLiteralControl("TitleStart"));

                if (this.buildTitle)
                {
                    this._header.Controls.Add(new LiteralControl(this.TitleText));
                }

                this._header.Controls.Add(this.CurrentTheme.GetLiteralControl("TitleMiddle"));

                if (this.buildButtons)
                {
                    foreach (Control button in this.buttonList)
                    {
                        this._header.Controls.Add(this.CurrentTheme.GetLiteralControl("TitleBeforeButton"));
                        this._header.Controls.Add(button);
                        this._header.Controls.Add(this.CurrentTheme.GetLiteralControl("TitleAfterButton"));
                    }
                }

                this._header.Controls.Add(this.CurrentTheme.GetLiteralControl("TitleEnd"));
            }

            if (!this._buildBody)
            {
                for (var i = 1; i < this.Controls.Count; i++)
                {
                    this.Controls[i].Visible = false;
                }
            }
            else
            {
                this._footer.Controls.Add(new LiteralControl(this.GetLastModified()));
            }

            // Added by gman3001: 2004/10/26 to support auto width sizing and scrollable module content
            // this must be the first footer control (besides custom ones such as GetLastModified)
            if (this._buildBody)
            {
                this._footer.Controls.Add(this.BuildModuleContentContainer(false));
            }

            // changed Jes1111: https://sourceforge.net/tracker/index.php?func=detail&aid=1034935&group_id=66837&atid=515929
            if (!this.buildTitle && !this.buildButtons)
            {
                this._footer.Controls.Add(this.CurrentTheme.GetLiteralControl("ControlNoTitleEnd"));
            }
            else
            {
                this._header.Controls.Add(this.CurrentTheme.GetLiteralControl("ControlTitleBeforeControl"));

                // Changed Rob Siera: Incorrect positioning of ControlTitleAfterControl
                // this._footer.Controls.AddAt(0, CurrentTheme.GetLiteralControl("ControlTitleAfterControl"));
                this._footer.Controls.Add(this.CurrentTheme.GetLiteralControl("ControlTitleAfterControl"));
                this._footer.Controls.Add(this.CurrentTheme.GetLiteralControl("ControlTitleEnd"));
            }

            // Added by gman3001: 2004/10/26 to support auto width sizing and scrollable module content
            // this must be the last header control
            if (this._buildBody)
            {
                this._header.Controls.Add(this.BuildModuleContentContainer(true));
            }

            if (!this.isPrint && this._header.Controls.Count > 0 && this._footer.Controls.Count > 0)
            {
                // Process the first header control as the module's outer most begin tag element
                this.ProcessModuleStrecthing(this._header.Controls[0], true);

                // Process the last footer control as the module's outer most end tag element
                this.ProcessModuleStrecthing(this._footer.Controls[this._footer.Controls.Count - 1], false);
            }

            // End add by gman3001
        }

        /// <summary>
        /// The Zen version of Build(). Parses XML Zen Module Layout.
        /// </summary>
        protected virtual void ZenBuild()
        {
            XmlTextReader xtr = null;
            var nt = new NameTable();
            var nsm = new XmlNamespaceManager(nt);
            nsm.AddNamespace(string.Empty, "http://www.w3.org/1999/xhtml");
            nsm.AddNamespace("if", "urn:MarinaTeq.Appleseed.Zen.Condition");
            nsm.AddNamespace("loop", "urn:Marinateq.Appleseed.Zen.Looping");
            nsm.AddNamespace("content", "urn:www.Appleseedportal.net");
            var context = new XmlParserContext(nt, nsm, string.Empty, XmlSpace.None);
            StringBuilder fragText;

            try
            {
                xtr = new XmlTextReader(this.CurrentTheme.GetThemePart("ModuleLayout"), XmlNodeType.Document, context);

                while (xtr.Read())
                {
                    LiteralControl frag = new LiteralControl();
                    switch (xtr.Prefix)
                    {
                        case "if":
                            {
                                if (xtr.NodeType == XmlNodeType.Element && !this.ZenEvaluate(xtr.LocalName))
                                {
                                    xtr.Skip();
                                }

                                break;
                            }

                        case "loop":
                            {
                                if (xtr.NodeType == XmlNodeType.Element)
                                {
                                    switch (xtr.LocalName)
                                    {
                                        case "Buttons":
                                            {
                                                // Menu btnMenu = new Menu();
                                                // btnMenu.Orientation = Orientation.Vertical;
                                                // btnMenu.StaticDisplayLevels = 1;
                                                // btnMenu.DisappearAfter = 500;
                                                // btnMenu.DynamicHorizontalOffset = 10;

                                                /*
                                                //btnMenu.StaticMenuStyle.CssClass = "CommandButton";
                                                btnMenu.StaticMenuItemStyle.CssClass = "CommandButton";
                                                
                                                btnMenu.DynamicMenuItemStyle.CssClass = "CommandButton";
                                                //btnMenu.DynamicHoverStyle.CssClass = "CommandButton";

                                                btnMenu.DynamicSelectedStyle.CssClass = "CommandButton";
                                                */

                                                // MenuItem rootNode = new MenuItem("Menu");
                                                // rootNode.ImageUrl = CurrentTheme.GetImage("Navlink", "icon/NavLink.gif").ImageUrl;
                                                // rootNode.ToolTip = "Module Control and Options Menu";
                                                // rootNode.Selected = true;
                                                string loopFrag = xtr.ReadInnerXml();
                                                foreach (Control c in this.buttonList)
                                                {
                                                    // ModuleButton mb = (ModuleButton)c;
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

                                                    XmlTextReader xtr2 = new XmlTextReader(loopFrag, XmlNodeType.Document, context);
                                                    while (xtr2.Read())
                                                    {
                                                        frag = new LiteralControl();
                                                        switch (xtr2.Prefix)
                                                        {
                                                            case "content":
                                                                {
                                                                    switch (xtr2.LocalName)
                                                                    {
                                                                        case "Button":

                                                                            // 																if ( this.CurrentTheme.Name.ToLower().Equals("zen-zero") && c is ModuleButton )
                                                                            // 																	((ModuleButton)c).RenderAs = ModuleButton.RenderOptions.TextOnly;
                                                                            // 																if ( _beforeContent )
                                                                            this._header.Controls.Add(c);

                                                                            // 																else
                                                                            // 																	this._footer.Controls.Add(c);
                                                                            break;
                                                                        default:
                                                                            break;
                                                                    }

                                                                    break;
                                                                }

                                                            // case "":
                                                            default:
                                                                {
                                                                    if (xtr2.NodeType == XmlNodeType.Element)
                                                                    {
                                                                        fragText = new StringBuilder("<");
                                                                        fragText.Append(xtr2.LocalName);
                                                                        while (xtr2.MoveToNextAttribute())
                                                                        {
                                                                            if (xtr2.LocalName != "xmlns")
                                                                            {
                                                                                fragText.Append(" ");
                                                                                fragText.Append(xtr.LocalName);
                                                                                fragText.Append("=\"");
                                                                                fragText.Append(xtr.Value);
                                                                                fragText.Append("\"");
                                                                            }
                                                                        }

                                                                        fragText.Append(">");
                                                                        frag.Text = fragText.ToString();
                                                                        if (this.beforeContent)
                                                                        {
                                                                            this._header.Controls.Add(frag);
                                                                        }
                                                                        else
                                                                        {
                                                                            this._footer.Controls.Add(frag);
                                                                        }
                                                                    }
                                                                    else if (xtr2.NodeType == XmlNodeType.EndElement)
                                                                    {
                                                                        frag.Text = string.Format(
                                                                            "</{0}>", xtr2.LocalName);
                                                                        if (this.beforeContent)
                                                                        {
                                                                            this._header.Controls.Add(frag);
                                                                        }
                                                                        else
                                                                        {
                                                                            this._footer.Controls.Add(frag);
                                                                        }
                                                                    }

                                                                    break;
                                                                }
                                                        }
 // end switch
                                                    }
 // end while
                                                }
                                                // end foreach

                                                // btnMenu.Items.Add(rootNode);

                                                // this._header.Controls.Add(btnMenu);
                                                break;
                                            }

                                        default:
                                            break;
                                    }

 // end inner switch
                                }

                                break;
                            }

                        case "content":
                            {
                                switch (xtr.LocalName)
                                {
                                    case "ModuleContent":
                                        this.beforeContent = false;
                                        break;
                                    case "TitleText":
                                        frag.Text = this.TitleText;
                                        if (this.beforeContent)
                                        {
                                            this._header.Controls.Add(frag);
                                        }
                                        else
                                        {
                                            this._footer.Controls.Add(frag);
                                        }

                                        break;
                                    case "ModifiedBy":
                                        frag.Text = this.GetLastModified();
                                        if (this.beforeContent)
                                        {
                                            this._header.Controls.Add(frag);
                                        }
                                        else
                                        {
                                            this._footer.Controls.Add(frag);
                                        }

                                        break;
                                    default:
                                        break;
                                }

                                break;
                            }

                        // case "":
                        default:
                            {
                                if (xtr.NodeType == XmlNodeType.Element)
                                {
                                    fragText = new StringBuilder("<");
                                    fragText.Append(xtr.LocalName);
                                    while (xtr.MoveToNextAttribute())
                                    {
                                        fragText.Append(" ");
                                        fragText.Append(xtr.LocalName);
                                        fragText.Append("=\"");
                                        fragText.Append(xtr.Value);
                                        fragText.Append("\"");
                                    }

                                    fragText.Append(">");
                                    frag.Text = fragText.ToString();
                                    if (this.beforeContent)
                                    {
                                        this._header.Controls.Add(frag);
                                    }
                                    else
                                    {
                                        this._footer.Controls.Add(frag);
                                    }
                                }
                                else if (xtr.NodeType == XmlNodeType.EndElement)
                                {
                                    frag.Text = string.Format("</{0}>", xtr.LocalName);
                                    if (this.beforeContent)
                                    {
                                        this._header.Controls.Add(frag);
                                    }
                                    else
                                    {
                                        this._footer.Controls.Add(frag);
                                    }
                                }

                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Publish(LogLevel.Fatal, "Fatal error in ZenBuildControlHierarchy(): " + ex.Message);
                throw new Exception("Fatal error in ZenBuildControlHierarchy(): " + ex.Message);
            }
            finally
            {
                if (xtr != null)
                {
                    xtr.Close();
                }
            }
        }

        /// <summary>
        /// Supports ZenBuild(), evaluates 'if' commands
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <returns>
        /// The zen evaluate.
        /// </returns>
        private bool ZenEvaluate(string condition)
        {
            var returnVal = false;

            switch (condition)
            {
                case "Title":
                    if (this.buildTitle)
                    {
                        returnVal = true;
                    }

                    break;
                case "Buttons":

                    // if ( _buildButtons && ButtonList.Count > 0 )
                    if (this.buildButtons)
                    {
                        returnVal = true;
                    }

                    break;
                case "Body":
                case "Footer":
                    if (this._buildBody)
                    {
                        returnVal = true;
                    }
                    else
                    {
                        returnVal = false;

                        // for ( int i = 1 ; i < this.Controls.Count - 1 ; i++ ) // Jes1111 - was missing last control
                        for (var i = 1; i < this.Controls.Count; i++)
                        {
                            this.Controls[i].Visible = false;
                        }
                    }

                    break;
                case "ShowModifiedBy":
                    if (
                        bool.Parse(
                            ((SettingItem)this.portalSettings.CustomSettings["SITESETTINGS_SHOW_MODIFIED_BY"]).Value) &&
                        bool.Parse(((SettingItem)this.Settings["MODULESETTINGS_SHOW_MODIFIED_BY"]).Value))
                    {
                        returnVal = true;
                    }

                    break;
                default:
                    returnVal = false;
                    break;
            }

            return returnVal;
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
            themeName = Int32.Parse(this.Settings["MODULESETTINGS_THEME"].ToString()) == (int)ThemeList.Alt ? "Alt" : "Default";

            // end: Jes1111

            // added: Jes1111 - 2004-08-05 - supports custom theme per module
            if (this.portalSettings.CustomSettings.ContainsKey("SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES") &&
                this.portalSettings.CustomSettings["SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES"].ToString().Length != 0 &&
                bool.Parse(this.portalSettings.CustomSettings["SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES"].ToString()) &&
                this.Settings.ContainsKey("MODULESETTINGS_MODULE_THEME") &&
                this.Settings["MODULESETTINGS_MODULE_THEME"].ToString().Trim().Length > 0)
            {
                // substitute custom theme for this module
                var tm = new ThemeManager(this.portalSettings.PortalPath);
                tm.Load(this.Settings["MODULESETTINGS_MODULE_THEME"].ToString());
                this.CurrentTheme = tm.CurrentTheme;

                // get CSS file, add ModuleID to each line and add resulting string to CssImportList
                try
                {
                    var cssHelper = new CssHelper();
                    var selectorPrefix = string.Concat("#mID", this.ModuleID);
                    var cssFileName = this.Page.Server.MapPath(this.CurrentTheme.CssFile);
                    this.Page.RegisterCssImport(
                        this.ModuleID.ToString(), cssHelper.ParseCss(cssFileName, selectorPrefix));
                }
                catch (Exception ex)
                {
                    ErrorHandler.Publish(
                        LogLevel.Error, 
                        string.Format("Failed to load custom theme '{0}' for ModuleID {1}. Continuing with default tab theme. Message was: {2}", this.CurrentTheme.CssFile, this.ModuleID, ex.Message));
                    this.CurrentTheme = this.portalSettings.GetCurrentTheme(themeName);
                }
            }
            else
            {
                // original behaviour unchanged
                this.CurrentTheme = this.portalSettings.GetCurrentTheme(themeName);
            }

            // end change: Jes1111
        }

        /// <summary>
        /// Merges the three public button lists into one.
        /// </summary>
        protected virtual void MergeButtonLists()
        {
            if (this.CurrentTheme.Type != "zen")
            {
                string divider;
                try
                {
                    divider = this.CurrentTheme.GetHTMLPart("ButtonGroupsDivider");
                }
                catch
                {
                    divider = string.Concat(
                        "<img src='", 
                        this.CurrentTheme.GetImage("Spacer", "Spacer.gif").ImageUrl, 
                        "' class='rb_mod_title_sep'/>");
                }

                // merge the button lists
                if (this.ButtonListUser.Count > 0 && (this.ButtonListCustom.Count > 0 || this.ButtonListAdmin.Count > 0))
                {
                    this.ButtonListUser.Add(new LiteralControl(divider));
                }

                if (this.ButtonListCustom.Count > 0 && this.ButtonListAdmin.Count > 0)
                {
                    this.ButtonListCustom.Add(new LiteralControl(divider));
                }
            }

            foreach (Control btn in this.ButtonListUser)
            {
                this.buttonList.Add(btn);
            }

            foreach (Control btn in this.ButtonListAdmin)
            {
                this.buttonList.Add(btn);
            }

            foreach (Control btn in this.ButtonListCustom)
            {
                this.buttonList.Add(btn);
            }
        }

        /// <summary>
        /// Builds the three public button lists
        /// </summary>
        protected virtual void BuildButtonLists()
        {
            // user buttons
            if (this.BackButton != null)
            {
                this.ButtonListUser.Add(this.BackButton);
            }

            if (this.PrintButton != null)
            {
                this.ButtonListUser.Add(this.PrintButton);
            }

            if (this.HelpButton != null)
            {
                this.ButtonListUser.Add(this.HelpButton);
            }

            // admin buttons
            if (this.AddButton != null)
            {
                this.ButtonListAdmin.Add(this.AddButton);
            }

            if (this.EditButton != null)
            {
                this.ButtonListAdmin.Add(this.EditButton);
            }

            if (this.DeleteModuleButton != null)
            {
                this.ButtonListAdmin.Add(this.DeleteModuleButton);
            }

            if (this.PropertiesButton != null)
            {
                this.ButtonListAdmin.Add(this.PropertiesButton);
            }

            if (this.SecurityButton != null)
            {
                this.ButtonListAdmin.Add(this.SecurityButton);
            }

            if (this.VersionButton != null)
            {
                this.ButtonListAdmin.Add(this.VersionButton);
            }

            if (this.PublishButton != null)
            {
                this.ButtonListAdmin.Add(this.PublishButton);
            }

            if (this.ApproveButton != null)
            {
                this.ButtonListAdmin.Add(this.ApproveButton);
            }

            if (this.RejectButton != null)
            {
                this.ButtonListAdmin.Add(this.RejectButton);
            }

            if (this.ReadyToApproveButton != null)
            {
                this.ButtonListAdmin.Add(this.ReadyToApproveButton);
            }

            if (this.RevertButton != null)
            {
                this.ButtonListAdmin.Add(this.RevertButton);
            }

            if (this.UpButton != null)
            {
                this.ButtonListAdmin.Add(this.UpButton);
            }

            if (this.DownButton != null)
            {
                this.ButtonListAdmin.Add(this.DownButton);
            }

            if (this.LeftButton != null)
            {
                this.ButtonListAdmin.Add(this.LeftButton);
            }

            if (this.RightButton != null)
            {
                this.ButtonListAdmin.Add(this.RightButton);
            }

            // custom buttons
            // recover any CustomButtons set the 'old way'
            if (this.ModuleTitle != null)
            {
                foreach (Control c in this.ModuleTitle.CustomButtons)
                {
                    this.ButtonListCustom.Add(c);
                }
            }

            // if ( MinMaxButton != null )
            // ButtonListCustom.Add( MinMaxButton );
            // if ( CloseButton != null )
            // ButtonListCustom.Add( CloseButton );

            // set image url for standard buttons edit & delete
            if (this.DeleteBtn != null)
            {
                this.DeleteBtn.ImageUrl = this.CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl;
            }

            if (this.EditBtn != null)
            {
                this.EditBtn.ImageUrl = this.CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl;
            }
        }

        /// <summary>
        /// Save module data
        /// </summary>
        /// <param name="ds">The ds.</param>
        /// <returns></returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual DataSet SaveData(DataSet ds)
        {
            return ds;
        }

        /// <summary>
        /// Load Data
        /// </summary>
        /// <param name="ds">The ds.</param>
        /// <returns></returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual DataSet LoadData(DataSet ds)
        {
            return ds;
        }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver">
        /// </param>
        public virtual void Install(IDictionary stateSaver)
        {
        }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver">
        /// </param>
        public virtual void Uninstall(IDictionary stateSaver)
        {
        }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver">The state saver.</param>
        public virtual void Commit(IDictionary stateSaver)
        {
        }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver">The state saver.</param>
        public virtual void Rollback(IDictionary stateSaver)
        {
        }

        #endregion
    }
}