using System;
using System.Web;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Security.Permissions;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Web.Caching;
using Appleseed.Framework;
using System.Collections;
using System.Threading;
using Appleseed.Framework.Settings;

namespace Appleseed.Framework.Providers.AppleseedSiteMapProvider
{

    /// <summary>
    /// Summary description for SqlSiteMapProvider
    /// </summary>
    [SqlClientPermission(SecurityAction.Demand, Unrestricted = true)]
    public class AppleseedSqlSiteMapProvider : AppleseedSiteMapProvider
    {

        private const int _rootNodeID = -1;

        private const string _errmsg1 = "Missing node ID";
        private const string _errmsg2 = "Duplicate node ID";
        private const string _errmsg4 = "Invalid parent ID: {0} on this list: {1}";
        private const string _errmsg5 = "Empty or missing connectionStringName";
        private const string _errmsg6 = "Missing connection string";
        private const string _errmsg7 = "Empty connection string";
        private const string _errmsg8 = "Invalid sqlCacheDependency";

        public const string _cacheDependencyName = "__SiteMapCacheDependency";

        private string _connect;              // Database connection string
        private string _database, _table;     // Database info for SQL Server 7/2000 cache dependency
        private bool _2005dependency = false; // Database info for SQL Server 2005 cache dependency

        private int _indexPageID, _indexParentPageID, _indexPageOrder, _indexPortalID,
                     _indexPageName, _indexAuthorizedRoles, _indexPageLayout, _indexPageDescription;

        private Dictionary<int, SiteMapNode> _nodes = new Dictionary<int, SiteMapNode>(16);
        private readonly object _lock = new object();
        private SiteMapNode _root;

