using System;
using System.Web.Mvc;
using SlideX.Models;

namespace SlideX.Controllers
{
    /// <summary>
    /// Lets admin to rule users' activity 
    /// </summary>
    public class AdminPanelController : Controller
    {
        private readonly PresentationDataAccessModel presentationData = new PresentationDataAccessModel();

        /// <summary>
        /// Provides user search tools
        /// </summary>
        public ActionResult Index()
        {
            return View(new UserSearchModel());
        }

        /// <summary>
        /// Provides user search result
        /// </summary>
        /// <param name="model">contains search string</param> 
        [HttpPost]
        public ActionResult Index(UserSearchModel model)
        {
            return View(model);
        }

        /// <summary>
        /// Provides change user activity
        /// </summary>
        /// <param name="id">User Id</param> 
        /// <returns></returns>
        [Authorize(Roles = "AdminUser")]
        public ActionResult Edit(Guid id)
        {
            
            return View(new UserEditModel{UserId = id});
        }

        /// <summary>
        /// Edits the specified model.
        /// </summary>
        /// <param name="model">Contains user's adminRole, BannedFlag and presentations.</param>
        [HttpPost]
        [Authorize(Roles="AdminUser")]
        public ActionResult Edit(UserEditModel model)
        {
            return View(model);
        }

        /// <summary>
        /// Deletes the specified presentation
        /// </summary>
        /// <param name="id"> presentation id</param>
        /// <returns></returns>
        [Authorize(Roles = "AdminUser")]
        public ActionResult Delete(Guid id)
        {
            Guid userEditId = presentationData.GetUserIdByPresentationId(id);
            
            if (presentationData.DeletePresentationById(id) || userEditId == id)
            {
                return View("Error", new ErrorPageModels
                                         {
                                             Title = Localization.ViewPhrases.PresentationNotFoundDelete,
                                             Message = Localization.ViewPhrases.PresentationNotFoundDeleteMessage,
                                             ShowGotoBack = true
                                         });
            }
            return RedirectToAction("Edit",new {id = userEditId});
        }
    }
}
