using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Appleseed.Framework;using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Settings.Cache;
using Label = System.Web.UI.WebControls.Label;
using Page = Appleseed.Framework.Web.UI.Page;
using Path = Appleseed.Framework.Settings.Path;
using Utils = Appleseed.Framework.Helpers.Utilities;

namespace AppleseedWebApplication.Error
{
    /// <summary>
    /// Smart Error page - Jes1111
    /// </summary>
	public partial class SmartError : Page
	{
		//protected PlaceHolder PageContent;

		protected Label Label1;
		protected Label Label2;
		protected Label Label3;
		//protected Esperantus.WebControls.HyperLink ReturnHome;

		protected System.Web.UI.WebControls.Label myTest;
		protected System.Web.UI.WebControls.Label myTest2;

		protected const int _LOGLEVEL_ = 0;
		protected const int _GUID_ = 1;
		protected const int _RENDEREDEVENT_ = 2;

		/// <summary>
		/// Handles the Error event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public void Page_Error(object sender,EventArgs e)
		{
			Response.Redirect("~/app_support/GeneralError.html", true);
		}

		/// <summary>
		/// Handles OnLoad event at Page level<br/>
		/// Performs OnLoad actions that are common to all Pages.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			// load the dedicated CSS
			if ( !this.IsCssFileRegistered("SmartError") )
				this.RegisterCssFile("Mod_SmartError");

			ArrayList storedError = null;
			StringBuilder sb = new StringBuilder(); // to build response text
			int _httpStatusCode = (int)HttpStatusCode.InternalServerError; // default value
			string _renderedEvent = string.Empty;
			string validStatus = "301;307;403;404;410;500;501;502;503;504";

			if ( Request.QueryString[0] != null )
			{
				// is this a "MagicUrl" request
				if ( Request.QueryString[0].StartsWith("404;http://") )
				{
					Hashtable magicUrlList = null;
					string redirectUrl = string.Empty;
					string qPart = string.Empty;
					int qPartPos = Request.QueryString[0].LastIndexOf("/") + 1 ;
					qPart = qPartPos < Request.QueryString[0].Length ? Request.QueryString[0].Substring(qPartPos) : string.Empty;
					if ( qPart.Length > 0 )
					{
						if ( Utils.IsInteger(qPart) )
							redirectUrl = HttpUrlBuilder.BuildUrl(Int32.Parse(qPart));
						else 
						{
							magicUrlList = GetMagicUrlList(Portal.UniqueID);
							if ( magicUrlList != null && magicUrlList.ContainsKey(HttpUtility.HtmlEncode(qPart)) )
							{
								redirectUrl = HttpUtility.HtmlDecode(magicUrlList[HttpUtility.HtmlEncode(qPart)].ToString());
								if ( Utils.IsInteger(redirectUrl) )
									redirectUrl = HttpUrlBuilder.BuildUrl(Int32.Parse(redirectUrl));
							}
						}
						if ( redirectUrl.Length != 0 )
							Response.Redirect(redirectUrl, true);
						else
							_httpStatusCode = (int)HttpStatusCode.NotFound;
					}

				}
				// get status code from querystring
				else if ( Utils.IsInteger(Request.QueryString[0]) && validStatus.IndexOf(Request.QueryString[0]) > -1 )
				{
					_httpStatusCode = int.Parse(Request.QueryString[0]);
				}
			}

			// get stored error
			if (Request.QueryString["eid"] != null && Request.QueryString["eid"].Length > 0)
			{
				storedError = (ArrayList)CurrentCache.Get(Request.QueryString["eid"]);
			}
			if ( storedError != null && storedError[_RENDEREDEVENT_] != null )
				_renderedEvent = storedError[_RENDEREDEVENT_].ToString();
			else
				_renderedEvent = @"<p>No exception event stored or cache has expired.</p>";


			// get home link
			string homeUrl = HttpUrlBuilder.BuildUrl();

