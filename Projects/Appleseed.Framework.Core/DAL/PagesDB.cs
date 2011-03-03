using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Appleseed.Framework.Data;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using System.Collections.Generic;

namespace Appleseed.Framework.Site.Data
{
    /// <summary>
    /// Class that encapsulates all data logic necessary to add/query/delete
    /// Portals within the Portal database.
    /// </summary>
    [History("jminond", "2005/03/10", "Tab to page conversion")]
    public class PagesDB
    {
        #region Declerations

        private const string strAllUsers = "All Users;";
        private const string strPortalID = "@PortalID";
        private const string strPageID = "@PageID";

        // New const for new method AddPage defaults
        // Mike Stone 30/12/2004
        private const int intParentPageID = 0; //SP will convert to NULL if 0
        private const bool boolShowMobile = false;
        private const string strMobilePageName = ""; // NULL NOT ALLOWED IN TABLE.

        #endregion

        #region Add Page

        /// <summary>
        /// The AddPage method adds a new tab to the portal.<br/>
        /// AddPage Stored Procedure
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageOrder">The page order.</param>
        /// <returns></returns>
        public int AddPage(int portalID, string pageName, int pageOrder)
        {
            return AddPage(portalID, pageName, strAllUsers, pageOrder);
        }

        /// <summary>
        /// The AddPage method adds a new tab to the portal.<br/>
        /// AddPage Stored Procedure
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="Roles">The roles.</param>
        /// <param name="pageOrder">The page order.</param>
        /// <returns></returns>
        public int AddPage(int portalID, string pageName, string Roles, int pageOrder)
        {
            // Change Method to use new all parms method below
            // SP call moved to new method AddPage below.
            // Mike Stone - 30/12/2004
            return
                AddPage(portalID, intParentPageID, pageName, pageOrder, strAllUsers, boolShowMobile, strMobilePageName);
        }

        /// <summary>
        /// The AddPage method adds a new tab to the portal.<br/>
        /// AddPage Stored Procedure
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="parentPageID">The parent page ID.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageOrder">The page order.</param>
        /// <param name="authorizedRoles">The authorized roles.</param>
        /// <param name="showMobile">if set to <c>true</c> [show mobile].</param>
        /// <param name="mobilePageName">Name of the mobile page.</param>
        /// <returns></returns>
        public int AddPage(int portalID, int parentPageID, string pageName, int pageOrder, string authorizedRoles,
                           bool showMobile, string mobilePageName)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_AddTab", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;


                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);

                    SqlParameter parameterParentPageID = new SqlParameter("@ParentPageID", SqlDbType.Int, 4);
                    parameterParentPageID.Value = parentPageID;
                    myCommand.Parameters.Add(parameterParentPageID);

                    //Fixes a missing tab name
                    if (pageName == null || pageName.Length == 0) pageName = "New Page";
                    SqlParameter parameterTabName = new SqlParameter("@PageName", SqlDbType.NVarChar, 50);

                    //Fixes tab name to long
                    if (pageName.Length > 50) parameterTabName.Value = pageName.Substring(0, 49);
                    else parameterTabName.Value = pageName.ToString();
                    myCommand.Parameters.Add(parameterTabName);

                    SqlParameter parameterTabOrder = new SqlParameter("@PageOrder", SqlDbType.Int, 4);
                    parameterTabOrder.Value = pageOrder;
                    myCommand.Parameters.Add(parameterTabOrder);

                    SqlParameter parameterAuthRoles = new SqlParameter("@AuthorizedRoles", SqlDbType.NVarChar, 256);
                    parameterAuthRoles.Value = authorizedRoles;
                    myCommand.Parameters.Add(parameterAuthRoles);

                    SqlParameter parameterShowMobile = new SqlParameter("@ShowMobile", SqlDbType.Bit, 1);
                    parameterShowMobile.Value = showMobile;
                    myCommand.Parameters.Add(parameterShowMobile);

                    SqlParameter parameterMobileTabName = new SqlParameter("@MobilePageName", SqlDbType.NVarChar, 50);
                    parameterMobileTabName.Value = mobilePageName;
                    myCommand.Parameters.Add(parameterMobileTabName);

