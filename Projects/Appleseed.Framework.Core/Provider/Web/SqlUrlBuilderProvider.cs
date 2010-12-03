// Created by John Mandia (john.mandia@whitelightsolutions.com)
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Web;

namespace Appleseed.Framework.Web
{
    /// <summary>
    /// Appleseed standard implementation.
    /// This code has been developed and extended by John Mandia (www.whitelightsolutions.com), 
    /// Manu (www.duemetri.com), Jes (www.marinateq.com) and Cory.
    /// </summary>
    public class SqlUrlBuilderProvider : UrlBuilderProvider
    {
        private string _defaultSplitter = "__";
        private string _handlerFlag = string.Empty;
        private bool _aliasInUrl = false;
        private bool _langInUrl = false;
        private string _ignoreTargetPage = "tablayout.aspx";
        private double _cacheMinutes = 5;
        private bool _pageidNoSplitter = false;
        private string _friendlyPageName = "default.aspx";

        /// <summary> 
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        /// containing the application path, portal alias, tab ID, and language. 
        /// </summary> 
        /// <param name="targetPage">Linked page</param> 
        /// <param name="pageID">ID of the page</param> 
        /// <param name="modID">ID of the module</param> 
        /// <param name="culture">Client culture</param> 
        /// <param name="customAttributes">Any custom attribute that can be needed. Use the following format...single attribute: paramname--paramvalue . Multiple attributes: paramname--paramvalue/paramname2--paramvalue2/paramname3--paramvalue3 </param> 
        /// <param name="currentAlias">Current Alias</param> 
        /// <param name="urlKeywords">Add some keywords to uniquely identify this tab. Usual source is UrlKeyword from TabSettings.</param> 
        public override string BuildUrl(string targetPage, int pageID, int modID, CultureInfo culture,
                                        string customAttributes, string currentAlias, string urlKeywords)
        {
            bool _isPlaceHolder = false;
            string _tabLink = string.Empty;
            string _urlKeywords = string.Empty;
            string _pageName = _friendlyPageName;

            // Get Url Elements this helper method (Will either retrieve from cache or database)
            UrlBuilderHelper.GetUrlElements(pageID, _cacheMinutes, ref _isPlaceHolder, ref _tabLink, ref _urlKeywords,
                                            ref _pageName);

            //2_aug_2004 Cory Isakson
            //Begin Navigation Enhancements
            if (!(targetPage.ToLower().EndsWith(_ignoreTargetPage.ToLower())))
                // Do not modify URLs when working with TabLayout Administration Page
            {
                // if it is a placeholder it is not supposed to have any url
                if (_isPlaceHolder) return string.Empty;

                // if it is a tab link it means it is a link to an external resource
                if (_tabLink.Length != 0) return _tabLink;
            }
            //End Navigation Enhancements
            StringBuilder sb = new StringBuilder();

            // Obtain ApplicationPath
            if (targetPage.StartsWith("~/"))
            {
                sb.Append(UrlBuilderHelper.ApplicationPath);
                targetPage = targetPage.Substring(2);
            }
            sb.Append("/");

            if (!targetPage.EndsWith(".aspx")) //Images
            {
                sb.Append(targetPage);
                return sb.ToString();
            }

            HttpContext.Current.Trace.Warn("Target Page = " + targetPage);

            // Separate path
            // If page contains path, or it is not an aspx 
            // or handlerFlag is not set: do not use handler
            if (targetPage.LastIndexOf('/') > 0 || !targetPage.EndsWith(".aspx") || _handlerFlag.Length == 0)
            {
                sb.Append(targetPage);
                sb.Append("?");
                // Add pageID to URL
                sb.Append("pageID=");
                sb.Append(pageID.ToString());

                // Add Alias to URL
                if (_aliasInUrl)
                {
                    sb.Append("&alias="); // changed for compatibility with handler
                    sb.Append(currentAlias);
                }

                // Add ModID to URL
                if (modID > 0)
                {
                    sb.Append("&mid=");
                    sb.Append(modID.ToString());
                }

                // Add Language to URL
                if (_langInUrl)
                {
                    sb.Append("&lang="); // changed for compatibility with handler
                    sb.Append(culture.Name); // manu fix: culture.Name
                }

                // Add custom attributes
                if (customAttributes != null && customAttributes != string.Empty)
                {
                    sb.Append("&");
                    customAttributes = customAttributes.ToString().Replace("/", "&");
                    customAttributes = customAttributes.ToString().Replace(_defaultSplitter, "=");
                    sb.Append(customAttributes);
                }
                return sb.ToString().Replace("&&", "&");
            }
            else // use handler
            {
                // Add smarturl tag
                sb.Append(_handlerFlag);
                sb.Append("/");

                // Add custom Keywords to the Url
                if (urlKeywords != null && urlKeywords != string.Empty)
                {
                    sb.Append(urlKeywords);
                    sb.Append("/");
                }
                else
                {
                    urlKeywords = _urlKeywords;

                    // Add custom Keywords to the Url
                    if (urlKeywords != null && urlKeywords.Length != 0)
                    {
                        sb.Append(urlKeywords);
                        sb.Append("/");
                    }
                }

                // Add Alias to URL
                if (_aliasInUrl)
                {
                    sb.Append("alias");
                    sb.Append(_defaultSplitter + currentAlias);
                    sb.Append("/");
                }

                // Add Language to URL
                if (_langInUrl)
                {
                    sb.Append("lang");
                    sb.Append(_defaultSplitter + culture.Name);
                    sb.Append("/");
                }
                // Add ModID to URL
                if (modID > 0)
                {
                    sb.Append("mid");
                    sb.Append(_defaultSplitter + modID.ToString());
                    sb.Append("/");
                }

                // Add custom attributes
                if (customAttributes != null && customAttributes != string.Empty)
                {
                    customAttributes = customAttributes.ToString().Replace("&", "/");
                    customAttributes = customAttributes.ToString().Replace("=", _defaultSplitter);
                    sb.Append(customAttributes);
                    sb.Append("/");
                }

                if (_pageidNoSplitter)
                {
                    // Add pageID to URL
                    sb.Append( "pageid" );
                    sb.Append( _defaultSplitter + pageID );
                    sb.Append( "/" );
                }
                else
                {
                    sb.Append( pageID );
                    sb.Append( "/" );
                }

                // TODO : Need to fix page names rewrites
                // if (targetPage == DefaultPage)
                //		sb.Append(_pageName);
                //	else
                //		sb.Append(targetPage);
                sb.Append( _friendlyPageName );

                //Return page
                return sb.ToString().Replace("//", "/");
            }
        }

