using System.Web;
using System.Web.SessionState;
//===============================================================================
//
//	Business Logic Layer
//
//	Appleseed.Framework.BLL.Utils
//
// Classes to Manage the User Web Collection
//
//===============================================================================
//  Created By : bja@reedtek.com Date: 26/04/2003
//===============================================================================
namespace Appleseed.Framework.BLL.Utils
{
	/// <summary>
	/// Define the Web Bag Interface as the
	/// repository for temporary data
	/// </summary>
	public interface IWebBagHolder
	{

		// attributes to get and set stored values
		// name-value pairs
		#region Attributes

		/// <summary>
		/// Get/Set values from integer key
		/// </summary>
		/// <value></value>
		object this[int index]
		{
			get;
			set;
		}
		/// <summary>
		/// Get/Set values from string key
		/// </summary>
		/// <value></value>
		object this[string index]
		{
			get;
			set;
		}

		#endregion

	} // end of IWebBagHolder

	//
	// These are the current concrete bag holders for the data.
	// 
	// 
	#region Cookie Bag

	/////////////////////////////////////////////////////////////////////////
	/// <summary>
	///  This class utilizes cookies to store the data
	///  Encryption was not added though and may not be complete
	/// </summary>
	/// /////////////////////////////////////////////////////////////////////////
	public class CookieBag : IWebBagHolder
	{
		/// <summary>
		/// Initializes the <see cref="CookieBag"/> class.
		/// </summary>
		static CookieBag()
		{
		}

		/// <summary>
		/// Default property for accessing resources
		/// <param name="index">
		/// Index for the desired resource</param>
		/// 	<returns>
		/// Resource string value</returns>
		/// </summary>
		/// <value></value>
		public object this[int index]
		{
			get
			{
				HttpCookie cookie = (HttpCookie)CookieUtil.Retrieve(index);

				if (cookie != null)
					return cookie.Value;
				return null;
			}
			set
			{
				lock (this)
				{
					CookieUtil.Add(index, value);
				}
			}
		} // endof this[string]

		/// <summary>
		/// Get Data Value
		/// </summary>
		/// <value></value>
		public object this[string index]
		{
			get
			{
				HttpCookie cookie = (HttpCookie)CookieUtil.Retrieve(index);

				if (cookie != null)
					return cookie.Value;
				return null;
			}
			set
			{
				lock (this)
				{
					CookieUtil.Add(index, value);
				}
			}
		} // endof this[int]
	} // end of cookiebag

	#endregion

	#region Session Bag

	/////////////////////////////////////////////////////////////////////////
	///
	/// <summary>
	/// The Session bag will attempt to use server-side Session Mgmt. However
	/// in the event session is disabled fallback to cookies; otherwise, fallback
	/// to the database :(
	/// </summary>
	//////////////////////////////////////////////////////////////////////////
	public class SessionBag : IWebBagHolder
	{
		/// <summary>
		/// Initializes the <see cref="SessionBag"/> class.
		/// </summary>
		static SessionBag()
		{
		}

		/// <summary>
		/// Default property for accessing resources
		/// <param name="index">
		/// Index for the desired resource</param>
		/// 	<returns>
		/// Resource string value</returns>
		/// </summary>
		/// <value></value>
		public object this[int index]
		{
			get
			{
				object obj = null;

				try
				{

					// session state enabled
					if (HttpContext.Current.Session != null)
						obj = HttpContext.Current.Session[index];
				}

				catch
				{
					obj = null;
				}
				return obj;
			}
			set
			{

				try
				{

					// session state enabled
					if (HttpContext.Current.Session != null)
						HttpContext.Current.Session[index] = value;
				}

				catch
				{
				}
			}
		}

		/// <summary>
		/// get data back
		/// </summary>
		/// <value></value>
		public object this[string index]
		{
			get
			{
				object obj = null;

				try
				{

					// session state  enabled
					if (HttpContext.Current.Session != null)
						obj = HttpContext.Current.Session[index];
				}

				catch
				{
					obj = null;
				}
				return obj;
			}
			set
			{

				try
				{

					// session state  enabled
					if (HttpContext.Current.Session != null)
						HttpContext.Current.Session[index] = value;
				}

				catch
				{
				}
			}
		}
	} // end of SessionBag

