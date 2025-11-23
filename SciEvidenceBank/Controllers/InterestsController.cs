using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SciEvidenceBank.Models;
using SciEvidenceBank.ViewModels;
using System.Collections.Generic;

namespace SciEvidenceBank.Controllers
{
    [Authorize]
    public class InterestsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Interests/Select?returnUrl=/...
        public ActionResult Select(string returnUrl = null)
        {
            var userId = User.Identity.GetUserId();
            var all = db.Categories.ToList(); // use Categories as the topics
            var selectedIds = db.UserInterests.Where(ui => ui.UserId == userId).Select(ui => ui.CategoryId).ToHashSet();

            var items = all.Select(f => new FieldItem
            {
                Id = f.Id,
                Name = f.Name,
                Selected = selectedIds.Contains(f.Id)
            }).ToList();

            ViewBag.ReturnUrl = returnUrl;
            return View(items); // view is strongly-typed to List<FieldItem>
        }

        // POST: /Interests/Select
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Select(int[] selectedFieldIds, string returnUrl = null)
        {
            var userId = User.Identity.GetUserId();
            selectedFieldIds = selectedFieldIds ?? new int[0];

            var current = db.UserInterests.Where(ui => ui.UserId == userId).ToList();

            // remove unselected
            foreach (var rel in current.Where(r => !selectedFieldIds.Contains(r.CategoryId)).ToList())
            {
                db.UserInterests.Remove(rel);
            }

            // add new
            var existingIds = current.Select(c => c.CategoryId).ToHashSet();
            foreach (var fid in selectedFieldIds.Where(fid => !existingIds.Contains(fid)))
            {
                db.UserInterests.Add(new UserInterest { UserId = userId, CategoryId = fid });
            }

            db.SaveChanges();

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}