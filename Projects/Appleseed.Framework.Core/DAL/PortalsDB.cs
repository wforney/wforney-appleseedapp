using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Users.Data;
using Path=Appleseed.Framework.Settings.Path;

namespace Appleseed.Framework.Site.Data
{
    /// <summary>
    /// Class that encapsulates all data logic necessary to add/query/delete
    /// Portals within the Portal database.
    /// 
    /// </summary>
    public class PortalsDB
    {
        private const string stradmin = "admin";
        private const string strAdmins = "Admins;";
        private const string strAllUsers = "All Users";
        private const string strATAlwaysShowEditButton = "@AlwaysShowEditButton";
        private const string strATPortalID = "@PortalID";
        private const string strATPortalName = "@PortalName";
        private const string strATPortalPath = "@PortalPath";
        private const string strContentPane = "ContentPane";
        private const string strGUIDHTMLDocument = "{0B113F51-FEA3-499A-98E7-7B83C192FDBB}";
        private const string strGUIDLanguageSwitcher = "{25E3290E-3B9A-4302-9384-9CA01243C00F}";
        private const string strGUIDLogin = "{A0F1F62B-FDC7-4DE5-BBAD-A5DAF31D960A}";
        private const string strGUIDManageUsers = "{B6A48596-9047-4564-8555-61E3B31D7272}";
        private const string strGUIDModules = "{5E0DB0C7-FD54-4F55-ACF5-6ECF0EFA59C0}";
        private const string strGUIDSecurityRoles = "{A406A674-76EB-4BC1-BB35-50CD2C251F9C}";
        private const string strGUIDSiteSettings = "{EBBB01B1-FBB5-4E79-8FC4-59BCA1D0554E}";
        private const string strGUIDPages = "{1C575D94-70FC-4A83-80C3-2087F726CBB3}";
        private const string strLeftPane = "LeftPane";
        //jes1111 - const string strPortalsDirectory = "PortalsDirectory";
        private const string strrb_GetPortals = "rb_GetPortals";
        private const string strRightPane = "RightPane";

        /// <summary>
        /// The AddPortal method add a new portal.<br/>
        /// AddPortal Stored Procedure
        /// </summary>
        /// <param name="portalAlias">The portal alias.</param>
        /// <param name="portalName">Name of the portal.</param>
        /// <param name="portalPath">The portal path.</param>
        /// <returns></returns>
        public int AddPortal(string portalAlias, string portalName, string portalPath)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_AddPortal", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalAlias = new SqlParameter("@PortalAlias", SqlDbType.NVarChar, 128);
                    parameterPortalAlias.Value = portalAlias;
                    myCommand.Parameters.Add(parameterPortalAlias);
                    SqlParameter parameterPortalName = new SqlParameter(strATPortalName, SqlDbType.NVarChar, 128);
                    parameterPortalName.Value = portalName;
                    myCommand.Parameters.Add(parameterPortalName);
                    //jes1111
                    //					string pd = ConfigurationSettings.AppSettings[strPortalsDirectory];
                    //
                    //					if(pd!=null)
                    //					{
                    //					if (portalPath.IndexOf (pd) > -1)
                    //						portalPath = portalPath.Substring(portalPath.IndexOf (pd) + pd.Length);
                    //					}
                    string pd = Config.PortalsDirectory;
                    if (portalPath.IndexOf(pd) > -1)
                        portalPath = portalPath.Substring(portalPath.IndexOf(pd) + pd.Length);

