using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SlideX.Models;


namespace SlideX.Controllers
{
    public class UserPresentationController : Controller
    {
        private readonly PresentationDataAccessModel presentationData = new PresentationDataAccessModel();
        
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
                    UserId = (Guid)Membership.GetUser().ProviderUserKey,
                    Data =  new StreamReader(Server.MapPath("/Content/DefaultPresentationData.txt")).ReadToEnd()
                };
            newPresentation.Data = newPresentation.Data.Replace("<%Title%>", model.Title);
            presentationData.AddPresentation(newPresentation);
            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            IEnumerable<Presentation> foundPresentation = presentationData.GetPresentationsByCurrentUser();
            return View(foundPresentation);
        }

        [Authorize]
        public ActionResult Edit(Guid id)
        {
            var foundPresentation = presentationData.GetPresentationByPresentationId(id);
            if (foundPresentation == null)
            {
                return View("Error", new ErrorPageModels { Title = "Presentation not found.", Message = "Presentation wasn't found. May be it was deleted or bad request string.", ShowGotoBack = true });
            }
            return View(new PresentationWithTagsModel { CurrentPresentation = foundPresentation });
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(PresentationWithTagsModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser currentUser = Membership.GetUser();
                if (currentUser != null)
                {
                    var foundPresentation = presentationData.GetPresentationByCurrentUserIdAndByPreasentationId(model.CurrentPresentation.Id);
                    if (foundPresentation == null)
                    {
                        return View("Error", new ErrorPageModels { Title = "Presentation not found.", Message = "Presentation wasn't found. Do you want to edit another's prasentation ?", ShowGotoBack = true });
                    }
                    foundPresentation.Title = model.CurrentPresentation.Title;
                    foundPresentation.Description = model.CurrentPresentation.Description;
                    
                    var nonExistingTags = foundPresentation.Tags.AsEnumerable().Where(temp => model.CurrentPresentation.Tags.SingleOrDefault(p => p.Name == temp.Name) == null).ToList();

                    foreach (var temp in nonExistingTags)
                    {
                        foundPresentation.Tags.Remove(temp);
                    }
                    foreach (var temp in model.CurrentPresentation.Tags.AsEnumerable())
                    {
                        if (foundPresentation.Tags.SingleOrDefault(p => p.Name == temp.Name) == null)
                        {
                            foundPresentation.Tags.Add(presentationData.GetTagByName(temp.Name));
                        }
                    }
                    presentationData.ApplyPresentation(foundPresentation);
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Details(Guid id)
        {
            var foundPresentation = presentationData.GetPresentationByPresentationId(id);
            if (foundPresentation == null)
            {
                return View("Error", new ErrorPageModels { Title = "Presentation not found.", Message = "Presentation wasn't found. May be it was deleted or bad request string.", ShowGotoBack = true });
            }
            return View(foundPresentation);
        }

        [Authorize]
        public ActionResult Delete(Guid id)
        {
            Presentation foundPresentation = presentationData.GetPresentationByCurrentUserIdAndByPreasentationId(id);
            if (foundPresentation != null)
            {
                foundPresentation.Tags.Clear();
                presentationData.DeletePresentation(foundPresentation);
                
                return RedirectToAction("Index");
            }
            return View("Error", new ErrorPageModels { Title = "Presentation not found.", Message = "Presentation wasn't found. Do you want to delete another's prasentation ?", ShowGotoBack = true });
        }

        [OutputCache(Duration = 300)]
        public JsonResult GetTagsJson()
        {
            return Json(presentationData.GetTagNames(), JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 300)]
        [HttpPost]
        public JsonResult GetPresentationData(Guid id)
        {
            return Json(presentationData.GetPresentationByPresentationId(id).Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Editor(Guid id)
        {
            return View(presentationData.GetPresentationByPresentationId(id));
        }

        public ActionResult Preview(Guid id)
        {
            return View(presentationData.GetPresentationByPresentationId(id));
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SavePresentationData(Presentation inputPresentation)
        {
         
            Presentation presentationToSave =
                presentationData.GetPresentationByCurrentUserIdAndByPreasentationId(inputPresentation.Id);
            
            if(presentationToSave != null)
            {
                presentationToSave.Data = inputPresentation.Data;
                presentationData.ApplyPresentation(presentationToSave);
            }
            return null;
        }
    }
}