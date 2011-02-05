using System;
using System.Web.UI.WebControls;
using System.Xml.Serialization;

namespace Appleseed.Framework.Design
{
    /// <summary>
    /// A single named Image
    /// </summary>
    [Serializable]
    public class ThemeImage
    {
        private double _Height;
        private string _ImageUrl;
        private string _Name;
        private double _Width;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ThemeImage"/> class.
        /// </summary>
        /// <returns>
        /// A void value...
        /// </returns>
        public ThemeImage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ThemeImage"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>
        /// A void value...
        /// </returns>
        public ThemeImage(string name, string imageUrl, double width, double height)
        {
            _Name = name;
            _ImageUrl = imageUrl;
            _Width = width;
            _Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ThemeImage"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="img">The img.</param>
        /// <returns>
        /// A void value...
        /// </returns>
        public ThemeImage(string name, Image img)
        {
            _Name = name;
            _ImageUrl = img.ImageUrl;
            _Width = img.Width.Value;
            _Height = img.Height.Value;
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <returns>
        /// A System.Web.UI.WebControls.Image value...
        /// </returns>
        public Image GetImage()
        {
            using (Image img = new Image())
            {
                img.ImageUrl = ImageUrl;
                img.Width = new Unit(Width);
                img.Height = new Unit(Height);
                return img;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        /// <remarks>
        /// </remarks>
        [XmlAttribute]
        public double Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>The image URL.</value>
        /// <remarks>
        /// </remarks>
        [XmlAttribute]
        public string ImageUrl
        {
            get { return _ImageUrl; }
            set { _ImageUrl = value; }
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

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        /// <remarks>
        /// </remarks>
        [XmlAttribute]
        public double Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
    }
}