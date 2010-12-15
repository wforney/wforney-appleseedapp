using System;
//===============================================================================
//
//	 Business Logic Layer
//
//	Appleseed.Framework.BLL.Base
//
//	"Business Layer Object" -- base class for all other classes in the BLL
//
//===============================================================================
//
//
//
// Created By : bja@reedtek.com Date: 26/04/2003
//===============================================================================
namespace Appleseed.Framework.BLL.Base
{
	///	<summary> 
	///	Base class for all the classes in the BLL
	///	</summary>
	public abstract class BLLBase : IDisposable
	{

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
		} // end of Dispose
	}
}
