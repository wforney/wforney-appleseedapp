using System;
using System.Collections;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// Implements a DropDownList with a list of directorys 
    /// and a TextBox for add and create a new directory.
    /// </summary>
    public class FolderDataType : BaseDataType
    {
        private string _dataValueField;
        private string _dataTextField;
        private string _baseDirectory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseDirectory">Create if not exists</param>
        /// <param name="defaultDirectory">Create if not exists</param>
        public FolderDataType(string baseDirectory, string defaultDirectory)
        {
            _baseDirectory = baseDirectory;
            InnerDataType = PropertiesDataType.List;

            if (defaultDirectory != null)
            {
                try
                {
                    if (!Directory.Exists(baseDirectory + "/" + defaultDirectory))
                    {
                        if (!Directory.Exists(baseDirectory))
                            Directory.CreateDirectory(baseDirectory);
                        Directory.CreateDirectory(baseDirectory + "/" + defaultDirectory);
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Cannot create the default directory '" + defaultDirectory + "'",
                                                "FolderDataType", ex);
                }
            }
            ArrayList result = new ArrayList();
            foreach (DirectoryInfo di in new DirectoryInfo(baseDirectory).GetDirectories())
            {
                if (di.Name != "CVS" && di.Name != "_svn") //Ignore CVS and _svn folders
                {
                    result.Add(new ListItem(di.Name, di.Name));
                }
            }
            InnerDataSource = result;
        }

        /// <summary>
        /// InitializeComponents
        /// </summary>
        protected override void InitializeComponents()
        {
            using (Panel panel = new Panel())
            {
                using (DropDownList dd = new DropDownList())
                {
                    dd.CssClass = "NormalTextBox";
                    dd.Width = new Unit(controlWidth);
                    dd.DataSource = DataSource;
                    dd.DataValueField = DataValueField;
                    dd.DataTextField = DataTextField;
                    dd.DataBind();
                    dd.Width = (base.controlWidth/2 - 1);
                    dd.ID = panel.ID + "dd";

                    panel.Controls.Add(dd);
                }

                using (TextBox tb = new TextBox())
                {
                    tb.CssClass = "NormalTextBox";
                    tb.Text = General.GetString("NEW_FOLDER", "New Folder ?");
                    tb.Columns = 30;
                    tb.Width = (base.controlWidth/2 - 1);
                    tb.ID = panel.ID + "tb";
                    tb.MaxLength = 1500;

                    panel.Controls.Add(tb);
                }

                innerControl = panel;
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
                    return InnerDataSource;
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
            set { innerValue = value; }
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
                Panel panel = (Panel) innerControl;
                DropDownList dd = null;
                foreach (Control c in panel.Controls)
                {
                    if (c is DropDownList)
                    {
                        //Update value in control
                        dd = (DropDownList) c;
                        dd.ClearSelection();
                        if (dd.Items.FindByValue(Value) != null)
                            dd.Items.FindByValue(Value).Selected = true;
                    }
                }
                //Return control
                return innerControl;
            }
            set
            {
                if (value is Panel)
                {
                    innerControl = value;
                    //Update value from control
                    DropDownList dd = null;
                    foreach (Control c in value.Controls)
                    {
                        if (c is DropDownList)
                        {
                            dd = (DropDownList) c;
                            if (dd.SelectedItem != null)
                                Value = dd.SelectedItem.Value;
                            else
                                Value = string.Empty;
                        }
                        if (c is TextBox)
                        {
                            TextBox tb = (TextBox) c;
                            if (tb.Text != General.GetString("NEW_FOLDER", "New Folder ?"))
                            {
                                try
                                {
                                    if (!Directory.Exists(_baseDirectory + "/" + tb.Text))
                                    {
                                        Directory.CreateDirectory(_baseDirectory + "/" + tb.Text);
                                        if (dd != null)
                                        {
                                            dd.Items.Add(new ListItem(tb.Text, tb.Text));
                                            dd.Items.FindByValue(tb.Text).Selected = true;
                                            dd.SelectedIndex = dd.Items.Count - 1;
                                            Value = dd.SelectedItem.Value;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                                tb.Text = General.GetString("NEW_FOLDER", "New Folder ?");
                            }
                        }
                    }
                }
                else
                    throw new ArgumentException(
                        "A Panel values is required, a '" + value.GetType().Name + "' is given.", "EditControl");
            }
        }
    }
}