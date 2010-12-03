using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;

namespace Appleseed.Framework.Providers.AppleseedRoleProvider {

    [global::System.Serializable]
    public class AppleseedRoleProviderException : ProviderException {

        public AppleseedRoleProviderException() {
        }

        public AppleseedRoleProviderException( string message )
            : base( message ) {
        }

        public AppleseedRoleProviderException( string message, Exception inner )
            : base( message, inner ) {
        }

        protected AppleseedRoleProviderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context )
            : base( info, context ) {
        }
    }
}
