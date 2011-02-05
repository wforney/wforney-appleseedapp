using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Appleseed.Framework.Web.UI.WebControls;
using Appleseed.Framework.Site.Configuration;
using System.Web;
using Appleseed.Framework.Security;
using System.Collections;
using Appleseed.Framework.Settings;
using System.Web.UI.WebControls;
using Appleseed.Framework.Site.Data;
using io = System.IO;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;



namespace Appleseed.Framework.Core.Model
{
    public class ModelServices
    {
        // Fields
        public const string ContentPaneIdx = "contentpane";

        // Methods
        public static Guid AddMVCActionModule(string name, string actionPath)
        {
            ModulesDB sdb = new ModulesDB();
            Guid generalModDefID = Guid.NewGuid();
            sdb.AddGeneralModuleDefinitions(generalModDefID, name, actionPath, string.Empty, string.Empty, string.Empty, false, false);
            return generalModDefID;
        }

       /// <summary>
        /// Si no existe, registra un portablearea como módulo.
       /// </summary>
       /// <param name="areaName"></param>
       /// <param name="assemblyFullName"></param>
       /// <param name="module"></param>
       /// <returns></returns>
        public static Guid RegisterPortableAreaModule(string areaName, string assemblyFullName, string module)
        {
            Guid mId;
            ModulesDB sdb = new ModulesDB();
            string friendlyName = String.Format("{0} - {1}", areaName, module);
            try
            {
                mId = sdb.GetGeneralModuleDefinitionByName(friendlyName);
            }
            catch (ArgumentException)
            {
                //No existe el módulo, entonces lo creo
                mId = AddPortableArea( areaName,  assemblyFullName,  module, friendlyName, sdb);
            }

            return mId;
        }

       /// <summary>
        /// Agrega el portablearea como modulo general
       /// </summary>
       /// <param name="areaName"></param>
       /// <param name="assemblyFullName"></param>
       /// <param name="module"></param>
       /// <param name="friendlyName"></param>
       /// <param name="sdb"></param>
       /// <returns></returns>
        private static Guid AddPortableArea(string areaName, string assemblyFullName, string module, string friendlyName, ModulesDB sdb)
        {
            Guid mId = Guid.NewGuid();
            string action = string.Format("Areas/{0}/Views/Home/Module", areaName);
            sdb.AddGeneralModuleDefinitions(mId, friendlyName, action, string.Empty, assemblyFullName, areaName, false, false);

            return mId;
        }

