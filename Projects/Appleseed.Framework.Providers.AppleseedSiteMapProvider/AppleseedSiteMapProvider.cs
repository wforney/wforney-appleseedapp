using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Appleseed.Framework.Providers.AppleseedSiteMapProvider {
	
	public abstract class AppleseedSiteMapProvider : StaticSiteMapProvider {

        public abstract void ClearCache();

        public static void ClearAllAppleseedSiteMapCaches() {
            // Removing Sitemap Cache
            foreach (SiteMapProvider siteMap in SiteMap.Providers) {
                if (siteMap is AppleseedSiteMapProvider) {
                    ((AppleseedSiteMapProvider)siteMap).ClearCache();
                }
            }
        }
    }
}
