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
    [Authorize(Roles = "Admin")]
    public class AuditRequestTypesController : Controller
    {
        private FntEntities db = new FntEntities();

        // GET: AuditRequestTypes
        public ActionResult Index()
        {
            return View(db.AuditRequestTypes.ToList());
        }

        // GET: AuditRequestTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditRequestType taxRequestType = db.AuditRequestTypes.Find(id);
            if (taxRequestType == null)
            {
                return HttpNotFound();
            }
            return View(taxRequestType);
        }

        // GET: AuditRequestTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AuditRequestTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] AuditRequestType taxRequestType)
        {
            if (ModelState.IsValid)
            {
                db.AuditRequestTypes.Add(taxRequestType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(taxRequestType);
        }

        // GET: AuditRequestTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditRequestType taxRequestType = db.AuditRequestTypes.Find(id);
            if (taxRequestType == null)
            {
                return HttpNotFound();
            }
            return View(taxRequestType);
        }

        // POST: AuditRequestTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] AuditRequestType taxRequestType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(taxRequestType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(taxRequestType);
        }

        // GET: AuditRequestTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditRequestType taxRequestType = db.AuditRequestTypes.Find(id);
            if (taxRequestType == null)
            {
                return HttpNotFound();
            }
            return View(taxRequestType);
        }

        // POST: AuditRequestTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AuditRequestType taxRequestType = db.AuditRequestTypes.Find(id);
            db.AuditRequestTypes.Remove(taxRequestType);
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
