namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// PathDataType
    /// </summary>
    public class PathDataType : StringDataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathDataType"/> class.
        /// </summary>
        public PathDataType()
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
                value = value.Replace("/", "\\");
                base.Value = value;
            }
        }

        /// <summary>
        /// String
        /// </summary>
        /// <value></value>
        public override string Description
        {
            get { return "File System path"; }
        }
    }
}