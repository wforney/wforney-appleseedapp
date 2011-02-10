// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleDataType.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   Double Data Type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// Double Data Type
    /// </summary>
    public class DoubleDataType : NumericDataType
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DoubleDataType" /> class.
        /// </summary>
        public DoubleDataType()
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
                return "Double";
            }
        }

        #endregion
    }
}