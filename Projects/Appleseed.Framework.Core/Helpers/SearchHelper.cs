using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Appleseed.Framework.Data;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Web.UI.WebControls;
using System.Collections.Generic;
using Appleseed.Framework.Providers.AppleseedRoleProvider;

namespace Appleseed.Framework.Helpers
{

	/// <summary>
	/// SearchHelper
	/// Original ideas from Jakob Hansen.
	/// Reflection and pluggable interface by Manu
	/// </summary>
	public class SearchHelper
	{

        /// <summary>
        /// Searches the portal.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="searchModule">The search module.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="searchField">The search field.</param>
        /// <param name="sortField">The sort field.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="topicName">Name of the topic.</param>
        /// <param name="addExtraSQL">The add extra SQL.</param>
        /// <returns>
        /// A System.Data.SqlClient.SqlDataReader value...
        /// </returns>
		// TODO --> [Obsolete("Replace me")]
		public static SqlDataReader SearchPortal(int portalID, Guid userID, string searchModule, string searchString, string searchField, string sortField, string sortDirection, string topicName, string addExtraSQL)
		{
			StringBuilder select;
			select = new StringBuilder(string.Empty, 2048);
			searchString = searchString.Replace('?','_');
			searchString = searchString.Replace(" AND "," +");
			searchString = searchString.Replace(" NOT "," -");
			searchString = searchString.Replace(" OR "," ");
			searchString = searchString.Replace(" NEAR "," ");

			if (addExtraSQL.Length != 0)
			{

				if (searchString == string.Empty)
					searchString = "AddExtraSQL:" + addExtraSQL;

				else
					searchString = "AddExtraSQL:" + addExtraSQL + " SearchString:" + searchString;
			}
			// TODO: This should be cached
			// move the code in a self cached property...
			StringBuilder modulesList = new StringBuilder();

			if (searchModule == string.Empty)
			{

				// Search all
				using (SqlDataReader drModules = GetSearchableModules(portalID)) 
				{

					try
					{

						while (drModules.Read())
						{
							modulesList.Append(drModules["AssemblyName"].ToString());
							modulesList.Append(";");
							modulesList.Append(drModules["ClassName"].ToString());
							modulesList.Append(";");
						}
					}

					finally
					{
						drModules.Close(); //by Manu, fixed bug 807858
					}
				}
			}

			else
			{
				modulesList.Append(searchModule);
				modulesList.Append(";");
			}
			// Builds searchables modules list (Assembly, Class)
			string[] arrModulesList = modulesList.ToString().TrimEnd(';').Split(';');
			// Cycle trhough modules and get search string
			ArrayList slqSelectQueries = new ArrayList();
			int modulesCount = arrModulesList.GetUpperBound(0);

			for (int i = 0; i <= modulesCount; i = i + 2)
			{
				// Use reflection
				Assembly a;
				//http://sourceforge.net/tracker/index.php?func=detail&aid=899424&group_id=66837&atid=515929
				string assemblyName = Path.WebPathCombine(Path.ApplicationRoot, "bin", arrModulesList[i]);
				assemblyName = HttpContext.Current.Server.MapPath(assemblyName);
				a = Assembly.LoadFrom(assemblyName);
				// ISearchable
				ISearchable p = (ISearchable) a.CreateInstance(arrModulesList[i+1]);

				if (p != null)
				{
					//Call a method with arguments
					object [] args = new object [] {portalID, userID, searchString, searchField};
					string currentSearch = (string) p.GetType().InvokeMember("SearchSqlSelect", BindingFlags.Default | BindingFlags.InvokeMethod, null, p, args);

					if (currentSearch.Length != 0 && currentSearch != null)
						slqSelectQueries.Add(currentSearch);
				}
			}
			int queriesCount = slqSelectQueries.Count;

			for (int i = 0; i < queriesCount; i++)
			{
				select.Append(slqSelectQueries[i]);

				if (i < (queriesCount - 1)) //Avoid last
					select.Append("\r\n\r\nUNION\r\n\r\n");
			}

			if (sortField.Length != 0)
			{

				if (sortDirection == string.Empty)
				{

					if (sortField == "CreatedDate")
						sortDirection = "DESC";

					else
						sortDirection = "ASC";
				}
				select.Append(" ORDER BY " + sortField + " " + sortDirection);
			}

			//Topic implementation
			if (topicName.Length != 0)
			{
				select.Replace("%TOPIC_PLACEHOLDER_JOIN%", "INNER JOIN rb_ModuleSettings modSet ON mod.ModuleID = modSet.ModuleID");
				select.Replace("%TOPIC_PLACEHOLDER%", " AND (modSet.SettingName = 'TopicName' AND modSet.SettingValue='" + topicName + "')");
			}

			else
			{
				select.Replace("%TOPIC_PLACEHOLDER_JOIN%", null);
				select.Replace("%TOPIC_PLACEHOLDER%", null);
			}
			string selectSQL = select.ToString();
			//string connectionString = Portal.ConnectionString;
			string connectionString = Config.ConnectionString;
			SqlConnection sqlConnection = new SqlConnection(connectionString);
			SqlCommand    sqlCommand    = new SqlCommand(selectSQL, sqlConnection);
				try 
				{
					sqlCommand.Connection.Open();
					return sqlCommand.ExecuteReader(CommandBehavior.CloseConnection); 
				}
				catch (Exception e)
				{
					ErrorHandler.Publish(LogLevel.Error, "Error in Search:SearchPortal()-> " + e.ToString() + " " + select.ToString(), e);
					throw new Exception("Error in Search selection.");
				}
			}

