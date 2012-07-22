using System.Linq;
using System.Web.Mvc;
using SlideX.Models;

namespace SlideX.Controllers
{
    /// <summary>
    /// Provides presentation search
    /// </summary>
    public class SearchController : Controller
    {
        /// <summary>
        /// Provides searching tools
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Check search model
        /// </summary>
        /// <param name="id">Contains search string and search type</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(SearchModel id)
        {
            switch (id.Type)
            {
                case SearchType.Presentations: return RedirectToAction("SearchByPresention", new { id = id.SearchString });
                case SearchType.Tags: return RedirectToAction("SearchByTag", new { id = id.SearchString });
                case SearchType.Users: return RedirectToAction("SearchByUser", new { id = id.SearchString });
                default: return View(id);
            }
        }

        /// <summary>
        /// Provides search by presentation result
        /// </summary>
        /// <param name="id">search string</param>
        /// <returns></returns>
        public ActionResult SearchByPresention(string id)
        {
            var foundPresentations = (new PresentationDataAccessModel()).GetPresentationsByNameTemplate(id);
            if (foundPresentations.Count() == 0)
            {
                return View("Error", new ErrorPageModels { Title = Localization.ViewPhrases.PresentationNotFoundSearch, Message = Localization.ViewPhrases.PresentationNotFoundSearchMessage, ShowGotoBack = true });
            }
            return View(foundPresentations);
        }

        /// <summary>
        /// Provides search by tag result
        /// </summary>
        /// <param name="id">search string</param>
        /// <returns></returns>
        public ActionResult SearchByTag(string id)
        {
            var foundPresentations = (new PresentationDataAccessModel()).GetPresentationsByTagTemplate(id);
            if (foundPresentations == null)
            {
                return View("Error", new ErrorPageModels { Title = Localization.ViewPhrases.PresentationNotFoundSearch, Message = Localization.ViewPhrases.PresentationNotFoundSearchMessage, ShowGotoBack = true });
            }
            return View(foundPresentations);
        }

        /// <summary>
        /// Provides search by user name result
        /// </summary>
        /// <param name="id">search string</param>
        /// <returns></returns>
        public ActionResult SearchByUser(string id)
        {
            var foundPresentations = (new PresentationDataAccessModel()).GetPresentationsByUserNameTemplate(id);
            if (foundPresentations == null)
            {
                return View("Error", new ErrorPageModels { Title = Localization.ViewPhrases.PresentationNotFoundSearch, Message = Localization.ViewPhrases.PresentationNotFoundSearchMessage, ShowGotoBack = true });
            }
            return View(foundPresentations);
        }
    }
}