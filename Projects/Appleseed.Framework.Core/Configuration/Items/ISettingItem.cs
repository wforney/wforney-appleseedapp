namespace Appleseed.Framework
{
    using System;

    using Appleseed.Framework.DataTypes;

    /// <summary>
    /// Setting Item Interface
    /// </summary>
    /// <typeparam name="T">The type of the setting value.</typeparam>
    /// <typeparam name="TEditControl">The type of the edit control.</typeparam>
    public interface ISettingItem<T, TEditControl> : IComparable
        where TEditControl : class
    {
        /// <summary>
        ///   Gets the data source.
        /// </summary>
        object DataSource { get; }

        /// <summary>
        ///   Gets the type of the data.
        /// </summary>
        /// <value>
        ///   The type of the data.
        /// </value>
        PropertiesDataType DataType { get; }

        /// <summary>
        ///   Gets or sets Provide help for parameter.
        ///   Should be a brief, descriptive text that explains what
        ///   this setting should do.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; set; }

        /// <summary>
        ///   Gets or sets the edit control.
        /// </summary>
        /// <value>
        ///   The edit control.
        /// </value>
        TEditControl EditControl { get; set; }

        /// <summary>
        ///   Gets or sets the name of the parameter in plain English.
        /// </summary>
        /// <value>The name of the English.</value>
        string EnglishName { get; set; }

        /// <summary>
        ///   Gets the full path.
        /// </summary>
        string FullPath { get; }

        /// <summary>
        ///   Gets or sets Allows grouping of settings in SettingsTable - use
        ///   Appleseed.Framework.Configuration.SettingItemGroup enum (convert to string)
        /// </summary>
        /// <value>The group.</value>
        /// <author>
        ///   Jes1111
        /// </author>
        SettingItemGroup Group { get; set; }

        /// <summary>
        ///   Gets a description in plain English for
        ///   Group Key (read only)
        /// </summary>
        /// <value>The group description.</value>
        string GroupDescription { get; }

        /// <summary>
        ///   Gets or sets the max value.
        /// </summary>
        /// <value>
        ///   The max value.
        /// </value>
        int MaxValue { get; set; }

        /// <summary>
        ///   Gets or sets the min value.
        /// </summary>
        /// <value>
        ///   The min value.
        /// </value>
        int MinValue { get; set; }

        /// <summary>
        ///   Gets or sets the Display Order - use Appleseed.Framework.Configuration.SettingItemGroup enum
        ///   (add integer in range 1-999)
        /// </summary>
        /// <value>The order.</value>
        int Order { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SettingItem&lt;T, TEditControl&gt;"/> is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if required; otherwise, <c>false</c>.
        /// </value>
        bool Required { get; set; }

        /// <summary>
        ///   Gets or sets the value.
        /// </summary>
        /// <value>
        ///   The value.
        /// </value>
        T Value { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        string ToString();
    }
}