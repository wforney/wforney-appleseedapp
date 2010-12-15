using System;
using System.Globalization;
using System.Threading;
using System.Web;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web;

namespace Appleseed.Framework
{
    /// <summary>
    /// HttpUrlBuilder
    /// This Class is Responsible for all the Urls in Appleseed to prevent
    /// hardcoded urls. 
    /// This makes it easier to update urls across the multiple portals
    /// Original ideas from John Mandia, Cory Isakson, Jes and Manu.
    /// </summary>
    [History("jminond", "2005/03/10", "Tab to page conversion")]
    [
        History("john.mandia@whitelightsolutions.com", "2004/09/2",
            "Introduced the provider pattern for UrlBuilder so that people can implement their rules for how urls should be built"
            )]
    [
        History("john.mandia@whitelightsolutions.com", "2003/08/13",
            "Removed Keywords splitter - rebuilt handler code to use a rules engine and changed code on url builder to make it cleaner and compatible"
            )]
    [History("Jes1111", "2003/03/18", "Added Keyword Splitter feature, see explanation in web.config")]
    [History("Jes1111", "2003/04/24", "Fixed problem with '=' in Keyword Splitter")]
    public class HttpUrlBuilder
    {
        private static UrlBuilderProvider provider = UrlBuilderProvider.Instance();

        /// <summary>
        /// Builds the url for get to current portal home page
        /// containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <returns></returns>
        public static string BuildUrl()
        {
            return (BuildUrl("~/" + DefaultPage, 0, 0, null, string.Empty, string.Empty, string.Empty));
        }

        /// <summary>
        /// Builds the url for get to current portal home page
        /// containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">Linked page</param>
        /// <returns></returns>
        public static string BuildUrl(string targetPage)
        {
            return (BuildUrl(targetPage, 0, 0, null, string.Empty, string.Empty, string.Empty));
        }

