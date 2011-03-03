using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Appleseed.Framework.Data;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Services.Client;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Web.UI;
using Path=Appleseed.Framework.Settings.Path;

namespace Appleseed.Framework.Services
{
    /// <summary>
    /// This class contains static methods for working with both community 
    /// and RSS services
    /// </summary>
    public class ServiceHelper
    {
        /// <summary>
        /// This function return's true if no problem reading ServiceRequestInfo parameters
        /// </summary>
        /// <param name="Params">The page Request.Params</param>
        /// <param name="parameterError">out: all param that were bad</param>
        /// <param name="requestInfo">out: a filled ServiceRequestInfo class</param>
        /// <returns>bool</returns>
        public static bool FillRSSServiceRequestInfo(NameValueCollection Params, ref string parameterError,
                                                     ref ServiceRequestInfo requestInfo)
        {
            bool retValue = true;
            string par;

            parameterError = string.Empty;

            requestInfo.Type = ServiceType.CommunityRSSService;
            requestInfo.LocalMode = true;

            if (Params["PortalAlias"] != null)
                requestInfo.PortalAlias = Params["PortalAlias"];
            if (Params["PA"] != null)
                requestInfo.PortalAlias = Params["PA"];

            if (Params["UserName"] != null)
                requestInfo.UserName = Params["UserName"];
            if (Params["UN"] != null)
                requestInfo.UserName = Params["UN"];
            if (Params["UserPassword"] != null)
                requestInfo.UserPassword = Params["UserPassword"];
            if (Params["UP"] != null)
                requestInfo.UserPassword = Params["UP"];

            par = string.Empty;
            if (Params["ListType"] != null)
                par = Params["ListType"].ToUpper();
            if (Params["LT"] != null)
                par = Params["LT"].ToUpper();
            if (par.Length != 0)
            {
                if (!(par == "ITEM" || par == "MODULE" || par == "TAB"))
                {
                    retValue = false;
                    if (parameterError.Length != 0) parameterError += ", ";
                    parameterError += "ListType";
                }
                else
                {
                    if (par == ServiceListType.Item.ToString().ToUpper())
                        requestInfo.ListType = ServiceListType.Item;
                    if (par == ServiceListType.Module.ToString().ToUpper())
                        requestInfo.ListType = ServiceListType.Module;
                    if (par == ServiceListType.Tab.ToString().ToUpper())
                        requestInfo.ListType = ServiceListType.Tab;
                }
            }

            par = string.Empty;
            if (Params["ModuleType"] != null)
                par = Params["ModuleType"];
            if (Params["MT"] != null)
                par = Params["MT"];
            if (par.Length != 0)
            {
                if (!(par == "All" ||
                      par == "Announcements" ||
                      par == "Contacts" ||
                      par == "Discussion" ||
                      par == "Events" ||
                      par == "HtmlModule" ||
                      par == "Documents" ||
                      par == "Pictures" ||
                      par == "Articles" ||
                      par == "Tasks" ||
                      par == "FAQs" ||
                      par == "ComponentModule"))
                {
                    retValue = false;
                    if (parameterError.Length != 0) parameterError += ", ";
                    parameterError += "ModuleType";
                }
                else
                    requestInfo.ModuleType = par;
            }

            par = string.Empty;
            if (Params["MaxHits"] != null)
                par = Params["MaxHits"];
            if (Params["MH"] != null)
                par = Params["MH"];
            if (par.Length != 0)
            {
                try
                {
                    requestInfo.MaxHits = int.Parse(par);
                }
                catch
                {
                    retValue = false;
                    if (parameterError.Length != 0) parameterError += ", ";
                    parameterError += "MaxHits";
                }
            }

            par = string.Empty;
            if (Params["ShowID"] != null)
                par = Params["ShowID"];
            if (Params["SID"] != null)
                par = Params["SID"];
            if (par.Length != 0)
            {
                try
                {
                    if (par == "0" || par == "1")
                        requestInfo.ShowID = (par == "1");
                    else
                        requestInfo.ShowID = bool.Parse(par);
                }
                catch
                {
                    retValue = false;
                    if (parameterError.Length != 0) parameterError += ", ";
                    parameterError += "ShowID";
                }
            }

            if (Params["SearchString"] != null)
                requestInfo.SearchString = Params["SearchString"];

            if (Params["SS"] != null)
                requestInfo.SearchString = Params["SS"];

            if (Params["SearchField"] != null)
                requestInfo.SearchField = Params["SearchField"];
            if (Params["SF"] != null)
                requestInfo.SearchField = Params["SF"];

            par = string.Empty;
            if (Params["SortField"] != null)
                par = Params["SortField"].ToUpper();
            if (Params["SoF"] != null)
                par = Params["SoF"].ToUpper();
            if (par.Length != 0)
            {
                switch (requestInfo.ListType)
                {
                    case ServiceListType.Item:
                        if (!(par == "MODULENAME" ||
                              par == "TITLE" ||
                              par == "CREATEDBYUSER" ||
                              par == "CREATEDDATE" ||
                              par == "PAGENAME"))
                        {
                            retValue = false;
                            if (parameterError.Length != 0) parameterError += ", ";
                            parameterError += "SortField";
                        }
                        else
                            requestInfo.SortField = par;
                        break;

                    case ServiceListType.Module:
                        if (!(par == "FRIENDLYNAME" ||
                              par == "TITLE"))
                        {
                            retValue = false;
                            if (parameterError.Length != 0) parameterError += ", ";
                            parameterError += "SortField";
                        }
                        else
                            requestInfo.SortField = par;
                        break;
                }
            }

            par = string.Empty;
            if (Params["SortDirection"] != null)
                par = Params["SortDirection"].ToUpper();
            if (Params["SD"] != null)
                par = Params["SD"].ToUpper();
            if (par.Length != 0)
            {
                if (!(par == "ASC" || par == "DESC"))
                {
                    retValue = false;
                    if (parameterError.Length != 0) parameterError += ", ";
                    parameterError += "SortDirection";
                }
                else
                    requestInfo.SortDirection = par;
            }

            par = string.Empty;
            if (Params["RootLevelOnly"] != null)
                par = Params["RootLevelOnly"];
            if (Params["RLO"] != null)
                par = Params["RLO"];
            if (par.Length != 0)
            {
                try
                {
                    if (par == "0" || par == "1")
                        requestInfo.RootLevelOnly = (par == "1");
                    else
                        requestInfo.RootLevelOnly = bool.Parse(par);
                }
                catch
                {
                    retValue = false;
                    if (parameterError.Length != 0) parameterError += ", ";
                    parameterError += "RootLevelOnly";
                }
            }

            par = string.Empty;
            if (Params["MobileOnly"] != null)
                par = Params["MobileOnly"];
            if (Params["MO"] != null)
                par = Params["MO"];
            if (par.Length != 0)
            {
                try
                {
                    if (par == "0" || par == "1")
                        requestInfo.MobileOnly = (par == "1");
                    else
                        requestInfo.MobileOnly = bool.Parse(par);
                }
                catch
                {
                    retValue = false;
                    if (parameterError.Length != 0) parameterError += ", ";
                    parameterError += "MobileOnly";
                }
            }

            if (Params["IDList"] != null)
                requestInfo.IDList = Params["IDList"];
            if (Params["IDL"] != null)
                requestInfo.IDList = Params["IDL"];

            par = string.Empty;
            if (Params["IDListType"] != null)
                par = Params["IDListType"].ToUpper();
            if (Params["IDLT"] != null)
                par = Params["IDLT"].ToUpper();
            if (par.Length != 0)
            {
                if (!(par == "ITEM" || par == "MODULE" || par == "TAB"))
                {
                    retValue = false;
                    if (parameterError.Length != 0) parameterError += ", ";
                    parameterError += "ListType";
                }
                else
                {
                    if (par == ServiceListType.Item.ToString().ToUpper())
                        requestInfo.IDListType = ServiceListType.Item;
                    if (par == ServiceListType.Module.ToString().ToUpper())
                        requestInfo.IDListType = ServiceListType.Module;
                    if (par == ServiceListType.Tab.ToString().ToUpper())
                        requestInfo.IDListType = ServiceListType.Tab;
                }
            }

            par = string.Empty;
            if (Params["Tag"] != null)
                par = Params["Tag"];
            if (Params["T"] != null)
                par = Params["T"];
            if (par.Length != 0)
            {
                try
                {
                    requestInfo.Tag = int.Parse(par);
                }
                catch
                {
                    retValue = false;
                    if (parameterError.Length != 0) parameterError += ", ";
                    parameterError += "Tag";
                }
            }

            return retValue;
        }

