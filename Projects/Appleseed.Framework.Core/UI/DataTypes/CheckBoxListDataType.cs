//Mike Stone 23/01/2005 based on John Bowens Multiselectlist
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// CheckBoxListDataType
    /// Implements a checkboxlist that allows multiple selections 
    /// (returns a colon-delimited string)
    /// by Mike Stone
    /// </summary>
    public class CheckBoxListDataType : BaseDataType
    {
        private string _dataValueField;
        private string _dataTextField;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBoxListDataType"/> class.
        /// </summary>
        /// <param name="_dataSource">The _data source.</param>
        /// <param name="_textField">The _text field.</param>
        /// <param name="_dataField">The _data field.</param>
        public CheckBoxListDataType(object _dataSource, string _textField, string _dataField)
        {
            InnerDataType = PropertiesDataType.List;
            InnerDataSource = _dataSource;
            DataTextField = _textField;
            DataValueField = _dataField;
        }

        /// <summary>
        /// InitializeComponents
        /// </summary>
        protected override void InitializeComponents()
        {
            //ListBox
            using (CheckBoxList cbl = new CheckBoxList())
            {
                // cbl.CssClass = "NormalTextBox";
                cbl.Width = new Unit("100%");
                cbl.RepeatColumns = 2;
                cbl.DataSource = DataSource;
                cbl.DataValueField = DataValueField;
                cbl.DataTextField = DataTextField;
                cbl.DataBind();

                innerControl = cbl;
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
            get { return InnerDataSource; }
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
                innerValue = value.TrimEnd(new char[] {';'}); //Remove trailing ';'
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
                CheckBoxList cbl = (CheckBoxList) innerControl;
                cbl.ClearSelection();
                // Store in string array
                string[] values = innerValue.Split(new char[] {';'});
                foreach (string _value in values)
                {
                    if (cbl.Items.FindByValue(_value) != null)
                        cbl.Items.FindByValue(_value).Selected = true;
                }
                //Return control
                return innerControl;
            }
            set
            {
                if (value.GetType().Name == "CheckBoxList")
                {
                    innerControl = value;

                    //Update value from control
                    CheckBoxList cbl = (CheckBoxList) innerControl;
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < cbl.Items.Count; i++)
                    {
                        if (cbl.Items[i].Selected)
                        {
                            sb.Append(cbl.Items[i].Value);
                            sb.Append(";");
                        }
                    }
                    Value = sb.ToString();
                }
                else
                    throw new ArgumentException(
                        "A CheckBoxList value is required, a '" + value.GetType().Name + "' is given.", "EditControl");
            }
        }
    }
}