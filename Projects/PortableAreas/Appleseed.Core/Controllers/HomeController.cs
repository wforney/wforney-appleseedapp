using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;

namespace Appleseed.Core.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult ListResources()
        {
            string[] resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            ViewData.Model = resources;
            return View();
        }

        public JsonResult TestValue()
        {
            return Json("testString");
        }
    }
}