        /// <summary>
        /// Adds the RSS request parameters.
        /// </summary>
        /// <param name="sri">The sri.</param>
        /// <returns>A string value...</returns>
        public static string AddRSSRequestParameters(ServiceRequestInfo sri)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("?LT=" + sri.ListType.ToString());
            if (sri.PortalAlias.Length != 0)
                sb.Append("&PA=" + sri.PortalAlias);
            if (sri.UserName.Length != 0)
                sb.Append("&UN=" + sri.UserName);
            if (sri.UserPassword.Length != 0)
                sb.Append("&UP=" + sri.UserPassword);
            if (sri.ModuleType.Length != 0)
                sb.Append("&MT=" + sri.ModuleType);
            sb.Append("&MH=" + sri.MaxHits.ToString());
            sb.Append("&SID=" + sri.ShowID.ToString());
            if (sri.SearchString.Length != 0)
                sb.Append("&SS=" + sri.SearchString);
            if (sri.SearchField.Length != 0)
                sb.Append("&SF=" + sri.SearchField);
            if (sri.SortField.Length != 0)
                sb.Append("&SoF=" + sri.SortField);
            if (sri.SortDirection.Length != 0)
                sb.Append("&SD=" + sri.SortDirection);
            sb.Append("&RLO=" + sri.RootLevelOnly.ToString());
            sb.Append("&MO=" + sri.MobileOnly.ToString());
            if (sri.IDList.Length != 0)
                sb.Append("&IDL=" + sri.IDList.ToString());
            sb.Append("&IDLT=" + sri.IDListType.ToString());

