using System.Web;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;

namespace Appleseed.Framework.DataTypes
{
    /// <summary>
    /// PortalUrlDataType
    /// </summary>
    public class PortalUrlDataType : StringDataType
    {
        /// <remarks>
        /// Change visibility to private because now we cache internal values.
        /// Could be moved to protected again if we transform in a property and invalidate cache.
        /// </remarks>
        private string _portalPathPrefix = string.Empty;


        /// <summary>
        /// Initializes a new instance of the <see cref="PortalUrlDataType"/> class.
        /// </summary>
        public PortalUrlDataType()
        {
            InnerDataType = PropertiesDataType.String;

            //InitializeComponents();

            if (HttpContext.Current.Items["PortalSettings"] != null)
            {
                // Obtain PortalSettings from Current Context
                PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
                _portalPathPrefix = portalSettings.PortalFullPath;
                if (!_portalPathPrefix.EndsWith("/"))
                    _portalPathPrefix += "/";
            }
        }

        /// <summary>
        /// Use this on portalsetting or when you want turn off automatic discovery
        /// </summary>
        /// <param name="PortalFullPath">The portal full path.</param>
        public PortalUrlDataType(string PortalFullPath)
        {
            InnerDataType = PropertiesDataType.String;

            //			InitializeComponents();			

            _portalPathPrefix = PortalFullPath;
        }

        /// <summary>
        /// Gets the portal path prefix.
        /// </summary>
        /// <value>The portal path prefix.</value>
        protected string PortalPathPrefix
        {
            get { return _portalPathPrefix; }
        }

        private string _innerFullPath;

        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <value>The full path.</value>
        public override string FullPath
        {
            get
            {
                if (_innerFullPath == null)
                {
                    _innerFullPath = Path.WebPathCombine(_portalPathPrefix, Value);
                    _innerFullPath = _innerFullPath.TrimEnd('/'); //Removes trailings
                }
                return _innerFullPath;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public override string Value
        {
            get { return (innerValue); }
            set
            {
                //Remove portal path if present
                if (value.StartsWith(_portalPathPrefix))
                    innerValue = value.Substring(_portalPathPrefix.Length);
                else
                    innerValue = value;
                //Reset _innerFullPath
                _innerFullPath = null;
            }
        }

        /// <summary>
        /// String
        /// </summary>
        /// <value></value>
        public override string Description
        {
            get { return "Url relative to Portal"; }
        }
    }
}