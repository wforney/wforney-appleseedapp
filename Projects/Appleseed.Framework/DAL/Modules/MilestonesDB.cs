using System;
using System.Data;
using System.Data.SqlClient;

using Appleseed.Framework;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Data;
namespace Appleseed.Framework.Content.Data
{
	public class MilestonesDB
	{
        /// <summary>
        /// GetSingleMilestones
        /// </summary>
        /// <param name="ItemID">ItemID</param>
        /// <param name="version">The version.</param>
        /// <returns>A SqlDataReader</returns>
		
		// change by David.Verberckmoes@syntegra.com in order to support workflow
		// Date: 20030324
		public SqlDataReader GetSingleMilestones(int ItemID, WorkFlowVersion version)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetSingleMilestones", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterItemID = new SqlParameter("@ItemID", SqlDbType.Int);
			parameterItemID.Value = ItemID;
			myCommand.Parameters.Add(parameterItemID);

			// Change by David.Verberckmoes@Syntegra.com on 20030324
			SqlParameter parameterWorkflowVersion = new SqlParameter("@WorkflowVersion", SqlDbType.Int, 4);
			parameterWorkflowVersion.Value = (int)version;
			myCommand.Parameters.Add(parameterWorkflowVersion);
			// End Change David.Verberckmoes@Syntegra.com 

			// Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

			// Return the datareader
			return result;
		}


        /// <summary>
        /// GetMilestones
        /// </summary>
        /// <param name="ModuleID">The module ID.</param>
        /// <param name="version">The version.</param>
        /// <returns>A SqlDataReader</returns>

		// change by David.Verberckmoes@syntegra.com in order to support workflow
		// Date: 20030324
		public SqlDataReader GetMilestones(int ModuleID, WorkFlowVersion version)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetMilestones", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int);
			parameterModuleID.Value = ModuleID;
			myCommand.Parameters.Add(parameterModuleID);

			// Change by David.Verberckmoes@Syntegra.com on 20030324
			SqlParameter parameterWorkflowVersion = new SqlParameter("@WorkflowVersion", SqlDbType.Int, 4);
			parameterWorkflowVersion.Value = (int)version;
			myCommand.Parameters.Add(parameterWorkflowVersion);
			// End Change David.Verberckmoes@Syntegra.com 

			// Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

			// Return the datareader
			return result;
		}


        /// <summary>
        /// DeleteMilestones
        /// </summary>
        /// <param name="ItemID">ItemID</param>
		public void DeleteMilestones(int ItemID)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_DeleteMilestones", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterItemID = new SqlParameter("@ItemID", SqlDbType.Int);
			parameterItemID.Value = ItemID;
			myCommand.Parameters.Add(parameterItemID);

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


        /// <summary>
        /// AddMilestones
        /// </summary>
        /// <param name="ItemID">ItemID</param>
        /// <param name="ModuleID">The module ID.</param>
        /// <param name="CreatedByUser">The created by user.</param>
        /// <param name="CreatedDate">The created date.</param>
        /// <param name="Title">The title.</param>
        /// <param name="EstCompleteDate">The est complete date.</param>
        /// <param name="Status">The status.</param>
        /// <returns>The newly created ID</returns>
		public int AddMilestones(int ItemID, int ModuleID, string CreatedByUser, DateTime CreatedDate, string Title, DateTime EstCompleteDate, string Status)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_AddMilestones", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterItemID = new SqlParameter("@ItemID", SqlDbType.Int);
			parameterItemID.Direction = ParameterDirection.Output;
			myCommand.Parameters.Add(parameterItemID);

			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int);
			parameterModuleID.Value = ModuleID;
			myCommand.Parameters.Add(parameterModuleID);

			SqlParameter parameterCreatedByUser = new SqlParameter("@CreatedByUser", SqlDbType.NVarChar, 100);
			parameterCreatedByUser.Value = CreatedByUser;
			myCommand.Parameters.Add(parameterCreatedByUser);

			SqlParameter parameterCreatedDate = new SqlParameter("@CreatedDate", SqlDbType.DateTime);
			parameterCreatedDate.Value = CreatedDate;
			myCommand.Parameters.Add(parameterCreatedDate);

			SqlParameter parameterTitle = new SqlParameter("@Title", SqlDbType.NVarChar, 100);
			parameterTitle.Value = Title;
			myCommand.Parameters.Add(parameterTitle);

			SqlParameter parameterEstCompleteDate = new SqlParameter("@EstCompleteDate", SqlDbType.DateTime);
			parameterEstCompleteDate.Value = EstCompleteDate;
			myCommand.Parameters.Add(parameterEstCompleteDate);

			SqlParameter parameterStatus = new SqlParameter("@Status", SqlDbType.NVarChar, 100);
			parameterStatus.Value = Status;
			myCommand.Parameters.Add(parameterStatus);


			// Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

			// Return the datareader
			return (int)parameterItemID.Value;
		}


        /// <summary>
        /// UpdateMilestones
        /// </summary>
        /// <param name="ItemID">ItemID</param>
        /// <param name="ModuleID">The module ID.</param>
        /// <param name="CreatedByUser">The created by user.</param>
        /// <param name="CreatedDate">The created date.</param>
        /// <param name="Title">The title.</param>
        /// <param name="EstCompleteDate">The est complete date.</param>
        /// <param name="Status">The status.</param>
		public void UpdateMilestones(int ItemID, int ModuleID, string CreatedByUser, DateTime CreatedDate, string Title, DateTime EstCompleteDate, string Status)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_UpdateMilestones", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Update Parameters to SPROC
			SqlParameter parameterItemID = new SqlParameter("@ItemID", SqlDbType.Int);
			parameterItemID.Value = ItemID;
			myCommand.Parameters.Add(parameterItemID);

			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int);
			parameterModuleID.Value = ModuleID;
			myCommand.Parameters.Add(parameterModuleID);

			SqlParameter parameterCreatedByUser = new SqlParameter("@CreatedByUser", SqlDbType.NVarChar, 100);
			parameterCreatedByUser.Value = CreatedByUser;
			myCommand.Parameters.Add(parameterCreatedByUser);

			SqlParameter parameterCreatedDate = new SqlParameter("@CreatedDate", SqlDbType.DateTime);
			parameterCreatedDate.Value = CreatedDate;
			myCommand.Parameters.Add(parameterCreatedDate);

			SqlParameter parameterTitle = new SqlParameter("@Title", SqlDbType.NVarChar, 100);
			parameterTitle.Value = Title;
			myCommand.Parameters.Add(parameterTitle);

			SqlParameter parameterEstCompleteDate = new SqlParameter("@EstCompleteDate", SqlDbType.DateTime);
			parameterEstCompleteDate.Value = EstCompleteDate;
			myCommand.Parameters.Add(parameterEstCompleteDate);

			SqlParameter parameterStatus = new SqlParameter("@Status", SqlDbType.NVarChar, 100);
			parameterStatus.Value = Status;
			myCommand.Parameters.Add(parameterStatus);


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
