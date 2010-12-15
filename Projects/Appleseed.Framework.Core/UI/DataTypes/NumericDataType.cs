namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// NumericDataType
    /// </summary>
    public class NumericDataType : BaseDataType
    {
        protected new string innerValue = "0";

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericDataType"/> class.
        /// </summary>
        public NumericDataType()
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
            get { return (innerValue); }
            set
            {
                //Type check
                innerValue = double.Parse(value).ToString();
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "Numeric"; }
        }
    }
}