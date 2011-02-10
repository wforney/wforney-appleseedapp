// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingItem.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   This class holds a single setting in the hash table,
//   providing information about data type, constraints.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework
{
    using System;
    using System.Web.UI.WebControls;

    using Appleseed.Framework.DataTypes;

    /// <summary>
    /// This class holds a single setting in the hash table,
    /// providing information about data type, constraints.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the setting item.
    /// </typeparam>
    /// <typeparam name="TEditControl">
    /// The edit control for the value.
    /// </typeparam>
    /// <author>
    /// by Manu
    /// </author>
    public class SettingItem<T, TEditControl> : ISettingItem<T, TEditControl>
        where TEditControl : class
    {
        #region Constants and Fields

        /// <summary>
        /// The data type.
        /// </summary>
        private readonly BaseDataType<T, TEditControl> datatype;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingItem&lt;T, TEditControl&gt;"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="value">The value.</param>
        public SettingItem(BaseDataType<T, TEditControl> dataType, T value)
        {
            this.EnglishName = string.Empty;
            this.Description = string.Empty;
            this.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            this.datatype = dataType;
            this.datatype.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingItem&lt;T, TEditControl&gt;"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        public SettingItem(BaseDataType<T, TEditControl> dataType)
        {
            this.EnglishName = string.Empty;
            this.Description = string.Empty;
            this.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            this.datatype = dataType;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the data source.
        /// </summary>
        public object DataSource
        {
            get
            {
                return this.datatype.DataSource;
            }
        }

        /// <summary>
        ///   Gets the type of the data.
        /// </summary>
        /// <value>
        ///   The type of the data.
        /// </value>
        public PropertiesDataType DataType
        {
            get
            {
                return this.datatype.Type;
            }
        }

        /// <summary>
        ///   Gets or sets Provide help for parameter.
        ///   Should be a brief, descriptive text that explains what
        ///   this setting should do.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        ///   Gets or sets the edit control.
        /// </summary>
        /// <value>
        ///   The edit control.
        /// </value>
        public TEditControl EditControl
        {
            get
            {
                return this.datatype.EditControl;
            }

            set
            {
                this.datatype.EditControl = value;
            }
        }

        /// <summary>
        ///   Gets or sets the name of the parameter in plain English.
        /// </summary>
        /// <value>The name of the English.</value>
        public string EnglishName { get; set; }

        /// <summary>
        ///   Gets the full path.
        /// </summary>
        public string FullPath
        {
            get
            {
                return this.datatype.FullPath;
            }
        }

        /// <summary>
        ///   Gets or sets Allows grouping of settings in SettingsTable - use
        ///   Appleseed.Framework.Configuration.SettingItemGroup enum (convert to string)
        /// </summary>
        /// <value>The group.</value>
        /// <author>
        ///   Jes1111
        /// </author>
        public SettingItemGroup Group { get; set; }

        /// <summary>
        ///   Gets a description in plain English for
        ///   Group Key (read only)
        /// </summary>
        /// <value>The group description.</value>
        public string GroupDescription
        {
            get
            {
                switch (this.Group)
                {
                    case SettingItemGroup.NONE:
                        return "Generic settings";

                    case SettingItemGroup.THEME_LAYOUT_SETTINGS:
                        return "Theme and layout settings";

                    case SettingItemGroup.SECURITY_USER_SETTINGS:
                        return "Users and Security settings";

                    case SettingItemGroup.CULTURE_SETTINGS:
                        return "Culture settings";

                    case SettingItemGroup.BUTTON_DISPLAY_SETTINGS:
                        return "Buttons and Display settings";

                    case SettingItemGroup.MODULE_SPECIAL_SETTINGS:
                        return "Specific Module settings";

                    case SettingItemGroup.META_SETTINGS:
                        return "Meta settings";

                    case SettingItemGroup.MISC_SETTINGS:
                        return "Miscellaneous settings";

                    case SettingItemGroup.NAVIGATION_SETTINGS:
                        return "Navigation settings";

                    case SettingItemGroup.CUSTOM_USER_SETTINGS:
                        return "Custom User Settings";
                }

                return "Settings";
            }
        }

        /// <summary>
        ///   Gets or sets the max value.
        /// </summary>
        /// <value>
        ///   The max value.
        /// </value>
        public int MaxValue { get; set; }

        /// <summary>
        ///   Gets or sets the min value.
        /// </summary>
        /// <value>
        ///   The min value.
        /// </value>
        public int MinValue { get; set; }

        /// <summary>
        ///   Gets or sets the Display Order - use Appleseed.Framework.Configuration.SettingItemGroup enum
        ///   (add integer in range 1-999)
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SettingItem&lt;T, TEditControl&gt;"/> is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if required; otherwise, <c>false</c>.
        /// </value>
        public bool Required { get; set; }

        /// <summary>
        ///   Gets or sets the value.
        /// </summary>
        /// <value>
        ///   The value.
        /// </value>
        public T Value
        {
            get
            {
                return this.datatype.Value;
            }

            set
            {
                this.datatype.Value = value;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        ///   ToString converter operator
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns></returns>
        public static implicit operator string(SettingItem<T, TEditControl> value)
        {
            return value.ToString();
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.datatype.Value.ToString();
        }

        #endregion

        #region Implemented Interfaces

        #region IComparable

        /// <summary>
        /// Public comparer
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The compare to.
        /// </returns>
        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }

            // Modified by Hongwei Shen(hongwei.shen@gmail.com) 10/9/2005
            // the "value" should be casted to SettingItem instead of ModuleItem 
            // int compareOrder = ((ModuleItem) value).Order;
            var compareOrder = ((SettingItem<T, TEditControl>)value).Order;

            // end of modification            
            return this.Order != compareOrder
                       ? (this.Order < compareOrder ? -1 : (this.Order > compareOrder ? 1 : 0))
                       : 0;
        }

        #endregion

        #endregion
    }
}