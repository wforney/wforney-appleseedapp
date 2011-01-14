// Created by Manu
using System;
using System.Configuration;
using System.Configuration.Provider;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Appleseed.Framework.Provider;

namespace Appleseed.Framework.Logging
{
    /// <summary>
    /// Summary description for LogProvider.
    /// </summary>
    public abstract class LogProvider : ProviderBase
    {
        /// <summary>
        /// Camel case. Must match web.config section name
        /// </summary>
        private const string providerType = "log";

        /// <summary>
        /// Initializes a new instance of the LogProvider class.
        /// </summary>
        protected LogProvider()
        {
        }

        /// <summary>
        /// Instances this instance.
        /// </summary>
        /// <returns></returns>
        public static LogProvider Instance()
        {
            // Use the cache because the reflection used later is expensive
            Cache cache = HttpRuntime.Cache;
            string cacheKey;

            // Get the names of providers
            ProviderConfiguration config;
            config = ProviderConfiguration.GetProviderConfiguration(providerType);

            //If config not found (missing web.config)
            if (config == null)
            {
                //Try to provide a default anyway
                XmlDocument defaultNode = new XmlDocument();
                defaultNode.LoadXml(
                    "<log defaultProvider=\"Log4NetLog\"><providers><clear /><add name=\"Log4NetLog\" type=\"Appleseed.Framework.Logging.Log4NetLogProvider, Appleseed.Provider.Implementation\" /></providers></log>");

                // Get the names of providers
                config = new ProviderConfiguration();
                config.LoadValuesFromConfigurationXml(defaultNode.DocumentElement);
            }

            // Read specific configuration information for this provider
            ProviderSettings providerSettings = (ProviderSettings) config.Providers[config.DefaultProvider];

            // In the cache?
            cacheKey = "Appleseed::Configuration::Log::" + config.DefaultProvider;
            if (cache[cacheKey] == null)
            {
                // The assembly should be in \bin or GAC, so we simply need
                // to get an instance of the type
                try
                {
                    cache.Insert(cacheKey, ProviderHelper.InstantiateProvider(providerSettings, typeof (LogProvider)));
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to load provider", e);
                }
            }

            return (LogProvider) cache[cacheKey];
        }

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        public abstract void Log(LogLevel level, object message);

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="t">The t.</param>
        public abstract void Log(LogLevel level, object message, Exception t);

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="sw">The sw.</param>
        public abstract void Log(LogLevel level, object message, StringWriter sw);

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="t">The t.</param>
        /// <param name="sw">The sw.</param>
        public abstract void Log(LogLevel level, object message, Exception t, StringWriter sw);
    }
}