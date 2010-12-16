// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BLLBase.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   Base class for all the classes in the BLL
// </summary>
// --------------------------------------------------------------------------------------------------------------------
// Business Logic Layer
// Appleseed.Framework.BLL.Base
// "Business Layer Object" -- base class for all other classes in the BLL
// Created By : bja@reedtek.com Date: 26/04/2003

namespace Appleseed.Framework.BLL.Base
{
    using System;

    /// <summary>
    /// Base class for all the classes in the BLL
    /// </summary>
    public abstract class BLLBase : IDisposable
    {
        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion

        #endregion
    }
}