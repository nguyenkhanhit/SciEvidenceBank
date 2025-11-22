using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SciEvidenceBank.Models;

namespace SciEvidenceBank.Controllers
{
    public class EvidencesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Evidences
        public ActionResult Index()
        {
            var evidences = db.Evidences.Include(e => e.Category);
            return View(evidences.ToList());
        }

        // GET: Evidences/Details/5
        public ActionResult Details(int? id)
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

        // GET: Evidences/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Evidences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Authors,Source,Year,AbstractText,Url,FilePath,IsPublished,Status,ApprovedById,ApprovedByName,ApprovedAt,CreatedAt,CreatedById,CreatedByName,LikesCount,BookmarksCount,ViewsCount,CitationCount,CategoryId")] Evidence evidence)
        {
            if (ModelState.IsValid)
            {
                db.Evidences.Add(evidence);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", evidence.CategoryId);
            return View(evidence);
        }

        // GET: Evidences/Edit/5
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
