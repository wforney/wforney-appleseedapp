using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Appleseed.Framework.Settings.Cache;
using System.Threading;

namespace Appleseed.Framework.Providers.Geographic {

    public class Country {

        #region Ctors

        public Country() {
            this.countryID = string.Empty;
            this.neutralName = string.Empty;
            this.administrativeDivisionNeutralName = string.Empty;
        }

        public Country( string countryID, string neutralName, string administrativeDivision ) {
            this.countryID = countryID;
            this.neutralName = neutralName;
            this.administrativeDivisionNeutralName = administrativeDivision;
        }

        #endregion

        private string countryID;
        private string neutralName;
        private string administrativeDivisionNeutralName;

        /// <summary>
        /// Gets or sets the country ID.
        /// </summary>
        /// <value>The country ID.</value>
        public string CountryID {
            get { return countryID; }
            set { countryID = value; }
        }

        /// <summary>
        /// Gets or sets the neutral name.
        /// </summary>
        /// <value>The name of the neutral.</value>
        public string NeutralName {
            get { return neutralName; }
            set { neutralName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the administrative division neutral name.
        /// </summary>
        /// <value>The name of the administrative division neutral.</value>
        public string AdministrativeDivisionNeutralName {
            get { return administrativeDivisionNeutralName; }
            set { administrativeDivisionNeutralName = value; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name {
            get {
                return GeographicProvider.Current.GetCountryDisplayName( countryID, Thread.CurrentThread.CurrentCulture );
            }
        }

        /// <summary>
        /// Gets the name of the administrative division.
        /// </summary>
        /// <value>The name of the administrative division.</value>
        public string AdministrativeDivisionName {
            get {
                return GeographicProvider.Current.GetAdministrativeDivisionName( administrativeDivisionNeutralName, Thread.CurrentThread.CurrentCulture );
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals( object obj ) {
            if ( obj == null || GetType() != obj.GetType() ) {
                return false;
            }

            Country otherCountry = ( Country )obj;
            return ( this.countryID == otherCountry.countryID ) &&
                ( this.neutralName == otherCountry.neutralName ) &&
                ( this.administrativeDivisionNeutralName == otherCountry.administrativeDivisionNeutralName );
        }
    }
}
