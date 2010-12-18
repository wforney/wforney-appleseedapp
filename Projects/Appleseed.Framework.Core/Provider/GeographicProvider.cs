using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using System.Web;
using System.Web.Caching;
using Appleseed.Framework.Provider;
using System.Configuration;
using System.Collections;
using System.Globalization;
using Appleseed.Framework.Helpers;

namespace Appleseed.Framework.Providers.Geographic {

    /// <summary>
    /// Defines the types of countries lists that can be retrieved using Country.GetCountries
    /// </summary>
    [Serializable]
    public enum CountryFields {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// NeutralName
        /// </summary>
        NeutralName,

        /// <summary>
        /// CountryID
        /// </summary>
        CountryID,

        /// <summary>
        /// Name
        /// </summary>
        Name
    }

    #region Country comparers

    internal class CountryNameComparer : IComparer<Country> {

        #region IComparer<Country> Members

        public int Compare( Country x, Country y ) {
            string xName = x.Name;
            string yName = y.Name;
            return new CaseInsensitiveComparer( System.Globalization.CultureInfo.CurrentUICulture ).Compare( xName, yName );
        }

        #endregion
    }

    #endregion

    /// <summary>
    /// Geographic prvodider API 
    /// </summary>
    public abstract class GeographicProvider : ProviderBase {

        /// <summary>
        /// Camel case. Must match web.config section name
        /// </summary>
        private const string providerType = "geographic";

        /// <summary>
        /// filteredCountries attribute from config
        /// </summary>
        protected string countriesFilter = string.Empty;

        /// <summary>
        /// Returns the a string of comma separated country IDs. This list is used when retrieving lists of countries.
        /// </summary>
        public string CountriesFilter {
            get {
                return countriesFilter;
            }
            set {
                countriesFilter = value;
            }
        }

        /// <summary>
        /// Instances this instance.
        /// </summary>
        /// <returns></returns>
        public static GeographicProvider Current {
            get {
                // Get the names of providers
                ProviderConfiguration config = ProviderConfiguration.GetProviderConfiguration( providerType );
                // Read specific configuration information for this provider
                ProviderSettings providerSettings = ( ProviderSettings )config.Providers[config.DefaultProvider];
                // In the cache?
                string cacheKey = "Appleseed::Web::GeographicProvider::" + config.DefaultProvider;

                if ( CurrentCache[cacheKey] == null ) {
                    // The assembly should be in \bin or GAC, so we simply need

                    // to get an instance of the type
                    try {
                        CurrentCache.Insert( cacheKey, ProviderHelper.InstantiateProvider( providerSettings, typeof( GeographicProvider ) ) );
                    }
                    catch ( Exception e ) {
                        throw new Exception( "Unable to load provider", e );
                    }
                }
                return ( GeographicProvider )CurrentCache[cacheKey];
            }
        }

        protected static Cache _currentCache = null;

        /// <summary>
        /// Gets the current cache.
        /// </summary>
        /// <value>The current cache.</value>
        protected static Cache CurrentCache {
            get {
                if ( _currentCache == null ) {
                    if ( System.Web.HttpContext.Current != null ) {
                        _currentCache = System.Web.HttpContext.Current.Cache;
                    }
                    else {
                        // I'm in a test environment
                        System.Web.HttpRuntime r = new System.Web.HttpRuntime();
                        _currentCache = System.Web.HttpRuntime.Cache;
                    }
                }
                return _currentCache;
            }
        }

        /// <summary>
        /// Builds the countries filter.
        /// </summary>
        /// <param name="configFilter">The config filter.</param>
        /// <param name="additionalFilter">The additional filter.</param>
        /// <returns>a string of comma-separated country codes, representing the built filter</returns>
        protected static string BuildCountriesFilter( string configFilter, string additionalFilter ) {
            string[] configFilterArray = configFilter.Trim().Split( ',' );
            string[] additionalFilterArray = additionalFilter.Trim().Split( ',' );
            string[] filterArray = null;

            foreach ( string s in configFilterArray ) {
                if ( ( !s.Equals( string.Empty ) ) && ( s.Length != 2 ) ) {
                    // this is not a valid country code
                    throw new ArgumentException( string.Format( "{0} is not a valid country code", s ) );
                }
            }

            foreach ( string s in additionalFilterArray ) {
                if ( ( !s.Equals( string.Empty ) ) && ( s.Length != 2 ) ) {
                    // this is not a valid country code
                    throw new ArgumentException( string.Format( "{0} is not a valid country code", s ) );
                }
            }

            if ( ( configFilterArray.Length > 1 ) && ( additionalFilterArray.Length > 1 ) ) {
                filterArray = Utilities.IntersectArrays( configFilterArray, additionalFilterArray );
            }
            else if ( configFilterArray.Length > 1 ) {
                filterArray = configFilterArray;
            }
            else if ( additionalFilterArray.Length > 1 ) {
                filterArray = additionalFilterArray;
            }
            else {
                filterArray = new string[0];
            }

            string filter = string.Empty;
            foreach ( string s in filterArray ) {
                filter = filter + s + ",";
            }

            if ( filter.EndsWith( "," ) ) {
                filter = filter.Substring( 0, filter.Length - 1 );
            }

            return filter;
        }


