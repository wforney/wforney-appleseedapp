using System;
using System.Collections;
using System.Text;

namespace Appleseed.Framework.Helpers
{
    /// <summary>
    /// This struct stores custom parametes needed by
    /// the search helper for do the search string.
    /// This make the search string consistent and easy
    /// to change without modify all the searchable modules
    /// </summary>
    public struct SearchDefinition
    {
        private const string strItm = "itm.";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SearchDefinition"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="titleField">The title field.</param>
        /// <param name="abstractField">The abstract field.</param>
        /// <param name="searchField">The search field.</param>
        /// <returns>
        /// A void value...
        /// </returns>
        public SearchDefinition(string tableName, string titleField, string abstractField, string searchField)
        {
            TableName = tableName;
            PageIDField = "mod.TabID";
            ItemIDField = "ItemID";
            TitleField = titleField;
            AbstractField = abstractField;
            CreatedByUserField = "''";
            CreatedDateField = "''";
            ArrSearchFields = new ArrayList();

            if (searchField == string.Empty)
            {
                ArrSearchFields.Add(strItm + TitleField);
                ArrSearchFields.Add(strItm + AbstractField);
            }

            else
            {
                if (searchField == "Title")
                    ArrSearchFields.Add(strItm + TitleField);

                else
                    ArrSearchFields.Add(strItm + searchField);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SearchDefinition"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="titleField">The title field.</param>
        /// <param name="abstractField">The abstract field.</param>
        /// <param name="createdByUserField">The created by user field.</param>
        /// <param name="createdDateField">The created date field.</param>
        /// <param name="searchField">The search field.</param>
        /// <returns>
        /// A void value...
        /// </returns>
        public SearchDefinition(string tableName, string titleField, string abstractField, string createdByUserField,
                                string createdDateField, string searchField)
        {
            TableName = tableName;
            PageIDField = "mod.TabID";
            ItemIDField = "ItemID";
            TitleField = titleField;
            AbstractField = abstractField;
            CreatedByUserField = createdByUserField;
            CreatedDateField = createdDateField;
            ArrSearchFields = new ArrayList();

            if (searchField == string.Empty)
            {
                ArrSearchFields.Add(strItm + TitleField);
                ArrSearchFields.Add(strItm + AbstractField);
            }

            else
            {
                if (searchField == "Title")
                    ArrSearchFields.Add(strItm + TitleField);

                else
                    ArrSearchFields.Add(strItm + searchField);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SearchDefinition"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tabIDField">The tab ID field.</param>
        /// <param name="itemIDField">The item ID field.</param>
        /// <param name="titleField">The title field.</param>
        /// <param name="abstractField">The abstract field.</param>
        /// <param name="createdByUserField">The created by user field.</param>
        /// <param name="createdDateField">The created date field.</param>
        /// <param name="searchField">The search field.</param>
        /// <returns>
        /// A void value...
        /// </returns>
        public SearchDefinition(string tableName, string tabIDField, string itemIDField, string titleField,
                                string abstractField, string createdByUserField, string createdDateField,
                                string searchField)
        {
            TableName = tableName;
            PageIDField = tabIDField;
            ItemIDField = itemIDField;
            TitleField = titleField;
            AbstractField = abstractField;
            CreatedByUserField = createdByUserField;
            CreatedDateField = createdDateField;
            ArrSearchFields = new ArrayList();

            if (searchField == string.Empty)
            {
                ArrSearchFields.Add(strItm + TitleField);
                ArrSearchFields.Add(strItm + AbstractField);
            }

            else
            {
                if (searchField == "Title")
                    ArrSearchFields.Add(strItm + TitleField);

                else
                    ArrSearchFields.Add(strItm + searchField);
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <remarks>
        ///     
        /// </remarks>
        public string TableName;

        /// <summary>
        ///     
        /// </summary>
        /// <remarks>
        ///     
        /// </remarks>
        public string PageIDField;

        /// <summary>
        ///     
        /// </summary>
        /// <remarks>
        ///     
        /// </remarks>
        public string ItemIDField;

        /// <summary>
        ///     
        /// </summary>
        /// <remarks>
        ///     
        /// </remarks>
        public string TitleField;

        /// <summary>
        ///     
        /// </summary>
        /// <remarks>
        ///     
        /// </remarks>
        public string AbstractField;

        /// <summary>
        ///     
        /// </summary>
        /// <remarks>
        ///     
        /// </remarks>
        public string CreatedByUserField;

        /// <summary>
        ///     
        /// </summary>
        /// <remarks>
        ///     
        /// </remarks>
        public string CreatedDateField;

        /// <summary>
        ///     
        /// </summary>
        /// <remarks>
        ///     
        /// </remarks>
        public ArrayList ArrSearchFields;

        /// <summary>
        /// Searches the SQL select.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="searchStr">The search STR.</param>
        /// <returns>A string value...</returns>
        public string SearchSqlSelect(int portalID, int userID, string searchStr)
        {
            return SearchSqlSelect(portalID, userID, searchStr, true);
        }

        /// <summary>
        /// SQL injection prevention
        /// </summary>
        /// <param name="toClean">To clean.</param>
        /// <returns></returns>
        private string FilterString(string toClean)
        {
            StringBuilder c = new StringBuilder(toClean);
            string[] knownbad =
                {
                    "select", "insert",
                    "update", "delete", "drop",
                    "--", "'", "char", ";"
                };

            for (int i = 0; i < knownbad.Length; i++)
                c.Replace(knownbad[i], string.Empty);
            return c.ToString();
        }

        /// <summary>
        /// Builds a SELECT query using given parameters
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="searchStr">The search STR.</param>
        /// <param name="hasItemID">if set to <c>true</c> [has item ID].</param>
        /// <returns></returns>
        public string SearchSqlSelect(int portalID, int userID, string searchStr, bool hasItemID)
        {
            if (CreatedByUserField == null || CreatedByUserField.Length == 0)
                CreatedByUserField = "''";

            if (CreatedDateField == null || CreatedDateField.Length == 0)
                CreatedDateField = "''";
            //SQL inection filter
            searchStr = FilterString(searchStr);

            if (searchStr.Length < 3)
                throw new ArgumentException(
                    "Please use a word with at least 3 valid chars (invalid chars were removed).");
            // special extended search feature (used by RSS/Community Service). Added by Jakob Hansen
            string ExtraSQL = string.Empty;

            if (searchStr.StartsWith("AddExtraSQL:"))
            {
                int posSS = searchStr.IndexOf("SearchString:");

                if (posSS > 0)
                {
                    // Get the added searchstring
                    if (posSS > 12)
                        ExtraSQL = searchStr.Substring(12, posSS - 12).Trim();

                    else
                        ExtraSQL = string.Empty; // no SQL - only searchstring
                    searchStr = searchStr.Substring(posSS + 14).Trim();
                }

                else
                {
                    // There are no added searchstring
                    ExtraSQL = searchStr.Substring(12).Trim();
                    searchStr = string.Empty;
                }

                // Are the required "AND " missing? (then add it!)
                if (ExtraSQL.Length != 0 && !ExtraSQL.StartsWith("AND"))
                    ExtraSQL = "AND " + ExtraSQL;
            }
            StringBuilder select = new StringBuilder();
            select.Append("SELECT TOP 50 ");
            select.Append("genModDef.FriendlyName AS ModuleName, ");
            select.Append("CAST (itm.");
            select.Append(TitleField);
            select.Append(" AS NVARCHAR(100)) AS Title, ");
            select.Append("CAST (itm.");
            select.Append(AbstractField);
            select.Append(" AS NVARCHAR(100)) AS Abstract, ");
            select.Append("itm.ModuleID AS ModuleID, ");

            if (hasItemID)
                select.Append(strItm + ItemIDField + " AS ItemID, ");

            else
                select.Append("itm.ModuleID AS ItemID, ");

            if (!CreatedByUserField.StartsWith("'"))
                select.Append(strItm); // Add itm only if not a constant value
            select.Append(CreatedByUserField);
            select.Append(" AS CreatedByUser, ");

            if (!CreatedDateField.StartsWith("'"))
                select.Append(strItm); // Add itm only if not a constant value
            select.Append(CreatedDateField);
            select.Append(" AS CreatedDate, ");
            select.Append(PageIDField + " AS TabID, ");
            select.Append("tab.TabName AS TabName, ");
            select.Append("genModDef.GeneralModDefID AS GeneralModDefID, ");
            select.Append("mod.ModuleTitle AS ModuleTitle ");
            select.Append("FROM ");
            select.Append(TableName);
            select.Append(" itm INNER JOIN ");
            select.Append("rb_Modules mod ON itm.ModuleID = mod.ModuleID INNER JOIN ");
            select.Append("rb_ModuleDefinitions modDef ON mod.ModuleDefID = modDef.ModuleDefID INNER JOIN ");
            select.Append("rb_Pages tab ON mod.TabID = tab.PageID INNER JOIN ");
            select.Append("rb_GeneralModuleDefinitions genModDef ON modDef.GeneralModDefID = genModDef.GeneralModDefID ");
            //			if (topicName.Length != 0)
            //				select.Append("INNER JOIN rb_ModuleSettings modSet ON mod.ModuleID = modSet.ModuleID");
            select.Append("%TOPIC_PLACEHOLDER_JOIN%");
            SearchHelper.AddSharedSQL(portalID, userID, ref select, TitleField);
            //			if (topicName.Length != 0)
            //				select.Append(" AND (modSet.SettingName = 'TopicName' AND modSet.SettingValue='" + topicName + "')");
            select.Append("%TOPIC_PLACEHOLDER%");

            if (searchStr.Length != 0)
                select.Append(" AND " + SearchHelper.CreateTestSQL(ArrSearchFields, searchStr, true));

            if (ExtraSQL.Length != 0)
                select.Append(ExtraSQL);
            return select.ToString();
        }

        /* Jakob Hansen, 20 may: Before the RSS/Web Service community release

				/// <summary>
				/// Builds a SELECT query using given parameters
				/// </summary>
				/// <param name="portalID"></param>
				/// <param name="userID"></param>
				/// <param name="searchString"></param>
				/// <returns></returns>
				public string SearchSqlSelect(int portalID, int userID, string 
		searchString, bool hasItemID)
				{
					System.Text.StringBuilder select = new System.Text.StringBuilder();
					select.Append("SELECT ");
					select.Append("genModDef.FriendlyName AS ModuleName, ");
					select.Append("CAST (itm.");
					select.Append(TitleField);
					select.Append(" AS NVARCHAR(100)) AS Title, ");
					select.Append("CAST (itm.");
					select.Append(AbstractField);
					select.Append(" AS NVARCHAR(100)) AS Abstract, ");
					select.Append("itm.ModuleID AS ModuleID, ");

					if (hasItemID)
						select.Append("itm.ItemID AS ItemID, ");

					else
						select.Append("itm.ModuleID AS ItemID, ");

					if (!CreatedByUserField.StartsWith("'"))
						select.Append(strItm);   // Add itm only if not a constant value
					select.Append(CreatedByUserField);
					select.Append(" AS CreatedByUser, ");

					if (!CreatedDateField.StartsWith("'"))
						select.Append(strItm);   // Add itm only if not a constant value
					select.Append(CreatedDateField);
					select.Append(" AS CreatedDate, ");
					select.Append("mod.TabID AS TabID, ");
					select.Append("tab.TabName AS TabName, ");
					select.Append("genModDef.GeneralModDefID AS GeneralModDefID, ");
					select.Append("mod.ModuleTitle AS ModuleTitle ");
					select.Append("FROM ");
					select.Append(TableName);
					select.Append(" itm INNER JOIN ");
					select.Append("rb_Modules mod ON itm.ModuleID = mod.ModuleID INNER JOIN ");
					select.Append("rb_ModuleDefinitions modDef ON mod.ModuleDefID = modDef.ModuleDefID INNER JOIN ");
					select.Append("rb_Tabs tab ON mod.TabID = tab.TabID INNER JOIN ");
					select.Append("rb_GeneralModuleDefinitions genModDef ON modDef.GeneralModDefID = genModDef.GeneralModDefID ");
					Helpers.SearchHelper.AddSharedSQL(portalID, userID, ref select, TitleField);
					select.Append(" AND " + Appleseed.Framework.Helpers.SearchHelper.CreateTestSQL(ArrSearchFields, searchString, true));
					return select.ToString();
				}
		*/
    }
}