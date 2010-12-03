using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Globalization;

using UrlRewritingNet.Web;
using System.Web;

namespace Appleseed.Framework.UrlRewriting {

    public class AppleseedUrlRewritingRule : RewriteRule {

        private string handlerFlag = "site";
        private string defaultSplitter = "__";
        private bool pageIdNoSplitter = false;
        private string friendlyPageName = "Default.aspx";

        public override void Initialize( UrlRewritingNet.Configuration.RewriteSettings rewriteSettings ) {
            base.Initialize( rewriteSettings );

            if ( !string.IsNullOrEmpty( rewriteSettings.Attributes[ "handlerflag" ] ) ) {
                handlerFlag = ( rewriteSettings.Attributes[ "handlerflag" ] ).ToLower( CultureInfo.InvariantCulture );
            }

            if ( !string.IsNullOrEmpty( rewriteSettings.Attributes[ "handlersplitter" ] ) ) {
                defaultSplitter = rewriteSettings.Attributes[ "handlersplitter" ].ToString();
            }
            else {
                if ( ConfigurationManager.AppSettings[ "HandlerDefaultSplitter" ] != null )
                    defaultSplitter = ConfigurationManager.AppSettings[ "HandlerDefaultSplitter" ].ToString();
            }

            if ( !string.IsNullOrEmpty( rewriteSettings.Attributes[ "pageidnosplitter" ] ) ) {
                pageIdNoSplitter = bool.Parse( rewriteSettings.Attributes[ "pageidnosplitter" ] );
            }

            if ( !string.IsNullOrEmpty( rewriteSettings.Attributes[ "friendlyPageName" ] ) ) {
                friendlyPageName = rewriteSettings.Attributes[ "friendlyPageName" ];
            }
        }

        public override bool IsRewrite( string requestUrl ) {
            return requestUrl.Contains( "/" + handlerFlag + "/" );
        }

        public override string RewriteUrl( string url ) {

            string rewrittenUrl = url.Substring( 0, url.IndexOf( "/" + handlerFlag + "/" ) );

            string[] parts = url.Substring( url.IndexOf( "/" + handlerFlag + "/" ) + ( "/" + handlerFlag + "/" ).Length ).Split( '/' );

            rewrittenUrl += "/" + friendlyPageName;
            string queryString = "?pageId=" + parts[ parts.Length - 2 ];

            string queryStringParam = string.Empty;
            if ( parts.Length > 2 ) {
                for ( int i = 0; i < (parts.Length-2); i++ ) {
                    queryStringParam = parts[ i ];

                    if (queryStringParam.IndexOf(defaultSplitter) >= 0)
                    {
                        queryString += "&" + queryStringParam.Substring(0, queryStringParam.IndexOf(defaultSplitter));
                        queryString += "=" + queryStringParam.Substring(queryStringParam.IndexOf(defaultSplitter) + defaultSplitter.Length);
                    }
                }
            }

            HttpContext.Current.RewritePath( rewrittenUrl, "", queryString );

            return rewrittenUrl + queryString;
        }
    }
}