	#endregion

	#region Application Bag

	/////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// The Application bag will attempt to use server-side Application Mgmt.
	/// This is the fallback. The database could also be a fallback as well
	/// but then anonnymous user information would be logged there as well which
	/// I don't you would want
	/// </summary>
	/////////////////////////////////////////////////////////////////////////
	public class ApplicationBag : IWebBagHolder
	{
		/// <summary>
		/// Initializes the <see cref="ApplicationBag"/> class.
		/// </summary>
		static ApplicationBag()
		{
		}

		/// <summary>
		/// Default property for accessing resources
		/// <param name="index">
		/// Index for the desired resource</param>
		/// 	<returns>
		/// Resource string value</returns>
		/// </summary>
		/// <value></value>
		public object this[int index]
		{
			get
			{
				return HttpContext.Current.Application.Get(index);
			}
			set
			{
				HttpContext.Current.Application.Lock();
				HttpContext.Current.Application.Add(index.ToString(), value);
				HttpContext.Current.Application.UnLock();
			}
		}

		/// <summary>
		/// Get data value
		/// </summary>
		/// <value></value>
		public object this[string index]
		{
			get
			{
				return HttpContext.Current.Application.Get(index);
			}
			set
			{
				HttpContext.Current.Application.Lock();
				HttpContext.Current.Application.Add(index, value);
				HttpContext.Current.Application.UnLock();
			}
		}
	} // end of ApplicationBag

	#endregion

	//////////////////////////////////////////////////////////////////////
	/// <summary>
	/// This singleton factory creates the appropriate Bag for a user to hold data
	/// The data retention mechanism varies.
	/// </summary>
	/// //////////////////////////////////////////////////////////////////////
	public sealed class BagFactory
	{

		/// <summary>
		/// valid bag types
		/// </summary>
		public enum BagFactoryType
		{
			/// <summary>
			///     
			/// </summary>
			/// <remarks>
			///     
			/// </remarks>
			None = 0,
			/// <summary>
			///     
			/// </summary>
			/// <remarks>
			///     
			/// </remarks>
			ApplicationType = 1,
			/// <summary>
			///     
			/// </summary>
			/// <remarks>
			///     
			/// </remarks>
			SessionType = 2,
			/// <summary>
			///     
			/// </summary>
			/// <remarks>
			///     
			/// </remarks>
			CookieType = 3,
			/// <summary>
			///     
			/// </summary>
			/// <remarks>
			///     
			/// </remarks>
			DbType = 4,
		}

		/// <summary>
		/// the singleton class instance (ONLY ONE -- APPLICATION LEVEL )
		/// </summary>
		public static readonly BagFactory instance = new BagFactory();

		/// <summary>
		/// Create the Bag according to what is available. First use Cookies,
		/// then try Session and then fallback to Application.
		/// </summary>
		/// <returns></returns>
		public IWebBagHolder create()
		{

			// if no session, use cookies
			if (HttpContext.Current.Session == null)
				return new CookieBag();

				// using a session
			else if (SessionStateMode.InProc == HttpContext.Current.Session.Mode)
				// use the session
				return new SessionBag();
			// get the default bag
			return new ApplicationBag();
		} // end of create

		/// <summary>
		/// Create the Bag according to what is available. First use Cookies,
		/// then try Session and then fallback to Application.
		/// </summary>
		/// <param name="bag_type">The bag_type.</param>
		/// <returns></returns>
		public IWebBagHolder create(BagFactoryType bag_type)
		{

			// application type
			if (bag_type == BagFactoryType.ApplicationType)
				// get the appli. bag
				return new ApplicationBag();

				// session type
			else if (bag_type == BagFactoryType.SessionType &&
				// if session is not available, then use the default
				HttpContext.Current.Session != null)
				return new SessionBag();

				// cookie type
			else if (bag_type == BagFactoryType.CookieType)
				return new CookieBag();
			// get the default bag
			return new ApplicationBag();
		} // end of create

		/// <summary>
		///     don't allow for construction outside of this class
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		private BagFactory() { }
	} // end of BagFactory
}
