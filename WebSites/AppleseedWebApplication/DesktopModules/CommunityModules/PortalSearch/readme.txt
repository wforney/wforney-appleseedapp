Portal Search - Portal Search module
Credits: Me! (Jakob Hansen, hansen3000@hotmail.com)


Another Appleseed desktop module - more to download on http://www.Appleseedportal.net


INSTALL
1. Install using strandard installer
2. If you have the FAQs module installed: Set collation to "Latin1_General_CI_AS"
   on fields Question and "Answer" in table "FAQ".
3. Log on as Admin and add the "Portal Search" module to a tabpage 
4. Edit module settings: Customize search for your needs!
5. Use it! ;o)


HISTORY
Ver. 2.0 - 19. feb 2003 - Created by Jakob Hansen
Ver. 2.1 - 08. okt 2003 - moved to seperate folder by mario@hartmann.net
Ver. 2.2 - 14. jan 2004 - More localizing...
Ver. 3.0 - 29. jan 2004 - by Manu (some code by Jakob) - Added topic functionality

Issues and Known problems:
- Tested with Appleseed version 1.3.0.1756a - 26/12/2003
- In the top of file DBPatch.sql is list of DB changes this patch introduces

Search tips:
- Use AND and NOT
- It is not necessary to type AND between words
- Use '-' in front of words. Example:
  "word1 word2 -word3" and "word1 word2 NOT word3" returns same result
- Use ? as singlecharacter wildcart
- OR is not supported (it is ignored)
- Phrase searching supported using quotes: "this is a phrase"

Topic functionality:
A new topic property is availabe for all modules that allow search, you have it automatically.
Simply add a keyword if you want to tag it.
NOTE: Only single topic is supported right now.
Sample:
MyTopic
MynewTopic
MyTopic MynewTopic

are three different topics.

Searching: you find a new dropdown named topic. 
All topcis found in current portal will be added automatically.


Developer's note:

NOTE: Your module MUST have a ModuleID field in its own table or search will fail.

Implementing search on your modules
1. remove abstract from searchable modules
   You need it: we use reflection to call the search code 
   and we cannot instantiate an abstract class!
   
2. Override the search method

Sample from contacts:

		/// <summary>
		/// Searchable module implementation
		/// </summary>
		/// <param name="portalID"></param>
		/// <param name="userID"></param>
		/// <param name="searchString"></param>
		/// <param name="searchField"></param>
		/// <param name="select"></param>
		/// <returns></returns>
		public override System.Text.StringBuilder SearchSqlSelect(int portalID, int userID, string searchString, string searchField, System.Text.StringBuilder select)
		{
			// Parameters:
			// Table Name: the table that holds the data
			// Title field: the field that contains the title for result, must be a field in the table
			// Abstract field: the field that contains the text for result, must be a field in the table
			// Search field: pass the searchField parameter you recieve.
			Appleseed.Framework.Helpers.SearchDefinition s = new Appleseed.Framework.Helpers.SearchDefinition("rb_Contacts", "Role", "Name", "CreatedByUser", "CreatedDate", searchField);
			
			//Add extra search fields here, this way
			s.ArrSearchFields.Add("itm.Email");
			s.ArrSearchFields.Add("itm.Contact1");
			s.ArrSearchFields.Add("itm.Contact2");
			
			// Builds and returns the SELECT query
			return s.SearchSqlSelect(portalID, userID, searchString, select);
		}

NOTE: You cannot search ntext fields.

You will get this error:
Exception: System.Data.SqlClient.SqlException
Message: Operand type clash: ntext is incompatible with xxxfieldname
Source: .Net SqlClient Data Provider



3. Override Searchable property

		/// <summary>
		/// Searchable module
		/// </summary>
		public override bool Searchable
		{
			get
			{
				return true;
			}
		}

4. Go to Register Module and register your module.
