using System;
using System.Collections.Generic;
using System.Text;

namespace Appleseed.Framework.Providers.Geographic {

    [global::System.Serializable]
    public class StateNotFoundException : ApplicationException {

        public StateNotFoundException() { }

        public StateNotFoundException( string message ) : base( message ) { }

        public StateNotFoundException( string message, Exception inner ) : base( message, inner ) { }

        protected StateNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context )
            : base( info, context ) { }
    }
}
