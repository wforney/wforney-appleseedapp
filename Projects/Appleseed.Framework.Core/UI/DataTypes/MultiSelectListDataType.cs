//John Bowen 4/9/2003 with help from jes1111
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// MultiSelectListDataType
    /// Implements a listbox that allows multiple selections 
    /// (returns a colon-delimited string)
    /// by John Bowen
    /// </summary>
    public class MultiSelectListDataType : BaseDataType
    {
        private string _dataValueField;
        private string _dataTextField;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSelectListDataType"/> class.
        /// </summary>
        /// <param name="_dataSource">The _data source.</param>
        /// <param name="_textField">The _text field.</param>
        /// <param name="_dataField">The _data field.</param>
        public MultiSelectListDataType(object _dataSource, string _textField, string _dataField)
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
            //ListBox
            using (ListBox lb = new ListBox())
            {
                lb.CssClass = "NormalTextBox";
                lb.SelectionMode = ListSelectionMode.Multiple;
                lb.Width = new Unit(controlWidth);
                lb.DataSource = DataSource;
                lb.DataValueField = DataValueField;
                lb.DataTextField = DataTextField;
                lb.DataBind();

                //Provide a smart height depending on items number
                if (lb.Items.Count > 5)
                    lb.Rows = 5;
                if (lb.Items.Count > 10)
                    lb.Rows = 10;
                if (lb.Items.Count > 20)
                    lb.Rows = 15;

                innerControl = lb;
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
                //				if(InnerDataSource != null)
                return InnerDataSource;
                //				else
                //					return null;
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
                innerValue = value.TrimEnd(new char[] {';'}); //Remove trailing ';'

                ////				//Fix by manu          
                ////				ListBox lb = (ListBox) innerControl;
                ////				lb.SelectionMode = ListSelectionMode.Multiple;
                ////				lb.ClearSelection();
                ////				//Clear inner value
                ////				innerValue = string.Empty;
                ////				if (value != null)
                ////				{
                ////					//Remove trailing ;
                ////					value = value.TrimEnd(new char[] {';'});
                ////					// Store in string array
                ////					string[] values = value.Split(new char[] {';'});
                ////					foreach (string _value in values)
                ////					{
                ////						if (lb.Items.FindByValue(_value) != null)
                ////						{
                ////							lb.Items.FindByValue(_value).Selected = true;
                ////							innerValue += value + ";";
                ////						}
                ////					}
                ////				}
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
                ListBox lb = (ListBox) innerControl;
                lb.ClearSelection();
                // Store in string array
                string[] values = innerValue.Split(new char[] {';'});
                foreach (string _value in values)
                {
                    if (lb.Items.FindByValue(_value) != null)
                        lb.Items.FindByValue(_value).Selected = true;
                }
                //Return control
                return innerControl;
            }
            set
            {
                if (value.GetType().Name == "ListBox")
                {
                    innerControl = value;

                    //Update value from control
                    ListBox lb = (ListBox) innerControl;
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < lb.Items.Count; i++)
                    {
                        if (lb.Items[i].Selected)
                        {
                            sb.Append(lb.Items[i].Value);
                            sb.Append(";");
                        }
                    }
                    Value = sb.ToString();
                }
                else
                    throw new ArgumentException(
                        "A ListBox value is required, a '" + value.GetType().Name + "' is given.", "EditControl");
            }
        }
    }
}