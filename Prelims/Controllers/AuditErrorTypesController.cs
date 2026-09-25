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
    public class AuditErrorTypesController : Controller
    {
        private FntEntities db = new FntEntities();

        // GET: AuditErrorTypes
        public ActionResult Index()
        {
            var AuditErrorTypes = db.AuditErrorTypes.Include(t => t.AuditErrorCategory);
            return View(AuditErrorTypes.ToList());
        }

        // GET: AuditErrorTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditErrorType AuditErrorType = db.AuditErrorTypes.Find(id);
            if (AuditErrorType == null)
            {
                return HttpNotFound();
            }
            return View(AuditErrorType);
        }

        // GET: AuditErrorTypes/Create
        public ActionResult Create()
        {
            ViewBag.AuditErrorCategoryId = new SelectList(db.AuditErrorCategories, "Id", "Name");
            return View();
        }

        // POST: AuditErrorTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,AuditErrorCategoryId,IsCritical")] AuditErrorType AuditErrorType)
        {
            if (ModelState.IsValid)
            {
                db.AuditErrorTypes.Add(AuditErrorType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AuditErrorCategoryId = new SelectList(db.AuditErrorCategories, "Id", "Name", AuditErrorType.AuditErrorCategoryId);
            return View(AuditErrorType);
        }

        // GET: AuditErrorTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditErrorType AuditErrorType = db.AuditErrorTypes.Find(id);
            if (AuditErrorType == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuditErrorCategoryId = new SelectList(db.AuditErrorCategories, "Id", "Name", AuditErrorType.AuditErrorCategoryId);
            return View(AuditErrorType);
        }

        // POST: AuditErrorTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,AuditErrorCategoryId,IsCritical")] AuditErrorType AuditErrorType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(AuditErrorType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuditErrorCategoryId = new SelectList(db.AuditErrorCategories, "Id", "Name", AuditErrorType.AuditErrorCategoryId);
            return View(AuditErrorType);
        }

        // GET: AuditErrorTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditErrorType AuditErrorType = db.AuditErrorTypes.Find(id);
            if (AuditErrorType == null)
            {
                return HttpNotFound();
            }
            return View(AuditErrorType);
        }

        // POST: AuditErrorTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AuditErrorType AuditErrorType = db.AuditErrorTypes.Find(id);
            db.AuditErrorTypes.Remove(AuditErrorType);
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
