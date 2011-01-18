using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Web.UI.WebControls;
using Path=Appleseed.Framework.Settings.Path;

namespace Appleseed.Framework.Helpers
{
    /// <summary>
    /// ModuleInstall incapsulates all the logic for install, 
    /// uninstall modules on portal
    /// </summary>
    [History("jminond", "2006/02/22", "corrected case where install group is null exception")]
    public class ModuleInstall
    {
        /// <summary>
        /// Installs the group.
        /// </summary>
        /// <param name="groupFileName">Name of the group file.</param>
        /// <param name="install">if set to <c>true</c> [install].</param>
        public static void InstallGroup(string groupFileName, bool install)
        {
            DataTable modules = GetInstallGroup(groupFileName);

            // In case Modules are null
            if (modules != null && (modules.Rows.Count > 0))
            {
                foreach (DataRow r in modules.Rows)
                {
                    string friendlyName = r["FriendlyName"].ToString();
                    string desktopSource = r["DesktopSource"].ToString();
                    string mobileSource = r["MobileSource"].ToString();

                    Install(friendlyName, desktopSource, mobileSource, install);
                }
            }
            else
            {
                Exception ex = new Exception("Tried to install 0 modules in groupFileName:" + groupFileName);
                ErrorHandler.Publish(LogLevel.Warn, ex);
            }
        }

        /// <summary>
        /// Gets the install group.
        /// </summary>
        /// <param name="groupFileName">Name of the group file.</param>
        /// <returns></returns>
        private static DataTable GetInstallGroup(string groupFileName)
        {
            //Load the XML as dataset
            using (DataSet ds = new DataSet())
            {
                string installer = groupFileName;

                try
                {
                    ds.ReadXml(installer);
                }
                catch (Exception ex)
                {
                    ErrorHandler.Publish(LogLevel.Error, "Exception installing module: " + installer, ex);
                    return null;
                }

                return ds.Tables[0];
            }
        }

        /// <summary>
        /// Uninstalls the group.
        /// </summary>
        /// <param name="groupFileName">Name of the group file.</param>
        public static void UninstallGroup(string groupFileName)
        {
            DataTable modules = GetInstallGroup(groupFileName);

            foreach (DataRow r in modules.Rows)
            {
                // string friendlyName = r["FriendlyName"].ToString();
                string desktopSource = r["DesktopSource"].ToString();
                string mobileSource = r["MobileSource"].ToString();

                Uninstall(desktopSource, mobileSource);
            }
        }

        /// <summary>
        /// Installs the specified friendly name.
        /// </summary>
        /// <param name="friendlyName">Name of the friendly.</param>
        /// <param name="desktopSource">The desktop source.</param>
        /// <param name="mobileSource">The mobile source.</param>
        public static void Install(string friendlyName, string desktopSource, string mobileSource)
        {
            Install(friendlyName, desktopSource, mobileSource, true);
        }

        /// <summary>
        /// Installs module
        /// </summary>
        /// <param name="friendlyName">Name of the friendly.</param>
        /// <param name="desktopSource">The desktop source.</param>
        /// <param name="mobileSource">The mobile source.</param>
        /// <param name="install">if set to <c>true</c> [install].</param>
        public static void Install(string friendlyName, string desktopSource, string mobileSource, bool install)
        {
            ErrorHandler.Publish(LogLevel.Info,
                                 "Installing DesktopModule '" + friendlyName + "' from '" + desktopSource + "'");
            if (mobileSource != null && mobileSource.Length > 0)
                ErrorHandler.Publish(LogLevel.Info,
                                     "Installing MobileModule '" + friendlyName + "' from '" + mobileSource + "'");

            string controlFullPath = Path.ApplicationRoot + "/" + desktopSource;

            // if not MVC -> Instantiate the module
            if (controlFullPath.LastIndexOf('.') >= 0) {
                Page page = new Page();

                Control myControl = page.LoadControl(controlFullPath);
                if (!(myControl is PortalModuleControl))
                    throw new Exception("Module '" + myControl.GetType().FullName + "' is not a PortalModule Control");

                PortalModuleControl portalModule = (PortalModuleControl)myControl;

                // Check mobile module
                if (mobileSource != null && mobileSource.Length != 0 && mobileSource.ToLower().EndsWith(".ascx")) {
                    //TODO: Check mobile module
                    //TODO: MobilePortalModuleControl mobileModule = (MobilePortalModuleControl) page.LoadControl(Appleseed.Framework.Settings.Path.ApplicationRoot + "/" + mobileSource);
                    if (!File.Exists(HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/" + mobileSource)))
                        throw new FileNotFoundException("Mobile Control not found");
                }

                // Get Module ID
                Guid defID = portalModule.GuidID;

                //Get Assembly name
                string assemblyName = portalModule.GetType().BaseType.Assembly.CodeBase;
                assemblyName = assemblyName.Substring(assemblyName.LastIndexOf('/') + 1); //Get name only

                // Get Module Class name
                string className = portalModule.GetType().BaseType.FullName;

                // Now we add the definition to module list 
                ModulesDB modules = new ModulesDB();

                if (install) {
                    //Install as new module

                    //Call Install
                    try {
                        ErrorHandler.Publish(LogLevel.Debug, "Installing '" + friendlyName + "' as new module.");
                        portalModule.Install(null);
                    } catch (Exception ex) {
                        //Error occurred
                        portalModule.Rollback(null);
                        //Rethrow exception
                        throw new Exception("Exception occurred installing '" + portalModule.GuidID.ToString() + "'!", ex);
                    }

                    try {
                        // Add a new module definition to the database
                        modules.AddGeneralModuleDefinitions(defID, friendlyName, desktopSource, mobileSource, assemblyName,
                                                            className, portalModule.AdminModule, portalModule.Searchable);
                    } catch (Exception ex) {
                        //Rethrow exception
                        throw new Exception(
                            "AddGeneralModuleDefinitions Exception '" + portalModule.GuidID.ToString() + "'!", ex);
                    }

                    // All is fine: we can call Commit
                    portalModule.Commit(null);
                } else {
                    // Update the general module definition
                    try {
                        ErrorHandler.Publish(LogLevel.Debug, "Updating '" + friendlyName + "' as new module.");
                        modules.UpdateGeneralModuleDefinitions(defID, friendlyName, desktopSource, mobileSource,
                                                               assemblyName, className, portalModule.AdminModule,
                                                               portalModule.Searchable);
                    } catch (Exception ex) {
                        //Rethrow exception
                        throw new Exception(
                            "UpdateGeneralModuleDefinitions Exception '" + portalModule.GuidID.ToString() + "'!", ex);
                    }
                }

                // Update the module definition - install for portal 0
                modules.UpdateModuleDefinitions(defID, 0, true);
            }
        }

        /// <summary>
        /// Uninstalls the specified desktop source.
        /// </summary>
        /// <param name="desktopSource">The desktop source.</param>
        /// <param name="mobileSource">The mobile source.</param>
        public static void Uninstall(string desktopSource, string mobileSource)
        {
            Page page = new Page();

            // Istantiate the module
            PortalModuleControl portalModule =
                (PortalModuleControl) page.LoadControl(Path.ApplicationRoot + "/" + desktopSource);

            //Call Uninstall
            try
            {
                portalModule.Uninstall(null);
            }
            catch (Exception ex)
            {
                //Rethrow exception
                throw new Exception("Exception during uninstall!", ex);
            }

            // Delete definition
            new ModulesDB().DeleteModuleDefinition(portalModule.GuidID);
        }
    }
}