using Microsoft.AspNet.Identity;
using SciEvidenceBank.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SciEvidenceBank.Controllers
{
    public class EvidenceController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult EviIndex()
        {
            var evidences = db.Evidences.Include(e => e.Category);
            return View(evidences.ToList());
        }
        // GET: /Evidence
        public ActionResult Index(string q = null, int? categoryId = null, int page = 1, int pageSize = 10)
        {
            var query = db.Evidences.Where(e => e.Status == EvidenceStatus.Approved && e.IsPublished);

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(e => e.Title.Contains(q) || e.AbstractText.Contains(q) || e.Authors.Contains(q) || e.Source.Contains(q));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryId.Value);
            }

            var results = query
                .OrderByDescending(e => e.LikesCount)
                .ThenByDescending(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(e => e.Category)
                .ToList();
            // inside Index or a child action
            ViewBag.TopLiked = db.Evidences.OrderByDescending(e => e.LikesCount).Take(3).ToList();
            ViewBag.TopCited = db.Evidences.OrderByDescending(e => e.CitationCount).Take(3).ToList();
            ViewBag.Categories = db.Categories.ToList();
            return View(results);
        }

        // GET: /Evidence/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var evidence = db.Evidences.Include(e => e.Category).Include(e => e.EvidenceTags.Select(et => et.Tag)).FirstOrDefault(e => e.Id == id.Value);
            if (evidence == null) return HttpNotFound();

            // Increment views
            evidence.ViewsCount++;
            db.Entry(evidence).State = EntityState.Modified;
            db.SaveChanges();

            return View(evidence);
        }

        // GET: /Evidence/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Categories = db.Categories.ToList();
            return View();
        }

        // POST: /Evidence/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Evidence model, string[] tags)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = db.Categories.ToList();
                return View(model);
            }

            model.CreatedAt = DateTime.UtcNow;
            model.CreatedById = User.Identity.GetUserId();
            model.CreatedByName = User.Identity.Name;
            model.Status = EvidenceStatus.Pending;
            model.IsPublished = false; // will be set on approval

            db.Evidences.Add(model);
            db.SaveChanges();

            if (tags != null)
            {
                foreach (var t in tags.Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    var tag = db.Tags.FirstOrDefault(x => x.Name == t) ?? new Tag { Name = t };
                    if (tag.Id == 0) db.Tags.Add(tag);
                    db.SaveChanges();

                    if (!db.EvidenceTags.Any(et => et.EvidenceId == model.Id && et.TagId == tag.Id))
                    {
                        db.EvidenceTags.Add(new EvidenceTag { EvidenceId = model.Id, TagId = tag.Id });
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Details", new { id = model.Id });
        }

        // POST: /Evidence/Like
        [HttpPost]
        public async Task<ActionResult> Like(int id)
        {
            var ev = await db.Evidences.FindAsync(id);
            if (ev == null) return HttpNotFound();

            ev.LikesCount = (ev.LikesCount < 0) ? 1 : ev.LikesCount + 1;
            await db.SaveChangesAsync();

            return Json(new { success = true, likes = ev.LikesCount });
        }

        // GET: /Evidence/Search?q=...
        [HttpGet]
        public async Task<ActionResult> Search(string q, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                // return empty result or index
                var empty = Enumerable.Empty<Evidence>();
                return View("Index", await Task.FromResult(empty));
            }

            q = q.Trim();

            var results = await db.Evidences
                .Where(e => e.Title.Contains(q) || e.AbstractText.Contains(q))
                .OrderByDescending(e => e.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // You can create a SearchViewModel for pagination; for brevity return Index view with results
            return View("Index", results);
        }

        // Child action to render sidebar partial with top liked & cited (call from _Layout or wherever)
        //[ChildActionOnly]
        //public ActionResult Sidebar()
        //{
        //    var topLiked = db.Evidences.OrderByDescending(e => e.LikesCount).Take(3).ToList();
        //    var topCited = db.Evidences.OrderByDescending(e => e.CitationCount).Take(3).ToList();

        //    var vm = new SidebarViewModel
        //    {
        //        TopLiked = topLiked,
        //        TopCited = topCited
        //    };

        //    return PartialView("_Sidebar", vm);
        //}
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evidence evidence = db.Evidences.Find(id);
            if (evidence == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", evidence.CategoryId);
            return View(evidence);
        }

        // POST: Evidences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Authors,Source,Year,AbstractText,Url,FilePath,IsPublished,Status,ApprovedById,ApprovedByName,ApprovedAt,CreatedAt,CreatedById,CreatedByName,LikesCount,BookmarksCount,ViewsCount,CitationCount,CategoryId")] Evidence evidence)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evidence).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", evidence.CategoryId);
            return View(evidence);
        }

        // GET: Evidences/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evidence evidence = db.Evidences.Find(id);
            if (evidence == null)
            {
                return HttpNotFound();
            }
            return View(evidence);
        }

        // POST: Evidences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Evidence evidence = db.Evidences.Find(id);
            db.Evidences.Remove(evidence);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: /Evidence/Bookmark/5
        [HttpPost]
        [Authorize]
        public ActionResult Bookmark(int id)
        {
            // For demo: increment counter. Implement user-specific bookmarks table in production.
            var evidence = db.Evidences.Find(id);
            if (evidence == null) return HttpNotFound();

            evidence.BookmarksCount++;
            db.SaveChanges();

            return Json(new { success = true, bookmarks = evidence.BookmarksCount });
        }

        // Moderation list - only Teacher/Admin
        [Authorize(Roles = "Teacher,Admin")]
        public ActionResult Moderate(int page = 1, int pageSize = 20)
        {
            var query = db.Evidences.Where(e => e.Status == EvidenceStatus.Pending)
                .OrderBy(e => e.CreatedAt)
                .Include(e => e.Category);

            var results = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return View(results);
        }

        // Approve evidence
        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        public ActionResult Approve(int id)
        {
            var evidence = db.Evidences.Find(id);
            if (evidence == null) return HttpNotFound();

            evidence.Status = EvidenceStatus.Approved;
            evidence.IsPublished = true;
            evidence.ApprovedAt = DateTime.UtcNow;
            evidence.ApprovedById = User.Identity.GetUserId();
            evidence.ApprovedByName = User.Identity.Name;
            db.SaveChanges();

            return Json(new { success = true, status = "approved" });
        }

        // Reject evidence
        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        public ActionResult Reject(int id, string reason = null)
        {
            var evidence = db.Evidences.Find(id);
            if (evidence == null) return HttpNotFound();

            evidence.Status = EvidenceStatus.Rejected;
            evidence.ApprovedAt = DateTime.UtcNow;
            evidence.ApprovedById = User.Identity.GetUserId();
            evidence.ApprovedByName = User.Identity.Name;
            // You can store 'reason' to a new field if needed
            db.SaveChanges();

            return Json(new { success = true, status = "rejected" });
        }

        // Tag suggestions for autocomplete
        [HttpGet]
        public ActionResult TagSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Json(new string[0], JsonRequestBehavior.AllowGet);

            var tags = db.Tags
                .Where(t => t.Name.Contains(term))
                .OrderBy(t => t.Name)
                .Select(t => t.Name)
                .Take(10)
                .ToArray();

            return Json(tags, JsonRequestBehavior.AllowGet);
        }

        // Search suggestions (titles)
        [HttpGet]
        public ActionResult SearchSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Json(new string[0], JsonRequestBehavior.AllowGet);

            var titles = db.Evidences
                .Where(e => e.Status == EvidenceStatus.Approved && e.IsPublished && e.Title.Contains(term))
                .OrderByDescending(e => e.LikesCount)
                .Select(e => e.Title)
                .Distinct()
                .Take(10)
                .ToArray();

            return Json(titles, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}