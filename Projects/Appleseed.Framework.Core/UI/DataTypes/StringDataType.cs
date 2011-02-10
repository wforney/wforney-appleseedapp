namespace Appleseed.Framework.DataTypes
{
    using System.Web.UI.WebControls;

    /// <summary>
    /// String Data Type
    /// </summary>
    public class StringDataType : BaseDataType<string, TextBox>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringDataType"/> class.
        /// </summary>
        public StringDataType()
        {
            this.Type = PropertiesDataType.String;
            
            // InitializeComponents();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringDataType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public StringDataType(string value)
        {
            this.Type = PropertiesDataType.String;
            this.Value = value;
            InitializeComponents();
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public override string Description
        {
            get { return "String"; }
        }
    }
}