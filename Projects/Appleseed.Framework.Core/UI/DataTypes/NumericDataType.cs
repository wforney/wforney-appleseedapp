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
    /// <summary>
    /// NumericDataType
    /// </summary>
    public class NumericDataType : BaseDataType
    {
        #region Constants and Fields

        /// <summary>
        /// The inner value.
        /// </summary>
        protected new string innerValue = "0";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "NumericDataType" /> class.
        /// </summary>
        public NumericDataType()
        {
            this.InnerDataType = PropertiesDataType.Double;

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
        public override string Value
        {
            get
            {
                return this.innerValue;
            }

            set
            {
                // Type check
                this.innerValue = double.Parse(value).ToString();
            }
        }

        #endregion
    }
}