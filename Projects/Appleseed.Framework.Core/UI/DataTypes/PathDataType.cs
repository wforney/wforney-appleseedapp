// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PathDataType.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   Path Data Type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// Path Data Type
    /// </summary>
    public class PathDataType : StringDataType
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PathDataType" /> class.
        /// </summary>
        public PathDataType()
        {
            this.Type = PropertiesDataType.String;

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
                return "File System path";
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
                value = value.Replace("/", "\\");
                base.Value = value;
            }
        }

        #endregion
    }
}