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
        //
        // GET: /UserPresentation/
        ASPNETDBEntities db = new ASPNETDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PresentationList()
        {
            return View(GetPresentationsByCurrentUserId());
        }

        public ActionResult Edit(Guid id)
        {
            return View(GetPresentationByPresentationId(id));
        }

        [HttpPost]
        public ActionResult Edit(presentation model)
        {

            if (ModelState.IsValid)
            {
                var pp = db.presentations.SingleOrDefault(p => p.PresentationId == model.PresentationId);
                Debug.Assert(model != null);
                pp.Title = model.Title;
                pp.Description = model.Description;
                db.SaveChanges();
            }

            return View(model);
        }


        public ActionResult Details(Guid id)
        {
            var foundPresentation = GetPresentationByPresentationId(id);
            if (foundPresentation == null)
            {
                return View("Error");

                // TODO error model
            }

            return View(foundPresentation);

        }


        private System.Object GetPresentationsByCurrentUserId()
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser != null)
            {

                return db.presentations.Where(P => P.UserId == (Guid)currentUser.ProviderUserKey);
            }
            else return null;
        }

        private System.Object GetPresentationsByCurrentUserIdAndByPreasentationId(Guid id)
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser != null)
            {
                Guid currentUserId = (Guid)currentUser.ProviderUserKey;
                return db.presentations.Where(P => P.UserId == currentUserId).Where(P => P.PresentationId == id).First();
            }
            else return null;
        }

        private presentation GetPresentationByPresentationId(Guid id)
        {
            return db.presentations.SingleOrDefault(P => P.PresentationId == id);
        }
    }
}