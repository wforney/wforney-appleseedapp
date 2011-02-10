// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateDataType.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   Date Data Type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.DataTypes
{
    using System;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Date Data Type
    /// </summary>
    public class DateDataType : BaseDataType<DateTime, TextBox>
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DateDataType" /> class.
        /// </summary>
        public DateDataType()
        {
            this.Type = PropertiesDataType.Date;

            // InitializeComponents();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the description.
        /// </summary>
        public override string Description
        {
            get
            {
                return "DateTime";
            }
        }

        #endregion
    }
}