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
    public class AuditRejectReasonsController : Controller
    {
        private FntEntities db = new FntEntities();

        // GET: AuditRejectReasons
        public ActionResult Index()
        {
            return View(db.AuditRejectReasons.ToList());
        }

        public JsonResult GetAll()
        {
            JsonResult jsonResult = new JsonResult();
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.Data = db.AuditRejectReasons.ToList();

            return jsonResult;
        }

        // GET: AuditRejectReasons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditRejectReason AuditRejectReason = db.AuditRejectReasons.Find(id);
            if (AuditRejectReason == null)
            {
                return HttpNotFound();
            }
            return View(AuditRejectReason);
        }

        // GET: AuditRejectReasons/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AuditRejectReasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] AuditRejectReason AuditRejectReason)
        {
            if (ModelState.IsValid)
            {
                db.AuditRejectReasons.Add(AuditRejectReason);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(AuditRejectReason);
        }

        // GET: AuditRejectReasons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditRejectReason AuditRejectReason = db.AuditRejectReasons.Find(id);
            if (AuditRejectReason == null)
            {
                return HttpNotFound();
            }
            return View(AuditRejectReason);
        }

        // POST: AuditRejectReasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] AuditRejectReason AuditRejectReason)
        {
            if (ModelState.IsValid)
            {
                db.Entry(AuditRejectReason).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(AuditRejectReason);
        }

        // GET: AuditRejectReasons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditRejectReason AuditRejectReason = db.AuditRejectReasons.Find(id);
            if (AuditRejectReason == null)
            {
                return HttpNotFound();
            }
            return View(AuditRejectReason);
        }

        // POST: AuditRejectReasons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AuditRejectReason AuditRejectReason = db.AuditRejectReasons.Find(id);
            db.AuditRejectReasons.Remove(AuditRejectReason);
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
