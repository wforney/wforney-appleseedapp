using System;
using System.Collections;

namespace Appleseed.Framework.Services
{
    /// <summary>
    /// Lists the possible types of services that can be used
    /// with a community.
    /// </summary>
    public enum ServiceType
    {
        Unknown = 0,
        CommunityWebService = 1,
        CommunityRSSService = 2,
        RSSService = 3
    }


    /// <summary>
    /// Lists the possible types of services
    /// </summary>
    public enum ServiceListType
    {
        Item = 1,
        Module = 2,
        Tab = 3
    }


    /// <summary>
    /// This class Represents all the information about what the requested
    /// service (described in the URL attribute) should return.
    /// </summary>
    public class ServiceRequestInfo
    {
        private ServiceType type = ServiceType.Unknown;
        private string url = string.Empty;
        private string portalAlias = string.Empty;
        private bool localMode = true;
        private string userName = string.Empty;
        private string userPassword = string.Empty;

        private ServiceListType listType = ServiceListType.Tab;

        private string moduleType = "All";
                       // Aka. field [rb_GeneralModuleDefinitions].[ClassName]. "All" is all classes supported by search

        // All;Announcements;Contacts;Discussion;Events;HtmlModule;Documents;Pictures;Articles;Tasks;FAQs;ComponentModule
        private int maxHits = 20;
        private bool showID = false;
        private string searchString = string.Empty;
        private string searchField = string.Empty;
        private string sortField = "ModuleName"; //ModuleName;Title;CreatedByUser;CreatedDate;TabName
        private string sortDirection = "ASC";
        private bool rootLevelOnly = false;
        private bool mobileOnly = false;
        private string _IDList = string.Empty; //Tab, Module and Item ID. Can be a list: "23,45,56"
        private ServiceListType _IDListType = ServiceListType.Tab;
        private int tag = 0;

        /// <summary>
        /// Represents the type of the service such as community Web
        /// service or RSS.
        /// Default value: ServiceType.Unknown
        /// </summary>
        /// <value>The type.</value>
        public ServiceType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Represents the URL of the service (aka. data source)
        /// Default value: string.Empty
        /// </summary>
        /// <value>The URL.</value>
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        /// <summary>
        /// Represents the Alias of the site
        /// See database: [rb_Portal].[PortalAlias]
        /// Default value: string.Empty (the value of web.config key DefaultPortal will be used!)
        /// </summary>
        /// <value>The portal alias.</value>
        public string PortalAlias
        {
            get { return portalAlias; }
            set { portalAlias = value; }
        }

        /// <summary>
        /// When true the Url is ignored and the local Appleseed site is used as data soure
        /// Default value: false
        /// </summary>
        /// <value><c>true</c> if [local mode]; otherwise, <c>false</c>.</value>
        public bool LocalMode
        {
            get { return localMode; }
            set { localMode = value; }
        }

        /// <summary>
        /// Used together with UserPassword to sign-in and retrieve data with this
        /// users credentials/rights
        /// Default value: string.Empty
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// Used together with UserName to sign-in and retrieve data with this
        /// users credentials/rights
        /// Default value: string.Empty
        /// </summary>
        /// <value>The user password.</value>
        public string UserPassword
        {
            get { return userPassword; }
            set { userPassword = value; }
        }

        /// <summary>
        /// The type of the required list. See enum ServiceListType
        /// Default value: ServiceListType.Tab
        /// </summary>
        /// <value>The type of the list.</value>
        public ServiceListType ListType
        {
            get { return listType; }
            set { listType = value; }
        }

        /// <summary>
        /// Module type
        /// See database: [rb_GeneralModuleDefinitions].[ClassName]
        /// Valid values: All;Announcements;Contacts;Discussion;Events;HtmlModule;Documents;Pictures;Articles;Tasks;FAQs;ComponentModule
        /// Default value: "All".
        /// </summary>
        /// <value>The type of the module.</value>
        public string ModuleType
        {
            get { return moduleType; }
            set { moduleType = value; }
        }

        /// <summary>
        /// Represents the number of items returned by the service.
        /// Default value: 20
        /// </summary>
        /// <value>The max hits.</value>
        public int MaxHits
        {
            get { return maxHits; }
            set { maxHits = value; }
        }

        /// <summary>
        /// If true ID's are displyed in the lists
        /// Default value: false
        /// </summary>
        /// <value><c>true</c> if [show ID]; otherwise, <c>false</c>.</value>
        public bool ShowID
        {
            get { return showID; }
            set { showID = value; }
        }