            return sb.ToString();
        }


        /// <summary>
        /// Gets a list of tabs as described in requestInfo
        /// </summary>
        /// <param name="portalID">Only tabs for portalID is returned</param>
        /// <param name="userID">User ID. If -1 the user is Annonymous/Guest</param>
        /// <param name="applicationFullPath">Full URL path to Appleseed site</param>
        /// <param name="requestInfo">a filled ServiceRequestInfo class</param>
        /// <returns>ArrayList</returns>
        public static ArrayList GetResponseTabList(int portalID, Guid userID, string applicationFullPath,
                                                   ref ServiceRequestInfo requestInfo)
        {
            ArrayList colItems = new ArrayList();
            ServiceResponseInfoItem responseItem = null;

            StringBuilder select;
            select = new StringBuilder(string.Empty, 1000);

            select.Append(" SELECT");
            if (requestInfo.MaxHits > 0)
                select.Append(" TOP " + requestInfo.MaxHits);
            select.Append(" Tab.*");
            select.Append(" ,ISNULL(ParTab.PageName,'') AS ParentTabName");
            select.Append(" ,ParTab.PageID AS ParentPageID");
            select.Append(" ,Mod.ModuleID AS ModuleID");
            select.Append(" ,Mod.ModuleTitle AS ModuleTitle");
            select.Append(
                " FROM rb_Pages Tab, rb_Pages ParTab, rb_Modules Mod, rb_ModuleDefinitions ModDef, rb_GeneralModuleDefinitions GenModDef");
            select.Append(" WHERE");
            select.Append(" Tab.AuthorizedRoles like '%All Users%'");
            select.Append(" AND Tab.ParentPageID *= ParTab.PageID");
            // Jakob: I just luv this outer join! (It's so easy!)
            select.Append(" AND Tab.PageID = Mod.TabID");
            select.Append(" AND Mod.ModuleDefID = ModDef.ModuleDefID");
            select.Append(" AND ModDef.GeneralModDefID = GenModDef.GeneralModDefID");
            select.Append(" AND Mod.AuthorizedViewRoles like '%All Users%'");
            select.Append(" AND Tab.PortalID = " + portalID);
            if (requestInfo.IDList.Length != 0)
            {
                if (requestInfo.IDList.Length != 0)
                    select.Append(" AND Tab.PageID IN (" + requestInfo.IDList + ")");
                if (requestInfo.IDListType == ServiceListType.Module)
                    select.Append(" AND Mod.ModuleID IN (" + requestInfo.IDList + ")");
            }
            if (requestInfo.ModuleType != "All")
                select.Append(" AND GenModDef.ClassName = 'Appleseed.DesktopModules." + requestInfo.ModuleType + "'");
            if (requestInfo.RootLevelOnly)
                select.Append(" AND Tab.ParentPageID IS NULL");
            if (requestInfo.MobileOnly)
                select.Append(" AND Tab.ShowMobile=1");
            if (requestInfo.SearchString.Length != 0)
                select.Append(" AND Tab.PageName like '%" + requestInfo.SearchString + "%'");
            select.Append(" ORDER BY Tab.PageOrder " + requestInfo.SortDirection +
                          ", Tab.ParentPageID, Mod.ModuleOrder ");

            int PageID = -1;
            int PageIDPrev = -1;
            int ModuleID;

            SqlDataReader dr = DBHelper.GetDataReader(select.ToString());
            try
            {
                while (dr.Read())
                {
                    PageID = (int) dr["PageID"];
                    ModuleID = (int) dr["ModuleID"];

                    if (PageID != PageIDPrev)
                    {
                        if (PageIDPrev != -1)
                            colItems.Add(responseItem);

                        responseItem = new ServiceResponseInfoItem();

                        if (requestInfo.ShowID)
                            responseItem.Title = (string) dr["PageName"] + " (PageID=" + PageID + ")";
                        else
                            responseItem.Title = (string) dr["PageName"];

                        if (requestInfo.ShowID)
                            responseItem.Description = (string) dr["ModuleTitle"] + " (ModuleID=" + ModuleID + ")";
                        else
                            responseItem.Description = (string) dr["ModuleTitle"];

                        responseItem.Link = applicationFullPath + "/DesktopDefault.aspx?tabID=" + PageID.ToString();
                        if (requestInfo.PortalAlias.Length != 0)
                            responseItem.Link += "&Alias=" + requestInfo.PortalAlias;
                    }
                    else
                    {
                        if (requestInfo.ShowID)
                            responseItem.Description += ", " + (string) dr["ModuleTitle"] + " (ModuleID=" + ModuleID +
                                                        ")";
                        else
                            responseItem.Description += ", " + (string) dr["ModuleTitle"];
                    }
                    PageIDPrev = PageID;
                }
            }
            finally
            {
                dr.Close(); //by Manu, fixed bug 807858
            }

            if (PageIDPrev != -1)
                colItems.Add(responseItem);

            return colItems;
        }


