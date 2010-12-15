using System;
using System.Configuration;
using System.Xml;

namespace Appleseed.Framework.Provider
{
    /// <summary>
    /// Summary description for ProviderConfigurationHandler.
    /// </summary>
    public class ProviderConfigurationHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates the specified parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="context">The context.</param>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public virtual object Create(Object parent, Object context, XmlNode node)
        {
            ProviderConfiguration config = new ProviderConfiguration();
            config.LoadValuesFromConfigurationXml(node);
            return config;
        }
    }
}