using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SlideX.Models;

namespace SlideX.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            SlideXDatabaseContext db = new SlideXDatabaseContext();

            
            return View(db.Tags.AsEnumerable());
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
