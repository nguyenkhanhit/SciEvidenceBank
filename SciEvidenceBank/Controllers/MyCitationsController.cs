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
    [Authorize]
    public class MyCitationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MyCitations
        public ActionResult Index()
        {
            return View(db.MyCitations.ToList());
        }

        // GET: MyCitations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyCitation myCitation = db.MyCitations.Find(id);
            if (myCitation == null)
            {
                return HttpNotFound();
            }
            return View(myCitation);
        }

        // GET: MyCitations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MyCitations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,EvidenceId,Style,CitationText,CreatedAt")] MyCitation myCitation)
        {
            if (ModelState.IsValid)
            {
                db.MyCitations.Add(myCitation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(myCitation);
        }

        // GET: MyCitations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyCitation myCitation = db.MyCitations.Find(id);
            if (myCitation == null)
            {
                return HttpNotFound();
            }
            return View(myCitation);
        }

        // POST: MyCitations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,EvidenceId,Style,CitationText,CreatedAt")] MyCitation myCitation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(myCitation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(myCitation);
        }

        // GET: MyCitations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyCitation myCitation = db.MyCitations.Find(id);
            if (myCitation == null)
            {
                return HttpNotFound();
            }
            return View(myCitation);
        }

        // POST: MyCitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MyCitation myCitation = db.MyCitations.Find(id);
            db.MyCitations.Remove(myCitation);
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
