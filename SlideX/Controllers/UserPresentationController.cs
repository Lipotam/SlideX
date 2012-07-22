using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SlideX.Models;

namespace SlideX.Controllers
{
    /// <summary>
    /// Provides tools for working with presentations to user 
    /// </summary>
    public class UserPresentationController : Controller
    {
        private readonly PresentationDataAccessModel presentationData = new PresentationDataAccessModel();

        /// <summary>
        /// Creates presentation
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Check the create presentation model
        /// </summary>
        /// <param name="model">contains presentation title, description</param>
        /// <returns></returns>
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

        /// <summary>
        /// Shows all the presentations of current user
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            IEnumerable<Presentation> foundPresentation = presentationData.GetPresentationsByCurrentUser();
            return View(foundPresentation);
        }

        /// <summary>
        /// Provides user with tool for editing presentation description
        /// </summary>
        /// <param name="id">presentation id</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(Guid id)
        {
            var foundPresentation = presentationData.GetPresentationByPresentationId(id);
            if (foundPresentation == null)
            {
                return View("Error", new ErrorPageModels
                                         {
                                             Title = Localization.ViewPhrases.PresentationNotFoundBadRequest,
                                             Message = Localization.ViewPhrases.PresentationNotFoundBadRequestMessage, 
                                             ShowGotoBack = true
                                         });
            }
            return View(new PresentationWithTagsModel { CurrentPresentation = foundPresentation });
        }

        /// <summary>
        /// Check the model
        /// </summary>
        /// <param name="model">contains presentation title, description</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult Edit(PresentationWithTagsModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser currentUser = Membership.GetUser();
                if (currentUser != null)
                {
                    var foundPresentation = presentationData.GetPresentationByCurrentUserIdAndByPresentationId(model.CurrentPresentation.Id);
                    if (foundPresentation == null)
                    {
                        return View("Error", new ErrorPageModels
                        {
                            Title = Localization.ViewPhrases.PresentationNotFoundBadRequest,
                            Message = Localization.ViewPhrases.PresentationNotFoundBadRequestMessage,
                            ShowGotoBack = true
                        });
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

        /// <summary>
        /// Shows presentation info
        /// </summary>
        /// <param name="id">presentation id</param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var foundPresentation = presentationData.GetPresentationByPresentationId(id);
            if (foundPresentation == null)
            {
                return View("Error", new ErrorPageModels
                {
                    Title = Localization.ViewPhrases.PresentationNotFoundBadRequest,
                    Message = Localization.ViewPhrases.PresentationNotFoundBadRequestMessage,
                    ShowGotoBack = true
                });
            }
            return View(foundPresentation);
        }

        /// <summary>
        /// Deletes the specified presentation
        /// </summary>
        /// <param name="id">presentation id</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Delete(Guid id)
        {
            Presentation foundPresentation = presentationData.GetPresentationByCurrentUserIdAndByPresentationId(id);
            if (foundPresentation != null)
            {
                foundPresentation.Tags.Clear();
                presentationData.DeletePresentation(foundPresentation);
                
                return RedirectToAction("Index");
            }
             return View("Error", new ErrorPageModels { Title = Localization.ViewPhrases.PresentationNotFoundDelete, Message = Localization.ViewPhrases.PresentationNotFoundDeleteMessage, ShowGotoBack = true });
        }

        /// <summary>
        /// Gets the tags json.
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 300)]
        public JsonResult GetTagsJson()
        {
            return Json(presentationData.GetTagNames(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the presentation content
        /// </summary>
        /// <param name="id">Presentation id</param>
        /// <returns></returns>
        [OutputCache(Duration = 300)]
        [HttpPost]
        public JsonResult GetPresentationData(Guid id)
        {
            return Json(presentationData.GetPresentationByPresentationId(id).Data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///Strut Editor for the specified presentation.
        /// </summary>
        /// <param name="id">Presentation id.</param>
        /// <returns></returns>
        public ActionResult Editor(Guid id)
        {
            return View(presentationData.GetPresentationByPresentationId(id));
        }

        /// <summary>
        /// Strut preview for the specified presentation.
        /// </summary>
        /// <param name="id">Presentation id.</param>
        /// <returns></returns>
        public ActionResult Preview(Guid id)
        {
            return View(presentationData.GetPresentationByPresentationId(id));
        }


        /// <summary>
        /// Saves the presentation content.
        /// </summary>
        /// <param name="inputPresentation">Contains presentation id and content.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SavePresentationData(Presentation inputPresentation)
        {
         
            Presentation presentationToSave =
                presentationData.GetPresentationByCurrentUserIdAndByPresentationId(inputPresentation.Id);
            
            if(presentationToSave != null)
            {
                presentationToSave.Data = inputPresentation.Data;
                presentationData.ApplyPresentation(presentationToSave);
            }
            return null;
        }
    }
}