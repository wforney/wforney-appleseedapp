using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Appleseed.Framework.DataTypes
{
    public class CustomListDelegateDataType : ListDataType
    {
        public delegate SortedList InitializeCustomSettingsDelegate();

        public CustomListDelegateDataType(InitializeCustomSettingsDelegate function, string dataTextField, string dataValueField)
        {
            InnerDataType = PropertiesDataType.List;
            InnerDataSource = function;
            DataValueField = dataValueField;
            DataTextField = dataTextField;
        }

        public override object DataSource
        {
            get
            {
                return ((Delegate)InnerDataSource).DynamicInvoke(null);
            }
        }

        public override string Description
        {
            get { return "Custom List with Delegate"; }
        }
    }
}