            /// <summary>
            /// Adds the modules that can be searched to the dropdown list.
            /// First entry is always the "All"
            /// </summary>
            /// <param name="portalID">The PortalID</param>
            /// <param name="ddList">The DropDown List</param>
		public static void AddToDropDownList(int portalID, ref DropDownList ddList)
		{

			using (SqlDataReader drModules = GetSearchableModules(portalID)) 
			{
                ddList.Items.Add(new ListItem(General.GetString("ALL", "All", null), string.Empty));

				//TODO JLH!! Should be read from context!!
				try
				{

					while (drModules.Read())
					{
						ddList.Items.Add(new ListItem(drModules["FriendlyName"].ToString(), drModules["AssemblyName"].ToString() + ";" + drModules["ClassName"].ToString()));
					}
				}

				finally
				{
					drModules.Close(); //by Manu, fixed bug 807858
				}
			}
		}

		/// <summary>
		/// The GetSearchableModules function returns a list of all Searchable modules
		/// </summary>
		/// <param name="portalID"></param>
		/// <returns></returns>
		// TODO --> [Obsolete("Replace me")]
		public static SqlDataReader GetSearchableModules(int portalID) 
		{

			//  Create Instance of Connection and Command Object
			SqlConnection myConnection = Config.SqlConnectionString;
			SqlCommand myCommand = new SqlCommand("rb_GetSearchableModules", myConnection);
					myCommand.CommandType = CommandType.StoredProcedure;
					//  Add Parameters to SPROC
					SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
					parameterPortalID.Value = portalID;
					myCommand.Parameters.Add(parameterPortalID);
					//  Execute the command
					myConnection.Open();
					SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
					//  return the datareader 
					return result;
		}

