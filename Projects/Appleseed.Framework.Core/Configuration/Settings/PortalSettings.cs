// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortalSettings.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   PortalSettings Class encapsulates all of the settings
//   for the Portal, as well as the configuration settings required
//   to execute the current tab view within the portal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.Site.Configuration
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Caching;
    using System.Xml;

    using Appleseed.Framework.DataTypes;
    using Appleseed.Framework.Design;
    using Appleseed.Framework.Exceptions;
    using Appleseed.Framework.Scheduler;
    using Appleseed.Framework.Security;
    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Settings.Cache;
    using Appleseed.Framework.Site.Data;
    using Appleseed.Framework.Web.UI.WebControls;

    using Path = Appleseed.Framework.Settings.Path;
    using System.Web.Security;

    /// <summary>
    /// PortalSettings Class encapsulates all of the settings 
    ///     for the Portal, as well as the configuration settings required 
    ///     to execute the current tab view within the portal.
    /// </summary>
    [History("jminond", "2005/03/10", "Tab to page conversion")]
    [History("gman3001", "2004/09/29", 
        "Added the GetCurrentUserProfile method to obtain a hashtable of the current user's profile details.")]
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
        #region Constants and Fields

        /// <summary>
        ///     The str at page id.
        /// </summary>
        private const string StringsAtPageId = "@PageID";

        /// <summary>
        ///     The str at portal id.
        /// </summary>
        private const string StringsAtPortalId = "@PortalID";

        /// <summary>
        ///     The str custom layout.
        /// </summary>
        private const string StringsCustomLayout = "CustomLayout";

        /// <summary>
        ///     The str custom theme.
        /// </summary>
        private const string StringsCustomTheme = "CustomTheme";

        /// <summary>
        ///     The str name.
        /// </summary>
        private const string StringsName = "Name";

        /// <summary>
        ///     The _portal path prefix.
        /// </summary>
        private readonly string portalPathPrefix = HttpContext.Current.Request.ApplicationPath == "/"
                                                       ? string.Empty
                                                       : HttpContext.Current.Request.ApplicationPath;

        /// <summary>
        ///     The _ appleseed cultures.
        /// </summary>
        private static CultureInfo[] appleseedCultures;

        /// <summary>
        ///     The _current layout.
        /// </summary>
        private string currentLayout;

        // private XPathDocument _desktopPagesXml;

        /// <summary>
        ///     The _portal pages xml.
        /// </summary>
        private XmlDocument portalPagesXml;

        /// <summary>
        ///     The _portal path.
        /// </summary>
        private string portalPath = string.Empty;

        /// <summary>
        ///     The _portal secure path.
        /// </summary>
        private string portalSecurePath;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalSettings"/> class. 
        ///     The PortalSettings Constructor encapsulates all of the logic
        ///     necessary to obtain configuration settings necessary to render
        ///     a Portal Page view for a given request.<br/>
        ///     These Portal Settings are stored within a SQL database, and are
        ///     fetched below by calling the "GetPortalSettings" stored procedure.<br/>
        ///     This stored procedure returns values as SPROC output parameters,
        ///     and using three result sets.
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <param name="portalAlias">
        /// The portal alias.
        /// </param>
        public PortalSettings(int pageId, string portalAlias)
        {
            this.ActivePage = new PageSettings();
            this.DesktopPages = new ArrayList();
            this.ShowPages = true;
            this.MobilePages = new ArrayList();

            // Changes culture/language according to settings
            try
            {
                // Moved here for support db call
                LanguageSwitcher.ProcessCultures(GetLanguageList(portalAlias), portalAlias);
            }
            catch (Exception ex)
            {
                ErrorHandler.Publish(LogLevel.Warn, "Failed to load languages, loading defaults.", ex); // Jes1111
                LanguageSwitcher.ProcessCultures(Localization.LanguageSwitcher.LANGUAGE_DEFAULT, portalAlias);
            }

            // Create Instance of Connection and Command Object
            using (var connection = Config.SqlConnectionString)
            using (var command = new SqlCommand("rb_GetPortalSettings", connection))
            {
                // Mark the Command as a SPROC
                command.CommandType = CommandType.StoredProcedure;

                // Add Parameters to SPROC
                var parameterPortalAlias = new SqlParameter("@PortalAlias", SqlDbType.NVarChar, 128)
                    {
                        Value = portalAlias // Specify the Portal Alias Dynamically 
                    };
                command.Parameters.Add(parameterPortalAlias);
                var parameterPageId = new SqlParameter(StringsAtPageId, SqlDbType.Int, 4) { Value = pageId };
                command.Parameters.Add(parameterPageId);
                var parameterPortalLanguage = new SqlParameter("@PortalLanguage", SqlDbType.NVarChar, 12)
                    {
                       Value = this.PortalContentLanguage.Name 
                    };
                command.Parameters.Add(parameterPortalLanguage);

                // Add out parameters to Sproc
                var parameterPortalId = new SqlParameter(StringsAtPortalId, SqlDbType.Int, 4)
                    {
                       Direction = ParameterDirection.Output 
                    };
                command.Parameters.Add(parameterPortalId);
                var parameterPortalName = new SqlParameter("@PortalName", SqlDbType.NVarChar, 128)
                    {
                       Direction = ParameterDirection.Output 
                    };
                command.Parameters.Add(parameterPortalName);
                var parameterPortalPath = new SqlParameter("@PortalPath", SqlDbType.NVarChar, 128)
                    {
                       Direction = ParameterDirection.Output 
                    };
                command.Parameters.Add(parameterPortalPath);
                var parameterEditButton = new SqlParameter("@AlwaysShowEditButton", SqlDbType.Bit, 1)
                    {
                       Direction = ParameterDirection.Output 
                    };
                command.Parameters.Add(parameterEditButton);
                var parameterPageName = new SqlParameter("@PageName", SqlDbType.NVarChar, 50)
                    {
                       Direction = ParameterDirection.Output 
                    };
                command.Parameters.Add(parameterPageName);
                var parameterPageOrder = new SqlParameter("@PageOrder", SqlDbType.Int, 4)
                    {
                       Direction = ParameterDirection.Output 
                    };
                command.Parameters.Add(parameterPageOrder);
                var parameterParentPageId = new SqlParameter("@ParentPageID", SqlDbType.Int, 4)
                    {
                       Direction = ParameterDirection.Output 
                    };
                command.Parameters.Add(parameterParentPageId);
                var parameterMobilePageName = new SqlParameter("@MobilePageName", SqlDbType.NVarChar, 50)
                    {
                       Direction = ParameterDirection.Output 
                    };
                command.Parameters.Add(parameterMobilePageName);
                var parameterAuthRoles = new SqlParameter("@AuthRoles", SqlDbType.NVarChar, 512)
                    {
                       Direction = ParameterDirection.Output 
                    };
                command.Parameters.Add(parameterAuthRoles);
                var parameterShowMobile = new SqlParameter("@ShowMobile", SqlDbType.Bit, 1)
                    {
                       Direction = ParameterDirection.Output 
                    };
                command.Parameters.Add(parameterShowMobile);
                SqlDataReader result;

                try
                {
                    // Open the database connection and execute the command
                    // try // jes1111
                    // {
                    connection.Open();
                    result = command.ExecuteReader(CommandBehavior.CloseConnection);
                    this.CurrentLayout = "Default";

                    // Read the first resultset -- Desktop Page Information
                    while (result.Read())
                    {
                        var tabDetails = new PageStripDetails
                            {
                                PageID = (int)result["PageID"], 
                                ParentPageID = Int32.Parse("0" + result["ParentPageID"]), 
                                PageName = (string)result["PageName"], 
                                PageOrder = (int)result["PageOrder"], 
                                PageLayout = this.CurrentLayout, 
                                AuthorizedRoles = (string)result["AuthorizedRoles"]
                            };
                        this.PortalAlias = portalAlias;

                        // Update the AuthorizedRoles Variable
                        this.DesktopPages.Add(tabDetails);
                    }

                    if (this.DesktopPages.Count == 0)
                    {
                        return; // Abort load

                        // throw new Exception("The portal you requested has no Pages. PortalAlias: '" + portalAlias + "'", new HttpException(404, "Portal not found"));
                    }

                    // Read the second result --  Mobile Page Information
                    result.NextResult();

                    while (result.Read())
                    {
                        var tabDetails = new PageStripDetails
                            {
                                PageID = (int)result["PageID"], 
                                PageName = (string)result["MobilePageName"], 
                                PageLayout = this.CurrentLayout, 
                                AuthorizedRoles = (string)result["AuthorizedRoles"]
                            };
                        this.MobilePages.Add(tabDetails);
                    }

                    // Read the third result --  Module Page Information
                    result.NextResult();

                    while (result.Read())
                    {
                        var m = new ModuleSettings
                            {
                                ModuleID = (int)result["ModuleID"], 
                                ModuleDefID = (int)result["ModuleDefID"], 
                                GuidID = (Guid)result["GeneralModDefID"], 
                                PageID = (int)result["TabID"], 
                                PaneName = (string)result["PaneName"], 
                                ModuleTitle = (string)result["ModuleTitle"]
                            };
                        var value = result["AuthorizedEditRoles"];
                        m.AuthorizedEditRoles = !Convert.IsDBNull(value) ? (string)value : string.Empty;
                        value = result["AuthorizedViewRoles"];
                        m.AuthorizedViewRoles = !Convert.IsDBNull(value) ? (string)value : string.Empty;
                        value = result["AuthorizedAddRoles"];
                        m.AuthorizedAddRoles = !Convert.IsDBNull(value) ? (string)value : string.Empty;
                        value = result["AuthorizedDeleteRoles"];
                        m.AuthorizedDeleteRoles = !Convert.IsDBNull(value) ? (string)value : string.Empty;
                        value = result["AuthorizedPropertiesRoles"];
                        m.AuthorizedPropertiesRoles = !Convert.IsDBNull(value) ? (string)value : string.Empty;

                        // jviladiu@portalServices.net (19/08/2004) Add support for move & delete module roles
                        value = result["AuthorizedMoveModuleRoles"];
                        m.AuthorizedMoveModuleRoles = !Convert.IsDBNull(value) ? (string)value : string.Empty;
                        value = result["AuthorizedDeleteModuleRoles"];
                        m.AuthorizedDeleteModuleRoles = !Convert.IsDBNull(value) ? (string)value : string.Empty;

                        // Change by Geert.Audenaert@Syntegra.Com
                        // Date: 6/2/2003
                        value = result["AuthorizedPublishingRoles"];
                        m.AuthorizedPublishingRoles = !Convert.IsDBNull(value) ? (string)value : string.Empty;
                        value = result["SupportWorkflow"];
                        m.SupportWorkflow = !Convert.IsDBNull(value) ? (bool)value : false;

                        // Date: 27/2/2003
                        value = result["AuthorizedApproveRoles"];
                        m.AuthorizedApproveRoles = !Convert.IsDBNull(value) ? (string)value : string.Empty;
                        value = result["WorkflowState"];
                        m.WorkflowStatus = !Convert.IsDBNull(value)
                                               ? (WorkflowState)(0 + (byte)value)
                                               : WorkflowState.Original;

                        // End Change Geert.Audenaert@Syntegra.Com
                        // Start Change bja@reedtek.com
                        try
                        {
                            value = result["SupportCollapsable"];
                        }
                        catch
                        {
                            value = DBNull.Value;
                        }

                        m.SupportCollapsable = DBNull.Value != value ? (bool)value : false;

                        // End Change  bja@reedtek.com
                        // Start Change john.mandia@whitelightsolutions.com
                        try
                        {
                            value = result["ShowEveryWhere"];
                        }
                        catch
                        {
                            value = DBNull.Value;
                        }

                        m.ShowEveryWhere = DBNull.Value != value ? (bool)value : false;

                        // End Change  john.mandia@whitelightsolutions.com
                        m.CacheTime = int.Parse(result["CacheTime"].ToString());
                        m.ModuleOrder = int.Parse(result["ModuleOrder"].ToString());
                        value = result["ShowMobile"];
                        m.ShowMobile = !Convert.IsDBNull(value) ? (bool)value : false;
                        m.DesktopSrc = result["DesktopSrc"].ToString();
                        m.MobileSrc = result["MobileSrc"].ToString();
                        m.Admin = bool.Parse(result["Admin"].ToString());
                        this.ActivePage.Modules.Add(m);
                    }

                    // Now read Portal out params 
                    result.NextResult();
                    this.PortalID = (int)parameterPortalId.Value;
                    this.PortalName = (string)parameterPortalName.Value;

                    // jes1111 - this.PortalTitle = ConfigurationSettings.AppSettings["PortalTitlePrefix"] + this.PortalName;
                    this.PortalTitle = String.Concat(Config.PortalTitlePrefix, this.PortalName);

                    // jes1111 - this.PortalPath = Settings.Path.WebPathCombine(ConfigurationSettings.AppSettings["PortalsDirectory"], (string) parameterPortalPath.Value);
                    this.PortalPath = Path.WebPathCombine(Config.PortalsDirectory, (string)parameterPortalPath.Value);

                    // jes1111 - this.PortalSecurePath = ConfigurationSettings.AppSettings["PortalSecureDirectory"]; // added Thierry (tiptopweb) 12 Apr 2003
                    this.PortalSecurePath = Config.PortalSecureDirectory;
                    this.ActivePage.PageID = pageId;
                    this.ActivePage.PageLayout = this.CurrentLayout;
                    this.ActivePage.ParentPageID = Int32.Parse("0" + parameterParentPageId.Value);
                    this.ActivePage.PageOrder = (int)parameterPageOrder.Value;
                    this.ActivePage.MobilePageName = (string)parameterMobilePageName.Value;
                    this.ActivePage.AuthorizedRoles = (string)parameterAuthRoles.Value;
                    this.ActivePage.PageName = (string)parameterPageName.Value;
                    this.ActivePage.ShowMobile = (bool)parameterShowMobile.Value;
                    this.ActivePage.PortalPath = this.PortalPath; // thierry@tiptopweb.com.au for page custom layout
                    result.Close(); // by Manu, fixed bug 807858

                    // }
                    // catch (Exception ex)
                    // {
                    // HttpContext.Current.Response.Write("Failed rb_GetPortalSettings for " + pageID.ToString() + ", " + portalAlias + ":<br/>"+ex.Message);
                    // HttpContext.Current.Response.End();
                    // //Response.Redirect("~/app_support/ErrorNoPortal.aspx",true);
                    // }
                }
                catch (SqlException sqex)
                {
                    var requestUri = HttpContext.Current.Request.Url;
                    var databaseUpdateRedirect = Config.DatabaseUpdateRedirect;
                    if (databaseUpdateRedirect.StartsWith("~/"))
                    {
                        databaseUpdateRedirect = databaseUpdateRedirect.TrimStart(new[] { '~' });
                    }

                    if (
                        !requestUri.AbsolutePath.ToLower(CultureInfo.InvariantCulture).EndsWith(
                            databaseUpdateRedirect.ToLower(CultureInfo.InvariantCulture)))
                    {
                        throw new DatabaseUnreachableException("This may be a new db", sqex);
                    }
                    else
                    {
                        ErrorHandler.Publish(LogLevel.Warn, "This may be a new db"); // Jes1111
                    }

                    return;
                }
                finally
                {
                    // by Manu fix close bug #2
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }

            // Provide a valid tab id if it is missing
            if (this.ActivePage.PageID == 0)
            {
                this.ActivePage.PageID = ((PageStripDetails)this.DesktopPages[0]).PageID;
            }

            // Go to get custom settings
            if (!Directory.Exists(Path.ApplicationPhysicalPath + this.PortalFullPath))
            {
                var portals = new PortalsDB();
                portals.CreatePortalPath(this.PortalAlias);
            }

            this.CustomSettings = GetPortalCustomSettings(this.PortalID, GetPortalBaseSettings(this.PortalPath));

            // Initialize Theme
            var themeManager = new ThemeManager(this.PortalPath);

            // Default
            themeManager.Load(this.CustomSettings["SITESETTINGS_THEME"].ToString());
            this.CurrentThemeDefault = themeManager.CurrentTheme;

            // Alternate
            if (this.CustomSettings["SITESETTINGS_ALT_THEME"].ToString() == this.CurrentThemeDefault.Name)
            {
                this.CurrentThemeAlt = this.CurrentThemeDefault;
            }
            else
            {
                themeManager.Load(this.CustomSettings["SITESETTINGS_ALT_THEME"].ToString());
                this.CurrentThemeAlt = themeManager.CurrentTheme;
            }

            // themeManager.Save(this.CustomSettings["SITESETTINGS_THEME"].ToString());
            // Set layout
            this.CurrentLayout = this.CustomSettings["SITESETTINGS_PAGE_LAYOUT"].ToString();

            // Jes1111
            // Generate DesktopPagesXml
            // jes1111 - if (bool.Parse(ConfigurationSettings.AppSettings["PortalSettingDesktopPagesXml"]))
            // if (Config.PortalSettingDesktopPagesXml)
            // this.DesktopPagesXml = GetDesktopPagesXml();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalSettings"/> class. 
        ///     The PortalSettings Constructor encapsulates all of the logic
        ///     necessary to obtain configuration settings necessary to get
        ///     custom setting for a different portal than current (EditPortal.aspx.cs)<br/>
        ///     These Portal Settings are stored within a SQL database, and are
        ///     fetched below by calling the "GetPortalSettings" stored procedure.<br/>
        ///     This overload it is used
        /// </summary>
        /// <param name="portalId">
        /// The portal ID.
        /// </param>
        public PortalSettings(int portalId)
        {
            this.ActivePage = new PageSettings();
            this.DesktopPages = new ArrayList();
            this.ShowPages = true;
            this.MobilePages = new ArrayList();

            // Create Instance of Connection and Command Object
            using (var connection = Config.SqlConnectionString)
            using (var command = new SqlCommand("rb_GetPortalSettingsPortalID", connection))
            {
                // Mark the Command as a SPROC
                command.CommandType = CommandType.StoredProcedure;

                // Add Parameters to SPROC
                var parameterPortalId = new SqlParameter(StringsAtPortalId, SqlDbType.Int) { Value = portalId };
                command.Parameters.Add(parameterPortalId);

                // Open the database connection and execute the command
                connection.Open();
                var result = command.ExecuteReader(CommandBehavior.CloseConnection); // by Manu CloseConnection

                try
                {
                    if (result.Read())
                    {
                        this.PortalID = Int32.Parse(result["PortalID"].ToString());
                        this.PortalName = result["PortalName"].ToString();
                        this.PortalAlias = result["PortalAlias"].ToString();

                        // jes1111 - this.PortalTitle = ConfigurationSettings.AppSettings["PortalTitlePrefix"] + result["PortalName"].ToString();
                        this.PortalTitle = String.Concat(Config.PortalTitlePrefix, result["PortalName"].ToString());
                        this.PortalPath = result["PortalPath"].ToString();
                        this.ActivePage.PageID = 0;

                        // added Thierry (tiptopweb) used for dropdown for layout and theme
                        this.ActivePage.PortalPath = this.PortalPath;
                        this.ActiveModule = 0;
                    }
                    else
                    {
                        throw new Exception(
                            "The portal you requested cannot be found. PortalID: " + portalId, 
                            new HttpException(404, "Portal not found"));
                    }
                }
                finally
                {
                    result.Close(); // by Manu, fixed bug 807858
                    connection.Close();
                }
            }

            // Go to get custom settings
            this.CustomSettings = GetPortalCustomSettings(portalId, GetPortalBaseSettings(this.PortalPath));
            this.CurrentLayout = this.CustomSettings["SITESETTINGS_PAGE_LAYOUT"].ToString();

            // Initialize Theme
            var themeManager = new ThemeManager(this.PortalPath);

            // Default
            themeManager.Load(this.CustomSettings["SITESETTINGS_THEME"].ToString());
            this.CurrentThemeDefault = themeManager.CurrentTheme;

            // Alternate
            themeManager.Load(this.CustomSettings["SITESETTINGS_ALT_THEME"].ToString());
            this.CurrentThemeAlt = themeManager.CurrentTheme;
        }

        #endregion

        // public bool AlwaysShowEditButton
        // {
        // get { return alwaysShowEditButton; }
        // set { alwaysShowEditButton = value; }
        // }
        #region Properties

        /// <summary>
        ///     Obsolete
        /// </summary>
        /// <value>The name of the AD user.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.ADUserName")]
        public static string ADUserName
        {
            get
            {
                return Config.ADUserName;
            }
        }

        /// <summary>
        ///     Obsolete
        /// </summary>
        /// <value>The AD user password.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.ADUserPassword")]
        public static string ADUserPassword
        {
            get
            {
                return Config.ADUserPassword;
            }
        }

        /// <summary>
        ///     ApplicationPath, Application dependent.
        ///     Used by newsletter. Needed if you want to reference a page
        ///     from an external resource (an email for example)
        ///     Since it is common for all portals is declared as static.
        /// </summary>
        /// <value>The application full path.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Path.ApplicationFullPath")]
        public static string ApplicationFullPath
        {
            get
            {
                return Path.ApplicationFullPath;
            }
        }

        /// <summary>
        ///     ApplicationPath, Application dependent relative Application Path.
        ///     Base dir for all portal code
        ///     Since it is common for all portals is declared as static
        /// </summary>
        /// <value>The application path.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Path.ApplicationRoot")]
        public static string ApplicationPath
        {
            get
            {
                return Path.ApplicationRoot;
            }
        }

        /// <summary>
        ///     ApplicationPhisicalPath.
        ///     File system property
        /// </summary>
        /// <value>The application phisical path.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Path.ApplicationPhysicalPath")]
        public static string ApplicationPhisicalPath
        {
            get
            {
                return Path.ApplicationPhysicalPath;
            }
        }

        /// <summary>
        ///     Obsolete
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
        ///     CurrentUser
        /// </summary>
        /// <value>The current user.</value>
        public static AppleseedPrincipal CurrentUser
        {
            get
            {
                AppleseedPrincipal r;

                if (HttpContext.Current.User is AppleseedPrincipal)
                {
                    r = (AppleseedPrincipal)HttpContext.Current.User;
                }
                else
                {
                    r = new AppleseedPrincipal(HttpContext.Current.User.Identity, Roles.GetRolesForUser());
                    HttpContext.Current.User = r;
                }

                return r;
            }

            set
            {
                HttpContext.Current.User = value;
            }
        }

        /// <summary>
        ///     Obsolete
        /// </summary>
        /// <value>The database version.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Database.DatabaseVersion", false)]
        public static int DatabaseVersion
        {
            get
            {
                return Database.DatabaseVersion;
            }
        }

        /// <summary>
        ///     Obsolete
        /// </summary>
        /// <value><c>true</c> if [enable AD user]; otherwise, <c>false</c>.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.EnableADUser")]
        public static bool EnableADUser
        {
            get
            {
                return Config.EnableADUser;
            }
        }

        /// <summary>
        ///     Obsolete
        /// </summary>
        /// <value><c>true</c> if [encrypt password]; otherwise, <c>false</c>.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.EncryptPassword")]
        public static bool EncryptPassword
        {
            get
            {
                return Config.EncryptPassword;
            }
        }

        /// <summary>
        ///     Gets static string fetches the portal's alias either via querystring, cookie or domain and returns it
        /// </summary>
        /// <value>The get portal unique ID.</value>
        [Obsolete("Use Appleseed.Framework.Settings.Portal.UniqueID")]
        public static string GetPortalUniqueID
        {
            get
            {
                return Portal.UniqueID;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether monitoring is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is monitoring enabled; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.EnableMonitoring")]
        public static bool IsMonitoringEnabled
        {
            get
            {
                return Config.EnableMonitoring;
            }
        }

        /// <summary>
        ///     Gets the product version.
        /// </summary>
        /// <value>The product version.</value>
        public static string ProductVersion
        {
            get
            {
                if (HttpContext.Current.Application["ProductVersion"] == null)
                {
                    var f = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application["ProductVersion"] = f.ProductVersion;
                    HttpContext.Current.Application.UnLock();
                }

                return (string)HttpContext.Current.Application["ProductVersion"];
            }
        }

        /// <summary>
        ///     Gets or sets the scheduler.
        /// </summary>
        /// <value>The scheduler.</value>
        public static IScheduler Scheduler { get; set; }

        /// <summary>
        ///     Gets SmtpServer
        /// </summary>
        /// <value>The SMTP server.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.SmtpServer")]
        public static string SmtpServer
        {
            get
            {
                return Portal.SmtpServer;
            }
        }

        /// <summary>
        ///     Database connection
        /// </summary>
        /// <value>The SQL connection string.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.SqlConnectionString")]
        public static SqlConnection SqlConnectionString
        {
            get
            {
                return Config.SqlConnectionString;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether all users will be loaded from portal 0 instance
        /// </summary>
        /// <value><c>true</c> if [use single user base]; otherwise, <c>false</c>.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.UseSingleUserBase")]
        public static bool UseSingleUserBase
        {
            get
            {
                return Config.UseSingleUserBase;
            }
        }

        /// <summary>
        ///     Gets or sets the active module.
        /// </summary>
        /// <value>The active module.</value>
        public int ActiveModule
        {
            get
            {
                if (HttpContext.Current.Request.Params["mID"] != null)
                {
                    SetActiveModuleCookie(int.Parse(HttpContext.Current.Request.Params["mID"]));
                    return int.Parse(HttpContext.Current.Request.Params["mID"]);
                }

                var activeModule = HttpContext.Current.Request.Cookies["ActiveModule"];
                return activeModule != null ? int.Parse(activeModule.Value) : 0;
            }

            set
            {
                SetActiveModuleCookie(value);
            }
        }

        /// <summary>
        ///     Gets or sets the active page.
        /// </summary>
        /// <value>The active page.</value>
        public PageSettings ActivePage { get; set; }

        /// <summary>
        ///     Gets or sets current layout
        /// </summary>
        /// <value>The current layout.</value>
        public string CurrentLayout
        {
            get
            {
                // Patch for possible .NET framework bug
                // if returned an empty string caused an endless loop
                return string.IsNullOrEmpty(this.currentLayout) ? "Default" : this.currentLayout;
            }

            set
            {
                this.currentLayout = value;
            }
        }

        /// <summary>
        ///     Gets or sets the current theme alt.
        /// </summary>
        /// <value>The current theme alt.</value>
        public Theme CurrentThemeAlt { get; set; }

        /// <summary>
        ///     Gets or sets the current theme default.
        /// </summary>
        /// <value>The current theme default.</value>
        public Theme CurrentThemeDefault { get; set; }

        /// <summary>
        ///     Gets or sets the custom settings.
        /// </summary>
        /// <value>The custom settings.</value>
        public Hashtable CustomSettings { get; set; }

        /// <summary>
        ///     Gets or sets the desktop pages.
        /// </summary>
        /// <value>The desktop pages.</value>
        public ArrayList DesktopPages { get; set; }

        /// <summary>
        ///     Gets the get terms of service.
        /// </summary>
        /// <value>The get terms of service.</value>
        public string GetTermsOfService
        {
            get
            {
                var termsOfService = string.Empty;

                // Verify if we have to show conditions
                if (this.CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"] != null &&
                    this.CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"].ToString().Length != 0)
                {
                    // // Attempt to load the required text
                    // Appleseed.Framework.DataTypes.PortalUrlDataType pt = new Appleseed.Framework.DataTypes.PortalUrlDataType();
                    // pt.Value = CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"].ToString();
                    // string terms = HttpContext.Current.Server.MapPath(pt.FullPath);
                    // //Try to get localized version
                    // string localized_terms;
                    // localized_terms = terms.Replace(".", "_" + Esperantus.Localize.GetCurrentUINeutralCultureName() + ".");
                    // if (System.IO.File.Exists(localized_terms))
                    // terms = localized_terms;
                    // Fix by Joerg Szepan - jszepan 
                    // http://sourceforge.net/tracker/index.php?func=detail&aid=852071&group_id=66837&atid=515929
                    // Wrong Terms-File if Dot in Mappath
                    // Attempt to load the required text
                    var terms = this.CustomSettings["SITESETTINGS_TERMS_OF_SERVICE"].ToString();

                    // Try to get localized version
                    var localizedTerms = string.Empty;

                    // TODO: FIX THIS
                    // localized_terms = terms.Replace(".", "_" + Localize.GetCurrentUINeutralCultureName() + ".");
                    var pt = new PortalUrlDataType { Value = localizedTerms };

                    if (File.Exists(HttpContext.Current.Server.MapPath(pt.FullPath)))
                    {
                        terms = localizedTerms;
                    }

                    pt.Value = terms;
                    terms = HttpContext.Current.Server.MapPath(pt.FullPath);

                    // Load conditions
                    if (File.Exists(terms))
                    {
                        // Try to open file
                        using (var s = new StreamReader(terms, Encoding.Default))
                        {
                            // Get the text of the conditions
                            termsOfService = s.ReadToEnd();

                            // Close Streamreader
                            s.Close();
                        }
                    }
                    else
                    {
                        // If load fails use default
                        termsOfService = string.Format("'{0}' not found!", terms);
                    }
                }

                // end Fix by Joerg Szepan - jszepan 
                return termsOfService;
            }
        }

        /// <summary>
        ///     Gets or sets the mobile pages.
        /// </summary>
        /// <value>The mobile pages.</value>
        public ArrayList MobilePages { get; set; }

        /// <summary>
        ///     Gets or sets the portal alias.
        /// </summary>
        /// <value>The portal alias.</value>
        public string PortalAlias { get; set; }

        /// <summary>
        ///     Gets or sets the portal content language.
        /// </summary>
        /// <value>The portal content language.</value>
        public CultureInfo PortalContentLanguage
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }

            set
            {
                Thread.CurrentThread.CurrentUICulture = value;
            }
        }

        /// <summary>
        ///     Gets or sets the portal data formatting culture.
        /// </summary>
        /// <value>The portal data formatting culture.</value>
        public CultureInfo PortalDataFormattingCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture;
            }

            set
            {
                Thread.CurrentThread.CurrentCulture = value;
            }
        }

        /// <summary>
        ///     Gets or sets the PortalPath.
        ///     Base dir for all portal data, relative to root web dir.
        /// </summary>
        /// <value>The portal full path.</value>
        public string PortalFullPath
        {
            get
            {
                var x = Path.WebPathCombine(this.portalPathPrefix, this.portalPath);

                // (_portalPathPrefix + _portalPath).Replace("//", "/");
                return x == "/" ? string.Empty : x;
            }

            set
            {
                this.portalPath = value.StartsWith(this.portalPathPrefix)
                                      ? value.Substring(this.portalPathPrefix.Length)
                                      : value;
            }
        }

        /// <summary>
        ///     Gets or sets the portal ID.
        /// </summary>
        /// <value>The portal ID.</value>
        public int PortalID { get; set; }

        /// <summary>
        ///     Gets PortalLayoutPath is the full path in which all Layout files are
        /// </summary>
        /// <value>The portal layout path.</value>
        public string PortalLayoutPath
        {
            get
            {
                var thisLayoutPath = this.CurrentLayout;
                var customLayout = string.Empty;

                // Thierry (Tiptopweb), 4 July 2003, switch to custom Layout
                if (this.ActivePage.CustomSettings[StringsCustomLayout] != null &&
                    this.ActivePage.CustomSettings[StringsCustomLayout].ToString().Length > 0)
                {
                    customLayout = this.ActivePage.CustomSettings[StringsCustomLayout].ToString();
                }

                if (customLayout.Length != 0)
                {
                    // we have a custom Layout
                    thisLayoutPath = customLayout;
                }

                // Try to get layout from querystring
                if (HttpContext.Current != null && HttpContext.Current.Request.Params["Layout"] != null)
                {
                    thisLayoutPath = HttpContext.Current.Request.Params["Layout"];
                }

                // yiming, 18 Aug 2003, get layout from portalWebPath, if no, then WebPath
                var layoutManager = new LayoutManager(this.PortalPath);

                return Directory.Exists(string.Format("{0}/{1}/", layoutManager.PortalLayoutPath, thisLayoutPath))
                           ? string.Format("{0}/{1}/", layoutManager.PortalWebPath, thisLayoutPath)
                           : string.Format("{0}/{1}/", LayoutManager.WebPath, thisLayoutPath);
            }
        }

        /// <summary>
        ///     Gets or sets the name of the portal.
        /// </summary>
        /// <value>The name of the portal.</value>
        public string PortalName { get; set; }

        /// <summary>
        ///     Gets the portal pages XML.
        /// </summary>
        /// <value>The portal pages XML.</value>
        public XmlDocument PortalPagesXml
        {
            get
            {
                using (var sw = new StringWriter())
                {
                    var writer = new XmlTextWriter(sw) { Formatting = Formatting.None };
                    writer.WriteStartDocument(true);
                    writer.WriteStartElement("MenuData"); // start MenuData element
                    writer.WriteStartElement("MenuGroup"); // start top MenuGroup element

                    foreach (var page in
                        this.DesktopPages.Cast<PageStripDetails>().Where(page => page.ParentPageID == 0))
                    {
                        writer.WriteStartElement("MenuItem"); // start MenuItem element
                        writer.WriteAttributeString("ParentPageId", page.ParentPageID.ToString());

                        writer.WriteAttributeString(
                            "UrlPageName", 
                            HttpUrlBuilder.UrlPageName(page.PageID) == HttpUrlBuilder.DefaultPage
                                ? page.PageName
                                : HttpUrlBuilder.UrlPageName(page.PageID).Replace(".aspx", string.Empty));

                        writer.WriteAttributeString("PageName", page.PageName);

                        // writer.WriteAttributeString("Label",myPage.PageName);
                        writer.WriteAttributeString("PageOrder", page.PageOrder.ToString());
                        writer.WriteAttributeString("PageIndex", page.PageIndex.ToString());
                        writer.WriteAttributeString("PageLayout", page.PageLayout);
                        writer.WriteAttributeString("AuthRoles", page.AuthorizedRoles);
                        writer.WriteAttributeString("ID", page.PageID.ToString());

                        // writer.WriteAttributeString("URL",HttpUrlBuilder.BuildUrl(string.Concat("~/",myPage.PageName,".aspx"),myPage.PageID,0,null,string.Empty,this.PortalAlias,"hello/goodbye"));
                        this.RecursePortalPagesXml(page, writer);
                        writer.WriteEndElement(); // end MenuItem element
                    }

                    writer.WriteEndElement(); // end top MenuGroup element
                    writer.WriteEndElement(); // end MenuData element
                    writer.Flush();
                    this.portalPagesXml = new XmlDocument();
                    this.portalPagesXml.LoadXml(sw.ToString());
                    writer.Close();
                }

                return this.portalPagesXml;
            }
        }

        /// <summary>
        ///     Gets or sets PortalPath.
        ///     Base dir for all portal data, relative to application
        /// </summary>
        /// <value>The portal path.</value>
        public string PortalPath
        {
            get
            {
                return this.portalPath;
            }

            set
            {
                this.portalPath = value;

                // // by manu
                // // be sure it starts with "/"
                // if (_portalPath.Length > 0 && !_portalPath.StartsWith("/"))
                // _portalPath = Appleseed.Framework.Settings.Path.WebPathCombine("/", _portalPath);
            }
        }

        /// <summary>
        ///     Gets or sets PortalSecurePath.
        ///     Base dir for SSL
        /// </summary>
        /// <value>The portal secure path.</value>
        public string PortalSecurePath
        {
            get
            {
                if (this.portalSecurePath == null)
                {
                    this.PortalSecurePath = Config.PortalSecureDirectory;
                }

                return this.portalSecurePath;
            }

            set
            {
                this.portalSecurePath = value;
            }
        }

        /// <summary>
        ///     Gets or sets the portal title.
        /// </summary>
        /// <value>The portal title.</value>
        public string PortalTitle { get; set; }

        /// <summary>
        ///     Gets or sets the portal UI language.
        /// </summary>
        /// <value>The portal UI language.</value>
        public CultureInfo PortalUILanguage
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }

            set
            {
                Thread.CurrentThread.CurrentUICulture = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [show pages].
        /// </summary>
        /// <value><c>true</c> if [show pages]; otherwise, <c>false</c>.</value>
        public bool ShowPages { get; set; }

        /// <summary>
        ///     Gets the Appleseed cultures.
        /// </summary>
        /// <value>The Appleseed cultures.</value>
        private static CultureInfo[] AppleseedCultures
        {
            get
            {
                var locker = new object();
                lock (locker)
                {
                    if (appleseedCultures == null || appleseedCultures.Length == 0)
                    {
                        var cultures = Config.DefaultLanguageList.Split(new[] { ';' });

                        var appleseedCulturesArray = cultures.Select(culture => new CultureInfo(culture)).ToList();

                        appleseedCultures = new CultureInfo[appleseedCulturesArray.Count];
                        appleseedCulturesArray.CopyTo(appleseedCultures);
                    }

                    return appleseedCultures;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Flushes the base settings cache.
        /// </summary>
        /// <param name="PortalPath">
        /// The portal path.
        /// </param>
        public static void FlushBaseSettingsCache(string PortalPath)
        {
            CurrentCache.Remove(Key.PortalBaseSettings());
            CurrentCache.Remove(Key.LanguageList());
        }

        /// <summary>
        /// Get the ParentPageID of a certain Page 06/11/2004 Rob Siera
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <param name="tabList">
        /// The tab list.
        /// </param>
        /// <returns>
        /// The get parent page id.
        /// </returns>
        public static int GetParentPageID(int pageId, ArrayList tabList)
        {
            foreach (var tmpPage in tabList.Cast<PageStripDetails>().Where(tmpPage => tmpPage.PageID == pageId))
            {
                return tmpPage.ParentPageID;
            }

            throw new ArgumentOutOfRangeException("pageId", "Root not found");
        }

        /// <summary>
        /// Gets the portal base settings.
        /// </summary>
        /// <param name="PortalPath">
        /// The portal path.
        /// </param>
        /// <returns>
        /// </returns>
        public static Hashtable GetPortalBaseSettings(string PortalPath)
        {
            Hashtable baseSettings;

            if (!CurrentCache.Exists(Key.PortalBaseSettings()))
            {
                // fix: Jes1111 - 27-02-2005 - for proper operation of caching
                var layoutManager = new LayoutManager(PortalPath);
                var layoutList = layoutManager.GetLayouts();
                var themeManager = new ThemeManager(PortalPath);
                var themeList = themeManager.GetThemes();

                // Define base settings
                baseSettings = new Hashtable();

                var group = SettingItemGroup.THEME_LAYOUT_SETTINGS;
                var groupOrderBase = (int)SettingItemGroup.THEME_LAYOUT_SETTINGS;

                // StringDataType
                var image =
                    new SettingItem(new UploadedFileDataType(Path.WebPathCombine(Path.ApplicationRoot, PortalPath)))
                        {
                            Order = groupOrderBase + 5, 
                            Group = group, 
                            EnglishName = "Logo", 
                            Description =
                                "Enter the name of logo file here. The logo will be searched in your portal dir. For the default portal is (~/_Appleseed)."
                        };

                baseSettings.Add("SITESETTINGS_LOGO", image);

                // ArrayList layoutList = new LayoutManager(PortalPath).GetLayouts();
                var tabLayoutSetting = new SettingItem(new CustomListDataType(layoutList, StringsName, StringsName))
                    {
                        Value = "Default", 
                        Order = groupOrderBase + 10, 
                        Group = group, 
                        EnglishName = "Page layout", 
                        Description = "Specify the site level page layout here."
                    };
                baseSettings.Add("SITESETTINGS_PAGE_LAYOUT", tabLayoutSetting);

                // ArrayList themeList = new ThemeManager(PortalPath).GetThemes();
                var Theme = new SettingItem(new CustomListDataType(themeList, StringsName, StringsName));
                Theme.Required = true;
                Theme.Order = groupOrderBase + 15;
                Theme.Group = group;
                Theme.EnglishName = "Theme";
                Theme.Description = "Specify the site level theme here.";
                baseSettings.Add("SITESETTINGS_THEME", Theme);

                // SettingItem ThemeAlt = new SettingItem(new CustomListDataType(new ThemeManager(PortalPath).GetThemes(), strName, strName));
                var ThemeAlt = new SettingItem(new CustomListDataType(themeList, StringsName, StringsName));
                ThemeAlt.Required = true;
                ThemeAlt.Order = groupOrderBase + 20;
                ThemeAlt.Group = group;
                ThemeAlt.EnglishName = "Alternate theme";
                ThemeAlt.Description = "Specify the site level alternate theme here.";
                baseSettings.Add("SITESETTINGS_ALT_THEME", ThemeAlt);

                // Jes1111 - 2004-08-06 - Zen support
                var AllowModuleCustomThemes = new SettingItem(new BooleanDataType());
                AllowModuleCustomThemes.Order = groupOrderBase + 25;
                AllowModuleCustomThemes.Group = group;
                AllowModuleCustomThemes.Value = "True";
                AllowModuleCustomThemes.EnglishName = "Allow Module Custom Themes?";
                AllowModuleCustomThemes.Description = "Select to allow Custom Theme to be set on Modules.";
                baseSettings.Add("SITESETTINGS_ALLOW_MODULE_CUSTOM_THEMES", AllowModuleCustomThemes);

                groupOrderBase = (int)SettingItemGroup.SECURITY_USER_SETTINGS;
                group = SettingItemGroup.SECURITY_USER_SETTINGS;

                // Show input for Portal Admins when using Windows Authenication and Multiportal
                // cisakson@yahoo.com 28.April.2003
                // This setting is removed in Global.asa for non-Windows authenticaton sites.
                var PortalAdmins = new SettingItem(new StringDataType());
                PortalAdmins.Order = groupOrderBase + 5;
                PortalAdmins.Group = group;

                // jes1111 - PortalAdmins.Value = ConfigurationSettings.AppSettings["ADAdministratorGroup"];
                PortalAdmins.Value = Config.ADAdministratorGroup;
                PortalAdmins.Required = false;
                PortalAdmins.Description =
                    "Show input for Portal Admins when using Windows Authenication and Multiportal";
                baseSettings.Add("WindowsAdmins", PortalAdmins);

                // Allow new registrations?
                var AllowNewRegistrations = new SettingItem(new BooleanDataType());
                AllowNewRegistrations.Order = groupOrderBase + 10;
                AllowNewRegistrations.Group = group;
                AllowNewRegistrations.Value = "True";
                AllowNewRegistrations.EnglishName = "Allow New Registrations?";
                AllowNewRegistrations.Description =
                    "Check this to allow users register themselves. Leave blank for register through User Manager only.";
                baseSettings.Add("SITESETTINGS_ALLOW_NEW_REGISTRATION", AllowNewRegistrations);

                // MH: added dynamic load of registertypes depending on the  content in the DesktopModules/Register/ folder
                // Register
                var regPages = new Hashtable();

                foreach (var registerPage in
                    Directory.GetFiles(
                        HttpContext.Current.Server.MapPath(
                            Path.ApplicationRoot + "/DesktopModules/CoreModules/Register/"), 
                        "register*.ascx", 
                        SearchOption.AllDirectories))
                {
                    var registerPageDisplayName = registerPage.Substring(
                        registerPage.LastIndexOf("\\") + 1, 
                        registerPage.LastIndexOf(".") - registerPage.LastIndexOf("\\") - 1);

                    // string registerPageName = registerPage.Substring(registerPage.LastIndexOf("\\") + 1);
                    var registerPageName = registerPage.Replace(Path.ApplicationPhysicalPath, "~/").Replace("\\", "/");
                    regPages.Add(registerPageDisplayName, registerPageName.ToLower());
                }

                // Register Layout Setting
                var RegType = new SettingItem(new CustomListDataType(regPages, "Key", "Value"));
                RegType.Required = true;
                RegType.Value = "RegisterFull.ascx";
                RegType.EnglishName = "Register Type";
                RegType.Description = "Choose here how Register Page should look like.";
                RegType.Order = groupOrderBase + 15;
                RegType.Group = group;
                baseSettings.Add("SITESETTINGS_REGISTER_TYPE", RegType);

                // MH:end
                // Register Layout Setting module id reference by manu
                var RegModuleID = new SettingItem(new IntegerDataType());
                RegModuleID.Value = "0";
                RegModuleID.Required = true;
                RegModuleID.Order = groupOrderBase + 16;
                RegModuleID.Group = group;
                RegModuleID.EnglishName = "Register Module ID";
                RegModuleID.Description =
                    "Some custom registration may require additional settings, type here the ID of the module from where we should load settings (0= not used). Usually this module is added in an hidden area.";
                baseSettings.Add("SITESETTINGS_REGISTER_MODULEID", RegModuleID);

                // Send mail on new registration to
                var OnRegisterSendTo = new SettingItem(new StringDataType());
                OnRegisterSendTo.Value = string.Empty;
                OnRegisterSendTo.Required = false;
                OnRegisterSendTo.Order = groupOrderBase + 17;
                OnRegisterSendTo.Group = group;
                OnRegisterSendTo.EnglishName = "Send Mail To";
                OnRegisterSendTo.Description =
                    "On new registration a mail will be send to the email address you provide here.";
                baseSettings.Add("SITESETTINGS_ON_REGISTER_SEND_TO", OnRegisterSendTo);

                // Send mail on new registration to User from
                var OnRegisterSendFrom = new SettingItem(new StringDataType());
                OnRegisterSendFrom.Value = string.Empty;
                OnRegisterSendFrom.Required = false;
                OnRegisterSendFrom.Order = groupOrderBase + 18;
                OnRegisterSendFrom.Group = group;
                OnRegisterSendFrom.EnglishName = "Send Mail From";
                OnRegisterSendFrom.Description =
                    "On new registration a mail will be send to the new user from the email address you provide here.";
                baseSettings.Add("SITESETTINGS_ON_REGISTER_SEND_FROM", OnRegisterSendFrom);

                // Terms of service
                var TermsOfService = new SettingItem(new PortalUrlDataType());
                TermsOfService.Order = groupOrderBase + 20;
                TermsOfService.Group = group;
                TermsOfService.EnglishName = "Terms file name";
                TermsOfService.Description =
                    "Type here a file name used for showing terms and condition in each register page. Provide localized version adding _<culturename>. E.g. Terms.txt, will search for Terms.txt and for Terms_en-US.txt";
                baseSettings.Add("SITESETTINGS_TERMS_OF_SERVICE", TermsOfService);

                var loginPages = new Hashtable();

                var baseDir = HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/DesktopModules/");

                foreach (var loginPage in Directory.GetFiles(baseDir, "signin*.ascx", SearchOption.AllDirectories))
                {
                    var loginPageDisplayName =
                        loginPage.Substring(loginPage.LastIndexOf("DesktopModules") + 1).Replace(".ascx", string.Empty);
                    var loginPageName = loginPage.Replace(Path.ApplicationPhysicalPath, "~/").Replace("\\", "/");
                    loginPages.Add(loginPageDisplayName, loginPageName.ToLower());
                }

                var LogonType = new SettingItem(new CustomListDataType(loginPages, "Key", "Value"));
                LogonType.Required = false;
                LogonType.Value = "Signin.ascx";
                LogonType.EnglishName = "Login Type";
                LogonType.Description = "Choose here how login Page should look like.";
                LogonType.Order = groupOrderBase + 21;
                LogonType.Group = group;
                baseSettings.Add("SITESETTINGS_LOGIN_TYPE", LogonType);


                // ReCaptcha public and private key
                var recaptchaPrivateKey = new SettingItem(new StringDataType());
                recaptchaPrivateKey.Required = false;
                recaptchaPrivateKey.Value = "6LeQmsASAAAAADS-WeMyg9mKo5l3ERKcB4LSQieI";
                recaptchaPrivateKey.EnglishName = "ReCaptcha private key";
                recaptchaPrivateKey.Description = "Insert here google's ReCaptcha private key for your portal's captchas.";
                recaptchaPrivateKey.Order = groupOrderBase + 22;
                recaptchaPrivateKey.Group = group;
                baseSettings.Add("SITESETTINGS_RECAPTCHA_PRIVATE_KEY", recaptchaPrivateKey);

                var recaptchaPublicKey = new SettingItem(new StringDataType());
                recaptchaPublicKey.Required = false;
                recaptchaPublicKey.Value = "6LeQmsASAAAAAIx9ZoRJXA44sajtJjPl2L_MFrTS";
                recaptchaPublicKey.EnglishName = "ReCaptcha public key";
                recaptchaPublicKey.Description = "Insert here google's ReCaptcha public key for your portal's captchas.";
                recaptchaPublicKey.Order = groupOrderBase + 23;
                recaptchaPublicKey.Group = group;
                baseSettings.Add("SITESETTINGS_RECAPTCHA_PUBLIC_KEY", recaptchaPublicKey);


                #region HTML Header Management

                groupOrderBase = (int)SettingItemGroup.META_SETTINGS;
                group = SettingItemGroup.META_SETTINGS;

                // added: Jes1111 - page DOCTYPE setting
                var DocType = new SettingItem(new StringDataType());

                DocType.Order = groupOrderBase + 5;

                DocType.Group = group;

                DocType.EnglishName = "DOCTYPE string";

                DocType.Description =
                    "Allows you to enter a DOCTYPE string which will be inserted as the first line of the HTML output page (i.e. above the <html> element). Use this to force Quirks or Standards mode, particularly in IE. See <a href=\"http://gutfeldt.ch/matthias/articles/doctypeswitch/table.html\" target=\"_blank\">here</a> for details. NOTE: Appleseed.Zen requires a setting that guarantees Standards mode on all browsers.";

                DocType.Value = string.Empty;
                baseSettings.Add("SITESETTINGS_DOCTYPE", DocType);

                // by John Mandia <john.mandia@whitelightsolutions.com>
                var TabTitle = new SettingItem(new StringDataType());
                TabTitle.Order = groupOrderBase + 10;
                TabTitle.Group = group;
                TabTitle.EnglishName = "Page title";
                TabTitle.Description =
                    "Allows you to enter a default tab / page title (Shows at the top of your browser).";
                baseSettings.Add("SITESETTINGS_PAGE_TITLE", TabTitle);

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
                var TabMetaKeyWords = new SettingItem(new StringDataType());
                TabMetaKeyWords.Order = groupOrderBase + 15;
                TabMetaKeyWords.Group = group;

                // john.mandia@whitelightsolutions.com: No Default Value In Case People Don't want Meta Keywords; http://sourceforge.net/tracker/index.php?func=detail&aid=915614&group_id=66837&atid=515929
                TabMetaKeyWords.EnglishName = "Page keywords";
                TabMetaKeyWords.Description =
                    "This setting is to help with search engine optimisation. Enter 1-15 Default Keywords that represent what your site is about.";
                baseSettings.Add("SITESETTINGS_PAGE_META_KEYWORDS", TabMetaKeyWords);
                var TabMetaDescription = new SettingItem(new StringDataType());
                TabMetaDescription.Order = groupOrderBase + 20;
                TabMetaDescription.Group = group;
                TabMetaDescription.EnglishName = "Page description";
                TabMetaDescription.Description =
                    "This setting is to help with search engine optimisation. Enter a default description (Not too long though. 1 paragraph is enough) that describes your portal.";

                // john.mandia@whitelightsolutions.com: No Default Value In Case People Don't want a defautl descripton
                baseSettings.Add("SITESETTINGS_PAGE_META_DESCRIPTION", TabMetaDescription);
                var TabMetaEncoding = new SettingItem(new StringDataType());
                TabMetaEncoding.Order = groupOrderBase + 25;
                TabMetaEncoding.Group = group;
                TabMetaEncoding.EnglishName = "Page encoding";
                TabMetaEncoding.Description =
                    "Every time your browser returns a page it looks to see what format it is retrieving. This allows you to specify the default content type.";
                TabMetaEncoding.Value =
                    "<META http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1252\" />";
                baseSettings.Add("SITESETTINGS_PAGE_META_ENCODING", TabMetaEncoding);
                var TabMetaOther = new SettingItem(new StringDataType());
                TabMetaOther.Order = groupOrderBase + 30;
                TabMetaOther.Group = group;
                TabMetaOther.EnglishName = "Default Additional Meta Tag Entries";
                TabMetaOther.Description =
                    "This setting allows you to enter new tags into the Tab / Page's HEAD Tag. As an example we have added a portal tag to identify the version, but you could have a meta refresh tag or something else like a css reference instead.";
                TabMetaOther.Value = string.Empty;
                baseSettings.Add("SITESETTINGS_PAGE_META_OTHERS", TabMetaOther);
                var TabKeyPhrase = new SettingItem(new StringDataType());
                TabKeyPhrase.Order = groupOrderBase + 35;
                TabKeyPhrase.Group = group;
                TabKeyPhrase.EnglishName = "Default Page Keyphrase";
                TabKeyPhrase.Description =
                    "This setting can be used by a module or by a control. It allows you to define a common message for the entire portal e.g. Welcome to x portal! This can be used for search engine optimisation. It allows you to define a keyword rich phrase to be used throughout your portal.";
                TabKeyPhrase.Value = "Enter your default keyword rich Tab / Page phrase here. ";
                baseSettings.Add("SITESETTINGS_PAGE_KEY_PHRASE", TabKeyPhrase);

                // added: Jes1111 - <body> element attributes setting
                var BodyAttributes = new SettingItem(new StringDataType());
                BodyAttributes.Order = groupOrderBase + 45;
                BodyAttributes.Group = group;
                BodyAttributes.EnglishName = "&lt;body&gt; attributes";
                BodyAttributes.Description =
                    "Allows you to enter a string which will be inserted within the <body> element, e.g. leftmargin=\"0\" bottommargin=\"0\", etc. NOTE: not advisable to use this to inject onload() function calls as there is a programmatic function for that. NOTE also that is your CSS is well sorted you should not need anything here.";
                BodyAttributes.Required = false;
                baseSettings.Add("SITESETTINGS_BODYATTS", BodyAttributes);

                // end by John Mandia <john.mandia@whitelightsolutions.com>
                var glAnalytics = new SettingItem(new StringDataType());
                glAnalytics.Order = groupOrderBase + 50;
                glAnalytics.Group = group;
                glAnalytics.EnglishName = "Google-Analytics Code";
                glAnalytics.Description = "Allows you get the tracker, with this can view the statistics of your site.";
                glAnalytics.Value = string.Empty;
                baseSettings.Add("SITESETTINGS_GOOGLEANALYTICS", glAnalytics);

                var alternativeUrl = new SettingItem(new StringDataType());
                alternativeUrl.Order = groupOrderBase + 55;
                alternativeUrl.Group = group;
                alternativeUrl.EnglishName = "Alternative site url";
                alternativeUrl.Description = "Indicate the site url for an alternative way.";
                alternativeUrl.Value = string.Empty;
                baseSettings.Add("SITESETTINGS_ALTERNATIVE_URL", alternativeUrl);

                var addThisUsername = new SettingItem(new StringDataType());
                addThisUsername.Order = groupOrderBase + 56;
                addThisUsername.Group = group;
                addThisUsername.EnglishName = "AddThis Username";
                addThisUsername.Description = "Username for AddThis sharing and tracking.";
                addThisUsername.Value = "appleseedapp";
                baseSettings.Add("SITESETTINGS_ADDTHIS_USERNAME", addThisUsername);

                #endregion

                #region Language/Culture Management

                groupOrderBase = (int)SettingItemGroup.CULTURE_SETTINGS;
                group = SettingItemGroup.CULTURE_SETTINGS;

                var langList = new SettingItem(new MultiSelectListDataType(AppleseedCultures, "DisplayName", "Name"))
                    {
                        Group = group, 
                        Order = groupOrderBase + 10, 
                        EnglishName = "Language list", 
                        Value = Config.DefaultLanguage, 
                        Required = false, 
                        Description =
                            "This is a list of the languages that the site will support. You can select multiples languages by pressing shift in your keyboard"
                    };

                // jes1111 - LangList.Value = ConfigurationSettings.AppSettings["DefaultLanguage"]; 
                baseSettings.Add("SITESETTINGS_LANGLIST", langList);

                var langDefault = new SettingItem(new ListDataType(AppleseedCultures, "DisplayName", "Name"))
                    {
                        Group = group, 
                        Order = groupOrderBase + 20, 
                        EnglishName = "Default Language", 
                        Value = Config.DefaultLanguage, 
                        Required = false, 
                        Description = "This is the default language for the site."
                    };

                // jes1111 - LangList.Value = ConfigurationSettings.AppSettings["DefaultLanguage"]; 
                baseSettings.Add("SITESETTINGS_DEFAULTLANG", langDefault);

                #endregion

                #region Miscellaneous Settings

                groupOrderBase = (int)SettingItemGroup.MISC_SETTINGS;
                group = SettingItemGroup.MISC_SETTINGS;

                // Show modified by summary on/off
                var showModifiedBy = new SettingItem(new BooleanDataType())
                    {
                        Order = groupOrderBase + 10, 
                        Group = group, 
                        Value = "False", 
                        EnglishName = "Show modified by", 
                        Description = "Check to show by whom the module is last modified."
                    };
                baseSettings.Add("SITESETTINGS_SHOW_MODIFIED_BY", showModifiedBy);

                // Default Editor Configuration used for new modules and workflow modules. jviladiu@portalServices.net 13/07/2004
                var defaultEditor = new SettingItem(new HtmlEditorDataType())
                    {
                        Order = groupOrderBase + 20, 
                        Group = group, 
                        Value = "FCKeditor", 
                        EnglishName = "Default Editor", 
                        Description = "This Editor is used by workflow and is the default for new modules."
                    };
                baseSettings.Add("SITESETTINGS_DEFAULT_EDITOR", defaultEditor);

                // Default Editor Width. jviladiu@portalServices.net 13/07/2004
                var defaultWidth = new SettingItem(new IntegerDataType())
                    {
                        Order = groupOrderBase + 25, 
                        Group = group, 
                        Value = "700", 
                        EnglishName = "Editor Width", 
                        Description = "Default Editor Width"
                    };
                baseSettings.Add("SITESETTINGS_EDITOR_WIDTH", defaultWidth);

                // Default Editor Height. jviladiu@portalServices.net 13/07/2004
                var defaultHeight = new SettingItem(new IntegerDataType())
                    {
                        Order = groupOrderBase + 30, 
                        Group = group, 
                        Value = "400", 
                        EnglishName = "Editor Height", 
                        Description = "Default Editor Height"
                    };
                baseSettings.Add("SITESETTINGS_EDITOR_HEIGHT", defaultHeight);

                // Show Upload (Active up editor only). jviladiu@portalServices.net 13/07/2004
                var showUpload = new SettingItem(new BooleanDataType())
                    {
                        Value = "true", 
                        Order = groupOrderBase + 35, 
                        Group = group, 
                        EnglishName = "Upload?", 
                        Description = "Only used if Editor is ActiveUp HtmlTextBox"
                    };
                baseSettings.Add("SITESETTINGS_SHOWUPLOAD", showUpload);

                // Default Image Folder. jviladiu@portalServices.net 29/07/2004
                var defaultImageFolder =
                    new SettingItem(
                        new FolderDataType(
                            HttpContext.Current.Server.MapPath(
                                string.Format("{0}/{1}/images", Path.ApplicationRoot, PortalPath)), 
                            "default"))
                        {
                            Order = groupOrderBase + 40, 
                            Group = group, 
                            Value = "default", 
                            EnglishName = "Default Image Folder", 
                            Description = "Set the default image folder used by Current Editor"
                        };
                baseSettings.Add("SITESETTINGS_DEFAULT_IMAGE_FOLDER", defaultImageFolder);
                groupOrderBase = (int)SettingItemGroup.MISC_SETTINGS;
                group = SettingItemGroup.MISC_SETTINGS;

                // Show module arrows to an administrator
                var showModuleArrows = new SettingItem(new BooleanDataType())
                    {
                        Order = groupOrderBase + 50, 
                        Group = group, 
                        Value = "False", 
                        EnglishName = "Show module arrows", 
                        Description = "Check to show the arrows in the module title to move modules."
                    };
                baseSettings.Add("SITESETTINGS_SHOW_MODULE_ARROWS", showModuleArrows);

                // BOWEN 11 June 2005
                // Use Recycler Module for deleted modules
                var useRecycler = new SettingItem(new BooleanDataType())
                    {
                        Order = groupOrderBase + 55, 
                        Group = group, 
                        Value = "True", 
                        EnglishName = "Use Recycle Bin for Deleted Modules", 
                        Description =
                            "Check to make deleted modules go to the recycler instead of permanently deleting them."
                    };
                baseSettings.Add("SITESETTINGS_USE_RECYCLER", useRecycler);

                // BOWEN 11 June 2005
                #endregion

                // Fix: Jes1111 - 27-02-2005 - incorrect setting for cache dependency
                // CacheDependency settingDependancies = new CacheDependency(null, new string[]{Appleseed.Framework.Settings.Cache.Key.ThemeList(ThemeManager.Path)});
                // set up a cache dependency object which monitors the four folders we are interested in
                var settingDependencies =
                    new CacheDependency(
                        new[]
                            {
                                LayoutManager.Path, layoutManager.PortalLayoutPath, ThemeManager.Path, 
                                themeManager.PortalThemePath
                            });

                using (settingDependencies)
                {
                    CurrentCache.Insert(Key.PortalBaseSettings(), baseSettings, settingDependencies);
                }
            }
            else
            {
                baseSettings = (Hashtable)CurrentCache.Get(Key.PortalBaseSettings());
            }

            return baseSettings;
        }

        /// <summary>
        /// The PortalSettings.GetPortalSettings Method returns a hashtable of
        ///     custom Portal specific settings from the database. This method is
        ///     used by Portals to access misc settings.
        /// </summary>
        /// <param name="portalId">
        /// The portal ID.
        /// </param>
        /// <param name="baseSettings">
        /// The base settings.
        /// </param>
        /// <returns>
        /// The hash table.
        /// </returns>
        public static Hashtable GetPortalCustomSettings(int portalId, Hashtable baseSettings)
        {
            if (!CurrentCache.Exists(Key.PortalSettings()))
            {
                // Get Settings for this Portal from the database
                var settings = new Hashtable();

                // Create Instance of Connection and Command Object
                using (var connection = Config.SqlConnectionString)
                using (var command = new SqlCommand("rb_GetPortalCustomSettings", connection))
                {
                    // Mark the Command as a SPROC
                    command.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    var parameterportalId = new SqlParameter(StringsAtPortalId, SqlDbType.Int, 4) { Value = portalId };
                    command.Parameters.Add(parameterportalId);

                    // Execute the command
                    connection.Open();
                    var dr = command.ExecuteReader(CommandBehavior.CloseConnection);

                    try
                    {
                        while (dr.Read())
                        {
                            settings[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
                        }
                    }
                    finally
                    {
                        dr.Close(); // by Manu, fixed bug 807858
                        connection.Close();
                    }
                }

                foreach (string key in baseSettings.Keys)
                {
                    if (settings[key] == null)
                    {
                        continue;
                    }

                    var s = (SettingItem)baseSettings[key];

                    if (settings[key].ToString().Length != 0)
                    {
                        s.Value = settings[key].ToString();
                    }
                }

                // Fix: Jes1111 - 27-02-2005 - change to make PortalSettings cache item dependent on PortalBaseSettings
                // Appleseed.Framework.Settings.Cache.CurrentCache.Insert(Appleseed.Framework.Settings.Cache.Key.PortalSettings(), _baseSettings);
                var settingDependencies = new CacheDependency(null, new[] { Key.PortalBaseSettings() });

                using (settingDependencies)
                {
                    CurrentCache.Insert(Key.PortalSettings(), baseSettings, settingDependencies);
                }
            }
            else
            {
                baseSettings = (Hashtable)CurrentCache.Get(Key.PortalSettings());
            }

            return baseSettings;
        }

        /// <summary>
        /// Get the proxy parameters as configured in web.config by Phillo 22/01/2003
        /// </summary>
        /// <returns>
        /// The web proxy.
        /// </returns>
        public static WebProxy GetProxy()
        {
            // jes1111 - if(ConfigurationSettings.AppSettings["ProxyServer"].Length > 0) 
            var webProxy = new WebProxy { Address = new Uri(Config.ProxyServer) };
            var credentials = new NetworkCredential
                {
                   Domain = Config.ProxyDomain, UserName = Config.ProxyUserID, Password = Config.ProxyPassword 
                };
            webProxy.Credentials = credentials;
            return webProxy;

            // } 

            // else 
            // { 
            // return(null); 
            // } 
        }

        /// <summary>
        /// The get tab root should get the first level tab:
        ///     <pre>
        ///         + Root
        ///         + Page1
        ///         + SubPage1		-&gt; returns Page1
        ///         + Page2
        ///         + SubPage2		-&gt; returns Page2
        ///         + SubPage2.1 -&gt; returns Page2
        ///     </pre>
        /// </summary>
        /// <param name="parentPageId">
        /// The parent page ID.
        /// </param>
        /// <param name="tabList">
        /// The tab list.
        /// </param>
        /// <returns>
        /// </returns>
        public static PageStripDetails GetRootPage(int parentPageId, ArrayList tabList)
        {
            // Changes Indah Fuldner 25.04.2003 (With assumtion that the rootlevel tab has ParentPageID = 0)
            // Search for the root tab in current array
            PageStripDetails rootPage;

            for (var i = 0; i < tabList.Count; i++)
            {
                rootPage = (PageStripDetails)tabList[i];

                // return rootPage;
                if (rootPage.PageID != parentPageId)
                {
                    continue;
                }

                parentPageId = rootPage.ParentPageID;

                //// string parentName=rootPage.PageName;
                if (parentPageId != 0)
                {
                    i = -1;
                }
                else
                {
                    return rootPage;
                }
            }

            // End Indah Fuldner
            throw new ArgumentOutOfRangeException("Page", "Root not found");
        }

        /// <summary>
        /// The GetRootPage should get the first level tab:
        ///     <pre>
        ///         + Root
        ///         + Page1
        ///         + SubPage1		-&gt; returns Page1
        ///         + Page2
        ///         + SubPage2		-&gt; returns Page2
        ///         + SubPage2.1 -&gt; returns Page2
        ///     </pre>
        /// </summary>
        /// <param name="tab">
        /// The tab.
        /// </param>
        /// <param name="tabList">
        /// The tab list.
        /// </param>
        /// <returns>
        /// </returns>
        public static PageStripDetails GetRootPage(PageSettings tab, ArrayList tabList)
        {
            return GetRootPage(tab.PageID, tabList);
        }

        /// <summary>
        /// Get resource
        /// </summary>
        /// <param name="resourceId">
        /// The resource ID.
        /// </param>
        /// <returns>
        /// The get string resource.
        /// </returns>
        public static string GetStringResource(string resourceId)
        {
            // TODO: Maybe this is doing something else?
            return General.GetString(resourceId);
        }

        /// <summary>
        /// Get resource
        /// </summary>
        /// <param name="resourceId">
        /// The resource ID.
        /// </param>
        /// <param name="localize">
        /// The localize.
        /// </param>
        /// <returns>
        /// The get string resource.
        /// </returns>
        public static string GetStringResource(string resourceId, string[] localize)
        {
            var res = General.GetString(resourceId);

            for (var i = 0; i <= localize.GetUpperBound(0); i++)
            {
                var thisparam = string.Format("%{0}%", i);
                res = res.Replace(thisparam, General.GetString(localize[i]));
            }

            return res;
        }

        /// <summary>
        /// The UpdatePortalSetting Method updates a single module setting
        ///     in the PortalSettings database table.
        /// </summary>
        /// <param name="portalId">
        /// The portal ID.
        /// </param>
        /// <param name="key">
        /// The setting key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void UpdatePortalSetting(int portalId, string key, string value)
        {
            // Create Instance of Connection and Command Object
            using (var connection = Config.SqlConnectionString)
            using (var command = new SqlCommand("rb_UpdatePortalSetting", connection))
            {
                // Mark the Command as a SPROC
                command.CommandType = CommandType.StoredProcedure;

                // Add Parameters to SPROC
                var parameterportalId = new SqlParameter(StringsAtPortalId, SqlDbType.Int, 4) { Value = portalId };
                command.Parameters.Add(parameterportalId);
                var parameterKey = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50) { Value = key };
                command.Parameters.Add(parameterKey);
                var parameterValue = new SqlParameter("@SettingValue", SqlDbType.NVarChar, 1500) { Value = value };
                command.Parameters.Add(parameterValue);

                // Execute the command
                connection.Open();

                try
                {
                    command.ExecuteNonQuery();
                }
                finally
                {
                    connection.Close();
                }
            }

            CurrentCache.Remove(Key.PortalSettings());
        }

        /// <summary>
        /// Theme definition and images
        /// </summary>
        /// <returns>
        /// The theme.
        /// </returns>
        public Theme GetCurrentTheme()
        {
            // look for an custom theme
            if (this.ActivePage.CustomSettings[StringsCustomTheme] != null &&
                this.ActivePage.CustomSettings[StringsCustomTheme].ToString().Length > 0)
            {
                var customTheme = this.ActivePage.CustomSettings[StringsCustomTheme].ToString().Trim();
                var themeManager = new ThemeManager(this.PortalPath);
                themeManager.Load(customTheme);
                return themeManager.CurrentTheme;
            }

            // no custom theme
            return this.CurrentThemeDefault;
        }

        /// <summary>
        /// Gets the current theme.
        /// </summary>
        /// <param name="requiredTheme">
        /// The required theme.
        /// </param>
        /// <returns>
        /// The theme.
        /// </returns>
        public Theme GetCurrentTheme(string requiredTheme)
        {
            switch (requiredTheme)
            {
                case "Alt":

                    // look for an alternate custom theme
                    if (this.ActivePage.CustomSettings["CustomThemeAlt"] != null &&
                        this.ActivePage.CustomSettings["CustomThemeAlt"].ToString().Length > 0)
                    {
                        var customTheme = this.ActivePage.CustomSettings["CustomThemeAlt"].ToString().Trim();
                        var themeManager = new ThemeManager(this.PortalPath);
                        themeManager.Load(customTheme);
                        return themeManager.CurrentTheme;
                    }

                    // no custom theme
                    return this.CurrentThemeAlt;
                default:

                    // look for an custom theme
                    if (this.ActivePage.CustomSettings[StringsCustomTheme] != null &&
                        this.ActivePage.CustomSettings[StringsCustomTheme].ToString().Length > 0)
                    {
                        var customTheme = this.ActivePage.CustomSettings[StringsCustomTheme].ToString().Trim();
                        var themeManager = new ThemeManager(this.PortalPath);
                        themeManager.Load(customTheme);
                        return themeManager.CurrentTheme;
                    }

                    // no custom theme
                    return this.CurrentThemeDefault;
            }
        }

        /// <summary>
        /// Get languages list from Portaldb
        /// </summary>
        /// <returns>
        /// The get language list.
        /// </returns>
        public string GetLanguageList()
        {
            return GetLanguageList(this.PortalAlias);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get languages list from Portaldb
        /// </summary>
        /// <param name="portalAlias">
        /// The portal alias.
        /// </param>
        /// <returns>
        /// The get language list.
        /// </returns>
        private static string GetLanguageList(string portalAlias)
        {
            var langlist = string.Empty;
            var defaultlang = string.Empty;

            if (!CurrentCache.Exists(Key.LanguageList()))
            {
                // Create Instance of Connection and Command Object
                using (var connection = Config.SqlConnectionString)
                {
                    using (var command = new SqlCommand("rb_GetPortalSetting", connection))
                    {
                        // Mark the Command as a SPROC
                        command.CommandType = CommandType.StoredProcedure;

                        // Add Parameters to SPROC
                        var parameterPortalAlias = new SqlParameter("@PortalAlias", SqlDbType.NVarChar, 50);
                        parameterPortalAlias.Value = portalAlias; // Specify the Portal Alias Dynamically 
                        command.Parameters.Add(parameterPortalAlias);
                        var parameterSettingName = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50);
                        parameterSettingName.Value = "SITESETTINGS_LANGLIST"; // Specify the SettingName 
                        command.Parameters.Add(parameterSettingName);

                        // Open the database connection and execute the command
                        connection.Open();

                        try
                        {
                            // Better null check here by Manu
                            var tmp = command.ExecuteScalar();

                            if (tmp != null)
                            {
                                langlist = tmp.ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            // Appleseed.Framework.Helpers.LogHelper.Logger.Log(Appleseed.Framework.Configuration.LogLevel.Warn, "Get languages from db", ex);
                            ErrorHandler.Publish(LogLevel.Warn, "Failed to get languages from database.", ex);

                            // Jes1111
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }

                    using (var command = new SqlCommand("rb_GetPortalSetting", connection))
                    {
                        // Mark the Command as a SPROC
                        command.CommandType = CommandType.StoredProcedure;

                        // Add Parameters to SPROC
                        var parameterPortalAlias = new SqlParameter("@PortalAlias", SqlDbType.NVarChar, 50);
                        parameterPortalAlias.Value = portalAlias; // Specify the Portal Alias Dynamically 
                        command.Parameters.Add(parameterPortalAlias);
                        var parameterSettingName = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50);
                        parameterSettingName.Value = "SITESETTINGS_DEFAULTLANG"; // Specify the SettingName 
                        command.Parameters.Add(parameterSettingName);

                        // Open the database connection and execute the command
                        connection.Open();

                        try
                        {
                            // Better null check here by Manu
                            var tmp = command.ExecuteScalar();

                            if (tmp != null)
                            {
                                defaultlang = tmp.ToString().Trim();
                            }
                        }
                        catch (Exception ex)
                        {
                            // Appleseed.Framework.Helpers.LogHelper.Logger.Log(Appleseed.Framework.Configuration.LogLevel.Warn, "Get languages from db", ex);
                            ErrorHandler.Publish(LogLevel.Warn, "Failed to get default language from database.", ex);

                            // Jes1111
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }

                if (langlist.Length == 0 && defaultlang.Length == 0)
                {
                    // jes1111 - langlist = ConfigurationSettings.AppSettings["DefaultLanguage"]; //default
                    langlist = Config.DefaultLanguage; // default
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.Append(defaultlang).Append(";"); // Add default lang as first item in lang list

                    foreach (var lang in langlist.Split(";".ToCharArray()))
                    {
                        var trimLang = lang.Trim();
                        if (trimLang.Length != 0 && trimLang != defaultlang)
                        {
                            // add non empty and non default languages in list
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
        /// <param name="mId">
        /// The module ID.
        /// </param>
        private static void SetActiveModuleCookie(int mId)
        {
            var cookie = new HttpCookie("ActiveModule", mId.ToString());
            var time = DateTime.Now;
            var span = new TimeSpan(0, 2, 0, 0, 0);
            cookie.Expires = time.Add(span);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// Recurses the portal pages XML.
        /// </summary>
        /// <param name="page">
        /// My page details.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private void RecursePortalPagesXml(PageStripDetails page, XmlWriter writer)
        {
            var children = page.Pages;
            var groupElementWritten = false;

            foreach (var mysubPage in children.Where(mysubPage => mysubPage.ParentPageID == page.PageID))
            {
                // if ( mySubPage.ParentPageID == page.PageID && PortalSecurity.IsInRoles(page.AuthorizedRoles) )
                if (!groupElementWritten)
                {
                    writer.WriteStartElement("MenuGroup"); // start MenuGroup element
                    groupElementWritten = true;
                }

                writer.WriteStartElement("MenuItem"); // start MenuItem element
                writer.WriteAttributeString("ParentPageId", mysubPage.ParentPageID.ToString());

                // writer.WriteAttributeString("Label",mySubPage.PageName);
                writer.WriteAttributeString(
                    "UrlPageName", 
                    HttpUrlBuilder.UrlPageName(mysubPage.PageID) == HttpUrlBuilder.DefaultPage
                        ? mysubPage.PageName
                        : HttpUrlBuilder.UrlPageName(mysubPage.PageID).Replace(".aspx", string.Empty));

                writer.WriteAttributeString("PageName", mysubPage.PageName);

                writer.WriteAttributeString("PageOrder", mysubPage.PageOrder.ToString());
                writer.WriteAttributeString("PageIndex", mysubPage.PageIndex.ToString());
                writer.WriteAttributeString("PageLayout", mysubPage.PageLayout);
                writer.WriteAttributeString("AuthRoles", mysubPage.AuthorizedRoles);
                writer.WriteAttributeString("ID", mysubPage.PageID.ToString());

                // writer.WriteAttributeString("URL",HttpUrlBuilder.BuildUrl(string.Concat("~/",mySubPage.PageName,".aspx"), mySubPage.PageID,0,null,string.Empty,this.PortalAlias,"hello/goodbye"));
                this.RecursePortalPagesXml(mysubPage, writer);
                writer.WriteEndElement(); // end MenuItem element
            }

            if (groupElementWritten)
            {
                writer.WriteEndElement(); // end MenuGroup element
            }
        }

        #endregion
    }
}