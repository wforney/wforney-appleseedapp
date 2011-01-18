using System.IO;
using System.Net;

namespace Appleseed.Framework.Helpers 
{ 

	/// <summary> 
	/// HttpHelper
	/// by Phillo 22/01/2003
	/// </summary> 
	public class HttpHelper 
	{

        /// <summary>
        /// Gets the HTTP stream.
        /// </summary>
        /// <param name="pUrl">The p URL.</param>
        /// <param name="pTimeout">The p timeout.</param>
        /// <returns>A System.IO.Stream value...</returns>
		public Stream GetHttpStream(string pUrl, int pTimeout)
		{ 
			// handle on the remote resource 
			HttpWebRequest wr = (HttpWebRequest) WebRequest.Create(pUrl); 
//jes1111 - not needed: global proxy is set in Global class Application Start
//			if(PortalSettings.GetProxy() != null) 
//				wr.Proxy = PortalSettings.GetProxy(); 
			// set the HTTP properties 
			wr.Timeout = pTimeout * 1000; // milliseconds to seconds 
			// Read the response 
			WebResponse resp = wr.GetResponse(); 
			// Stream read the response 
			return(resp.GetResponseStream()); 
		} 
	} 
}