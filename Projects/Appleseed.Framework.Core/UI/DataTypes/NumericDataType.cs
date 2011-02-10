// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumericDataType.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   NumericDataType
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.DataTypes
{
    using System.Web.UI.WebControls;

    /// <summary>
    /// Numeric Data Type
    /// </summary>
    public class NumericDataType : BaseDataType<double, TextBox>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericDataType"/> class.
        /// </summary>
        public NumericDataType()
        {
            this.Type = PropertiesDataType.Double;

            // InitializeComponents();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get
            {
                return "Numeric";
            }
        }

        /// <summary>
        ///   Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public override double Value { get; set; }

        #endregion
    }
}