        public override void Initialize(string name, NameValueCollection config)
        {
            // Verify that config isn't null
            if (config == null)
                throw new ArgumentNullException("config");

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
                name = "AppleseedSqlSiteMapProvider";

            // Add a default "description" attribute to config if the
            // attribute doesn’t exist or is empty
            if (string.IsNullOrEmpty(config["description"])) {
                config.Remove("description");
                config.Add("description", "Appleseed SQL site map provider");
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            // Initialize _connect
            string connect = config["connectionStringName"];

            if (String.IsNullOrEmpty(connect)) {
                throw new ProviderException(_errmsg5);
            }

            config.Remove("connectionStringName");

            if (WebConfigurationManager.ConnectionStrings[connect] == null) {
                throw new ProviderException(_errmsg6);
            }

            _connect = WebConfigurationManager.ConnectionStrings[connect].ConnectionString;

            if (String.IsNullOrEmpty(_connect)) {
                throw new ProviderException(_errmsg7);
            }

            // Initialize SQL cache dependency info
            string dependency = config["sqlCacheDependency"];

            if (!String.IsNullOrEmpty(dependency)) {
                if (String.Equals(dependency, "CommandNotification", StringComparison.InvariantCultureIgnoreCase)) {
                    SqlDependency.Start(_connect);
                    _2005dependency = true;
                } else {
                    // If not "CommandNotification", then extract database and table names
                    string[] info = dependency.Split(new char[] { ':' });
                    if (info.Length != 2) {
                        throw new ProviderException(_errmsg8);
                    }
                    _database = info[0];
                    _table = info[1];
                }
                config.Remove("sqlCacheDependency");
            }

            // SiteMapProvider processes the securityTrimmingEnabled
            // attribute but fails to remove it. Remove it now so we can
            // check for unrecognized configuration attributes.
            if (config["securityTrimmingEnabled"] != null) {
                config.Remove("securityTrimmingEnabled");
            }

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0) {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr)) {
                    throw new ProviderException("Unrecognized attribute: " + attr);
                }
            }
        }


        /// <summary>
        /// Loads the site map information from rb_Pages table and builds the site map information 
        /// in memory.
        /// </summary>
        /// <returns>The root System.Web.SiteMapNode of the site map navigation structure.</returns>
        public override SiteMapNode BuildSiteMap()
        {
            lock (_lock) {
                // Return immediately if this method has been called before
                //if (_root != null) {
                //    if (_root["PortalID"] == PortalID) {
                //        return _root;
                //    } else {
                        this.Clear();
                //    }
                //}

                // Query the database for site map nodes
                SqlConnection connection = new SqlConnection(_connect);

                try {
                    SqlCommand command = new SqlCommand(BuildSiteMap_Query(), connection);
                    command.CommandType = CommandType.Text;

                    // Create a SQL cache dependency if requested
                    SqlCacheDependency dependency = null;

                    if (_2005dependency) {
                        dependency = new SqlCacheDependency(command);
                    } else if (!String.IsNullOrEmpty(_database) && !string.IsNullOrEmpty(_table)) {
                        dependency = new SqlCacheDependency(_database, _table);
                    }

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    _indexPageID = reader.GetOrdinal("PageID");
                    _indexParentPageID = reader.GetOrdinal("ParentPageID");
                    _indexPageOrder = reader.GetOrdinal("PageOrder");
                    _indexPortalID = reader.GetOrdinal("PortalID");
                    _indexPageName = reader.GetOrdinal("PageName");
                    _indexAuthorizedRoles = reader.GetOrdinal("AuthorizedRoles");
                    _indexPageLayout = reader.GetOrdinal("PageLayout");
                    _indexPageDescription = reader.GetOrdinal("PageDescription");

                    if (reader.Read()) {
                        // Create an empty root node and add it to the site map
                        _root = new SiteMapNode(this, _rootNodeID.ToString(), HttpUrlBuilder.BuildUrl(), string.Empty, string.Empty, new string[] { "All Users" }, null, null, null);
                        _root["PortalID"] = PortalID;
                        _nodes.Add(_rootNodeID, _root);
                        AddNode(_root, null);

                        // Build a tree of SiteMapNodes underneath the root node
                        do {
                            // Create another site map node and add it to the site map
                            SiteMapNode node = CreateSiteMapNodeFromDataReader(reader);
                            SiteMapNode parentNode = GetParentNodeFromDataReader(reader);
                            if (parentNode != null) {
                                try
                                {
                                    AddNode(node, parentNode);
                                }
                                catch
                                {
                                    if (node.Url.Contains("?"))
                                        node.Url = node.Url + "&lnkId=" + node.Key;
                                    else
                                        node.Url = node.Url + "?lnkId=" + node.Key;
                                    AddNode(node, parentNode);
                                }
                            }
                        } while (reader.Read());

                        // Use the SQL cache dependency
                        if (dependency != null) {
                            HttpRuntime.Cache.Insert(_cacheDependencyName + PortalID, new object(), dependency,
                                Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable,
                                new CacheItemRemovedCallback(OnSiteMapChanged));
                        }
                    }
                } finally {
                    connection.Close();
                }

                // Return the root SiteMapNode
                return _root;
            }
        }



        /// <summary>
        /// Returns the root node.
        /// </summary>
        /// <returns>The root node.</returns>
        protected override SiteMapNode GetRootNodeCore()
        {
            lock (_lock) {
                BuildSiteMap();
                return _root;
            }
        }

        // Helper methods
        private SiteMapNode CreateSiteMapNodeFromDataReader(DbDataReader reader)
        {
            // Make sure the node ID is present
            if (reader.IsDBNull(_indexPageID)) {
                throw new ProviderException(_errmsg1);
            }

            // Get the node ID from the DataReader
            int id = reader.GetInt32(_indexPageID);

            // Make sure the node ID is unique
            if (_nodes.ContainsKey(id)) {
                throw new ProviderException(_errmsg2);
            }

            string name = reader.IsDBNull(_indexPageName) ? null : reader.GetString(_indexPageName).Trim();
            string description = reader.IsDBNull(_indexPageDescription) ? null : reader.GetString(_indexPageDescription).Trim();
            string roles = reader.IsDBNull(_indexAuthorizedRoles) ? null : reader.GetString(_indexAuthorizedRoles).Trim();

            string url = HttpUrlBuilder.BuildUrl(id);
            if (!url.StartsWith(Path.ApplicationRoot) || !url.StartsWith(Path.ApplicationFullPath))
            {
                url = HttpUrlBuilder.BuildUrl("~/Default.aspx", "sitemapTargetPage=" + id);
            }
            
            // If roles were specified, turn the list into a string array
            string[] rolelist = null;
            if (!String.IsNullOrEmpty(roles)) {
                rolelist = roles.Split(new char[] { ',', ';' }, 512);
            }
            if (rolelist != null) {
                int rolelistLength = rolelist.Length;
                if (rolelistLength > 0) {
                    if (rolelist[rolelistLength - 1].Equals(string.Empty)) {
                        string[] auxrolelist = new string[rolelistLength - 1];
                        for (int i = 0; i < rolelistLength - 1; i++) {
                            auxrolelist[i] = rolelist[i];
                        }

                        rolelist = auxrolelist;
                    }
                }
            }

            // Create a SiteMapNode
            SiteMapNode node = new SiteMapNode(this, id.ToString(), url, name, description, rolelist, null, null, null);

            // Record the node in the _nodes dictionary
          
                _nodes.Add(id, node);
          


            // Return the node        
            return node;
        }


        private SiteMapNode GetParentNodeFromDataReader(DbDataReader reader)
        {
            // Make sure the parent ID is present
            if (reader.IsDBNull(_indexParentPageID)) {
                return _nodes[_rootNodeID];
            }

            // Get the parent ID from the DataReader
            int pid = reader.GetInt32(_indexParentPageID);

            // Make sure the parent ID is valid
            if (!_nodes.ContainsKey(pid)) {
                //string list = string.Empty;
                //foreach (int key in _nodes.Keys) {
                //    list += String.Format("{0}-{1};", key, _nodes[key].Key);
                //}
                //throw new ProviderException(String.Format(_errmsg4, pid, list));
                return null;
            }

            // Return the parent SiteMapNode
            return _nodes[pid];
        }

        void OnSiteMapChanged(string key, object item, CacheItemRemovedReason reason)
        {
            lock (_lock) {
                if (key == _cacheDependencyName && reason == CacheItemRemovedReason.DependencyChanged) {
                    // Refresh the site map
                    Clear();
                    _nodes.Clear();
                    _root = null;
                }
            }
        }


        /// <summary>
        /// Removes all elements in the collections of child and parent site map nodes
        /// that the System.Web.StaticSiteMapProvider tracks as part of its state.
        /// </summary>
        public override void ClearCache()
        {
            this.Clear();
        }


        /// <summary>
        /// Removes all elements in the collections of child and parent site map nodes
        /// that the System.Web.StaticSiteMapProvider tracks as part of its state.
        /// </summary>
        protected override void Clear()
        {
            base.Clear();
            _nodes.Clear();
            _root = null;
        }


        private string BuildSiteMap_Query()
        {
           
            string s = @"
				SELECT	[PageID], [ParentPageID], [PageOrder], [PortalID], COALESCE (
                   (SELECT SettingValue
                    FROM   rb_TabSettings
                    WHERE  TabID = rb_Pages.PageID 
                       AND SettingName = '" + Thread.CurrentThread.CurrentUICulture + @"'
                       AND Len(SettingValue) > 0), 
                    PageName)  AS [PageName],[AuthorizedRoles], [PageLayout], [PageDescription]
				FROM  [dbo].[rb_Pages] 
				WHERE [PortalID] = " + PortalID + @" 
				ORDER BY [PageOrder]
			";
            return s;
        }

        private string PortalID
        {
            get
            {
                Appleseed.Context.Reader contextReader = new Appleseed.Context.Reader(new Appleseed.Context.WebContextReader());
                HttpContext context = contextReader.Current;
                return context.Items["PortalID"].ToString();
            }
        }

        public override bool IsAccessibleToUser(HttpContext context, SiteMapNode node)
        {
            bool isVisible = false;

            if (node.Roles != null) {
                if (context.User.Identity.IsAuthenticated) {
                    if (node.Roles.Contains("All Users") || node.Roles.Contains("Authenticated Users")) {
                        isVisible = true;
                    } else {
                        IEnumerator enumerator = node.Roles.GetEnumerator();
                        while (!isVisible && enumerator.MoveNext()) {
                            isVisible = context.User.IsInRole((string)enumerator.Current);
                        }
                    }
                } else {
                    isVisible = (node.Roles.Contains("All Users") || node.Roles.Contains("Unauthenticated Users"));
                }
            }
            return isVisible;
        }
    }
}
		
		


