using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SlideX.Models;

namespace SlideX.Controllers
{
    public class AdminPanelController : Controller
    {
        //
        // GET: /AdminPanel/

        public ActionResult Index()
        {
            return View(new UserSearchModel());
        }

   
   
        [HttpPost]
        public ActionResult Index(UserSearchModel model)
        {
            return View(model);
        }

      
    }
}
