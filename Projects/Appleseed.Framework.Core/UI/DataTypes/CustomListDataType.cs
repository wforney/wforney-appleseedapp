using System;
namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// CustomListDataType
    /// </summary>
    public class CustomListDataType : ListDataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomListDataType"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="dataTextField">The data text field.</param>
        /// <param name="dataValueField">The data value field.</param>
        public CustomListDataType(object dataSource, string dataTextField, string dataValueField)
        {
            InnerDataType = PropertiesDataType.List;
            InnerDataSource = dataSource;
            DataValueField = dataValueField;
            DataTextField = dataTextField;
            //InitializeComponents();
        }

        /// <summary>
        /// Gets DataSource
        /// Should be overrided from inherited classes
        /// </summary>
        /// <value>The data source.</value>
        public override object DataSource
        {
            get {
                object result = InnerDataSource;
                if (InnerDataSource is Delegate)
                {
                    result = ((Delegate)InnerDataSource).DynamicInvoke(null);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "Custom List"; }
        }
    }
}