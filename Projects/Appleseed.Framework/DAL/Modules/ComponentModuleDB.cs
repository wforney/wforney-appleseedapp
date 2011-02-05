using System.Data;
using System.Data.SqlClient;

using Appleseed.Framework.Settings;
using Appleseed.Framework.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Data;
namespace Appleseed.Framework.Content.Data
{
	public class ComponentModuleDB
	{
        /// <summary>
        /// GetComponentModule
        /// </summary>
        /// <param name="ModuleID">ModuleID</param>
        /// <returns>A SqlDataReader</returns>
		public SqlDataReader GetComponentModule(int ModuleID)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetComponentModule", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int);
			parameterModuleID.Value = ModuleID;
			myCommand.Parameters.Add(parameterModuleID);

			// Execute the command
			myConnection.Open();
			SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

			// Return the datareader
			return result;
		}

        /// <summary>
        /// UpdateComponentModule
        /// </summary>
        /// <param name="ModuleID">The module ID.</param>
        /// <param name="CreatedByUser">The created by user.</param>
        /// <param name="Title">The title.</param>
        /// <param name="Component">Void</param>
		public void UpdateComponentModule(int ModuleID, string CreatedByUser, string Title, string Component)
		{
			// Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_UpdateComponentModule", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			// Update Parameters to SPROC
			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int);
			parameterModuleID.Value = ModuleID;
			myCommand.Parameters.Add(parameterModuleID);

			SqlParameter parameterCreatedByUser = new SqlParameter("@CreatedByUser", SqlDbType.NVarChar, 100);
			parameterCreatedByUser.Value = CreatedByUser;
			myCommand.Parameters.Add(parameterCreatedByUser);

			SqlParameter parameterTitle = new SqlParameter("@Title", SqlDbType.NVarChar, 100);
			parameterTitle.Value = Title;
			myCommand.Parameters.Add(parameterTitle);

			SqlParameter parameterComponent = new SqlParameter("@Component", SqlDbType.NVarChar, 2000);
			parameterComponent.Value = Component;
			myCommand.Parameters.Add(parameterComponent);

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