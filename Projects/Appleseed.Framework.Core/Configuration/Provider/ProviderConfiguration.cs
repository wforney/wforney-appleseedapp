using System.Collections;
using System.Configuration;
using System.Xml;

namespace Appleseed.Framework.Provider
{
    /// <summary>
    /// Summary description for ProviderConfiguration.
    /// </summary>
    public class ProviderConfiguration
    {
        private string defaultProvider;
        private Hashtable providers = new Hashtable();

        /// <summary>
        /// Gets the default provider
        /// </summary>
        /// <value>The default provider.</value>
        public string DefaultProvider
        {
            get { return defaultProvider; }
        }

        /// <summary>
        /// Gets the loaded providers
        /// </summary>
        /// <value>The providers.</value>
        public Hashtable Providers
        {
            get { return providers; }
        }

        /// <summary>
        /// Gets the configuration object for the specified provider
        /// </summary>
        /// <param name="provider">Name of the provider object to retrieve</param>
        /// <returns></returns>
        public static ProviderConfiguration GetProviderConfiguration(string provider)
        {
            return (ProviderConfiguration) ConfigurationManager.GetSection("providers/" + provider);
        }

        /// <summary>
        /// Loads provider information from the configuration node
        /// </summary>
        /// <param name="node">Node representing configuration information</param>
        public void LoadValuesFromConfigurationXml(XmlNode node)
        {
            XmlAttributeCollection attributeCollection = node.Attributes;

            // Get the default provider
            defaultProvider = attributeCollection["defaultProvider"].Value;

            // Read child nodes
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "providers")
                    GetProviders(child);
            }
        }

        /// <summary>
        /// Configures Provider(s) based on the configuration node
        /// </summary>
        /// <param name="node"></param>
        private void GetProviders(XmlNode node)
        {
            foreach (XmlNode provider in node.ChildNodes)
            {
                switch (provider.Name)
                {
                    case "add":
                        providers.Add(provider.Attributes["name"].Value,
                                      new ProviderSettings(provider.Attributes["name"].Value,
                                                           provider.Attributes["type"].Value));
                        break;

                    case "remove":
                        providers.Remove(provider.Attributes["name"].Value);
                        break;

                    case "clear":
                        providers.Clear();
                        break;
                }
            }
        }
    }
}