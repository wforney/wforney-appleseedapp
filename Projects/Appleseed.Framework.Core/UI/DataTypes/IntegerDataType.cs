// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerDataType.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   Integer Data Type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.DataTypes
{
    using System.Web.UI.WebControls;

    /// <summary>
    /// Integer Data Type
    /// </summary>
    public class IntegerDataType : BaseDataType<int, TextBox>
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "IntegerDataType" /> class.
        /// </summary>
        public IntegerDataType()
        {
            this.Type = PropertiesDataType.Integer;
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
                return "Integer";
            }
        }

        #endregion
    }
}