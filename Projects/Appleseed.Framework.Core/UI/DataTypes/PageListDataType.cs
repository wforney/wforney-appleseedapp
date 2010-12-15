using System;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Site.Data;
using System.Web;
using System.Collections;


namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// PageListDataType
    /// </summary>
    public class PageListDataType : CustomListDataType
    {
        /// <summary>
        /// 
        /// </summary>
        public PageListDataType()
            : base(GetPageList(), "Name", "Id")
        {
            InnerDataType = PropertiesDataType.PageList;
        }
        
        private static ArrayList GetPageList()
        {
            PortalSettings portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            return new PagesDB().GetPagesFlat(portalSettings.PortalID);

        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "Pages List"; }
        }

        public string GetDefaultValue()
        {
            return ((PageItem)((ArrayList)this.InnerDataSource)[0]).ID.ToString();
        }

    }
}