using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Prelims;

namespace Prelims.Controllers
{
    public class AuditErrorCategoriesController : Controller
    {
        private FntEntities db = new FntEntities();

        // GET: AuditErrorCategories
        public ActionResult Index()
        {
            return View(db.AuditErrorCategories.ToList());
        }

        // GET: AuditErrorCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditErrorCategory AuditErrorCategory = db.AuditErrorCategories.Find(id);
            if (AuditErrorCategory == null)
            {
                return HttpNotFound();
            }
            return View(AuditErrorCategory);
        }

        // GET: AuditErrorCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AuditErrorCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] AuditErrorCategory AuditErrorCategory)
        {
            if (ModelState.IsValid)
            {
                db.AuditErrorCategories.Add(AuditErrorCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(AuditErrorCategory);
        }

        // GET: AuditErrorCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditErrorCategory AuditErrorCategory = db.AuditErrorCategories.Find(id);
            if (AuditErrorCategory == null)
            {
                return HttpNotFound();
            }
            return View(AuditErrorCategory);
        }

        // POST: AuditErrorCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] AuditErrorCategory AuditErrorCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(AuditErrorCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(AuditErrorCategory);
        }

        // GET: AuditErrorCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditErrorCategory AuditErrorCategory = db.AuditErrorCategories.Find(id);
            if (AuditErrorCategory == null)
            {
                return HttpNotFound();
            }
            return View(AuditErrorCategory);
        }

        // POST: AuditErrorCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AuditErrorCategory AuditErrorCategory = db.AuditErrorCategories.Find(id);
            db.AuditErrorCategories.Remove(AuditErrorCategory);
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
