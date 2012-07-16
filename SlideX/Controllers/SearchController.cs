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
            return View((new PresentationDataAccessModel()).GetPresentationsByNameTemplate(id));
        }

        public ActionResult SearchByTag(string id)
        {
            return View((new PresentationDataAccessModel()).GetPresentationsByTagTemplate(id));
        }

        public ActionResult SearchByUser(string id)
        {
            return View((new PresentationDataAccessModel()).GetPresentationsByUserNameTemplate(id));
        }
    }
}
