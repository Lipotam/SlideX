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

            return View((new PresentationSearchModel()).GetPresentationsByName(id));
        }

        public ActionResult SearchByTag(string id)
        {
            return View((new PresentationSearchModel()).GetPresentationsByTag(id));
        }
        public ActionResult SearchByUser(string id)
        {

            var p = (new PresentationSearchModel()).GetPresentationsByUserName(id);
            
            return View(p);
        }
    }
}
