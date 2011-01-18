using System;
using System.Data;
using System.Data.SqlClient;
using Appleseed.Framework.Settings;

namespace Appleseed.Framework.Content.Data
{
    /// <summary>
    /// Class that encapsulates all data logic necessary
    /// articles within the Portal database.
    /// </summary>
    public class ArticlesDB
    {
        //This is used as a common setting from Articles
        public static string ImagesSetting = "ImageCollection";

        /// <summary>
        /// The GetArticles method returns a SqlDataReader containing all of the
        /// Articles for a specific portal module from the announcements
        /// database.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public SqlDataReader GetArticles(int moduleID, WorkFlowVersion version)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetArticles", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

            SqlParameter parameterWorkflowVersion = new SqlParameter("@WorkflowVersion", SqlDbType.Int, 4);
            parameterWorkflowVersion.Value = (int) version;
            myCommand.Parameters.Add(parameterWorkflowVersion);

            // Execute the command
            myConnection.Open();
            SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the datareader 
            return result;
        }

        /// <summary>
        /// The GetArticlesAll method returns a SqlDataReader containing all of the
        /// Articles for a specific portal module from the announcements
        /// database (including expired one).
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public SqlDataReader GetArticlesAll(int moduleID, WorkFlowVersion version)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetArticlesAll", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

            SqlParameter parameterWorkflowVersion = new SqlParameter("@WorkflowVersion", SqlDbType.Int, 4);
            parameterWorkflowVersion.Value = (int) version;
            myCommand.Parameters.Add(parameterWorkflowVersion);