        /// <summary>
        /// Add most of the WHERE part to a search sql
        /// </summary>
        /// <param name="portalID">portalID</param>
        /// <param name="userID">userID</param>
        /// <param name="select">SQL string to add sql to</param>
        /// <param name="fieldName">Field to do IS NOT NULL test on</param>
		public static void AddSharedSQL(int portalID, int userID, ref StringBuilder select, string fieldName) 
		{ 

			HttpContext context=HttpContext.Current;
            IList<AppleseedRole> asRoles = PortalSecurity.GetRoles(); 

/*
			if (userID>-1) 
			select.Append(", rb_Roles, rb_UserRoles"); 
			select.Append(" WHERE itm." + fieldName + " IS NOT NULL"); 
			select.Append(" AND itm.ModuleID = mod.ModuleID"); 
			select.Append(" AND mod.ModuleDefID = modDef.ModuleDefID"); 
			select.Append(" AND modDef.PortalID = " + portalID.ToString()); 
			select.Append(" AND tab.PortalID = " + portalID.ToString()); 
			select.Append(" AND tab.TabID = mod.TabID"); 
*/

			if (userID>-1) 
			{ 
				//select.Append(" AND rb_UserRoles.UserID = " + userID.ToString()); 
				//select.Append(" AND rb_UserRoles.RoleID = rb_Roles.RoleID"); 
				//select.Append(" AND rb_Roles.PortalID = " + portalID.ToString()); 


				select.Append(" AND ((mod.AuthorizedViewRoles LIKE '%All Users%') "); 
				if (context.Request.IsAuthenticated) // - no tenia en cuenta el rol "Authenticated users" 
					select.Append(" OR (mod.AuthorizedViewRoles LIKE '%Authenticated Users%')"); 
				else 
					select.Append(" OR (mod.AuthorizedViewRoles LIKE '%Unauthenticated Users%')"); 
				foreach (AppleseedRole sRole in asRoles) 
					select.Append(" OR (mod.AuthorizedViewRoles LIKE '%" + sRole.Name + "%')"); 
				select.Append(")"); 
				select.Append(" AND ((tab.AuthorizedRoles LIKE '%All Users%')"); 
				if (context.Request.IsAuthenticated) // - no tenia en cuenta el rol "Authenticated users" 
					select.Append(" OR (tab.AuthorizedRoles LIKE '%Authenticated Users%')"); 
				foreach (AppleseedRole sRole in asRoles) 
					select.Append(" OR (tab.AuthorizedRoles LIKE '%" + sRole.Name + "%')"); 
				select.Append(")"); 

			} 
			else 
			{ 
				select.Append(" AND (mod.AuthorizedViewRoles LIKE '%All Users%')"); 
				select.Append(" AND (tab.AuthorizedRoles LIKE '%All Users%')"); 
			} 
		} 

		// old search
		/* public static void AddSharedSQL(int portalID, int userID, ref StringBuilder select, string fieldName)
		{

			if (userID>-1)
				select.Append(", rb_Roles, rb_UserRoles");
			select.Append(" WHERE itm." + fieldName + " IS NOT NULL");
			select.Append(" AND itm.ModuleID = mod.ModuleID");
			select.Append(" AND mod.ModuleDefID = modDef.ModuleDefID");
			select.Append(" AND modDef.PortalID = " + portalID.ToString());
			select.Append(" AND tab.PortalID = " + portalID.ToString());
			select.Append(" AND tab.TabID = mod.TabID");

			if (userID>-1)
			{
				select.Append(" AND rb_UserRoles.UserID = " + userID.ToString());
				select.Append(" AND rb_UserRoles.RoleID = rb_Roles.RoleID");
				select.Append(" AND rb_Roles.PortalID = " + portalID.ToString());
				select.Append(" AND ((mod.AuthorizedViewRoles LIKE '%All Users%') OR (mod.AuthorizedViewRoles LIKE '%'+rb_Roles.RoleName+'%'))");
				select.Append(" AND ((tab.AuthorizedRoles LIKE '%All Users%') OR (tab.AuthorizedRoles LIKE '%'+rb_Roles.RoleName+'%'))");
			}

			else
			{
				select.Append(" AND (mod.AuthorizedViewRoles LIKE '%All Users%')");
				select.Append(" AND (tab.AuthorizedRoles LIKE '%All Users%')");
			}
		} */

