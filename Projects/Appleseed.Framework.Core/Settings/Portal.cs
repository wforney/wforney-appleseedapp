using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using Appleseed.Context;

namespace Appleseed.Framework.Settings
{
    /// <summary>
    /// This class contains useful information for Extension, Module and Core Developers.
    /// </summary>
    public sealed class Portal
    {
        private static Context.Reader context = new Context.Reader(new WebContextReader());

        /// <summary>
        /// Sets reader for context in this class
        /// </summary>
        /// <param name="reader">an instance of a Concrete Strategy Reader</param>
        public static void SetReader(Context.Reader reader)
        {
            context = reader;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        private Portal()
        {
        }

        /// <summary>
        /// Gets the code version.
        /// </summary>
        /// <value>The code version.</value>
        public static int CodeVersion
        {
            get
            {
                if (context.Current != null && context.Current.Application["CodeVersion"] != null)
                    return (int) context.Current.Application["CodeVersion"];
                else
                    return 0;
            }
        }

        /// <summary>
        /// Gets the page ID.
        /// </summary>
        /// <value>The page ID.</value>
        public static int PageID
        {
            get
            {
                string strPageID = null;

                if (FindPageIdFromQueryString(context.Current.Request.QueryString, ref strPageID))
                    return Config.GetIntegerFromString(false, strPageID, 0);
                else
                    return 0;
            }
        }

        /// <summary>
        /// This static string fetches the site's alias either via querystring, cookie or domain and returns it
        /// </summary>
        /// <value>The unique ID.</value>
        public static string UniqueID
        {
            // new version - Jes1111 - 07/07/2005
            get
            {
                if (context.Current.Items["PortalAlias"] == null) // not already in context
                {
                    string uniquePortalID = Config.DefaultPortal; // set default value

                    FindAlias(context.Current.Request, ref uniquePortalID); // will change uniquePortalID if it can

                    context.Current.Items.Add("PortalAlias", uniquePortalID); // add to context

                    return uniquePortalID; // return current value
                }
                else // already in context
                {
                    return (string) context.Current.Items["PortalAlias"]; // return from context
                }
            }
        }

        /// <summary>
        /// Finds the alias.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="alias">The alias.</param>
        private static void FindAlias(HttpRequest request, ref string alias)
        {
            if (FindAliasFromQueryString(request.QueryString, ref alias))
            {
                return;
            }
            else if (FindAliasFromCookies(request.Cookies, ref alias))
            {
                return;
            }
            else
            {
                FindAliasFromUri(request.Url, ref alias, Config.DefaultPortal, Config.RemoveDomainPrefixes, Config.RemoveTLD,
                                 Config.SecondLevelDomains, Config.DomainPrefixes, Config.ForceFullRemoving);
                return;
            }
        }

        /// <summary>
        /// Finds the alias from cookies.
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static bool FindAliasFromCookies(HttpCookieCollection cookies, ref string alias)
        {
            if (cookies["PortalAlias"] != null)
            {
                string cookieValue = cookies["PortalAlias"].Value.Trim().ToLower(CultureInfo.InvariantCulture);
                if (cookieValue.Length != 0)
                {
                    alias = cookieValue;
                    return true;
                }
                else
                {
                    //ErrorHandler.Publish(Appleseed.Framework.LogLevel.Warn, "FindAliasFromCookies failed - PortalAlias found but value was empty.");
                    return false;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// Finds the alias from query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static bool FindAliasFromQueryString(NameValueCollection queryString, ref string alias)
        {
            if (queryString != null)
            {
                if (queryString["Alias"] != null)
                {
                    string[] queryStringValues = queryString.GetValues("Alias");
                    string queryStringValue = string.Empty;

                    if (queryStringValues.Length > 0)
                        queryStringValue = queryStringValues[0].Trim().ToLower(CultureInfo.InvariantCulture);

                    if (queryStringValue.Length != 0)
                    {
                        alias = queryStringValue;
                        return true;
                    }
                    else
                    {
                        //ErrorHandler.Publish(Appleseed.Framework.LogLevel.Warn, "FindAliasFromQueryString failed - Alias param found but value was empty.");
                        return false;
                    }
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Finds the page id from query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public static bool FindPageIdFromQueryString(NameValueCollection queryString, ref string pageID)
        {
            string[] queryStringValues;
            // tabID = 240
            if (queryString != null)
            {
                if (queryString[GlobalInternalStrings.str_PageID] != null)
                {
                    queryStringValues = queryString.GetValues(GlobalInternalStrings.str_PageID);
                }
                else if (queryString[GlobalInternalStrings.str_TabID] != null)
                {
                    queryStringValues = queryString.GetValues(GlobalInternalStrings.str_TabID);
                }
                else
                {
                    return false;
                }

                string queryStringValue = string.Empty;

                if (queryStringValues != null && queryStringValues.Length > 0)
                    queryStringValue = queryStringValues[0].Trim().ToLower(CultureInfo.InvariantCulture);

                if (queryStringValue.Length != 0)
                {
                    pageID = queryStringValue;
                    return true;
                }
                else
                {
                    //ErrorHandler.Publish(Appleseed.Framework.LogLevel.Warn, "FindPageIDFromQueryString failed - Alias param found but value was empty.");
                    return false;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// Finds the alias from URI.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="defaultPortal">The default portal.</param>
        /// <param name="removeWWW">if set to <c>true</c> [remove WWW].</param>
        /// <param name="removeTLD">if set to <c>true</c> [remove TLD].</param>
        /// <param name="secondLevelDomains">The second level domains.</param>
        /// <returns></returns>
        public static bool FindAliasFromUri(Uri requestUri, ref string alias, string defaultPortal, bool removeDomainPrefixes,
                                            bool removeTLD, string secondLevelDomains, string domainPrefixes, bool forceFullRemoving)
        {
            // if request is to localhost, return default portal 
            if (requestUri.IsLoopback) {
                alias = defaultPortal;

                return true;

            } else if (requestUri.HostNameType == UriHostNameType.Dns) // get it from hostname
            {
                char[] hostDelim = new char[] { '.' };

                // step 1: split hostname into parts
                ArrayList hostPartsList = new ArrayList(requestUri.Host.Split(hostDelim));
                ArrayList prefixes = new ArrayList(domainPrefixes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                ArrayList gTLDs = new ArrayList(secondLevelDomains.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

                if (forceFullRemoving) {
                    //Saco todo 
                    alias = string.Empty;
                    foreach (string s in hostPartsList){
                        if (!((prefixes.Contains(s) && removeDomainPrefixes)  || (gTLDs.Contains(s) && removeTLD ))) {
                            alias += s;
                        }
                    }
                } else {

                    // step 2: do we need to remove "www"?
                    if (removeDomainPrefixes && prefixes.Contains(hostPartsList[0].ToString())) {
                        hostPartsList.RemoveAt(0);
                    }

                    // step 3: do we need to remove TLD?
                    if (removeTLD) {
                        hostPartsList.Reverse();
                        if (hostPartsList.Count > 2 && hostPartsList[0].ToString().Length == 2) {
                            // this is a ccTLD, so need to check if next segment is a pseudo-gTLD                            
                            if (gTLDs.Contains(hostPartsList[1].ToString()))
                                hostPartsList.RemoveRange(0, 2);
                            else
                                hostPartsList.RemoveAt(0);
                        } else {
                            hostPartsList.RemoveAt(0);
                        }
                        hostPartsList.Reverse();
                    }

                    // step 4: re-assemble the remaining parts
                    alias = String.Join(".", (string[])hostPartsList.ToArray(typeof(String)));
                }

                return true;
            } else {
                alias = defaultPortal;
                return true;
            }
        }

        /// <summary>
        /// Database connection
        /// </summary>
        /// <value>The connection string.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.ConnectionString")]
        public static string ConnectionString
        {
            get { return Config.ConnectionString; }
        }

        /// <summary>
        /// SmtpServer
        /// </summary>
        /// <value>The SMTP server.</value>
        [Obsolete("Please use Appleseed.Framework.Settings.Config.SmtpServer")]
        public static string SmtpServer
        {
            get { return Config.SmtpServer; }
        }
    }
}