			// try localizing message
			try
			{
				switch ( _httpStatusCode )
				{
					case (int)HttpStatusCode.NotFound : // 404
					case (int)HttpStatusCode.Gone : // 410
					case (int)HttpStatusCode.MovedPermanently : // 301
					case (int)HttpStatusCode.TemporaryRedirect : // 307
						sb.AppendFormat("<h3>{0}</h3>",General.GetString("SMARTERROR_404HEADING","Page Not Found", null));
						sb.AppendFormat("<p>{0}</p>",General.GetString("SMARTERROR_404TEXT","We're sorry, but there is no page that matches your entry. It is possible you typed the address incorrectly, or the page may no longer exist. You may wish to try another entry or choose from the links below, which we hope will help you find what you’re looking for.", null));
						break;
					case (int)HttpStatusCode.Forbidden : // 403
						sb.AppendFormat("<h3>{0}</h3>",General.GetString("SMARTERROR_403HEADING","Not Authorised", null));
						sb.AppendFormat("<p>{0}</p>",General.GetString("SMARTERROR_403TEXT","You do not have the required authority for the requested page or action.", null));
						break;
					default :
						sb.AppendFormat("<h3>{0}</h3>",General.GetString("SMARTERROR_500HEADING","Our Apologies", null));
						sb.AppendFormat("<p>{0}</p>",General.GetString("SMARTERROR_500TEXT","We're sorry, but we were unable to service your request. It's possible that the problem is a temporary condition.", null));
						break;
				}
				sb.AppendFormat("<p><a href=\"{0}\">{1}</a></p>", homeUrl,General.GetString("HOME","Home Page",null));
			}
			catch // default to english message
			{
				switch ( _httpStatusCode )
				{
					case (int)HttpStatusCode.NotFound :
						sb.Append("<h3>Page Not Found</h3>");
						sb.Append("<p>We're sorry, but there is no page that matches your entry. It is possible you typed the address incorrectly, or the page may no longer exist. You may wish to try another entry or choose from the links below, which we hope will help you find what you’re looking for.</p>");
						break;
					case (int)HttpStatusCode.Forbidden :
						sb.Append("<h3>Not Authorised</h3>");
						sb.Append("<p>You do not have the required authority for the requested page or action.</p>");
						break;
					default :
						sb.Append("<h3>Our Apologies</h3>");
						sb.AppendFormat("<p>We're sorry, but we were unable to service your request. It's possible that the problem is a temporary condition.</p>");
						break;
				}
				sb.AppendFormat("<p><a href=\"{0}\">{1}</a></p>",homeUrl, "Home Page");
			}

			// find out if user is on allowed IP Address
			if ( Request.UserHostAddress != null
				&& Request.UserHostAddress.Length > 0 )
			{
				// construct IPList
				string[] lockKeyHolders = Config.LockKeyHolders.Split(new char[]{';'}); //ConfigurationSettings.AppSettings["LockKeyHolders"].Split(new char[]{';'});
				IPList ipList = new IPList();
				try
				{
					foreach ( string lockKeyHolder in lockKeyHolders )
					{
						if ( lockKeyHolder.IndexOf("-") > -1 )
							ipList.AddRange(lockKeyHolder.Substring(0, lockKeyHolder.IndexOf("-")), lockKeyHolder.Substring(lockKeyHolder.IndexOf("-") + 1));
						else
							ipList.Add(lockKeyHolder);
					}

					// check if requestor's IP address is in allowed list
					if ( ipList.CheckNumber(Request.UserHostAddress) )
					{
						// we can show error details
						sb.AppendFormat("<h3>{0} - {1}</h3>",General.GetString("SMARTERROR_SUPPORTDETAILS_HEADING","Support Details", null), _httpStatusCode.ToString());
						sb.Append(_renderedEvent);
					}
				}
				catch
				{
					// if there was a problem, let's assume that user is not authorised
				}
			}
            Control pageContent = this.FindControl("PageContent");
            if (pageContent == null){
                pageContent = this.Master.FindControl("Content").FindControl("PageContent");
            }

            pageContent.Controls.Add(new LiteralControl(sb.ToString()));
			Response.StatusCode = _httpStatusCode;
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}

		/// <summary>
		/// Gets the magic URL list.
		/// </summary>
		/// <param name="portalID">The portal ID.</param>
		/// <returns></returns>
		private Hashtable GetMagicUrlList(string portalID)
		{
			Hashtable _result = new Hashtable();

			if ( Cache["Appleseed_MagicUrlList_" + Portal.UniqueID] == null )
			{
				string myPath = Server.MapPath(Path.WebPathCombine(portalSettings.PortalFullPath, "MagicUrl/MagicUrlList.xml"));
				if ( File.Exists(myPath) )
				{
					XmlDocument xmlDoc = new XmlDocument();
					xmlDoc.Load(myPath);
					XmlNodeList xnl = xmlDoc.SelectNodes("/MagicUrlList/MagicUrl");
					foreach ( XmlNode node in xnl )
					{
						try
						{
							_result.Add(node.Attributes["key"].Value, HttpUtility.HtmlDecode(node.Attributes["value"].Value));
						}
						catch
						{}
					}
					Cache.Insert("Appleseed_MagicUrlList_" + Portal.UniqueID, _result, new CacheDependency(myPath));
				}
			}
			else
			{
				_result = (Hashtable) Cache["Appleseed_MagicUrlList_" + Portal.UniqueID];
			}

			return _result;
		}

       
		#region Web Form Designer generated code
        /// <summary>
        /// Raises the Init event.
        /// </summary>
        /// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();

			//ReturnHome.NavigateUrl = HttpUrlBuilder.BuildUrl();
		
			base.OnInit(e);
		}

        /// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{    
            this.Error += new EventHandler(this.Page_Error);
		}
		#endregion
    }
}