        /// <summary>
        /// Gets the list of countries
        /// </summary>
        /// <returns>a <code>IList<Country></code> containing a list of all countries. 
        /// This method ignores the CountriesFilter property.</returns>
        public abstract IList<Country> GetUnfilteredCountries();

        /// <summary>
        /// Gets the list of countries
        /// </summary>
        /// <returns>a <code>IList<Country></code> containing a list of all countries. 
        /// This method takes into account the CountriesFilter property.</returns>
        public abstract IList<Country> GetCountries();

        /// <summary>
        /// Gets the list of countries sorted by a certain field.
        /// This method takes into account the CountriesFilter property.</returns>
        /// <param name="types">CountryTypes enum value</param>
        /// <returns>The list of countries</returns>
        public abstract IList<Country> GetCountries( CountryFields sortBY );

        /// <summary>
        /// Gets the list of countries filtered by the specified filter.
        /// This method takes into account the CountriesFilter property.</returns>
        /// <param name="filter">A colon separated string of TwoLetterISONames</param>
        /// <exception cref="ArgumentException">If the the filter is not valid (comma-separated two-letter codes)</exception>
        /// <returns>The list of countries</returns>
        public abstract IList<Country> GetCountries( string filter );

        /// <summary>
        /// Gets the list of countries filtered by the specified filter and sorted by a certain field. 
        /// This method takes into account the CountriesFilter property.</returns>
        /// <param name="filter">A colon separated string of TwoLetterISONames</param>
        /// <exception cref="ArgumentException">If the the filter is not valid (comma-separated two-letter codes)</exception>
        /// <returns>The list of countries</returns>
        public abstract IList<Country> GetCountries( string filter, CountryFields sortBY );

        /// <summary>
        /// Returns a list of states for a specified country.
        /// </summary>
        /// <param name="countryID">The country code</param>
        /// <returns>The list of states for the specified country</returns>
        public abstract IList<State> GetCountryStates( string countryID );

        /// <summary>
        /// Gets the country's name in the specified language if available. It not, gets the default one.
        /// </summary>
        /// <param name="countryID">The country's id</param>
        /// <param name="c">a <code>System.Globalization.CultureInfo</code> describing the language we want the name for</param>
        /// <returns>A <code>string</code> containing the localized name.</returns>
        /// <exception cref="CountryNotFoundException">If the country is not found</exception>
        public abstract string GetCountryDisplayName( string countryID, System.Globalization.CultureInfo c );

        /// <summary>
        /// Gets the states's name in the specified language if available. It not, gets the default one.
        /// </summary>
        /// <param name="countryCode">The state's id</param>
        /// <param name="c">a <code>System.Globalization.CultureInfo</code> describing the language we want the name for</param>
        /// <returns>A <code>string</code> containing the localized name.</returns>
        /// <exception cref="StateNotFoundException">If the state is not found</exception>
        public abstract string GetStateDisplayName( int stateID, System.Globalization.CultureInfo c );

        /// <summary>
        /// Gets the administrative division's name in the specified language if available. It not, gets the default one.
        /// </summary>
        /// <param name="countryCode">The administrative division's neutral name</param>
        /// <param name="c">a <code>System.Globalization.CultureInfo</code> describing the language we want the name for</param>
        /// <returns>A <code>string</code> containing the localized name.</returns>
        public abstract string GetAdministrativeDivisionName( string administrativeDivisionName, System.Globalization.CultureInfo c );

        /// <summary>
        /// Returns a <code>Country</code> object
        /// </summary>
        /// <param name="countryID">The country's id</param>
        /// <returns>A <code>Country</code> object containing the country info, or null if the country doesn't exist</returns>
        /// <exception cref="CountryNotFoundException">If the country is not found</exception>
        public abstract Country GetCountry( string countryID );

        /// <summary>
        /// Returns a <code>State</code> object
        /// </summary>
        /// <param name="stateID">The state's id</param>
        /// <returns>A <code>State</code> object containing the State info, or null if the state doesn't exist</returns>
        /// <exception cref="StateNotFoundException">If the state is not found</exception>
        public abstract State GetState( int stateID );

        /// <summary>
        /// Get a Country objects that represents the country of the current thread
        /// </summary>
        public Country CurrentCountry {
            get {
                return GeographicProvider.Current.GetCountry( RegionInfo.CurrentRegion.Name );
            }
        }
    }
}
