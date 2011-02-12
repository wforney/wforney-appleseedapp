// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseDataType.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   Base Data Type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.DataTypes
{
    using System;
    using System.ComponentModel;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Base Data Type
    /// </summary>
    /// <typeparam name="T">
    /// The type of data.
    /// </typeparam>
    /// <typeparam name="TEditControl">
    /// The edit control for the data type.
    /// </typeparam>
    public abstract class BaseDataType<T, TEditControl>
        where TEditControl : class
    {
        #region Constants and Fields

        /// <summary>
        /// The inner data source.
        /// </summary>
        protected object InnerDataSource;

        /// <summary>
        /// The control width.
        /// </summary>
        protected int ControlWidth = 350;

        /// <summary>
        /// The inner control.
        /// </summary>
        protected TEditControl InnerControl;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>
        /// The data source.
        /// </value>
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
        ///   Gets or sets the data text field.
        /// </summary>
        /// <value>
        ///   The data text field.
        /// </value>
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
        ///   Gets or sets the data value field.
        /// </summary>
        /// <value>
        ///   The data value field.
        /// </value>
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
        ///   Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public virtual string Description
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the edit control.
        /// </summary>
        /// <value>
        /// The edit control.
        /// </value>
        public virtual TEditControl EditControl
        {
            get
            {
                if (this.InnerControl == null)
                {
                    this.InitializeComponents();
                }

                // Update value in control
                var converter = TypeDescriptor.GetConverter(typeof(TEditControl));
                if (converter != null)
                {
                    if (converter.CanConvertTo(typeof(TextBox)) && this.InnerControl != null)
                    {
                        var txt = (TextBox)converter.ConvertFrom(this.InnerControl);
                        if (txt != null)
                        {
                            txt.Text = this.Value.ToString();
                        }
                    }
                }

                // Return control
                return this.InnerControl;
            }

            set
            {
                if (value.GetType().Name != "TextBox")
                {
                    throw new ArgumentException(
                        "EditControl",
                        string.Format("A TextBox value is required, a '{0}' is given.", value.GetType().Name));
                }

                this.InnerControl = value;

                // Update value from control
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter == null)
                {
                    return;
                }

                var converter2 = TypeDescriptor.GetConverter(typeof(TEditControl));
                if (converter2 == null)
                {
                    return;
                }

                if (!converter2.CanConvertTo(typeof(TextBox)))
                {
                    return;
                }

                var txt = (TextBox)converter2.ConvertFrom(this.InnerControl);
                if (txt != null)
                {
                    this.Value = (T)converter.ConvertFrom(txt.Text);
                }
            }
        }

        /// <summary>
        ///   Gets the full path.
        /// </summary>
        public virtual string FullPath
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type of data (enum).
        /// </value>
        [Obsolete("This class is generic now so we don't really need this do we?")]
        public virtual PropertiesDataType Type { get; protected set; }

        /// <summary>
        ///   Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public virtual T Value { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the components.
        /// </summary>
        protected virtual void InitializeComponents()
        {
            if (typeof(TEditControl) == typeof(TextBox))
            {
                // Text box
                // changed max value to 1500 since most of settings are string
                var tx =
                    new TextBox
                        {
                            CssClass = "NormalTextBox",
                            Columns = 30,
                            Width = new Unit(this.ControlWidth),
                            MaxLength = 1500
                        }

                    as TEditControl;

                this.InnerControl = tx;
            }
        }

        #endregion
    }
}