// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationUrlDataType.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   Application URL Data Type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// Application URL Data Type
    /// </summary>
    public class ApplicationUrlDataType : UrlDataType
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ApplicationUrlDataType" /> class.
        /// </summary>
        public ApplicationUrlDataType()
        {
            this.Type = PropertiesDataType.String;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   URL relative to Application
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get
            {
                return "URL relative to Application";
            }
        }

        #endregion
    }
}