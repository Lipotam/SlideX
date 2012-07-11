using System;
using System.Collections;
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

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Presentation model)
        {
            Presentation newPresentation = new Presentation()
                                               {
                                                   PresentationId = Guid.NewGuid(),
                                                   Title = model.Title,
                                                   Description = model.Description,
                                                   UserId = (Guid)Membership.GetUser().ProviderUserKey
                                               };
            db.Presentations.AddObject(newPresentation);
            db.SaveChanges();
            return RedirectToAction("PresentationList");
        }

        public ActionResult PresentationList()
        {
            IEnumerable<Presentation> foundPresentation = GetPresentationsByCurrentUserId();
            return View(foundPresentation);
        }

        public ActionResult Edit(Guid id)
        {
            var foundPresentation = GetPresentationByPresentationId(id);
            if (foundPresentation == null)
            {
                return View("Error", new ErrorPageModels { Title = "Presentation not found.", Message = "Presentation wasn't found. May be it was deleted or bad request string.", ShowGotoBack = true });
            }
            return View(new SlideX.Models.PresentationWithTags(foundPresentation));
        }

        [HttpPost]
        public ActionResult Edit(PresentationWithTags model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser currentUser = Membership.GetUser();
                if (currentUser != null)
                {
                    Guid currentUserId = (Guid)currentUser.ProviderUserKey;
                    var foundPresentation = db.Presentations.SingleOrDefault(p => p.PresentationId == model.presentation.PresentationId && p.UserId == currentUserId);
                    if (foundPresentation == null)
                    {
                        return View("Error", new ErrorPageModels { Title = "Presentation not found.", Message = "Presentation wasn't found. Do you want to edit another's prasentation ?", ShowGotoBack = true });
                    }
                    foundPresentation.Title = model.presentation.Title;
                    foundPresentation.Description = model.presentation.Description;
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

        public ActionResult Delete(Guid id)
        {
            Presentation foundPresentation = GetPresentationByCurrentUserIdAndByPreasentationId(id);
            if (foundPresentation != null)
            {
                db.Presentations.DeleteObject(foundPresentation);
                db.SaveChanges();
                return RedirectToAction("PresentationList");
            } 
            else
            {
                return View("Error", new ErrorPageModels { Title = "Presentation not found.", Message = "Presentation wasn't found. Do you want to delete another's prasentation ?", ShowGotoBack = true });
            }
        }

        private IEnumerable<Presentation> GetPresentationsByCurrentUserId()
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser != null)
            {
                return db.Presentations.Where(P => P.UserId == (Guid)currentUser.ProviderUserKey).AsEnumerable();
            }
            else return null;
        }

        private Presentation GetPresentationByCurrentUserIdAndByPreasentationId(Guid presentationId)
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser != null)
            {
                Guid currentUserId = (Guid)currentUser.ProviderUserKey;
                return db.Presentations.Where(P => P.UserId == currentUserId).Where(P => P.PresentationId == presentationId).First();
            }
           else return null;
        }

        private Presentation GetPresentationByPresentationId(Guid id)
        {
            return db.Presentations.SingleOrDefault(P => P.PresentationId == id);
        }
    }
}