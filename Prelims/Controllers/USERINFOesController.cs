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
    public class USERINFOesController : Controller
    {
        private FntEntities db = new FntEntities();

        // GET: USERINFOes
        public ActionResult Index()
        {
            return View(db.USERINFOes.ToList());
        }

        // GET: USERINFOes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USERINFO uSERINFO = db.USERINFOes.Find(id);
            if (uSERINFO == null)
            {
                return HttpNotFound();
            }
            return View(uSERINFO);
        }

        // GET: USERINFOes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: USERINFOes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "USERID,USERNAME,FULLNAME,PASSWORD,FIRSTNAME,LASTNAME,VENDORID,CUSTOMERID,ROLEID,DT_ADDED,LOGINDATE,ISACTIVE,EMAIL,USERTYPE,ASSIGNCUSTOMER,ISDELETED,ISREVIEWER,IsSupportingServicesReviewer")] USERINFO uSERINFO)
        {
            if (ModelState.IsValid)
            {
                db.USERINFOes.Add(uSERINFO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(uSERINFO);
        }

        // GET: USERINFOes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USERINFO uSERINFO = db.USERINFOes.Find(id);
            if (uSERINFO == null)
            {
                return HttpNotFound();
            }
            return View(uSERINFO);
        }

        // POST: USERINFOes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "USERID,USERNAME,FULLNAME,PASSWORD,FIRSTNAME,LASTNAME,VENDORID,CUSTOMERID,ROLEID,DT_ADDED,LOGINDATE,ISACTIVE,EMAIL,USERTYPE,ASSIGNCUSTOMER,ISDELETED,ISREVIEWER,IsSupportingServicesReviewer")] USERINFO uSERINFO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uSERINFO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(uSERINFO);
        }

        // GET: USERINFOes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USERINFO uSERINFO = db.USERINFOes.Find(id);
            if (uSERINFO == null)
            {
                return HttpNotFound();
            }
            return View(uSERINFO);
        }

        // POST: USERINFOes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            USERINFO uSERINFO = db.USERINFOes.Find(id);
            db.USERINFOes.Remove(uSERINFO);
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
