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
    public class AuditSendersController : Controller
    {
        private FntEntities db = new FntEntities();

        // GET: AuditSenders
        public ActionResult Index()
        {
            return View(db.AuditSenders.ToList());
        }

        // GET: AuditSenders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditSender AuditSender = db.AuditSenders.Find(id);
            if (AuditSender == null)
            {
                return HttpNotFound();
            }
            return View(AuditSender);
        }

        // GET: AuditSenders/Create
        public ActionResult Create()
        {
            ViewBag.CrnId = new SelectList(db.CRNs.Where(x => x.CRNID > 20), "CRNID", "CRNNAME");
            return View();
        }

        // POST: AuditSenders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,EmailAddress,CrnId,AttachmentRequired")] AuditSender AuditSender)
        {
            if (ModelState.IsValid)
            {
                db.AuditSenders.Add(AuditSender);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CrnId = new SelectList(db.CRNs.Where(x => x.CRNID > 20), "CRNID", "CRNNAME");

            return View(AuditSender);
        }

        // GET: AuditSenders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditSender AuditSender = db.AuditSenders.Find(id);
            if (AuditSender == null)
            {
                return HttpNotFound();
            }

            ViewBag.CrnId = new SelectList(db.CRNs.Where(x => x.CRNID > 20), "CRNID", "CRNNAME", AuditSender.CrnId);
            return View(AuditSender);
        }

        // POST: AuditSenders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,EmailAddress, CrnId,AttachmentRequired")] AuditSender AuditSender)
        {
            if (ModelState.IsValid)
            {
                db.Entry(AuditSender).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(AuditSender);
        }

        // GET: AuditSenders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditSender AuditSender = db.AuditSenders.Find(id);
            if (AuditSender == null)
            {
                return HttpNotFound();
            }
            return View(AuditSender);
        }

        // POST: AuditSenders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AuditSender AuditSender = db.AuditSenders.Find(id);
            db.AuditSenders.Remove(AuditSender);
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
