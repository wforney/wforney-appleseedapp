using System;
using System.Threading;
using System.Web;
using Appleseed.Framework.Configuration;
//===============================================================================
//
//	Base Logic Layer
//
//	Appleseed.Framework.BLL.Utils
//
//
//===============================================================================
// Cookie Utility
//===============================================================================
namespace Appleseed.Framework.BLL.Utils
{
	/// <summary>
	/// This class manages cookies
	/// </summary>
	sealed class CookieUtil
	{
		//  minutes
		static TimeSpan expire_ = new TimeSpan(0, 0, 25, 0);
		/// <summary>
		/// Initializes the <see cref="CookieUtil"/> class.
		/// </summary>
		static CookieUtil()
		{
		}

		#region Interface

		/// <summary>
		/// Cookie Expiration
		/// </summary>
		/// <value>The expiration.</value>
		public static TimeSpan Expiration
		{
			get
			{
				return expire_;
			}
			set
			{
				Monitor.Enter(expire_);
				expire_ = value;
				Monitor.Exit(expire_);
			}
		} // end of Expiration

		/// <summary>
		/// Add the cookie
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public static void Add(string name, object value)
		{

			// is it a string
			if (value is string)
				addImpl(name, (string)value);
		} // end of Add

		/// <summary>
		/// Add the cookie
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public static void Add(int name, object value)
		{

			// is it a string
			if (value != null && value is string)
				addImpl(name.ToString(), (string)value);
		} // end of Add

		/// <summary>
		/// Remove a cookie
		/// </summary>
		/// <param name="name">The name.</param>
		public static void Remove(int name)
		{
			removeImpl(name.ToString());
		} // end of Remove

		/// <summary>
		/// Remove a Cookie
		/// </summary>
		/// <param name="name">The name.</param>
		public static void Remove(string name)
		{
			removeImpl(name);
		} // end of Remove

		/// <summary>
		/// Retrieve a cookie
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static object Retrieve(int name)
		{
			return retrieveImpl(name.ToString());
		} // end of Retrieve

		/// <summary>
		/// Retrieve a Cookie
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static object Retrieve(string name)
		{
			return retrieveImpl(name);
		} // end of Retrieve

		#endregion

		#region Implementation

		// Implementation
		/// <summary>
		/// Implemented the remove functionality
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		private static object retrieveImpl(string name)
		{
			return HttpContext.Current.Request.Cookies[name];
		} // end of retrieveImpl

		/// <summary>
		/// Implemented the remove functionality
		/// </summary>
		/// <param name="name">The name.</param>
		private static void removeImpl(string name)
		{
			HttpCookie hcookie = HttpContext.Current.Response.Cookies[name];

			if (hcookie != null)
				// clear the cookie
				clearCookie(ref hcookie);
		}

		/// <summary>
		/// Implementation of the add cookie
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		private static void addImpl(string name, string value)
		{
			// create cookie
			HttpCookie hcookie = new HttpCookie(name, value);
			setCookie(ref hcookie);
		} // end of addImpl

		/// <summary>
		/// Set cookie
		/// </summary>
		/// <param name="cookie">The cookie.</param>
		private static void setCookie(ref HttpCookie cookie)
		{
			// expire in timespan
			cookie.Expires = DateTime.Now + expire_;
			cookie.Path = GlobalInternalStrings.CookiePath;

			// see if cookie exists, otherwise create it
			if (HttpContext.Current.Response.Cookies[cookie.Name] != null)
				HttpContext.Current.Response.Cookies.Set(cookie);

			else
				HttpContext.Current.Response.Cookies.Add(cookie);
		} // end of setCookie

		/// <summary>
		/// Clear the cookie
		/// </summary>
		/// <param name="cookie">The cookie.</param>
		private static void clearCookie(ref HttpCookie cookie)
		{
			cookie.Expires = new DateTime(1999, 10, 12);
			cookie.Value = null;
			//HttpContext.Current.Response.Cookies.Remove(cookie.Name);
		} // end of clearCookie

		#endregion

	}
}
