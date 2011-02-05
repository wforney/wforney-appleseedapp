// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageStripDetails.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   PageStripDetails Class encapsulates the tabstrip details
//   -- PageName, PageID and PageOrder -- for a specific Page in the Portal
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.Site.Configuration
{
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Xml.Serialization;

    using Appleseed.Framework.Settings.Cache;

    /// <summary>
    /// PageStripDetails Class encapsulates the tabstrip details
    ///     -- PageName, PageID and PageOrder -- for a specific Page in the Portal
    /// </summary>
    [XmlType(TypeName = "MenuItem")]
    public class PageStripDetails
    {
        #region Constants and Fields

        /// <summary>
        /// The authorized roles.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlAttribute("AuthRoles")]
        public string AuthorizedRoles;

        /// <summary>
        /// The page image.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlAttribute("PageImage")]
        public string PageImage;

        /// <summary>
        /// The page index.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlAttribute("PageIndex")]
        public int PageIndex;

        /// <summary>
        /// The page layout.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlAttribute("PageLayout")]
        public string PageLayout;

        /// <summary>
        /// The page name.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlAttribute("Label")]
        public string PageName;

        /// <summary>
        /// The page order.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlAttribute("PageOrder")]
        public int PageOrder;

        /// <summary>
        /// The parent page id.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlAttribute("ParentPageID")]
        public int ParentPageID;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the page ID.
        /// </summary>
        /// <value>The page ID.</value>
        /// <remarks>
        /// </remarks>
        [XmlAttribute("ID")]
        public int PageID { get; set; }

        /// <summary>
        ///     Gets the pages.
        /// </summary>
        /// <value>The pages.</value>
        /// <remarks>
        /// </remarks>
        [XmlArray(ElementName = "MenuGroup", IsNullable = false)]
        public Collection<PageStripDetails> Pages
        {
            get
            {
                var cacheKey = Key.TabNavigationSettings(this.PageID, Thread.CurrentThread.CurrentUICulture.ToString());
                Collection<PageStripDetails> tabs;

                if (!CurrentCache.Exists(cacheKey))
                {
                    tabs = PageSettings.GetPageSettingsPagesBox(this.PageID);
                    CurrentCache.Insert(cacheKey, tabs);
                }
                else
                {
                    tabs = (Collection<PageStripDetails>)CurrentCache.Get(cacheKey);
                }

                return tabs;
            }
        }

        #endregion
    }
}