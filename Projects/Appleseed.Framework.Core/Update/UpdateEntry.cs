// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateEntry.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   The update entry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.Core.Update
{
    using System;
    using System.Collections;

    /// <summary>
    /// The update entry.
    /// </summary>
    [Serializable]
    public class UpdateEntry : IComparable
    {
        #region Constants and Fields

        /// <summary>
        /// The apply.
        /// </summary>
        public bool Apply;

        /// <summary>
        /// The date.
        /// </summary>
        public DateTime Date;

        /// <summary>
        /// The modules.
        /// </summary>
        public ArrayList Modules = new ArrayList();

        /// <summary>
        /// The version.
        /// </summary>
        public string Version = string.Empty;

        /// <summary>
        /// The version number.
        /// </summary>
        public int VersionNumber;

        /// <summary>
        /// The script names.
        /// </summary>
        public ArrayList scriptNames = new ArrayList();

        #endregion

        #region Implemented Interfaces

        #region IComparable

        /// <summary>
        /// IComparable.CompareTo implementation.
        /// </summary>
        /// <param name="obj">
        /// An object to compare with this instance.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than obj. Zero This instance is equal to obj. Greater than zero This instance is greater than obj.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// obj is not the same type as this instance. 
        /// </exception>
        public int CompareTo(object obj)
        {
            if (obj is UpdateEntry)
            {
                var upd = (UpdateEntry)obj;
                
                // if .. then Version numbers are equal else ..
                return this.VersionNumber.CompareTo(upd.VersionNumber) == 0 ? this.Version.CompareTo(upd.Version) : this.VersionNumber.CompareTo(upd.VersionNumber);
            }

            throw new ArgumentException("object is not a UpdateEntry");
        }

        #endregion

        #endregion
    }
}