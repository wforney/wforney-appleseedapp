namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// StringDataType
    /// </summary>
    public class StringDataType : BaseDataType
    {
        /// <summary>
        /// StringDataType
        /// </summary>
        public StringDataType()
        {
            InnerDataType = PropertiesDataType.String;
            //InitializeComponents();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringDataType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public StringDataType(string value)
        {
            InnerDataType = PropertiesDataType.String;
            Value = value;
            InitializeComponents();
        }

        /// <summary>
        /// String
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "String"; }
        }
    }
}