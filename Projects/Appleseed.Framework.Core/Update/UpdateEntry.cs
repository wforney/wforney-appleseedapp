using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Appleseed.Framework.Core.Update
{
    [Serializable]
    public class UpdateEntry : IComparable
    {
        /// <summary>
        /// IComparable.CompareTo implementation.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than obj. Zero This instance is equal to obj. Greater than zero This instance is greater than obj.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">obj is not the same type as this instance. </exception>
        public int CompareTo(object obj)
        {
            if (obj is UpdateEntry)
            {
                UpdateEntry upd = (UpdateEntry)obj;
                if (VersionNumber.CompareTo(upd.VersionNumber) == 0) //Version numbers are equal
                    return Version.CompareTo(upd.Version);
                else
                    return VersionNumber.CompareTo(upd.VersionNumber);
            }
            throw new ArgumentException("object is not a UpdateEntry");
        }

        public int VersionNumber = 0;
        public string Version = string.Empty;
        public ArrayList scriptNames = new ArrayList();
        public DateTime Date;
        public ArrayList Modules = new ArrayList();
        public bool Apply = false;
    }
}
