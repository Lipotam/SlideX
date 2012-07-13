using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SlideX.Models;


namespace SlideX.Controllers
{
    public class UserPresentationController : Controller
    {
        readonly SlideXDatabaseContext db = new SlideXDatabaseContext();

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Presentation model)
        {
            var newPresentation = new Presentation
                {
                    Title = model.Title,
                    Description = model.Description,
                    UserId = (Guid)Membership.GetUser().ProviderUserKey
                };
            db.Presentations.AddObject(newPresentation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            IEnumerable<Presentation> foundPresentation = GetPresentationsByCurrentUserId();
            return View(foundPresentation);
        }

        [Authorize]
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
        [Authorize]
        public ActionResult Edit(Presentation model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser currentUser = Membership.GetUser();
                if (currentUser != null)
                {
                    var currentUserId = (Guid)currentUser.ProviderUserKey;
                    var foundPresentation = db.Presentations.SingleOrDefault(p => p.Id == model.Id && p.UserId == currentUserId);
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

        public ActionResult Delete(Guid id)
        {
            Presentation foundPresentation = GetPresentationByCurrentUserIdAndByPreasentationId(id);
            if (foundPresentation != null)
            {
                db.Presentations.DeleteObject(foundPresentation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Error", new ErrorPageModels { Title = "Presentation not found.", Message = "Presentation wasn't found. Do you want to delete another's prasentation ?", ShowGotoBack = true });
        }

        [OutputCache(Duration = 300)]
        public JsonResult GetTagsJson()
        {
            return Json(db.Tags.Select(t => t.Name).ToArray(), JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<Presentation> GetPresentationsByCurrentUserId()
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser != null)
            {
                return db.Presentations.Where(p => p.UserId == (Guid)currentUser.ProviderUserKey).AsEnumerable();
            }
            return null;
        }

        private Presentation GetPresentationByCurrentUserIdAndByPreasentationId(Guid presentationId)
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser != null)
            {
                var currentUserId = (Guid)currentUser.ProviderUserKey;
                return db.Presentations.SingleOrDefault(p => p.UserId == currentUserId && p.Id == presentationId);
            }
            return null;
        }

        private Presentation GetPresentationByPresentationId(Guid id)
        {
            return db.Presentations.SingleOrDefault(p => p.Id == id);
        }
    }
}