        /// <summary>
        /// Builds the url for get to current portal home page
        /// containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">Linked page</param>
        /// <param name="customAttributes">Any custom attribute that can be needed</param>
        /// <returns></returns>
        public static string BuildUrl(string targetPage, string customAttributes)
        {
            return (BuildUrl(targetPage, 0, 0, null, customAttributes, string.Empty, string.Empty));
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page
        /// containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="pageID">ID of the tab</param>
        /// <returns></returns>
        public static string BuildUrl(int pageID)
        {
            return (BuildUrl("~/" + DefaultPage, pageID, 0, null, string.Empty, string.Empty, string.Empty));
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page
        /// containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="pageID">ID of the page</param>
        /// <param name="urlKeywords">Add some keywords to uniquely identify this tab. Usual source is UrlKeyword from TabSettings.</param>
        /// <returns></returns>
        public static string BuildUrl(int pageID, string urlKeywords)
        {
            return (BuildUrl("~/" + DefaultPage, pageID, 0, null, string.Empty, string.Empty, urlKeywords));
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        /// containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">Linked page</param>
        /// <param name="pageID">ID of the page</param>
        /// <returns></returns>
        public static string BuildUrl(string targetPage, int pageID)
        {
            return (BuildUrl(targetPage, pageID, 0, null, string.Empty, string.Empty, string.Empty));
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        /// containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">Linked page</param>
        /// <param name="pageID">ID of the page</param>
        /// <param name="customAttributes">Any custom attribute that can be needed</param>
        /// <returns></returns>
        public static string BuildUrl(string targetPage, int pageID, string customAttributes)
        {
            return BuildUrl(targetPage, pageID, 0, null, customAttributes, string.Empty, string.Empty);
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        /// containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">Linked page</param>
        /// <param name="pageID">ID of the page</param>
        /// <param name="customAttributes">Any custom attribute that can be needed</param>
        /// <param name="currentAlias">Current Alias</param>
        /// <returns></returns>
        public static string BuildUrl(string targetPage, int pageID, string customAttributes, string currentAlias)
        {
            return BuildUrl(targetPage, pageID, 0, null, customAttributes, currentAlias, string.Empty);
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        /// containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">Linked page</param>
        /// <param name="pageID">ID of the page</param>
        /// <param name="culture">Client culture</param>
        /// <param name="customAttributes">Any custom attribute that can be needed</param>
        /// <param name="currentAlias">Current Alias</param>
        /// <returns></returns>
        public static string BuildUrl(string targetPage, int pageID, CultureInfo culture, string customAttributes,
                                      string currentAlias)
        {
            return BuildUrl(targetPage, pageID, 0, culture, customAttributes, currentAlias, string.Empty);
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        /// containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">Linked page</param>
        /// <param name="pageID">ID of the page</param>
        /// <param name="modID">ID of the module</param>
        /// <returns></returns>
        public static string BuildUrl(string targetPage, int pageID, int modID)
        {
            return BuildUrl(targetPage, pageID, modID, null, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        /// containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">Linked page</param>
        /// <param name="pageID">ID of the page</param>
        /// <param name="modID">ID of the module</param>
        /// <param name="culture">Client culture</param>
        /// <param name="customAttributes">Any custom attribute that can be needed. Use the following format...single attribute: paramname--paramvalue . Multiple attributes: paramname--paramvalue/paramname2--paramvalue2/paramname3--paramvalue3</param>
        /// <param name="currentAlias">Current Alias</param>
        /// <param name="urlKeywords">Add some keywords to uniquely identify this tab. Usual source is UrlKeyword from TabSettings.</param>
        /// <returns></returns>
        public static string BuildUrl(string targetPage, int pageID, int modID, CultureInfo culture,
                                      string customAttributes, string currentAlias, string urlKeywords)
        {
            PortalSettings currentSetting = null;

            if (HttpContext.Current.Items["PortalSettings"] != null)
                currentSetting = (PortalSettings)HttpContext.Current.Items["PortalSettings"];

            if (culture == null)
            {
                if (currentSetting != null)
                    culture = currentSetting.PortalContentLanguage;
                else
                    culture = Thread.CurrentThread.CurrentUICulture;
            }

            if (currentAlias == null || currentAlias == string.Empty)
            {
                if (currentSetting != null)
                    currentAlias = currentSetting.PortalAlias;
                else
                    //jes1111 - currentAlias = ConfigurationSettings.AppSettings["DefaultPortal"];
                    currentAlias = Config.DefaultPortal;
            }
            // prepare for additional querystring values
            string completeCustomAttributes = customAttributes;

            /*

			// Start of John Mandia's UrlBuilder Enhancement - Uncomment to test (see history for details)
			// prepare the customAttributes so that they may include any additional existing parameters
			
			// get the current tab id
			int currentTabID = 0;
			if (HttpContext.Current.Request.Params["tabID"] != null)
				currentTabID = Int32.Parse(HttpContext.Current.Request.Params["tabID"]);

			if(tabID == currentTabID)
			{
				// this link is being generated for the current page the user is on
				foreach(string name in HttpContext.Current.Request.QueryString)
				{
					if((HttpContext.Current.Request.QueryString[ name ].Length != 0) && (HttpContext.Current.Request.QueryString[ name ] != null) && (name != null))
					{
							// do not add any of the common parameters
							if((name.ToLower() !="tabid") && (name.ToLower() != "mid") && (name.ToLower() != "alias") && (name.ToLower() != "lang") && (name.ToLower() != "returntabid") && (name != null))
							{
								if(!(customAttributes.ToLower().IndexOf(name.ToLower()+"=")> -1))
								{
									completeCustomAttributes += "&" + name + "=" + HttpContext.Current.Request.QueryString[ name ];
								}						
							}
					}
				}
			}
			
			*/

            return
                provider.BuildUrl(targetPage, pageID, modID, culture, completeCustomAttributes, currentAlias,
                                  urlKeywords);
        }

        /// <summary>
        /// Gets the default page.
        /// </summary>
        /// <value>The default page.</value>
        public static string DefaultPage
        {
            get
            {
                //UrlBuilderProvider provider = UrlBuilderProvider.Instance();
                return provider.DefaultPage;
            }
        }

        /// <summary>
        /// Gets the default splitter.
        /// </summary>
        /// <value>The default splitter.</value>
        public static string DefaultSplitter
        {
            get
            {
                //UrlBuilderProvider provider = UrlBuilderProvider.Instance();
                return provider.DefaultSplitter;
            }
        }

        /// <summary>
        /// Webs the path combine.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        [Obsolete("Please use the new Appleseed.Framework.Settings.Path.WebPathCombine()")]
        public static string WebPathCombine(params string[] values)
        {
            return Path.WebPathCombine(values);
        }

        /// <summary>
        /// Builds the url for get to current portal home page
        /// containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static string UrlKeyword(int pageID)
        {
            //UrlBuilderProvider provider = UrlBuilderProvider.Instance();
            return provider.UrlKeyword(pageID);
        }

        /// <summary>
        /// Returns the page name that has been specified.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static string UrlPageName(int pageID)
        {
            //UrlBuilderProvider provider = UrlBuilderProvider.Instance();
            return provider.UrlPageName(pageID);
        }

        /// <summary>
        /// 2_aug_2004 Cory Isakson enhancement
        /// Determines if a tab is simply a placeholder in the navigation
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns>
        /// 	<c>true</c> if the specified page ID is placeholder; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPlaceholder(int pageID)
        {
            //UrlBuilderProvider provider = UrlBuilderProvider.Instance();
            return provider.IsPlaceholder(pageID);
        }

        /// <summary>
        /// 2_aug_2004 Cory Isakson enhancement
        /// Returns the URL for a tab that is a link only.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static string TabLink(int pageID)
        {
            //UrlBuilderProvider provider = UrlBuilderProvider.Instance();
            return provider.TabLink(pageID);
        }

        /// <summary>
        /// Clears any Url Elements e.g IsPlaceHolder, TabLink, UrlKeywords and PageName etc
        /// that may be stored (either in cache, xml etc depending on the UrlBuilder implementation
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        public static void Clear(int pageID)
        {
            provider.Clear(pageID);
        }
    }
}