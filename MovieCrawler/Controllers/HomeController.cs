using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieCrawler.Models;
namespace MovieCrawler.Controllers
{
    public class HomeController : Controller
    {
        MovieCrawlerEntities db = new MovieCrawlerEntities();

        public ActionResult Index()
        {
            var query = from o in db.MovieLists
                        select o;
            return View(query.ToList());
        }
        //[HttpPost]
        //public ActionResult Index(int MovieID)
        //{
        //    var query = from o in db.MovieInfoes
        //                where o.MovieID==MovieID
        //                select o;
        //    return View(query.ToList());
        //}

        public ActionResult items(int MovieID)
        {
            ViewBag.item = MovieID;
            return View();
        }

        public JsonResult MovieInfoData()
        {
            var query = from o in db.MovieInfoes
                        select o;
            var movieList = query.ToList();
            return new JsonResult { Data = movieList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Detail(int MovieID )
        {
            var query = from o in db.MovieLists
                        where o.MovieID == MovieID
                        select o;
            var MovieInfo = query.FirstOrDefault();
            return View(MovieInfo);
        }
    }
}