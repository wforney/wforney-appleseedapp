using System;
using System.Xml.Serialization;

namespace Appleseed.Framework.Design
{
    /// <summary>
    /// A single named HTML fragment
    /// </summary>
    [Serializable]
    public class ThemePart
    {
        private string _Html;
        private string _Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ThemePart"/> class.
        /// </summary>
        /// <returns>
        /// A void value...
        /// </returns>
        public ThemePart()
        {
            _Name = string.Empty;
            _Html = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ThemePart"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="html">The HTML.</param>
        /// <returns>
        /// A void value...
        /// </returns>
        public ThemePart(string name, string html)
        {
            _Name = name;
            _Html = html;
        }

        /// <summary>
        /// Gets or sets the HTML.
        /// </summary>
        /// <value>The HTML.</value>
        /// <remarks>
        /// </remarks>
        public string Html
        {
            get { return _Html; }
            set { _Html = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks>
        /// </remarks>
        [XmlAttribute(DataType = "string")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
    }
}