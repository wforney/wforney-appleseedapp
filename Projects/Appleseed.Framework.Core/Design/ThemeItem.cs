using System;

namespace Appleseed.Framework.Design
{
    /// <summary>
    /// ThemeItem encapsulates the items of Theme list.
    /// Uses IComparable interface to allow sorting by name.
    /// </summary>
    public class ThemeItem : IComparable
    {
        private string _name;

        /// <summary>
        /// The name of the theme
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A int value...</returns>
        public int CompareTo(object value)
        {
            return CompareTo((object) Name);
        }
    }
}