        /// <summary>
        /// Search string. Note: different behaviour depending on the ListType
        /// Default value: string.Empty
        /// </summary>
        /// <value>The search string.</value>
        public string SearchString
        {
            get { return searchString; }
            set { searchString = value; }
        }

        /// <summary>
        /// Set this if only a single field should be searched e.g.: "Title"
        /// Default value: string.Empty
        /// </summary>
        /// <value>The search field.</value>
        public string SearchField
        {
            get { return searchField; }
            set { searchField = value; }
        }

        /// <summary>
        /// Sort list on this field
        /// Valid values: ModuleName;Title;CreatedByUser;CreatedDate;TabName
        /// Default value: ModuleName
        /// </summary>
        /// <value>The sort field.</value>
        public string SortField
        {
            get { return sortField; }
            set { sortField = value; }
        }

        /// <summary>
        /// Sort Ascending or Descending
        /// Valid values: ASC;DESC
        /// Default value: ASC
        /// </summary>
        /// <value>The sort direction.</value>
        public string SortDirection
        {
            get { return sortDirection; }
            set { sortDirection = value; }
        }

        /// <summary>
        /// If true only tabs or modules where tab parent is at top level are listed
        /// Default value: false
        /// </summary>
        /// <value><c>true</c> if [root level only]; otherwise, <c>false</c>.</value>
        public bool RootLevelOnly
        {
            get { return rootLevelOnly; }
            set { rootLevelOnly = value; }
        }

        /// <summary>
        /// If true only data for mobile devices are listed
        /// Default value: false
        /// </summary>
        /// <value><c>true</c> if [mobile only]; otherwise, <c>false</c>.</value>
        public bool MobileOnly
        {
            get { return mobileOnly; }
            set { mobileOnly = value; }
        }

        /// <summary>
        /// Comma separated list of ID's. e.g.: 1234,234,5454.
        /// Only data for these ID's are listed. The ID type is controlled using
        /// attribute IDListType
        /// Default value: string.Empty
        /// </summary>
        /// <value>The ID list.</value>
        public string IDList
        {
            get { return _IDList; }
            set { _IDList = value; }
        }

        /// <summary>
        /// Controls the type of ID's in attribute IDList
        /// Default value: ServiceListType.Tab
        /// </summary>
        /// <value>The type of the ID list.</value>
        public ServiceListType IDListType
        {
            get { return _IDListType; }
            set { _IDListType = value; }
        }

        /// <summary>
        /// The service that receives this tag does a check and see if it can
        /// do the special thingy that the tag value controls.
        /// If the tag value is not supported "Tag=X not supported" is returned
        /// in field ServiceResponseInfo.ServiceStatus. The service should try
        /// to deliver data for "normal case" (Tag=0).
        /// </summary>
        /// <value>The tag.</value>
        public int Tag
        {
            get { return tag; }
            set { tag = value; }
        }
    }


    /// <summary>
    /// This class Represents the response from a service including
    /// the response from a community Web service and a RSS service.
    /// </summary>
    public class ServiceResponseInfo
    {
        private string serviceStatus = "Unknown";
        private string serviceTitle = string.Empty;
        private string serviceLink = string.Empty;
        private string serviceDescription = string.Empty;
        private string serviceCopyright = string.Empty;
        private string serviceImageTitle = string.Empty;
        private string serviceImageUrl = string.Empty;
        private string serviceImageLink = string.Empty;
        private ArrayList items = new ArrayList();

        /// <summary>
        /// Contains a response code from the service. If everything
        /// works, the response is "OK".
        /// Default value: the string "Unknown"
        /// </summary>
        /// <value>The service status.</value>
        public string ServiceStatus
        {
            get { return serviceStatus; }
            set { serviceStatus = value; }
        }

        /// <summary>
        /// Represents the title of the service.
        /// Default value: string.Empty
        /// </summary>
        /// <value>The service title.</value>
        public string ServiceTitle
        {
            get { return serviceTitle; }
            set { serviceTitle = value; }
        }

        /// <summary>
        /// Represents the URL of the service.
        /// Default value: string.Empty
        /// </summary>
        /// <value>The service link.</value>
        public string ServiceLink
        {
            get { return serviceLink; }
            set { serviceLink = value; }
        }

        /// <summary>
        /// Represents the description of the service.
        /// Default value: string.Empty
        /// </summary>
        /// <value>The service description.</value>
        public string ServiceDescription
        {
            get { return serviceDescription; }
            set { serviceDescription = value; }
        }

        /// <summary>
        /// Represents copyright information associated with the service.
        /// Default value: string.Empty
        /// </summary>
        /// <value>The service copyright.</value>
        public string ServiceCopyright
        {
            get { return serviceCopyright; }
            set { serviceCopyright = value; }
        }

