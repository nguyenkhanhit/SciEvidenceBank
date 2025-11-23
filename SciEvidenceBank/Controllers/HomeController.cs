using Microsoft.AspNet.Identity;
using SciEvidenceBank.Models;
using SciEvidenceBank.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SciEvidenceBank.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // create a short-lived context to fetch recommendations
                using (var db = new ApplicationDbContext())
                {
                    var svc = new RecommendationService(db);
                    var userId = User.Identity.GetUserId();
                    var recs = svc.GetRecommendationsForUser(userId, top: 5).Select(r => r.Evidence).ToList();
                    ViewBag.Recommendations = recs;
                }
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}