        /// <summary>
        /// Gets a list of modules as described in requestInfo
        /// </summary>
        /// <param name="portalID">Only modules for portalID is returned</param>
        /// <param name="userID">User ID. If -1 the user is Annonymous/Guest</param>
        /// <param name="applicationFullPath">Full URL path to Appleseed site</param>
        /// <param name="requestInfo">a filled ServiceRequestInfo class</param>
        /// <returns>ArrayList</returns>
        public static ArrayList GetResponseModuleList(int portalID, Guid userID, string applicationFullPath,
                                                      ref ServiceRequestInfo requestInfo)
        {
            ArrayList colItems = new ArrayList();
            ServiceResponseInfoItem responseItem = null;

            StringBuilder select;
            select = new StringBuilder(string.Empty, 1000);

            select.Append(" SELECT");
            if (requestInfo.MaxHits > 0)
                select.Append(" TOP " + requestInfo.MaxHits);
            select.Append(" Mod.ModuleID, Mod.ModuleTitle, Mod.ModuleDefID, Mod.ShowMobile, Mod.WorkflowState");
            select.Append(" ,Tab.PageID AS PageID");
            select.Append(" ,Tab.PageName AS TabName");
            select.Append(" ,Tab.ParentPageID AS ParentPageID");
            select.Append(" ,GenModDef.GeneralModDefID AS GeneralModDefID");
            select.Append(" ,GenModDef.FriendlyName AS FriendlyName");
            select.Append(
                " FROM rb_Modules Mod, rb_Pages Tab, rb_ModuleDefinitions ModDef, rb_GeneralModuleDefinitions GenModDef");
            select.Append(" WHERE");
            select.Append(" Tab.AuthorizedRoles like '%All Users%'");
            select.Append(" AND Tab.PageID = Mod.PageID");
            select.Append(" AND Mod.ModuleDefID = ModDef.ModuleDefID");
            select.Append(" AND ModDef.GeneralModDefID = GenModDef.GeneralModDefID");
            select.Append(" AND Mod.AuthorizedViewRoles like '%All Users%'");
            select.Append(" AND ModDef.PortalID = Tab.PortalID");
            select.Append(" AND Tab.PortalID = " + portalID);
            if (requestInfo.IDList.Length != 0)
            {
                if (requestInfo.IDListType == ServiceListType.Tab)
                    select.Append(" AND Tab.PageID IN (" + requestInfo.IDList + ")");
                if (requestInfo.IDListType == ServiceListType.Module)
                    select.Append(" AND Mod.ModuleID IN (" + requestInfo.IDList + ")");
            }
            if (requestInfo.ModuleType != "All")
                select.Append(" AND GenModDef.ClassName = 'Appleseed.DesktopModules." + requestInfo.ModuleType + "'");
            if (requestInfo.RootLevelOnly)
                select.Append(" AND Tab.ParentPageID IS NULL");
            if (requestInfo.MobileOnly)
                select.Append(" AND Mod.ShowMobile=1");
            if (requestInfo.SearchString.Length != 0)
                select.Append(" AND Mod.ModuleTitle like '%" + requestInfo.SearchString + "%'");

            string sortField = "GenModDef.FriendlyName";
            if (requestInfo.SortField == "TITLE")
                sortField = "Mod.ModuleTitle";
            select.Append(" ORDER BY " + sortField);
            select.Append(" ");
            select.Append(requestInfo.SortDirection);

            int PageID, ModuleID;

            SqlDataReader dr = DBHelper.GetDataReader(select.ToString());
            try
            {
                while (dr.Read())
                {
                    responseItem = new ServiceResponseInfoItem();
                    PageID = (int) dr["PageID"];
                    ModuleID = (int) dr["ModuleID"];

                    if (requestInfo.ShowID)
                    {
                        responseItem.Title = (string) dr["ModuleTitle"] + " (ModuleID=" + ModuleID + ")";
                        responseItem.Description = "Type: " + (string) dr["FriendlyName"] + ", Page: " +
                                                   (string) dr["PageName"] + " (PageID=" + PageID + ")";
                    }
                    else
                    {
                        responseItem.Title = (string) dr["ModuleTitle"] + " (ModuleID=" + ModuleID + ")";
                        responseItem.Description = "Type: " + (string) dr["FriendlyName"] + ", Page: " +
                                                   (string) dr["PageName"];
                    }
                    responseItem.Link = applicationFullPath + "/DesktopDefault.aspx?tabID=" + PageID.ToString();
                    if (requestInfo.PortalAlias.Length != 0)
                        responseItem.Link += "&Alias=" + requestInfo.PortalAlias;

                    colItems.Add(responseItem);
                }
            }
            finally
            {
                dr.Close(); //by Manu, fixed bug 807858
            }

            return colItems;
        }