            // Execute the command
            myConnection.Open();
            SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the datareader 
            return result;
        }

        /// <summary>
        /// The GetSingleArticle method returns a SqlDataReader containing details
        /// about a specific Article from the Articles database table.
        /// </summary>
        /// <param name="itemID">The item ID.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public SqlDataReader GetSingleArticle(int itemID, WorkFlowVersion version)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetSingleArticle", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterItemID = new SqlParameter("@ItemID", SqlDbType.Int, 4);
            parameterItemID.Value = itemID;
            myCommand.Parameters.Add(parameterItemID);

            SqlParameter parameterWorkflowVersion = new SqlParameter("@WorkflowVersion", SqlDbType.Int, 4);
            parameterWorkflowVersion.Value = (int) version;
            myCommand.Parameters.Add(parameterWorkflowVersion);

            // Execute the command
            myConnection.Open();
            SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the datareader 
            return result;
        }

        /// <summary>
        /// The DeleteArticle method deletes a specified Article from
        /// the Articles database table.
        /// </summary>
        /// <param name="itemID">The item ID.</param>
        public void DeleteArticle(int itemID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_DeleteArticle", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterItemID = new SqlParameter("@ItemID", SqlDbType.Int, 4);
            parameterItemID.Value = itemID;
            myCommand.Parameters.Add(parameterItemID);

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
        /// The AddArticle method adds a new Article within the
        /// Articles database table, and returns ItemID value as a result.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="title">The title.</param>
        /// <param name="subtitle">The subtitle.</param>
        /// <param name="articleAbstract">The article abstract.</param>
        /// <param name="description">The description.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="expireDate">The expire date.</param>
        /// <param name="isInNewsletter">if set to <c>true</c> [is in newsletter].</param>
        /// <param name="moreLink">The more link.</param>
        /// <returns></returns>
        public int AddArticle(int moduleID, string userName, string title, string subtitle, string articleAbstract,
                              string description, DateTime startDate, DateTime expireDate, bool isInNewsletter,
                              string moreLink)
        {
            if (userName.Length < 1)
            {
                userName = "unknown";
            }

            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_AddArticle", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterItemID = new SqlParameter("@ItemID", SqlDbType.Int, 4);
            parameterItemID.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterItemID);

            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

            SqlParameter parameterUserName = new SqlParameter("@UserName", SqlDbType.NVarChar, 100);
            parameterUserName.Value = userName;
            myCommand.Parameters.Add(parameterUserName);

            SqlParameter parameterTitle = new SqlParameter("@Title", SqlDbType.NVarChar, 100);
            parameterTitle.Value = title;
            myCommand.Parameters.Add(parameterTitle);

            SqlParameter parameterSubtitle = new SqlParameter("@Subtitle", SqlDbType.NVarChar, 200);
            parameterSubtitle.Value = subtitle;
            myCommand.Parameters.Add(parameterSubtitle);

            SqlParameter parameterAbstract = new SqlParameter("@Abstract", SqlDbType.NText);
            parameterAbstract.Value = articleAbstract;
            myCommand.Parameters.Add(parameterAbstract);

            SqlParameter parameterDescription = new SqlParameter("@Description", SqlDbType.NText);
            parameterDescription.Value = description;
            myCommand.Parameters.Add(parameterDescription);

            SqlParameter parameterStartDate = new SqlParameter("@StartDate", SqlDbType.DateTime);
            parameterStartDate.Value = startDate;
            myCommand.Parameters.Add(parameterStartDate);

            SqlParameter parameterExpireDate = new SqlParameter("@ExpireDate", SqlDbType.DateTime);
            parameterExpireDate.Value = expireDate;
            myCommand.Parameters.Add(parameterExpireDate);

            SqlParameter parameterIsInNewsletter = new SqlParameter("@IsInNewsletter", SqlDbType.Bit);
            parameterIsInNewsletter.Value = isInNewsletter;
            myCommand.Parameters.Add(parameterIsInNewsletter);

            SqlParameter parameterMoreLink = new SqlParameter("@MoreLink", SqlDbType.NVarChar, 150);
            parameterMoreLink.Value = moreLink;
            myCommand.Parameters.Add(parameterMoreLink);

            myConnection.Open();
            try
            {
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                myConnection.Close();
            }

            return (int) parameterItemID.Value;
        }

        /// <summary>
        /// The UpdateArticle method updates a specified Article within
        /// the Articles database table.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="itemID">The item ID.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="title">The title.</param>
        /// <param name="subtitle">The subtitle.</param>
        /// <param name="articleAbstract">The article abstract.</param>
        /// <param name="description">The description.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="expireDate">The expire date.</param>
        /// <param name="isInNewsletter">if set to <c>true</c> [is in newsletter].</param>
        /// <param name="moreLink">The more link.</param>
        public void UpdateArticle(int moduleID, int itemID, string userName, string title, string subtitle,
                                  string articleAbstract, string description, DateTime startDate, DateTime expireDate,
                                  bool isInNewsletter, string moreLink)
        {
            if (userName.Length < 1)
            {
                userName = "unknown";
            }

            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_UpdateArticle", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterItemID = new SqlParameter("@ItemID", SqlDbType.Int, 4);
            parameterItemID.Value = itemID;
            myCommand.Parameters.Add(parameterItemID);

            SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
            parameterModuleID.Value = moduleID;
            myCommand.Parameters.Add(parameterModuleID);

            SqlParameter parameterUserName = new SqlParameter("@UserName", SqlDbType.NVarChar, 100);
            parameterUserName.Value = userName;
            myCommand.Parameters.Add(parameterUserName);

            SqlParameter parameterTitle = new SqlParameter("@Title", SqlDbType.NVarChar, 100);
            parameterTitle.Value = title;
            myCommand.Parameters.Add(parameterTitle);

            SqlParameter parameterSubtitle = new SqlParameter("@Subtitle", SqlDbType.NVarChar, 200);
            parameterSubtitle.Value = subtitle;
            myCommand.Parameters.Add(parameterSubtitle);

            SqlParameter parameterAbstract = new SqlParameter("@Abstract", SqlDbType.NText);
            parameterAbstract.Value = articleAbstract;
            myCommand.Parameters.Add(parameterAbstract);

            SqlParameter parameterDescription = new SqlParameter("@Description", SqlDbType.NText);
            parameterDescription.Value = description;
            myCommand.Parameters.Add(parameterDescription);

            SqlParameter parameterStartDate = new SqlParameter("@StartDate", SqlDbType.DateTime);
            parameterStartDate.Value = startDate;
            myCommand.Parameters.Add(parameterStartDate);

            SqlParameter parameterExpireDate = new SqlParameter("@ExpireDate", SqlDbType.DateTime);
            parameterExpireDate.Value = expireDate;
            myCommand.Parameters.Add(parameterExpireDate);

            SqlParameter parameterIsInNewsletter = new SqlParameter("@IsInNewsletter", SqlDbType.Bit);
            parameterIsInNewsletter.Value = isInNewsletter;
            myCommand.Parameters.Add(parameterIsInNewsletter);

            SqlParameter parameterMoreLink = new SqlParameter("@MoreLink", SqlDbType.NVarChar, 150);
            parameterMoreLink.Value = moreLink;
            myCommand.Parameters.Add(parameterMoreLink);

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