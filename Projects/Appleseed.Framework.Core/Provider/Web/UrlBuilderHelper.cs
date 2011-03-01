// Created by John Mandia (john.mandia@whitelightsolutions.com)
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using Appleseed.Framework.Settings;

namespace Appleseed.Framework.Web
{
	/// <summary>
	/// Summary description for Helper.
	/// </summary>
	internal sealed class UrlBuilderHelper
	{
		/// <summary>
		///     ctor
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		private UrlBuilderHelper()
		{
		}

		public const string IsPlaceHolderID = "TabPlaceholder";
		public const string TabLinkID = "TabLink";
		public const string PageNameID = "UrlPageName";
		public const string UrlKeywordsID = "TabUrlKeyword";

		/// <summary>
		/// Builds up a cache key for Url Elements/Properties
		/// </summary>
		/// <param name="pageID">The ID of the page for which you want to generate a url element cache key for</param>
		/// <param name="UrlElement">The Url element you are after (IsPlaceHolderID/TabLinkID/PageNameID/UrlKeywordsID) constants</param>
		/// <returns>A unique key</returns>
		private static string UrlElementCacheKey(int pageID, string UrlElement)
		{
			return string.Concat(SiteUniqueID.ToString(), pageID, UrlElement);
		}

		/// <summary>
		/// Clears all cached url elements for a given page
		/// </summary>
		/// <param name="pageID"></param>
		public static void ClearUrlElements(int pageID)
		{
			Cache applicationCache = HttpContext.Current.Cache;

			string placeHolderCacheKey = UrlElementCacheKey(pageID, IsPlaceHolderID);
			string tabLinkCacheKey = UrlElementCacheKey(pageID, TabLinkID);
			string pageNameCacheKey = UrlElementCacheKey(pageID, PageNameID);
			string urlKeywordsCacheKey = UrlElementCacheKey(pageID, UrlKeywordsID);

			if (applicationCache[placeHolderCacheKey] != null)
				applicationCache.Remove(placeHolderCacheKey);

			if (applicationCache[tabLinkCacheKey] != null)
				applicationCache.Remove(tabLinkCacheKey);

			if (applicationCache[pageNameCacheKey] != null)
				applicationCache.Remove(pageNameCacheKey);

			if (applicationCache[urlKeywordsCacheKey] != null)
				applicationCache.Remove(urlKeywordsCacheKey);
		}

		/// <summary>
		/// This method is used to retrieve a specific Url Property
		/// </summary>
		/// <param name="pageID">The ID of the page the Url belongs to</param>
		/// <param name="propertyID">The ID of the property you are interested in</param>
		/// <param name="cacheDuration">The number of minutes you want to cache this returned value for</param>
		/// <returns>A string value representing the property you are interested in</returns>
		public static string PageSpecificProperty(int pageID, string propertyID, double cacheDuration)
		{
			// Page 0 is shared across portals as a default setting (It doesn't have any real data associated with it so return defaults);
			if (pageID == 0)
			{
				if (propertyID == IsPlaceHolderID)
				{
					return "False";
				}
				else
				{
					return string.Empty;
				}
			}

			// get the unique cache key for the property requested
			string uniquePropertyCacheKey = UrlElementCacheKey(pageID, propertyID);

			// calling HttpContext.Current.Cache all the time incurs a small performance hit so get a reference to it once and reuse that for greater performance
			Cache applicationCache = HttpContext.Current.Cache;

			if (applicationCache[uniquePropertyCacheKey] == null)
			{
				string property = string.Empty;

				using (SqlConnection conn = new SqlConnection(SiteConnectionString))
				{
					try
					{
						// Open the connection
						conn.Open();

						using (SqlCommand cmd = new SqlCommand("SELECT SettingValue FROM rb_TabSettings WHERE TabID=" + pageID.ToString() + " AND SettingName = '" + propertyID + "'", conn))
						{
							// 1. Instantiate a new command above
							// 2. Call ExecuteNonQuery to send command
							property = (string) cmd.ExecuteScalar();
						}
					}

					catch
					{
						// TODO: Decide whether or not this should be logged. If it is a large site upgrading then it would quickly fill up a log file.
						// If there is no value in the database then it thows an error as it is expecting something.
						// This can happen with the initial setup or if no entries for a tab have been made
					}

					finally
					{
						// Close the connection
						if (conn != null)
							conn.Close();
					}
				}

				// if null is returned always ensure that either a bool (if it is a TabPlaceholder) or an empty string is returned.
				if ((property == null) || (property.Length == 0))
				{
					// Check to make sure it is not a placeholder...if it is change it to false otherwise ensure that it's value is ""
					if (propertyID == IsPlaceHolderID)
						property = "False";

					else
						property = string.Empty;
				}

				else
				{
					// Just check to see that it is cleaned before caching it (i.e. removing illegal characters)
					// If this section grows too much I will clean it up into methods instead of using if else checks.
					if ((propertyID == PageNameID) || (propertyID == UrlKeywordsID))
					{
						// Replace any illegal characters such as space and special characters and replace it with "-"
						property = Regex.Replace(property, @"[^A-Za-z0-9]", "-");
						if (propertyID == PageNameID)
							property += ".aspx";
					}
				}

				// NOTE: Below you will see an implementation that has been commented out as it didn't seem to work well with the tabsetting cache dependency and always retrieved it again and again.
				//       If someone can figure out why it cant see the cached value please apply the fix and switch the implementation back as it is more ideal (would allow users to see their changes straight away)

				// If this changes it means that the tabsettings have changed which means the urlkeyword, tablink or placeholder status has changed

				// String[] dependencyKey = new String[1];
				// dependencyKey[0] = Appleseed.Framework.Settings.Cache.Key.TabSettings(pageID);
				// applicationCache.Insert(uniquePropertyCacheKey, property, new CacheDependency(null, dependencyKey));

				if (cacheDuration == 0)
				{
					applicationCache.Insert(uniquePropertyCacheKey, property);
				}
				else
				{
					applicationCache.Insert(uniquePropertyCacheKey, property, null, DateTime.Now.AddMinutes(cacheDuration), Cache.NoSlidingExpiration);
				}
				return property;
			}

			else
				return applicationCache[uniquePropertyCacheKey].ToString();
		}