        /// <summary>
        /// Creates search sql to the WHERE clause
        /// </summary>
        /// <param name="arrFields">Array of fieldnames to search e.g. ArrayList("fld1", "fld2")</param>
        /// <param name="strSearchwords">Whatever words the user entered to perform the search with e.g. "smukke jakob". Note: Must be seperated by spaces!</param>
        /// <param name="useAnd">Controls wether all or only one word in the searchstring strSearchwords should result in a hit.</param>
        /// <returns>
        /// (fld1 LIKE '%smukke%' OR fld2 LIKE '%smukke%') AND (fld1 LIKE '%jakob%' OR fld2 LIKE '%jakob%')
        /// </returns>
		public static string CreateTestSQL(ArrayList arrFields, string strSearchwords, bool useAnd)
		{
			Int32 i = 0;
			Int32 j = 0;
			string [] arrWord;
			string strWord, strTmp, strNot;
			string strBoolOp;
			string vbCrLf = "\r\n";

			if (useAnd)
				strBoolOp = "AND";

			else
				strBoolOp = "OR";
			strTmp = string.Empty;

			if (strSearchwords.IndexOf('"') > -1)
				MarkPhrase(ref strSearchwords);
			strSearchwords = strSearchwords.Replace(" +", " ");
			arrWord = strSearchwords.Split(' ');

			foreach (string strItem in arrWord)
			{
				strWord = strItem;
				strWord = strWord.Replace('=', ' ');  // dephrase!

				if (strWord.StartsWith("-"))
				{
					strNot = "NOT";
					strWord = strWord.Substring(1);
				}

				else
					strNot = string.Empty;

				if (strTmp == string.Empty) strTmp = strNot + "(";

				if (j>0)
					strTmp = strTmp + ")" + vbCrLf + " " + strBoolOp + " " + strNot + "(";
				j = j + 1;

				foreach (string strField in arrFields)
				{

					if (i>0)
						strTmp = strTmp + " OR ";
					strTmp = strTmp + strField + " LIKE N'%" + strWord + "%'";
					i = i + 1;
				}
				i = 0;
			}
			return "(" + strTmp + "))";
		}

        /// <summary>
        /// A phrase is marked with '"' as a stating and ending charater.
        /// If a phrase is discovered the '"' is replaced with '='.
        /// </summary>
        /// <param name="strWords">The staring of words that might contain a phrase</param>
		public static void MarkPhrase(ref string strWords)
		{
			int idxStartQuote = strWords.IndexOf('"');
			int idxEndQuote = strWords.IndexOf('"', idxStartQuote+1);

			if (idxEndQuote == -1) return;
			string phrase;
			phrase = strWords.Substring(idxStartQuote+1, (idxEndQuote-idxStartQuote)-1);
			phrase = phrase.Replace(' ', '=');
			strWords = strWords.Substring(0, idxStartQuote) + phrase + strWords.Substring(idxEndQuote+1);
		}

        /// <summary>
        /// DeleteBeforeBody
        /// </summary>
        /// <param name="strHtml">strHtml</param>
        /// <returns></returns>
		public static string DeleteBeforeBody(string strHtml)
		{
			int idxBody = strHtml.ToLower().IndexOf("<body");

			// Get rid of anything before the body tag
			if (idxBody != -1)
				strHtml = strHtml.Substring(idxBody);
			return strHtml;
		}

        /// <summary>
        /// Gets the topic list.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns>A string[] value...</returns>
		public static string[] GetTopicList(int portalID)
		{
			ArrayList al = new ArrayList();
            al.Add(General.GetString("PORTALSEARCH_ALL", "All", null));
			SqlDataReader dr = DBHelper.GetDataReader
				("SELECT DISTINCT rb_ModuleSettings.SettingValue " +
				"FROM rb_ModuleSettings INNER JOIN rb_Modules ON rb_ModuleSettings.ModuleID = rb_Modules.ModuleID INNER JOIN rb_ModuleDefinitions ON rb_Modules.ModuleDefID = rb_ModuleDefinitions.ModuleDefID " + 
				"WHERE (rb_ModuleDefinitions.PortalID = " + portalID.ToString()  + ") AND (rb_ModuleSettings.SettingName = N'TopicName') AND (rb_ModuleSettings.SettingValue <> '') ORDER BY rb_ModuleSettings.SettingValue");

			try
			{

				while(dr.Read())
				{
					al.Add(dr["SettingValue"].ToString());
				}
			}

			finally
			{
				dr.Close(); //by Manu fix close bug #2 found
			}
			return (string[]) al.ToArray(typeof(string));
		}
	}
}