using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignalRPlay.Web.Models;

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

        [HttpPost]
        public ActionResult Index(string name)
        {
            
            if(string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("name", "Får inte vara tomt.");
            }

            if(Models.Game.World.AllUserData().Any(p => p.Key == name))
            {
                ModelState.AddModelError("name", "Upptaget namn.");
            }

            if(!ModelState.IsValid)
            {
                return View();
            }

            var c = new HttpCookie("user", name) {Expires = DateTime.Now.AddYears(1), Path = "/"};
            Response.Cookies.Add(c);
            return RedirectToAction("Game");
        }
        
        public ActionResult Game()
        {
            return View();
        }

        public ActionResult Reset()
        {
            Models.Game.World.ResetAll();
            return RedirectToAction("Game");
        }
    }
}