                    SqlParameter parameterPageID = new SqlParameter(strPageID, SqlDbType.Int, 4);
                    parameterPageID.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterPageID);

                    myConnection.Open();

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }

                    finally
                    {
                        myConnection.Close();
                    }
                    return (int) parameterPageID.Value;
                }
            }
        }

        #endregion

        #region Delete Page

        /// <summary>
        /// The DeletePage method deletes the selected tab from the portal.<br/>
        /// DeletePage Stored Procedure
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        public void DeletePage(int pageID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_DeleteTab", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPageID = new SqlParameter(strPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    // Open the database connection and execute the command
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

        #endregion

        #region Update Pages

        /// <summary>
        /// UpdatePage Method<br/>
        /// UpdatePage Stored Procedure
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <param name="parentPageID">The parent page ID.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="pageOrder">The page order.</param>
        /// <param name="authorizedRoles">The authorized roles.</param>
        /// <param name="mobilePageName">Name of the mobile page.</param>
        /// <param name="showMobile">if set to <c>true</c> [show mobile].</param>
        public void UpdatePage(int portalID, int pageID, int parentPageID, string pageName, int pageOrder,
                               string authorizedRoles, string mobilePageName, bool showMobile)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_UpdateTab", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);
                    SqlParameter parameterPageID = new SqlParameter(strPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    SqlParameter parameterParentPageID = new SqlParameter("@ParentPageID", SqlDbType.Int, 4);
                    parameterParentPageID.Value = parentPageID;
                    myCommand.Parameters.Add(parameterParentPageID);

                    //Fixes a missing tab name
                    if (pageName == null || pageName.Length == 0) pageName = "&nbsp;";
                    SqlParameter parameterTabName = new SqlParameter("@PageName", SqlDbType.NVarChar, 50);

                    if (pageName.Length > 50) parameterTabName.Value = pageName.Substring(0, 49);

                    else parameterTabName.Value = pageName.ToString();
                    myCommand.Parameters.Add(parameterTabName);
                    SqlParameter parameterTabOrder = new SqlParameter("@PageOrder", SqlDbType.Int, 4);
                    parameterTabOrder.Value = pageOrder;
                    myCommand.Parameters.Add(parameterTabOrder);
                    SqlParameter parameterAuthRoles = new SqlParameter("@AuthorizedRoles", SqlDbType.NVarChar, 256);
                    parameterAuthRoles.Value = authorizedRoles;
                    myCommand.Parameters.Add(parameterAuthRoles);
                    SqlParameter parameterMobileTabName = new SqlParameter("@MobilePageName", SqlDbType.NVarChar, 50);
                    parameterMobileTabName.Value = mobilePageName;
                    myCommand.Parameters.Add(parameterMobileTabName);
                    SqlParameter parameterShowMobile = new SqlParameter("@ShowMobile", SqlDbType.Bit, 1);
                    parameterShowMobile.Value = showMobile;
                    myCommand.Parameters.Add(parameterShowMobile);
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
        /// Update Page Custom Settings
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        [Obsolete("UpdatePageCustomSettings was moved in PageSettings.UpdatePageSetting", false)]
        public void UpdatePageCustomSettings(int pageID, string key, string value)
        {
            PageSettings.UpdatePageSettings(pageID, key, value);
        }

        /// <summary>
        /// The UpdatePageOrder method changes the position of the tab with respect
        /// to other tabs in the portal.<br/>
        /// UpdatePageOrder Stored Procedure
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="pageOrder">The page order.</param>
        public void UpdatePageOrder(int pageID, int pageOrder)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_UpdateTabOrder", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPageID = new SqlParameter(strPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    SqlParameter parameterTabOrder = new SqlParameter("@PageOrder", SqlDbType.Int, 4);
                    parameterTabOrder.Value = pageOrder;
                    myCommand.Parameters.Add(parameterTabOrder);
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
        /// The UpdatePageOrder method changes the position of the tab with respect
        /// to other tabs in the portal.<br/>
        /// UpdatePageOrder Stored Procedure
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="parentPageID">The parent page ID.</param>
        /// <param name="portalID">The portal ID.</param>
        public void UpdatePageParent(int pageID, int parentPageID, int portalID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_UpdateTabParent", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);
                    SqlParameter parameterPageID = new SqlParameter(strPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    SqlParameter parameterParentPageID = new SqlParameter("@ParentPageID", SqlDbType.Int, 4);
                    parameterParentPageID.Value = parentPageID;
                    myCommand.Parameters.Add(parameterParentPageID);

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

        #endregion

        /// <summary>
        /// Return the portal home page in case you are on pageid = 0
        /// </summary>
        /// <param name="portalID"></param>
        /// <returns></returns>
        public static int PortalHomePageID(int portalID)
        {
            // TODO: COnvert to stored procedure?
            // TODO: Appleseed.Framwork.Application.Site.Pages Api 
            int ret = 0;

            string sql = "Select PageID  From rb_Pages  Where " +
                         "(PortalID = " + portalID.ToString() + ") and " +
                         "(ParentPageID is null) and  " +
                         "(PageID > 0) and ( " +
                         "PageOrder < 2)";

            ret = Convert.ToInt32(DBHelper.ExecuteSQLScalar(sql));

            return ret;
        }

        /// <summary>
        /// This user control will render the breadcrumb navigation for the current tab.
        /// Ver. 1.0 - 24. dec 2002 - First realase by Cory Isakson
        /// </summary>
        /// <param name="pageID">ID of the page</param>
        /// <returns></returns>
        public ArrayList GetPageCrumbs(int pageID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetTabCrumbs", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPageID = new SqlParameter(strPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    SqlParameter parameterCrumbs = new SqlParameter("@CrumbsXML", SqlDbType.NVarChar, 4000);
                    parameterCrumbs.Direction = ParameterDirection.Output;
                    myCommand.Parameters.Add(parameterCrumbs);
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
                    // Build a Hashtable from the XML string returned
                    ArrayList Crumbs = new ArrayList();
                    XmlDocument CrumbXML = new XmlDocument();
                    CrumbXML.LoadXml(parameterCrumbs.Value.ToString().Replace("&", "&amp;"));

                    //Iterate through the Crumbs XML
                    foreach (XmlNode node in CrumbXML.FirstChild.ChildNodes)
                    {
                        PageItem tab = new PageItem();
                        tab.ID = Int16.Parse(node.Attributes.GetNamedItem("tabID").Value);
                        tab.Name = node.InnerText;
                        tab.Order = Int16.Parse(node.Attributes.GetNamedItem("level").Value);
                        Crumbs.Add(tab);
                    }
                    //Return the Crumb Page Items as an arraylist 
                    return Crumbs;
                }
            }
        }

        /// <summary>
        /// Gets the pages by portal.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns>
        /// A System.Data.SqlClient.SqlDataReader value...
        /// </returns>
        // TODO --> [Obsolete("Replace me")]
        public SqlDataReader GetPagesByPortal(int portalID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetTabsByPortal", myConnection);
            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;
            // Add Parameters to SPROC
            SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
            parameterPortalID.Value = portalID;
            myCommand.Parameters.Add(parameterPortalID);
            // Execute the command
            myConnection.Open();
            SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            // Return the datareader 
            return result;
        }

        /// <summary>
        /// Gets the pages flat table.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        public DataTable GetPagesFlatTable(int portalID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                string strSQL = "rb_GetPageTree " + portalID.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, myConnection);

                DataTable dt_tbl = new DataTable("Pages");
                // Read the resultset
                try
                {
                    da.Fill(dt_tbl);
                }

                finally
                {
                    da.Dispose();
                }

                return dt_tbl;
            }
        }

        /// <summary>
        /// Get a pages parentID
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public int GetPageParentID(int portalID, int pageID)
        {
            string strSQL = "rb_GetPagesParentTabID " + portalID.ToString() + ", " + pageID.ToString();
            int parentID = 0;
            // Read the resultset
            parentID = Convert.ToInt32(DBHelper.ExecuteSQLScalar(strSQL));
            return parentID;
        }

        /// <summary>
        /// Get a pages tab order
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public int GetPageTabOrder(int portalID, int pageID)
        {
            string strSQL = "select PageOrder from rb_Pages Where (PortalID = " + portalID.ToString() + ") AND (PageID = " +
                            pageID.ToString() + ")";
            int t_order = 99990;
            // Read the resultset
            t_order = Convert.ToInt32(DBHelper.ExecuteSQLScalar(strSQL));
            return t_order;
        }

        /// <summary>
        /// Gets the pages flat.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns></returns>
        public ArrayList GetPagesFlat(int portalID)
        {
            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetTabsFlat", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);
                    // Execute the command
                    myConnection.Open();
                    SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    ArrayList DesktopPages = new ArrayList();

                    // Read the resultset
                    try
                    {
                        while (result.Read())
                        {
                            PageItem tabItem = new PageItem();
                            tabItem.ID = (int) result["PageID"];
                            tabItem.Name = (string) result["PageName"];
                            tabItem.Order = (int) result["PageOrder"];
                            tabItem.NestLevel = (int) result["NestLevel"];
                            DesktopPages.Add(tabItem);
                        }
                    }

                    finally
                    {
                        result.Close(); //by Manu, fixed bug 807858
                    }
                    return DesktopPages;
                }
            }
        }

        /// <summary>
        /// Gets the pagesin page.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <returns>A System.Collections.ArrayList value...</returns>
        public ArrayList GetPagesinPage(int portalID, int pageID)
        {
            ArrayList DesktopPages = new ArrayList();

            // Create Instance of Connection and Command Object
            using (SqlConnection myConnection = Config.SqlConnectionString)
            {
                using (SqlCommand myCommand = new SqlCommand("rb_GetTabsinTab", myConnection))
                {
                    // Mark the Command as a SPROC
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Add Parameters to SPROC
                    SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
                    parameterPortalID.Value = portalID;
                    myCommand.Parameters.Add(parameterPortalID);
                    SqlParameter parameterPageID = new SqlParameter(strPageID, SqlDbType.Int, 4);
                    parameterPageID.Value = pageID;
                    myCommand.Parameters.Add(parameterPageID);
                    // Execute the command
                    myConnection.Open();
                    SqlDataReader result = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

                    // Read the resultset
                    try
                    {
                        while (result.Read())
                        {
                            PageStripDetails tabDetails = new PageStripDetails();
                            tabDetails.PageID = (int) result["PageID"];
                            tabDetails.ParentPageID = Int32.Parse("0" + result["ParentPageID"]);
                            tabDetails.PageName = (string) result["PageName"];
                            tabDetails.PageOrder = (int) result["PageOrder"];
                            tabDetails.AuthorizedRoles = (string) result["AuthorizedRoles"];
                            // Update the AuthorizedRoles Variable
                            DesktopPages.Add(tabDetails);
                        }
                    }

                    finally
                    {
                        result.Close(); //by Manu, fixed bug 807858
                    }
                }
            }
            return DesktopPages;
        }

        /// <summary>
        /// Gets the pages parent.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="pageID">The page ID.</param>
        /// <returns>
        /// A System.Data.SqlClient.SqlDataReader value...
        /// </returns>
        public IList<PageItem> GetPagesParent(int portalID, int pageID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetTabsParent", myConnection);
            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;
            // Add Parameters to SPROC
            SqlParameter parameterPortalID = new SqlParameter(strPortalID, SqlDbType.Int, 4);
            parameterPortalID.Value = portalID;
            myCommand.Parameters.Add(parameterPortalID);
            SqlParameter parameterPageID = new SqlParameter(strPageID, SqlDbType.Int, 4);
            parameterPageID.Value = pageID;
            myCommand.Parameters.Add(parameterPageID);
            // Execute the command
            myConnection.Open();
            SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            IList<PageItem> result = new List<PageItem>();

            while ( dr.Read() ) {
                PageItem item = new PageItem();
                item.ID = Convert.ToInt32( dr[ "PageID" ] );
                item.Name = ( string )dr[ "PageName" ];
                result.Add( item );
            }

            return result;
        }
    }
}