        /// <summary>
        /// The initialize method lets you retrieve provider specific settings from web.config
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configValue"></param>
        public override void Initialize(string name, NameValueCollection configValue)
        {

            base.Initialize( name, configValue );

            // For legacy support first check provider settings then web.config/Appleseed.config legacy settings
            if (configValue["handlersplitter"] != null)
            {
                _defaultSplitter = configValue["handlersplitter"].ToString();
            }
            else
            {
                if (ConfigurationManager.AppSettings["HandlerDefaultSplitter"] != null)
                    _defaultSplitter = ConfigurationManager.AppSettings["HandlerDefaultSplitter"];
            }

            // For legacy support first check provider settings then web.config/Appleseed.config legacy settings
            if (configValue["handlerflag"] != null)
            {
                _handlerFlag = configValue["handlerflag"].ToString();
            }
            else
            {
                if (ConfigurationManager.AppSettings["HandlerFlag"] != null)
                    _handlerFlag = ConfigurationManager.AppSettings["HandlerFlag"];
            }

            // For legacy support first check provider settings then web.config/Appleseed.config legacy settings
            if (configValue["aliasinurl"] != null)
            {
                _aliasInUrl = bool.Parse(configValue["aliasinurl"].ToString());
            }
            else
            {
                if (ConfigurationManager.AppSettings["UseAlias"] != null)
                    _aliasInUrl = bool.Parse(ConfigurationManager.AppSettings["UseAlias"]);
            }

            // For legacy support first check provider settings then web.config/Appleseed.config legacy settings
            if (configValue["langinurl"] != null)
            {
                _langInUrl = bool.Parse(configValue["langinurl"].ToString());
            }
            else
            {
                if (ConfigurationManager.AppSettings["LangInURL"] != null)
                    _langInUrl = bool.Parse(ConfigurationManager.AppSettings["LangInURL"]);
            }

            if (configValue["ignoretargetpage"] != null)
            {
                _ignoreTargetPage = configValue["ignoretargetpage"].ToString();
            }

            if (configValue["cacheminutes"] != null)
            {
                _cacheMinutes = Convert.ToDouble(configValue["cacheminutes"].ToString());
            }

            if (configValue["pageidnosplitter"] != null)
            {
                _pageidNoSplitter = bool.Parse(configValue["pageidnosplitter"].ToString());
            }
            else {
                if ( ConfigurationManager.AppSettings[ "PageIdNoSplitter" ] != null )
                    _pageidNoSplitter = bool.Parse( ConfigurationManager.AppSettings[ "PageIdNoSplitter" ] );
            }

            // For legacy support first check provider settings then web.config/Appleseed.config legacy settings
            if ( configValue[ "friendlypagename" ] != null ) {
                // TODO: Friendly url's need to be fixed
                _friendlyPageName = configValue[ "friendlypagename" ].ToString();
            }
            else {
                if ( ConfigurationManager.AppSettings[ "FriendlyPageName" ] != null )
                    _friendlyPageName = ConfigurationManager.AppSettings[ "FriendlyPageName" ];
            }
        }

