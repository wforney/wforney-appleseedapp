// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlDataType.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   Url Data Type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.DataTypes
{
    using System;

    /// <summary>
    /// Url Data Type
    /// </summary>
    public class UrlDataType : BaseDataType
    {
        #region Constants and Fields

        /// <summary>
        /// The inner value.
        /// </summary>
        protected new string innerValue = "http://localhost";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UrlDataType" /> class.
        /// </summary>
        public UrlDataType()
        {
            this.InnerDataType = PropertiesDataType.String;

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
                return "Full valid URI";
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
                return base.Value;
            }

            set
            {
                // Check type
                // Check by Bill (blarm)
                base.Value = !string.IsNullOrEmpty(value) ? new Uri(value).ToString() : value;
            }
        }

        #endregion
    }
}