        /// <summary>
        /// Gets a list of items as described in requestInfo
        /// </summary>
        /// <param name="portalID">Only items for portalID is returned</param>
        /// <param name="userID">User ID. If -1 the user is Annonymous/Guest</param>
        /// <param name="applicationFullPath">Full URL path to Appleseed site</param>
        /// <param name="requestInfo">a filled ServiceRequestInfo class</param>
        /// <returns>ArrayList</returns>
        public static ArrayList GetResponseItemList(int portalID, Guid userID, string applicationFullPath,
                                                    ref ServiceRequestInfo requestInfo)
        {
            ArrayList colItems = new ArrayList();
            ServiceResponseInfoItem responseItem;

            string moduleType = requestInfo.ModuleType;
            // JLH!! We really should lookup compleate ModuleName in db...
            if (moduleType.ToUpper() == "ALL")
                moduleType = string.Empty;
            else
                moduleType = "Appleseed.DLL;Appleseed.DesktopModules." + moduleType;

            StringBuilder select;
            select = new StringBuilder(string.Empty, 1000);
            if (requestInfo.IDList.Length != 0)
            {
                if (requestInfo.IDListType == ServiceListType.Tab)
                    select.Append(" AND tab.PageID IN (" + requestInfo.IDList + ")");
                if (requestInfo.IDListType == ServiceListType.Module)
                    select.Append(" AND mod.ModuleID IN (" + requestInfo.IDList + ")");
            }
            // We can not add this code - original search sql not ready for this!
            //if (requestInfo.RootLevelOnly)  
            //	select.Append(" AND tab.ParentPageID IS NULL");
            if (requestInfo.MobileOnly)
                select.Append(" AND mod.ShowMobile=1");

            SqlDataReader dr;
            dr =
                SearchHelper.SearchPortal(portalID, userID, moduleType, requestInfo.SearchString,
                                          requestInfo.SearchField, requestInfo.SortField, requestInfo.SortDirection,
                                          string.Empty, select.ToString());

            int hits = 1;
            try
            {
                while (dr.Read() && hits <= requestInfo.MaxHits)
                {
                    responseItem = new ServiceResponseInfoItem();

                    if (!dr.IsDBNull(0))
                        responseItem.FriendlyName = dr.GetString(0);
                    if (!dr.IsDBNull(1))
                        responseItem.Title = dr.GetString(1);
                    if (!dr.IsDBNull(2))
                    {
                        HTMLText html = SearchHelper.DeleteBeforeBody(HttpUtility.HtmlDecode(dr.GetString(2)));
                        responseItem.Description = html.InnerText;
                    }
                    if (!dr.IsDBNull(3))
                        responseItem.ModuleID = dr.GetInt32(3);
                    if (!dr.IsDBNull(4))
                    {
                        responseItem.ItemID = dr.GetInt32(4);
                        if (requestInfo.ShowID)
                            responseItem.Title += " (ItemID=" + responseItem.ItemID + ")";
                    }
                    if (!dr.IsDBNull(5))
                        responseItem.CreatedByUser = dr.GetString(5);
                    if (!dr.IsDBNull(6))
                    {
                        try
                        {
                            responseItem.CreatedDate = dr.GetDateTime(6);
                        }
                        catch
                        {
                            // Modules like HtmlModule has not field CreatedDate 
                        }
                    }
                    if (!dr.IsDBNull(7))
                    {
                        responseItem.PageID = dr.GetInt32(7);
                        if (requestInfo.ShowID)
                            responseItem.Description += " (PageID=" + responseItem.PageID + ", ModuleID=" +
                                                        responseItem.ModuleID + ")";
                    }
                    if (!dr.IsDBNull(8))
                        responseItem.PageName = dr.GetString(8);
                    if (!dr.IsDBNull(9))
                        responseItem.GeneralModDefID = dr.GetGuid(9).ToString().ToUpper();
                    if (!dr.IsDBNull(10))
                        responseItem.ModuleTitle = dr.GetString(10);

                    if (responseItem.GeneralModDefID == "0B113F51-FEA3-499A-98E7-7B83C192FDBB" || //Html Document
                        responseItem.GeneralModDefID == "2B113F51-FEA3-499A-98E7-7B83C192FDBB") //Html WYSIWYG Edit (V2)
                    {
                        // We use the database field [rb.Modules].[ModuleTitle]:
                        responseItem.Title = responseItem.ModuleTitle;
                    }

                    responseItem.Link = applicationFullPath + "/DesktopDefault.aspx?tabID=" + responseItem.PageID;
                    if (requestInfo.PortalAlias.Length != 0)
                        responseItem.Link += "&Alias=" + requestInfo.PortalAlias;

                    colItems.Add(responseItem);
                    hits++;
                }
            }
            finally
            {
                dr.Close(); //by Manu, fixed bug 807858
            }

            return colItems;
        }


