namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// DoubleDataType
    /// </summary>
    public class DoubleDataType : NumericDataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleDataType"/> class.
        /// </summary>
        public DoubleDataType()
        {
            InnerDataType = PropertiesDataType.Double;
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
                double i = double.Parse(value);
                base.Value = value;
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "Double"; }
        }
    }
}