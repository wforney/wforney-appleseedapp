using System;
using System.Web;

namespace Appleseed.Framework.Design
{
    /// <summary>
    /// The Layout class encapsulates all the settings
    /// of the currently selected layout
    /// </summary>
    /// <remarks>by Cory Isakson</remarks>
    public class Layout
    {
        private string _name;

        /// <summary>
        /// The Layout Name (must be the directory in which is located)
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Current Web Path.
        /// It is set at runtime and therefore is not serialized
        /// </summary>
        [NonSerialized()] public string WebPath;

        /// <summary>
        /// Current Phisical Path. Readonly.
        /// </summary>
        /// <value>The path.</value>
        public string Path
        {
            get { return (HttpContext.Current.Server.MapPath(WebPath)); }
        }
    }
}