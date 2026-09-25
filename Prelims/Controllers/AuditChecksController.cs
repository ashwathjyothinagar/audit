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
    public class AuditChecksController : Controller
    {
        private FntEntities db = new FntEntities();

        // GET: AuditChecks
        public ActionResult Index()
        {
            var checks = db.AuditChecks.ToList();
            var taskNames = db.AuditTasks.ToDictionary(x => x.Id, y => y.Name);
            var officeNames = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).ToDictionary(x => x.CRNID, y => y.CRNNAME);
            foreach (var check in checks)
            {
                var selectedTaskIds = db.AuditCheckTasks.Where(x => x.AuditCheckId == check.Id).Select(x => x.TaskId).ToList();
                var selectedOfficeIds = db.AuditCheckCrns.Where(x => x.AuditCheckId == check.Id).Select(x => x.CrnId).ToList();

                var selectedTaskNames = new List<string>();
                foreach(var taskId in selectedTaskIds)
                {
                    selectedTaskNames.Add(taskNames[taskId]);
                }

                var selectedOfficeNames = new List<string>();
                foreach (var officeId in selectedOfficeIds)
                {
                    selectedOfficeNames.Add(officeNames[officeId]);
                }

                check.SelectedOfficeNames = string.Join(", ", selectedOfficeNames);
                check.TaskNames = string.Join(", ", selectedTaskNames);
            }

            return View(checks);
        }

        // GET: AuditChecks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditCheck check = db.AuditChecks.Find(id);
            if (check == null)
            {
                return HttpNotFound();
            }
            return View(check);
        }

        // GET: AuditChecks/Create
        public ActionResult Create()
        {
            ViewBag.TaskIds = new SelectList(db.AuditTasks, "Id", "Name");
            ViewBag.OfficeId = new SelectList(db.CRNs.Where(x => x.IsVisibleInMainApplication == true), "CRNID", "CRNDISPLAYNAME");
            ViewBag.Offices = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).ToList();
            

            return View();
        }

        // POST: AuditChecks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,TaskId,OfficeId,OrderNumberSeries,AuditCheckValues,TaskIds,SelectedOffices")] AuditCheck check)
        {
            ViewBag.TaskId = new SelectList(db.AuditTasks, "Id", "Name");
            ViewBag.OfficeId = new SelectList(db.CRNs.Where(x => x.IsVisibleInMainApplication == true), "CRNID", "CRNDISPLAYNAME");

            if (ModelState.IsValid)
            {
                db.AuditChecks.Add(check);
                db.SaveChanges();

                if (!string.IsNullOrEmpty(check.SelectedOffices))
                {
                    foreach (var checkOfficeId in check.SelectedOffices.Split(',').ToList())
                    {
                        db.AuditCheckCrns.Add(new AuditCheckCrn() { AuditCheckId = check.Id, CrnId = Convert.ToInt32(checkOfficeId) });
                    }
                }
                if (check.TaskIds != null)
                {
                    foreach (var taskId in check.TaskIds)
                    {
                        db.AuditCheckTasks.Add(new AuditCheckTask() { AuditCheckId = check.Id, TaskId = taskId });
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(check);
        }

        // GET: AuditChecks/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditCheck check = db.AuditChecks.Find(id);
            if (check == null)
            {
                return HttpNotFound();
            }
            var taskSelectList = new SelectList(db.AuditTasks, "Id", "Name", check.TaskId);
            ViewBag.SelectedTaskIds = db.AuditCheckTasks.Where(x=> x.AuditCheckId == id).Select(x=>x.TaskId.ToString()).ToList();

            ViewBag.TaskId = taskSelectList;


            ViewBag.OfficeId = new SelectList(db.CRNs.Where(x => x.IsVisibleInMainApplication == true), "CRNID", "CRNDISPLAYNAME", check.OfficeId);
            ViewBag.SelectedOffices = db.AuditCheckCrns.Where(x => x.AuditCheckId == id).Select(x => x.CrnId.ToString()).ToList();
            ViewBag.Offices = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).ToList();

            return View(check);
        }

        // POST: AuditChecks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,TaskId,OfficeId,OrderNumberSeries,AuditCheckValues,TaskIds,SelectedOffices")] AuditCheck check)
        {
            ViewBag.TaskId = new SelectList(db.AuditTasks, "Id", "Name", check.TaskId);
            ViewBag.OfficeId = new SelectList(db.CRNs.Where(x => x.IsVisibleInMainApplication == true), "CRNID", "CRNDISPLAYNAME", check.OfficeId);

            if (ModelState.IsValid)
            {

                db.Entry(check).State = EntityState.Modified;
                db.SaveChanges();

                var itemsToDeleteCheckTask = new List<AuditCheckTask>();
                foreach (var item in db.AuditCheckTasks.Where(x => x.AuditCheckId == check.Id))
                {
                    itemsToDeleteCheckTask.Add(item);
                }
                foreach (var item in itemsToDeleteCheckTask)
                {
                    db.AuditCheckTasks.Remove(item);
                }

                var itemsToDeleteCheckCrn = new List<AuditCheckCrn>();
                foreach (var item in db.AuditCheckCrns.Where(x => x.AuditCheckId == check.Id))
                {
                    itemsToDeleteCheckCrn.Add(item);
                }
                foreach (var item in itemsToDeleteCheckCrn)
                {
                    db.AuditCheckCrns.Remove(item);
                }

                db.SaveChanges();

                if (!string.IsNullOrEmpty(check.SelectedOffices))
                {
                    foreach (var checkOfficeId in check.SelectedOffices.Split(',').ToList())
                    {
                        db.AuditCheckCrns.Add(new AuditCheckCrn() { AuditCheckId = check.Id, CrnId = Convert.ToInt32(checkOfficeId) });
                    }
                }

                if (check.TaskIds != null)
                {
                    foreach (var taskId in check.TaskIds)
                    {
                        db.AuditCheckTasks.Add(new AuditCheckTask() { AuditCheckId = check.Id, TaskId = taskId });
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(check);
        }

        // GET: AuditChecks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditCheck check = db.AuditChecks.Find(id);
            if (check == null)
            {
                return HttpNotFound();
            }
            return View(check);
        }

        // POST: AuditChecks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AuditCheck check = db.AuditChecks.Find(id);

            var itemsToDeleteCheckTask = new List<AuditCheckTask>();
            foreach (var item in db.AuditCheckTasks.Where(x => x.AuditCheckId == check.Id))
            {
                itemsToDeleteCheckTask.Add(item);
            }
            foreach (var item in itemsToDeleteCheckTask)
            {
                db.AuditCheckTasks.Remove(item);
            }

            var itemsToDeleteCheckCrn = new List<AuditCheckCrn>();
            foreach (var item in db.AuditCheckCrns.Where(x => x.AuditCheckId == check.Id))
            {
                itemsToDeleteCheckCrn.Add(item);
            }
            foreach (var item in itemsToDeleteCheckCrn)
            {
                db.AuditCheckCrns.Remove(item);
            }

            db.SaveChanges();

            db.AuditChecks.Remove(check);
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
