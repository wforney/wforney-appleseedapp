using System;
using System.Globalization;
using System.Net;
using System.Collections;

namespace Appleseed.Framework.Helpers {
    /// <summary>
    /// General utility methods
    /// </summary>
    public class Utilities {

        /// <summary>
        /// Tests a string to ensure that it is equivalent to a valid HTTP Status code
        /// </summary>
        /// <param name="str">the string to be tested</param>
        /// <returns>a boolean value</returns>
        public static bool IsHttpStatusCode( string str ) {
            if ( str == null )
                return false;

            if ( Enum.IsDefined( typeof( HttpStatusCode ), str ) )
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether the specified STR is integer.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>
        /// 	<c>true</c> if the specified STR is integer; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInteger( string str ) {
            int aux;

            return int.TryParse( str, out aux );
        }

        /// <summary>
        /// Intersects the string arrays.
        /// </summary>
        /// <param name="arrayA">The array A.</param>
        /// <param name="arrayB">The array B.</param>
        /// <returns>
        /// An <c>string[]</c> with the elements obtained from intersecting the arrays
        /// </returns>
        public static string[] IntersectArrays( string[] arrayA, string[] arrayB ) {
            ArrayList resultArray = new ArrayList();

            foreach ( object elem1 in arrayA ) {
                foreach ( object elem2 in arrayB ) {
                    if ( elem1.Equals( elem2 ) ) {
                        resultArray.Add( elem1 );
                        break;
                    }
                }
            }

            string[] result = new string[resultArray.Count];
            for ( int i = 0; i < resultArray.Count; i++ ) {
                result[i] = ( string )resultArray[i];
            }

            return result;
        }
    }
}
