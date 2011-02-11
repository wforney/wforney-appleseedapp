// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleSettings.cs" company="--">
//   Copyright © -- 2011. All Rights Reserved.
// </copyright>
// <summary>
//   The module settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.Site.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Web.UI;

    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Settings.Cache;
    using Appleseed.Framework.Web.UI.WebControls;

    /// <summary>
    /// The module settings.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class ModuleSettings : IModuleSettings
    {
        #region Constants and Fields

        /// <summary>
        ///   The string desktop source.
        /// </summary>
        private const string StringsDesktopSrc = "DesktopSrc";

        /// <summary>
        ///   The authorized add roles.
        /// </summary>
        private string authorizedAddRoles = "Admin;";

        /// <summary>
        ///   The authorized delete module roles.
        /// </summary>
        private string authorizedDeleteModuleRoles = "Admin;";

        /// <summary>
        ///   The authorized delete roles.
        /// </summary>
        private string authorizedDeleteRoles = "Admin;";

        /// <summary>
        ///   The authorized edit roles.
        /// </summary>
        private string authorizedEditRoles = "Admin;";

        /// <summary>
        ///   The authorized move module roles.
        /// </summary>
        private string authorizedMoveModuleRoles = "Admin;";

        /// <summary>
        ///   The authorized properties roles.
        /// </summary>
        private string authorizedPropertiesRoles = "Admin;";

        /// <summary>
        ///   The authorized view roles.
        /// </summary>
        private string authorizedViewRoles = "All Users;";

        /// <summary>
        ///   The cache dependency.
        /// </summary>
        private ArrayList cacheDependency = new ArrayList();

        /// <summary>
        ///   The desktop source.
        /// </summary>
        private string desktopSrc = string.Empty;

        /// <summary>
        ///   The mobile source.
        /// </summary>
        private string mobileSrc = string.Empty;

        /// <summary>
        ///   The module title.
        /// </summary>
        private string moduleTitle = string.Empty;

        /// <summary>
        ///   The pane name.
        /// </summary>
        private string paneName = "no pane";

        #endregion

        #region Properties

        /// <summary>
        ///   The admin.
        /// </summary>
        public bool Admin { get; set; }

        /// <summary>
        ///   The authorized add roles.
        /// </summary>
        public string AuthorizedAddRoles
        {
            get
            {
                return this.authorizedAddRoles;
            }

            set
            {
                this.authorizedAddRoles = value;
            }
        }

        /// <summary>
        ///   The authorized approve roles.
        /// </summary>
        public string AuthorizedApproveRoles { get; set; }

        /// <summary>
        ///   The authorized delete module roles.
        /// </summary>
        public string AuthorizedDeleteModuleRoles
        {
            get
            {
                return this.authorizedDeleteModuleRoles;
            }

            set
            {
                this.authorizedDeleteModuleRoles = value;
            }
        }

        /// <summary>
        ///   The authorized delete roles.
        /// </summary>
        public string AuthorizedDeleteRoles
        {
            get
            {
                return this.authorizedDeleteRoles;
            }

            set
            {
                this.authorizedDeleteRoles = value;
            }
        }

        /// <summary>
        ///   The authorized edit roles.
        /// </summary>
        public string AuthorizedEditRoles
        {
            get
            {
                return this.authorizedEditRoles;
            }

            set
            {
                this.authorizedEditRoles = value;
            }
        }

        /// <summary>
        ///   The authorized move module roles.
        /// </summary>
        public string AuthorizedMoveModuleRoles
        {
            get
            {
                return this.authorizedMoveModuleRoles;
            }

            set
            {
                this.authorizedMoveModuleRoles = value;
            }
        }

        /// <summary>
        ///   The authorized properties roles.
        /// </summary>
        public string AuthorizedPropertiesRoles
        {
            get
            {
                return this.authorizedPropertiesRoles;
            }

            set
            {
                this.authorizedPropertiesRoles = value;
            }
        }

        /// <summary>
        ///   The authorized publishing roles.
        /// </summary>
        public string AuthorizedPublishingRoles { get; set; }

        /// <summary>
        ///   The authorized view roles.
        /// </summary>
        public string AuthorizedViewRoles
        {
            get
            {
                return this.authorizedViewRoles;
            }

            set
            {
                this.authorizedViewRoles = value;
            }
        }

        /// <summary>
        ///   The cache dependency.
        /// </summary>
        public ArrayList CacheDependency
        {
            get
            {
                return this.cacheDependency;
            }

            set
            {
                this.cacheDependency = value;
            }
        }

        /// <summary>
        ///   The cache time.
        /// </summary>
        public int CacheTime { get; set; }

        /// <summary>
        ///   The cacheable.
        /// </summary>
        public bool Cacheable { get; set; }

        /// <summary>
        ///   The desktop source.
        /// </summary>
        public string DesktopSrc
        {
            get
            {
                return this.desktopSrc;
            }

            set
            {
                this.desktopSrc = value;
            }
        }

        /// <summary>
        ///   The guid id.
        /// </summary>
        public Guid GuidID { get; set; }

        /// <summary>
        ///   The mobile source.
        /// </summary>
        public string MobileSrc
        {
            get
            {
                return this.mobileSrc;
            }

            set
            {
                this.mobileSrc = value;
            }
        }

        /// <summary>
        ///   The module def id.
        /// </summary>
        public int ModuleDefID { get; set; }

        /// <summary>
        ///   The module id.
        /// </summary>
        public int ModuleID { get; set; }

        /// <summary>
        ///   The module order.
        /// </summary>
        public int ModuleOrder { get; set; }

        /// <summary>
        ///   The module title.
        /// </summary>
        public string ModuleTitle
        {
            get
            {
                return this.moduleTitle;
            }

            set
            {
                this.moduleTitle = value;
            }
        }

        /// <summary>
        ///   The page id.
        /// </summary>
        public int PageID { get; set; }

        /// <summary>
        ///   The pane name.
        /// </summary>
        public string PaneName
        {
            get
            {
                return this.paneName;
            }

            set
            {
                this.paneName = value;
            }
        }

        /// <summary>
        ///   The show every where.
        /// </summary>
        public bool ShowEveryWhere { get; set; }

        /// <summary>
        ///   The show mobile.
        /// </summary>
        public bool ShowMobile { get; set; }

        /// <summary>
        ///   The support collapsible.
        /// </summary>
        public bool SupportCollapsable { get; set; }

        /// <summary>
        ///   The support workflow.
        /// </summary>
        public bool SupportWorkflow { get; set; }

        /// <summary>
        ///   The workflow status.
        /// </summary>
        public WorkflowState WorkflowStatus { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the module definition by ID.
        /// </summary>
        /// <param name="moduleId">
        /// The module id.
        /// </param>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// </remarks>
        public static SqlDataReader GetModuleDefinitionByID(int moduleId)
        {
            var sqlConnectionString = Config.SqlConnectionString;
            var command = new SqlCommand("rb_GetModuleDefinitionByID", sqlConnectionString)
                {
                   CommandType = CommandType.StoredProcedure 
                };
            var parameter = new SqlParameter("@ModuleID", SqlDbType.Int) { Value = moduleId };
            command.Parameters.Add(parameter);
            sqlConnectionString.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// Gets the module desktop SRC.
        /// </summary>
        /// <param name="moduleId">
        /// The module id.
        /// </param>
        /// <returns>
        /// The get module desktop src.
        /// </returns>
        /// <remarks>
        /// </remarks>
        public static string GetModuleDesktopSrc(int moduleId)
        {
            var str = string.Empty;
            using (var reader = GetModuleDefinitionByID(moduleId))
            {
                if (reader.Read())
                {
                    str = Path.WebPathCombine(new[] { Path.ApplicationRoot, reader["DesktopSrc"].ToString() });
                }
            }

            return str;
        }

        /// <summary>
        /// Gets the module settings.
        /// </summary>
        /// <param name="moduleId">
        /// The module id.
        /// </param>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// </remarks>
        public static Dictionary<string, ISettingItem> GetModuleSettings(int moduleId)
        {
            return GetModuleSettings(moduleId, new Page());
        }

        /// <summary>
        /// Gets module settings.
        /// </summary>
        /// <param name="moduleId">
        /// The module id.
        /// </param>
        /// <param name="baseSettings">
        /// The base settings.
        /// </param>
        /// <returns>
        /// A hash table.
        /// </returns>
        /// <remarks>
        /// </remarks>
        public static Dictionary<string, ISettingItem> GetModuleSettings(
            int moduleId, Dictionary<string, ISettingItem> baseSettings)
        {
            if (!CurrentCache.Exists(Key.ModuleSettings(moduleId)))
            {
                var hashtable = new Hashtable();
                using (var connection = Config.SqlConnectionString)
                using (var command = new SqlCommand("rb_GetModuleSettings", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var parameter = new SqlParameter("@ModuleID", SqlDbType.Int, 4) { Value = moduleId };
                    command.Parameters.Add(parameter);
                    connection.Open();
                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            hashtable[reader["SettingName"].ToString()] = reader["SettingValue"].ToString();
                        }
                    }
                }

                foreach (var key in
                    baseSettings.Keys.Where(key => hashtable[key] != null).Where(
                        key => hashtable[key].ToString().Length != 0))
                {
                    baseSettings[key].Value = hashtable[key];
                }

                CurrentCache.Insert(Key.ModuleSettings(moduleId), baseSettings);
                return baseSettings;
            }

            baseSettings = (Dictionary<string, ISettingItem>)CurrentCache.Get(Key.ModuleSettings(moduleId));
            return baseSettings;
        }

        /// <summary>
        /// Gets the module settings.
        /// </summary>
        /// <param name="moduleId">
        /// The module id.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// </remarks>
        public static Dictionary<string, ISettingItem> GetModuleSettings(int moduleId, Page page)
        {
            var virtualPath = Path.ApplicationRoot + "/";
            var desktopSrc = string.Empty;
            using (var reader = GetModuleDefinitionByID(moduleId))
            {
                if (reader.Read())
                {
                    desktopSrc = reader[StringsDesktopSrc].ToString();
                }
            }

            virtualPath += desktopSrc;

            Dictionary<string, ISettingItem> moduleSettings;
            try
            {
                PortalModuleControl control;
                if (!virtualPath.ToLower().StartsWith("/areas/"))
                {
                    control = (PortalModuleControl)page.LoadControl(virtualPath);
                }
                else
                {
                    var strArray = virtualPath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    var areaName = (strArray[1].ToLower() == "views") ? string.Empty : strArray[1];
                    var controllerName = strArray[strArray.Length - 2];
                    var actionName = strArray[strArray.Length - 1];
                    var ns = strArray[2];

                    // string query = string.Format("?area={0}&controller={1}&action={2}", areaName, controllerName, actionName);
                    control = (PortalModuleControl)page.LoadControl("/DesktopModules/CoreModules/MVC/MVCModule.ascx");
                    ((MVCModuleControl)control).ControllerName = controllerName;
                    ((MVCModuleControl)control).ActionName = actionName;
                    ((MVCModuleControl)control).AreaName = areaName;
                    ((MVCModuleControl)control).Initialize();
                }

                moduleSettings = GetModuleSettings(moduleId, control.BaseSettings);
                control.InitializeCustomSettings();
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("There was a problem loading: '{0}'", virtualPath), exception);
            }

            return moduleSettings;
        }

        /// <summary>
        /// Updates the module setting.
        /// </summary>
        /// <param name="moduleId">
        /// The module id.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <remarks>
        /// </remarks>
        public static void UpdateModuleSetting(int moduleId, string key, string value)
        {
            using (var connection = Config.SqlConnectionString)
            using (var command = new SqlCommand("rb_UpdateModuleSetting", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                var parameter = new SqlParameter("@ModuleID", SqlDbType.Int, 4) { Value = moduleId };
                command.Parameters.Add(parameter);
                var parameter2 = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50) { Value = key };
                command.Parameters.Add(parameter2);
                var parameter3 = new SqlParameter("@SettingValue", SqlDbType.NVarChar, 0x5dc) { Value = value };
                command.Parameters.Add(parameter3);
                connection.Open();
                command.ExecuteNonQuery();
            }

            CurrentCache.Remove(Key.ModuleSettings(moduleId));
        }

        #endregion
    }
}