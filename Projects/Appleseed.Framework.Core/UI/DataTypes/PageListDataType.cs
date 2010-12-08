namespace Appleseed.Framework.DataTypes
{
    using System.Collections;
    using System.Web;

    using Appleseed.Framework.Site.Configuration;
    using Appleseed.Framework.Site.Data;

    /// <summary>
    /// Page List Data Type
    /// </summary>
    public class PageListDataType : CustomListDataType
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PageListDataType"/> class. 
        /// The page list data type.
        /// </summary>
        public PageListDataType()
            : base(GetPageList(), "Name", "Id")
        {
            this.InnerDataType = PropertiesDataType.PageList;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get
            {
                return "Pages List";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <returns>The default value.</returns>
        public string GetDefaultValue()
        {
            return ((PageItem)((ArrayList)this.InnerDataSource)[0]).ID.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the page list.
        /// </summary>
        /// <returns>The page list.</returns>
        private static ArrayList GetPageList()
        {
            var portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            return new PagesDB().GetPagesFlat(portalSettings.PortalID);
        }

        #endregion
    }
}