        public static Dictionary<string, ArrayList> GetCurrentPageModules()
        {
            Dictionary<string, ArrayList> dictionary = new Dictionary<string, ArrayList>();
            PortalSettings settings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            if (settings.ActivePage.Modules.Count > 0)
            {
                Page page = new Page();
                foreach (ModuleSettings settings2 in settings.ActivePage.Modules)
                {
                    if (!settings2.Cacheable)
                    {
                        settings2.CacheTime = -1;
                    }
                    if (PortalSecurity.IsInRoles(settings2.AuthorizedViewRoles))
                    {
                        ArrayList list;
                        Exception exception;
                        string str = settings2.PaneName.ToLower();
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (!dictionary.ContainsKey(str))
                            {
                                dictionary.Add(str, new ArrayList());
                            }
                            list = dictionary[str];
                        }
                        else
                        {
                            if (!dictionary.ContainsKey("contentpane"))
                            {
                                dictionary.Add("contentpane", new ArrayList());
                            }
                            list = dictionary["contentpane"];
                        }
                        if (!settings2.Admin && (settings2.CacheTime == 0))
                        {
                            int moduleOverrideCache = Config.ModuleOverrideCache;
                            if (moduleOverrideCache > 0)
                            {
                                settings2.CacheTime = moduleOverrideCache;
                            }
                        }
                        if ((((settings2.CacheTime <= 0) || PortalSecurity.HasEditPermissions(settings2.ModuleID)) || (PortalSecurity.HasPropertiesPermissions(settings2.ModuleID) || PortalSecurity.HasAddPermissions(settings2.ModuleID))) || PortalSecurity.HasDeletePermissions(settings2.ModuleID))
                        {
                            try
                            {
                                PortalModuleControl control;
                                string virtualPath = Path.ApplicationRoot + "/" + settings2.DesktopSrc;
                                if (virtualPath.LastIndexOf('.') >= 0)
                                {
                                    control = (PortalModuleControl)page.LoadControl(virtualPath);
                                }
                                else
                                {


                                    string[] strArray = virtualPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                                    string areaName = (strArray[1].ToLower() == "views") ? string.Empty : strArray[1];
                                    string controllerName = strArray[strArray.Length - 2];
                                    string actionName = strArray[strArray.Length - 1];
                                    string ns = strArray[2];


                                    control = (PortalModuleControl)page.LoadControl("/DesktopModules/CoreModules/MVC/MVCModule.ascx");

                                    (control as MVCModuleControl).ControllerName = controllerName;
                                    (control as MVCModuleControl).ActionName = actionName;
                                    (control as MVCModuleControl).AreaName = areaName;
                                    (control as MVCModuleControl).ModID = settings2.ModuleID;

                                    (control as MVCModuleControl).Initialize();


                                }
                                control.PortalID = settings.PortalID;
                                control.ModuleConfiguration = settings2;
                                if ((control.Cultures == string.Empty) || ((control.Cultures + ";").IndexOf(settings.PortalContentLanguage.Name + ";") >= 0))
                                {
                                    list.Add(control);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                ErrorHandler.Publish(LogLevel.Error, "DesktopPanes: Unable to load control '" + settings2.DesktopSrc + "'!", exception);
                                if (PortalSecurity.IsInRoles("Admins"))
                                {
                                    list.Add(new LiteralControl("<br><span class=NormalRed>Unable to load control '" + settings2.DesktopSrc + "'! (Full Error Logged)<br />Error Message: " + exception.Message.ToString()));
                                }
                                else
                                {
                                    list.Add(new LiteralControl("<br><span class=NormalRed>Unable to load control '" + settings2.DesktopSrc + "'!"));
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                using (CachedPortalModuleControl control2 = new CachedPortalModuleControl())
                                {
                                    control2.PortalID = settings.PortalID;
                                    control2.ModuleConfiguration = settings2;
                                    list.Add(control2);
                                }
                            }
                            catch (Exception exception2)
                            {
                                exception = exception2;
                                ErrorHandler.Publish(LogLevel.Error, "DesktopPanes: Unable to load cached control '" + settings2.DesktopSrc + "'!", exception);
                                if (PortalSecurity.IsInRoles("Admins"))
                                {
                                    list.Add(new LiteralControl("<br><span class=NormalRed>Unable to load cached control '" + settings2.DesktopSrc + "'! (Full Error Logged)<br />Error Message: " + exception.Message.ToString()));
                                }
                                else
                                {
                                    list.Add(new LiteralControl("<br><span class=NormalRed>Unable to load cached control '" + settings2.DesktopSrc + "'!"));
                                }
                            }
                        }
                    }
                }
            }
            return dictionary;
        }

        public static Dictionary<string, string> GetMVCActionModules()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(Path.ApplicationPhysicalPath + "/Areas/");
            if (info.Exists)
            {
                System.IO.FileInfo[] files = info.GetFiles("*.ascx", System.IO.SearchOption.AllDirectories);
                foreach (System.IO.FileInfo info2 in files)
                {
                    string key = "[MVC Action] " + info2.DirectoryName.Substring(info2.DirectoryName.LastIndexOf("MVC") + 4) + @"\" + info2.Name.Split(new char[] { '.' })[0];
                    dictionary.Add(key, info2.FullName);
                }
            }
            return dictionary;
        }

        public static void Reorder(Dictionary<string, ArrayList> modulesByPane)
        {
            PortalSettings settings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            int pageID = settings.ActivePage.PageID;
            ModulesDB sdb = new ModulesDB();
            foreach (string str in modulesByPane.Keys)
            {
                ArrayList list = modulesByPane[str];
                int moduleOrder = 0;
                foreach (int num3 in list)
                {
                    sdb.UpdateModuleOrder(num3, moduleOrder, str);
                    moduleOrder++;
                }
            }
        }
    }
}