		/// <summary>
		/// This method is used to get all Url Elements in one go
		/// </summary>
		/// <param name="pageID">The ID of the page you are interested in</param>
		/// <param name="cacheDuration">The length of time these values should be cached once retrieved</param>
		/// <param name="_isPlaceHolder">Is this url a place holder (Not a real url)</param>
		/// <param name="_tabLink">Is this Url a link to an external site/resource</param>
		/// <param name="_urlKeywords">Are there any keywords that should be added to this url</param>
		/// <param name="_pageName">Does this url have a friendly page name other than the default</param>
		public static void GetUrlElements(int pageID, double cacheDuration, ref bool _isPlaceHolder, ref string _tabLink, ref string _urlKeywords, ref string _pageName)
		{
			// pageID 0 is a default page shared across portals with no real settings
			if (pageID == 0)
				return;

			string isPlaceHolderKey = UrlElementCacheKey(pageID, IsPlaceHolderID);
			string tabLinkKey = UrlElementCacheKey(pageID, TabLinkID);
			string pageNameKey = UrlElementCacheKey(pageID, PageNameID);
			string urlKeywordsKey = UrlElementCacheKey(pageID, UrlKeywordsID);

			// calling HttpContext.Current.Cache all the time incurs a small performance hit so get a reference to it once and reuse that for greater performance
			Cache applicationCache = HttpContext.Current.Cache;

			// if any values are null refetch
			if (applicationCache[isPlaceHolderKey] == null || applicationCache[tabLinkKey] == null || applicationCache[pageNameKey] == null || applicationCache[urlKeywordsKey] == null)
			{
				using (SqlConnection conn = new SqlConnection(SiteConnectionString))
				{
					try
					{
						// Open the connection
						conn.Open();

						using (SqlCommand cmd = new SqlCommand("SELECT ISNULL((SELECT SettingValue FROM rb_TabSettings WHERE TabID=" + pageID.ToString() + " AND SettingName = '" + PageNameID + "'),'') as PageName,ISNULL((SELECT SettingValue FROM rb_TabSettings WHERE TabID=" + pageID.ToString() + " AND SettingName = '" + UrlKeywordsID + "'),'') as Keywords,ISNULL((SELECT SettingValue FROM rb_TabSettings WHERE TabID=" + pageID.ToString() + " AND SettingName = '" + TabLinkID + "'),'') as ExternalLink,ISNULL((SELECT SettingValue FROM rb_TabSettings WHERE TabID=" + pageID.ToString() + " AND SettingName = '" + IsPlaceHolderID + "'),'') as IsPlaceHolder", conn))
						{
							// 1. Instantiate a new command above
							// 2. populate values
							SqlDataReader pageElements = cmd.ExecuteReader(CommandBehavior.CloseConnection);
							if (pageElements.HasRows)
							{
								pageElements.Read();

								// NOTE: Below you will see an implementation that has been commented out as it didn't seem to work well with the tabsetting cache dependency and always retrieved it again and again.
								//       If someone can figure out why it cant see the cached value please apply the fix and switch the implementation back as it is more ideal (would allow users to see their changes straight away)

								// If this changes it means that the tabsettings have changed which means the urlkeyword, tablink or placeholder status has changed
								// String[] dependencyKey = new String[1];
								// dependencyKey[0] = Appleseed.Framework.Settings.Cache.Key.TabSettings(pageID);

								if (pageElements["PageName"].ToString() != String.Empty)
								{
									_pageName = Convert.ToString(pageElements["PageName"]);
									_pageName = Regex.Replace(_pageName, @"[^A-Za-z0-9]", "-");
									_pageName += ".aspx";

									// insert value in cache so it doesn't always try to retrieve it

									// NOTE: This is the tabsettings Cache Dependency approach see note above
									// applicationCache.Insert(pageNameKey, _pageName, new CacheDependency(null, dependencyKey));
									if (cacheDuration == 0)
									{
										applicationCache.Insert(pageNameKey, _pageName);
									}
									else
									{
										applicationCache.Insert(pageNameKey, _pageName, null, DateTime.Now.AddMinutes(cacheDuration), Cache.NoSlidingExpiration);
									}
								}
								else
								{
									// insert value in cache so it doesn't always try to retrieve it add empty string so as not to use up too much resources

									// NOTE: This is the tabsettings Cache Dependency approach see note above
									// applicationCache.Insert(pageNameKey, string.Empty, new CacheDependency(null, dependencyKey));
									if (cacheDuration == 0)
									{
										applicationCache.Insert(pageNameKey, string.Empty);
									}
									else
									{
										applicationCache.Insert(pageNameKey, string.Empty, null, DateTime.Now.AddMinutes(cacheDuration), Cache.NoSlidingExpiration);
									}
								}

								if (pageElements["Keywords"].ToString() != String.Empty)
								{
									_urlKeywords = Convert.ToString(pageElements["Keywords"]);
									_urlKeywords = Regex.Replace(_urlKeywords, @"[^A-Za-z0-9]", "-");
								}
								// insert value in cache so it doesn't always try to retrieve it

								// NOTE: This is the tabsettings Cache Dependency approach see note above
								// applicationCache.Insert(urlKeywordsKey, _urlKeywords, new CacheDependency(null, dependencyKey));								

								if (cacheDuration == 0)
								{
									applicationCache.Insert(urlKeywordsKey, _urlKeywords);
								}
								else
								{
									applicationCache.Insert(urlKeywordsKey, _urlKeywords, null, DateTime.Now.AddMinutes(cacheDuration), Cache.NoSlidingExpiration);
								}

								if (pageElements["ExternalLink"].ToString() != String.Empty)
								{
									_tabLink = Convert.ToString(pageElements["ExternalLink"]);
								}
								// insert value in cache so it doesn't always try to retrieve it

								// NOTE: This is the tabsettings Cache Dependency approach see note above
								// applicationCache.Insert(tabLinkKey, _tabLink, new CacheDependency(null, dependencyKey));
								if (cacheDuration == 0)
								{
									applicationCache.Insert(tabLinkKey, _tabLink);
								}
								else
								{
									applicationCache.Insert(tabLinkKey, _tabLink, null, DateTime.Now.AddMinutes(cacheDuration), Cache.NoSlidingExpiration);
								}

								if (pageElements["IsPlaceHolder"].ToString() != String.Empty)
								{
									_isPlaceHolder = bool.Parse(pageElements["IsPlaceHolder"].ToString());
								}
								// insert value in cache so it doesn't always try to retrieve it

								// NOTE: This is the tabsettings Cache Dependency approach see note above
								// applicationCache.Insert(isPlaceHolderKey, _isPlaceHolder.ToString(), new CacheDependency(null, dependencyKey));
								if (cacheDuration == 0)
								{
									applicationCache.Insert(isPlaceHolderKey, _isPlaceHolder.ToString());
								}
								else
								{
									applicationCache.Insert(isPlaceHolderKey, _isPlaceHolder.ToString(), null, DateTime.Now.AddMinutes(cacheDuration), Cache.NoSlidingExpiration);
								}
							}
							// close the reader
							pageElements.Close();
						}
					}
					catch
					{
						// TODO: Decide whether or not this should be logged. If it is a large site upgrading then it would quickly fill up a log file.
						// If there is no value in the database then it thows an error as it is expecting something.
						// This can happen with the initial setup or if no entries for a tab have been made
					}

					finally
					{
						// Close the connection
						if (conn != null)
							conn.Close();
					}
				}
			}
			else
			{
				// if cached value is empty string then leave it as default
				if (applicationCache[pageNameKey].ToString() != String.Empty)
					_pageName = applicationCache[pageNameKey].ToString();

				_urlKeywords = applicationCache[urlKeywordsKey].ToString();
				_tabLink = applicationCache[tabLinkKey].ToString();
				_isPlaceHolder = bool.Parse(applicationCache[isPlaceHolderKey].ToString());
			}
		}


		/// <summary>
		/// ApplicationPath, Application dependent relative Application Path.
		/// Base dir for all portal code
		/// Since it is common for all portals is declared as static
		/// </summary>
		public static string ApplicationPath
		{
			get { return Path.ApplicationRoot; }
		}

		/// <summary>
		///     Returns the current site's database connection string
		/// </summary>
		/// <value>
		///     <para>
		///         
		///     </para>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		private static string SiteConnectionString
		{
			get { return Config.ConnectionString; }
		}

		/// <summary>
		/// This static string fetches the site's alias either via querystring, cookie or domain and returns it
		/// </summary>
		private static string SiteUniqueID
		{
			get { return Portal.UniqueID; }
		}
	}
}