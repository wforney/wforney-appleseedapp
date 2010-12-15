using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Globalization;
using Appleseed.Framework.Core.Model;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Text;
using System.Data.SqlClient;



[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService()]
public class PortalService : System.Web.Services.WebService
{

    public PortalService()
    {

    }

    [WebMethod]
    public string Reorder(string data)
    {
        string result = "OK";
        try
        {
            char[] separator = { ';' };
            char[] separator2 = { ',' };


            string[] modules = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, ArrayList> newModulesOrder = new Dictionary<string, ArrayList>();
            foreach (string s in modules)
            {
                int arrobaIdx = s.IndexOf('@');
                string paneName = s.Substring(0, arrobaIdx);
                string[] modulesId = s.Substring(arrobaIdx + 1).Split(separator2, StringSplitOptions.RemoveEmptyEntries);

                ArrayList moduleArray = new ArrayList();
                foreach (string modId in modulesId)
                {
                    moduleArray.Add(Convert.ToInt32(modId.ToLower().Replace("mid", "")));
                }

                newModulesOrder.Add(paneName, moduleArray);
            }


            ModelServices.Reorder(newModulesOrder);
        }
        catch (Exception exc)
        {
            result = "ERROR:" + exc.Message;
        }

        return result;
    }
}