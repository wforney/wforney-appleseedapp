// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModulesDB.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   Class that encapsulates all data logic necessary to add/query/delete
//   configuration, layout and security settings values within the Portal database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.Site.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Web;
    using System.Web.UI.WebControls;

    using Appleseed.Framework.Core.BLL;
    using Appleseed.Framework.Data;
    using Appleseed.Framework.Helpers;
    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Settings.Cache;
    using Appleseed.Framework.Site.Configuration;

    /// <summary>
    /// Class that encapsulates all data logic necessary to add/query/delete
    ///   configuration, layout and security settings values within the Portal database.
    /// </summary>
    public class ModulesDB
    {
        #region Constants and Fields

        /// <summary>
        /// The str at add roles.
        /// </summary>
        private const string strATAddRoles = "@AddRoles";

        /// <summary>
        /// The str at admin.
        /// </summary>
        private const string strATAdmin = "@Admin";

        /// <summary>
        /// The str at approval roles.
        /// </summary>
        private const string strATApprovalRoles = "@ApprovalRoles";

        /// <summary>
        /// The str at assembly name.
        /// </summary>
        private const string strATAssemblyName = "@AssemblyName";

        /// <summary>
        /// The str at cache time.
        /// </summary>
        private const string strATCacheTime = "@CacheTime";

        /// <summary>
        /// The str at class name.
        /// </summary>
        private const string strATClassName = "@ClassName";

        /// <summary>
        /// The str at delete module roles.
        /// </summary>
        private const string strATDeleteModuleRoles = "@DeleteModuleRoles";

        /// <summary>
        /// The str at delete roles.
        /// </summary>
        private const string strATDeleteRoles = "@DeleteRoles";

        /// <summary>
        /// The str at desktop src.
        /// </summary>
        private const string strATDesktopSrc = "@DesktopSrc";

        /// <summary>
        /// The str at edit roles.
        /// </summary>
        private const string strATEditRoles = "@EditRoles";

        /// <summary>
        /// The str at friendly name.
        /// </summary>
        private const string strATFriendlyName = "@FriendlyName";

        /// <summary>
        /// The str at general mod def id.
        /// </summary>
        private const string strATGeneralModDefID = "@GeneralModDefID";

        /// <summary>
        /// The str at guid.
        /// </summary>
        private const string strATGuid = "@Guid";

        /// <summary>
        /// The str at mobile src.
        /// </summary>
        private const string strATMobileSrc = "@MobileSrc";

        /// <summary>
        /// The str at module def id.
        /// </summary>
        private const string strATModuleDefID = "@ModuleDefID";

        /// <summary>
        /// The str at module id.
        /// </summary>
        private const string strATModuleID = "@ModuleID";

        /// <summary>
        /// The str at module order.
        /// </summary>
        private const string strATModuleOrder = "@ModuleOrder";

        /// <summary>
        /// The str at module title.
        /// </summary>
        private const string strATModuleTitle = "@ModuleTitle";

        /// <summary>
        /// The str at move module roles.
        /// </summary>
        private const string strATMoveModuleRoles = "@MoveModuleRoles";

        /// <summary>
        /// The str at page id.
        /// </summary>
        private const string strATPageID = "@TabID";

        /// <summary>
        /// The str at pane name.
        /// </summary>
        private const string strATPaneName = "@PaneName";

        /// <summary>
        /// The str at portal id.
        /// </summary>
        private const string strATPortalID = "@PortalID";

        /// <summary>
        /// The str at properties roles.
        /// </summary>
        private const string strATPropertiesRoles = "@PropertiesRoles";

        /// <summary>
        /// The str at publishing roles.
        /// </summary>
        private const string strATPublishingRoles = "@PublishingRoles";

        /// <summary>
        /// The str at searchable.
        /// </summary>
        private const string strATSearchable = "@Searchable";

        /// <summary>
        /// The str at show every where.
        /// </summary>
        private const string strATShowEveryWhere = "@ShowEveryWhere";

        /// <summary>
        /// The str at show mobile.
        /// </summary>
        private const string strATShowMobile = "@ShowMobile";

        /// <summary>
        /// The str at support collapsable.
        /// </summary>
        private const string strATSupportCollapsable = "@SupportCollapsable";

        /// <summary>
        /// The str at support workflow.
        /// </summary>
        private const string strATSupportWorkflow = "@SupportWorkflow";

        /// <summary>
        /// The str at view roles.
        /// </summary>
        private const string strATViewRoles = "@ViewRoles";

        // const string strGUID = "GUID";
        /// <summary>
        /// The str no module.
        /// </summary>
        private const string strNoModule = "NO_MODULE";

        /// <summary>
        /// The strrb_ get modules in page.
        /// </summary>
        private const string strrb_GetModulesInPage = "rb_GetModulesInTab";

        #endregion

        #region Public Methods

        /// <summary>
        /// AddGeneralModuleDefinitions
        /// </summary>
        /// <param name="GeneralModDefID">
        /// GeneralModDefID
        /// </param>
        /// <param name="FriendlyName">
        /// Name of the friendly.
        /// </param>
        /// <param name="DesktopSrc">
        /// The desktop SRC.
        /// </param>
        /// <param name="MobileSrc">
        /// The mobile SRC.
        /// </param>
        /// <param name="AssemblyName">
        /// Name of the assembly.
        /// </param>
        /// <param name="ClassName">
        /// Name of the class.
        /// </param>
        /// <param name="Admin">
        /// if set to <c>true</c> [admin].
        /// </param>
        /// <param name="Searchable">
        /// if set to <c>true</c> [searchable].
        /// </param>
        /// <returns>
        /// The newly created ID
        /// </returns>
        public Guid AddGeneralModuleDefinitions(
            Guid GeneralModDefID,
            string FriendlyName,
            string DesktopSrc,
            string MobileSrc,
            string AssemblyName,
            string ClassName,
            bool Admin,
            bool Searchable)
        {
            using (var myConnection = Config.SqlConnectionString)
            {
                using (var myCommand = new SqlCommand("rb_AddGeneralModuleDefinitions", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    var parameterGeneralModDefID = new SqlParameter(strATGeneralModDefID, SqlDbType.UniqueIdentifier);
                    parameterGeneralModDefID.Value = GeneralModDefID;
                    myCommand.Parameters.Add(parameterGeneralModDefID);
                    var parameterFriendlyName = new SqlParameter(strATFriendlyName, SqlDbType.NVarChar, 128);
                    parameterFriendlyName.Value = FriendlyName;
                    myCommand.Parameters.Add(parameterFriendlyName);
                    var parameterDesktopSrc = new SqlParameter(strATDesktopSrc, SqlDbType.NVarChar, 256);
                    parameterDesktopSrc.Value = DesktopSrc;
                    myCommand.Parameters.Add(parameterDesktopSrc);
                    var parameterMobileSrc = new SqlParameter(strATMobileSrc, SqlDbType.NVarChar, 256);
                    parameterMobileSrc.Value = MobileSrc;
                    myCommand.Parameters.Add(parameterMobileSrc);
                    var parameterAssemblyName = new SqlParameter(strATAssemblyName, SqlDbType.VarChar, 50);
                    parameterAssemblyName.Value = AssemblyName;
                    myCommand.Parameters.Add(parameterAssemblyName);
                    var parameterClassName = new SqlParameter(strATClassName, SqlDbType.NVarChar, 128);
                    parameterClassName.Value = ClassName;
                    myCommand.Parameters.Add(parameterClassName);
                    var parameterAdmin = new SqlParameter(strATAdmin, SqlDbType.Bit);
                    parameterAdmin.Value = Admin;
                    myCommand.Parameters.Add(parameterAdmin);
                    var parameterSearchable = new SqlParameter(strATSearchable, SqlDbType.Bit);
                    parameterSearchable.Value = Searchable;
                    myCommand.Parameters.Add(parameterSearchable);

                    // Open the database connection and execute the command
                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in AddGeneralModuleDefinitions. ", ex);
                    }

                    // Return the newly created ID
                    return new Guid(parameterGeneralModDefID.Value.ToString());
                }
            }
        }

        /// <summary>
        /// The AddModule method updates a specified Module within the Modules database table.
        ///   If the module does not yet exist,the stored procedure adds it.<br/>
        ///   AddModule Stored Procedure
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <param name="moduleOrder">
        /// The module order.
        /// </param>
        /// <param name="paneName">
        /// Name of the pane.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="moduleDefId">
        /// The module def ID.
        /// </param>
        /// <param name="cacheTime">
        /// The cache time.
        /// </param>
        /// <param name="editRoles">
        /// The edit roles.
        /// </param>
        /// <param name="viewRoles">
        /// The view roles.
        /// </param>
        /// <param name="addRoles">
        /// The add roles.
        /// </param>
        /// <param name="deleteRoles">
        /// The delete roles.
        /// </param>
        /// <param name="propertiesRoles">
        /// The properties roles.
        /// </param>
        /// <param name="moveModuleRoles">
        /// The move module roles.
        /// </param>
        /// <param name="deleteModuleRoles">
        /// The delete module roles.
        /// </param>
        /// <param name="showMobile">
        /// if set to <c>true</c> [show mobile].
        /// </param>
        /// <param name="publishingRoles">
        /// The publishing roles.
        /// </param>
        /// <param name="supportWorkflow">
        /// if set to <c>true</c> [support workflow].
        /// </param>
        /// <param name="showEveryWhere">
        /// if set to <c>true</c> [show every where].
        /// </param>
        /// <param name="supportCollapsable">
        /// if set to <c>true</c> [support collapsible].
        /// </param>
        /// <returns>
        /// The add module.
        /// </returns>
        [History("jviladiu@portalServices.net", "2004/08/19", "Added support for move & delete modules roles")]
        [History("john.mandia@whitelightsolutions.com", "2003/05/24", "Added support for showEveryWhere")]
        [History("bja@reedtek.com", "2003/05/16", "Added support for win. mgmt min/max/close -- supportCollapsable")]
        public int AddModule(
            int pageId,
            int moduleOrder,
            string paneName,
            string title,
            int moduleDefId,
            int cacheTime,
            string editRoles,
            string viewRoles,
            string addRoles,
            string deleteRoles,
            string propertiesRoles,
            string moveModuleRoles,
            string deleteModuleRoles,
            bool showMobile,
            string publishingRoles,
            bool supportWorkflow,
            bool showEveryWhere,
            bool supportCollapsable)
        {
            // Changes by Geert.Audenaert@Syntegra.Com Date: 6/2/2003
            // Create Instance of Connection and Command Object
            using (var myConnection = Config.SqlConnectionString)
            {
                using (var myCommand = new SqlCommand("rb_AddModule", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    var parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
                    parameterModuleID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterModuleID);
                    var parameterModuleDefinitionID = new SqlParameter(strATModuleDefID, SqlDbType.Int, 4);
                    parameterModuleDefinitionID.Value = moduleDefId;
                    myCommand.Parameters.Add(parameterModuleDefinitionID);
                    var parameterPageID = new SqlParameter(strATPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageId;
                    myCommand.Parameters.Add(parameterPageID);
                    var parameterModuleOrder = new SqlParameter(strATModuleOrder, SqlDbType.Int, 4);
                    parameterModuleOrder.Value = moduleOrder;
                    myCommand.Parameters.Add(parameterModuleOrder);
                    var parameterTitle = new SqlParameter(strATModuleTitle, SqlDbType.NVarChar, 256);
                    parameterTitle.Value = title;
                    myCommand.Parameters.Add(parameterTitle);
                    var parameterPaneName = new SqlParameter(strATPaneName, SqlDbType.NVarChar, 256);
                    parameterPaneName.Value = paneName;
                    myCommand.Parameters.Add(parameterPaneName);
                    var parameterCacheTime = new SqlParameter(strATCacheTime, SqlDbType.Int, 4);
                    parameterCacheTime.Value = cacheTime;
                    myCommand.Parameters.Add(parameterCacheTime);
                    var parameterEditRoles = new SqlParameter(strATEditRoles, SqlDbType.NVarChar, 256);
                    parameterEditRoles.Value = editRoles;
                    myCommand.Parameters.Add(parameterEditRoles);
                    var parameterViewRoles = new SqlParameter(strATViewRoles, SqlDbType.NVarChar, 256);
                    parameterViewRoles.Value = viewRoles;
                    myCommand.Parameters.Add(parameterViewRoles);
                    var parameterAddRoles = new SqlParameter(strATAddRoles, SqlDbType.NVarChar, 256);
                    parameterAddRoles.Value = addRoles;
                    myCommand.Parameters.Add(parameterAddRoles);
                    var parameterDeleteRoles = new SqlParameter(strATDeleteRoles, SqlDbType.NVarChar, 256);
                    parameterDeleteRoles.Value = deleteRoles;
                    myCommand.Parameters.Add(parameterDeleteRoles);
                    var parameterPropertiesRoles = new SqlParameter(strATPropertiesRoles, SqlDbType.NVarChar, 256);
                    parameterPropertiesRoles.Value = propertiesRoles;
                    myCommand.Parameters.Add(parameterPropertiesRoles);

                    // Added by jviladiu@portalservices.net (19/08/2004)
                    var parameterMoveModuleRoles = new SqlParameter(strATMoveModuleRoles, SqlDbType.NVarChar, 256);
                    parameterMoveModuleRoles.Value = moveModuleRoles;
                    myCommand.Parameters.Add(parameterMoveModuleRoles);

                    // Added by jviladiu@portalservices.net (19/08/2004)
                    var parameterDeleteModuleRoles = new SqlParameter(strATDeleteModuleRoles, SqlDbType.NVarChar, 256);
                    parameterDeleteModuleRoles.Value = deleteModuleRoles;
                    myCommand.Parameters.Add(parameterDeleteModuleRoles);

                    // Change by Geert.Audenaert@Syntegra.Com
                    // Date: 6/2/2003
                    var parameterPublishingRoles = new SqlParameter(strATPublishingRoles, SqlDbType.NVarChar, 256);
                    parameterPublishingRoles.Value = publishingRoles;
                    myCommand.Parameters.Add(parameterPublishingRoles);
                    var parameterSupportWorkflow = new SqlParameter(strATSupportWorkflow, SqlDbType.Bit, 1);
                    parameterSupportWorkflow.Value = supportWorkflow;
                    myCommand.Parameters.Add(parameterSupportWorkflow);

                    // End Change Geert.Audenaert@Syntegra.Com
                    var parameterShowMobile = new SqlParameter(strATShowMobile, SqlDbType.Bit, 1);
                    parameterShowMobile.Value = showMobile;
                    myCommand.Parameters.Add(parameterShowMobile);

                    // Start Change john.mandia@whitelightsolutions.com
                    var parameterShowEveryWhere = new SqlParameter(strATShowEveryWhere, SqlDbType.Bit, 1);
                    parameterShowEveryWhere.Value = showEveryWhere;
                    myCommand.Parameters.Add(parameterShowEveryWhere);

                    // End Change  john.mandia@whitelightsolutions.com
                    // Start Change bja@reedtek.com
                    var parameterSupportCollapsable = new SqlParameter(strATSupportCollapsable, SqlDbType.Bit, 1);
                    parameterSupportCollapsable.Value = supportCollapsable;
                    myCommand.Parameters.Add(parameterSupportCollapsable);

                    // End Change  bja@reedtek.com
                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // ErrorHandler.Publish(Appleseed.Framework.LogLevel.Warn, "An Error Occurred in AddModule. ", ex);
                        ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in AddModule. ", ex);
                    }

                    return (int)parameterModuleID.Value;
                }
            }
        }

        /// <summary>
        /// The DeleteModule method deletes a specified Module from the Modules database table.<br/>
        ///   DeleteModule Stored Procedure
        /// </summary>
        /// <param name="moduleID">
        /// The module ID.
        /// </param>
        [History("JB - john@bowenweb.com", "2005/05/12", "Added support for Recycler module")]
        public void DeleteModule(int moduleID)
        {
            // BOWEN 11 June 2005 - BEGIN
            var portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            var useRecycler =
                bool.Parse(
                    PortalSettings.GetPortalCustomSettings(
                        portalSettings.PortalID, PortalSettings.GetPortalBaseSettings(portalSettings.PortalPath))[
                            "SITESETTINGS_USE_RECYCLER"].ToString());

            // TODO: THIS LINE DISABLES THE RECYCLER DUE SOME TROUBLES WITH IT !!!!!! Fix those troubles and then discomment.
            useRecycler = false;

            using (var myConnection = Config.SqlConnectionString)
            {
                using (
                    var myCommand = new SqlCommand(
                        useRecycler ? "rb_DeleteModuleToRecycler" : "rb_DeleteModule", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    var parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
                    parameterModuleID.Value = moduleID;
                    myCommand.Parameters.Add(parameterModuleID);

                    if (useRecycler)
                    {
                        // Recycler needs some extra params for entry
                        // Add Recycler-specific Parameters to SPROC
                        var paramDeletedBy = new SqlParameter("@DeletedBy", SqlDbType.NVarChar, 250);
                        paramDeletedBy.Value = MailHelper.GetCurrentUserEmailAddress();
                        myCommand.Parameters.Add(paramDeletedBy);

                        var paramDeletedDate = new SqlParameter("@DateDeleted", SqlDbType.DateTime, 8);
                        paramDeletedDate.Value = DateTime.Now;
                        myCommand.Parameters.Add(paramDeletedDate);
                    }

                    // BOWEN 11 June 2005 - END
                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Publish(
                            LogLevel.Warn, "An Error Occurred in DeleteModule. Parameter : " + moduleID, ex);
                    }
                }
            }
        }

        /// <summary>
        /// The DeleteModuleDefinition method deletes the specified
        ///   module type definition from the portal.
        /// </summary>
        /// <param name="defID">
        /// The def ID.
        /// </param>
        /// <remarks>
        /// Other relevant sources: DeleteModuleDefinition Stored Procedure
        /// </remarks>
        public void DeleteModuleDefinition(Guid defID)
        {
            // Create Instance of Connection and Command Object
            using (var myConnection = Config.SqlConnectionString)
            {
                using (var myCommand = new SqlCommand("rb_DeleteModuleDefinition", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    var parameterModuleDefID = new SqlParameter(strATModuleDefID, SqlDbType.UniqueIdentifier);
                    parameterModuleDefID.Value = defID;
                    myCommand.Parameters.Add(parameterModuleDefID);

                    // Open the database connection and execute the command
                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // ErrorHandler.Publish(Appleseed.Framework.LogLevel.Warn, "An Error Occurred in DeleteModuleDefinition. Parameter : " + defID.ToString(), ex);
                        ErrorHandler.Publish(
                            LogLevel.Warn, "An Error Occurred in DeleteModuleDefinition. Parameter : " + defID, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Exists the module products in page.
        /// </summary>
        /// <param name="tabID">
        /// The tab ID.
        /// </param>
        /// <param name="portalID">
        /// The portal ID.
        /// </param>
        /// <returns>
        /// A bool value...
        /// </returns>
        public bool ExistModuleProductsInPage(int tabID, int portalID)
        {
            // Create Instance of Connection and Command Object
            using (var myConnection = Config.SqlConnectionString)
            {
                using (var myCommand = new SqlCommand(strrb_GetModulesInPage, myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    var parameterPageID = new SqlParameter(strATPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = tabID;
                    myCommand.Parameters.Add(parameterPageID);

                    // Add Parameters to SPROC
                    var parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);

                    myConnection.Open();
                    var moduleGuid = new Guid("{EC24FABD-FB16-4978-8C81-1ADD39792377}");
                    var retorno = false;

                    using (var result = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (result.Read())
                        {
                            if (moduleGuid.Equals(result.GetGuid(1)))
                            {
                                retorno = true;
                            }
                        }
                    }

                    return retorno;
                }
            }
        }

        /// <summary>
        /// Find module id defined by the guid in a tab in the portal
        /// </summary>
        /// <param name="portalID">
        /// The portal ID.
        /// </param>
        /// <param name="guid">
        /// The GUID.
        /// </param>
        /// <returns>
        /// </returns>
        public ArrayList FindModuleItemsByGuid(int portalID, Guid guid)
        {
            // Create Instance of Connection and Command Object
            using (var myConnection = Config.SqlConnectionString)
            {
                using (var myCommand = new SqlCommand("rb_FindModulesByGuid", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    var parameterFriendlyName = new SqlParameter(strATGuid, SqlDbType.UniqueIdentifier);
                    parameterFriendlyName.Value = guid;
                    myCommand.Parameters.Add(parameterFriendlyName);
                    var parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);

                    // Open the database connection and execute the command
                    myConnection.Open();
                    var modList = new ArrayList();

                    using (var result = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        ModuleItem m = null;
                        while (result.Read())
                        {
                            m = new ModuleItem();
                            m.ID = (int)result["ModuleId"];
                            modList.Add(m);
                        }
                    }

                    return modList;
                }
            }
        }

        /// <summary>
        /// Find modules defined by the guid in a tab in the portal
        /// </summary>
        /// <param name="portalID">
        /// The portal ID.
        /// </param>
        /// <param name="guid">
        /// The GUID.
        /// </param>
        /// <returns>
        /// </returns>
        [Obsolete("Use FindModuleItemsByGuid instead.")]
        public SqlDataReader FindModulesByGuid(int portalID, Guid guid)
        {
            // Create Instance of Connection and Command Object
            var myConnection = Config.SqlConnectionString;
            var myCommand = new SqlCommand("rb_FindModulesByGuid", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            var parameterFriendlyName = new SqlParameter(strATGuid, SqlDbType.UniqueIdentifier);
            parameterFriendlyName.Value = guid;
            myCommand.Parameters.Add(parameterFriendlyName);
            var parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
            parameterPortalID.Value = portalID;
            myCommand.Parameters.Add(parameterPortalID);

            // Open the database connection and execute the command
            myConnection.Open();
            var dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the datareader
            return dr;
        }

        /// <summary>
        /// The GetModuleDefinitions method returns a list of all module type definitions.
        /// </summary>
        /// <param name="portalID">
        /// The portal ID.
        /// </param>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// Other relevant sources: GetModuleDefinitions Stored Procedure
        /// </remarks>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public SqlDataReader GetCurrentModuleDefinitions(int portalID)
        {
            // Create Instance of Connection and Command Object
            var myConnection = Config.SqlConnectionString;
            var myCommand = new SqlCommand("rb_GetCurrentModuleDefinitions", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            var parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
            parameterPortalID.Value = portalID;
            myCommand.Parameters.Add(parameterPortalID);

            // Open the database connection and execute the command
            myConnection.Open();
            var dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the datareader
            return dr;
        }

        /// <summary>
        /// The GetModuleDefinitions method returns a list of all module type definitions.
        /// </summary>
        /// <param name="portalID">
        /// The portal ID.
        /// </param>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// Other relevant sources: GetModuleDefinitions Stored Procedure
        /// </remarks>
        public IList<GeneralModuleDefinition> GetCurrentModuleDefinitionsList(int portalID)
        {
            // Create Instance of Connection and Command Object
            var myConnection = Config.SqlConnectionString;
            var myCommand = new SqlCommand("rb_GetCurrentModuleDefinitions", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            var parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
            parameterPortalID.Value = portalID;
            myCommand.Parameters.Add(parameterPortalID);

            // Open the database connection and execute the command
            myConnection.Open();

            IList<GeneralModuleDefinition> result = new List<GeneralModuleDefinition>();
            GeneralModuleDefinition genModDef = null;
            using (var dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (dr.Read())
                {
                    genModDef = new GeneralModuleDefinition();

                    genModDef.FriendlyName = dr.GetString(0);
                    genModDef.DesktopSource = dr.GetString(1);
                    genModDef.MobileSource = dr.GetString(2);
                    genModDef.Admin = dr.GetBoolean(3);
                    genModDef.GeneralModDefID = dr.GetInt32(4);

                    result.Add(genModDef);
                }
            }

            return result;
        }

        /// <summary>
        /// The GetGeneralModuleDefinitionByName method returns the id of the Module
        ///   that matches the named Module in general list.
        /// </summary>
        /// <param name="moduleName">
        /// Name of the module.
        /// </param>
        /// <returns>
        /// </returns>
        public Guid GetGeneralModuleDefinitionByName(string moduleName)
        {
            // Create Instance of Connection and Command Object
            using (var myConnection = Config.SqlConnectionString)
            {
                using (var myCommand = new SqlCommand("rb_GetGeneralModuleDefinitionByName", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    var parameterFriendlyName = new SqlParameter("@FriendlyName", SqlDbType.NVarChar, 128);
                    parameterFriendlyName.Value = moduleName;
                    myCommand.Parameters.Add(parameterFriendlyName);
                    var parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.UniqueIdentifier);
                    parameterModuleID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterModuleID);

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

                    if (parameterModuleID.Value != null && parameterModuleID.Value.ToString().Length != 0)
                    {
                        try
                        {
                            return new Guid(parameterModuleID.Value.ToString());
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("'" + parameterModuleID.Value + "' seems not a valid Module GUID.", ex);

                            // Jes1111
                            // Appleseed.Framework.Configuration.ErrorHandler.HandleException("'" + parameterModuleID.Value.ToString() + "' seems not a valid GUID.", ex);
                            // throw;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Null Module GUID!"); // Jes1111

                        // Appleseed.Framework.Configuration.ErrorHandler.HandleException("Null GUID!.", new ArgumentException("Null GUID!", strGUID));
                    }

                    // throw new ArgumentException("Invalid GUID", strGUID);
                }
            }
        }

        /// <summary>
        /// The GetModuleDefinitionByGUID method returns the id of the Module
        ///   that matches the named Module for the specified Portal.
        /// </summary>
        /// <param name="portalID">
        /// The portal ID.
        /// </param>
        /// <param name="guid">
        /// The GUID.
        /// </param>
        /// <returns>
        /// The get module definition by guid.
        /// </returns>
        public int GetModuleDefinitionByGuid(int portalID, Guid guid)
        {
            // Create Instance of Connection and Command Object
            using (var myConnection = Config.SqlConnectionString)
            {
                using (var myCommand = new SqlCommand("rb_GetModuleDefinitionByGuid", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    var parameterFriendlyName = new SqlParameter(strATGuid, SqlDbType.UniqueIdentifier);
                    parameterFriendlyName.Value = guid;
                    myCommand.Parameters.Add(parameterFriendlyName);
                    var parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);
                    var parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
                    parameterModuleID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterModuleID);

                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Publish(
                            LogLevel.Warn, "An Error Occurred in GetModuleDefinitionByGuid. Parameter : " + guid, ex);
                    }

                    return (int)parameterModuleID.Value;
                }
            }
        }

        /// <summary>
        /// The GetModuleDefinitionByName method returns the id of the Module
        ///   that matches the named Module for the specified Portal.
        /// </summary>
        /// <param name="portalID">
        /// The portal ID.
        /// </param>
        /// <param name="moduleName">
        /// Name of the module.
        /// </param>
        /// <returns>
        /// The get module definition by name.
        /// </returns>
        public int GetModuleDefinitionByName(int portalID, string moduleName)
        {
            // Create Instance of Connection and Command Object
            using (var myConnection = Config.SqlConnectionString)
            {
                using (var myCommand = new SqlCommand("rb_GetModuleDefinitionByName", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    var parameterFriendlyName = new SqlParameter(strATFriendlyName, SqlDbType.NVarChar, 128);
                    parameterFriendlyName.Value = moduleName;
                    myCommand.Parameters.Add(parameterFriendlyName);
                    var parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);
                    var parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
                    parameterModuleID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterModuleID);

                    myConnection.Open();
                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Publish(
                            LogLevel.Warn,
                            "An Error Occurred in GetModuleDefinitionByName. Parameter : " + moduleName,
                            ex);
                    }

                    return (int)parameterModuleID.Value;
                }
            }
        }

        /// <summary>
        /// The GetModuleDefinitions method returns a list of all module type
        ///   definitions for the portal.<br/>
        ///   GetModuleDefinitions Stored Procedure
        /// </summary>
        /// <returns>
        /// </returns>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public SqlDataReader GetModuleDefinitions()
        {
            // Create Instance of Connection and Command Object
            var myConnection = Config.SqlConnectionString;
            var myCommand = new SqlCommand("rb_GetModuleDefinitions", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Open the database connection and execute the command
            myConnection.Open();
            var dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the datareader
            return dr;
        }

        /// <summary>
        /// Gets the module GUID.
        /// </summary>
        /// <param name="moduleID">
        /// The module ID.
        /// </param>
        /// <returns>
        /// A System.Guid value...
        /// </returns>
        public Guid GetModuleGuid(int moduleID)
        {
            var moduleGuid = Guid.Empty;
            var cacheGuid = Key.ModuleSettings(moduleID) + "GUID";
            if (CurrentCache.Get(cacheGuid) == null)
            {
                using (var myConnection = Config.SqlConnectionString)
                {
                    using (var myCommand = new SqlCommand("rb_GetGuid", myConnection))
                    {
                        myCommand.CommandType = CommandType.StoredProcedure;
                        var parameterModuleID = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
                        parameterModuleID.Value = moduleID;
                        myCommand.Parameters.Add(parameterModuleID);
                        myConnection.Open();
                        using (var dr = myCommand.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                moduleGuid = dr.GetGuid(0);
                            }
                        }
                    }
                }

                CurrentCache.Insert(cacheGuid, moduleGuid);
            }
            else
            {
                moduleGuid = (Guid)CurrentCache.Get(cacheGuid);
            }

            return moduleGuid;
        }

        /// <summary>
        /// The GetModuleInUse method returns a list of modules in use with this portal<br/>
        ///   GetModuleInUse Stored Procedure
        /// </summary>
        /// <param name="defId">
        /// The def ID.
        /// </param>
        /// <returns>
        /// A list of ListItem controls.
        /// </returns>
        public List<ListItem> GetModuleInUse(Guid defId)
        {
            var portalList = new List<ListItem>();

            using (var sqlCommand = new SqlCommand("rb_GetModuleInUse")
            { 
                CommandType = CommandType.StoredProcedure
            })
            {
                var parameterdefId = new SqlParameter(strATModuleID, SqlDbType.UniqueIdentifier) { Value = defId };
                sqlCommand.Parameters.Add(parameterdefId);

                using (var portals = DBHelper.GetDataReader(sqlCommand))
                {
                    while (portals.Read())
                    {
                        if (Convert.ToInt32(portals["PortalID"]) < 0)
                        {
                            continue;
                        }

                        var item = new ListItem
                            {
                                Text = portals["PortalName"].ToString(),
                                Value = portals["PortalID"].ToString(),
                                Selected = portals["checked"].ToString() == "1"
                            };

                        portalList.Add(item);
                    }

                    portals.Close();
                }
            }

            return portalList;
        }

        /// <summary>
        /// GetModulesAllPortals
        /// </summary>
        /// <returns>
        /// </returns>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public DataTable GetModulesAllPortals()
        {
            // Create Instance of Connection and Command Object
            using (var myConnection = Config.SqlConnectionString)
            {
                using (var myCommand = new SqlDataAdapter("rb_GetModulesAllPortals", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

                    // Create and Fill the DataSet
                    using (var myDataTable = new DataTable())
                    {
                        try
                        {
                            myCommand.Fill(myDataTable);
                        }
                        finally
                        {
                            myConnection.Close(); // by Manu fix close bug #2
                        }

                        // Translate
                        foreach (DataRow dr in myDataTable.Rows)
                        {
                            if (dr[1].ToString() == strNoModule)
                            {
                                dr[1] = General.GetString(strNoModule);
                                break;
                            }
                        }

                        // Return the datareader
                        return myDataTable;
                    }
                }
            }
        }

        /// <summary>
        /// The GetModuleByName method returns a list of all module with
        ///   the specified Name (Type) within the Portal.
        ///   It is used to get all instances of a specified module used in a Portal.
        ///   e.g. All Image Gallery
        /// </summary>
        /// <param name="moduleName">
        /// Name of the module.
        /// </param>
        /// <param name="portalID">
        /// The portal ID.
        /// </param>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// Other relevant sources: GetModuleByName Stored Procedure
        /// </remarks>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public SqlDataReader GetModulesByName(string moduleName, int portalID)
        {
            // Create Instance of Connection and Command Object
            var myConnection = Config.SqlConnectionString;
            var myCommand = new SqlCommand("rb_GetModulesByName", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            var parameterFriendlyName = new SqlParameter("@moduleName", SqlDbType.NVarChar, 128);
            parameterFriendlyName.Value = moduleName;
            myCommand.Parameters.Add(parameterFriendlyName);
            var parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
            parameterPortalID.Value = portalID;
            myCommand.Parameters.Add(parameterPortalID);

            // Open the database connection and execute the command
            myConnection.Open();
            var dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the datareader
            return dr;
        }

        /// <summary>
        /// Gets the modules in page.
        /// </summary>
        /// <param name="portalID">
        /// The portal ID.
        /// </param>
        /// <param name="pageID">
        /// The page ID.
        /// </param>
        /// <returns>
        /// A System.Data.SqlClient.SqlDataReader value...
        /// </returns>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public SqlDataReader GetModulesInPage(int portalID, int pageID)
        {
            // Create Instance of Connection and Command Object
            var myConnection = Config.SqlConnectionString;
            var myCommand = new SqlCommand(strrb_GetModulesInPage, myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;
            var parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
            parameterPortalID.Value = portalID;
            myCommand.Parameters.Add(parameterPortalID);

            // Add Parameters to SPROC
            var parameterPageID = new SqlParameter(strATPageID, SqlDbType.Int, 4);
            parameterPageID.Value = pageID;
            myCommand.Parameters.Add(parameterPageID);

            // Open the database connection and execute the command
            myConnection.Open();
            var dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the datareader
            return dr;
        }

        /// <summary>
        /// GetModulesSinglePortal
        /// </summary>
        /// <param name="PortalID">
        /// The portal ID.
        /// </param>
        /// <returns>
        /// </returns>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public DataTable GetModulesSinglePortal(int PortalID)
        {
            // Create Instance of Connection and Command Object
            using (var myConnection = Config.SqlConnectionString)
            {
                using (var myCommand = new SqlDataAdapter("rb_GetModulesSinglePortal", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    var parameterPortalID = new SqlParameter(strATPortalID, SqlDbType.Int);
                    parameterPortalID.Value = PortalID;
                    myCommand.SelectCommand.Parameters.Add(parameterPortalID);

                    // Create and Fill the DataSet
                    using (var myDataTable = new DataTable())
                    {
                        try
                        {
                            myCommand.Fill(myDataTable);
                        }
                        finally
                        {
                            myConnection.Close(); // by Manu fix close bug #2
                        }

                        // Translate
                        foreach (DataRow dr in myDataTable.Rows)
                        {
                            if (dr[1].ToString() == strNoModule)
                            {
                                dr[1] = General.GetString(strNoModule);
                                break;
                            }
                        }

                        // Return the datareader
                        return myDataTable;
                    }
                }
            }
        }

        /// <summary>
        /// The GetSingleModuleDefinition method returns a SqlDataReader
        ///   containing details about a specific module definition
        ///   from the ModuleDefinitions table.
        /// </summary>
        /// <param name="generalModDefId">
        /// The general mod def ID.
        /// </param>
        /// <returns>
        /// A single module definition.
        /// </returns>
        /// <remarks>
        /// Other relevant sources: GetSingleModuleDefinition Stored Procedure
        /// </remarks>
        public GeneralModuleDefinition GetSingleModuleDefinition(Guid generalModDefId)
        {
            // Create Instance of Connection and Command Object
            using (var sqlCommand = new SqlCommand("rb_GetSingleModuleDefinition")
            {
                // Mark the Command as a SPROC
                CommandType = CommandType.StoredProcedure
            })
            {
                var parameterGeneralModDefId = new SqlParameter(strATGeneralModDefID, SqlDbType.UniqueIdentifier)
                    {
                        Value = generalModDefId
                    };
                sqlCommand.Parameters.Add(parameterGeneralModDefId);

                // Execute the command and get the data reader 
                var dr = DBHelper.GetDataReader(sqlCommand);
                var moduleDefinition = new GeneralModuleDefinition();
                while (dr.Read())
                {
                    moduleDefinition.FriendlyName = dr["FriendlyName"].ToString();
                    moduleDefinition.DesktopSource = dr["DesktopSrc"].ToString();
                    moduleDefinition.MobileSource = dr["MobileSrc"].ToString();
                    moduleDefinition.GeneralModDefID = int.Parse(dr["GeneralModDefID"].ToString());
                }

                dr.Close();

                return moduleDefinition;
            }
        }

        /// <summary>
        /// The GetSolutionModuleDefinitions method returns a list of all module type definitions.<br></br>
        ///   GetSolutionModuleDefinitions Stored Procedure
        /// </summary>
        /// <param name="solutionID">
        /// The solution ID.
        /// </param>
        /// <returns>
        /// </returns>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public SqlDataReader GetSolutionModuleDefinitions(int solutionID)
        {
            // Create Instance of Connection and Command Object
            var myConnection = Config.SqlConnectionString;
            var myCommand = new SqlCommand("rb_GetSolutionModuleDefinitions", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            var parameterSolutionID = new SqlParameter("@SolutionID", SqlDbType.Int, 4);
            parameterSolutionID.Value = solutionID;
            myCommand.Parameters.Add(parameterSolutionID);

            // Open the database connection and execute the command
            myConnection.Open();
            var dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the datareader
            return dr;
        }

        /// <summary>
        /// The GetSolutions method returns the Solution list.
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// Other relevant sources: GetUsers Stored Procedure
        /// </remarks>
        [Obsolete("Replace me, bad design practive to pass SqlDataReaders to the UI")]
        public SqlDataReader GetSolutions()
        {
            // Create Instance of Connection and Command Object
            var myConnection = Config.SqlConnectionString;
            var myCommand = new SqlCommand("rb_GetSolutions", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Open the database connection and execute the command
            myConnection.Open();
            var dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the datareader
            return dr;
        }

        /// <summary>
        /// Update General Module Definitions
        /// </summary>
        /// <param name="generalModDefId">
        /// General Mod Def ID
        /// </param>
        /// <param name="friendlyName">
        /// Name of the friendly.
        /// </param>
        /// <param name="desktopSrc">
        /// The desktop SRC.
        /// </param>
        /// <param name="mobileSrc">
        /// The mobile SRC.
        /// </param>
        /// <param name="assemblyName">
        /// Name of the assembly.
        /// </param>
        /// <param name="className">
        /// Name of the class.
        /// </param>
        /// <param name="admin">
        /// if set to <c>true</c> [admin].
        /// </param>
        /// <param name="searchable">
        /// if set to <c>true</c> [searchable].
        /// </param>
        public void UpdateGeneralModuleDefinitions(
            Guid generalModDefId,
            string friendlyName,
            string desktopSrc,
            string mobileSrc,
            string assemblyName,
            string className,
            bool admin,
            bool searchable)
        {
            // Create Instance of Connection and Command Object
            using (var myConnection = Config.SqlConnectionString)
            using (var myCommand = new SqlCommand("rb_UpdateGeneralModuleDefinitions", myConnection))
            {
                // Mark the Command as a SPROC
                myCommand.CommandType = CommandType.StoredProcedure;

                // Update Parameters to SPROC
                var parameterGeneralModDefId = new SqlParameter(strATGeneralModDefID, SqlDbType.UniqueIdentifier);
                parameterGeneralModDefId.Value = generalModDefId;
                myCommand.Parameters.Add(parameterGeneralModDefId);
                var parameterFriendlyName = new SqlParameter(strATFriendlyName, SqlDbType.NVarChar, 128);
                parameterFriendlyName.Value = friendlyName;
                myCommand.Parameters.Add(parameterFriendlyName);
                var parameterDesktopSrc = new SqlParameter(strATDesktopSrc, SqlDbType.NVarChar, 256);
                parameterDesktopSrc.Value = desktopSrc;
                myCommand.Parameters.Add(parameterDesktopSrc);
                var parameterMobileSrc = new SqlParameter(strATMobileSrc, SqlDbType.NVarChar, 256);
                parameterMobileSrc.Value = mobileSrc;
                myCommand.Parameters.Add(parameterMobileSrc);
                var parameterAssemblyName = new SqlParameter(strATAssemblyName, SqlDbType.VarChar, 50);
                parameterAssemblyName.Value = assemblyName;
                myCommand.Parameters.Add(parameterAssemblyName);
                var parameterClassName = new SqlParameter(strATClassName, SqlDbType.NVarChar, 128);
                parameterClassName.Value = className;
                myCommand.Parameters.Add(parameterClassName);
                var parameterAdmin = new SqlParameter(strATAdmin, SqlDbType.Bit);
                parameterAdmin.Value = admin;
                myCommand.Parameters.Add(parameterAdmin);
                var parameterSearchable = new SqlParameter(strATSearchable, SqlDbType.Bit);
                parameterSearchable.Value = searchable;
                myCommand.Parameters.Add(parameterSearchable);

                // Execute the command
                myConnection.Open();

                try
                {
                    myCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // ErrorHandler.Publish(Appleseed.Framework.LogLevel.Warn, "An Error Occurred in UpdateGeneralModuleDefinitions", ex));
                    ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateGeneralModuleDefinitions", ex);
                }
            }
        }

        /// <summary>
        /// The UpdateModule method updates a specified Module within the Modules database table.
        ///   If the module does not yet exist, the stored procedure adds it.<br/>
        ///   UpdateModule Stored Procedure
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <param name="moduleId">
        /// The module ID.
        /// </param>
        /// <param name="moduleOrder">
        /// The module order.
        /// </param>
        /// <param name="paneName">
        /// Name of the pane.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="cacheTime">
        /// The cache time.
        /// </param>
        /// <param name="editRoles">
        /// The edit roles.
        /// </param>
        /// <param name="viewRoles">
        /// The view roles.
        /// </param>
        /// <param name="addRoles">
        /// The add roles.
        /// </param>
        /// <param name="deleteRoles">
        /// The delete roles.
        /// </param>
        /// <param name="propertiesRoles">
        /// The properties roles.
        /// </param>
        /// <param name="moveModuleRoles">
        /// The move module roles.
        /// </param>
        /// <param name="deleteModuleRoles">
        /// The delete module roles.
        /// </param>
        /// <param name="showMobile">
        /// if set to <c>true</c> [show mobile].
        /// </param>
        /// <param name="publishingRoles">
        /// The publishing roles.
        /// </param>
        /// <param name="supportWorkflow">
        /// if set to <c>true</c> [support workflow].
        /// </param>
        /// <param name="approvalRoles">
        /// The approval roles.
        /// </param>
        /// <param name="showEveryWhere">
        /// if set to <c>true</c> [show every where].
        /// </param>
        /// <param name="supportCollapsable">
        /// if set to <c>true</c> [support collapsible].
        /// </param>
        /// <returns>
        /// The update module.
        /// </returns>
        public int UpdateModule(
            int pageId,
            int moduleId,
            int moduleOrder,
            string paneName,
            string title,
            int cacheTime,
            string editRoles,
            string viewRoles,
            string addRoles,
            string deleteRoles,
            string propertiesRoles,
            string moveModuleRoles,
            string deleteModuleRoles,
            bool showMobile,
            string publishingRoles,
            bool supportWorkflow,
            string approvalRoles,
            bool showEveryWhere,
            bool supportCollapsable)
        {
            // Changes by Geert.Audenaert@Syntegra.Com Date: 6/2/2003
            // Create Instance of Connection and Command Object
            using (var myConnection = Config.SqlConnectionString)
            {
                using (var myCommand = new SqlCommand("rb_UpdateModule", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Add Parameters to SPROC
                    var parameterModuleId = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
                    parameterModuleId.Value = moduleId;
                    myCommand.Parameters.Add(parameterModuleId);
                    var parameterPageId = new SqlParameter(strATPageID, SqlDbType.Int, 4);
                    parameterPageId.Value = pageId;
                    myCommand.Parameters.Add(parameterPageId);
                    var parameterModuleOrder = new SqlParameter(strATModuleOrder, SqlDbType.Int, 4);
                    parameterModuleOrder.Value = moduleOrder;
                    myCommand.Parameters.Add(parameterModuleOrder);
                    var parameterTitle = new SqlParameter(strATModuleTitle, SqlDbType.NVarChar, 256);
                    parameterTitle.Value = title;
                    myCommand.Parameters.Add(parameterTitle);
                    var parameterPaneName = new SqlParameter(strATPaneName, SqlDbType.NVarChar, 256);
                    parameterPaneName.Value = paneName;
                    myCommand.Parameters.Add(parameterPaneName);
                    var parameterCacheTime = new SqlParameter(strATCacheTime, SqlDbType.Int, 4);
                    parameterCacheTime.Value = cacheTime;
                    myCommand.Parameters.Add(parameterCacheTime);
                    var parameterEditRoles = new SqlParameter(strATEditRoles, SqlDbType.NVarChar, 256);
                    parameterEditRoles.Value = editRoles;
                    myCommand.Parameters.Add(parameterEditRoles);
                    var parameterViewRoles = new SqlParameter(strATViewRoles, SqlDbType.NVarChar, 256);
                    parameterViewRoles.Value = viewRoles;
                    myCommand.Parameters.Add(parameterViewRoles);
                    var parameterAddRoles = new SqlParameter(strATAddRoles, SqlDbType.NVarChar, 256);
                    parameterAddRoles.Value = addRoles;
                    myCommand.Parameters.Add(parameterAddRoles);
                    var parameterDeleteRoles = new SqlParameter(strATDeleteRoles, SqlDbType.NVarChar, 256);
                    parameterDeleteRoles.Value = deleteRoles;
                    myCommand.Parameters.Add(parameterDeleteRoles);
                    var parameterPropertiesRoles = new SqlParameter(strATPropertiesRoles, SqlDbType.NVarChar, 256);
                    parameterPropertiesRoles.Value = propertiesRoles;
                    myCommand.Parameters.Add(parameterPropertiesRoles);

                    // Added by jviladiu@portalservices.net (19/08/2004)
                    var parameterMoveModuleRoles = new SqlParameter(strATMoveModuleRoles, SqlDbType.NVarChar, 256);
                    parameterMoveModuleRoles.Value = moveModuleRoles;
                    myCommand.Parameters.Add(parameterMoveModuleRoles);

                    // Added by jviladiu@portalservices.net (19/08/2004)
                    var parameterDeleteModuleRoles = new SqlParameter(strATDeleteModuleRoles, SqlDbType.NVarChar, 256);
                    parameterDeleteModuleRoles.Value = deleteModuleRoles;
                    myCommand.Parameters.Add(parameterDeleteModuleRoles);

                    // Change by Geert.Audenaert@Syntegra.Com
                    // Date: 6/2/2003
                    var parameterPublishingRoles = new SqlParameter(strATPublishingRoles, SqlDbType.NVarChar, 256);
                    parameterPublishingRoles.Value = publishingRoles;
                    myCommand.Parameters.Add(parameterPublishingRoles);
                    var parameterSupportWorkflow = new SqlParameter(strATSupportWorkflow, SqlDbType.Bit, 1);
                    parameterSupportWorkflow.Value = supportWorkflow;
                    myCommand.Parameters.Add(parameterSupportWorkflow);

                    // End Change Geert.Audenaert@Syntegra.Com
                    // Change by Geert.Audenaert@Syntegra.Com
                    // Date: 27/2/2003
                    var parameterApprovalRoles = new SqlParameter(strATApprovalRoles, SqlDbType.NVarChar, 256);
                    parameterApprovalRoles.Value = approvalRoles;
                    myCommand.Parameters.Add(parameterApprovalRoles);

                    // End Change Geert.Audenaert@Syntegra.Com
                    var parameterShowMobile = new SqlParameter(strATShowMobile, SqlDbType.Bit, 1);
                    parameterShowMobile.Value = showMobile;
                    myCommand.Parameters.Add(parameterShowMobile);

                    // Addition by john.mandia@whitelightsolutions.com to add show on every page functionality
                    var parameterShowEveryWhere = new SqlParameter(strATShowEveryWhere, SqlDbType.Bit, 1);
                    parameterShowEveryWhere.Value = showEveryWhere;
                    myCommand.Parameters.Add(parameterShowEveryWhere);

                    // Change by baj@reedtek.com
                    // Date: 16/5/2003
                    var parameterSupportCollapsable = new SqlParameter(strATSupportCollapsable, SqlDbType.Bit, 1);
                    parameterSupportCollapsable.Value = supportCollapsable;
                    myCommand.Parameters.Add(parameterSupportCollapsable);

                    // End Change baj@reedtek.com
                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // ErrorHandler.Publish(Appleseed.Framework.LogLevel.Warn, "An Error Occurred in UpdateModule", ex);
                        ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateModule", ex);
                    }

                    return (int)parameterModuleId.Value;
                }
            }
        }

        /// <summary>
        /// The UpdateModuleDefinitions method updates
        ///   all module definitions in every portal
        /// </summary>
        /// <param name="generalModDefId">
        /// The general mod def ID.
        /// </param>
        /// <param name="portalId">
        /// The portal ID.
        /// </param>
        /// <param name="ischecked">
        /// if set to <c>true</c> [ischecked].
        /// </param>
        public void UpdateModuleDefinitions(Guid generalModDefId, int portalId, bool ischecked)
        {
            // Create Instance of Connection and Command Object
            using (var myConnection = Config.SqlConnectionString)
            using (var myCommand = new SqlCommand("rb_UpdateModuleDefinitions", myConnection))
            {
                // Mark the Command as a SPROC
                myCommand.CommandType = CommandType.StoredProcedure;

                // Add Parameters to SPROC
                var parameterGeneralModDefId = new SqlParameter(strATGeneralModDefID, SqlDbType.UniqueIdentifier);
                parameterGeneralModDefId.Value = generalModDefId;
                myCommand.Parameters.Add(parameterGeneralModDefId);

                // Add Parameters to SPROC
                var parameterPortalId = new SqlParameter(strATPortalID, SqlDbType.Int, 4);
                parameterPortalId.Value = portalId;
                myCommand.Parameters.Add(parameterPortalId);
                var parameterischecked = new SqlParameter("@ischecked", SqlDbType.Bit);
                parameterischecked.Value = ischecked;
                myCommand.Parameters.Add(parameterischecked);

                myConnection.Open();

                try
                {
                    myCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // ErrorHandler.Publish(Appleseed.Framework.LogLevel.Warn, "An Error Occurred in UpdateModuleDefinitions", ex);
                    ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateModuleDefinitions", ex);
                }
            }
        }

        /// <summary>
        /// The UpdateModuleOrder method update Modules Order.<br/>
        ///   UpdateModuleOrder Stored Procedure
        /// </summary>
        /// <param name="moduleId">
        /// The module ID.
        /// </param>
        /// <param name="moduleOrder">
        /// The module order.
        /// </param>
        /// <param name="pane">
        /// The pane.
        /// </param>
        public void UpdateModuleOrder(int moduleId, int moduleOrder, string pane)
        {
            // Create Instance of Connection and Command Object
            using (var myConnection = Config.SqlConnectionString)
            using (var myCommand = new SqlCommand("rb_UpdateModuleOrder", myConnection))
            {
                // Mark the Command as a SPROC
                myCommand.CommandType = CommandType.StoredProcedure;

                // Add Parameters to SPROC
                var parameterModuleId = new SqlParameter(strATModuleID, SqlDbType.Int, 4);
                parameterModuleId.Value = moduleId;
                myCommand.Parameters.Add(parameterModuleId);
                var parameterModuleOrder = new SqlParameter(strATModuleOrder, SqlDbType.Int, 4);
                parameterModuleOrder.Value = moduleOrder;
                myCommand.Parameters.Add(parameterModuleOrder);
                var parameterPaneName = new SqlParameter(strATPaneName, SqlDbType.NVarChar, 256);
                parameterPaneName.Value = pane;
                myCommand.Parameters.Add(parameterPaneName);
                myConnection.Open();

                try
                {
                    myCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // ErrorHandler.Publish(Appleseed.Framework.LogLevel.Warn, "An Error Occurred in UpdateModuleOrder", ex);
                    ErrorHandler.Publish(LogLevel.Warn, "An Error Occurred in UpdateModuleOrder", ex);
                }
            }
        }

        /// <summary>
        /// The UpdateModuleSetting Method updates a single module setting
        ///   in the ModuleSettings database table.
        /// </summary>
        /// <param name="moduleId">
        /// The module ID.
        /// </param>
        /// <param name="key">
        /// The setting key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        [Obsolete("UpdateModuleSetting was moved to ModuleSettings.UpdateModuleSetting", false)]
        public void UpdateModuleSetting(int moduleId, string key, string value)
        {
            ModuleSettings.UpdateModuleSetting(moduleId, key, value);
        }

        #endregion
    }
}