using System;
using System.Data;
using System.Data.SqlClient;
using Appleseed.Framework.Settings;

namespace Appleseed.Framework.Site.Data
{
    /// <summary>
    /// Class that encapsulates all data logic necessary 
    /// to publish a module using workflow
    /// </summary>
    public class WorkFlowDB
    {
        /// <summary>
        /// This function publishes the staging data of a module.
        /// </summary>
        /// <param name="moduleID"></param>
        public static void Publish(int moduleID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_Publish", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

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

        /// <summary>
        /// This function reverts the staging data to the content in production of a module.
        /// </summary>
        /// <param name="moduleID"></param>
        public static void Revert(int moduleID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_Revert", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

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

        /// <summary>
        /// This function puts the status of a module to request approval
        /// </summary>
        /// <param name="moduleID"></param>
        public static void RequestApproval(int moduleID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_RequestApproval", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@moduleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

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

        /// <summary>
        /// This function puts the status of a module to approved
        /// </summary>
        /// <param name="moduleID"></param>
        public static void Approve(int moduleID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_Approve", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@moduleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

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

        /// <summary>
        /// This function puts the status of a module back to working
        /// </summary>
        /// <param name="moduleID"></param>
        public static void Reject(int moduleID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_Reject", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@moduleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

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

        public static void GetLastModified(int moduleID, WorkFlowVersion Version, ref string Email,
                                           ref DateTime Timestamp)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetLastModified", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

            SqlParameter parameterWorkflowVersion = new SqlParameter("@WorkflowVersion", SqlDbType.Int, 4);
            parameterWorkflowVersion.Value = (int) Version;
            myCommand.Parameters.Add(parameterWorkflowVersion);

            SqlParameter parameterEmail = new SqlParameter("@LastModifiedBy", SqlDbType.NVarChar, 256);
            parameterEmail.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterEmail);

            SqlParameter parameterDate = new SqlParameter("@LastModifiedDate", SqlDbType.DateTime, 8);
            parameterDate.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterDate);

            myConnection.Open();
            try
            {
                myCommand.ExecuteNonQuery();
                Email = Convert.IsDBNull(parameterEmail.Value) ? string.Empty : (string) parameterEmail.Value;
                Timestamp = Convert.IsDBNull(parameterDate.Value) ? DateTime.MinValue : (DateTime) parameterDate.Value;
            }
            finally
            {
                myConnection.Close();
            }
        }

        public static void SetLastModified(int moduleID, string email)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_SetLastModified", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

            SqlParameter parameterEmail = new SqlParameter("@LastModifiedBy", SqlDbType.NVarChar, 256);
            parameterEmail.Value = email;
            myCommand.Parameters.Add(parameterEmail);

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