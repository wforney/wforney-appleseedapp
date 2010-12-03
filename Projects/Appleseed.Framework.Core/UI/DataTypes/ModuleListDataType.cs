using System;
using System.Web;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Site.Data;

namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// ModuleListDataType
    /// </summary>
    public class ModuleListDataType : ListDataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleListDataType"/> class.
        /// </summary>
        /// <param name="ModuleType">The Module name</param>
        public ModuleListDataType(string ModuleType)
        {
            InnerDataSource = ModuleType;
            //InitializeComponents();
        }

        /// <summary>
        /// Gets DataSource
        /// Should be overrided from inherited classes
        /// </summary>
        /// <value>The data source.</value>
        public override object DataSource
        {
            get
            {
                // Obtain PortalSettings from Current Context
                PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
                return new ModulesDB().GetModulesByName(InnerDataSource.ToString(), portalSettings.PortalID);
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "Module List"; }
        }

        /// <summary>
        /// Gets or sets the data value field.
        /// </summary>
        /// <value>The data value field.</value>
        public override string DataValueField
        {
            get { return "ModuleID"; }
            set { throw new ArgumentException("Value cannot be set", "ModuleID"); }
        }

        /// <summary>
        /// Gets or sets the data text field.
        /// </summary>
        /// <value>The data text field.</value>
        public override string DataTextField
        {
            get { return "ModuleTitle"; }
            set { throw new ArgumentException("Value cannot be set", "ModuleTitle"); }
        }
    }
}