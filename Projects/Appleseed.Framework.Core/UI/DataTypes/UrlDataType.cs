using System;

namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// UrlDataType
    /// </summary>
    public class UrlDataType : BaseDataType
    {
        protected new string innerValue = "http://localhost";

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlDataType"/> class.
        /// </summary>
        public UrlDataType()
        {
            InnerDataType = PropertiesDataType.String;
            //InitializeComponents();
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public override string Value
        {
            get { return base.Value; }
            set
            {
                //Check type
                if ((value != null) && (value.Length != 0)) //Check by Bill (blarm)
                    base.Value = new Uri(value).ToString();
                else
                    base.Value = value;
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "Full valid URI"; }
        }
    }
}