using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalRPlay.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Test(string name)
        {
            var c = new HttpCookie("user", name);
            c.Expires = DateTime.Now.AddYears(1);
            c.Path = "/";
            Response.Cookies.Add(c);
            return RedirectToAction("Test2");
        }

        public ActionResult Test2()
        {
            return View();
        }

    }
}
