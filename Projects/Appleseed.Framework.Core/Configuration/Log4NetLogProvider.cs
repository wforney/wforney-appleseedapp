// Created by Manu
// Import log4net classes.
using System;
using System.Collections.Specialized;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Appleseed.Framework.Logging
{
    /// <summary>
    /// Log4Net provider implementation
    /// </summary>
    public class Log4NetLogProvider : LogProvider
    {
        // Define a static log4net logger variable so that it references the
        // Logger instance named "LogHelper".
        private static readonly ILog log = LogManager.GetLogger("Appleseed");
        private static MemoryAppender ma;

        /// <summary>
        /// Initializes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="configValue">The config value.</param>
        public override void Initialize(string name, NameValueCollection configValue)
        {
            // Initialise the logging when the application loads
            // DOMConfigurator.Configure(); // Jes1111 - deprecated in log4net 1.2.9
            XmlConfigurator.Configure();

            GlobalContext.Properties["CodeVersion"] = new LogCodeVersionProperty();
            GlobalContext.Properties["User"] = new LogUserNameProperty();
            GlobalContext.Properties["RewrittenUrl"] = new LogRewrittenUrlProperty();
            GlobalContext.Properties["UserAgent"] = new LogUserAgentProperty();
            GlobalContext.Properties["UserIP"] = new LogUserIpProperty();
            GlobalContext.Properties["UserLanguages"] = new LogUserLanguagesProperty();

            Hierarchy h = (Hierarchy) LogManager.GetRepository();
            ma = (MemoryAppender) ((Logger) h.GetLogger("Appleseed")).GetAppender("SmartError");
        }

        /// <summary>
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public override void Log(LogLevel level, object message)
        {
            //It is VERY Important that the log are 
            //in the very same order that appear on log4net
            if (log.IsDebugEnabled)
            {
                if (level == LogLevel.Debug)
                {
                    log.Debug(message);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsInfoEnabled)
            {
                if (level == LogLevel.Info)
                {
                    log.Info(message);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsWarnEnabled)
            {
                if (level == LogLevel.Warn)
                {
                    log.Warn(message);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsErrorEnabled)
            {
                if (level == LogLevel.Error)
                {
                    log.Error(message);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsFatalEnabled)
            {
                if (level == LogLevel.Fatal)
                    log.Fatal(message);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="t"></param>
        public override void Log(LogLevel level, object message, Exception t)
        {
            //It is VERY Important that the log are 
            //in the very same order that appear on log4net
            if (log.IsDebugEnabled)
            {
                if (level == LogLevel.Debug)
                {
                    log.Debug(message, t);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsInfoEnabled)
            {
                if (level == LogLevel.Info)
                {
                    log.Info(message, t);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsWarnEnabled)
            {
                if (level == LogLevel.Warn)
                {
                    log.Warn(message, t);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsErrorEnabled)
            {
                if (level == LogLevel.Error)
                {
                    log.Error(message, t);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsFatalEnabled)
            {
                if (level == LogLevel.Fatal)
                    log.Fatal(message, t);
            }
        }

        /// <summary>
        /// Fills the text writer.
        /// </summary>
        /// <param name="sw">The sw.</param>
        private void FillTextWriter(TextWriter sw)
        {
            if (ma != null)
            {
                LoggingEvent[] events = ma.GetEvents();
                PatternLayout iLayout = (PatternLayout) ma.Layout;
                iLayout.Format(sw, events[events.GetUpperBound(0)]);
                ma.Clear();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="sw"></param>
        public override void Log(LogLevel level, object message, StringWriter sw)
        {
            //It is VERY Important that the log are 
            //in the very same order that appear on log4net
            if (log.IsDebugEnabled)
            {
                if (level == LogLevel.Debug)
                {
                    log.Debug(message);
                    FillTextWriter(sw);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsInfoEnabled)
            {
                if (level == LogLevel.Info)
                {
                    log.Info(message);
                    FillTextWriter(sw);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsWarnEnabled)
            {
                if (level == LogLevel.Warn)
                {
                    log.Warn(message);
                    FillTextWriter(sw);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsErrorEnabled)
            {
                if (level == LogLevel.Error)
                {
                    log.Error(message);
                    FillTextWriter(sw);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsFatalEnabled)
            {
                if (level == LogLevel.Fatal)
                {
                    log.Fatal(message);
                    FillTextWriter(sw);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="t"></param>
        /// <param name="sw"></param>
        public override void Log(LogLevel level, object message, Exception t, StringWriter sw)
        {
            //It is VERY Important that the log are 
            //in the very same order that appear on log4net
            if (log.IsDebugEnabled)
            {
                if (level == LogLevel.Debug)
                {
                    log.Debug(message, t);
                    FillTextWriter(sw);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsInfoEnabled)
            {
                if (level == LogLevel.Info)
                {
                    log.Info(message, t);
                    FillTextWriter(sw);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsWarnEnabled)
            {
                if (level == LogLevel.Warn)
                {
                    log.Warn(message, t);
                    FillTextWriter(sw);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsErrorEnabled)
            {
                if (level == LogLevel.Error)
                {
                    log.Error(message, t);
                    FillTextWriter(sw);
                    //no need to go on
                    return;
                }
            }
            else
            {
                //No need to test others: if the lower level is false other will be false
                return;
            }

            if (log.IsFatalEnabled)
            {
                if (level == LogLevel.Fatal)
                {
                    log.Fatal(message, t);
                    FillTextWriter(sw);
                }
            }
        }
    }
}