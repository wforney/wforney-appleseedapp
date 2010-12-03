using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;

using Appleseed.Framework.Providers.AppleseedRoleProvider;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using Appleseed.Framework.Providers.AppleseedMembershipProvider;
using System.Collections;
using System.Configuration.Provider;
using System.Web;

namespace Appleseed.Framework.Providers.AppleseedRoleProvider {

    public class AppleseedSqlRoleProvider : AppleseedRoleProvider {

        protected string pApplicationName;
        protected string connectionString;

        private string eventSource = "AppleseedSqlRoleProvider";
        private string eventLog = "Application";

        public override void Initialize( string name, System.Collections.Specialized.NameValueCollection config ) {

            //
            // Initialize values from web.config.
            //

            if ( config == null )
                throw new ArgumentNullException( "config" );

            if ( name == null || name.Length == 0 )
                name = "AppleseedSqlRoleProvider";

            if ( String.IsNullOrEmpty( config["description"] ) ) {
                config.Remove( "description" );
                config.Add( "description", "Appleseed Sql Role provider" );
            }

            // Initialize the abstract base class.
            base.Initialize( name, config );

            pApplicationName = GetConfigValue( config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath );
            pWriteExceptionsToEventLog = Convert.ToBoolean( GetConfigValue( config["writeExceptionsToEventLog"], "true" ) );

            // Initialize SqlConnection.
            ConnectionStringSettings ConnectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            if ( ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim().Equals( string.Empty ) ) {
                throw new AppleseedRoleProviderException( "Connection string cannot be blank." );
            }

            connectionString = ConnectionStringSettings.ConnectionString;
        }

        #region Overriden properties

        public override string ApplicationName
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return (string)HttpContext.Current.Items["Role.ApplicationName"];
                }
                else
                {
                    return pApplicationName;
                }
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Items["Role.ApplicationName"] = value;

