using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using Appleseed.Framework.Settings;
using System.Xml;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Settings.Cache;

namespace Appleseed.Framework.Core.Update
{
    public class Services: IDisposable
    {
        private UpdateEntry[] scriptsList;

        /// <summary>
        /// This property returns db version.
        /// It does not rely on cached value and always gets the actual value.
        /// </summary>
        /// <value>The database version.</value>
        private int DatabaseVersion
        {
            get
            {
                //Clear version cache so we are sure we update correctly
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[Database.dbKey] = null;
                HttpContext.Current.Application.UnLock();
                return Database.DatabaseVersion;
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public bool RunDBUpdate(string connectionString)
        {
            CurrentCache.Insert(Portal.UniqueID + "_ConnectionString", connectionString);

            int dbVersion = DatabaseVersion;

            XmlDocument myDoc = new XmlDocument();
            ArrayList tempScriptsList = new ArrayList();

            if (dbVersion < Portal.CodeVersion)
            {
                ErrorHandler.Publish(LogLevel.Debug, "db:" + dbVersion + " Code:" + Portal.CodeVersion);

                // load the history file
                string myDocPath = HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/Setup/Scripts/History.xml");
                myDoc.Load(myDocPath);

                // get a list of <Release> nodes
                XmlNodeList releases = myDoc.DocumentElement.SelectNodes("Release");

                // iterate over the <Release> nodes
                // (we can do this because XmlNodeList implements IEnumerable)
                foreach (XmlNode release in releases)
                {
                    UpdateEntry myUpdate = new UpdateEntry();

                    // get the header information
                    // we check for null to avoid exception if any of these nodes are not present
                    if (release.SelectSingleNode("ID") != null)
                    {
                        myUpdate.VersionNumber = Int32.Parse(release.SelectSingleNode("ID/text()").Value);
                    }

                    if (release.SelectSingleNode("Version") != null)
                    {
                        myUpdate.Version = release.SelectSingleNode("Version/text()").Value;
                    }

                    if (release.SelectSingleNode("Script") != null)
                    {
                        myUpdate.scriptNames.Add(release.SelectSingleNode("Script/text()").Value);
                    }

                    if (release.SelectSingleNode("Date") != null)
                    {
                        myUpdate.Date = DateTime.Parse(release.SelectSingleNode("Date/text()").Value);
                    }

                    //We should apply this patch
                    if (dbVersion < myUpdate.VersionNumber)
                    {
                        //Appleseed.Framework.Helpers.LogHelper.Logger.Log(Appleseed.Framework.Site.Configuration.LogLevel.Debug, "Detected version to apply: " + myUpdate.Version);

                        myUpdate.Apply = true;

                        // get a list of <Installer> nodes
                        XmlNodeList installers = release.SelectNodes("Modules/Installer/text()");

                        // iterate over the <Installer> Nodes (in original document order)
                        // (we can do this because XmlNodeList implements IEnumerable)
                        foreach (XmlNode installer in installers)
                        {
                            //and build an ArrayList of the scripts... 
                            myUpdate.Modules.Add(installer.Value);
                            //Appleseed.Framework.Helpers.LogHelper.Logger.Log(Appleseed.Framework.Site.Configuration.LogLevel.Debug, "Detected module to install: " + installer.Value);
                        }

                        // get a <Script> node, if any
                        XmlNodeList sqlScripts = release.SelectNodes("Scripts/Script/text()");

                        // iterate over the <Installer> Nodes (in original document order)
                        // (we can do this because XmlNodeList implements IEnumerable)
                        foreach (XmlNode sqlScript in sqlScripts)
                        {
                            //and build an ArrayList of the scripts... 
                            myUpdate.scriptNames.Add(sqlScript.Value);
                            //Appleseed.Framework.Helpers.LogHelper.Logger.Log(Appleseed.Framework.Site.Configuration.LogLevel.Debug, "Detected script to run: " + sqlScript.Value);
                        }

                        tempScriptsList.Add(myUpdate);
                    }
                }

                //If we have some version to apply...
                if (tempScriptsList.Count > 0)
                {
                    scriptsList = (UpdateEntry[])tempScriptsList.ToArray(typeof(UpdateEntry));

                    //by Manu. Versions are sorted by version number
                    Array.Sort(scriptsList);

                    //Create a flat version for binding
                    int currentVersion = 0;
                    ArrayList databindList = new ArrayList();
                    foreach (UpdateEntry myUpdate in scriptsList)
                    {
                        if (myUpdate.Apply)
                        {
                            if (currentVersion != myUpdate.VersionNumber)
                            {
                                databindList.Add("Version: " + myUpdate.VersionNumber);
                                currentVersion = myUpdate.VersionNumber;
                            }

                            foreach (string scriptName in myUpdate.scriptNames)
                            {
                                if (scriptName.Length > 0)
                                {
                                    databindList.Add("-- Script: " + scriptName);
                                }
                            }

                            foreach (string moduleInstaller in myUpdate.Modules)
                            {
                                if (moduleInstaller.Length > 0)
                                    databindList.Add("-- Module: " + moduleInstaller);
                            }
                        }
                    }

                    ArrayList errors = new ArrayList();
                    ArrayList messages = new ArrayList();

                    foreach (UpdateEntry myUpdate in scriptsList)
                    {
                        //Version check (a script may update more than one version at once)
                        if (myUpdate.Apply && DatabaseVersion < myUpdate.VersionNumber && DatabaseVersion < Portal.CodeVersion)
                        {

                            foreach (string scriptName in myUpdate.scriptNames)
                            {
                                //It may be a module update only
                                if (scriptName.Length > 0)
                                {
                                    string currentScriptName =
                                    HttpContext.Current.Server.MapPath(System.IO.Path.Combine(Path.ApplicationRoot + "/Setup/Scripts/", scriptName));
                                    ErrorHandler.Publish(LogLevel.Info,
                                                          "CODE: " + Portal.CodeVersion + " - DB: " + DatabaseVersion + " - CURR: " +
                                                          myUpdate.VersionNumber + " - Applying: " + currentScriptName);
                                    ArrayList myErrors = Appleseed.Framework.Data.DBHelper.ExecuteScript(currentScriptName, true);
                                    errors.AddRange(myErrors);                    //Display errors if any

                                    if (myErrors.Count > 0)
                                    {
                                        errors.Insert(0, "<P>" + scriptName + "</P>");
                                        ErrorHandler.Publish(LogLevel.Error,
                                                              "Version " + myUpdate.Version + " completed with errors.  - " +
                                                              scriptName);
                                        break;
                                    }
                                }
                            }

                            //Installing modules
                            foreach (string moduleInstaller in myUpdate.Modules)
                            {
                                string currentModuleInstaller =
                                     HttpContext.Current.Server.MapPath(System.IO.Path.Combine(Path.ApplicationRoot + "/", moduleInstaller));

                                try
                                {
                                    ModuleInstall.InstallGroup(currentModuleInstaller, true);
                                }
                                catch (Exception ex)
                                {
                                    ErrorHandler.Publish(LogLevel.Fatal,
                                                         "Exception in UpdateDatabaseCommand installing module: " +
                                                         currentModuleInstaller, ex);
                                    if (ex.InnerException != null)
                                    {
                                        // Display more meaningful error message if InnerException is defined
                                        ErrorHandler.Publish(LogLevel.Warn,
                                                             "Exception in UpdateDatabaseCommand installing module: " +
                                                             currentModuleInstaller, ex.InnerException);
                                        errors.Add("Exception in UpdateDatabaseCommand installing module: " +
                                                   currentModuleInstaller + "<br/>" + ex.InnerException.Message + "<br/>" +
                                                   ex.InnerException.StackTrace);
                                    }
                                    else
                                    {
                                        ErrorHandler.Publish(LogLevel.Warn,
                                                             "Exception in UpdateDatabaseCommand installing module: " +
                                                             currentModuleInstaller, ex);
                                        errors.Add(ex.Message);
                                    }
                                }
                            }

                            if (Equals(errors.Count, 0))
                            {
                                //Update db with version
                                string versionUpdater;
                                versionUpdater = "INSERT INTO [rb_Versions] ([Release],[Version],[ReleaseDate]) VALUES('" +
                                                 myUpdate.VersionNumber + "','" + myUpdate.Version + "', CONVERT(datetime, '" +
                                                 myUpdate.Date.Month + "/" + myUpdate.Date.Day + "/" + myUpdate.Date.Year + "', 101))";
                                Appleseed.Framework.Data.DBHelper.ExeSQL(versionUpdater);
                                ErrorHandler.Publish(LogLevel.Info,
                                                     "Version number: " + myUpdate.Version + " applied successfully.");

                                //Mark this update as done
                                ErrorHandler.Publish(LogLevel.Info, "Sucessfully applied version: " + myUpdate.Version);
                            }
                        }
                        else
                        {

                            ErrorHandler.Publish(LogLevel.Info,
                                                 "CODE: " + Portal.CodeVersion + " - DB: " + DatabaseVersion + " - CURR: " +
                                                 myUpdate.VersionNumber + " - Skipping: " + myUpdate.Version);
                        }
                    }


                }
                else
                {
                    //No update is needed
                }
            }

            return true;
        }

        public void Dispose()
        {
         //TODO Nothing
        }
    }
}