        /// <summary>
        /// Get portal ID from database [rb_Portals].[PortalAlias].
        /// Return true if success and fill parameter portalID
        /// </summary>
        /// <param name="portalAlias">The alias to look up</param>
        /// <param name="portalID">Return ID here for portalAlias</param>
        /// <param name="AppleseedPage">Null is allowed here</param>
        /// <returns>True if a ID for portalAlias was found</returns>
        public static bool GetPortalIDViaAlias(string portalAlias, ref int portalID, Page AppleseedPage)
        {
            bool portalIDOK = true;

            if (AppleseedPage != null)
            {
                portalID = AppleseedPage.portalSettings.PortalID;
                return true;
            }


            if (portalAlias == string.Empty)
                // jes1111 - portalAlias = ConfigurationSettings.AppSettings["DefaultPortal"];
                portalAlias = Config.DefaultPortal;
            // jes1111 - bad assumption!
            //			if (portalAlias.ToLower() == "Appleseed")
            //				portalID = 0;  
            //			else
            //			{
            SqlDataReader dr =
                DBHelper.GetDataReader("SELECT PortalID FROM rb_Portals WHERE PortalAlias='" + portalAlias + "'");

            try
            {
                if (dr.Read())
                    portalID = dr.GetInt32(0);
                else
                    portalIDOK = false;
            }
            finally
            {
                dr.Close(); //by Manu, fixed bug 807858
            }
            //			}

            return portalIDOK;
        }


        /// <summary>
        /// Call local or external service
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="applicationFullPath">The application full path.</param>
        /// <param name="requestInfo">a filled ServiceRequestInfo class</param>
        /// <param name="AppleseedPage">The Appleseed page.</param>
        /// <returns>ServiceResponseInfo</returns>
        public static ServiceResponseInfo CallService(int portalID, Guid userID, string applicationFullPath,
                                                      ref ServiceRequestInfo requestInfo, Page AppleseedPage)
        {
            ServiceResponseInfo responseInfo = new ServiceResponseInfo();
            responseInfo.ServiceStatus = string.Empty;

            if (requestInfo.Type != ServiceType.RSSService)
            {
                if (portalID < 0)
                {
                    if (!GetPortalIDViaAlias(requestInfo.PortalAlias, ref portalID, AppleseedPage))
                        responseInfo.ServiceStatus += "WARNING! Unknown PortalAlias";
                }

                if (userID != Guid.Empty)
                {
                    // TBD!!
                    // Note: should be controlled by key in appSettings: 
                    // <add key="AllowRSSServiceSignin" value="false" />
                    // <add key="AllowWebServiceSignin" value="false" />
                    // userID = GetUserID(requestInfo.UserName, requestInfo.UserPassword);
                    /* Jakob says: later
					if (requestInfo.UserName == string.Empty)		
						userID = -1;
					else
					{
						Appleseed.Framework.Security.User user;
						UsersDB usersDB = new UsersDB();
						user = usersDB.Login(requestInfo.UserName, requestInfo.UserPassword, portalID);
						if (user != null)
							userID = int.Parse(user.ID.ToString());
						else
							userID = -1;
					}
					*/
                }


                if (requestInfo.LocalMode)
                {
                    if (AppleseedPage != null)
                    {
                        responseInfo.ServiceTitle = AppleseedPage.portalSettings.PortalName;
                        responseInfo.ServiceLink = Path.ApplicationFullPath;
                        responseInfo.ServiceDescription = AppleseedPage.portalSettings.PortalTitle;

                        responseInfo.ServiceImageTitle = AppleseedPage.portalSettings.PortalTitle;
                        responseInfo.ServiceImageUrl = Path.ApplicationFullPath + AppleseedPage.portalSettings.PortalPath +
                                                       "/logo.gif";
                        responseInfo.ServiceImageLink = Path.ApplicationFullPath;
                    }
                    else
                    {
                        responseInfo.ServiceTitle = "ServiceTitle TBD!"; //JLH!!
                        responseInfo.ServiceLink = Path.ApplicationFullPath;
                        responseInfo.ServiceDescription = "ServiceDescription TBD!";

                        responseInfo.ServiceImageTitle = "ImageTitle TBD!";
                        responseInfo.ServiceImageUrl = Path.ApplicationFullPath + "_Appleseed/logo.gif"; //JLH!! TBD!
                        responseInfo.ServiceImageLink = Path.ApplicationFullPath;
                    }
                }
            }

            //For test
            //responseInfo.ServiceStatus = "TEST: UserID=" + userID + " PortalID=" + portalID + " LocalMode=" + requestInfo.LocalMode;

            try
            {
                switch (requestInfo.Type)
                {
                    case ServiceType.CommunityWebService:
                        if (requestInfo.LocalMode)
                        {
                            // Run local code (not calling service via http/soap)
                            responseInfo.Items =
                                GetResponseItemList(portalID, userID, applicationFullPath, ref requestInfo);
                            if (responseInfo.ServiceStatus == string.Empty)
                                responseInfo.ServiceStatus = "OK";
                        }
                        else
                        {
                            responseInfo = CallCommunityService(requestInfo, "CommunityService.asmx");
                        }
                        responseInfo.ServiceDescription += " (" + requestInfo.ListType.ToString() + "List WebService)";
                        break;
                    case ServiceType.CommunityRSSService:
                        if (requestInfo.LocalMode)
                        {
                            switch (requestInfo.ListType)
                            {
                                case ServiceListType.Tab:
                                    responseInfo.Items =
                                        GetResponseTabList(portalID, userID, Path.ApplicationFullPath, ref requestInfo);
                                    break;
                                case ServiceListType.Module:
                                    responseInfo.Items =
                                        GetResponseModuleList(portalID, userID, Path.ApplicationFullPath,
                                                              ref requestInfo);
                                    break;
                                case ServiceListType.Item:
                                    responseInfo.Items =
                                        GetResponseItemList(portalID, userID, Path.ApplicationFullPath, ref requestInfo);
                                    break;
                            }
                            if (responseInfo.ServiceStatus == string.Empty)
                                responseInfo.ServiceStatus = "OK";
                        }
                        else
                        {
                            string oldUrl = requestInfo.Url;
                            if (!requestInfo.Url.EndsWith("/"))
                                requestInfo.Url += "/";
                            requestInfo.Url += "CommunityRSS.aspx" + AddRSSRequestParameters(requestInfo);
                            responseInfo = CallRssService(requestInfo);
                            requestInfo.Url = oldUrl;
                        }
                        responseInfo.ServiceDescription += " (RSS " + requestInfo.ListType.ToString() + "List Service)";
                        break;
                    case ServiceType.RSSService:
                        responseInfo = CallRssService(requestInfo);
                        if (responseInfo.ServiceStatus == string.Empty)
                            responseInfo.ServiceStatus = "OK";
                        break;
                    default:
                        responseInfo.ServiceStatus = "ERROR! The requested service type is not supported";
                        break;
                }
            }
            catch (Exception ex)
            {
                responseInfo.ServiceStatus = "FATAL ERROR! Can not call service. Problem: " + ex.Message;
            }

            return responseInfo;
        }


