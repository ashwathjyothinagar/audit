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
    public class AuditHoldReasonsController : Controller
    {
        private FntEntities db = new FntEntities();

        // GET: AuditHoldReasons
        public ActionResult Index()
        {
            return View(db.AuditHoldReasons.ToList());
        }

        public JsonResult GetAll()
        {
            JsonResult jsonResult = new JsonResult();
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.Data = db.AuditHoldReasons.ToList();

            return jsonResult;
        }

        // GET: AuditHoldReasons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditHoldReason AuditHoldReason = db.AuditHoldReasons.Find(id);
            if (AuditHoldReason == null)
            {
                return HttpNotFound();
            }
            return View(AuditHoldReason);
        }

        // GET: AuditHoldReasons/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AuditHoldReasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] AuditHoldReason AuditHoldReason)
        {
            if (ModelState.IsValid)
            {
                db.AuditHoldReasons.Add(AuditHoldReason);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(AuditHoldReason);
        }

        // GET: AuditHoldReasons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditHoldReason AuditHoldReason = db.AuditHoldReasons.Find(id);
            if (AuditHoldReason == null)
            {
                return HttpNotFound();
            }
            return View(AuditHoldReason);
        }

        // POST: AuditHoldReasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] AuditHoldReason AuditHoldReason)
        {
            if (ModelState.IsValid)
            {
                db.Entry(AuditHoldReason).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(AuditHoldReason);
        }

        // GET: AuditHoldReasons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditHoldReason AuditHoldReason = db.AuditHoldReasons.Find(id);
            if (AuditHoldReason == null)
            {
                return HttpNotFound();
            }
            return View(AuditHoldReason);
        }

        // POST: AuditHoldReasons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AuditHoldReason AuditHoldReason = db.AuditHoldReasons.Find(id);
            db.AuditHoldReasons.Remove(AuditHoldReason);
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
