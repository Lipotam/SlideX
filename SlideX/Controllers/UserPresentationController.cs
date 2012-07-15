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
            return View(new PresentationDataModel { CurrentPresentation = foundPresentation });
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(PresentationDataModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser currentUser = Membership.GetUser();
                if (currentUser != null)
                {
                    var foundPresentation = db.Presentations.SingleOrDefault(p => p.Id == model.CurrentPresentation.Id && p.UserId == (Guid)currentUser.ProviderUserKey );
                    if (foundPresentation == null)
                    {
                        return View("Error", new ErrorPageModels { Title = "Presentation not found.", Message = "Presentation wasn't found. Do you want to edit another's prasentation ?", ShowGotoBack = true });
                    }
                    foundPresentation.Title = model.CurrentPresentation.Title;
                    foundPresentation.Description = model.CurrentPresentation.Description;
                    
                    List<Tag> nonExistingTags = new List<Tag>();
                    foreach (var temp in foundPresentation.Tags.AsEnumerable())
                    {
                        if (model.CurrentPresentation.Tags.SingleOrDefault(p => p.Name == temp.Name) == null)
                        {
                            nonExistingTags.Add(temp);
                        }
                    }

                    foreach (var temp in nonExistingTags)
                    {
                        foundPresentation.Tags.Remove(temp);
                    }
                    foreach (var temp in model.CurrentPresentation.Tags.AsEnumerable())
                    {
                        if (foundPresentation.Tags.SingleOrDefault(p => p.Name == temp.Name) == null)
                        {
                            foundPresentation.Tags.Add(db.Tags.SingleOrDefault(p => p.Name == temp.Name));
                        }
                    }

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
                foundPresentation.Tags.Clear();
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