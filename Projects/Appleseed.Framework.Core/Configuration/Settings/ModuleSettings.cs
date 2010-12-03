using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Settings.Cache;
using Appleseed.Framework;
using Appleseed.Framework.Web.UI.WebControls;
using System.Web;
using System.Reflection;
using System.Web.Routing;

namespace Appleseed.Framework.Site.Configuration
{

    public class ModuleSettings
    {
        // Fields
        public bool Admin;
        public string AuthorizedAddRoles = "Admin;";
        public string AuthorizedApproveRoles;
        public string AuthorizedDeleteModuleRoles = "Admin;";
        public string AuthorizedDeleteRoles = "Admin;";
        public string AuthorizedEditRoles = "Admin;";
        public string AuthorizedMoveModuleRoles = "Admin;";
        public string AuthorizedPropertiesRoles = "Admin;";
        public string AuthorizedPublishingRoles;
        public string AuthorizedViewRoles = "All Users;";
        public bool Cacheable;
        public ArrayList CacheDependency = new ArrayList();
        public int CacheTime = 0;
        public string DesktopSrc = string.Empty;
        public Guid GuidID;
        public string MobileSrc = string.Empty;
        public int ModuleDefID;
        public int ModuleID = 0;
        public int ModuleOrder = 0;
        public string ModuleTitle = string.Empty;
        public int PageID;
        public string PaneName = "no pane";
        public bool ShowEveryWhere;
        public bool ShowMobile = false;
        private const string strAdmin = "Admin;";
        private const string strATModuleID = "@ModuleID";
        private const string strDesktopSrc = "DesktopSrc";
        public bool SupportCollapsable = false;
        public bool SupportWorkflow = false;
        public WorkflowState WorkflowStatus;

        // Methods
        public static SqlDataReader GetModuleDefinitionByID(int ModuleID)
        {
            SqlConnection sqlConnectionString = Config.SqlConnectionString;
            SqlCommand command = new SqlCommand("rb_GetModuleDefinitionByID", sqlConnectionString);
            command.CommandType = CommandType.StoredProcedure;
            SqlParameter parameter = new SqlParameter("@ModuleID", SqlDbType.Int);
            parameter.Value = ModuleID;
            command.Parameters.Add(parameter);
            sqlConnectionString.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public static string GetModuleDesktopSrc(int moduleID)
        {
            string str = string.Empty;
            using (SqlDataReader reader = GetModuleDefinitionByID(moduleID))
            {
                if (reader.Read())
                {
                    str = Path.WebPathCombine(new string[] { Path.ApplicationRoot, reader["DesktopSrc"].ToString() });
                }
            }
            return str;
        }

        public static Hashtable GetModuleSettings(int moduleID)
        {
            return GetModuleSettings(moduleID, new Page());
        }

        public static Hashtable GetModuleSettings(int moduleID, Hashtable _baseSettings)
        {
            if (!CurrentCache.Exists(Key.ModuleSettings(moduleID)))
            {
                Hashtable hashtable = new Hashtable();
                using (SqlConnection connection = Config.SqlConnectionString)
                {
                    using (SqlCommand command = new SqlCommand("rb_GetModuleSettings", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlParameter parameter = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
                        parameter.Value = moduleID;
                        command.Parameters.Add(parameter);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                                hashtable[reader["SettingName"].ToString()] = reader["SettingValue"].ToString();
                            }
                        }
                    }
                }
                foreach (string str in _baseSettings.Keys)
                {
                    if (hashtable[str] != null)
                    {
                        SettingItem item = (SettingItem)_baseSettings[str];
                        if (hashtable[str].ToString().Length != 0)
                        {
                            item.Value = hashtable[str].ToString();
                        }
                    }
                }
                CurrentCache.Insert(Key.ModuleSettings(moduleID), _baseSettings);
                return _baseSettings;
            }
            _baseSettings = (Hashtable)CurrentCache.Get(Key.ModuleSettings(moduleID));
            return _baseSettings;
        }

        public static Hashtable GetModuleSettings(int moduleID, Page page)
        {
            string virtualPath = Path.ApplicationRoot + "/";
            string desktopSrc = string.Empty;
            using (SqlDataReader reader = GetModuleDefinitionByID(moduleID))
            {
                if (reader.Read())
                {
                    desktopSrc = reader[strDesktopSrc].ToString();

                }
            }
            virtualPath += desktopSrc;

            Hashtable moduleSettings = null;
            try
            {
                PortalModuleControl control;
                if (!virtualPath.ToString().ToLower().StartsWith("/areas/"))
                {
                    control = (PortalModuleControl)page.LoadControl(virtualPath);
                }
                else
                {
                 

                    string[] strArray = virtualPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    string areaName = (strArray[1].ToLower() == "views") ? string.Empty : strArray[1];
                    string controllerName = strArray[strArray.Length - 2];
                    string actionName = strArray[strArray.Length - 1];
                    string ns = strArray[2];

                    //string query = string.Format("?area={0}&controller={1}&action={2}", areaName, controllerName, actionName);
                    control = (PortalModuleControl)page.LoadControl("/DesktopModules/CoreModules/MVC/MVCModule.ascx");
                    (control as MVCModuleControl).ControllerName = controllerName;
                    (control as MVCModuleControl).ActionName = actionName;
                    (control as MVCModuleControl).AreaName = areaName;
                    (control as MVCModuleControl).Initialize();
                }

                moduleSettings = GetModuleSettings(moduleID, control.BaseSettings);
                control.InitializeCustomSettings();
            }
            catch (Exception exception)
            {
                throw new Exception("There was a problem loading: '" + virtualPath + "'", exception);
            }

            return moduleSettings;
        }

       

        public static void UpdateModuleSetting(int moduleID, string key, string value)
        {
            using (SqlConnection connection = Config.SqlConnectionString)
            {
                using (SqlCommand command = new SqlCommand("rb_UpdateModuleSetting", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    SqlParameter parameter = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
                    parameter.Value = moduleID;
                    command.Parameters.Add(parameter);
                    SqlParameter parameter2 = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50);
                    parameter2.Value = key;
                    command.Parameters.Add(parameter2);
                    SqlParameter parameter3 = new SqlParameter("@SettingValue", SqlDbType.NVarChar, 0x5dc);
                    parameter3.Value = value;
                    command.Parameters.Add(parameter3);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            CurrentCache.Remove(Key.ModuleSettings(moduleID));
        }
    }
}