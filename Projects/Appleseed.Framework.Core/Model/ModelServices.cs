// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelServices.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   The model services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.Core.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.UI;

    using Appleseed.Framework.Security;
    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Site.Configuration;
    using Appleseed.Framework.Site.Data;
    using Appleseed.Framework.Web.UI.WebControls;

    using Path = Appleseed.Framework.Settings.Path;

    /// <summary>
    /// The model services.
    /// </summary>
    public class ModelServices
    {
        // Fields
        #region Constants and Fields

        /// <summary>
        /// The content pane idx.
        /// </summary>
        public const string ContentPaneIdx = "contentpane";

        #endregion

        // Methods
        #region Public Methods

        /// <summary>
        /// Adds the MVC action module.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="actionPath">The action path.</param>
        /// <returns></returns>
        public static Guid AddMVCActionModule(string name, string actionPath)
        {
            var sdb = new ModulesDB();
            var generalModDefId = Guid.NewGuid();
            sdb.AddGeneralModuleDefinitions(
                generalModDefId, name, actionPath, string.Empty, string.Empty, string.Empty, false, false);
            return generalModDefId;
        }

        /// <summary>
        /// Gets the current page modules.
        /// </summary>
        /// <returns>A dictionary.</returns>
        public static Dictionary<string, List<Control>> GetCurrentPageModules()
        {
            var dictionary = new Dictionary<string, List<Control>>();
            var settings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            if (settings.ActivePage.Modules.Count > 0)
            {
                var page = new Page();
                foreach (ModuleSettings settings2 in settings.ActivePage.Modules)
                {
                    if (!settings2.Cacheable)
                    {
                        settings2.CacheTime = -1;
                    }

                    if (PortalSecurity.IsInRoles(settings2.AuthorizedViewRoles))
                    {
                        List<Control> list;
                        Exception exception;
                        var str = settings2.PaneName.ToLower();
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (!dictionary.ContainsKey(str))
                            {
                                dictionary.Add(str, new List<Control>());
                            }

                            list = dictionary[str];
                        }
                        else
                        {
                            if (!dictionary.ContainsKey("contentpane"))
                            {
                                dictionary.Add("contentpane", new List<Control>());
                            }

                            list = dictionary["contentpane"];
                        }

                        if (!settings2.Admin && (settings2.CacheTime == 0))
                        {
                            var moduleOverrideCache = Config.ModuleOverrideCache;
                            if (moduleOverrideCache > 0)
                            {
                                settings2.CacheTime = moduleOverrideCache;
                            }
                        }

                        if ((((settings2.CacheTime <= 0) || PortalSecurity.HasEditPermissions(settings2.ModuleID)) ||
                             (PortalSecurity.HasPropertiesPermissions(settings2.ModuleID) ||
                              PortalSecurity.HasAddPermissions(settings2.ModuleID))) ||
                            PortalSecurity.HasDeletePermissions(settings2.ModuleID))
                        {
                            try
                            {
                                PortalModuleControl control;
                                var virtualPath = Path.ApplicationRoot + "/" + settings2.DesktopSrc;
                                if (virtualPath.LastIndexOf('.') >= 0)
                                {
                                    control = (PortalModuleControl)page.LoadControl(virtualPath);
                                }
                                else
                                {
                                    var strArray = virtualPath.Split(
                                        new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                                    var areaName = (strArray[1].ToLower() == "views") ? string.Empty : strArray[1];
                                    var controllerName = strArray[strArray.Length - 2];
                                    var actionName = strArray[strArray.Length - 1];
                                    
                                    // var ns = strArray[2];
                                    control =
                                        (PortalModuleControl)
                                        page.LoadControl("/DesktopModules/CoreModules/MVC/MVCModule.ascx");

                                    ((MVCModuleControl)control).ControllerName = controllerName;
                                    ((MVCModuleControl)control).ActionName = actionName;
                                    ((MVCModuleControl)control).AreaName = areaName;
                                    ((MVCModuleControl)control).ModID = settings2.ModuleID;

                                    ((MVCModuleControl)control).Initialize();
                                }

                                control.PortalID = settings.PortalID;
                                control.ModuleConfiguration = settings2;
                                if ((control.Cultures == string.Empty) ||
                                    ((control.Cultures + ";").IndexOf(settings.PortalContentLanguage.Name + ";") >= 0))
                                {
                                    list.Add(control);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                ErrorHandler.Publish(
                                    LogLevel.Error, 
                                    string.Format("DesktopPanes: Unable to load control '{0}'!", settings2.DesktopSrc), 
                                    exception);
                                if (PortalSecurity.IsInRoles("Admins"))
                                {
                                    list.Add(
                                        new LiteralControl(
                                            string.Format("<br><span class=NormalRed>Unable to load control '{0}'! (Full Error Logged)<br />Error Message: {1}", settings2.DesktopSrc, exception.Message)));
                                }
                                else
                                {
                                    list.Add(
                                        new LiteralControl(
                                            string.Format("<br><span class=NormalRed>Unable to load control '{0}'!", settings2.DesktopSrc)));
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                using (var control2 = new CachedPortalModuleControl())
                                {
                                    control2.PortalID = settings.PortalID;
                                    control2.ModuleConfiguration = settings2;
                                    list.Add(control2);
                                }
                            }
                            catch (Exception exception2)
                            {
                                exception = exception2;
                                ErrorHandler.Publish(
                                    LogLevel.Error, 
                                    string.Format("DesktopPanes: Unable to load cached control '{0}'!", settings2.DesktopSrc), 
                                    exception);
                                if (PortalSecurity.IsInRoles("Admins"))
                                {
                                    list.Add(
                                        new LiteralControl(
                                            string.Format("<br><span class=NormalRed>Unable to load cached control '{0}'! (Full Error Logged)<br />Error Message: {1}", settings2.DesktopSrc, exception.Message)));
                                }
                                else
                                {
                                    list.Add(
                                        new LiteralControl(
                                            string.Format("<br><span class=NormalRed>Unable to load cached control '{0}'!", settings2.DesktopSrc)));
                                }
                            }
                        }
                    }
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Gets the MVC action modules.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetMVCActionModules()
        {
            var dictionary = new Dictionary<string, string>();
            var info = new DirectoryInfo(Path.ApplicationPhysicalPath + "/Areas/");
            if (info.Exists)
            {
                var files = info.GetFiles("*.ascx", SearchOption.AllDirectories);
                foreach (var info2 in files)
                {
                    var key = string.Format(@"[MVC Action] {0}\{1}", info2.DirectoryName.Substring(info2.DirectoryName.LastIndexOf("MVC") + 4), info2.Name.Split(new[] { '.' })[0]);
                    dictionary.Add(key, info2.FullName);
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Registers the portable area module.
        /// </summary>
        /// <param name="areaName">Name of the area.</param>
        /// <param name="assemblyFullName">Full name of the assembly.</param>
        /// <param name="module">The module.</param>
        /// <returns></returns>
        public static Guid RegisterPortableAreaModule(string areaName, string assemblyFullName, string module)
        {
            Guid mId;
            var sdb = new ModulesDB();
            var friendlyName = String.Format("{0} - {1}", areaName, module);
            try
            {
                mId = sdb.GetGeneralModuleDefinitionByName(friendlyName);
            }
            catch (ArgumentException)
            {
                // No existe el módulo, entonces lo creo
                mId = AddPortableArea(areaName, assemblyFullName, module, friendlyName, sdb);
            }

            return mId;
        }

        /// <summary>
        /// The reorder.
        /// </summary>
        /// <param name="modulesByPane">
        /// The modules by pane.
        /// </param>
        public static void Reorder(Dictionary<string, ArrayList> modulesByPane)
        {
            // var settings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            // var pageId = settings.ActivePage.PageID;
            var sdb = new ModulesDB();
            foreach (var str in modulesByPane.Keys)
            {
                var list = modulesByPane[str];
                var moduleOrder = 0;
                foreach (int num3 in list)
                {
                    sdb.UpdateModuleOrder(num3, moduleOrder, str);
                    moduleOrder++;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the portable area.
        /// </summary>
        /// <param name="areaName">Name of the area.</param>
        /// <param name="assemblyFullName">Full name of the assembly.</param>
        /// <param name="module">The module.</param>
        /// <param name="friendlyName">Name of the friendly.</param>
        /// <param name="sdb">The SDB.</param>
        /// <returns></returns>
        private static Guid AddPortableArea(
            string areaName, string assemblyFullName, string module, string friendlyName, ModulesDB sdb)
        {
            var mId = Guid.NewGuid();
            var action = string.Format("Areas/{0}/Views/Home/Module", areaName);
            sdb.AddGeneralModuleDefinitions(
                mId, friendlyName, action, string.Empty, assemblyFullName, areaName, false, false);

            return mId;
        }

        #endregion
    }
}