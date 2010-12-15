using System.Threading;
using System.Xml.Serialization;
using Appleseed.Framework.Settings.Cache;

namespace Appleseed.Framework.Site.Configuration
{

	/// <summary>
	/// PageStripDetails Class encapsulates the tabstrip details
	/// -- PageName, PageID and PageOrder -- for a specific Page in the Portal
	/// </summary>
	[XmlType(TypeName = "MenuItem")]
	public class PageStripDetails
	{
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute("AuthRoles")]
		public string AuthorizedRoles;
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute("ParentPageID")]
		public int ParentPageID;
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute("PageImage")]
		public string PageImage;
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute("PageIndex")]
		public int PageIndex;
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute("PageLayout")]
		public string PageLayout;
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute("Label")]
		public string PageName;
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		[XmlAttribute("PageOrder")]
		public int PageOrder;
		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		int pageID;

		/// <summary>
		/// Gets or sets the page ID.
		/// </summary>
		/// <value>The page ID.</value>
		/// <remarks>
		/// </remarks>
		[XmlAttribute("ID")]
		public int PageID
		{
			get
			{
				return pageID;
			}
			set
			{
				pageID = value;
			}
		}

		/// <summary>
		/// Gets the pages.
		/// </summary>
		/// <value>The pages.</value>
		/// <remarks>
		/// </remarks>
		[XmlArray(ElementName = "MenuGroup", IsNullable = false)]
        public PagesBox Pages
		{
			get
			{
				string cacheKey = Key.TabNavigationSettings(pageID, Thread.CurrentThread.CurrentUICulture.ToString());
                Appleseed.Framework.Site.Configuration.PagesBox tabs;

				if (!CurrentCache.Exists(cacheKey))
				{
					tabs = PageSettings.GetPageSettingsPagesBox(pageID);
					CurrentCache.Insert(cacheKey, tabs);
				}
				else
				{
                    tabs = (Appleseed.Framework.Site.Configuration.PagesBox)CurrentCache.Get(cacheKey);
				}
				return tabs;
			}
		}
	}
}
