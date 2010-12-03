using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Appleseed.Framework;
using Appleseed.Framework.Configuration;
using Appleseed.Framework.Users.Data;
using Appleseed.Framework.Exceptions;
using Appleseed.Framework.Design;
using Appleseed.Framework.Scheduler;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Settings.Cache;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;
using Path = Appleseed.Framework.Settings.Path;
using Appleseed.Framework.Site.Data;


namespace Appleseed.Framework.Site.Configuration
{
    /// <summary>
    /// PortalSettings Class encapsulates all of the settings 
    /// for the Portal, as well as the configuration settings required 
    /// to execute the current tab view within the portal.
    /// </summary>
    [History("jminond", "2005/03/10", "Tab to page conversion")]
    [History("gman3001", "2004/09/29", "Added the GetCurrentUserProfile method to obtain a hashtable of the current user's profile details.")]
    [History("jviladiu@portalServices.net", "2004/08/19", "Add support for move & delete module roles")]
    [History("jviladiu@portalServices.net", "2004/07/30", "Added new ActiveModule property")]
    [History("Jes1111", "2003/03/09", "Added new ShowTabs property")]
    [History("Jes1111", "2003/04/02", "Added new DesktopTabsXml property (an XPathDocument)")]
    [History("Thierry", "2003/04/12", "Added PortalSecurePath property")]
    [History("Jes1111", "2003/04/17", "Added new language-related properties and methods")]
    [History("Jes1111", "2003/04/23", "Corrected string comparison case problem in language settings")]
    [History("cisakson@yahoo.com", "2003/04/28", "Added a custom setting for Windows users to assign a portal Admin")]
    public class PortalSettings
    {
        #region local constants
        private const string strATPortalID = "@PortalID";
        private const string strATPageID = "@PageID";
        private const string strCustomLayout = "CustomLayout";
        private const string strCustomTheme = "CustomTheme";
        private const string strName = "Name";
        #endregion

        #region private members
        private PageSettings _activePage = new PageSettings();
        private Hashtable _customSettings;
        private ArrayList _desktopPages = new ArrayList();
        //private XPathDocument _desktopPagesXml;
        private ArrayList _mobilePages = new ArrayList();
        private string _portalAlias;
        private int _portalID;
        private string _portalName;
        private string _portalTitle;
        private bool _showPages = true;
        private string _currentLayout;
        private string _portalPath = string.Empty;
        private string _portalPathPrefix = HttpContext.Current.Request.ApplicationPath == "/" ? string.Empty : HttpContext.Current.Request.ApplicationPath;
        private string _portalSecurePath;
        private XmlDocument _portalPagesXml;
        private Theme _currentThemeAlt;
        private Theme _currentThemeDefault;
        private static CultureInfo[] _AppleseedCultures = null;
        private static IScheduler scheduler; // Federico (ifof@libero.it) 18 jun 2003
        //private bool alwaysShowEditButton;

        /// <summary>
        /// Gets the Appleseed cultures.
        /// </summary>
        /// <value>The Appleseed cultures.</value>
        private static CultureInfo[] AppleseedCultures
        {
            get
            {
                var locker = new object();
                lock (locker)
                {
                    if (_AppleseedCultures == null || _AppleseedCultures.Length == 0)
                    {
                        string[] cultures = Config.DefaultLanguageList.Split(new char[] { ';' });

                        ArrayList AppleseedCulturesArray = new ArrayList();
                        foreach (string culture in cultures)
                        {
                            AppleseedCulturesArray.Add(new CultureInfo(culture));
                        }
                        _AppleseedCultures = new CultureInfo[AppleseedCulturesArray.Count];
                        AppleseedCulturesArray.CopyTo(_AppleseedCultures);
                    }
                    return _AppleseedCultures;
                }
            }
        }

        #endregion

