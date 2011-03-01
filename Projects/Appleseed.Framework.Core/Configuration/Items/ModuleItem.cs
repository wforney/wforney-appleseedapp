using System;

namespace Appleseed.Framework
{
    /// <summary>
    /// This class encapsulates the basic attributes of a Module, and is used
    /// by the administration pages when manipulating modules.<br/>
    /// ModuleItem implements the IComparable interface so that an ArrayList
    /// of ModuleItems may be sorted by ModuleOrder, using the 
    /// ArrayList's Sort() method.
    /// </summary>
    public class ModuleItem : IComparable
    {
        private int _moduleOrder;
        private string _title;
        private string _pane;
        private int _ID;
        private int _defID;

        /// <summary>
        /// Order
        /// </summary>
        /// <value>The order.</value>
        public int Order
        {
            get { return _moduleOrder; }
            set { _moduleOrder = value; }
        }

        /// <summary>
        /// Title
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Pane name
        /// </summary>
        /// <value>The name of the pane.</value>
        public string PaneName
        {
            get { return _pane; }
            set { _pane = value; }
        }

        /// <summary>
        /// ID
        /// </summary>
        /// <value>The ID.</value>
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        /// <summary>
        /// Definition id
        /// </summary>
        /// <value>The module def ID.</value>
        public int ModuleDefID
        {
            get { return _defID; }
            set { _defID = value; }
        }

        /// <summary>
        /// Public comparer
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int CompareTo(object value)
        {
            if (value == null) return 1;

            int compareOrder = ((ModuleItem) value).Order;

            if (Order == compareOrder) return 0;
            if (Order < compareOrder) return -1;
            if (Order > compareOrder) return 1;
            return 0;
        }
    }
}