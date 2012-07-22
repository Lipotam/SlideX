using System.Linq;
using System.Web.Mvc;
using SlideX.Models;

namespace SlideX.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
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

        public ActionResult SearchByPresention(string id)
        {
            var foundPresentations = (new PresentationDataAccessModel()).GetPresentationsByNameTemplate(id);
            if (foundPresentations.Count() == 0)
            {
                return View("Error", new ErrorPageModels { Title = "Presentations not found.", Message = "Presentations were not found by this search string.", ShowGotoBack = true });
            }
            return View(foundPresentations);
        }

        public ActionResult SearchByTag(string id)
        {
            var foundPresentations = (new PresentationDataAccessModel()).GetPresentationsByTagTemplate(id);
            if (foundPresentations.Count() == 0)
            {
              return View("Error", new ErrorPageModels { Title = "Presentations not found.", Message = "Presentations were not found by this search string.", ShowGotoBack = true });
            }
            return View(foundPresentations);
        }

        public ActionResult SearchByUser(string id)
        {
            var foundPresentations = (new PresentationDataAccessModel()).GetPresentationsByUserNameTemplate(id);
            if (foundPresentations.Count() == 0)
            {
                return View("Error", new ErrorPageModels { Title = "Presentations not found.", Message = "Presentations were not found by this search string.", ShowGotoBack = true });
            }
            return View(foundPresentations);
        }
    }
}
