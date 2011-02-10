// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Services.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   The services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.Core.Update
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Xml;

    using Appleseed.Framework.Data;
    using Appleseed.Framework.Helpers;
    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Settings.Cache;

    /// <summary>
    /// The services.
    /// </summary>
    public class Services : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// The scripts list.
        /// </summary>
        private UpdateEntry[] scriptsList;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets db version.
        ///   It does not rely on cached value and always gets the actual value.
        /// </summary>
        /// <value>The database version.</value>
        private static int DatabaseVersion
        {
            get
            {
                // Clear version cache so we are sure we update correctly
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[Database.dbKey] = null;
                HttpContext.Current.Application.UnLock();
                return Database.DatabaseVersion;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The run db update.
        /// </summary>
        /// <param name="connectionString">
        /// </param>
        /// <returns>
        /// The run db update.
        /// </returns>
        public bool RunDBUpdate(string connectionString)
        {
            CurrentCache.Insert(Portal.UniqueID + "_ConnectionString", connectionString);

            var databaseVersion = DatabaseVersion;

            var xmlDocument = new XmlDocument();
            var tempScriptsList = new ArrayList();

            if (databaseVersion < Portal.CodeVersion)
            {
                ErrorHandler.Publish(LogLevel.Debug, string.Format("db:{0} Code:{1}", databaseVersion, Portal.CodeVersion));

                // load the history file
                var docPath = HttpContext.Current.Server.MapPath(string.Format("{0}/Setup/Scripts/History.xml", Path.ApplicationRoot));
                xmlDocument.Load(docPath);

                // get a list of <Release> nodes
                if (xmlDocument.DocumentElement != null)
                {
                    var releases = xmlDocument.DocumentElement.SelectNodes("Release");

                    // iterate over the <Release> nodes
                    // (we can do this because XmlNodeList implements IEnumerable)
                    if (releases != null)
                    {
                        foreach (XmlNode release in releases)
                        {
                            var updateEntry = new UpdateEntry();

                            // get the header information
                            // we check for null to avoid exception if any of these nodes are not present
                            if (release.SelectSingleNode("ID") != null)
                            {
                                updateEntry.VersionNumber = Int32.Parse(release.SelectSingleNode("ID/text()").Value);
                            }

                            if (release.SelectSingleNode("Version") != null)
                            {
                                updateEntry.Version = release.SelectSingleNode("Version/text()").Value;
                            }

                            if (release.SelectSingleNode("Script") != null)
                            {
                                updateEntry.scriptNames.Add(release.SelectSingleNode("Script/text()").Value);
                            }

                            if (release.SelectSingleNode("Date") != null)
                            {
                                updateEntry.Date = DateTime.Parse(release.SelectSingleNode("Date/text()").Value);
                            }

                            // We should apply this patch
                            if (databaseVersion < updateEntry.VersionNumber)
                            {
                                // Appleseed.Framework.Helpers.LogHelper.Logger.Log(Appleseed.Framework.Site.Configuration.LogLevel.Debug, "Detected version to apply: " + myUpdate.Version);
                                updateEntry.Apply = true;

                                // get a list of <Installer> nodes
                                var installers = release.SelectNodes("Modules/Installer/text()");

                                // iterate over the <Installer> Nodes (in original document order)
                                // (we can do this because XmlNodeList implements IEnumerable)
                                if (installers != null)
                                {
                                    foreach (XmlNode installer in installers)
                                    {
                                        // and build an ArrayList of the scripts... 
                                        updateEntry.Modules.Add(installer.Value);

                                        // Appleseed.Framework.Helpers.LogHelper.Logger.Log(Appleseed.Framework.Site.Configuration.LogLevel.Debug, "Detected module to install: " + installer.Value);
                                    }
                                }

                                // get a <Script> node, if any
                                var sqlScripts = release.SelectNodes("Scripts/Script/text()");

                                // iterate over the <Installer> Nodes (in original document order)
                                // (we can do this because XmlNodeList implements IEnumerable)
                                if (sqlScripts != null)
                                {
                                    foreach (XmlNode sqlScript in sqlScripts)
                                    {
                                        // and build an ArrayList of the scripts... 
                                        updateEntry.scriptNames.Add(sqlScript.Value);

                                        // Appleseed.Framework.Helpers.LogHelper.Logger.Log(Appleseed.Framework.Site.Configuration.LogLevel.Debug, "Detected script to run: " + sqlScript.Value);
                                    }
                                }

                                tempScriptsList.Add(updateEntry);
                            }
                        }
                    }
                }

                // If we have some version to apply...
                if (tempScriptsList.Count <= 0)
                {
                    // No update is needed
                }
                else
                {
                    this.scriptsList = (UpdateEntry[])tempScriptsList.ToArray(typeof(UpdateEntry));

                    // by Manu. Versions are sorted by version number
                    Array.Sort(this.scriptsList);

                    // Create a flat version for binding
                    var currentVersion = 0;
                    var databindList = new ArrayList();
                    foreach (var updateEntry in this.scriptsList.Where(updateEntry => updateEntry.Apply))
                    {
                        if (currentVersion != updateEntry.VersionNumber)
                        {
                            databindList.Add("Version: " + updateEntry.VersionNumber);
                            currentVersion = updateEntry.VersionNumber;
                        }

                        foreach (string scriptName in
                            updateEntry.scriptNames.Cast<string>().Where(scriptName => scriptName.Length > 0))
                        {
                            databindList.Add("-- Script: " + scriptName);
                        }

                        foreach (string moduleInstaller in
                            updateEntry.Modules.Cast<string>().Where(moduleInstaller => moduleInstaller.Length > 0))
                        {
                            databindList.Add("-- Module: " + moduleInstaller);
                        }
                    }

                    var errors = new List<object>();

                    // var messages = new List<object>();
                    foreach (var updateEntry in this.scriptsList)
                    {
                        if (updateEntry.Apply && DatabaseVersion < updateEntry.VersionNumber &&
                            DatabaseVersion < Portal.CodeVersion)
                        {
                            // Version check (a script may update more than one version at once)
                            foreach (string scriptName in updateEntry.scriptNames)
                            {
                                if (scriptName.Length <= 0)
                                {
                                    continue;
                                }

                                // It may be a module update only
                                var currentScriptName =
                                    HttpContext.Current.Server.MapPath(
                                        System.IO.Path.Combine(Path.ApplicationRoot + "/Setup/Scripts/", scriptName));
                                ErrorHandler.Publish(
                                    LogLevel.Info,
                                    string.Format(
                                        "CODE: {0} - DB: {1} - CURR: {2} - Applying: {3}",
                                        Portal.CodeVersion,
                                        DatabaseVersion,
                                        updateEntry.VersionNumber,
                                        currentScriptName));
                                var myerrors = DBHelper.ExecuteScript(currentScriptName, true);
                                errors.AddRange(myerrors); // Display errors if any

                                if (myerrors.Count <= 0)
                                {
                                    continue;
                                }

                                errors.Insert(0, string.Format("<p>{0}</p>", scriptName));
                                ErrorHandler.Publish(
                                    LogLevel.Error,
                                    string.Format(
                                        "Version {0} completed with errors.  - {1}", updateEntry.Version, scriptName));
                                break;
                            }

                            // Installing modules
                            foreach (var currentModuleInstaller in from string moduleInstaller in updateEntry.Modules
                                                                   select
                                                                       HttpContext.Current.Server.MapPath(
                                                                           System.IO.Path.Combine(
                                                                               Path.ApplicationRoot + "/",
                                                                               moduleInstaller)))
                            {
                                try
                                {
                                    ModuleInstall.InstallGroup(currentModuleInstaller, true);
                                }
                                catch (Exception ex)
                                {
                                    ErrorHandler.Publish(
                                        LogLevel.Fatal,
                                        string.Format(
                                            "Exception in UpdateDatabaseCommand installing module: {0}",
                                            currentModuleInstaller),
                                        ex);
                                    if (ex.InnerException != null)
                                    {
                                        // Display more meaningful error message if InnerException is defined
                                        ErrorHandler.Publish(
                                            LogLevel.Warn,
                                            string.Format(
                                                "Exception in UpdateDatabaseCommand installing module: {0}",
                                                currentModuleInstaller),
                                            ex.InnerException);
                                        errors.Add(
                                            string.Format(
                                                "Exception in UpdateDatabaseCommand installing module: {0}<br />{1}<br />{2}",
                                                currentModuleInstaller,
                                                ex.InnerException.Message,
                                                ex.InnerException.StackTrace));
                                    }
                                    else
                                    {
                                        ErrorHandler.Publish(
                                            LogLevel.Warn,
                                            string.Format(
                                                "Exception in UpdateDatabaseCommand installing module: {0}",
                                                currentModuleInstaller),
                                            ex);
                                        errors.Add(ex.Message);
                                    }
                                }
                            }

                            if (Equals(errors.Count, 0))
                            {
                                // Update db with version
                                string versionUpdater =
                                    string.Format(
                                        "INSERT INTO [rb_Versions] ([Release],[Version],[ReleaseDate]) VALUES('{0}','{1}', CONVERT(datetime, '{2}/{3}/{4}', 101))",
                                        updateEntry.VersionNumber,
                                        updateEntry.Version,
                                        updateEntry.Date.Month,
                                        updateEntry.Date.Day,
                                        updateEntry.Date.Year);
                                DBHelper.ExeSQL(versionUpdater);
                                ErrorHandler.Publish(
                                    LogLevel.Info,
                                    string.Format("Version number: {0} applied successfully.", updateEntry.Version));

                                // Mark this update as done
                                ErrorHandler.Publish(
                                    LogLevel.Info,
                                    string.Format("Successfully applied version: {0}", updateEntry.Version));
                            }
                        }
                        else
                        {
                            ErrorHandler.Publish(
                                LogLevel.Info,
                                string.Format(
                                    "CODE: {0} - DB: {1} - CURR: {2} - Skipping: {3}",
                                    Portal.CodeVersion,
                                    DatabaseVersion,
                                    updateEntry.VersionNumber,
                                    updateEntry.Version));
                        }
                    }
                }
            }

            return true;
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            // TODO Implement the pattern properly.
        }

        #endregion

        #endregion
    }
}