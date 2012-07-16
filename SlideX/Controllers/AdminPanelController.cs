using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SlideX.Models;

namespace SlideX.Controllers
{
    public class AdminPanelController : Controller
    {
        private PresentationDataAccessModel presentationData = new PresentationDataAccessModel();
        public ActionResult Index()
        {
            return View(new UserSearchModel());
        }

   
   
        [HttpPost]
        public ActionResult Index(UserSearchModel model)
        {
            return View(model);
        }

        [Authorize(Roles = "AdminUser")]
        public ActionResult Edit(Guid id)
        {
            
            return View(new UserEditModel{UserId = id});
        }

        [HttpPost]
        [Authorize(Roles="AdminUser")]
        public ActionResult Edit(UserEditModel model)
        {
            return View(model);
        }

        [Authorize(Roles = "AdminUser")]

        public ActionResult Delete(Guid id)
        {
            Guid userEditId = presentationData.GetUserIdByPresentationId(id);
            
            if (presentationData.DeletePresentationById(id) || userEditId == id)
            {
                return View("Error", new ErrorPageModels { Title = "Presentation not found.", Message = "Presentation wasn't found. Do you want to delete another's prasentation ?", ShowGotoBack = true });
            }
            return RedirectToAction("Edit",new {id = userEditId});
        }
    }
}
