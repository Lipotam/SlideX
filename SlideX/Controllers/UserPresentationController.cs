using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SlideX.Models;


namespace SlideX.Controllers
{
    public class UserPresentationController : Controller
    {
        ASPNETDBEntities db = new ASPNETDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Presentation model)
        {
            MembershipUser currentUser = Membership.GetUser();
            Guid currentUserId = (Guid) currentUser.ProviderUserKey;

            Presentation newPresentation = new Presentation();
            newPresentation.PresentationId = Guid.NewGuid();
            newPresentation.Title = model.Title;
            newPresentation.Description = model.Description;

            newPresentation.UserId = currentUserId;
            db.Presentations.AddObject(newPresentation);
            db.SaveChanges();

            return RedirectToAction("PresentationList");
        }

        public ActionResult PresentationList()
        {
            var foundPresentation = GetPresentationsByCurrentUserId();
            if (foundPresentation == null)
            {
                return View();
            }
            return View(foundPresentation);
        }

        public ActionResult Edit(Guid id)
        {
            var foundPresentation = GetPresentationByPresentationId(id);
            if (foundPresentation == null)
            {
                return View("Error", new ErrorPageModels { Title = "Presentation not found.", Message = "Presentation wasn't found. May be it was deleted or bad request string.", ShowGotoBack = true });
            }
            return View(foundPresentation);
        }

        [HttpPost]
        public ActionResult Edit(Presentation model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser currentUser = Membership.GetUser();
                if (currentUser != null)
                {
                    Guid currentUserId = (Guid)currentUser.ProviderUserKey;
                    var foundPresentation = db.Presentations.SingleOrDefault(p => p.PresentationId == model.PresentationId && p.UserId == currentUserId);
                    if (foundPresentation == null)
                    {
                        return View("Error", new ErrorPageModels { Title = "Presentation not found.", Message = "Presentation wasn't found. Do you want to edit another's prasentation ?", ShowGotoBack = true });
                    }
                    foundPresentation.Title = model.Title;
                    foundPresentation.Description = model.Description;
                    db.SaveChanges();
                }
            }
            return View(model);
        }


        public ActionResult Details(Guid id)
        {
            var foundPresentation = GetPresentationByPresentationId(id);
            if (foundPresentation == null)
            {
                return View("Error", new ErrorPageModels { Title = "Presentation not found.", Message = "Presentation wasn't found. May be it was deleted or bad request string.", ShowGotoBack = true });
            }
            return View(foundPresentation);
        }


        private System.Object GetPresentationsByCurrentUserId()
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser != null)
            {
                return db.Presentations.Where(P => P.UserId == (Guid)currentUser.ProviderUserKey);
            }
            else return null;
        }

        private System.Object GetPresentationsByCurrentUserIdAndByPreasentationId(Guid id)
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser != null)
            {
                Guid currentUserId = (Guid)currentUser.ProviderUserKey;
                return db.Presentations.Where(P => P.UserId == currentUserId).Where(P => P.PresentationId == id).First();
            }
            else return null;
        }

        private Presentation GetPresentationByPresentationId(Guid id)
        {
            return db.Presentations.SingleOrDefault(P => P.PresentationId == id);
        }
    }
}