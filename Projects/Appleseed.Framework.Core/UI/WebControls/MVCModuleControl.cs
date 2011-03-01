using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appleseed.Framework.Web.UI.WebControls;
using System.Web.Routing;
using System.Collections;
using System.Reflection;
using Appleseed.Framework.Settings;

namespace Appleseed.Framework.Web.UI.WebControls
{
    public class MVCModuleControl: PortalModuleControl
    {
        public string ActionName { get; set; }
        public string AreaName { get; set; }
        public string ControllerName { get; set; }
        public int ModID { get; set; }

        public MVCModuleControl()
        {
          
        }

        public void Initialize(){
            if (!String.IsNullOrEmpty(ActionName))
            {
                RouteCollection routes = RouteTable.Routes;
                int num = 0;

                for (num = 0; num < routes.Count; num++)
                {
                    if (routes[num].GetType().Name == "Route")
                    {
                        string[] strArray = (string[])((Route)routes[num]).DataTokens["namespaces"];
                        if (strArray != null && strArray[0].Contains(AreaName))
                        {
                            Hashtable hashtable2 = GetMVCModuleSettingsDefinitions(strArray[0], AreaName, ControllerName, ActionName);

                            foreach (string key in hashtable2.Keys)
                            {
                                if (!_baseSettings.ContainsKey(key))
                                {
                                    _baseSettings.Add(key, hashtable2[key]);
                                }
                            }

                        }
                    }
                }

                this.ModuleID = ModID;  
                var s = Settings;
            }
        }

        private static Hashtable GetMVCModuleSettingsDefinitions(string controllerNamespace, string areaName, string controllerName, string actionName)
        {
            Hashtable hashtable = new Hashtable();
            int index = controllerNamespace.ToLower().IndexOf(".mvc");
            if (index < 0)
            {
                index = controllerNamespace.ToLower().IndexOf(".areas");
            }
            if (index < 0)
            {
                //Caso para las portableareas
                index = controllerNamespace.ToLower().IndexOf(".controllers");
            }
            if (index >= 0)
            {
                string str = controllerNamespace.Substring(0, index);
                try
                {
                    object target = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "/bin/" + str + ".dll").CreateInstance(controllerNamespace + "." + controllerName + "Controller");

                    Hashtable hashtable2 = (Hashtable)target.GetType().InvokeMember(actionName + "_SettingsDefinitions", BindingFlags.InvokeMethod, null, target, null, null);
                    hashtable = hashtable2;
                }
                catch (Exception exc)
                {
                    ErrorHandler.Publish(LogLevel.Debug, String.Format("Error al obtener los settings para {0}/{1}/{2} con namespace: {3}", areaName, controllerName, actionName, controllerNamespace), exc);
                }
            }
            

            return hashtable;


        }
    
    }
}

