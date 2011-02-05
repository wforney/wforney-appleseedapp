using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// ListDataType
    /// </summary>
    public class ListDataType : BaseDataType
    {
        private string _dataValueField;
        private string _dataTextField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListDataType"/> class.
        /// </summary>
        public ListDataType()
        {
            InnerDataType = PropertiesDataType.List;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListDataType"/> class.
        /// </summary>
        /// <param name="CsvList">The CSV list.</param>
        public ListDataType(string CsvList)
        {
            InnerDataType = PropertiesDataType.List;
            InnerDataSource = CsvList;
            //InitializeComponents();
        }

        /// <summary>
        /// Returning dropdown list databound to FileInfo[] array
        /// </summary>
        /// <param name="fileList">The file list.</param>
        public ListDataType(FileInfo[] fileList)
        {
            InnerDataType = PropertiesDataType.List;
            InnerDataSource = fileList;
            //this._dataTextField = "Name";
            //this._dataValueField = "Name";
            //InitializeComponents();
            //innerControl.DataBind();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListDataType"/> class.
        /// </summary>
        /// <param name="_dataSource">The _data source.</param>
        /// <param name="_textField">The _text field.</param>
        /// <param name="_dataField">The _data field.</param>
        public ListDataType(object _dataSource, string _textField, string _dataField)
        {
            InnerDataType = PropertiesDataType.List;
            InnerDataSource = _dataSource;
            DataTextField = _textField;
            DataValueField = _dataField;
            //InitializeComponents();
        }


        /// <summary>
        /// InitializeComponents
        /// </summary>
        protected override void InitializeComponents()
        {
            //Dropdown list
            using (DropDownList dd = new DropDownList())
            {
                dd.CssClass = "NormalTextBox";
                dd.Width = new Unit(controlWidth);
                dd.DataSource = DataSource;
                dd.DataValueField = DataValueField;
                dd.DataTextField = DataTextField;
                dd.DataBind();

                innerControl = dd;
            }
        }

        /// <summary>
        /// Gets or sets the data value field.
        /// </summary>
        /// <value>The data value field.</value>
        public override string DataValueField
        {
            get { return _dataValueField; }
            set { _dataValueField = value; }
        }

        /// <summary>
        /// Gets or sets the data text field.
        /// </summary>
        /// <value>The data text field.</value>
        public override string DataTextField
        {
            get { return _dataTextField; }
            set { _dataTextField = value; }
        }

        /// <summary>
        /// Gets DataSource
        /// Should be overrided from inherited classes
        /// </summary>
        /// <value>The data source.</value>
        public override object DataSource
        {
            get
            {
                if (InnerDataSource != null)
                    if (InnerDataSource is string)
                    {
                        return InnerDataSource.ToString().Split(';');
                    }
                    else
                    {
                        return InnerDataSource;
                    }
                else
                    return null;
            }
            set { InnerDataSource = value; }
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
                innerValue = value;

                //				DropDownList dd = (DropDownList) innerControl;
                //				if (dd.Items.FindByValue(value) != null)
                //				{
                //					dd.ClearSelection();
                //					dd.Items.FindByValue(value).Selected = true;
                //					innerValue = value;
                //				}
                //				else
                //				{
                //					//Invalid value
                //				}            
            }
        }


        /// <summary>
        /// EditControl
        /// </summary>
        /// <value>The edit control.</value>
        public override Control EditControl
        {
            get
            {
                if (innerControl == null)
                    InitializeComponents();

                //Update value in control
                DropDownList dd = (DropDownList) innerControl;
                dd.ClearSelection();
                if (dd.Items.FindByValue(Value) != null)
                    dd.Items.FindByValue(Value).Selected = true;
                //Return control
                return innerControl;
            }
            set
            {
                if (value.GetType().Name == "DropDownList")
                {
                    innerControl = value;
                    //Update value from control
                    DropDownList dd = (DropDownList) innerControl;
                    if (dd.SelectedItem != null)
                        Value = dd.SelectedItem.Value;
                    else
                        Value = string.Empty;
                }
                else
                    throw new ArgumentException(
                        "A DropDownList values is required, a '" + value.GetType().Name + "' is given.", "EditControl");
            }
        }
    }
}