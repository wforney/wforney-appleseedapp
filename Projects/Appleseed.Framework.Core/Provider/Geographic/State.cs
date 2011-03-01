using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Appleseed.Framework.Providers.Geographic {

    public class State {

        private int stateID;
        private string neutralName;
        private string countryID;

        /// <summary>
        /// Gets or sets the state ID.
        /// </summary>
        /// <value>The state ID.</value>
        public int StateID {
            get { return stateID; }
            set { stateID = value; }
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
        /// Gets or sets the country ID.
        /// </summary>
        /// <value>The country ID.</value>
        public string CountryID {
            get { return countryID; }
            set { countryID = value; }
        }

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        public State() {
            this.stateID = 0;
            this.countryID = string.Empty;
            this.neutralName = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        /// <param name="stateID">The state ID.</param>
        /// <param name="countryID">The country ID.</param>
        /// <param name="neutralName">Neutral Name.</param>
        public State( int stateID, string countryID, string neutralName ) {
            this.stateID = stateID;
            this.countryID = countryID;
            this.neutralName = neutralName;
        }

        #endregion

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name {
            get {
                return GeographicProvider.Current.GetStateDisplayName( stateID, Thread.CurrentThread.CurrentCulture );
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

            State otherState = ( State )obj;
            return ( this.countryID == otherState.countryID ) &&
                ( this.neutralName == otherState.neutralName ) &&
                ( this.stateID == otherState.stateID );
        }
    }
}