        /// <summary>
        /// Represents the text associated with the service image.
        /// Default value: string.Empty
        /// </summary>
        /// <value>The service image title.</value>
        public string ServiceImageTitle
        {
            get { return serviceImageTitle; }
            set { serviceImageTitle = value; }
        }

        /// <summary>
        /// Represents the URL of an image associated with the service.
        /// Default value: string.Empty
        /// </summary>
        /// <value>The service image URL.</value>
        public string ServiceImageUrl
        {
            get { return serviceImageUrl; }
            set { serviceImageUrl = value; }
        }

        /// <summary>
        /// Represents a URL associated with a service image.
        /// Default value: string.Empty
        /// </summary>
        /// <value>The service image link.</value>
        public string ServiceImageLink
        {
            get { return serviceImageLink; }
            set { serviceImageLink = value; }
        }

        /// <summary>
        /// Represents the content items returned by the service.
        /// </summary>
        /// <value>The items.</value>
        public ArrayList Items
        {
            set { items = value; }
            get { return items; }
        }
    }


    /// <summary>
    /// This class represents a particular content item returned by a service.
    /// </summary>
    public class ServiceResponseInfoItem
    {
        // RSS NOTE: RSS only uses attributes Link, Title and Description
        private string link = string.Empty;
        private string title = string.Empty;
        private string description = string.Empty;

        private string friendlyName = string.Empty;
        private int moduleID = -1;
        private int itemID = -1;
        private string createdByUser = string.Empty;
        private DateTime createdDate;
        private int tabID = -1;
        private string tabName = string.Empty;
        private string generalModDefID = string.Empty;
        private string moduleTitle = string.Empty;

        /// <summary>
        /// URL link that later is applyed to the attribute Title
        /// Default value: string.Empty
        /// </summary>
        /// <value>The link.</value>
        public string Link
        {
            get { return link; }
            set { link = value; }
        }

        /// <summary>
        /// Title of the item/module/tab
        /// Default value: string.Empty
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// Title of the item/module/tab (aka Abstract)
        /// Default value: string.Empty
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Name of module in english
        /// See database: [rb_GeneralModuleDefinitions].[FriendlyName]
        /// Default value: string.Empty
        /// </summary>
        /// <value>The name of the friendly.</value>
        public string FriendlyName
        {
            get { return friendlyName; }
            set { friendlyName = value; }
        }

        /// <summary>
        /// The module ID of the item
        /// Default value: -1
        /// </summary>
        /// <value>The module ID.</value>
        public int ModuleID
        {
            get { return moduleID; }
            set { moduleID = value; }
        }

        /// <summary>
        /// Item ID
        /// Default value: -1
        /// </summary>
        /// <value>The item ID.</value>
        public int ItemID
        {
            get { return itemID; }
            set { itemID = value; }
        }

        /// <summary>
        /// Name (email) of the user that created the item
        /// Default value: string.Empty
        /// </summary>
        /// <value>The created by user.</value>
        public string CreatedByUser
        {
            get { return createdByUser; }
            set { createdByUser = value; }
        }

        /// <summary>
        /// Creation date of the item
        /// </summary>
        /// <value>The created date.</value>
        public DateTime CreatedDate
        {
            get { return createdDate; }
            set { createdDate = value; }
        }

        /// <summary>
        /// The Tab ID of the items module
        /// Default value: -1
        /// </summary>
        /// <value>The page ID.</value>
        public int PageID
        {
            get { return tabID; }
            set { tabID = value; }
        }

        /// <summary>
        /// Name of tab where the item is displayed
        /// See database: [rb_Tabs].[TabName]
        /// Default value: string.Empty
        /// </summary>
        /// <value>The name of the page.</value>
        public string PageName
        {
            get { return tabName; }
            set { tabName = value; }
        }

        /// <summary>
        /// GUID ID of module.
        /// See database: [rb_GeneralModuleDefinitions].[GeneralModDefID]
        /// Default value: string.Empty
        /// </summary>
        /// <value>The general mod def ID.</value>
        public string GeneralModDefID
        {
            get { return generalModDefID; }
            set { generalModDefID = value; }
        }

        /// <summary>
        /// Name of module where the item is placed
        /// See database: [rb_Modules].[ModuleTitle]
        /// Default value: string.Empty
        /// </summary>
        /// <value>The module title.</value>
        public string ModuleTitle
        {
            get { return moduleTitle; }
            set { moduleTitle = value; }
        }

        /// <summary>
        /// Initializes a new instance of the ServiceResponseInfoItem Class
        /// </summary>
        public ServiceResponseInfoItem()
        {
        }
    }
}