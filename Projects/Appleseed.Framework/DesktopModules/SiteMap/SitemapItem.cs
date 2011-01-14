using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules.Sitemap
{
	/// <summary>
	/// A sitemap item. This just defines the simple data needed for the sitemap items.
	/// </summary>
	public class SitemapItem
	{
		/// <summary>
		/// Item Id
		/// </summary>
		public int ID;
		/// <summary>
		/// Item Name
		/// </summary>
		public string Name;
		/// <summary>
		/// Item URL
		/// </summary>
		public string Url;
		/// <summary>
		/// Item Nest Level
		/// </summary>
		public int NestLevel;

		/// <summary>
		/// SitemapItem Contstructor
		/// </summary>
		public SitemapItem()
		{
		}

        /// <summary>
        /// SitemapItem Contstructor
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="url">The URL.</param>
        /// <param name="nestlevel">The nestlevel.</param>
		public SitemapItem(int id, string name, string url, int nestlevel)
		{
			ID = id;
			Name = name;
			Url = url;
			NestLevel = nestlevel;
		}
	}
}
