using System;

namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// IntegerDataType
    /// </summary>
    public class IntegerDataType : NumericDataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerDataType"/> class.
        /// </summary>
        public IntegerDataType()
        {
            InnerDataType = PropertiesDataType.Integer;
            InitializeComponents();
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
                base.Value = Int32.Parse(value).ToString();
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "Integer"; }
        }
    }
}