                pApplicationName = value;
            }
        }

        #endregion

        #region Properties

        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //

        private bool pWriteExceptionsToEventLog;

        public bool WriteExceptionsToEventLog {
            get {
                return pWriteExceptionsToEventLog;
            }
            set {
                pWriteExceptionsToEventLog = value;
            }
        }

        #endregion

        #region Overriden methods

        public override void AddUsersToRoles( string[] usernames, string[] roleNames ) {
            Guid[] userIds = new Guid[usernames.Length];
            Guid[] roleIds = new Guid[roleNames.Length];

            AppleseedUser user = null;
            for ( int i = 0; i < usernames.Length; i++ ) {
                user = ( AppleseedUser )Membership.GetUser( usernames[i] );

                if ( user == null ) {
                    throw new AppleseedMembershipProviderException( "User " + usernames[i] + " doesn't exist" );
                }

                userIds[i] = user.ProviderUserKey;
            }

            AppleseedRole role = null;
            for ( int i = 0; i < roleNames.Length; i++ ) {
                role = GetRoleByName( ApplicationName, roleNames[i] );
                roleIds[i] = role.Id;
            }

            AddUsersToRoles( ApplicationName, userIds, roleIds );
        }

        public override void CreateRole( string roleName ) {
            CreateRole( ApplicationName, roleName );
        }

        public override bool DeleteRole( string roleName, bool throwOnPopulatedRole ) {
            AppleseedRole role = GetRoleByName( ApplicationName, roleName );

            return DeleteRole( ApplicationName, role.Id, throwOnPopulatedRole );
        }

        public override string[] FindUsersInRole( string roleName, string usernameToMatch ) {
            return FindUsersInRole( ApplicationName, roleName, usernameToMatch );
        }

        public override string[] GetAllRoles() {
            IList<AppleseedRole> roles = GetAllRoles( ApplicationName );
                        
            string[] result = new string[roles.Count];

            for ( int i = 0; i < roles.Count; i++ ) {
                result[i] = roles[i].Name;
            }

            return result;
        }

        public override string[] GetRolesForUser( string username ) {

            AppleseedUser user = ( AppleseedUser )Membership.GetUser( username );
            if (user != null) {
                IList<AppleseedRole> roles = GetRolesForUser(ApplicationName, user.ProviderUserKey);

                string[] result = new string[roles.Count];

                for (int i = 0; i < roles.Count; i++) {
                    result[i] = roles[i].Name;
                }

                return result;
            }
            return new string[0];
        }

        public override string[] GetUsersInRole( string roleName ) {
            AppleseedRole role = GetRoleByName( ApplicationName, roleName );
            return GetUsersInRole( ApplicationName, role.Id );
        }

        public override bool IsUserInRole( string username, string roleName ) {

            AppleseedUser user = ( AppleseedUser )Membership.GetUser( username );

            if ( user == null ) {
                throw new AppleseedRoleProviderException( "User doesn't exist" );
            }

            AppleseedRole role = GetRoleByName( ApplicationName, roleName );

            return IsUserInRole( ApplicationName, user.ProviderUserKey, role.Id );
        }

        public override void RemoveUsersFromRoles( string[] usernames, string[] roleNames ) {
            Guid[] userIds = new Guid[usernames.Length];
            Guid[] roleIds = new Guid[roleNames.Length];

            AppleseedUser user = null;
            for ( int i = 0; i < usernames.Length; i++ ) {
                user = ( AppleseedUser )Membership.GetUser( usernames[i] );

                if ( user == null ) {
                    throw new AppleseedMembershipProviderException( "User " + usernames[i] + " doesn't exist" );
                }

                userIds[i] = user.ProviderUserKey;
            }

            AppleseedRole role = null;
            for ( int i = 0; i < roleNames.Length; i++ ) {
                role = GetRoleByName( ApplicationName, roleNames[i] );
                roleIds[i] = role.Id;
            }

            RemoveUsersFromRoles( ApplicationName, userIds, roleIds );
        }

        public override bool RoleExists( string roleName ) {
            try {
                IList<AppleseedRole> allRoles = GetAllRoles( ApplicationName );
                foreach ( AppleseedRole role in allRoles ) {
                    if ( role.Name.Equals( roleName ) ) {
                        return true;
                    }
                }
                return false;
            }
            catch {
                return false;
            }
        }

        #endregion

        #region Appleseed-specific Provider methods

        public override void AddUsersToRoles( string portalAlias, Guid[] userIds, Guid[] roleIds ) {
            string userIdsStr = string.Empty;
            string roleIdsStr = string.Empty;

            foreach ( Guid userId in userIds ) {
                userIdsStr += userId.ToString() + ",";
            }
            userIdsStr = userIdsStr.Substring( 0, userIdsStr.Length - 1 );

            foreach ( Guid roleId in roleIds ) {
                roleIdsStr += roleId.ToString() + ",";
            }
            roleIdsStr = roleIdsStr.Substring( 0, roleIdsStr.Length - 1 );

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "aspnet_UsersInRoles_AddUsersToRoles";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = new SqlConnection( connectionString );

            cmd.Parameters.Add( "@ApplicationName", SqlDbType.NVarChar, 256 ).Value = portalAlias;
            cmd.Parameters.Add( "@UserIds", SqlDbType.VarChar, 4000 ).Value = userIdsStr;
            cmd.Parameters.Add( "@RoleIds", SqlDbType.VarChar, 4000 ).Value = roleIdsStr;

            SqlParameter returnCodeParam = cmd.Parameters.Add( "@ReturnCode", SqlDbType.Int );
            returnCodeParam.Direction = ParameterDirection.ReturnValue;

            try {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();

                int returnCode = ( int )returnCodeParam.Value;

                switch ( returnCode ) {
                    case 0:
                        return;
                    case 2:
                        throw new AppleseedRoleProviderException( "Application " + portalAlias + " doesn't exist" );
                    case 3:
                        throw new AppleseedRoleProviderException( "One of the roles doesn't exist" );
                    case 4:
                        throw new AppleseedMembershipProviderException( "One of the users doesn't exist" );
                    default:
                        throw new AppleseedRoleProviderException( "aspnet_UsersInRoles_AddUsersToRoles returned error code " + returnCode );
                }
            }
            catch ( SqlException e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "CreateRole" );
                }

                throw new AppleseedRoleProviderException( "Error executing aspnet_UsersInRoles_AddUsersToRoles stored proc", e );
            }
            finally {
                cmd.Connection.Close();
            }
        }

        public override Guid CreateRole( string portalAlias, string roleName ) {

            if ( roleName.IndexOf( ',' ) != -1 ) {
                throw new AppleseedRoleProviderException( "Role name can't contain commas" );
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "aspnet_Roles_CreateRole";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = new SqlConnection( connectionString );

            cmd.Parameters.Add( "@ApplicationName", SqlDbType.NVarChar, 256 ).Value = portalAlias;
            cmd.Parameters.Add( "@RoleName", SqlDbType.NVarChar, 256 ).Value = roleName;

            SqlParameter newRoleIdParam = cmd.Parameters.Add( "@NewRoleId", SqlDbType.UniqueIdentifier );
            newRoleIdParam.Direction = ParameterDirection.Output;

            SqlParameter returnCodeParam = cmd.Parameters.Add( "@ReturnCode", SqlDbType.Int );
            returnCodeParam.Direction = ParameterDirection.ReturnValue;

            SqlDataReader reader = null;
            try {
                cmd.Connection.Open();

                cmd.ExecuteNonQuery();

                int returnCode = ( int )returnCodeParam.Value;

                if ( returnCode != 0 ) {
                    throw new AppleseedRoleProviderException( "Error creating role " + roleName );
                }

                return ( Guid )newRoleIdParam.Value;
            }
            catch ( SqlException e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "CreateRole" );
                }

                throw new AppleseedRoleProviderException( "Error executing aspnet_Roles_CreateRole stored proc", e );
            }
            finally {
                if ( reader != null ) {
                    reader.Close();
                }
                cmd.Connection.Close();
            }
        }

        public override bool DeleteRole( string portalAlias, Guid roleId, bool throwOnPopulatedRole ) {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "aspnet_Roles_DeleteRole";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = new SqlConnection( connectionString );

            cmd.Parameters.Add( "@ApplicationName", SqlDbType.NVarChar, 256 ).Value = portalAlias;
            cmd.Parameters.Add( "@RoleId", SqlDbType.UniqueIdentifier ).Value = roleId;
            if ( throwOnPopulatedRole ) {
                cmd.Parameters.Add( "@DeleteOnlyIfRoleIsEmpty", SqlDbType.Bit ).Value = 1;
            }
            else {
                cmd.Parameters.Add( "@DeleteOnlyIfRoleIsEmpty", SqlDbType.Bit ).Value = 0;
            }

            SqlParameter returnCodeParam = cmd.Parameters.Add( "@ReturnCode", SqlDbType.Int );
            returnCodeParam.Direction = ParameterDirection.ReturnValue;

            SqlDataReader reader = null;
            try {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();

                int returnCode = ( int )returnCodeParam.Value;

                switch ( returnCode ) {
                    case 0:
                        return true;
                    case 1:
                        throw new AppleseedRoleProviderException( "Application " + portalAlias + " doesn't exist" );
                    case 2:
                        throw new AppleseedRoleProviderException( "Role has members and throwOnPopulatedRole is true" );
                    default:
                        throw new AppleseedRoleProviderException( "Error deleting role" );
                }
            }
            catch ( SqlException e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "DeleteRole" );
                }

                throw new AppleseedRoleProviderException( "Error executing aspnet_Roles_DeleteRole stored proc", e );
            }
            catch ( Exception e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "DeleteRole" );
                }

                throw new AppleseedRoleProviderException( "Error deleting role " + roleId.ToString(), e );
            }
            finally {
                if ( reader != null ) {
                    reader.Close();
                }
                cmd.Connection.Close();
            }
        }

        public override bool RenameRole( string portalAlias, Guid roleId, string newRoleName ) {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "aspnet_rbRoles_RenameRole";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = new SqlConnection( connectionString );

            cmd.Parameters.Add( "@RoleId", SqlDbType.UniqueIdentifier ).Value = roleId;
            cmd.Parameters.Add( "@NewRoleName", SqlDbType.VarChar, 256 ).Value = newRoleName;

            SqlParameter returnCodeParam = cmd.Parameters.Add( "@ReturnCode", SqlDbType.Int );
            returnCodeParam.Direction = ParameterDirection.ReturnValue;

            SqlDataReader reader = null;
            try {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();

                int returnCode = ( int )returnCodeParam.Value;

                switch ( returnCode ) {
                    case 0:
                        return true;
                    case 1:
                        throw new AppleseedRoleProviderException( "Role doesn't exist" );
                    default:
                        throw new AppleseedRoleProviderException( "Error renaming role" );
                }
            }
            catch ( SqlException e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "DeleteRole" );
                }

                throw new AppleseedRoleProviderException( "Error executing aspnet_rbRoles_RenameRole stored proc", e );
            }
            catch ( Exception e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "RenameRole" );
                }

                throw new AppleseedRoleProviderException( "Error renaming role " + roleId.ToString(), e );
            }
            finally {
                if ( reader != null ) {
                    reader.Close();
                }
                cmd.Connection.Close();
            }
        }

        public override string[] FindUsersInRole( string portalAlias, string roleName, string usernameToMatch ) {
            throw new Exception( "The method or operation is not implemented." );
        }

        public override IList<AppleseedRole> GetAllRoles( string portalAlias ) {

            IList<AppleseedRole> result = new List<AppleseedRole>();
            result.Insert( 0, new AppleseedRole( AllUsersGuid, AllUsersRoleName, AllUsersRoleName ) );
            result.Insert( 1, new AppleseedRole( AuthenticatedUsersGuid, AuthenticatedUsersRoleName, AuthenticatedUsersRoleName ) );
            result.Insert( 2, new AppleseedRole( UnauthenticatedUsersGuid, UnauthenticatedUsersRoleName, UnauthenticatedUsersRoleName ) );

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "aspnet_Roles_GetAllRoles";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = new SqlConnection( connectionString );

            cmd.Parameters.Add( "@ApplicationName", SqlDbType.NVarChar, 256 ).Value = portalAlias;

            SqlDataReader reader = null;
            try {
                cmd.Connection.Open();

                using ( reader = cmd.ExecuteReader() ) {

                    while ( reader.Read() ) {
                        AppleseedRole role = GetRoleFromReader( reader );

                        result.Add( role );
                    }
                    reader.Close();
                }

                return result;
            }
            catch ( SqlException e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "GetAllRoles" );
                }

                throw new AppleseedRoleProviderException( "Error executing aspnet_Roles_GetAllRoles stored proc", e );
            }
            catch ( Exception e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "GetAllRoles" );
                }

                throw new AppleseedRoleProviderException( "Error getting all roles", e );
            }
            finally {
                if ( reader != null ) {
                    reader.Close();
                }
                cmd.Connection.Close();
            }
        }

        public override IList<AppleseedRole> GetRolesForUser( string portalAlias, Guid userId ) {
            IList<AppleseedRole> result = new List<AppleseedRole>();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "aspnet_UsersInRoles_GetRolesForUser";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = new SqlConnection( connectionString );

            cmd.Parameters.Add( "@ApplicationName", SqlDbType.NVarChar, 256 ).Value = portalAlias;
            cmd.Parameters.Add( "@UserId", SqlDbType.UniqueIdentifier ).Value = userId;

            SqlParameter returnCode = cmd.Parameters.Add( "@ReturnCode", SqlDbType.Int );
            returnCode.Direction = ParameterDirection.ReturnValue;

            SqlDataReader reader = null;
            try {
                cmd.Connection.Open();

                using ( reader = cmd.ExecuteReader() ) {

                    while ( reader.Read() ) {

                        AppleseedRole role = GetRoleFromReader( reader );

                        result.Add( role );
                    }
                    reader.Close();
                    if ( ( ( int )returnCode.Value ) == 1 ) {
                        throw new AppleseedRoleProviderException( "User doesn't exist" );
                    }
                }

                return result;
            }
            catch ( SqlException e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "GetAllRoles" );
                }

                throw new AppleseedRoleProviderException( "Error executing aspnet_Roles_GetAllRoles stored proc", e );
            }
            catch ( AppleseedRoleProviderException ) {
                throw;
            }
            catch ( Exception e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "GetAllRoles" );
                }

                throw new AppleseedRoleProviderException( "Error getting roles for user " + userId.ToString(), e );
            }
            finally {
                if ( reader != null ) {
                    reader.Close();
                }
                cmd.Connection.Close();
            }
        }

        public override string[] GetUsersInRole( string portalAlias, Guid roleId ) {
            ArrayList result = new ArrayList();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "aspnet_UsersInRoles_GetUsersInRoles";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = new SqlConnection( connectionString );

            cmd.Parameters.Add( "@ApplicationName", SqlDbType.NVarChar, 256 ).Value = portalAlias;
            cmd.Parameters.Add( "@RoleId", SqlDbType.UniqueIdentifier ).Value = roleId;

            SqlParameter returnCode = cmd.Parameters.Add( "@ReturnCode", SqlDbType.Int );
            returnCode.Direction = ParameterDirection.ReturnValue;

            SqlDataReader reader = null;
            try {
                cmd.Connection.Open();

                using ( reader = cmd.ExecuteReader() ) {

                    while ( reader.Read() ) {
                        result.Add( reader.GetString( 0 ) );
                    }
                    reader.Close();
                    if ( ( ( int )returnCode.Value ) == 1 ) {
                        throw new AppleseedRoleProviderException( "Role doesn't exist" );
                    }
                }

                return ( string[] )result.ToArray( typeof( string ) );
            }
            catch ( SqlException e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "GetAllRoles" );
                }

                throw new AppleseedRoleProviderException( "Error executing aspnet_UsersInRoles_GetUsersInRoles stored proc", e );
            }
            catch ( Exception e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "GetAllRoles" );
                }

                throw new AppleseedRoleProviderException( "Error getting users for role " + roleId.ToString(), e );
            }
            finally {
                if ( reader != null ) {
                    reader.Close();
                }
                cmd.Connection.Close();
            }
        }

        public override bool IsUserInRole( string portalAlias, Guid userId, Guid roleId ) {

            if ( roleId.Equals( AllUsersGuid ) || roleId.Equals( AuthenticatedUsersGuid ) ) {
                return true;
            }
            else if ( roleId.Equals( UnauthenticatedUsersGuid ) ) {
                return false;
            }
            
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "aspnet_UsersInRoles_IsUserInRole";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = new SqlConnection( connectionString );

            cmd.Parameters.Add( "@ApplicationName", SqlDbType.NVarChar, 256 ).Value = portalAlias;
            cmd.Parameters.Add( "@RoleId", SqlDbType.UniqueIdentifier ).Value = roleId;
            cmd.Parameters.Add( "@userId", SqlDbType.UniqueIdentifier ).Value = userId;

            SqlParameter returnCodeParam = cmd.Parameters.Add( "@ReturnCode", SqlDbType.Int );
            returnCodeParam.Direction = ParameterDirection.ReturnValue;

            try {
                cmd.Connection.Open();

                cmd.ExecuteNonQuery();

                int returnCode = ( int )returnCodeParam.Value;

                switch ( returnCode ) {
                    case 0:
                        return false;
                    case 1:
                        return true;
                    case 2:
                        throw new AppleseedRoleProviderException( "User with Id = " + userId + " does not exist" );
                    case 3:
                        throw new AppleseedRoleProviderException( "Role with Id = " + roleId + " does not exist" );
                    default:
                        throw new AppleseedRoleProviderException( "Error executing IsUserInRole" );
                }
            }
            catch ( SqlException e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "IsUserInRole" );
                }

                throw new AppleseedRoleProviderException( "Error executing aspnet_UsersInRoles_IsUserInRole stored proc", e );
            }
            finally {
                cmd.Connection.Close();
            }
        }

        public override void RemoveUsersFromRoles( string portalAlias, Guid[] userIds, Guid[] roleIds ) {
            string userIdsStr = string.Empty;
            string roleIdsStr = string.Empty;

            foreach ( Guid userId in userIds ) {
                userIdsStr += userId.ToString() + ",";
            }
            userIdsStr = userIdsStr.Substring( 0, userIdsStr.Length - 1 );

            foreach ( Guid roleId in roleIds ) {
                roleIdsStr += roleId.ToString() + ",";
            }
            roleIdsStr = roleIdsStr.Substring( 0, roleIdsStr.Length - 1 );

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "aspnet_UsersInRoles_RemoveUsersFromRoles";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = new SqlConnection( connectionString );

            cmd.Parameters.Add( "@ApplicationName", SqlDbType.NVarChar, 256 ).Value = portalAlias;
            cmd.Parameters.Add( "@UserIds", SqlDbType.VarChar, 4000 ).Value = userIdsStr;
            cmd.Parameters.Add( "@RoleIds", SqlDbType.VarChar, 4000 ).Value = roleIdsStr;

            SqlParameter returnCodeParam = cmd.Parameters.Add( "@ReturnCode", SqlDbType.Int );
            returnCodeParam.Direction = ParameterDirection.ReturnValue;

            try {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();

                int returnCode = ( int )returnCodeParam.Value;

                switch ( returnCode ) {
                    case 0:
                        return;
                    case 1:
                        throw new AppleseedRoleProviderException( "One of the users is not in one of the specified roles" );
                    case 2:
                        throw new AppleseedRoleProviderException( "Application " + portalAlias + " doesn't exist" );
                    case 3:
                        throw new AppleseedRoleProviderException( "One of the roles doesn't exist" );
                    case 4:
                        throw new AppleseedMembershipProviderException( "One of the users doesn't exist" );
                    default:
                        throw new AppleseedRoleProviderException( "aspnet_UsersInRoles_RemoveUsersToRoles returned error code " + returnCode );
                }
            }
            catch ( SqlException e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "CreateRole" );
                }

                throw new AppleseedRoleProviderException( "Error executing aspnet_UsersInRoles_RemoveUsersToRoles stored proc", e );
            }
            finally {
                cmd.Connection.Close();
            }
        }

        public override bool RoleExists( string portalAlias, Guid roleId ) {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "aspnet_Roles_RoleExists";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = new SqlConnection( connectionString );

            cmd.Parameters.Add( "@ApplicationName", SqlDbType.NVarChar, 256 ).Value = portalAlias;
            cmd.Parameters.Add( "@RoleId", SqlDbType.UniqueIdentifier ).Value = roleId;

            SqlParameter returnCodeParam = cmd.Parameters.Add( "@ReturnCode", SqlDbType.Int );
            returnCodeParam.Direction = ParameterDirection.ReturnValue;

            try {
                cmd.Connection.Open();

                cmd.ExecuteNonQuery();

                int returnCode = ( int )returnCodeParam.Value;
                return ( returnCode == 1 );
            }
            catch ( SqlException e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "RoleExists" );
                }

                throw new AppleseedRoleProviderException( "Error executing aspnet_Roles_RoleExists stored proc", e );
            }
            finally {
                cmd.Connection.Close();
            }
        }

        public override AppleseedRole GetRoleByName( string portalAlias, string roleName ) {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "aspnet_Roles_GetRoleByName";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = new SqlConnection( connectionString );

            cmd.Parameters.Add( "@ApplicationName", SqlDbType.NVarChar, 256 ).Value = portalAlias;
            cmd.Parameters.Add( "@RoleName", SqlDbType.NVarChar, 256 ).Value = roleName;

            SqlParameter returnCodeParam = cmd.Parameters.Add( "@ReturnCode", SqlDbType.Int );
            returnCodeParam.Direction = ParameterDirection.ReturnValue;

            SqlDataReader reader = null;
            try {
                cmd.Connection.Open();

                using ( reader = cmd.ExecuteReader() ) {

                    if ( reader.Read() ) {

                        return GetRoleFromReader( reader );
                    }
                    else {
                        throw new AppleseedRoleProviderException( "Role doesn't exist" );
                    }
                }
            }
            catch ( SqlException e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "CreateRole" );
                }

                throw new AppleseedRoleProviderException( "Error executing aspnet_UsersInRoles_IsUserInRole stored proc", e );
            }
            finally {
                if ( reader != null ) {
                    reader.Close();
                }
                cmd.Connection.Close();
            }
        }

        public override AppleseedRole GetRoleById( Guid roleId ) {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT RoleId, RoleName, Description FROM aspnet_Roles WHERE RoleId=@RoleId";
            cmd.Parameters.Add( "@RoleId", SqlDbType.UniqueIdentifier ).Value = roleId;

            cmd.Connection = new SqlConnection( connectionString );

            SqlDataReader reader = null;
            try {
                cmd.Connection.Open();

                using ( reader = cmd.ExecuteReader() ) {

                    if ( reader.Read() ) {

                        return GetRoleFromReader( reader );
                    }
                    else {
                        throw new AppleseedRoleProviderException( "Role doesn't exist" );
                    }
                }
            }
            catch ( SqlException e ) {
                if ( WriteExceptionsToEventLog ) {
                    WriteToEventLog( e, "GetRoleById" );
                }

                throw new AppleseedRoleProviderException( "Error executing method GetRoleById", e );
            }
            finally {
                if ( reader != null ) {
                    reader.Close();
                }
                cmd.Connection.Close();
            }
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// A helper function to retrieve config values from the configuration file. 
        /// </summary>
        /// <param name="configValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private string GetConfigValue( string configValue, string defaultValue ) {
            if ( String.IsNullOrEmpty( configValue ) )
                return defaultValue;

            return configValue;
        }


        /// <summary>
        /// A helper function that writes exception detail to the event log. Exceptions are written to the event log as a security 
        /// measure to avoid private database details from being returned to the browser. If a method does not return a status
        /// or boolean indicating the action succeeded or failed, a generic exception is also thrown by the caller.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="action"></param>
        private void WriteToEventLog( Exception e, string action ) {
            EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;

            string message = "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e.ToString();
            ErrorHandler.Publish(LogLevel.Error, message, e);

            //log.WriteEntry( message );
        }


        private AppleseedRole GetRoleFromReader( SqlDataReader reader ) {
            Guid roleId = reader.GetGuid( 0 );
            string roleName = reader.GetString( 1 );

            string roleDescription =  string.Empty;
            if ( !reader.IsDBNull( 2 ) ) {
                roleDescription = reader.GetString( 2 );
            }

            return new AppleseedRole( roleId, roleName, roleDescription );
        }

        #endregion
    }
}
