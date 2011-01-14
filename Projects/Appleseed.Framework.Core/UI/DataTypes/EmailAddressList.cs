using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// This implementation of the ArrayList class, allows only valid 
    /// emailadresses to be added to the collection
    /// </summary>
    public class EmailAddressList : ArrayList
    {
        private static Regex _regex;

        /// <summary>
        /// Gets the emailaddress.
        /// </summary>
        /// <value>The emailaddress.</value>
        private static Regex emailaddress
        {
            get
            {
                if (_regex == null)
                    _regex =
                        new Regex("^([a-zA-Z0-9_\\.\\-])+\\@(([a-zA-Z0-9\\-])+\\.)+([a-zA-Z0-9]{2,4})+$",
                                  RegexOptions.Compiled & RegexOptions.IgnoreCase);

                return _regex;
            }
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="T:System.Collections.ArrayList"></see>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"></see> to be added to the end of the <see cref="T:System.Collections.ArrayList"></see>. The value can be null.</param>
        /// <returns>
        /// The <see cref="T:System.Collections.ArrayList"></see> index at which the value has been added.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.ArrayList"></see> is read-only.-or- The <see cref="T:System.Collections.ArrayList"></see> has a fixed size. </exception>
        public override int Add(object value)
        {
            // Check if the value isn't null
            if (value == null)
                throw new ArgumentNullException("value", "You can not add null email-addresses to the collection.");
            // Check if the value is a string
            if (! (value is string))
                throw new ArgumentOutOfRangeException("value", "Only string values are allowed.");
            // Check if the value can be matched to the regular expression
            if (! emailaddress.IsMatch((string) value))
                throw new ArgumentException("value", "Only valid email-addresses are allowed.");
            // This is a valid email address
            return base.Add(value);
        }
    }
}