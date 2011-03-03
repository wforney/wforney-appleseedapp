using System;
using System.Collections.Generic;
using System.Text;

namespace Appleseed.Framework.Providers.Geographic {

    [global::System.Serializable]
    public class CountryNotFoundException : ApplicationException {

        public CountryNotFoundException() { }

        public CountryNotFoundException( string message ) : base( message ) { }

        public CountryNotFoundException( string message, Exception inner ) : base( message, inner ) { }

        protected CountryNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context )
            : base( info, context ) { }
    }
}
