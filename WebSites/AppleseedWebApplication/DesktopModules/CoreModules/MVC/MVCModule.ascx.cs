using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework.Web.UI.WebControls;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Appleseed.Framework;

public partial class DesktopModules_CoreModules_MVC_MVCModule :  MVCModuleControl
{

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!Page.ClientScript.IsClientScriptBlockRegistered("extra"))
        {
            string extraScripts = GetExtraScripts();
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "extra", extraScripts, false);
        }
    }


    private string GetExtraScripts()
    {
        string scripts = string.Empty;
        try
        {
            string filePath = HttpContext.Current.Server.MapPath("~/Scripts/Scripts.xml");
            XDocument xml = XDocument.Load(filePath);
            foreach (var s in xml.Descendants("scripts").DescendantNodes())
            {
                scripts += s.ToString() + Environment.NewLine;
            }
        }
        catch (Exception exc)
        {
            ErrorHandler.Publish(LogLevel.Debug, exc);
        }
        return scripts;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public DesktopModules_CoreModules_MVC_MVCModule()
    {
       
    }


    public override Guid GuidID
    {
        get
        {
            return new Guid("{9073EC6C-9E21-44ba-A33E-22F0E301B867}");
        }
    }


}
