using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Appleseed.Framework.DataTypes
{
	/// <summary>
	/// BaseDataType
	/// </summary>
	public abstract class BaseDataType
	{
		/// <summary>
		/// Holds the value
		/// </summary>
		protected PropertiesDataType InnerDataType;
		/// <summary>
		/// 
		/// </summary>
		protected object InnerDataSource;
		/// <summary>
		/// 
		/// </summary>
		protected int controlWidth = 350;
		/// <summary>
		/// 
		/// </summary>
		protected Control innerControl;
		/// <summary>
		/// 
		/// </summary>
		protected string innerValue = string.Empty;

        /// <summary>
        /// InitializeComponents
        /// </summary>
		protected virtual void InitializeComponents()
		{
			//Text box
			using (TextBox tx = new TextBox())
			{
				tx.CssClass = "NormalTextBox";
				tx.Columns = 30;
				tx.Width = new Unit(controlWidth);
				tx.MaxLength = 1500; //changed max value to 1500 since most of settings are string

				innerControl = tx;
			}
		}

        /// <summary>
        /// Gets DataSource
        /// Should be overrided from inherited classes
        /// </summary>
        /// <value>The data source.</value>
		public virtual object DataSource
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the data value field.
		/// </summary>
		/// <value>The data value field.</value>
		public virtual string DataValueField
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the data text field.
		/// </summary>
		/// <value>The data text field.</value>
		public virtual string DataTextField
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Not Implemented
		/// </summary>
		/// <value>The full path.</value>
		public virtual string FullPath
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// EditControl
		/// </summary>
		/// <value>The edit control.</value>
		public virtual Control EditControl
		{
			get
			{
				if (innerControl == null)
					InitializeComponents();

				//Update value in control
				((TextBox)innerControl).Text = Value;
				//Return control
				return innerControl;
			}
			set
			{
				if (value.GetType().Name == "TextBox")
				{
					innerControl = value;
					//Update value from control
					Value = ((TextBox)innerControl).Text;
				}
				else
					throw new ArgumentException("A TextBox values is required, a '" + value.GetType().Name + "' is given.", "EditControl");
			}
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>The type.</value>
		public virtual PropertiesDataType Type
		{
			get
			{
				return InnerDataType;
			}
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public virtual string Value
		{
			get
			{
				return (innerValue);
			}
			set
			{
				innerValue = value;
			}
		}

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		public virtual string Description
		{
			get
			{
				return string.Empty;
			}
		}
	}
}