        #region constructors
        /// <summary>
        /// The PortalSettings Constructor encapsulates all of the logic
        /// necessary to obtain configuration settings necessary to render
        /// a Portal Page view for a given request.<br/>
        /// These Portal Settings are stored within a SQL database, and are
        /// fetched below by calling the "GetPortalSettings" stored procedure.<br/>
        /// This stored procedure returns values as SPROC output parameters,
        /// and using three result sets.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="portalAlias">The portal alias.</param>
        public PortalSettings(int pageID, string portalAlias)
        {
            // Changes culture/language according to settings
			try
			{
                //Moved here for support db call
                Appleseed.Framework.Web.UI.WebControls.LanguageSwitcher.ProcessCultures(GetLanguageList(portalAlias), portalAlias);
			}

			catch (Exception ex)
			{
                ErrorHandler.Publish(Appleseed.Framework.LogLevel.Warn, "Failed to load languages, loading defaults.", ex); // Jes1111
                Appleseed.Framework.Web.UI.WebControls.LanguageSwitcher.ProcessCultures(Localization.LanguageSwitcher.LANGUAGE_DEFAULT, portalAlias);
            }

            // Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_GetPortalSettings", myConnection))
				{
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalAlias = new SqlParameter("@PortalAlias", SqlDbType.NVarChar, 128);
                    parameterPortalAlias.Value = portalAlias; // Specify the Portal Alias Dynamically 
                    myCommand.Parameters.Add(parameterPortalAlias);
                    SqlParameter parameterPageID = new SqlParameter(strATPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    SqlParameter parameterPortalLanguage = new SqlParameter("@PortalLanguage", SqlDbType.NVarChar, 12);
                    parameterPortalLanguage.Value = this.PortalContentLanguage.Name;
                    myCommand.Parameters.Add(parameterPortalLanguage);
                    // Add out parameters to Sproc
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPortalID);
                    SqlParameter parameterPortalName = new SqlParameter("@PortalName", SqlDbType.NVarChar, 128);
                    parameterPortalName.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPortalName);
                    SqlParameter parameterPortalPath = new SqlParameter("@PortalPath", SqlDbType.NVarChar, 128);
                    parameterPortalPath.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPortalPath);
                    SqlParameter parameterEditButton = new SqlParameter("@AlwaysShowEditButton", SqlDbType.Bit, 1);
                    parameterEditButton.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterEditButton);
                    SqlParameter parameterPageName = new SqlParameter("@PageName", SqlDbType.NVarChar, 50);
                    parameterPageName.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPageName);
                    SqlParameter parameterPageOrder = new SqlParameter("@PageOrder", SqlDbType.Int, 4);
                    parameterPageOrder.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPageOrder);
                    SqlParameter parameterParentPageID = new SqlParameter("@ParentPageID", SqlDbType.Int, 4);
                    parameterParentPageID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterParentPageID);
                    SqlParameter parameterMobilePageName = new SqlParameter("@MobilePageName", SqlDbType.NVarChar, 50);
                    parameterMobilePageName.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterMobilePageName);
                    SqlParameter parameterAuthRoles = new SqlParameter("@AuthRoles", SqlDbType.NVarChar, 512);
                    parameterAuthRoles.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterAuthRoles);
                    SqlParameter parameterShowMobile = new SqlParameter("@ShowMobile", SqlDbType.Bit, 1);
                    parameterShowMobile.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterShowMobile);
                    SqlDataReader result;

					try
					{
                        // Open the database connection and execute the command
                        //						try // jes1111
                        //						{
                        myConnection.Open();
                        result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                        this.CurrentLayout = "Default";

                        // Read the first resultset -- Desktop Page Information
						while (result.Read())
						{
                            PageStripDetails tabDetails = new PageStripDetails();
                            tabDetails.PageID = (int)result["PageID"];
                            tabDetails.ParentPageID = Int32.Parse("0" + result["ParentPageID"]);
                            tabDetails.PageName = (string)result["PageName"];
                            tabDetails.PageOrder = (int)result["PageOrder"];
                            tabDetails.PageLayout = this.CurrentLayout;
                            tabDetails.AuthorizedRoles = (string)result["AuthorizedRoles"];
                            this.PortalAlias = portalAlias;
                            // Update the AuthorizedRoles Variable
                            this.DesktopPages.Add(tabDetails);
                        }


						if (DesktopPages.Count == 0)
						{
                            return; //Abort load
                            //throw new Exception("The portal you requested has no Pages. PortalAlias: '" + portalAlias + "'", new HttpException(404, "Portal not found"));
                        }
                        // Read the second result --  Mobile Page Information
                        result.NextResult();

						while (result.Read())
						{
                            PageStripDetails tabDetails = new PageStripDetails();
                            tabDetails.PageID = (int)result["PageID"];
                            tabDetails.PageName = (string)result["MobilePageName"];
                            tabDetails.PageLayout = this.CurrentLayout;
                            tabDetails.AuthorizedRoles = (string)result["AuthorizedRoles"];
                            this.MobilePages.Add(tabDetails);
                        }
                        // Read the third result --  Module Page Information
                        result.NextResult();
                        object myValue;

						while (result.Read())
						{
                            ModuleSettings m = new ModuleSettings();
                            m.ModuleID = (int)result["ModuleID"];
                            m.ModuleDefID = (int)result["ModuleDefID"];
                            m.GuidID = (Guid)result["GeneralModDefID"];
                            m.PageID = (int)result["TabID"];
                            m.PaneName = (string)result["PaneName"];
                            m.ModuleTitle = (string)result["ModuleTitle"];
                            myValue = result["AuthorizedEditRoles"];
                            m.AuthorizedEditRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedViewRoles"];
                            m.AuthorizedViewRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedAddRoles"];
                            m.AuthorizedAddRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedDeleteRoles"];
                            m.AuthorizedDeleteRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedPropertiesRoles"];
                            m.AuthorizedPropertiesRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            // jviladiu@portalServices.net (19/08/2004) Add support for move & delete module roles
                            myValue = result["AuthorizedMoveModuleRoles"];
                            m.AuthorizedMoveModuleRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["AuthorizedDeleteModuleRoles"];
                            m.AuthorizedDeleteModuleRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            // Change by Geert.Audenaert@Syntegra.Com
                            // Date: 6/2/2003
                            myValue = result["AuthorizedPublishingRoles"];
                            m.AuthorizedPublishingRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["SupportWorkflow"];
                            m.SupportWorkflow = !Convert.IsDBNull(myValue) ? (bool)myValue : false;
                            // Date: 27/2/2003
                            myValue = result["AuthorizedApproveRoles"];
                            m.AuthorizedApproveRoles = !Convert.IsDBNull(myValue) ? (string)myValue : string.Empty;
                            myValue = result["WorkflowState"];
                            m.WorkflowStatus = !Convert.IsDBNull(myValue) ? (WorkflowState)(0 + (byte)myValue) : WorkflowState.Original;

                            // End Change Geert.Audenaert@Syntegra.Com
                            // Start Change bja@reedtek.com
							try
							{
                                myValue = result["SupportCollapsable"];
							}

							catch
							{
                                myValue = DBNull.Value;
                            }
                            m.SupportCollapsable = DBNull.Value != myValue ? (bool)myValue : false;

                            // End Change  bja@reedtek.com
                            // Start Change john.mandia@whitelightsolutions.com
							try
							{
                                myValue = result["ShowEveryWhere"];
							}

							catch
							{
                                myValue = DBNull.Value;
                            }
                            m.ShowEveryWhere = DBNull.Value != myValue ? (bool)myValue : false;
                            // End Change  john.mandia@whitelightsolutions.com
                            m.CacheTime = int.Parse(result["CacheTime"].ToString());
                            m.ModuleOrder = int.Parse(result["ModuleOrder"].ToString());
                            myValue = result["ShowMobile"];
                            m.ShowMobile = !Convert.IsDBNull(myValue) ? (bool)myValue : false;
                            m.DesktopSrc = result["DesktopSrc"].ToString();
                            m.MobileSrc = result["MobileSrc"].ToString();
                            m.Admin = bool.Parse(result["Admin"].ToString());
                            this.ActivePage.Modules.Add(m);
                        }
                        // Now read Portal out params 
                        result.NextResult();
                        this.PortalID = (int)parameterPortalID.Value;
                        this.PortalName = (string)parameterPortalName.Value;
                        //jes1111 - this.PortalTitle = ConfigurationSettings.AppSettings["PortalTitlePrefix"] + this.PortalName;
                        this.PortalTitle = String.Concat(Config.PortalTitlePrefix, this.PortalName);
                        //jes1111 - this.PortalPath = Settings.Path.WebPathCombine(ConfigurationSettings.AppSettings["PortalsDirectory"], (string) parameterPortalPath.Value);
                        this.PortalPath = Path.WebPathCombine(Config.PortalsDirectory, (string)parameterPortalPath.Value);
                        //jes1111 - this.PortalSecurePath = ConfigurationSettings.AppSettings["PortalSecureDirectory"]; // added Thierry (tiptopweb) 12 Apr 2003
                        this.PortalSecurePath = Config.PortalSecureDirectory;
                        this.ActivePage.PageID = pageID;
                        this.ActivePage.PageLayout = this.CurrentLayout;
                        this.ActivePage.ParentPageID = Int32.Parse("0" + parameterParentPageID.Value);
                        this.ActivePage.PageOrder = (int)parameterPageOrder.Value;
                        this.ActivePage.MobilePageName = (string)parameterMobilePageName.Value;
                        this.ActivePage.AuthorizedRoles = (string)parameterAuthRoles.Value;
                        this.ActivePage.PageName = (string)parameterPageName.Value;
                        this.ActivePage.ShowMobile = (bool)parameterShowMobile.Value;
                        this.ActivePage.PortalPath = PortalPath; // thierry@tiptopweb.com.au for page custom layout
                        result.Close(); //by Manu, fixed bug 807858
                        //						}
                        //						catch (Exception ex)
                        //						{
                        //							HttpContext.Current.Response.Write("Failed rb_GetPortalSettings for " + pageID.ToString() + ", " + portalAlias + ":<br/>"+ex.Message);
                        //							HttpContext.Current.Response.End();
                        //							//Response.Redirect("~/app_support/ErrorNoPortal.aspx",true);
                        //						}
					}

					catch (SqlException sqex)
					{
                        Uri _requestUri = HttpContext.Current.Request.Url;
                        string _databaseUpdateRedirect = Config.DatabaseUpdateRedirect;
                        if (_databaseUpdateRedirect.StartsWith("~/"))
                            _databaseUpdateRedirect = _databaseUpdateRedirect.TrimStart(new char[] { '~' });
                        if (!_requestUri.AbsolutePath.ToLower(CultureInfo.InvariantCulture).EndsWith(_databaseUpdateRedirect.ToLower(CultureInfo.InvariantCulture)))
                            throw new DatabaseUnreachableException("This may be a new db", sqex);
                        else
                            ErrorHandler.Publish(Appleseed.Framework.LogLevel.Warn, "This may be a new db"); // Jes1111
                        return;
					}
					finally
					{
                        //by Manu fix close bug #2
                        if (myConnection.State == ConnectionState.Open)
                            myConnection.Close();
                    }
                }
            }

            //Provide a valid tab id if it is missing
            if (this.ActivePage.PageID == 0)
                this.ActivePage.PageID = ((PageStripDetails)this.DesktopPages[0]).PageID;
            //Go to get custom settings
            
            if (!Directory.Exists(Appleseed.Framework.Settings.Path.ApplicationPhysicalPath + this.PortalFullPath))
            {
                PortalsDB portals = new PortalsDB();
                portals.CreatePortalPath(PortalAlias);
            }

            CustomSettings = GetPortalCustomSettings(PortalID, GetPortalBaseSettings(PortalPath));
            //Initialize Theme
            ThemeManager themeManager = new ThemeManager(PortalPath);
            //Default
            themeManager.Load(this.CustomSettings["SITESETTINGS_THEME"].ToString());
            CurrentThemeDefault = themeManager.CurrentTheme;

            //Alternate
            if (this.CustomSettings["SITESETTINGS_ALT_THEME"].ToString() == CurrentThemeDefault.Name)
                CurrentThemeAlt = CurrentThemeDefault;

			else
			{
                themeManager.Load(this.CustomSettings["SITESETTINGS_ALT_THEME"].ToString());
                CurrentThemeAlt = themeManager.CurrentTheme;
            }
            //themeManager.Save(this.CustomSettings["SITESETTINGS_THEME"].ToString());
            //Set layout
            this.CurrentLayout = CustomSettings["SITESETTINGS_PAGE_LAYOUT"].ToString();

            // Jes1111
            // Generate DesktopPagesXml
            //jes1111 - if (bool.Parse(ConfigurationSettings.AppSettings["PortalSettingDesktopPagesXml"]))
            //if (Config.PortalSettingDesktopPagesXml)
            //	this.DesktopPagesXml = GetDesktopPagesXml();
        }

        /// <summary>
        /// The PortalSettings Constructor encapsulates all of the logic
        /// necessary to obtain configuration settings necessary to get
        /// custom setting for a different portal than current (EditPortal.aspx.cs)<br/>
        /// These Portal Settings are stored within a SQL database, and are
        /// fetched below by calling the "GetPortalSettings" stored procedure.<br/>
        /// This overload it is used
        /// </summary>
        /// <param name="PortalID">The portal ID.</param>
        public PortalSettings(int PortalID)
        {
            // Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_GetPortalSettingsPortalID", myConnection))
				{
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int);
                    parameterPortalID.Value = PortalID;
                    myCommand.Parameters.Add(parameterPortalID);
                    // Open the database connection and execute the command
                    myConnection.Open();
                    SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection); //by Manu CloseConnection

					try
					{
						if (result.Read())
						{
                            this.PortalID = Int32.Parse(result["PortalID"].ToString());
                            this.PortalName = result["PortalName"].ToString();
                            this.PortalAlias = result["PortalAlias"].ToString();
                            //jes1111 - this.PortalTitle = ConfigurationSettings.AppSettings["PortalTitlePrefix"] + result["PortalName"].ToString();
                            this.PortalTitle = String.Concat(Config.PortalTitlePrefix, result["PortalName"].ToString());
                            this.PortalPath = result["PortalPath"].ToString();
                            this.ActivePage.PageID = 0;
                            // added Thierry (tiptopweb) used for dropdown for layout and theme
                            this.ActivePage.PortalPath = PortalPath;
                            this.ActiveModule = 0;
						}

						else
						{
                            throw new Exception("The portal you requested cannot be found. PortalID: " + PortalID.ToString(), new HttpException(404, "Portal not found"));
                        }
					}

					finally
					{
                        result.Close(); //by Manu, fixed bug 807858
                        myConnection.Close();
                    }
                }
            }
            //Go to get custom settings
            CustomSettings = GetPortalCustomSettings(PortalID, GetPortalBaseSettings(PortalPath));
            this.CurrentLayout = CustomSettings["SITESETTINGS_PAGE_LAYOUT"].ToString();
            //Initialize Theme
            ThemeManager themeManager = new ThemeManager(PortalPath);
            //Default
            themeManager.Load(this.CustomSettings["SITESETTINGS_THEME"].ToString());
            CurrentThemeDefault = themeManager.CurrentTheme;
            //Alternate
            themeManager.Load(this.CustomSettings["SITESETTINGS_ALT_THEME"].ToString());
            CurrentThemeAlt = themeManager.CurrentTheme;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Flushes the base settings cache.
        /// </summary>
        /// <param name="PortalPath">The portal path.</param>
        public static void FlushBaseSettingsCache(string PortalPath)
        {
            CurrentCache.Remove(Key.PortalBaseSettings());
            CurrentCache.Remove(Key.LanguageList());
        }

        // -- Thierry (Tiptopweb), 21 Jun 2003 [START] 
        // -- Thierry (Tiptopweb),  3 Feb 2004, fixed mismatch Alt and Default theme (Alt always returned)
        // Switch the Theme if a custom theme is defined in the tab settings
        // (using custom themes from PageSettings.cs)
        // if not use the theme defined from the portalsettings
        /// <summary>
        /// Theme definition and images
        /// </summary>
        /// <returns></returns>
        public Theme GetCurrentTheme()
        {
            // look for an custom theme
			if (this.ActivePage.CustomSettings[strCustomTheme] != null && this.ActivePage.CustomSettings[strCustomTheme].ToString().Length > 0)
			{
                string customTheme = this.ActivePage.CustomSettings[strCustomTheme].ToString().Trim();
                ThemeManager themeManager = new ThemeManager(PortalPath);
                themeManager.Load(customTheme);
                return themeManager.CurrentTheme;
            }
            // no custom theme
            return CurrentThemeDefault;
        }

        /// <summary>
        /// Gets the current theme.
        /// </summary>
        /// <param name="requiredTheme">The required theme.</param>
        /// <returns></returns>
        public Theme GetCurrentTheme(string requiredTheme)
        {
			switch (requiredTheme)
			{
                case "Alt":

                    // look for an alternate custom theme
					if (this.ActivePage.CustomSettings["CustomThemeAlt"] != null && this.ActivePage.CustomSettings["CustomThemeAlt"].ToString().Length > 0)
					{
                        string customTheme = this.ActivePage.CustomSettings["CustomThemeAlt"].ToString().Trim();
                        ThemeManager themeManager = new ThemeManager(PortalPath);
                        themeManager.Load(customTheme);
                        return themeManager.CurrentTheme;
                    }
                    // no custom theme
                    return CurrentThemeAlt;
                default:

                    // look for an custom theme
					if (this.ActivePage.CustomSettings[strCustomTheme] != null && this.ActivePage.CustomSettings[strCustomTheme].ToString().Length > 0)
					{
                        string customTheme = this.ActivePage.CustomSettings[strCustomTheme].ToString().Trim();
                        ThemeManager themeManager = new ThemeManager(PortalPath);
                        themeManager.Load(customTheme);
                        return themeManager.CurrentTheme;
                    }
                    // no custom theme
                    return CurrentThemeDefault;
            }
        }

        /// <summary>
        /// Get the ParentPageID of a certain Page 06/11/2004 Rob Siera
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="tabList">The tab list.</param>
        /// <returns></returns>
        public static int GetParentPageID(int pageID, ArrayList tabList)
        {
            PageStripDetails tmpPage;

			for (int i = 0; i < tabList.Count; i++)
			{
                tmpPage = (PageStripDetails)tabList[i];

				if (tmpPage.PageID == pageID)
				{
                    return tmpPage.ParentPageID;
                }
            }
            throw new ArgumentOutOfRangeException("Page", "Root not found");
        }

        /// <summary>
        /// Gets the portal base settings.
        /// </summary>
        /// <param name="PortalPath">The portal path.</param>
        /// <returns></returns>
        public static Hashtable GetPortalBaseSettings(string PortalPath)
        {
            Hashtable _baseSettings;

			if (!CurrentCache.Exists(Key.PortalBaseSettings()))
			{
                // fix: Jes1111 - 27-02-2005 - for proper operation of caching
                LayoutManager layoutManager = new LayoutManager(PortalPath);
                ArrayList layoutList = layoutManager.GetLayouts();
                ThemeManager themeManager = new ThemeManager(PortalPath);
                ArrayList themeList = themeManager.GetThemes();

                //Define base settings
                _baseSettings = new Hashtable();
                int _groupOrderBase;
                SettingItemGroup _Group;

                #region Theme Management

                _Group = SettingItemGroup.THEME_LAYOUT_SETTINGS;
                _groupOrderBase = (int)SettingItemGroup.THEME_LAYOUT_SETTINGS;
                SettingItem Image = new SettingItem(new UploadedFileDataType(Path.WebPathCombine(Path.ApplicationRoot, PortalPath))); //StringDataType
                Image.Order = _groupOrderBase + 5;
                Image.Group = _Group;
                Image.EnglishName = "Logo";
                Image.Description = "Enter the name of logo file here. The logo will be searched in your portal dir. For the default portal is (~/_Appleseed).";
                _baseSettings.Add("SITESETTINGS_LOGO", Image);

                //ArrayList layoutList = new LayoutManager(PortalPath).GetLayouts();
                SettingItem TabLayoutSetting = new SettingItem(new CustomListDataType(layoutList, strName, strName));
                TabLayoutSetting.Value = "Default";
                TabLayoutSetting.Order = _groupOrderBase + 10;
                TabLayoutSetting.Group = _Group;
                TabLayoutSetting.EnglishName = "Page layout";
                TabLayoutSetting.Description = "Specify the site level page layout here.";
                _baseSettings.Add("SITESETTINGS_PAGE_LAYOUT", TabLayoutSetting);

                //ArrayList themeList = new ThemeManager(PortalPath).GetThemes();
                SettingItem Theme = new SettingItem(new CustomListDataType(themeList, strName, strName));
                Theme.Required = true;
                Theme.Order = _groupOrderBase + 15;
                Theme.Group = _Group;
                Theme.EnglishName = "Theme";
                Theme.Description = "Specify the site level theme here.";
                _baseSettings.Add("SITESETTINGS_THEME", Theme);

                //SettingItem ThemeAlt = new SettingItem(new CustomListDataType(new ThemeManager(PortalPath).GetThemes(), strName, strName));
                SettingItem ThemeAlt = new SettingItem(new CustomListDataType(themeList, strName, strName));
                ThemeAlt.Required = true;
                ThemeAlt.Order = _groupOrderBase + 20;
                ThemeAlt.Group = _Group;
                ThemeAlt.EnglishName = "Alternate theme";
                ThemeAlt.Description = "Specify the site level alternate theme here.";
                _baseSettings.Add("SITESETTINGS_ALT_THEME", ThemeAlt);
                // Jes1111 - 2004-08-06 - Zen support
                SettingItem AllowModuleCustomThemes = new SettingItem(new BooleanDataType());
                AllowModuleCustomThemes.Order = _groupOrderBase + 25;
                AllowModuleCustomThemes.Group = _Group;
                AllowModuleCustomThemes.Value = "True";
                AllowModuleCustomThemes.EnglishName = "Allow Module Custom Themes?";
                AllowModuleCustomThemes.Description = "Select to allow Custom Theme to be set on Modules.";
                _baseSettings.Add("SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES", AllowModuleCustomThemes);

                #endregion

                #region Security/User Management

                _groupOrderBase = (int)SettingItemGroup.SECURITY_USER_SETTINGS;
                _Group = SettingItemGroup.SECURITY_USER_SETTINGS;
                // Show input for Portal Admins when using Windows Authenication and Multiportal
                // cisakson@yahoo.com 28.April.2003
                // This setting is removed in Global.asa for non-Windows authenticaton sites.
                SettingItem PortalAdmins = new SettingItem(new StringDataType());
                PortalAdmins.Order = _groupOrderBase + 5;
                PortalAdmins.Group = _Group;
                //jes1111 - PortalAdmins.Value = ConfigurationSettings.AppSettings["ADAdministratorGroup"];
                PortalAdmins.Value = Config.ADAdministratorGroup;
                PortalAdmins.Required = false;
                PortalAdmins.Description = "Show input for Portal Admins when using Windows Authenication and Multiportal";
                _baseSettings.Add("WindowsAdmins", PortalAdmins);
                // Allow new registrations?
                SettingItem AllowNewRegistrations = new SettingItem(new BooleanDataType());
                AllowNewRegistrations.Order = _groupOrderBase + 10;
                AllowNewRegistrations.Group = _Group;
                AllowNewRegistrations.Value = "True";
                AllowNewRegistrations.EnglishName = "Allow New Registrations?";
                AllowNewRegistrations.Description = "Check this to allow users register themselves. Leave blank for register through User Manager only.";
                _baseSettings.Add("SITESETTINGS_ALLOW_NEW_REGISTRATION", AllowNewRegistrations);
                //MH: added dynamic load of registertypes depending on the  content in the DesktopModules/Register/ folder
                // Register
                Hashtable regPages = new Hashtable();

				foreach (string registerPage in Directory.GetFiles(HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/DesktopModules/CoreModules/Register/"), "register*.ascx", SearchOption.AllDirectories))
				{
                    string registerPageDisplayName = registerPage.Substring(registerPage.LastIndexOf("\\") + 1, registerPage.LastIndexOf(".") - registerPage.LastIndexOf("\\") - 1);
                    //string registerPageName = registerPage.Substring(registerPage.LastIndexOf("\\") + 1);
                    string registerPageName = registerPage.Replace(Path.ApplicationPhysicalPath, "~/").Replace("\\", "/");
                    regPages.Add(registerPageDisplayName, registerPageName.ToLower());
                }
                // Register Layout Setting
                SettingItem RegType = new SettingItem(new CustomListDataType(regPages, "Key", "Value"));
                RegType.Required = true;
                RegType.Value = "RegisterFull.ascx";
                RegType.EnglishName = "Register Type";
                RegType.Description = "Choose here how Register Page should look like.";
                RegType.Order = _groupOrderBase + 15;
                RegType.Group = _Group;
                _baseSettings.Add("SITESETTINGS_REGISTER_TYPE", RegType);
                //MH:end
                // Register Layout Setting module id reference by manu
                SettingItem RegModuleID = new SettingItem(new IntegerDataType());
                RegModuleID.Value = "0";
                RegModuleID.Required = true;
                RegModuleID.Order = _groupOrderBase + 16;
                RegModuleID.Group = _Group;
                RegModuleID.EnglishName = "Register Module ID";
                RegModuleID.Description = "Some custom registration may require additional settings, type here the ID of the module from where we should load settings (0= not used). Usually this module is added in an hidden area.";
                _baseSettings.Add("SITESETTINGS_REGISTER_MODULEID", RegModuleID);
                // Send mail on new registration to
                SettingItem OnRegisterSendTo = new SettingItem(new StringDataType());
                OnRegisterSendTo.Value = string.Empty;
                OnRegisterSendTo.Required = false;
                OnRegisterSendTo.Order = _groupOrderBase + 17;
                OnRegisterSendTo.Group = _Group;
                OnRegisterSendTo.EnglishName = "Send Mail To";
                OnRegisterSendTo.Description = "On new registration a mail will be send to the email address you provide here.";
                _baseSettings.Add("SITESETTINGS_ON_REGISTER_SEND_TO", OnRegisterSendTo);

                // Send mail on new registration to User from
                SettingItem OnRegisterSendFrom = new SettingItem(new StringDataType());
                OnRegisterSendFrom.Value = string.Empty;
                OnRegisterSendFrom.Required = false;
                OnRegisterSendFrom.Order = _groupOrderBase + 18;
                OnRegisterSendFrom.Group = _Group;
                OnRegisterSendFrom.EnglishName = "Send Mail From";
                OnRegisterSendFrom.Description = "On new registration a mail will be send to the new user from the email address you provide here.";
                _baseSettings.Add("SITESETTINGS_ON_REGISTER_SEND_FROM", OnRegisterSendFrom);

                //Terms of service
                SettingItem TermsOfService = new SettingItem(new PortalUrlDataType());
                TermsOfService.Order = _groupOrderBase + 20;
                TermsOfService.Group = _Group;
                TermsOfService.EnglishName = "Terms file name";
                TermsOfService.Description = "Type here a file name used for showing terms and condition in each register page. Provide localized version adding _<culturename>. E.g. Terms.txt, will search for Terms.txt and for Terms_en-US.txt";
                _baseSettings.Add("SITESETTINGS_TERMS_OF_SERVICE", TermsOfService);


                Hashtable loginPages = new Hashtable();

                string baseDir = HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/DesktopModules/");

                foreach (string loginPage in Directory.GetFiles(baseDir, "signin*.ascx", SearchOption.AllDirectories)) {
                    string loginPageDisplayName = loginPage.Substring(loginPage.LastIndexOf("DesktopModules") + 1).Replace(".ascx", string.Empty);
                    string loginPageName = loginPage.Replace(Path.ApplicationPhysicalPath, "~/").Replace("\\", "/");
                    loginPages.Add(loginPageDisplayName, loginPageName.ToLower());
                }

                SettingItem LogonType = new SettingItem(new CustomListDataType(loginPages, "Key", "Value"));
                LogonType.Required = false;
                LogonType.Value = "Signin.ascx";
                LogonType.EnglishName = "Login Type";
                LogonType.Description = "Choose here how login Page should look like.";
                LogonType.Order = _groupOrderBase + 21;
                LogonType.Group = _Group;
                _baseSettings.Add("SITESETTINGS_LOGIN_TYPE", LogonType);

                #endregion

                #region HTML Header Management

                _groupOrderBase = (int)SettingItemGroup.META_SETTINGS;
                _Group = SettingItemGroup.META_SETTINGS;
                // added: Jes1111 - page DOCTYPE setting
                SettingItem DocType = new SettingItem(new StringDataType());

                DocType.Order = _groupOrderBase + 5;

                DocType.Group = _Group;

                DocType.EnglishName = "DOCTYPE string";

                DocType.Description = "Allows you to enter a DOCTYPE string which will be inserted as the first line of the HTML output page (i.e. above the <html> element). Use this to force Quirks or Standards mode, particularly in IE. See <a href=\"http://gutfeldt.ch/matthias/articles/doctypeswitch/table.html\" target=\"_blank\">here</a> for details. NOTE: Appleseed.Zen requires a setting that guarantees Standards mode on all browsers.";

                DocType.Value = string.Empty;
                _baseSettings.Add("SITESETTINGS_DOCTYPE", DocType);
                //by John Mandia <john.mandia@whitelightsolutions.com>
                SettingItem TabTitle = new SettingItem(new StringDataType());
                TabTitle.Order = _groupOrderBase + 10;
                TabTitle.Group = _Group;
                TabTitle.EnglishName = "Page title";
                TabTitle.Description = "Allows you to enter a default tab / page title (Shows at the top of your browser).";
                _baseSettings.Add("SITESETTINGS_PAGE_TITLE", TabTitle);
                /*
                 * John Mandia: Removed This Setting. Now You can define specific Url Keywords via Tab Settings only. This is to speed up url building.
                 * 
                SettingItem TabUrlKeyword = new SettingItem(new StringDataType());
                TabUrlKeyword.Order = _groupOrderBase + 15;
                TabUrlKeyword.Group = _Group;
                TabUrlKeyword.Value = "Portal";
                TabUrlKeyword.EnglishName = "Keyword to Identify all pages";
                TabUrlKeyword.Description = "This setting is not fully implemented yet. It was to help with search engine optimisation by allowing you to specify a default keyword that would appear in your url."; 
                _baseSettings.Add("SITESETTINGS_PAGE_URL_KEYWORD", TabUrlKeyword);
                */
                SettingItem TabMetaKeyWords = new SettingItem(new StringDataType());
                TabMetaKeyWords.Order = _groupOrderBase + 15;
                TabMetaKeyWords.Group = _Group;
                // john.mandia@whitelightsolutions.com: No Default Value In Case People Don't want Meta Keywords; http://sourceforge.net/tracker/index.php?func=detail&aid=915614&group_id=66837&atid=515929
                TabMetaKeyWords.EnglishName = "Page keywords";
                TabMetaKeyWords.Description = "This setting is to help with search engine optimisation. Enter 1-15 Default Keywords that represent what your site is about.";
                _baseSettings.Add("SITESETTINGS_PAGE_META_KEYWORDS", TabMetaKeyWords);
                SettingItem TabMetaDescription = new SettingItem(new StringDataType());
                TabMetaDescription.Order = _groupOrderBase + 20;
                TabMetaDescription.Group = _Group;
                TabMetaDescription.EnglishName = "Page description";
                TabMetaDescription.Description = "This setting is to help with search engine optimisation. Enter a default description (Not too long though. 1 paragraph is enough) that describes your portal.";
                // john.mandia@whitelightsolutions.com: No Default Value In Case People Don't want a defautl descripton
                _baseSettings.Add("SITESETTINGS_PAGE_META_DESCRIPTION", TabMetaDescription);
                SettingItem TabMetaEncoding = new SettingItem(new StringDataType());
                TabMetaEncoding.Order = _groupOrderBase + 25;
                TabMetaEncoding.Group = _Group;
                TabMetaEncoding.EnglishName = "Page encoding";
                TabMetaEncoding.Description = "Every time your browser returns a page it looks to see what format it is retrieving. This allows you to specify the default content type.";
                TabMetaEncoding.Value = "<META http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1252\" />";
                _baseSettings.Add("SITESETTINGS_PAGE_META_ENCODING", TabMetaEncoding);
                SettingItem TabMetaOther = new SettingItem(new StringDataType());
                TabMetaOther.Order = _groupOrderBase + 30;
                TabMetaOther.Group = _Group;
                TabMetaOther.EnglishName = "Default Additional Meta Tag Entries";
                TabMetaOther.Description = "This setting allows you to enter new tags into the Tab / Page's HEAD Tag. As an example we have added a portal tag to identify the version, but you could have a meta refresh tag or something else like a css reference instead.";
                TabMetaOther.Value = string.Empty;
                _baseSettings.Add("SITESETTINGS_PAGE_META_OTHERS", TabMetaOther);
                SettingItem TabKeyPhrase = new SettingItem(new StringDataType());
                TabKeyPhrase.Order = _groupOrderBase + 35;
                TabKeyPhrase.Group = _Group;
                TabKeyPhrase.EnglishName = "Default Page Keyphrase";
                TabKeyPhrase.Description = "This setting can be used by a module or by a control. It allows you to define a common message for the entire portal e.g. Welcome to x portal! This can be used for search engine optimisation. It allows you to define a keyword rich phrase to be used throughout your portal.";
                TabKeyPhrase.Value = "Enter your default keyword rich Tab / Page phrase here. ";
                _baseSettings.Add("SITESETTINGS_PAGE_KEY_PHRASE", TabKeyPhrase);
                // added: Jes1111 - <body> element attributes setting
                SettingItem BodyAttributes = new SettingItem(new StringDataType());
                BodyAttributes.Order = _groupOrderBase + 45;
                BodyAttributes.Group = _Group;
                BodyAttributes.EnglishName = "&lt;body&gt; attributes";
                BodyAttributes.Description = "Allows you to enter a string which will be inserted within the <body> element, e.g. leftmargin=\"0\" bottommargin=\"0\", etc. NOTE: not advisable to use this to inject onload() function calls as there is a programmatic function for that. NOTE also that is your CSS is well sorted you should not need anything here.";
                BodyAttributes.Required = false;
                _baseSettings.Add("SITESETTINGS_BODYATTS", BodyAttributes);

                //end by John Mandia <john.mandia@whitelightsolutions.com>


                SettingItem glAnalytics = new SettingItem(new StringDataType());
                glAnalytics.Order = _groupOrderBase + 50;
                glAnalytics.Group = _Group;
                glAnalytics.EnglishName = "Google-Analytics Code";
                glAnalytics.Description = "Allows you get the tracker, with this can view the statistics of your site.";
                glAnalytics.Value = string.Empty;
                _baseSettings.Add("SITESETTINGS_GOOGLEANALYTICS", glAnalytics);

                SettingItem alternativeUrl = new SettingItem(new StringDataType());
                alternativeUrl.Order = _groupOrderBase + 55;
                alternativeUrl.Group = _Group;
                alternativeUrl.EnglishName = "Alternative site url";
                alternativeUrl.Description = "Indicate the site url for an alternative way.";
                alternativeUrl.Value = string.Empty;
                _baseSettings.Add("SITESETTINGS_ALTERNATIVE_URL", alternativeUrl);

                SettingItem addThisUsername = new SettingItem(new StringDataType());
                addThisUsername.Order = _groupOrderBase + 56;
                addThisUsername.Group = _Group;
                addThisUsername.EnglishName = "AddThis Username";
                addThisUsername.Description = "Username for AddThis sharing and tracking.";
                addThisUsername.Value = "appleseedapp";
                _baseSettings.Add("SITESETTINGS_ADDTHIS_USERNAME", addThisUsername);

                #endregion

                # region Language/Culture Management

                _groupOrderBase = (int)SettingItemGroup.CULTURE_SETTINGS;
                _Group = SettingItemGroup.CULTURE_SETTINGS;

                SettingItem LangList = new SettingItem(new MultiSelectListDataType(AppleseedCultures, "DisplayName", "Name"));
                LangList.Group = _Group;
                LangList.Order = _groupOrderBase + 10;
                LangList.EnglishName = "Language list";
                //jes1111 - LangList.Value = ConfigurationSettings.AppSettings["DefaultLanguage"]; 
                LangList.Value = Config.DefaultLanguage;
                LangList.Required = false;
                LangList.Description = "This is a list of the languages that the site will support. You can select multiples languages by pressing shift in your keyboard";
                _baseSettings.Add("SITESETTINGS_LANGLIST", LangList);

                SettingItem LangDefault = new SettingItem(new ListDataType(AppleseedCultures, "DisplayName", "Name"));
                LangDefault.Group = _Group;
                LangDefault.Order = _groupOrderBase + 20;
                LangDefault.EnglishName = "Default Language";
                //jes1111 - LangList.Value = ConfigurationSettings.AppSettings["DefaultLanguage"]; 
                LangDefault.Value = Config.DefaultLanguage;
                LangDefault.Required = false;
                LangDefault.Description = "This is the default language for the site.";
                _baseSettings.Add("SITESETTINGS_DEFAULTLANG", LangDefault);

                # endregion

                #region Miscellaneous Settings

                _groupOrderBase = (int)SettingItemGroup.MISC_SETTINGS;
                _Group = SettingItemGroup.MISC_SETTINGS;
                // Show modified by summary on/off
                SettingItem ShowModifiedBy = new SettingItem(new BooleanDataType());
                ShowModifiedBy.Order = _groupOrderBase + 10;
                ShowModifiedBy.Group = _Group;
                ShowModifiedBy.Value = "False";
                ShowModifiedBy.EnglishName = "Show modified by";
                ShowModifiedBy.Description = "Check to show by whom the module is last modified.";
                _baseSettings.Add("SITESETTINGS_SHOW_MODIFIED_BY", ShowModifiedBy);
                // Default Editor Configuration used for new modules and workflow modules. jviladiu@portalServices.net 13/07/2004
                SettingItem DefaultEditor = new SettingItem(new HtmlEditorDataType());
                DefaultEditor.Order = _groupOrderBase + 20;
                DefaultEditor.Group = _Group;
                DefaultEditor.Value = "FCKeditor";
                DefaultEditor.EnglishName = "Default Editor";
                DefaultEditor.Description = "This Editor is used by workflow and is the default for new modules.";
                _baseSettings.Add("SITESETTINGS_DEFAULT_EDITOR", DefaultEditor);
                // Default Editor Width. jviladiu@portalServices.net 13/07/2004
                SettingItem DefaultWidth = new SettingItem(new IntegerDataType());
                DefaultWidth.Order = _groupOrderBase + 25;
                DefaultWidth.Group = _Group;
                DefaultWidth.Value = "700";
                DefaultWidth.EnglishName = "Editor Width";
                DefaultWidth.Description = "Default Editor Width";
                _baseSettings.Add("SITESETTINGS_EDITOR_WIDTH", DefaultWidth);
                // Default Editor Height. jviladiu@portalServices.net 13/07/2004
                SettingItem DefaultHeight = new SettingItem(new IntegerDataType());
                DefaultHeight.Order = _groupOrderBase + 30;
                DefaultHeight.Group = _Group;
                DefaultHeight.Value = "400";
                DefaultHeight.EnglishName = "Editor Height";
                DefaultHeight.Description = "Default Editor Height";
                _baseSettings.Add("SITESETTINGS_EDITOR_HEIGHT", DefaultHeight);
                //Show Upload (Active up editor only). jviladiu@portalServices.net 13/07/2004
                SettingItem ShowUpload = new SettingItem(new BooleanDataType());
                ShowUpload.Value = "true";
                ShowUpload.Order = _groupOrderBase + 35;
                ShowUpload.Group = _Group;
                ShowUpload.EnglishName = "Upload?";
                ShowUpload.Description = "Only used if Editor is ActiveUp HtmlTextBox";
                _baseSettings.Add("SITESETTINGS_SHOWUPLOAD", ShowUpload);
                // Default Image Folder. jviladiu@portalServices.net 29/07/2004
                SettingItem DefaultImageFolder = new SettingItem(new FolderDataType(HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/" + PortalPath + "/images"), "default"));
                DefaultImageFolder.Order = _groupOrderBase + 40;
                DefaultImageFolder.Group = _Group;
                DefaultImageFolder.Value = "default";
                DefaultImageFolder.EnglishName = "Default Image Folder";
                DefaultImageFolder.Description = "Set the default image folder used by Current Editor";
                _baseSettings.Add("SITESETTINGS_DEFAULT_IMAGE_FOLDER", DefaultImageFolder);
                _groupOrderBase = (int)SettingItemGroup.MISC_SETTINGS;
                _Group = SettingItemGroup.MISC_SETTINGS;
                // Show module arrows to an administrator
                SettingItem ShowModuleArrows = new SettingItem(new BooleanDataType());
                ShowModuleArrows.Order = _groupOrderBase + 50;
                ShowModuleArrows.Group = _Group;
                ShowModuleArrows.Value = "False";
                ShowModuleArrows.EnglishName = "Show module arrows";
                ShowModuleArrows.Description = "Check to show the arrows in the module title to move modules.";
                _baseSettings.Add("SITESETTINGS_SHOW_MODULE_ARROWS", ShowModuleArrows);

                //BOWEN 11 June 2005
                // Use Recycler Module for deleted modules
                SettingItem UseRecycler = new SettingItem(new BooleanDataType());
                UseRecycler.Order = _groupOrderBase + 55;
                UseRecycler.Group = _Group;
                UseRecycler.Value = "True";
                UseRecycler.EnglishName = "Use Recycle Bin for Deleted Modules";
                UseRecycler.Description = "Check to make deleted modules go to the recycler instead of permanently deleting them.";
                _baseSettings.Add("SITESETTINGS_USE_RECYCLER", UseRecycler);
                //BOWEN 11 June 2005

                #endregion

                // Fix: Jes1111 - 27-02-2005 - incorrect setting for cache dependency
                //CacheDependency settingDependancies = new CacheDependency(null, new string[]{Appleseed.Framework.Settings.Cache.Key.ThemeList(ThemeManager.Path)});
                // set up a cache dependency object which monitors the four folders we are interested in
                CacheDependency settingDependencies =
                    new CacheDependency(
                        new string[]
							{
								LayoutManager.Path,
								layoutManager.PortalLayoutPath,
								ThemeManager.Path,
								themeManager.PortalThemePath
							});

				using (settingDependencies)
				{
                    CurrentCache.Insert(Key.PortalBaseSettings(), _baseSettings, settingDependencies);
                }
			}

			else
			{
                _baseSettings = (Hashtable)CurrentCache.Get(Key.PortalBaseSettings());
            }
            return _baseSettings;
        }

        /// <summary>
        /// The PortalSettings.GetPortalSettings Method returns a hashtable of
        /// custom Portal specific settings from the database. This method is
        /// used by Portals to access misc settings.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="_baseSettings">The _base settings.</param>
        /// <returns></returns>
        public static Hashtable GetPortalCustomSettings(int portalID, Hashtable _baseSettings)
        {
			if (!CurrentCache.Exists(Key.PortalSettings()))
			{
                // Get Settings for this Portal from the database
                Hashtable _settings = new Hashtable();

                // Create Instance of Connection and Command Object
				using (SqlConnection myConnection = Config.SqlConnectionString)
				{
					using (SqlCommand myCommand = new SqlCommand("rb_GetPortalCustomSettings", myConnection))
					{
                        // Mark the Command as a SPROC
                        myCommand.CommandType = CommandType.StoredProcedure;
                        // Add Parameters to SPROC
                        SqlParameter parameterportalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                        parameterportalID.Value = portalID;
                        myCommand.Parameters.Add(parameterportalID);
                        // Execute the command
                        myConnection.Open();
                        SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

						try
						{
							while (dr.Read())
							{
                                _settings[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
                            }
						}

						finally
						{
                            dr.Close(); //by Manu, fixed bug 807858
                            myConnection.Close();
                        }
                    }
                }

				foreach (string key in _baseSettings.Keys)
				{
					if (_settings[key] != null)
					{
                        SettingItem s = ((SettingItem)_baseSettings[key]);

                        if (_settings[key].ToString().Length != 0)
                            s.Value = _settings[key].ToString();
                    }
                }
                // Fix: Jes1111 - 27-02-2005 - change to make PortalSettings cache item dependent on PortalBaseSettings
                //Appleseed.Framework.Settings.Cache.CurrentCache.Insert(Appleseed.Framework.Settings.Cache.Key.PortalSettings(), _baseSettings);
                CacheDependency settingDependencies =
                    new CacheDependency(
                        null,
                        new string[]
							{
								Key.PortalBaseSettings()
							});

				using (settingDependencies)
				{
                    CurrentCache.Insert(Key.PortalSettings(), _baseSettings, settingDependencies);
                }
			}

			else
			{
                _baseSettings = (Hashtable)CurrentCache.Get(Key.PortalSettings());
            }
            return _baseSettings;
        }

        /// <summary>
        /// Get the proxy parameters as configured in web.config by Phillo 22/01/2003
        /// </summary>
        /// <returns></returns>
        public static WebProxy GetProxy()
        {
            //jes1111 - if(ConfigurationSettings.AppSettings["ProxyServer"].Length > 0) 
            WebProxy myProxy = new WebProxy();
            NetworkCredential myCredential = new NetworkCredential();
            myCredential.Domain = Config.ProxyDomain;
            myCredential.UserName = Config.ProxyUserID;
            myCredential.Password = Config.ProxyPassword;
            myProxy.Credentials = myCredential;
            myProxy.Address = new Uri(Config.ProxyServer);
            return (myProxy);
            //} 

            //else 
            //{ 
            //	return(null); 
            //} 
        }

        /// <summary>
        /// The get tab root should get the first level tab:
        /// <pre>
        /// + Root
        /// + Page1
        /// + SubPage1		-&gt; returns Page1
        /// + Page2
        /// + SubPage2		-&gt; returns Page2
        /// + SubPage2.1 -&gt; returns Page2
        /// </pre>
        /// </summary>
        /// <param name="parentPageID">The parent page ID.</param>
        /// <param name="tabList">The tab list.</param>
        /// <returns></returns>
        public static PageStripDetails GetRootPage(int parentPageID, ArrayList tabList)
        {
            //Changes Indah Fuldner 25.04.2003 (With assumtion that the rootlevel tab has ParentPageID = 0)
            //Search for the root tab in current array
            PageStripDetails rootPage;

			for (int i = 0; i < tabList.Count; i++)
			{
                rootPage = (PageStripDetails)tabList[i];

                // return rootPage;
				if (rootPage.PageID == parentPageID)
				{
                    parentPageID = rootPage.ParentPageID;
                    //string parentName=rootPage.PageName;

                    if (parentPageID != 0)
                        i = -1;

                    else
                        return rootPage;
                }
            }
            //End Indah Fuldner
            throw new ArgumentOutOfRangeException("Page", "Root not found");
        }

        /// <summary>
        /// The GetRootPage should get the first level tab:
        /// <pre>
        /// + Root
        /// + Page1
        /// + SubPage1		-&gt; returns Page1
        /// + Page2
        /// + SubPage2		-&gt; returns Page2
        /// + SubPage2.1 -&gt; returns Page2
        /// </pre>
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="tabList">The tab list.</param>
        /// <returns></returns>
        public static PageStripDetails GetRootPage(PageSettings tab, ArrayList tabList)
        {
            return GetRootPage(tab.PageID, tabList);
        }

        /// <summary>
        /// Get resource
        /// </summary>
        /// <param name="resourceID">The resource ID.</param>
        /// <returns></returns>
        public static string GetStringResource(string resourceID)
        {
            // TODO: MAybe this is doins something else?
            return General.GetString(resourceID);
        }

        /// <summary>
        /// Get resource
        /// </summary>
        /// <param name="resourceID">The resource ID.</param>
        /// <param name="_localize">The _localize.</param>
        /// <returns></returns>
        public static string GetStringResource(string resourceID, string[] _localize)
        {
            string res = General.GetString(resourceID);

			for (int i = 0; i <= _localize.GetUpperBound(0); i++)
			{
                string thisparam = "%" + i.ToString() + "%";
                res = res.Replace(thisparam, General.GetString(_localize[i]));
            }
            return res;
        }


        /// <summary>
        /// The UpdatePortalSetting Method updates a single module setting
        /// in the PortalSettings database table.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void UpdatePortalSetting(int portalID, string key, string value)
        {
            // Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdatePortalSetting", myConnection))
				{
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterportalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterportalID.Value = portalID;
                    myCommand.Parameters.Add(parameterportalID);
                    SqlParameter parameterKey = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50);
                    parameterKey.Value = key;
                    myCommand.Parameters.Add(parameterKey);
                    SqlParameter parameterValue = new SqlParameter("@SettingValue", SqlDbType.NVarChar, 1500);
                    parameterValue.Value = value;
                    myCommand.Parameters.Add(parameterValue);
                    // Execute the command
                    myConnection.Open();

					try
					{
                        myCommand.ExecuteNonQuery();
					}

					finally
					{
                        myConnection.Close();
                    }
                }
            }
            CurrentCache.Remove(Key.PortalSettings());
        }

        /// <summary>
        /// Get languages list from Portaldb
        /// </summary>
        /// <returns></returns>
        public string GetLanguageList()
        {
            return GetLanguageList(PortalAlias);
        }
        #endregion

        #region private methods
        /// <summary>
        /// Get languages list from Portaldb
        /// </summary>
        /// <param name="portalAlias">The portal alias.</param>
        /// <returns></returns>
        private string GetLanguageList(string portalAlias)
        {
            string langlist = string.Empty;
            string defaultlang = string.Empty;

			if (!CurrentCache.Exists(Key.LanguageList()))
			{
                // Create Instance of Connection and Command Object
				using (SqlConnection myConnection = Config.SqlConnectionString)
				{
					using (SqlCommand myCommand = new SqlCommand("rb_GetPortalSetting", myConnection))
					{
                        // Mark the Command as a SPROC
                        myCommand.CommandType = CommandType.StoredProcedure;
                        // Add Parameters to SPROC
                        SqlParameter parameterPortalAlias = new SqlParameter("@PortalAlias", SqlDbType.NVarChar, 50);
                        parameterPortalAlias.Value = portalAlias; // Specify the Portal Alias Dynamically 
                        myCommand.Parameters.Add(parameterPortalAlias);
                        SqlParameter parameterSettingName = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50);
                        parameterSettingName.Value = "SITESETTINGS_LANGLIST"; // Specify the SettingName 
                        myCommand.Parameters.Add(parameterSettingName);
                        // Open the database connection and execute the command
                        myConnection.Open();

						try
						{
                            //Better null check here by Manu
                            object tmp = myCommand.ExecuteScalar();

                            if (tmp != null) langlist = tmp.ToString();
						}

						catch (Exception ex)
						{
                            // Appleseed.Framework.Helpers.LogHelper.Logger.Log(Appleseed.Framework.Configuration.LogLevel.Warn, "Get languages from db", ex);
                            Appleseed.Framework.ErrorHandler.Publish(LogLevel.Warn, "Failed to get languages from database.", ex); // Jes1111
						}

						finally
						{
                            myConnection.Close();
                        }
                    }

                    using (SqlCommand myCommand = new SqlCommand("rb_GetPortalSetting", myConnection))
                    {
                        // Mark the Command as a SPROC
                        myCommand.CommandType = CommandType.StoredProcedure;
                        // Add Parameters to SPROC
                        SqlParameter parameterPortalAlias = new SqlParameter("@PortalAlias", SqlDbType.NVarChar, 50);
                        parameterPortalAlias.Value = portalAlias; // Specify the Portal Alias Dynamically 
                        myCommand.Parameters.Add(parameterPortalAlias);
                        SqlParameter parameterSettingName = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50);
                        parameterSettingName.Value = "SITESETTINGS_DEFAULTLANG"; // Specify the SettingName 
                        myCommand.Parameters.Add(parameterSettingName);
                        // Open the database connection and execute the command
                        myConnection.Open();

                        try
                        {
                            //Better null check here by Manu
                            object tmp = myCommand.ExecuteScalar();

                            if (tmp != null) defaultlang = tmp.ToString().Trim();
                        }

                        catch (Exception ex)
                        {
                            // Appleseed.Framework.Helpers.LogHelper.Logger.Log(Appleseed.Framework.Configuration.LogLevel.Warn, "Get languages from db", ex);
                            Appleseed.Framework.ErrorHandler.Publish(LogLevel.Warn, "Failed to get default language from database.", ex); // Jes1111
                        }

                        finally
                        {
                            myConnection.Close();
                        }
                    }
                }

                if (langlist.Length == 0 && defaultlang.Length == 0)
                //jes1111 - langlist = ConfigurationSettings.AppSettings["DefaultLanguage"]; //default
                {
                    langlist = Config.DefaultLanguage; //default
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.Append(defaultlang).Append(";"); // Add default lang as first item in lang list

                    foreach (string lang in langlist.Split(";".ToCharArray()))
                    {
                        var trimLang = lang.Trim();
                        if (trimLang.Length != 0 && trimLang != defaultlang) // add non empty and non default languages in list
                        {
                            sb.Append(trimLang).Append(";");
                        }
                    }
                    langlist = sb.ToString();
                }
                CurrentCache.Insert(Key.LanguageList(), langlist);
			}

			else
			{
                langlist = (string)CurrentCache.Get(Key.LanguageList());
            }
            return langlist;
        }


        /// <summary>
        /// Sets the active module cookie.
        /// </summary>
        /// <param name="mID">The m ID.</param>
        private void setActiveModuleCookie(int mID)
        {
            HttpCookie cookie;
            DateTime time;
            TimeSpan span;
            cookie = new HttpCookie("ActiveModule", mID.ToString());
            time = DateTime.Now;
            span = new TimeSpan(0, 2, 0, 0, 0); // 120 minutes to expire
            cookie.Expires = time.Add(span);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// Recurses the portal pages XML.
        /// </summary>
        /// <param name="myPage">My page.</param>
        /// <param name="writer">The writer.</param>
        private void RecursePortalPagesXml(PageStripDetails myPage, XmlTextWriter writer)
        {
            PagesBox children = myPage.Pages;
            bool _groupElementWritten = false;

			for (int child = 0; child < children.Count; child++)
			{
                //PageStripDetails mySubPage = (PageStripDetails) children[child];
                PageStripDetails mySubPage = children[child];

                //if ( mySubPage.ParentPageID == myPage.PageID && PortalSecurity.IsInRoles(myPage.AuthorizedRoles) )
				if (mySubPage.ParentPageID == myPage.PageID)
				{
					if (!_groupElementWritten)
					{
                        writer.WriteStartElement("MenuGroup"); // start MenuGroup element
                        _groupElementWritten = true;
                    }
                    writer.WriteStartElement("MenuItem"); // start MenuItem element
                    writer.WriteAttributeString("ParentPageId", mySubPage.ParentPageID.ToString());
                    //writer.WriteAttributeString("Label",mySubPage.PageName);

                    if (HttpUrlBuilder.UrlPageName(mySubPage.PageID) == HttpUrlBuilder.DefaultPage)
                        writer.WriteAttributeString("UrlPageName", mySubPage.PageName);
                    else
                        writer.WriteAttributeString("UrlPageName", HttpUrlBuilder.UrlPageName(mySubPage.PageID).Replace(".aspx", ""));

                    writer.WriteAttributeString("PageName", mySubPage.PageName);

                    writer.WriteAttributeString("PageOrder", mySubPage.PageOrder.ToString());
                    writer.WriteAttributeString("PageIndex", mySubPage.PageIndex.ToString());
                    writer.WriteAttributeString("PageLayout", mySubPage.PageLayout);
                    writer.WriteAttributeString("AuthRoles", mySubPage.AuthorizedRoles);
                    writer.WriteAttributeString("ID", mySubPage.PageID.ToString());
                    //writer.WriteAttributeString("URL",HttpUrlBuilder.BuildUrl(string.Concat("~/",mySubPage.PageName,".aspx"), mySubPage.PageID,0,null,string.Empty,this.PortalAlias,"hello/goodbye"));
                    RecursePortalPagesXml(mySubPage, writer);
                    writer.WriteEndElement(); // end MenuItem element
                }
            }

            if (_groupElementWritten)
                writer.WriteEndElement(); // end MenuGroup element
        }

        #endregion

        #region public read-only members
        /// <summary>
        /// Gets the get terms of service.
        /// </summary>
        /// <value>The get terms of service.</value>
        public string GetTermsOfService
        {
            get
            {
                string termsOfService = string.Empty;

                //Verify if we have to show conditions
				if (CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"] != null && CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"].ToString().Length != 0)
				{
                    //				// Attempt to load the required text
                    //				Appleseed.Framework.DataTypes.PortalUrlDataType pt = new Appleseed.Framework.DataTypes.PortalUrlDataType();
                    //				pt.Value = CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"].ToString();
                    //				string terms = HttpContext.Current.Server.MapPath(pt.FullPath);
                    //				
                    //				//Try to get localized version
                    //				string localized_terms;
                    //				localized_terms = terms.Replace(".", "_" + Esperantus.Localize.GetCurrentUINeutralCultureName() + ".");
                    //				if (System.IO.File.Exists(localized_terms))
                    //					terms = localized_terms;
                    //Fix by Joerg Szepan - jszepan 
                    // http://sourceforge.net/tracker/index.php?func=detail&aid=852071&group_id=66837&atid=515929
                    // Wrong Terms-File if Dot in Mappath
                    // Attempt to load the required text
                    string terms;
                    terms = CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"].ToString();
                    //Try to get localized version
                    string localized_terms = "";
                    // TODO: FIX THIS
                    // localized_terms = terms.Replace(".", "_" + Localize.GetCurrentUINeutralCultureName() + ".");
                    PortalUrlDataType pt = new PortalUrlDataType();
                    pt.Value = localized_terms;

                    if (File.Exists(HttpContext.Current.Server.MapPath(pt.FullPath)))
                        terms = localized_terms;
                    pt.Value = terms;
                    terms = HttpContext.Current.Server.MapPath(pt.FullPath);

                    //Load conditions
					if (File.Exists(terms))
					{
                        //Try to open file
						using (StreamReader s = new StreamReader(terms, Encoding.Default))
						{
                            //Get the text of the conditions
                            termsOfService = s.ReadToEnd();
                            // Close Streamreader
                            s.Close();
                        }
					}

					else
					{
                        //If load fails use default
                        termsOfService = "'" + terms + "' not found!";
                    }
                }
                //end Fix by Joerg Szepan - jszepan 
                return termsOfService;
            }
        }

        /// <summary>
        /// PortalLayoutPath is the full path in which all Layout files are
        /// </summary>
        /// <value>The portal layout path.</value>
        public string PortalLayoutPath
        {
            get
            {
                string ThisLayoutPath = this.CurrentLayout;
                string customLayout = string.Empty;

                // Thierry (Tiptopweb), 4 July 2003, switch to custom Layout
                if (this.ActivePage.CustomSettings[strCustomLayout] != null && this.ActivePage.CustomSettings[strCustomLayout].ToString().Length > 0)
                    customLayout = ActivePage.CustomSettings[strCustomLayout].ToString();

				if (customLayout.Length != 0)
				{
                    // we have a custom Layout
                    ThisLayoutPath = customLayout;
                }

                // Try to get layout from querystring
                if (HttpContext.Current != null && HttpContext.Current.Request.Params["Layout"] != null)
                    ThisLayoutPath = HttpContext.Current.Request.Params["Layout"];
                // yiming, 18 Aug 2003, get layout from portalWebPath, if no, then WebPath
                LayoutManager layoutManager = new LayoutManager(PortalPath);

                if (Directory.Exists(layoutManager.PortalLayoutPath + "/" + ThisLayoutPath + "/"))
                    return layoutManager.PortalWebPath + "/" + ThisLayoutPath + "/";

                else
                    return LayoutManager.WebPath + "/" + ThisLayoutPath + "/";
            }
        }

        /// <summary>
        /// Gets the portal pages XML.
        /// </summary>
        /// <value>The portal pages XML.</value>
        public XmlDocument PortalPagesXml
        {
            get
            {
				using (StringWriter sw = new StringWriter())
				{
                    XmlTextWriter writer = new XmlTextWriter(sw);
                    writer.Formatting = Formatting.None;
                    writer.WriteStartDocument(true);
                    writer.WriteStartElement("MenuData"); // start MenuData element
                    writer.WriteStartElement("MenuGroup"); // start top MenuGroup element

					for (int i = 0; i < this.DesktopPages.Count; i++)
					{
                        PageStripDetails myPage = (PageStripDetails)this.DesktopPages[i];

                        //if ( myPage.ParentPageID == 0 && PortalSecurity.IsInRoles(myPage.AuthorizedRoles) )
						if (myPage.ParentPageID == 0)
						{
                            writer.WriteStartElement("MenuItem"); // start MenuItem element
                            writer.WriteAttributeString("ParentPageId", myPage.ParentPageID.ToString());

                            if (HttpUrlBuilder.UrlPageName(myPage.PageID) == HttpUrlBuilder.DefaultPage)
                                writer.WriteAttributeString("UrlPageName", myPage.PageName);
                            else
                                writer.WriteAttributeString("UrlPageName", HttpUrlBuilder.UrlPageName(myPage.PageID).Replace(".aspx", ""));

                            writer.WriteAttributeString("PageName", myPage.PageName);

                            //writer.WriteAttributeString("Label",myPage.PageName);
                            writer.WriteAttributeString("PageOrder", myPage.PageOrder.ToString());
                            writer.WriteAttributeString("PageIndex", myPage.PageIndex.ToString());
                            writer.WriteAttributeString("PageLayout", myPage.PageLayout);
                            writer.WriteAttributeString("AuthRoles", myPage.AuthorizedRoles);
                            writer.WriteAttributeString("ID", myPage.PageID.ToString());
                            //writer.WriteAttributeString("URL",HttpUrlBuilder.BuildUrl(string.Concat("~/",myPage.PageName,".aspx"),myPage.PageID,0,null,string.Empty,this.PortalAlias,"hello/goodbye"));
                            RecursePortalPagesXml(myPage, writer);
                            writer.WriteEndElement(); // end MenuItem element
                        }
                    }
                    writer.WriteEndElement(); // end top MenuGroup element
                    writer.WriteEndElement(); // end MenuData element
                    writer.Flush();
                    _portalPagesXml = new XmlDocument();
                    _portalPagesXml.LoadXml(sw.ToString());
                    writer.Close();
                }
                return _portalPagesXml;
            }
        }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <value>The product version.</value>
        public static string ProductVersion
        {
            get
            {
				if (HttpContext.Current.Application["ProductVersion"] == null)
				{
                    FileVersionInfo f = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application["ProductVersion"] = f.ProductVersion;
                    HttpContext.Current.Application.UnLock();
                }
                return (string)HttpContext.Current.Application["ProductVersion"];
            }
        }

        #endregion

        #region public read/write members

        /// <summary>
        /// Gets or sets the active module.
        /// </summary>
        /// <value>The active module.</value>
        public int ActiveModule
        {
            get
            {
				if (HttpContext.Current.Request.Params["mID"] != null)
				{
                    setActiveModuleCookie(int.Parse(HttpContext.Current.Request.Params["mID"]));
                    return int.Parse(HttpContext.Current.Request.Params["mID"]);
                }

                if (HttpContext.Current.Request.Cookies["ActiveModule"] != null)
                    return int.Parse(HttpContext.Current.Request.Cookies["ActiveModule"].Value);
                return 0;
            }
            set { setActiveModuleCookie(value); }
        }

        /// <summary>
        /// Current Layout
        /// </summary>
        /// <value>The current layout.</value>
        public string CurrentLayout
        {
            get
            {
                //Patch for possible .NET framework bug
                //if returned an empty string caused an endless loop
                if (_currentLayout != null && _currentLayout.Length != 0)
                    return _currentLayout;
                else
                    return "Default";
            }
            set { _currentLayout = value; }
        }

        /// <summary>
        /// CurrentUser
        /// </summary>
        /// <value>The current user.</value>
        public static AppleseedPrincipal CurrentUser
        {
            get
            {
                AppleseedPrincipal r;

                if (HttpContext.Current.User is AppleseedPrincipal)
                    r = (AppleseedPrincipal)HttpContext.Current.User;
                else
                    r = new AppleseedPrincipal(HttpContext.Current.User.Identity, null);
                return r;
            }
            set { HttpContext.Current.User = value; }
        }



        /// <summary>
        /// Gets or sets the portal content language.
        /// </summary>
        /// <value>The portal content language.</value>
        public CultureInfo PortalContentLanguage
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set { Thread.CurrentThread.CurrentUICulture = value; }
        }

        /// <summary>
        /// Gets or sets the portal data formatting culture.
        /// </summary>
        /// <value>The portal data formatting culture.</value>
        public CultureInfo PortalDataFormattingCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
            set { Thread.CurrentThread.CurrentCulture = value; }
        }

        /// <summary>
        /// PortalPath.
        /// Base dir for all portal data, relative to root web dir.
        /// </summary>
        /// <value>The portal full path.</value>
        public string PortalFullPath
        {
            get
            {
                string x = Path.WebPathCombine(_portalPathPrefix, _portalPath);

                //(_portalPathPrefix + _portalPath).Replace("//", "/");
                if (x == "/") return string.Empty;
                return x;
            }
            set
            {
                if (value.StartsWith(_portalPathPrefix))
                    _portalPath = value.Substring(_portalPathPrefix.Length);

                else
                    _portalPath = value;
            }
        }



        /// <summary>
        /// PortalPath.
        /// Base dir for all portal data, relative to application
        /// </summary>
        /// <value>The portal path.</value>
        public string PortalPath
        {
            get { return _portalPath; }
            set
            {
                _portalPath = value;
                //				//by manu
                //				//be sure it starts with "/"
                //				if (_portalPath.Length > 0 && !_portalPath.StartsWith("/"))
                //					_portalPath = Appleseed.Framework.Settings.Path.WebPathCombine("/", _portalPath);
            }
        }

        /// <summary>
        /// PortalSecurePath.
        /// Base dir for SSL
        /// </summary>
        /// <value>The portal secure path.</value>
        public string PortalSecurePath
        {
            get
            {
				if (_portalSecurePath == null)
				{
                    this.PortalSecurePath = Config.PortalSecureDirectory;
                }
                return _portalSecurePath;
            }
            set { _portalSecurePath = value; }
        }

        /// <summary>
        /// Gets or sets the portal UI language.
        /// </summary>
        /// <value>The portal UI language.</value>
        public CultureInfo PortalUILanguage
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set { Thread.CurrentThread.CurrentUICulture = value; }
        }

        /// <summary>
        /// Gets or sets the portal ID.
        /// </summary>
        /// <value>The portal ID.</value>
        public int PortalID
        {
            get { return _portalID; }
            set { _portalID = value; }
        }

        /// <summary>
        /// Gets or sets the mobile pages.
        /// </summary>
        /// <value>The mobile pages.</value>
        public ArrayList MobilePages
        {
            get { return _mobilePages; }
            set { _mobilePages = value; }
        }

        /// <summary>
        /// Gets or sets the active page.
        /// </summary>
        /// <value>The active page.</value>
        public PageSettings ActivePage
        {
            get { return _activePage; }
            set { _activePage = value; }
        }

        /// <summary>
        /// Gets or sets the custom settings.
        /// </summary>
        /// <value>The custom settings.</value>
        public Hashtable CustomSettings
        {
            get { return _customSettings; }
            set { _customSettings = value; }
        }

        /// <summary>
        /// Gets or sets the desktop pages.
        /// </summary>
        /// <value>The desktop pages.</value>
        public ArrayList DesktopPages
        {
            get { return _desktopPages; }
            set { _desktopPages = value; }
        }

        //		/// <summary>
        //		/// 
        //		/// </summary>
        //		public XPathDocument DesktopPagesXml
        //		{
        //			get { return _desktopPagesXml; }
        //			set { _desktopPagesXml = value; }
        //		}

        /// <summary>
        /// Gets or sets the portal alias.
        /// </summary>
        /// <value>The portal alias.</value>
        public string PortalAlias
        {
            get { return _portalAlias; }
            set { _portalAlias = value; }
        }

        /// <summary>
        /// Gets or sets the name of the portal.
        /// </summary>
        /// <value>The name of the portal.</value>
        public string PortalName
        {
            get { return _portalName; }
            set { _portalName = value; }
        }

        /// <summary>
        /// Gets or sets the portal title.
        /// </summary>
        /// <value>The portal title.</value>
        public string PortalTitle
        {
            get { return _portalTitle; }
            set { _portalTitle = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show pages].
        /// </summary>
        /// <value><c>true</c> if [show pages]; otherwise, <c>false</c>.</value>
        public bool ShowPages
        {
            get { return _showPages; }
            set { _showPages = value; }
        }

        /// <summary>
        /// Gets or sets the current theme alt.
        /// </summary>
        /// <value>The current theme alt.</value>
        public Theme CurrentThemeAlt
        {
            get { return _currentThemeAlt; }
            set { _currentThemeAlt = value; }
        }

        /// <summary>
        /// Gets or sets the current theme default.
        /// </summary>
        /// <value>The current theme default.</value>
        public Theme CurrentThemeDefault
        {
            get { return _currentThemeDefault; }
            set { _currentThemeDefault = value; }
        }

        /// <summary>
        /// Gets or sets the scheduler.
        /// </summary>
        /// <value>The scheduler.</value>
        public static IScheduler Scheduler
        {
            get { return scheduler; }
            set { scheduler = value; }
        }

        //		public bool AlwaysShowEditButton
        //		{
        //			get { return alwaysShowEditButton; }
        //			set { alwaysShowEditButton = value; }
        //		}
        #endregion

        #region obsolete
        /// <summary>
        /// Obsolete
        /// </summary>
        /// <value>The name of the AD user.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.ADUserName")]
        public static string ADUserName
        {
            get { return Config.ADUserName; }
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <value>The AD user password.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.ADUserPassword")]
        public static string ADUserPassword
        {
            get { return Config.ADUserPassword; }
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <value><c>true</c> if [encrypt password]; otherwise, <c>false</c>.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.EncryptPassword")]
        public static bool EncryptPassword
        {
            get { return Config.EncryptPassword; }
        }

        /// <summary>
        /// ApplicationPath, Application dependent.
        /// Used by newsletter. Needed if you want to reference a page
        /// from an external resource (an email for example)
        /// Since it is common for all portals is declared as static.
        /// </summary>
        /// <value>The application full path.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Path.ApplicationFullPath")]
        public static string ApplicationFullPath
        {
            get { return Path.ApplicationFullPath; }
        }

        /// <summary>
        /// ApplicationPath, Application dependent relative Application Path.
        /// Base dir for all portal code
        /// Since it is common for all portals is declared as static
        /// </summary>
        /// <value>The application path.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Path.ApplicationRoot")]
        public static string ApplicationPath
        {
            get { return Path.ApplicationRoot; }
        }

        /// <summary>
        /// ApplicationPhisicalPath.
        /// File system property
        /// </summary>
        /// <value>The application phisical path.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Path.ApplicationPhysicalPath")]
        public static string ApplicationPhisicalPath
        {
            get { return Path.ApplicationPhysicalPath; }
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <value>The code version.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Portal.CodeVersion")]
        public static int CodeVersion
        {
            get
            {
                return Portal.CodeVersion;
            }
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <value>The database version.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Database.DatabaseVersion", false)]
        public static int DatabaseVersion
        {
            get { return Database.DatabaseVersion; }
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <value><c>true</c> if [enable AD user]; otherwise, <c>false</c>.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.EnableADUser")]
        public static bool EnableADUser
        {
            get { return Config.EnableADUser; }
        }

        /// <summary>
        /// This static string fetches the portal's alias either via querystring, cookie or domain and returns it
        /// </summary>
        /// <value>The get portal unique ID.</value>
        [Obsolete("Use Appleseed.Framework.Settings.Portal.UniqueID")]
        public static string GetPortalUniqueID
        {
            get { return Portal.UniqueID; }
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is monitoring enabled; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.EnableMonitoring")]
        public static bool IsMonitoringEnabled
        {
            get { return Config.EnableMonitoring; }
        }

        /// <summary>
        /// SmtpServer
        /// </summary>
        /// <value>The SMTP server.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.SmtpServer")]
        public static string SmtpServer
        {
            get { return Portal.SmtpServer; }
        }

        /// <summary>
        /// Database connection
        /// </summary>
        /// <value>The SQL connection string.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.SqlConnectionString")]
        public static SqlConnection SqlConnectionString
        {
            get { return Config.SqlConnectionString; }
        }

        /// <summary>
        /// If true all users will be loaded from portal 0 instance
        /// </summary>
        /// <value><c>true</c> if [use single user base]; otherwise, <c>false</c>.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.UseSingleUserBase")]
        public static bool UseSingleUserBase
        {
            get { return Config.UseSingleUserBase; }
        }
        #endregion

    }
}