        /// <summary> 
        /// Determines if a tab is simply a placeholder in the navigation
        /// </summary> 
        public override bool IsPlaceholder(int pageID)
        {
            return
                bool.Parse(
                    UrlBuilderHelper.PageSpecificProperty(pageID, UrlBuilderHelper.IsPlaceHolderID, _cacheMinutes).
                        ToString());
        }

        /// <summary> 
        /// Returns the URL for a tab that is a link only.
        /// </summary> 
        public override string TabLink(int pageID)
        {
            return UrlBuilderHelper.PageSpecificProperty(pageID, UrlBuilderHelper.TabLinkID, _cacheMinutes);
        }

        /// <summary> 
        /// Returns any keywords which are meant to be placed in the url
        /// </summary> 
        public override string UrlKeyword(int pageID)
        {
            return
                UrlBuilderHelper.PageSpecificProperty(pageID, UrlBuilderHelper.UrlKeywordsID, _cacheMinutes).ToString();
        }

        /// <summary> 
        /// Returns the page name that has been specified. 
        /// </summary> 
        public override string UrlPageName(int pageID)
        {
            string _urlPageName =
                UrlBuilderHelper.PageSpecificProperty(pageID, UrlBuilderHelper.PageNameID, _cacheMinutes).ToString();
            // TODO: URL Firendly names need to be fixed
            if (_urlPageName.Length == 0)
                _urlPageName = _friendlyPageName;

            return _urlPageName;
        }

        /// <summary>
        /// Gets the default page from web.config/Appleseed.config
        /// </summary>
        public override string DefaultPage
        {
            get
            {
                // TODO: Jes1111 - check this with John
                //string strTemp = ConfigurationSettings.AppSettings["HandlerTargetUrl"];

                // TODO : JONATHAN - PROBLEM WITH DEFAULT PAGE LIKE THIS
                string strTemp = _friendlyPageName;
                // TODO : JONATHAN - PROBLEM WITH DEFAULT PAGE LIKE THIS
                if (strTemp.Length == 0 || strTemp == null)
                    strTemp = "Default.aspx";

                return strTemp;
            }
        }

        /// <summary>
        /// Returns the default paramater splitter from provider settings (or web.config/Appleseed.config if not specified in provider) 
        /// </summary>
        public override string DefaultSplitter
        {
            get { return _defaultSplitter; }
        }

        /// <summary> 
        /// Clears the cached url element settings
        /// </summary> 
        public override void Clear(int pageID)
        {
            UrlBuilderHelper.ClearUrlElements(pageID);
        }
    }
}