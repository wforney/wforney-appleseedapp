// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleSettings.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   The module settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.Site.Configuration
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.SqlClient;
    using System.Web.UI;

    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Settings.Cache;
    using Appleseed.Framework.Web.UI.WebControls;

    /// <summary>
    /// The module settings.
    /// </summary>
    public class ModuleSettings
    {
        // Fields
        #region Constants and Fields

        /// <summary>
        /// The admin.
        /// </summary>
        public bool Admin;

        /// <summary>
        /// The authorized add roles.
        /// </summary>
        public string AuthorizedAddRoles = "Admin;";

        /// <summary>
        /// The authorized approve roles.
        /// </summary>
        public string AuthorizedApproveRoles;

        /// <summary>
        /// The authorized delete module roles.
        /// </summary>
        public string AuthorizedDeleteModuleRoles = "Admin;";

        /// <summary>
        /// The authorized delete roles.
        /// </summary>
        public string AuthorizedDeleteRoles = "Admin;";

        /// <summary>
        /// The authorized edit roles.
        /// </summary>
        public string AuthorizedEditRoles = "Admin;";

        /// <summary>
        /// The authorized move module roles.
        /// </summary>
        public string AuthorizedMoveModuleRoles = "Admin;";

        /// <summary>
        /// The authorized properties roles.
        /// </summary>
        public string AuthorizedPropertiesRoles = "Admin;";

        /// <summary>
        /// The authorized publishing roles.
        /// </summary>
        public string AuthorizedPublishingRoles;

        /// <summary>
        /// The authorized view roles.
        /// </summary>
        public string AuthorizedViewRoles = "All Users;";

        /// <summary>
        /// The cache dependency.
        /// </summary>
        public ArrayList CacheDependency = new ArrayList();

        /// <summary>
        /// The cache time.
        /// </summary>
        public int CacheTime;

        /// <summary>
        /// The cacheable.
        /// </summary>
        public bool Cacheable;

        /// <summary>
        /// The desktop src.
        /// </summary>
        public string DesktopSrc = string.Empty;

        /// <summary>
        /// The guid id.
        /// </summary>
        public Guid GuidID;

        /// <summary>
        /// The mobile src.
        /// </summary>
        public string MobileSrc = string.Empty;

        /// <summary>
        /// The module def id.
        /// </summary>
        public int ModuleDefID;

        /// <summary>
        /// The module id.
        /// </summary>
        public int ModuleID;

        /// <summary>
        /// The module order.
        /// </summary>
        public int ModuleOrder;

        /// <summary>
        /// The module title.
        /// </summary>
        public string ModuleTitle = string.Empty;

        /// <summary>
        /// The page id.
        /// </summary>
        public int PageID;

        /// <summary>
        /// The pane name.
        /// </summary>
        public string PaneName = "no pane";

        /// <summary>
        /// The show every where.
        /// </summary>
        public bool ShowEveryWhere;

        /// <summary>
        /// The show mobile.
        /// </summary>
        public bool ShowMobile;

        /// <summary>
        /// The support collapsable.
        /// </summary>
        public bool SupportCollapsable;

        /// <summary>
        /// The support workflow.
        /// </summary>
        public bool SupportWorkflow;

        /// <summary>
        /// The workflow status.
        /// </summary>
        public WorkflowState WorkflowStatus;

/*
        /// <summary>
        /// The str at module id.
        /// </summary>
        private const string strATModuleID = "@ModuleID";
*/

/*
        /// <summary>
        /// The str admin.
        /// </summary>
        private const string strAdmin = "Admin;";
*/

        /// <summary>
        /// The str desktop src.
        /// </summary>
        private const string StringsDesktopSrc = "DesktopSrc";

        #endregion

        #region Public Methods

        /// <summary>
        /// The get module definition by id.
        /// </summary>
        /// <param name="moduleId">
        /// The module id.
        /// </param>
        /// <returns>
        /// </returns>
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
        /// The get module desktop src.
        /// </summary>
        /// <param name="moduleId">
        /// The module id.
        /// </param>
        /// <returns>
        /// The get module desktop src.
        /// </returns>
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
        /// The get module settings.
        /// </summary>
        /// <param name="moduleId">
        /// The module id.
        /// </param>
        /// <returns>
        /// </returns>
        public static Hashtable GetModuleSettings(int moduleId)
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
        public static Hashtable GetModuleSettings(int moduleId, Hashtable baseSettings)
        {
            if (!CurrentCache.Exists(Key.ModuleSettings(moduleId)))
            {
                var hashtable = new Hashtable();
                using (var connection = Config.SqlConnectionString)
                {
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
                }

                foreach (string str in baseSettings.Keys)
                {
                    if (hashtable[str] == null)
                    {
                        continue;
                    }

                    var item = (SettingItem)baseSettings[str];
                    if (hashtable[str].ToString().Length != 0)
                    {
                        item.Value = hashtable[str].ToString();
                    }
                }

                CurrentCache.Insert(Key.ModuleSettings(moduleId), baseSettings);
                return baseSettings;
            }

            baseSettings = (Hashtable)CurrentCache.Get(Key.ModuleSettings(moduleId));
            return baseSettings;
        }

        /// <summary>
        /// Gets the module settings.
        /// </summary>
        /// <param name="moduleId">The module id.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public static Hashtable GetModuleSettings(int moduleId, Page page)
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

            Hashtable moduleSettings;
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
        /// The update module setting.
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
        public static void UpdateModuleSetting(int moduleId, string key, string value)
        {
            using (var connection = Config.SqlConnectionString)
            {
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
            }

            CurrentCache.Remove(Key.ModuleSettings(moduleId));
        }

        #endregion
    }
}