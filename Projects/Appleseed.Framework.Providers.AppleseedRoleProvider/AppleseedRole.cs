using System;
using System.Collections.Generic;
using System.Text;

namespace Appleseed.Framework.Providers.AppleseedRoleProvider {

    public class AppleseedRole : IComparable {

        public AppleseedRole( Guid roleId, string roleName ) {
            this.id = roleId;
            this.name = roleName;
            this.description = string.Empty;
        }

        public AppleseedRole( Guid roleId, string roleName, string roleDescription ) {
            this.id = roleId;
            this.name = roleName;
            this.description = roleDescription;
        }

        private Guid id;

        public Guid Id {
            get {
                return id;
            }
            set {
                id = value;
            }
        }

        private string name;

        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        private string description;

        public string Description {
            get {
                return description;
            }
            set {
                description = value;
            }
        }

        public override bool Equals( object obj ) {
            //Check for null and compare run-time types.
            if ( obj == null || GetType() != obj.GetType() ) {
                return false;
            }

            AppleseedRole role = ( AppleseedRole )obj;
            return ( id == role.id ) && ( name == role.name );
        }

        #region IComparable Members

        public int CompareTo( object obj ) {
            if ( obj is AppleseedRole ) {
                AppleseedRole role = ( AppleseedRole )obj;
                return name.CompareTo( role.name );
            }
            throw new ArgumentException( "object is not a AppleseedRole" );    
        }

        #endregion
    }
}
