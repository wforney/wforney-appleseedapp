using System;

namespace Appleseed.Framework.Design
{
    /// <summary>
    /// LayoutItem encapsulates the items of Layout list.
    /// Uses IComparable interface to allow sorting by name.
    /// </summary>
    /// <remarks>by Cory Isakson</remarks>
    public class LayoutItem : IComparable
    {
        /// <summary>
        ///     
        /// </summary>
        /// <remarks>
        ///     
        /// </remarks>
        private string _name;

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A int value...</returns>
        public int CompareTo(object value)
        {
            return CompareTo((object) Name);
        }

        /// <summary>
        /// The name of the layout
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}