using System.Collections;

namespace Appleseed.Framework.Site.Configuration
{
    /// <summary>
    /// Box tab
    /// </summary>
    [History("jminond", "2005/03/10", "Tab to page conversion")]
    public class PagesBox : CollectionBase
    {
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public int Add(PageStripDetails t)
        {
            return InnerList.Add(t);
        }

        /// <summary>
        /// PageStripDetails indexer
        /// </summary>
        /// <value></value>
        public PageStripDetails this[int index]
        {
            get { return ((PageStripDetails) InnerList[index]); }
        }
    }
}