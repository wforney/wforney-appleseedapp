using System;

namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// DateDataType
    /// </summary>
    public class DateDataType : StringDataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateDataType"/> class.
        /// </summary>
        public DateDataType()
        {
            InnerDataType = PropertiesDataType.Date;
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
                DateTime.Parse(value);
                base.Value = value;
            }
        }

        /// <summary>
        /// String
        /// </summary>
        /// <value></value>
        public override string Description
        {
            get { return "DateTime"; }
        }
    }
}