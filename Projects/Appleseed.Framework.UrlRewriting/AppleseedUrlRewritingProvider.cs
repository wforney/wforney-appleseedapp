using System;
using System.Collections.Generic;
using System.Text;
using UrlRewritingNet.Configuration.Provider;
using System.Web.SessionState;

namespace Appleseed.Framework.UrlRewriting
{

    public class AppleseedUrlRewritingProvider : UrlRewritingProvider, IRequiresSessionState
    {


        public override UrlRewritingNet.Web.RewriteRule CreateRewriteRule()
        {
            return new AppleseedUrlRewritingRule();
        }
    }
}