                    SqlParameter parameterPortalPath = new SqlParameter(strATPortalPath, SqlDbType.NVarChar, 128);
                    parameterPortalPath.Value = portalPath;
                    myCommand.Parameters.Add(parameterPortalPath);
                    SqlParameter parameterAlwaysShow = new SqlParameter(strATAlwaysShowEditButton, SqlDbType.Bit, 1);
                    parameterAlwaysShow.Value = false;
                    myCommand.Parameters.Add(parameterAlwaysShow);
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPortalID);
                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }

                    finally
                    {
                        myConnection.Close();
                    }
                    return (int) parameterPortalID.Value;
                }
            }
        }

        private PortalSettings CurrentPortalSettings
        {
            get
            {
                return (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            }
        }

        /// <summary>
        /// The CreatePortal method create a new basic portal based on solutions table.
        /// </summary>
        /// <param name="solutionID">The solution ID.</param>
        /// <param name="portalAlias">The portal alias.</param>
        /// <param name="portalName">Name of the portal.</param>
        /// <param name="portalPath">The portal path.</param>
        /// <returns></returns>
        [
            History("john.mandia@whitelightsolutions.com", "2003/05/26",
                "Added extra info so that sign in is added to home tab of new portal and lang switcher is added to module list"
                )]
        [History("bja@reedtek.com", "2003/05/16", "Added extra parameter for collpasable window")]
        public int CreatePortal(int solutionID, string portalAlias, string portalName, string portalPath)
        {
            int portalID;
            PagesDB tabs = new PagesDB();
            ModulesDB modules = new ModulesDB();
            // Create a new portal
            portalID = AddPortal(portalAlias, portalName, portalPath);
            // get module definitions
            SqlDataReader myReader;
            myReader = modules.GetSolutionModuleDefinitions(solutionID);

            // Always call Read before accessing data.
            try
            {
                while (myReader.Read())
                {
                    modules.UpdateModuleDefinitions(new Guid(myReader["GeneralModDefID"].ToString()), portalID, true);
                }
            }

            finally
            {
                myReader.Close(); //by Manu, fixed bug 807858
            }

            if ( !Config.UseSingleUserBase ) {
                string AdminEmail = "admin@Appleseedportal.net";
                
                // Create the stradmin User for the new portal
                UsersDB User = new UsersDB();
                // Create the "Admins" role for the new portal
                Guid roleID = User.AddRole( portalAlias, "Admins" );
                Guid userID = User.AddUser( stradmin, AdminEmail, stradmin, portalAlias );
                // Create a new row in a many to many table (userroles)
                // giving the "admins" role to the stradmin user
                User.AddUserRole( roleID, userID, portalAlias );
            }
            // Create a new Page "home"
            int homePageID = tabs.AddPage(portalID, "Home", 1);
            // Create a new Page "admin"
            string localizedString = General.GetString("ADMIN_TAB_NAME");
            int adminPageID = tabs.AddPage(portalID, localizedString, strAdmins, 9999);
            // Add Modules for portal use
            // Html Document
            modules.UpdateModuleDefinitions(new Guid(strGUIDHTMLDocument), portalID, true);
            // Add Modules for portal administration
            // Site Settings (Admin)
            localizedString = General.GetString("MODULE_SITE_SETTINGS");
            modules.UpdateModuleDefinitions(new Guid(strGUIDSiteSettings), portalID, true);
            modules.AddModule(adminPageID, 1, strContentPane, localizedString,
                              modules.GetModuleDefinitionByGuid(portalID, new Guid(strGUIDSiteSettings)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // Pages (Admin)
            localizedString = General.GetString("MODULE_TABS");
            modules.UpdateModuleDefinitions(new Guid(strGUIDPages), portalID, true);
            modules.AddModule(adminPageID, 2, strContentPane, localizedString,
                              modules.GetModuleDefinitionByGuid(portalID, new Guid(strGUIDPages)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // Roles (Admin)
            localizedString = General.GetString("MODULE_SECURITY_ROLES");
            modules.UpdateModuleDefinitions(new Guid(strGUIDSecurityRoles), portalID, true);
            modules.AddModule(adminPageID, 3, strContentPane, localizedString,
                              modules.GetModuleDefinitionByGuid(portalID, new Guid(strGUIDSecurityRoles)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // Manage Users (Admin)
            localizedString = General.GetString("MODULE_MANAGE_USERS");
            modules.UpdateModuleDefinitions(new Guid(strGUIDManageUsers), portalID, true);
            modules.AddModule(adminPageID, 4, strContentPane, localizedString,
                              modules.GetModuleDefinitionByGuid(portalID, new Guid(strGUIDManageUsers)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // Module Definitions (Admin)
            localizedString = General.GetString("MODULE_MODULES");
            modules.UpdateModuleDefinitions(new Guid(strGUIDModules), portalID, true);
            modules.AddModule(adminPageID, 1, strRightPane, localizedString,
                              modules.GetModuleDefinitionByGuid(portalID, new Guid(strGUIDModules)), 0, strAdmins,
                              strAllUsers, strAdmins, strAdmins, strAdmins, strAdmins, strAdmins, false, string.Empty,
                              false, false, false);
            // End Change Geert.Audenaert@Syntegra.Com
            // Change by john.mandia@whitelightsolutions.com
            // Add Signin Module and put it on the hometab
            // Signin
            localizedString = General.GetString("MODULE_LOGIN", "Login");
            modules.UpdateModuleDefinitions(new Guid(strGUIDLogin), portalID, true);
            modules.AddModule(homePageID, -1, strLeftPane, localizedString,
                              modules.GetModuleDefinitionByGuid(portalID, new Guid(strGUIDLogin)), 0, strAdmins,
                              "Unauthenticated Users;Admins;", strAdmins, strAdmins, strAdmins, strAdmins, strAdmins,
                              false, string.Empty, false, false, false);
            // Add language switcher to available modules
            // Language Switcher
            modules.UpdateModuleDefinitions(new Guid(strGUIDLanguageSwitcher), portalID, true);
            // End of change by john.mandia@whitelightsolutions.com
            // Create paths
            CreatePortalPath(portalPath);
            return portalID;
        }

        /// <summary>
        /// Creates the portal path.
        /// </summary>
        /// <param name="portalPath">The portal path.</param>
        public void CreatePortalPath(string portalPath)
        {
            portalPath = portalPath.Replace("/", string.Empty);
            portalPath = portalPath.Replace("\\", string.Empty);
            portalPath = portalPath.Replace(".", string.Empty);

            if (!portalPath.StartsWith("_"))
                portalPath = "_" + portalPath;
            // jes1111			
            //			string pd = ConfigurationSettings.AppSettings[strPortalsDirectory];
            //
            //			if(pd!=null)
            //			{
            //			if (portalPath.IndexOf (pd) > -1)
            //				portalPath = portalPath.Substring(portalPath.IndexOf (pd) + pd.Length);
            //			}
            string pd = Config.PortalsDirectory;
            if (portalPath.IndexOf(pd) > -1)
                portalPath = portalPath.Substring(portalPath.IndexOf(pd) + pd.Length);

            // jes1111 - string portalPhisicalDir = HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/" + ConfigurationSettings.AppSettings[strPortalsDirectory] + "/" + portalPath);
            string portalPhisicalDir =
                HttpContext.Current.Server.MapPath(
                    Path.WebPathCombine(Path.ApplicationRoot, Config.PortalsDirectory, portalPath));
            if (!Directory.Exists(portalPhisicalDir))
                Directory.CreateDirectory(portalPhisicalDir);
            // Subdirs
            string[] subdirs = {"images", "polls", "documents", "xml"};

            for (int i = 0; i <= subdirs.GetUpperBound(0); i++)

                if (!Directory.Exists(portalPhisicalDir + "\\" + subdirs[i]))
                    Directory.CreateDirectory(portalPhisicalDir + "\\" + subdirs[i]);
        }

        /// <summary>
        /// Removes portal from database. All tabs, modules and data wil be removed.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        public void DeletePortal(int portalID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_DeletePortal", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);
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
        }

        /// <summary>
        /// The GetPortals method returns a SqlDataReader containing all of the
        /// Portals registered in this database.<br/>
        /// GetPortals Stored Procedure
        /// </summary>
        /// <returns></returns>
        // TODO --> [Obsolete("Replace me")]
        public SqlDataReader GetPortals()
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetPortals", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Execute the command
            myConnection.Open();
            SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the datareader 
            return result;
        }

        /// <summary>
        /// The GetPortals method returns an ArrayList containing all of the
        /// Portals registered in this database.<br/>
        /// GetPortals Stored Procedure
        /// </summary>
        /// <returns>portals</returns>
        public ArrayList GetPortalsArrayList()
        {
            ArrayList portals = new ArrayList();

            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand(strrb_GetPortals, myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Execute the command
                    myConnection.Open();

                    using (SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        try
                        {
                            while (dr.Read())
                            {
                                PortalItem p = new PortalItem();
                                p.Name = dr["PortalName"].ToString();
                                p.Path = dr["PortalPath"].ToString();
                                p.ID = Convert.ToInt32(dr["PortalID"].ToString());
                                portals.Add(p);
                            }
                        }

                        finally
                        {
                            dr.Close(); //by Manu, fixed bug 807858
                        }
                    }
                    // Return the portals
                    return portals;
                }
            }
        }

        /// <summary>
        /// The GetTemplates method returns a SqlDataReader containing all of the
        /// Templates Availables.
        /// </summary>
        /// <returns></returns>
        // TODO --> [Obsolete("Replace me")]
        public SqlDataReader GetTemplates()
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand(strrb_GetPortals, myConnection);
            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;
            // Execute the command
            myConnection.Open();
            SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            // Return the datareader 
            return result;
        }

        /// <summary>
        /// The UpdatePortalInfo method updates the name and access settings for the portal.<br/>
        /// Uses UpdatePortalInfo Stored Procedure.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="portalName">Name of the portal.</param>
        /// <param name="portalPath">The portal path.</param>
        /// <param name="alwaysShow">if set to <c>true</c> [always show].</param>
        public void UpdatePortalInfo(int portalID, string portalName, string portalPath, bool alwaysShow)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_UpdatePortalInfo", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);
                    SqlParameter parameterPortalName = new SqlParameter(strATPortalName, SqlDbType.NVarChar, 128);
                    parameterPortalName.Value = portalName;
                    myCommand.Parameters.Add(parameterPortalName);
                    // jes1111
                    //					string pd = ConfigurationSettings.AppSettings[strPortalsDirectory];
                    //
                    //					if(pd!=null)
                    //					{
                    //						if (portalPath.IndexOf (pd) > -1)
                    //							portalPath = portalPath.Substring(portalPath.IndexOf (pd) + pd.Length);
                    //					}
                    string pd = Config.PortalsDirectory;
                    if (portalPath.IndexOf(pd) > -1)
                        portalPath = portalPath.Substring(portalPath.IndexOf(pd) + pd.Length);

                    SqlParameter parameterPortalPath = new SqlParameter(strATPortalPath, SqlDbType.NVarChar, 128);
                    parameterPortalPath.Value = portalPath;
                    myCommand.Parameters.Add(parameterPortalPath);
                    SqlParameter parameterAlwaysShow = new SqlParameter(strATAlwaysShowEditButton, SqlDbType.Bit, 1);
                    parameterAlwaysShow.Value = alwaysShow;
                    myCommand.Parameters.Add(parameterAlwaysShow);
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
        }

        /// <summary>
        /// The UpdatePortalSetting Method updates a single module setting
        /// in the PortalSettings database table.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        [Obsolete("UpdatePortalSetting was moved in PortalSettings.UpdatePortalSetting", false)]
        public void UpdatePortalSetting(int portalID, string key, string value)
        {
            PortalSettings.UpdatePortalSetting(portalID, key, value);
        }
    }
}