        /// <summary>
        /// Calls an RSS service and returns the service response.
        /// This method goes crazy with the Regex class to parse the response
        /// from an RSS service. After testing, I discovered that most
        /// RSS services do not return a valid XML response. That requires
        /// parsing through possibly faulty XML (missing end tags, etc.)
        /// </summary>
        /// <param name="requestInfo">a filled ServiceRequestInfo class</param>
        /// <returns>ServiceResponseInfo</returns>
        private static ServiceResponseInfo CallRssService(ServiceRequestInfo requestInfo)
        {
            ServiceResponseInfo responseInfo = new ServiceResponseInfo();

            // Grab the page
            string rawResponse = GetWebPage(requestInfo.Url);
            if (rawResponse == string.Empty)
            {
                responseInfo.ServiceStatus = "No reply from URL (could be timeout)";
                return responseInfo;
            }

            // Get Channel
            Match channel = Regex.Match(rawResponse, @"<channel>(.*?)</channel>", RegexOptions.Singleline);
            if (channel.Success)
            {
                // Get Service title
                Match serviceTitle =
                    Regex.Match(channel.Result("$1"), @"<title>(.*?)</title>",
                                RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (serviceTitle.Success)
                    responseInfo.ServiceTitle = serviceTitle.Result("$1");

                // Get Service Link
                Match serviceLink =
                    Regex.Match(channel.Result("$1"), @"<link>(.*?)</link>",
                                RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (serviceLink.Success)
                    responseInfo.ServiceLink = serviceLink.Result("$1");

                // Get Service Description
                Match serviceDescription =
                    Regex.Match(channel.Result("$1"), @"<description>(.*?)</description>",
                                RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (serviceDescription.Success)
                    responseInfo.ServiceDescription = serviceDescription.Result("$1");

                // Get Service Copyright
                Match serviceCopyright =
                    Regex.Match(channel.Result("$1"), @"<copyright>(.*?)</copyright>",
                                RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (serviceCopyright.Success)
                    responseInfo.ServiceCopyright = serviceCopyright.Result("$1");
            }


            // Get Image
            Match image = Regex.Match(rawResponse, @"<image>(.*?)</image>", RegexOptions.Singleline);
            if (image.Success)
            {
                // Get Image title
                Match imageTitle =
                    Regex.Match(image.Result("$1"), @"<title>(.*?)</title>",
                                RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (imageTitle.Success)
                    responseInfo.ServiceImageTitle = imageTitle.Result("$1");

                // Get Image Url
                Match imageUrl =
                    Regex.Match(image.Result("$1"), @"<url>(.*?)</url>",
                                RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (imageUrl.Success)
                    responseInfo.ServiceImageUrl = imageUrl.Result("$1");

                // Get Image Link
                Match imageLink =
                    Regex.Match(image.Result("$1"), @"<link>(.*?)</link>",
                                RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (imageLink.Success)
                    responseInfo.ServiceImageLink = imageLink.Result("$1");
            }

            // Get Items
            MatchCollection items = Regex.Matches(rawResponse, @"<item>(.*?)</item>", RegexOptions.Singleline);
            int counter = 0;
            foreach (Match item in items)
            {
                counter++;
                if (counter > requestInfo.MaxHits)
                    break;

                ServiceResponseInfoItem responseInfoItem = new ServiceResponseInfoItem();

                // Get Item title
                Match itemTitle =
                    Regex.Match(item.Result("$1"), @"<title>(.*?)</title>",
                                RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (itemTitle.Success)
                    responseInfoItem.Title = itemTitle.Result("$1");

                // Get Item Link
                Match itemLink =
                    Regex.Match(item.Result("$1"), @"<link>(.*?)</link>",
                                RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (itemLink.Success)
                    responseInfoItem.Link = itemLink.Result("$1");

                // Get Item Description
                Match itemDescription =
                    Regex.Match(item.Result("$1"), @"<description>(.*?)</description>",
                                RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (itemDescription.Success)
                    responseInfoItem.Description = itemDescription.Result("$1");


                // Add new item to Response Info
                responseInfo.Items.Add(responseInfoItem);
            }

            return responseInfo;
        }


        /// <summary>
        /// Retrieves a Web page that represents an RSS response.
        /// If no reply in 15 seconds, it gives up.
        /// </summary>
        /// <param name="url">URL of RSS service</param>
        /// <returns>string</returns>
        private static string GetWebPage(string url)
        {
            StringBuilder builder = new StringBuilder();

            WebRequest req = WebRequest.Create(url);

            // Set Timeout to 15 seconds
            req.Timeout = 15000;

            try
            {
                WebResponse result = req.GetResponse();
                Stream ReceiveStream = result.GetResponseStream();

                Byte[] read = new Byte[512];
                try
                {
                    int bytes = ReceiveStream.Read(read, 0, 512);

                    while (bytes > 0)
                    {
                        Encoding encode = Encoding.GetEncoding("utf-8");
                        builder.Append(encode.GetString(read, 0, bytes));
                        bytes = ReceiveStream.Read(read, 0, 512);
                    }
                }
                finally
                {
                    ReceiveStream.Close(); // by Manu
                }
            }
            catch (Exception)
            {
            }

            return builder.ToString();
        }


        /// <summary>
        /// Calls a community Web service. This method uses a Web service
        /// proxy class to call a community service from (possibly) another
        /// community Web site.
        /// </summary>
        /// <param name="requestInfo">The request info.</param>
        /// <param name="servicefile">The servicefile.</param>
        /// <returns></returns>
        private static ServiceResponseInfo CallCommunityService(ServiceRequestInfo requestInfo, string servicefile)
        {
            // instantiate the community web service proxy class
            using (communityService objService = new communityService())
            {
                // Set the url
                objService.Url = requestInfo.Url;
                if (!objService.Url.EndsWith("/"))
                    objService.Url += "/";
                objService.Url += servicefile;

                // return result
                ServiceResponseInfo responseInfo = objService.GetCommunityContent(requestInfo);
                return responseInfo;
            }
        }

        /// <summary>
        /// Use private constructor for class with static methods.
        /// </summary>
        private ServiceHelper()
        {
        }


        /// <summary>
        /// Creates a Error RSS Appleseed Feed
        /// </summary>
        /// <param name="title">Used in the title section of the RSS feed</param>
        /// <param name="link">Used as the link</param>
        /// <param name="description">Used in the description section of the RSS feed</param>
        /// <returns>string</returns>
        public static string CreateErrorRSSFeed(
            string title,
            string link,
            string description)
        {
            return CreateSimpleRSSFeed(
                title,
                link,
                description,
                "Appleseed Site",
                Path.WebPathCombine(Path.ApplicationFullPath, "aspnet_client/logo_sml.gif"),
                Path.ApplicationFullPath,
                100,
                40,
                string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Creates a simple RSS Appleseed Feed
        /// </summary>
        /// <param name="title">Used in the title section of the RSS feed</param>
        /// <param name="link">Used as the link</param>
        /// <param name="description">Used in the description section of the RSS feed</param>
        /// <param name="imageTitle">The image title.</param>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="imageLink">The image link.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="itemTitle">The item title.</param>
        /// <param name="itemLink">The item link.</param>
        /// <param name="itemDescription">The item description.</param>
        /// <returns>string</returns>
        public static string CreateSimpleRSSFeed(
            string title,
            string link,
            string description,
            string imageTitle,
            string imageUrl,
            string imageLink,
            int imageWidth,
            int imageHeight,
            string itemTitle,
            string itemLink,
            string itemDescription)
        {
            StringBuilder sb;
            sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.Append(
                "<!DOCTYPE rss PUBLIC \"-//Netscape Communications//DTD RSS 0.91//EN\" \"http://my.netscape.com/publish/formats/rss-0.91.dtd\">");
            sb.Append("<rss version=\"0.91\">");
            sb.Append("<channel>");
            sb.Append("<title>" + title + "</title>");
            sb.Append("<link>" + link + "</link>");
            sb.Append("<description>" + description + "</description>");
            sb.Append("<image>");
            sb.Append("<title>" + imageTitle + "</title>");
            sb.Append("<url>" + imageUrl + "</url>");
            sb.Append("<link>" + imageLink + "</link>");
            sb.Append("<width>" + imageWidth + "</width>");
            sb.Append("<height>" + imageHeight + "</height>");
            sb.Append("</image>");

            if (itemTitle.Length != 0)
            {
                sb.Append("<item>");
                sb.Append("<title>" + itemTitle + "</title>");
                sb.Append("<link>" + itemLink + "</link>");
                sb.Append("<description>" + itemDescription + "</description>");
                sb.Append("</item>");
            }

            sb.Append("</channel>");
            sb.Append("</rss>");

            return sb.ToString();
        }
    }
}