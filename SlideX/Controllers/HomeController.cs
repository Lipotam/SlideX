using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SlideX.Models;

namespace SlideX.Controllers
{
    /// <summary>
    /// Provides information about project
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Contains tagCloud
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            Random randomNumber = new Random();
            string url = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "Search/SearchByTag/";
            var tagsForCloud = new PresentationDataAccessModel().GetAllTags().Select(tag => new TagCloudModel
                                                                                                {
                                                                                                    Text = tag.Name,
                                                                                                    Link = url + tag.Name,
                                                                                                    Weight = randomNumber.Next(7).ToString()
                                                                                                }).ToList();
            ViewBag.TagsCloudString = JsonConvert.SerializeObject(tagsForCloud, Formatting.Indented,
                                          new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return View();
        }

        /// <summary>
        /// Abouts this project
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            return View();
        }
    }
}
