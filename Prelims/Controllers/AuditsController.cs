using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Prelims;
using Prelims.Models;
using System.Data.Entity.Validation;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Prelims.Models.Reports;
using Prelims.Helpers;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Configuration;
using System.Text;
using System.Data.OleDb;
using System.ComponentModel.Design;
using System.Drawing;

namespace Prelims.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditsController : Controller
    {
        private FntEntities db = new FntEntities();

        private const string regionCode = "Audits";
        private const string AuditUiFilter = "UIFilter";
        // GET: Audits
        public ActionResult Index(int? crnId, int? taskId, int? requestTypeId, string groupName)
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            FilterService filterService = new FilterService(db, regionCode, userInfo.USERID);

            if (!crnId.HasValue && !taskId.HasValue && !requestTypeId.HasValue)
            {
                //Try loading it from filter if any.
                var uiFilter = filterService.GetFilter(AuditUiFilter);

                if (uiFilter != null)
                {
                    AuditUiFilter AuditFilter = JsonConvert.DeserializeObject<AuditUiFilter>(uiFilter.FilterJson);

                    if (AuditFilter != null)
                    {
                        crnId = AuditFilter.CrnId;
                        taskId = AuditFilter.TaskId;
                        requestTypeId = AuditFilter.RequestTypeId;
                    }
                }
            }
            else
            {
                AuditUiFilter AuditFilterToSave = new AuditUiFilter() { CrnId = crnId, TaskId = taskId, RequestTypeId = requestTypeId };
                filterService.SaveFilter(AuditUiFilter, JsonConvert.SerializeObject(AuditFilterToSave));
            }

            var Audits = (from k in db.Audits.Include(t => t.AuditStatus)
                .Include(t => t.AuditTask)
                .Include(t => t.AuditRequestType)
                .Include(t => t.AuditSender)
                             where k.StatusId != 3 && k.StatusId != 4 && k.StatusId != 5 && !k.AuditTimeEntries.Any(x => x.IsInprogress) && k.UserAssigned == null
                              select k).AsQueryable();

           

            if (!taskId.HasValue)
            {
                Audits = Audits.Where(x => x.TaskId == 1);
            }else
            {
                Audits = Audits.Where(x => x.TaskId == taskId.Value);
            }

            if (requestTypeId.HasValue)
            {
                Audits = Audits.Where(x => x.RequestTypeId == requestTypeId.Value);
            }
            List<int> allowedCrnIdsForThisUser = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).Select(x => x.CRNID).ToList();
            if (!string.IsNullOrEmpty(groupName))
            {
                allowedCrnIdsForThisUser = (from crn in db.CRNs
                                            where crn.GroupName == groupName
                                            select crn.CRNID).ToList();
            }

            if (crnId.HasValue)
            {
                Audits = Audits.Where(x => x.CrnId == crnId.Value);
            }else
            {
                Audits = Audits.Where(x => allowedCrnIdsForThisUser.Contains(x.CrnId));
            }

            AuditTimeEntry timeEntry = db.AuditTimeEntries.FirstOrDefault(x => x.IsInprogress && x.UserId == userInfo.USERID);

            if(timeEntry != null)
            {
                ViewBag.InprogressAuditId = timeEntry.AuditId;
            }

            ViewBag.TaskId = new SelectList(db.AuditTasks, "Id", "Name", taskId ?? 1);
            ViewBag.RequestTypeId = new SelectList(db.AuditRequestTypes, "Id", "Name", requestTypeId ?? 0);
            ViewBag.CrnId = new SelectList(db.CRNs.Where(x => x.CRNID > 20), "CRNID", "CRNNAME", crnId);
            ViewBag.OfficeGroups = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).Select(x => x.GroupName).Distinct().ToList();
            ViewBag.Crns = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).Select(x => new { x.CRNID, x.CRNNAME, x.GroupName }).ToList();
            ViewBag.GroupName = groupName;
            return View(Audits.OrderByDescending(x => x.IsRushOrder).ToList());
        }

        public ActionResult SearchOrder(string orderNo)
        {
            if (string.IsNullOrEmpty(orderNo))
            {
                return View(new List<Audit>());
            }

            var policyKeys = (from k in db.Audits.Include(t => t.AuditStatus)
               .Include(t => t.AuditTask)
               .Include(t => t.AuditRequestType)
               .Include(t => t.AuditSender)
                              where k.OrderNo.Contains(orderNo)
                              select k).AsQueryable();

            return View(policyKeys);
        }

        public JsonResult CheckOrderStatus(string orderNo)
        {
            DateTime lastTwentyFourHour = DateTime.Now.AddHours(-24);
            var orderInfo = db.Audits.Any(x => x.DateCreated > lastTwentyFourHour && x.OrderNo == orderNo);

            return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = orderInfo };
        }

        // GET: Audits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Audit Audit = db.Audits.Find(id);
            if (Audit == null)
            {
                return HttpNotFound();
            }

            Audit.Errors = GetErrors(Audit.Id);

            return View(Audit);
        }

        // GET: Audits/Create
        public ActionResult Create()
        {
            ViewBag.StatusId = new SelectList(db.AuditStatuses, "Id", "Name");
            ViewBag.TaskId = new SelectList(db.AuditTasks, "Id", "Name");
            ViewBag.RequestTypeId = new SelectList(db.AuditRequestTypes, "Id", "Name");
            ViewBag.Senderid = new SelectList(db.AuditSenders, "Id", "Name");
            ViewBag.CrnId= new SelectList(db.CRNs.Where(x => x.IsVisibleInMainApplication == true), "CRNID", "CRNNAME");
            ViewBag.LastOrder = db.Audits.OrderByDescending(x => x.Id).FirstOrDefault();
            ViewBag.Counties = db.STATECOUNTies.Select(x => x.COUNTYNAME).ToList();

            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View();
        }

        // POST: Audits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OrderNo,OwnerNames,PropertyAddress,APNNo,RequestTypeId,StatusId,TaskId,CrnId,SenderId,RequestedByEmailId,ClientInstructions,DateCreated,UserCreated,DateModified,UserModified,County,IsRushOrder")] Audit Audit)
        {
            if (ModelState.IsValid)
            {
                DateTime dayBefore = DateTime.Now.AddDays(-1);
                var anyOrderExists = db.Audits.Any(x => x.OrderNo == Audit.OrderNo && x.DateCreated > dayBefore);

                //if (!anyOrderExists)
                //{
                Audit.StatusId = 1;
                Audit.TaskId = 1;
                //Audit.DateCreated = DateTime.Now;
                db.Audits.Add(Audit);
                db.SaveChanges();
                TempData.Add("SuccessMessage", "Order Created successfully.");
                return RedirectToAction("Create");
                //}else
                //{
                //    ModelState.AddModelError(string.Empty, "Order with same order no already exists for day");
                //}
            }

            ViewBag.StatusId = new SelectList(db.AuditStatuses, "Id", "Name", Audit.StatusId);
            ViewBag.TaskId = new SelectList(db.AuditTasks, "Id", "Name", Audit.TaskId);
            ViewBag.RequestTypeId = new SelectList(db.AuditRequestTypes, "Id", "Name", Audit.RequestTypeId);
            ViewBag.Senderid = new SelectList(db.AuditSenders, "Id", "Name");
            ViewBag.CrnId = new SelectList(db.CRNs.Where(x=> x.CRNID > 20), "CRNID", "CRNNAME");
            ViewBag.Counties = db.STATECOUNTies.Select(x => x.COUNTYNAME).ToList();
            ViewBag.LastOrder = db.Audits.OrderByDescending(x => x.Id).FirstOrDefault();
            return View(Audit);
        }

        // GET: Audits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Audit Audit = db.Audits.Find(id);
            if (Audit == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusId = new SelectList(db.AuditStatuses, "Id", "Name", Audit.StatusId);
            ViewBag.TaskId = new SelectList(db.AuditTasks, "Id", "Name", Audit.TaskId);
            ViewBag.RequestTypeId = new SelectList(db.AuditRequestTypes, "Id", "Name", Audit.RequestTypeId);
            ViewBag.Senderid = new SelectList(db.AuditSenders, "Id", "Name", Audit.SenderId);
            return View(Audit);
        }

        // POST: Audits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OrderNo,OwnerNames,PropertyAddress,APNNo,RequestTypeId,StatusId,SenderId,TaskId,RequestedByEmailId,ClientInstructions")] Audit Audit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Audit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StatusId = new SelectList(db.AuditStatuses, "Id", "Name", Audit.StatusId);
            ViewBag.TaskId = new SelectList(db.AuditTasks, "Id", "Name", Audit.TaskId);
            ViewBag.RequestTypeId = new SelectList(db.AuditRequestTypes, "Id", "Name", Audit.RequestTypeId);
            ViewBag.Senderid = new SelectList(db.AuditSenders, "Id", "Name", Audit.SenderId);
            return View(Audit);
        }

        // GET: Audits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Audit Audit = db.Audits.Find(id);
            if (Audit == null)
            {
                return HttpNotFound();
            }
            return View(Audit);
        }

        // POST: Audits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Audit Audit = db.Audits.Find(id);
            db.Audits.Remove(Audit);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Production(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var AuditProductionModel = new AuditProductionModel();
            USERINFO userInfo = (USERINFO)Session["UserInfo"];

            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Audit Audit = db.Audits.Find(id);
            if (Audit == null)
            {
                return HttpNotFound();
            }
            else if (Audit != null && Audit.StatusId == 2)
            {
                AuditTimeEntry timeEntry = db.AuditTimeEntries.FirstOrDefault(x => x.AuditId == Audit.Id && x.IsInprogress);

                if (timeEntry != null)
                {
                    if (timeEntry.UserId != userInfo.USERID)
                    {
                        return View("AuditAlreadyInprogress");
                    }

                    AuditProductionModel.TimeEntry = timeEntry;
                    AuditProductionModel.TaxTimeEntryId = timeEntry.Id;
                }
                else
                {
                    // 1- Peding|2-Inprogress|3-Hold|4-Completed

                    AuditTimeEntry newTimeEntry = new AuditTimeEntry();
                    newTimeEntry.AuditId = Audit.Id;
                    newTimeEntry.AuditTaskId = Audit.TaskId;
                    newTimeEntry.UserId = userInfo.USERID;
                    newTimeEntry.StartTime = DateTime.Now;
                    newTimeEntry.IsInprogress = true;

                    db.AuditTimeEntries.Add(newTimeEntry);

                    db.SaveChanges();

                    AuditProductionModel.TaxTimeEntryId = newTimeEntry.Id;
                }
            }
            else if (Audit.StatusId == 1)
            {
                Audit.StatusId = 2;  // 1- Peding|2-Inprogress|3-Hold|4-Completed

                AuditTimeEntry timeEntry = new AuditTimeEntry();
                timeEntry.AuditId = Audit.Id;
                timeEntry.AuditTaskId = Audit.TaskId;
                timeEntry.UserId = userInfo.USERID;
                timeEntry.StartTime = DateTime.Now;
                timeEntry.IsInprogress = true;

                db.AuditTimeEntries.Add(timeEntry);

                db.SaveChanges();

                AuditProductionModel.TaxTimeEntryId = timeEntry.Id;
            }
            else if (Audit.StatusId == 4 || Audit.StatusId == 3)
            {
                return View("TitleOrderAlreadyCompleted");
            }

            AuditProductionModel.Checks = GetAuditChecks(Audit.OrderNo, Audit.TaskId, Audit.CrnId);
            var dictionaryOfCheckList = new Dictionary<string, string>();

            foreach (var check in db.AuditCheckValues.Where(x => x.TitleOrderId == Audit.Id))
            {
                dictionaryOfCheckList.Add(check.AuditCheckId.ToString(), check.AuditCheckValue1);
            }

            AuditProductionModel.CheckText = JsonConvert.SerializeObject(dictionaryOfCheckList);

            ViewBag.StatusId = new SelectList(db.AuditStatuses, "Id", "Name", Audit.StatusId);
            ViewBag.TaskId = new SelectList(db.AuditTasks, "Id", "Name", Audit.TaskId);
            ViewBag.RequestTypeId = new SelectList(db.AuditRequestTypes, "Id", "Name", Audit.RequestTypeId);
            ViewBag.Senderid = new SelectList(db.AuditSenders, "Id", "Name", Audit.SenderId);

            AuditProductionModel.TimeEntries = db.AuditTimeEntries.Where(x => x.AuditId == Audit.Id).Include(x => x.AuditTask).Include(x => x.USERINFO).ToList();
            AuditProductionModel.AuditUpdates = db.AuditUpdates.Where(x => x.AuditId == Audit.Id).Include(x => x.USERINFO).ToList();

            AuditProductionModel.Audit = Audit;
            AuditProductionModel.OwnerName = Audit.OwnerNames;
            AuditProductionModel.AuditId = Audit.Id;
            AuditProductionModel.IsDirectHitUploaded = Audit.CheckDirectHitUploaded ?? false;
            AuditProductionModel.AnyChangeInVesting = Audit.AnyChangeInVesting;
            AuditProductionModel.AnyUpdateOnNewGIDocs = Audit.AnyUpdateOnNewGIDocs;
            AuditProductionModel.DidYouReviewTwentyFourMonthChainOfTitle = Audit.DidYouReviewTwentyFourMonthChainOfTitle;
            AuditProductionModel.AnyPostingFoundInPIGI = Audit.AnyPostingFoundInPIGI;

            AuditProductionModel.DidYouUploadPrelimAndSPToSmartView = Audit.DidYouUploadPrelimAndSPToSmartView;
            AuditProductionModel.DidYouSendCompletionEmailToClient = Audit.DidYouSendCompletionEmailToClient;

            AuditProductionModel.AnyUpdateOnNewPIDocs = Audit.AnyUpdateOnNewPIDocs;
            AuditProductionModel.AnyUpdateOnTaxInformation = Audit.AnyUpdateOnTaxInformation;
            AuditProductionModel.EffectiveDateChanged = Audit.EffectiveDateChanged;

            AuditProductionModel.CWLTTitleOfficeName = Audit.CWLTTitleOfficeName;
            AuditProductionModel.CWLTTitleOfficerName = Audit.CWLTTitleOfficerName;

            AuditProductionModel.OwnerSearchCount = Audit.CheckOwnerSearchCount ?? 0;
            AuditProductionModel.OrderErrorJson = "[{ selectedCategory: null, selectedType: [], errorTypes: [], comments: '', isCritical: false }]";

            AuditProductionModel.AmendmentDate = Audit.AmendmentDate;

            ViewBag.CWLTTitleOfficers = db.CWTTitleOfficerNames
                                    .Select(to => new SelectListItem
                                    {
                                        Value = to.Id.ToString(),
                                        Text = to.Name,
                                        Selected = (to.Name == Audit.CWLTTitleOfficerName)
                                    })
                                    .ToList();

            ViewBag.CWLTTitleOffices = db.CWTRequestedOffices
                .Select(o => new SelectListItem
                {
                    Value = o.Id.ToString(),
                    Text = o.Name,
                    Selected = (o.Name == Audit.CWLTTitleOfficeName)
                })
                .ToList();

            return View(AuditProductionModel);
        }

        [HttpPost]
        public ActionResult Production(AuditProductionModel AuditProductionModel, HttpPostedFileBase[] files)
        {
            if (AuditProductionModel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            USERINFO userInfo = (USERINFO)Session["UserInfo"];

            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Audit Audit = db.Audits.Find(AuditProductionModel.AuditId);
            AuditTimeEntry timeEntry = db.AuditTimeEntries.Find(AuditProductionModel.TaxTimeEntryId);

            if (timeEntry.EndTime.HasValue)
            {
                return View("TitleOrderTaskIsDone");
            }

            if (!string.IsNullOrEmpty(AuditProductionModel.CheckText))
            {
                var checksData = JsonConvert.DeserializeObject<Dictionary<string, string>>(AuditProductionModel.CheckText);

                foreach (var checkItem in checksData)
                {
                    int checkId = Convert.ToInt32(checkItem.Key);
                    var checkDbItem = db.AuditCheckValues.FirstOrDefault(x => x.TitleOrderId == Audit.Id && x.AuditCheckId == checkId);

                    if (checkDbItem != null)
                    {
                        checkDbItem.AuditCheckValue1 = checkItem.Value;
                    }
                    else
                    {
                        db.AuditCheckValues.Add(new AuditCheckValue()
                        {
                            AuditCheckId = Convert.ToInt32(checkItem.Key),
                            AuditCheckValue1 = checkItem.Value,
                            TitleOrderId = Audit.Id
                        });
                    }

                }
            }

            int previousTaskId = Audit.TaskId;
            if (Audit != null && timeEntry != null)
            {
                var lastStatusId = Audit.TaskId;
                if (Audit.TaskId == 1)
                {
                    // Task 1: L&V -> Move to Task 2: PI
                    Audit.TaskId = 2;
                    SaveAuditFiles(Audit.Id, this.Request.Files);
                }
                else if (Audit.TaskId == 2)
                {
                    // Task 2: PI -> Move to Task 3: GI
                    Audit.TaskId = 3;
                    SaveAuditFiles(Audit.Id, this.Request.Files);
                }
                else if (Audit.TaskId == 3)
                {
                    // Task 3: GI -> Move to Task 4: Starter
                    Audit.TaskId = 4;
                    SaveAuditFiles(Audit.Id, this.Request.Files);
                }
                else if (Audit.TaskId == 4)
                {
                    // Task 4: Starter -> Move to Task 5: Notes
                    Audit.TaskId = 5;
                    SaveAuditFiles(Audit.Id, this.Request.Files);
                }
                else if (Audit.TaskId == 5)
                {
                    // Task 5: Notes -> After the 5th task, order should be completed
                    Audit.StatusId = 4;
                    Audit.UploadDateTime = DateTime.Now;

                    SaveAuditFiles(Audit.Id, this.Request.Files);

                    // Move corresponding TitleOrder to QC task (TaskId=8) after Audit completion
                    // and populate denormalized audit fields for production reports
                    try
                    {
                        var auditComments = string.IsNullOrEmpty(AuditProductionModel.Updates) ? "--" : AuditProductionModel.Updates;

                        if (Audit.TitleOrderId.HasValue)
                        {
                            // Use TitleOrderId (FK) for reliable matching
                            db.Database.ExecuteSqlCommand(
                                @"UPDATE TitleOrders 
                                  SET TaskId = 6, StatusId = 1, UserAssigned = NULL,
                                      AuditStartTime = @p1, AuditEndTime = @p2,
                                      AuditDoneBy = @p3, AuditDoneByUserId = @p4,
                                      AuditComments = @p5, DateModified = @p6
                                  WHERE Id = @p0 AND StatusId = 6",
                                Audit.TitleOrderId.Value,
                                timeEntry.StartTime,
                                (object)(timeEntry.EndTime ?? DateTime.Now),
                                userInfo.USERNAME,
                                userInfo.USERID,
                                auditComments,
                                DateTime.Now
                            );
                        }
                        else
                        {
                            // Fallback for older Audit records without TitleOrderId — match by OrderNo
                            db.Database.ExecuteSqlCommand(
                                @"UPDATE TitleOrders 
                                  SET TaskId = 6, StatusId = 1, UserAssigned = NULL,
                                      AuditStartTime = @p1, AuditEndTime = @p2,
                                      AuditDoneBy = @p3, AuditDoneByUserId = @p4,
                                      AuditComments = @p5, DateModified = @p6
                                  WHERE OrderNo = @p0 AND StatusId = 6",
                                Audit.OrderNo,
                                timeEntry.StartTime,
                                (object)(timeEntry.EndTime ?? DateTime.Now),
                                userInfo.USERNAME,
                                userInfo.USERID,
                                auditComments,
                                DateTime.Now
                            );
                        }
                    }
                    catch (Exception)
                    {
                        // Ignore if TitleOrders table does not exist or order is already unlocked
                    }

                    var emailMessage = new StringBuilder();
                    emailMessage.AppendLine("Hi " + (Audit.AuditSender != null ? Audit.AuditSender.Name : "Team"));
                    emailMessage.AppendLine("");
                    if (Audit.CheckDirectHitUploaded ?? false)
                    {
                        if (Audit.AuditSender != null && Audit.AuditSender.AttachmentRequired)
                        {
                            emailMessage.AppendLine("The below SI request has been cleared, We found direct hit JG/LN.");
                            emailMessage.AppendLine("");
                            emailMessage.AppendLine("Attached are the documents for your reference.");
                        }
                        else
                        {
                            emailMessage.AppendLine("The below SI request has been cleared, We found direct hit document uploaded in the smart view.");
                        }
                    }
                    else
                    {
                        emailMessage.AppendLine("The below SI request has been cleared, there is no direct hit found.");
                    }
                    emailMessage.AppendLine("");
                    emailMessage.AppendLine("Regards,");
                    emailMessage.AppendLine("SI Clearance Team");

                    var smtpFromConfig = ConfigurationManager.AppSettings.Get("SMTP-FROMADDRESS");
                    string fromAddresss = !string.IsNullOrEmpty(smtpFromConfig) ? smtpFromConfig : string.Empty;

                    var smtpToConfig = ConfigurationManager.AppSettings.Get("SMTP-TOADDRESS");
                    string toAddresss = !string.IsNullOrEmpty(smtpToConfig) ? smtpToConfig : "ashwathjyothinagar@gmail.com";

                    List<AuditDocument> documents = new List<AuditDocument>();
                    if (Audit.AuditSender != null && Audit.AuditSender.AttachmentRequired)
                    {
                        documents = db.AuditDocuments.Where(x => x.AuditId == Audit.Id).ToList();
                    }

                    foreach (AuditDocument AuditDoc in documents)
                    {
                        if (System.IO.File.Exists(AuditDoc.DocumentPath))
                        {
                            System.IO.File.Delete(AuditDoc.DocumentPath);
                        }
                    }

                    //sendMail(toAddresss, fromAddresss, Audit.OrderNo, emailMessage.ToString(), documents);
                }

                timeEntry.IsInprogress = false;
                timeEntry.EndTime = DateTime.Now;

                
                AuditUpdate AuditUpdate = new AuditUpdate();
                AuditUpdate.AuditId = Audit.Id;
                AuditUpdate.UserId = userInfo.USERID;
                AuditUpdate.Updates = string.IsNullOrEmpty(AuditProductionModel.Updates) ? "--" : AuditProductionModel.Updates;
                AuditUpdate.AuditTaskId = lastStatusId;

                db.AuditUpdates.Add(AuditUpdate);

                Audit.CheckDirectHitUploaded = AuditProductionModel.IsDirectHitUploaded;
                Audit.UserAssigned = null;
                Audit.AnyChangeInVesting = AuditProductionModel.AnyChangeInVesting;
                Audit.AnyUpdateOnNewGIDocs = AuditProductionModel.AnyUpdateOnNewGIDocs;
                Audit.DidYouReviewTwentyFourMonthChainOfTitle = AuditProductionModel.DidYouReviewTwentyFourMonthChainOfTitle;
                Audit.AnyPostingFoundInPIGI = AuditProductionModel.AnyPostingFoundInPIGI;

                Audit.DidYouUploadPrelimAndSPToSmartView = AuditProductionModel.DidYouUploadPrelimAndSPToSmartView;
                Audit.DidYouSendCompletionEmailToClient = AuditProductionModel.DidYouSendCompletionEmailToClient;

                Audit.AnyUpdateOnNewPIDocs = AuditProductionModel.AnyUpdateOnNewPIDocs;
                Audit.AnyUpdateOnTaxInformation = AuditProductionModel.AnyUpdateOnTaxInformation;
                Audit.EffectiveDateChanged = AuditProductionModel.EffectiveDateChanged;

                Audit.CheckOwnerSearchCount = AuditProductionModel.OwnerSearchCount;
                Audit.OwnerNames = AuditProductionModel.OwnerName;

                Audit.CWLTTitleOfficeName = AuditProductionModel.CWLTTitleOfficeName;
                Audit.CWLTTitleOfficerName = AuditProductionModel.CWLTTitleOfficerName;

                Audit.AmendmentDate = AuditProductionModel.AmendmentDate;

                if (!string.IsNullOrEmpty(AuditProductionModel.OrderErrorJson) && AuditProductionModel.AnyOrderErrors == "YES")
                {
                    var orderErrorJson = db.AuditErrorJsons.FirstOrDefault(x => x.TaskId == previousTaskId && x.AuditId == Audit.Id);

                    if (orderErrorJson != null)
                    {
                        orderErrorJson.OrderErrorJson = AuditProductionModel.OrderErrorJson;
                    }
                    else
                    {
                        db.AuditErrorJsons.Add(new AuditErrorJson() { AuditId = Audit.Id, TaskId = previousTaskId, OrderErrorJson = AuditProductionModel.OrderErrorJson });
                    }

                    sendErrorEmail(Audit.Id, previousTaskId, AuditProductionModel.OrderErrorJson, timeEntry.Id);
                }

                var isFailded = false;
                try { 
                db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    List<string> validationErrorList = new List<string>();
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            validationErrorList.Add(string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage));
                        }
                    }
                    isFailded = true;
                    ViewBag.ValidationErrors = string.Join(",", validationErrorList);
                
                    
                }

                if (isFailded)
                {
                    AuditProductionModel.Audit = Audit;
                    return View(AuditProductionModel);
                }

                
            }

            return RedirectToAction("Index", "Audits");
        }

        private void SaveAuditFiles(int auditId, HttpFileCollectionBase files)
        {
            if (files != null && files.Count > 0)
            {
                if (!string.IsNullOrEmpty(files[0].FileName))
                {
                    var uploadFilePath = ConfigurationManager.AppSettings.Get("UPLOAD-FILEPATH");
                    if (string.IsNullOrEmpty(uploadFilePath)) return;

                    if (!Directory.Exists(uploadFilePath))
                    {
                        Directory.CreateDirectory(uploadFilePath);
                    }

                    int fileInx = 0;
                    foreach (string requestFileName in files)
                    {
                        HttpPostedFileBase file = files[fileInx];
                        if (file != null && !string.IsNullOrEmpty(file.FileName) && file.ContentLength > 0)
                        {
                            string fileNameGuid = Guid.NewGuid().ToString();
                            string fileName = Path.Combine(uploadFilePath, fileNameGuid + Path.GetExtension(file.FileName));
                            file.SaveAs(fileName);

                            AuditDocument auditDocument = new AuditDocument();
                            auditDocument.FileName = file.FileName;
                            auditDocument.DocumentPath = fileName;
                            auditDocument.AuditId = auditId;

                            db.AuditDocuments.Add(auditDocument);
                        }
                        fileInx++;
                    }
                    db.SaveChanges();
                }
            }
        }

        public ActionResult TitleOrderTaskIsDone()
        {
            return View();
        }

        public ActionResult TitleOrderAlreadyCompleted()
        {
            return View();
        }

        public FileResult downloadDocument(int AuditId) {

            var AuditDocument = db.AuditDocuments.FirstOrDefault(x => x.AuditId == AuditId);

            return File(AuditDocument.DocumentPath, "application/pdf", Server.UrlEncode(AuditDocument.FileName));
        }

        public ActionResult Hold(int orderId)
        {
            Audit txOrder = db.Audits.Find(orderId);

            return View(txOrder);
        }

        [HttpPost]
        public ActionResult Hold(int id, string holdReason)
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];

            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Audit orderInfo = db.Audits.Find(id);

            AuditHold AuditHold = new AuditHold();
            AuditHold.AuditId = orderInfo.Id;
            AuditHold.AuditTaskId = orderInfo.TaskId;
            AuditHold.HoldBy = userInfo.USERID;
            AuditHold.HoldDateTime = DateTime.Now;
            AuditHold.Comment = holdReason;

            db.AuditHolds.Add(AuditHold);

            AuditTimeEntry timeEntry = db.AuditTimeEntries.FirstOrDefault(x => x.IsInprogress && x.UserId == userInfo.USERID && x.AuditId == id);

            db.AuditTimeEntries.Remove(timeEntry);

            orderInfo.StatusId = 3;

            string holdEmailTo = !string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("HOLDEMAIL-TO")) ? ConfigurationManager.AppSettings.Get("HOLDEMAIL-TO") : "prelims@firsttitlebpo.com";
            string holdEmailFrom = !string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("HOLDEMAIL-FROM")) ? ConfigurationManager.AppSettings.Get("HOLDEMAIL-FROM") : "notifications@ftbpo.com";

            if (!string.IsNullOrEmpty(holdEmailTo) && !string.IsNullOrEmpty(holdEmailFrom))
            {
                string emailContent = string.Format("Hi,\n\nThe subject order has been put on hold for \"{0}\".\n\nPlease verify and advice.\n\nRegards,\nUser:{1}", holdReason, userInfo.FIRSTNAME + " " + userInfo.LASTNAME);
                string subject = string.Format("FNT Update Hold  Order No: {0}, Office: {1}, Task: {2}", orderInfo.OrderNo, orderInfo.CRN.CRNNAME, orderInfo.AuditTask.Name);

                sendMail(holdEmailTo, holdEmailFrom, subject, emailContent);
            }

            db.SaveChanges();

            return RedirectToAction("Index", "Audits");
        }

        public ActionResult UnHold(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Audit Audit = db.Audits.Find(id);
            if (Audit == null)
            {
                return HttpNotFound();
            }
            return View(Audit);
        }

        [HttpPost, ActionName("UnHold")]
        [ValidateAntiForgeryToken]
        public ActionResult UnHoldConfirmed(int id)
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo != null)
            {
                Audit orderInfo = db.Audits.Find(id);

                AuditHold AuditHold = db.AuditHolds.Where(x => x.AuditId == id).OrderByDescending(x => x.HoldDateTime).FirstOrDefault();

                AuditHold.HoldRelievedBy = userInfo.USERID;
                AuditHold.HoldRelivedDateTime = DateTime.Now;

                orderInfo.StatusId = 2;

                db.SaveChanges();

            }

            return RedirectToAction("TaskWisePending");
        }


        public ActionResult Reject(int orderId)
        {
            Audit txOrder = db.Audits.Find(orderId);

            return View(txOrder);
        }

        [HttpPost]
        public ActionResult Reject(int id, string rejectReason)
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];

            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Audit orderInfo = db.Audits.Find(id);

            AuditReject AuditReject = new AuditReject();
            AuditReject.AuditId = orderInfo.Id;
            AuditReject.RejectedBy = userInfo.USERID;
            AuditReject.RejectedDateTime = DateTime.Now;
            AuditReject.Reason = rejectReason;

            db.AuditRejects.Add(AuditReject);

            AuditTimeEntry timeEntry = db.AuditTimeEntries.FirstOrDefault(x => x.IsInprogress && x.UserId == userInfo.USERID && x.AuditId == id);

            db.AuditTimeEntries.Remove(timeEntry);

            orderInfo.StatusId = 5;

            db.SaveChanges();

            return RedirectToAction("Index", "Audits");
        }

        public ActionResult AuditAlreadyInprogress()
        {
            return View();
        }

        public ActionResult EOD()
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];

            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            SetOfficeFields(userInfo);

            return View();
        }

        private void SetOfficeFields(USERINFO userInfo)
        {
            ViewBag.CrnId = new SelectList(db.CRNs.Where(x => x.IsVisibleInMainApplication == true), "CRNID", "CRNDISPLAYNAME", 0);
            var offices = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).ToList();
            ViewBag.Crns = offices.Select(x => new { x.CRNID, x.CRNNAME, x.GroupName }).ToList();
            ViewBag.OfficeGroups = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).Select(x => x.GroupName).Distinct().ToList();
            ViewBag.GroupName = "";
        }



        public void ExportEOD(DateTime startDateTime, DateTime endDateTime, string groupName, int? crnId)
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];

            IQueryable<Audit> Audits;

            List<int> crnIds = new List<int>();

            if (crnId > 0)
            {
                crnIds.Add(crnId.Value);
            }
            else if (!string.IsNullOrEmpty(groupName))
            {
                crnIds = (from crn in db.CRNs
                          where crn.GroupName == groupName
                          select crn.CRNID).ToList();

            }

            if (crnIds.Count > 0)
            {
                Audits = db.Audits.Where(x => x.UploadDateTime > startDateTime && x.UploadDateTime < endDateTime && crnIds.Contains(x.CrnId)).Include(x => x.AuditRequestType).Include(x => x.AuditStatus).Include(x => x.AuditTask).Include(x => x.AuditTimeEntries).Include(x => x.AuditSender).Include(x => x.CRN).Include(x => x.AuditUpdates);

            }
            else
            {
                Audits = db.Audits.Where(x => x.UploadDateTime > startDateTime && x.UploadDateTime < endDateTime).Include(x => x.AuditRequestType).Include(x => x.AuditStatus).Include(x => x.AuditTask).Include(x => x.AuditTimeEntries).Include(x => x.AuditSender).Include(x => x.CRN).Include(x => x.AuditUpdates);
            }

            List<EodReportViewModel> listItem = new List<EodReportViewModel>();
            int serialNumer = 1;
            foreach (var Audit in Audits)
            {
                EodReportViewModel item = new EodReportViewModel();

                item.SerialNumber = serialNumer;
                item.OrderNo = Audit.OrderNo;

                if (Audit.CRN != null)
                    item.OfficeName = Audit.CRN.CRNNAME;

                if (Audit.AuditRequestType != null)
                    item.ProductType = Audit.AuditRequestType.Name;
                if (Audit.AuditSender != null)
                    item.RequestedBy = Audit.AuditSender.Name;

                item.Instruction = Audit.ClientInstructions;
                item.APNNo = Audit.APNNo;

                item.County = Audit.County;
                if (Audit.DateCreated.HasValue)
                {
                    item.RecievedDateTime = Audit.DateCreated.Value.ToString("yyyy/MM/dd hh:mm tt");
                }

                if (Audit.UploadDateTime.HasValue)
                {
                    item.CompletedDateTime = Audit.UploadDateTime.Value.ToString("yyyy/MM/dd hh:mm tt");
                }

                item.EffectiveDateChanged = Audit.EffectiveDateChanged.HasValue ? (Audit.EffectiveDateChanged.Value ? "Yes" : "No") : "Not Set";
                item.AnyChangeInVesting = Audit.AnyChangeInVesting.HasValue ? (Audit.AnyChangeInVesting.Value ? "Yes" : "No") : "Not Set";
                item.AnyUpdateOnTaxInformation = Audit.AnyUpdateOnTaxInformation.HasValue ? (Audit.AnyUpdateOnTaxInformation.Value ? "Yes" : "No") : "Not Set";
                item.AnyUpdateOnNewPIDocs = Audit.AnyUpdateOnNewPIDocs.HasValue ? (Audit.AnyUpdateOnNewPIDocs.Value ? "Yes" : "No") : "Not Set";
                item.AnyUpdateOnNewGIDocs = Audit.AnyUpdateOnNewGIDocs.HasValue ? (Audit.AnyUpdateOnNewGIDocs.Value ? "Yes" : "No") : "Not Set";
                item.DidYouReviewTwentyFourMonthChainOfTitle = Audit.DidYouReviewTwentyFourMonthChainOfTitle.HasValue ? (Audit.DidYouReviewTwentyFourMonthChainOfTitle.Value ? "Yes" : "No") : "Not Set";

                item.CWLTTitleOfficeName = Audit.CWLTTitleOfficeName;
                item.CWLTTitleOfficerName = Audit.CWLTTitleOfficerName;

                var npcItegrationItem = Audit.AuditTimeEntries.FirstOrDefault(x => x.AuditTaskId == 3);

                if (npcItegrationItem != null)
                {
                    item.CompletedBy = npcItegrationItem.USERINFO.USERNAME;
                }

                if (Audit.AuditStatus != null)
                    item.Status = Audit.AuditStatus.Name;
                listItem.Add(item);
                serialNumer++;
            }

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Contact.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteHtmlTable<EodReportViewModel>(listItem, Response.Output);
            Response.End();
        }

        public JsonResult GetAllTasks()
        {
            return new JsonResult() { Data = db.AuditTasks.Select(x => new { Id = x.Id, Name = x.Name }).ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ProductionReport()
        {
            return View();
        }

        public void ExportProductionReport(int taskId, DateTime startDateTime, DateTime endDateTime)
        {
            List<ProductionReportViewModel> productionViewModels = new List<ProductionReportViewModel>();
            var Audits = db.Audits.AsQueryable();
                if(taskId == -1)
            {
                Audits = Audits.Where(x => x.AuditTimeEntries.Any(y => y.StartTime > startDateTime && y.EndTime < endDateTime && y.AuditTaskId == 1) ||
                                                 x.AuditTimeEntries.Any(y => y.StartTime > startDateTime && y.EndTime < endDateTime && y.AuditTaskId == 2) ||
                                                 x.AuditTimeEntries.Any(y => y.StartTime > startDateTime && y.EndTime < endDateTime && y.AuditTaskId == 3) ||
                                                 x.AuditTimeEntries.Any(y => y.StartTime > startDateTime && y.EndTime < endDateTime && y.AuditTaskId == 4) ||
                                                 x.AuditTimeEntries.Any(y => y.StartTime > startDateTime && y.EndTime < endDateTime && y.AuditTaskId == 5));
            }
            else
            {
                Audits = Audits.Where(x => x.AuditTimeEntries.Any(y => y.StartTime > startDateTime && y.EndTime < endDateTime && y.AuditTaskId == taskId));
            }

            Audits = Audits
            .Include(x => x.AuditRequestType)
            .Include(x => x.AuditStatus)
            .Include(x => x.AuditTask)
            .Include(x => x.AuditTimeEntries)
            .Include(x => x.AuditSender)
            .Include(x => x.CRN)
            .Include(x => x.AuditUpdates);

            int serialNumber = 1;
            foreach(var Audit in Audits)
            {
                ProductionReportViewModel productionViewModel = new ProductionReportViewModel();
                List<AuditError> errors = GetErrors(Audit.Id);
                productionViewModel.SerialNumber = serialNumber;
                productionViewModel.OrderNo = Audit.OrderNo;
                productionViewModel.OfficeName = Audit.CRN.CRNNAME;
                productionViewModel.ProductType = Audit.AuditRequestType.Name;

                if (Audit.DateCreated.HasValue)
                {
                    productionViewModel.RecievedDateTime = Audit.DateCreated.Value.ToString("yyyy/MM/dd hh:mm tt");
                }

                var processingTaskInfo = Audit.AuditTimeEntries.FirstOrDefault(x => x.AuditTaskId == 1);

                if(processingTaskInfo != null)
                {
                    productionViewModel.ProcessingDoneBy = processingTaskInfo.USERINFO.USERNAME;
                    productionViewModel.ProcessingStartTime = processingTaskInfo.StartTime.ToString("yyyy/MM/dd hh:mm tt");

                    if (processingTaskInfo.EndTime.HasValue)
                    {
                        productionViewModel.ProcessingEndTime = processingTaskInfo.EndTime.Value.ToString("yyyy/MM/dd hh:mm tt");
                        productionViewModel.ProcessingTimeTaken = processingTaskInfo.EndTime.Value.Subtract(processingTaskInfo.StartTime).ToString("c");
                    }
                }

                var qcTaskInfo = Audit.AuditTimeEntries.FirstOrDefault(x => x.AuditTaskId == 2);

                if (qcTaskInfo != null)
                {
                    productionViewModel.QCDoneBy = qcTaskInfo.USERINFO.USERNAME;
                    productionViewModel.QCStartTime = qcTaskInfo.StartTime.ToString("yyyy/MM/dd hh:mm tt");

                    if (qcTaskInfo.EndTime.HasValue)
                    {
                        productionViewModel.QCEndTime = qcTaskInfo.EndTime.Value.ToString("yyyy/MM/dd hh:mm tt");
                        productionViewModel.QCTimeTaken = qcTaskInfo.EndTime.Value.Subtract(qcTaskInfo.StartTime).ToString("c");

                        var updateInfo = Audit.AuditUpdates.FirstOrDefault(x => x.AuditTaskId == 2);

                        if (updateInfo != null)
                        {
                            productionViewModel.QCComments = updateInfo.Updates;
                        }

                        productionViewModel.AuditErrors = errors.Where(x => x.TaskName == "PI").ToList();
                        productionViewModel.AnyCriticalErrors = productionViewModel.AuditErrors.Any() ? "YES" : "NO";
                    }
                }

                var deliveryTaskInfo = Audit.AuditTimeEntries.FirstOrDefault(x => x.AuditTaskId == 3);

                if (deliveryTaskInfo != null)
                {
                    productionViewModel.DeliveryDoneBy = deliveryTaskInfo.USERINFO.USERNAME;
                    productionViewModel.DeliveryStartTime = deliveryTaskInfo.StartTime.ToString("yyyy/MM/dd hh:mm tt");

                    if (deliveryTaskInfo.EndTime.HasValue)
                    {
                        productionViewModel.DeliveryEndTime = deliveryTaskInfo.EndTime.Value.ToString("yyyy/MM/dd hh:mm tt");
                        productionViewModel.DeliveryTimeTaken = deliveryTaskInfo.EndTime.Value.Subtract(deliveryTaskInfo.StartTime).ToString("c");
                    }
                }

                var starterTaskInfo = Audit.AuditTimeEntries.FirstOrDefault(x => x.AuditTaskId == 4);

                if (starterTaskInfo != null)
                {
                    productionViewModel.StarterDoneBy = starterTaskInfo.USERINFO.USERNAME;
                    productionViewModel.StarterStartTime = starterTaskInfo.StartTime.ToString("yyyy/MM/dd hh:mm tt");

                    if (starterTaskInfo.EndTime.HasValue)
                    {
                        productionViewModel.StarterEndTime = starterTaskInfo.EndTime.Value.ToString("yyyy/MM/dd hh:mm tt");
                        productionViewModel.StarterTimeTaken = starterTaskInfo.EndTime.Value.Subtract(starterTaskInfo.StartTime).ToString("c");
                    }
                }

                var notesTaskInfo = Audit.AuditTimeEntries.FirstOrDefault(x => x.AuditTaskId == 5);

                if (notesTaskInfo != null)
                {
                    productionViewModel.NotesDoneBy = notesTaskInfo.USERINFO.USERNAME;
                    productionViewModel.NotesStartTime = notesTaskInfo.StartTime.ToString("yyyy/MM/dd hh:mm tt");

                    if (notesTaskInfo.EndTime.HasValue)
                    {
                        productionViewModel.NotesEndTime = notesTaskInfo.EndTime.Value.ToString("yyyy/MM/dd hh:mm tt");
                        productionViewModel.NotesTimeTaken = notesTaskInfo.EndTime.Value.Subtract(notesTaskInfo.StartTime).ToString("c");
                    }
                }
                productionViewModels.Add(productionViewModel);

                serialNumber++;
            }

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Production.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteHtmlTable<ProductionReportViewModel>(productionViewModels, Response.Output);
            Response.End();
        }

        public ActionResult TaskWisePending()
        {
            List<TaskWisePendingReportViewModel> listItemToReturn = new List<TaskWisePendingReportViewModel>();
            var listOfCrn = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).ToList();

            foreach(var crnItem in listOfCrn)
            {
                TaskWisePendingReportViewModel taskPendingReportViewModel = new TaskWisePendingReportViewModel();

                taskPendingReportViewModel.OfficeName = crnItem.CRNNAME;
                taskPendingReportViewModel.GroupName = crnItem.GroupName;
                taskPendingReportViewModel.CrnId = crnItem.CRNID;
                taskPendingReportViewModel.LVCount = db.Audits.Count(x => x.StatusId != 4 && x.StatusId != 5 && x.StatusId != 3 && x.CrnId == crnItem.CRNID && x.TaskId == 1);

                taskPendingReportViewModel.PICount = db.Audits.Count(x => x.StatusId != 4 && x.StatusId != 5 && x.StatusId != 3 && x.CrnId == crnItem.CRNID && x.TaskId == 2);
                taskPendingReportViewModel.GICount = db.Audits.Count(x => x.StatusId != 4 && x.StatusId != 5 && x.StatusId != 3 && x.CrnId == crnItem.CRNID && x.TaskId == 3);
                taskPendingReportViewModel.StarterCount = db.Audits.Count(x => x.StatusId != 4 && x.StatusId != 5 && x.StatusId != 3 && x.CrnId == crnItem.CRNID && x.TaskId == 4);
                taskPendingReportViewModel.NotesCount = db.Audits.Count(x => x.StatusId != 4 && x.StatusId != 5 && x.StatusId != 3 && x.CrnId == crnItem.CRNID && x.TaskId == 5);
                taskPendingReportViewModel.HoldCount = db.Audits.Count(x => x.StatusId != 4 && x.StatusId != 5 && x.CrnId == crnItem.CRNID && x.StatusId == 3);
                taskPendingReportViewModel.Total = db.Audits.Count(x => x.StatusId != 4 && x.StatusId != 5 && x.CrnId == crnItem.CRNID);

                listItemToReturn.Add(taskPendingReportViewModel);
            }

            return View(listItemToReturn);
        }

        public ActionResult TaskWisePendingDetail(int? taskId, int? crnId, bool? showHold, bool? showCrnTotal, string crnGroupName)
        {
            var Audits = db.Audits.Where(x => x.StatusId != 4 && x.StatusId != 5 );

            if (taskId.HasValue)
            {
                Audits = Audits.Where(x => x.TaskId == taskId.Value);
            }

            if (crnId.HasValue)
            {
                Audits = Audits.Where(x => x.CrnId == crnId.Value);
            }

            if (!string.IsNullOrEmpty(crnGroupName))
            {
                var crnIds = db.CRNs.Where(x => x.IsVisibleInMainApplication == true && x.GroupName == crnGroupName).Select(x => x.CRNID).ToList();

                Audits = Audits.Where(x => crnIds.Contains(x.CrnId));
            }

            if (showHold.HasValue && !showCrnTotal.HasValue)
            {
                Audits = db.Audits.Where(x => x.StatusId != 4 && x.StatusId != 5 && x.StatusId == 3);

                if (crnId.HasValue)
                {
                    Audits = Audits.Where(x => x.CrnId == crnId.Value);
                }
            }
            
            if(!showHold.HasValue && (taskId.HasValue || crnId.HasValue))
            {
                Audits = Audits.Where(x => x.StatusId != 3);
            }

            Audits = Audits.Include(x => x.CRN).Include(x => x.AuditRequestType).Include(x => x.AuditTimeEntries);
            TaskWisePendingReportDetailViewModel viewModel = new TaskWisePendingReportDetailViewModel();

            viewModel.Audits = Audits.ToList();
            viewModel.IsHoldOrders = showHold ?? false;

            if (showCrnTotal.HasValue && showCrnTotal.Value)
            {
                viewModel.IsHoldOrders = false;
            }

            return View(viewModel);
        }

        public ActionResult HoldOrders()
        {
            List<HoldOrderReportViewModel> viewModel = new List<HoldOrderReportViewModel>();

            var groupByCrn = (from k in db.Audits.Where(x => x.StatusId == 3 && x.StatusId != 4 && x.StatusId != 5)
                              group k by k.CrnId into grp
                              select new { key = grp.Key, Count = grp.Count() }).ToList();

            foreach (var crn in db.CRNs.Where(x => x.IsVisibleInMainApplication == true))
            {
                var grpItem = groupByCrn.FirstOrDefault(x => x.key == crn.CRNID);
                HoldOrderReportViewModel holdOrder = new HoldOrderReportViewModel();
                holdOrder.CrnId = crn.CRNID;
                holdOrder.OfficeName = crn.CRNNAME;

                if (grpItem != null)
                {
                    holdOrder.Count = grpItem.Count;
                }
                else
                {
                    holdOrder.Count = 0;
                }
                viewModel.Add(holdOrder);
            }

            return View(viewModel);
        }

        public ActionResult HoldOrderDetails(int? crnId)
        {
            var Audits = db.Audits.Where(x => x.StatusId == 3);

            if (crnId.HasValue)
            {
                Audits = Audits.Where(x => x.CrnId == crnId.Value);
            }
            Audits = Audits.Include(x => x.CRN).Include(x => x.AuditRequestType).Include(x => x.AuditTask).Include(y => y.AuditHolds);

            return View(Audits.ToList());
        }

        public ActionResult RejectedOrders()
        {
            return View();
        }

        public JsonResult GetRejectedOrders(DateTime? startDateTime, DateTime? endDateTime)
        {
            var Audits = db.Audits.Where(x => x.StatusId == 5 && x.AuditRejects.Any(y => y.RejectedDateTime > startDateTime && y.RejectedDateTime < endDateTime))
                 .Include(x => x.AuditRequestType)
                 .Include(x => x.AuditStatus)
                 .Include(x => x.AuditTask)
                 .Include(x => x.AuditRejects)
                 .Include(x => x.AuditSender)
                 .Include(x => x.CRN);

            List<RejectOrderInfo> rejectOrders = new List<RejectOrderInfo>();
            int serialNumber = 1;
            foreach (var rejectedOrder in Audits)
            {
                RejectOrderInfo rejOrderInfo = new RejectOrderInfo();
                rejOrderInfo.OfficeName = rejectedOrder.CRN.CRNNAME;
                rejOrderInfo.OrderNo = rejectedOrder.OrderNo;
                rejOrderInfo.SerialNumber = serialNumber;
                rejOrderInfo.OrderType = rejectedOrder.AuditRequestType.Name;
                rejOrderInfo.RecievedDateTime = rejectedOrder.DateCreated.ToString();


                var rejectedOrderInfo = rejectedOrder.AuditRejects.FirstOrDefault();

                if (rejectedOrderInfo != null) {
                    rejOrderInfo.RejectedBy = rejectedOrderInfo.USERINFO.USERNAME;
                    rejOrderInfo.RejectedDateTime = rejectedOrderInfo.RejectedDateTime.ToString();
                    rejOrderInfo.Reason = rejectedOrderInfo.Reason;
                }

                rejectOrders.Add(rejOrderInfo);

                serialNumber++;
            }
            

            return new JsonResult() { Data = rejectOrders, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult OrderCompleted()
        {
            return View();
        }

        public JsonResult GetCompletedOrders(DateTime? startDateTime, DateTime? endDateTime)
        {
            var Audits = db.Audits.Where(x => x.StatusId == 4 && x.UploadDateTime > startDateTime && x.UploadDateTime < endDateTime)
                 .Include(x => x.AuditRequestType)
                 .Include(x => x.AuditStatus)
                 .Include(x => x.AuditTask)
                 .Include(x => x.AuditTimeEntries)
                 .Include(x => x.AuditSender)
                 .Include(x => x.CRN);

            List<OrderCompletedInfo> completedOrders = new List<OrderCompletedInfo>();
            int serialNumber = 1;
            foreach (var completedOrder in Audits)
            {
                OrderCompletedInfo completeOrderInfo = new OrderCompletedInfo();
                completeOrderInfo.OfficeName = completedOrder.CRN.CRNNAME;
                completeOrderInfo.OrderNo = completedOrder.OrderNo;
                completeOrderInfo.SerialNumber = serialNumber;
                completeOrderInfo.RecievedDateTime = completedOrder.DateCreated.ToString();
                completeOrderInfo.OrderType = completedOrder.AuditRequestType.Name;

                var completedOrderInfo = completedOrder.AuditTimeEntries.FirstOrDefault(x => x.AuditTaskId == 3);

                if (completedOrderInfo != null)
                {
                    completeOrderInfo.UploadedBy = completedOrderInfo.USERINFO.USERNAME;
                    completeOrderInfo.UploadDateTime = completedOrderInfo.EndTime.ToString();
                }

                completedOrders.Add(completeOrderInfo);

                serialNumber++;
            }

            return new JsonResult() { Data = completedOrders, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public void ExportListFromTable()
        {
            
        }


        public void WriteHtmlTable<T>(IEnumerable<T> data, TextWriter output)
        {
            // Writes markup characters and text to an ASP.NET server control output stream.
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    // Create a form to contain the List
                    Table table = new Table();
                    TableRow row = new TableRow();
                    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

                    // Create header row
                    foreach (PropertyDescriptor prop in props)
                    {
                        TableHeaderCell hcell = new TableHeaderCell();
                        hcell.Text = prop.DisplayName;
                        hcell.BackColor = System.Drawing.Color.Yellow;
                        hcell.BorderStyle = BorderStyle.Solid;
                        hcell.BorderColor = System.Drawing.Color.LightGray;
                        hcell.Width = 150;

                        row.Cells.Add(hcell);
                    }
                    table.Rows.Add(row);

                    // Add each data item to the table
                    foreach (T item in data)
                    {
                        row = new TableRow();
                        foreach (PropertyDescriptor prop in props)
                        {
                            TableCell cell = new TableCell();

                            if (prop.Name == "AuditErrors")
                            {
                                var errors = (IEnumerable<AuditError>)prop.GetValue(item);

                                if (errors != null && errors.Any())
                                {
                                    // Render errors as an inner table
                                    Table errorsTable = new Table();

                                    //TableRow headerRow = new TableRow();
                                    //TableHeaderCell categoryCell = new TableHeaderCell();
                                    //categoryCell.BorderStyle = BorderStyle.Solid;
                                    //categoryCell.Text = "Category";
                                    //categoryCell.BorderColor = System.Drawing.Color.LightGray;
                                    //headerRow.Cells.Add(categoryCell);

                                    //TableHeaderCell errorTypesHeaderCell = new TableHeaderCell();
                                    //errorTypesHeaderCell.BorderStyle = BorderStyle.Solid;
                                    //errorTypesHeaderCell.Text = "Error Types";
                                    //errorTypesHeaderCell.BorderColor = System.Drawing.Color.LightGray;
                                    //headerRow.Cells.Add(errorTypesHeaderCell);

                                    //TableHeaderCell commentsCell = new TableHeaderCell();
                                    //commentsCell.BorderStyle = BorderStyle.Solid;
                                    //commentsCell.Text = "Comments";
                                    //commentsCell.BorderColor = System.Drawing.Color.LightGray;
                                    //headerRow.Cells.Add(commentsCell);

                                    //errorsTable.Rows.Add(headerRow);

                                    foreach (var error in errors)
                                    {
                                        TableRow errorRow = new TableRow();
                                        TableCell errorCell = new TableCell
                                        {
                                            Text = error.SelectedCategoryName, // Customize how you want to display the error
                                            Height = 20
                                        };
                                        errorRow.Cells.Add(errorCell);

                                        List<string> errorTypes = new List<string>();
                                        foreach (var errorType in error.ErrorTypes)
                                        {
                                            errorTypes.Add(errorType.Name + "(" + @errorType.IsCriticalText + ")");
                                        }

                                        TableCell errorTypesCell = new TableCell
                                        {
                                            Text = string.Join(", ", errorTypes), // Customize how you want to display the error
                                            Height = 20
                                        };
                                        errorRow.Cells.Add(errorTypesCell);
                                        errorRow.Cells.Add(new TableCell() { Text = error.Comments, Height = 20 });

                                        errorsTable.Rows.Add(errorRow);


                                    }

                                    // Render the inner errors table to this cell
                                    using (StringWriter innerSw = new StringWriter())
                                    {
                                        using (HtmlTextWriter innerHtw = new HtmlTextWriter(innerSw))
                                        {
                                            errorsTable.RenderControl(innerHtw);
                                            cell.Text = innerSw.ToString(); // Add the rendered HTML of the errors table
                                        }
                                    }
                                }
                                else
                                {
                                    cell.Text = ""; // Add the rendered HTML of the errors table
                                }
                            }
                            else
                            {
                                // Default behavior for other properties
                                cell.Text = prop.Converter.ConvertToString(prop.GetValue(item));
                                cell.Height = 20;
                            }

                            row.Cells.Add(cell);
                        }
                        table.Rows.Add(row);
                    }

                    // Render the table into the HtmlTextWriter
                    table.RenderControl(htw);

                    // Output the rendered HTML table
                    output.Write(sw.ToString());
                }
            }
        }

        public ActionResult UploadOrders()
        {
            OrderUploadViewModel uploadViewModel = new OrderUploadViewModel();
            return View(uploadViewModel);
        }

        [HttpPost]
        public ActionResult UploadOrders(HttpPostedFileBase[] files)
        {
            OrderUploadViewModel uploadViewModel = new OrderUploadViewModel();
            if (this.Request.Files != null && this.Request.Files.Count > 0)
            {
                if (!string.IsNullOrEmpty(this.Request.Files[0].FileName))
                {
                    string mainPath = "";
                    bool anyErrors = false;

                    if (ConfigurationManager.AppSettings.Get("UPLOAD-FILEPATH") != null)
                    {
                        mainPath = ConfigurationManager.AppSettings.Get("UPLOAD-FILEPATH");
                    }

                    var uploads = this.Request.Files;

                    string newUploadGuid = Guid.NewGuid().ToString();
                    string orderUploadPath = "";

                    if (Directory.Exists(mainPath + @"\Data\NewUpload\"))
                    {
                        Directory.CreateDirectory(mainPath + @"\Data\NewUpload\");
                    }

                    if (!Directory.Exists(mainPath + @"\Data\NewUpload\" + newUploadGuid))
                    {
                        Directory.CreateDirectory(mainPath + @"\Data\NewUpload\" + newUploadGuid);
                    }

                    int fileInx = 0;
                    foreach (string requestFileName in this.Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[fileInx];

                        if (Path.GetExtension(file.FileName).ToUpper() == ".PDF")
                        {
                            file.SaveAs(mainPath + @"\Data\NewUpload\" + newUploadGuid + @"\" + file.FileName);
                        }

                        if (Path.GetExtension(file.FileName).ToUpper() == ".XLSX")
                        {
                            orderUploadPath = mainPath + @"\Data\NewUpload\" + newUploadGuid + Path.GetExtension(file.FileName);
                            file.SaveAs(orderUploadPath);
                        }

                        fileInx++;
                    }

                    var dataInExcel = readDataFromExcel(orderUploadPath, "[All Orders$]");


                    var allCrns = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).ToList();

                    List<OrderUploadModel> uploadModelItems = new List<OrderUploadModel>();
                    List<Audit> titleOrders = new List<Audit>();
                    foreach (DataRow dataRow in dataInExcel.Rows)
                    {
                        Audit newTitleOrder = new Audit();
                        OrderUploadModel orderUploadModel = new OrderUploadModel();
                        newTitleOrder.StatusId = 1;
                        newTitleOrder.TaskId = 1;
                        newTitleOrder.UserCreated = 1;

                        var crnName = dataRow["OPERATION"].ToString();

                        crnName = crnName.ToUpper() == "FIDELITY TUSTIN RESALE" ? "Fidelity Tustin" : crnName;

                        var crn = allCrns.FirstOrDefault(x => x.CRNNAME.ToUpper() == crnName.ToUpper());

                        if (crn != null)
                        {
                            newTitleOrder.CrnId = crn.CRNID;
                            orderUploadModel.Customer = crnName;
                        }
                        else
                        {
                            anyErrors = true;
                            orderUploadModel.Message = orderUploadModel.Message + " - INVALID CRN - ";
                        }


                        newTitleOrder.OrderNo = dataRow["ORDER_NUMBER"].ToString();
                        orderUploadModel.OrderNumber = newTitleOrder.OrderNo;

                        var senderEmailAddress = dataRow["SENDEREMAILADDRESS"].ToString();

                        if (!string.IsNullOrEmpty(senderEmailAddress))
                        {
                            var senderId = db.AuditSenders.FirstOrDefault(x => x.EmailAddress.Equals(senderEmailAddress, StringComparison.OrdinalIgnoreCase));

                            if (senderId != null)
                            {
                                newTitleOrder.SenderId = senderId.Id;
                            }
                        }
                        else
                        {
                            anyErrors = true;
                            orderUploadModel.Message = orderUploadModel.Message + " - INVALID SENDER - ";
                        }

                        var productType = dataRow["PRODUCTTYPE"].ToString();

                        if (!string.IsNullOrEmpty(productType))
                        {
                            var requestType = db.AuditRequestTypes.FirstOrDefault(x => x.Name.Equals(productType, StringComparison.OrdinalIgnoreCase));

                            if (requestType != null)
                            {
                                newTitleOrder.RequestTypeId = requestType.Id;
                            }
                        }
                        else
                        {
                            anyErrors = true;
                            orderUploadModel.Message = orderUploadModel.Message + " - INVALID PRODUCT TYPE - ";
                        }

                        var clientInstructions = dataRow["CLIENTINSTRUCTIONS"].ToString();
                        if (!string.IsNullOrEmpty(clientInstructions))
                        {
                            newTitleOrder.ClientInstructions = clientInstructions;
                        }

                        var county = dataRow["COUNTY"].ToString();
                        if (!string.IsNullOrEmpty(county))
                        {
                            newTitleOrder.County = county;
                        }

                        string receivedDateTime = dataRow["RECEIVEDDATETIME"].ToString();

                        if (!string.IsNullOrEmpty(receivedDateTime))
                        {
                            string dateToParseString = receivedDateTime.Replace(" PDT", "");
                            var pdtDate = DateTime.Parse(dateToParseString);
                            newTitleOrder.DateCreated = pdtDate;
                        }else
                        {
                            anyErrors = true;
                            orderUploadModel.Message = orderUploadModel.Message + " - INVALID RECEIVED DATE TIME - ";
                        }

                        if (uploadModelItems.Any(x => x.OrderNumber.Equals(orderUploadModel.OrderNumber)))
                        {
                            orderUploadModel.Message = orderUploadModel.Message + " DUPLICATE ORDER";
                            anyErrors = true;
                        }

                        string isRush = dataRow["ISRUSH"].ToString();

                        if (!string.IsNullOrEmpty(isRush))
                        {
                            if (isRush == "YES" || isRush == "NO")
                            {
                                newTitleOrder.IsRushOrder = isRush == "YES";
                            }
                            else
                            {
                                orderUploadModel.Message = orderUploadModel.Message + " Wrong value in ISRUSH";
                                anyErrors = true;
                            }
                        }
                        else
                        {
                            orderUploadModel.Message = orderUploadModel.Message + " Wrong value in ISRUSH";
                            anyErrors = true;
                        }


                        uploadModelItems.Add(orderUploadModel);

                        titleOrders.Add(newTitleOrder);
                    }

                    if (!anyErrors)
                    {
                        uploadViewModel.AnyErrors = false;
                        Session.Add("NEWORDERTOSAVE", titleOrders);
                    }
                    else
                    {
                        uploadViewModel.AnyErrors = true;
                    }

                    uploadViewModel.Items = uploadModelItems;
                }
            }

            return View(uploadViewModel);
        }

        public ActionResult SaveUploadedOrders()
        {
            try
            {


                List<Audit> titleOrders = Session["NEWORDERTOSAVE"] as List<Audit>;

                if (titleOrders != null)
                {
                    var allCrns = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).ToList();

                    foreach (Audit titleOrder in titleOrders)
                    {
                        var crnObject = allCrns.FirstOrDefault(x => x.CRNID == titleOrder.CrnId);


                        if (crnObject != null)
                        {
                            db.Audits.Add(titleOrder);
                        }
                        else
                        {
                            ViewBag.ValidationErrors = "NO CRN";
                        }
                    }


                    db.SaveChanges();

                    Session.Remove("NEWORDERTOSAVE");
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                List<string> validationErrorList = new List<string>();
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        validationErrorList.Add(string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage));
                    }
                }
                ViewBag.ValidationErrors = string.Join(",", validationErrorList);
            }

            return View();
        }

        public ActionResult DownloadSampleExcel()
        {
            // Path to the sample Excel file
            string filePath = Server.MapPath("~/SampleUpload.xlsx");

            // Set the content type and header for the file
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = Path.GetFileName(filePath);

            // Return the file as a file result
            return File(filePath, contentType, fileName);
        }

        public ActionResult MakeRush(int orderId)
        {
            Audit txOrder = db.Audits.Find(orderId);

            ViewBag.OrderNo = txOrder.OrderNo;

            return View();
        }

        public ActionResult MakeNormal(int orderId)
        {
            Audit txOrder = db.Audits.Find(orderId);

            txOrder.IsRushOrder = false;

            db.SaveChanges();

            return RedirectToAction("Details", new { id = txOrder.Id });
        }

        [HttpPost, ActionName("MakeRush")]
        [ValidateAntiForgeryToken]
        public ActionResult MakeRushConfirmed(int orderId, string comments)
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Audit txOrder = db.Audits.Find(orderId);

            txOrder.IsRushOrder = true;
            txOrder.RushOrderComments = comments;

            db.SaveChanges();

            return RedirectToAction("Details", new { id = txOrder.Id });
        }

        private void sendMail(string mailTo, string from, string subject, string bodyContent, List<AuditDocument> AuditDocuments, string mailCC = null, bool isHtml = false)
        {
            var emailMessage = new MailMessage();

            emailMessage.From = new MailAddress(from);

            foreach (var emailAddressItem in mailTo.Split(';'))
            {
                emailMessage.To.Add(new MailAddress(emailAddressItem));
            }

            if (!string.IsNullOrEmpty(mailCC))
            {
                foreach (var emailAddressItem in mailCC.Split(';'))
                {
                    emailMessage.CC.Add(new MailAddress(emailAddressItem));
                }
            }

            var smtpServerConfig = ConfigurationManager.AppSettings.Get("SMTP-SERVERNAME");
            var smtpUserConfig = ConfigurationManager.AppSettings.Get("SMTP-USERNAME");
            var smtpPasswordConfig = ConfigurationManager.AppSettings.Get("SMTP-PASSWORD");

            string smtpServer = !string.IsNullOrEmpty(smtpServerConfig) ? smtpServerConfig : "smtp.fnfsocal.com";
            string smtpUser = !string.IsNullOrEmpty(smtpUserConfig) ? smtpUserConfig : "si@fnfsocal.com";
            string smtpPassword = !string.IsNullOrEmpty(smtpPasswordConfig) ? smtpPasswordConfig : "Title@123";

            var smtpEmailClient = new SmtpClient(smtpServer);
            smtpEmailClient.UseDefaultCredentials = true;
            smtpEmailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpEmailClient.EnableSsl = false;
            smtpEmailClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);

            emailMessage.Subject = subject;
            emailMessage.Body = bodyContent;
            emailMessage.IsBodyHtml = isHtml;

            if (AuditDocuments != null && AuditDocuments.Count > 0)
            {
                List<string> fileToDelete = new List<string>();
                var uploadFilePath = ConfigurationManager.AppSettings.Get("UPLOAD-FILEPATH");
                var attachmentTempFolder = uploadFilePath + "\\" + AuditDocuments[0].AuditId.ToString();
                if (!Directory.Exists(attachmentTempFolder))
                {
                    Directory.CreateDirectory(attachmentTempFolder);
                }

                foreach (AuditDocument AuditDoc in AuditDocuments)
                {
                    var newFileName = attachmentTempFolder + "\\" + AuditDoc.FileName;
                    fileToDelete.Add(newFileName);
                    System.IO.File.Copy(AuditDoc.DocumentPath, newFileName);
                    emailMessage.Attachments.Add(new Attachment(newFileName));
                }
            }

            try
            {
                smtpEmailClient.Send(emailMessage);
            }
            catch (Exception exe) { }
        }

        private DataTable readDataFromExcel(string filePath, string sheetName)
        {
            string connStr = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES""", filePath);
            OleDbConnection oleDbConnection = new OleDbConnection(connStr);

            oleDbConnection.Open();

            string query = string.Format("SELECT * FROM {0}", sheetName);

            OleDbCommand command = new OleDbCommand(query, oleDbConnection);
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(command);

            DataTable dtTable = new DataTable();
            dataAdapter.Fill(dtTable);
            oleDbConnection.Close();

            return dtTable;
        }

        public ActionResult RushOrders()
        {
            List<RushOrderReportViewModel> viewModel = new List<RushOrderReportViewModel>();

            var groupByCrn = (from k in db.Audits.Where(x => x.IsRushOrder == true && x.StatusId != 4 && x.StatusId != 5)
                              group k by k.CrnId into grp
                              select new { key = grp.Key, Count = grp.Count() }).ToList();

            foreach (var crn in db.CRNs.Where(x => x.IsVisibleInMainApplication == true))
            {
                var grpItem = groupByCrn.FirstOrDefault(x => x.key == crn.CRNID);
                RushOrderReportViewModel rushOrder = new RushOrderReportViewModel();
                rushOrder.CrnId = crn.CRNID;
                rushOrder.OfficeName = crn.CRNDISPLAYNAME;

                if (grpItem != null)
                {
                    rushOrder.Count = grpItem.Count;
                }
                else
                {
                    rushOrder.Count = 0;
                }
                viewModel.Add(rushOrder);
            }

            return View(viewModel);
        }

        public ActionResult RushOrderDetails(int? crnId)
        {
            var titleOrders = db.Audits.Where(x => x.IsRushOrder == true && x.StatusId != 4 && x.StatusId != 5);

            if (crnId.HasValue)
            {
                titleOrders = titleOrders.Where(x => x.CrnId == crnId.Value);
            }
            titleOrders = titleOrders.Include(x => x.CRN).Include(x => x.AuditRequestType).Include(x => x.AuditTask).Include(y => y.AuditHolds);

            return View(titleOrders.OrderBy(x => x.CRN.CRNNAME).ThenBy(x => x.DateCreated).ToList());
        }

        public ActionResult OrderAssignment(int? crnId, int? taskId, int? requestTypeId, bool? isRush, string groupName)
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            FilterService filterService = new FilterService(db, regionCode, userInfo.USERID);

            if (!crnId.HasValue && !taskId.HasValue && !requestTypeId.HasValue)
            {
                //Try loading it from filter if any.
                var uiFilter = filterService.GetFilter(AuditUiFilter);

                if (uiFilter != null)
                {
                    AuditUiFilter titleOrderFilter = JsonConvert.DeserializeObject<AuditUiFilter>(uiFilter.FilterJson);

                    if (titleOrderFilter != null)
                    {
                        crnId = titleOrderFilter.CrnId;
                        taskId = titleOrderFilter.TaskId;
                        requestTypeId = titleOrderFilter.RequestTypeId;
                    }
                }
            }
            else
            {
                AuditUiFilter titleOrderFilterToSave = new AuditUiFilter() { CrnId = crnId, TaskId = taskId, RequestTypeId = requestTypeId };
                filterService.SaveFilter(AuditUiFilter, JsonConvert.SerializeObject(titleOrderFilterToSave));
            }

            var titleOrders = (from k in db.Audits.Include(t => t.AuditStatus)
                .Include(t => t.AuditTask)
                .Include(t => t.AuditRequestType)

                               where k.StatusId != 3 && k.StatusId != 4 && k.StatusId != 5 && !k.AuditTimeEntries.Any(x => x.IsInprogress)
                               select k).AsQueryable();



            if (!taskId.HasValue)
            {
                titleOrders = titleOrders.Where(x => x.TaskId == 1);
            }
            else
            {
                titleOrders = titleOrders.Where(x => x.TaskId == taskId.Value);
            }

            if (requestTypeId.HasValue)
            {
                titleOrders = titleOrders.Where(x => x.RequestTypeId == requestTypeId.Value);
            }

            List<int> allowedCrnIdsForThisUser = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).Select(x => x.CRNID).ToList();
            if (!string.IsNullOrEmpty(groupName))
            {
                allowedCrnIdsForThisUser = (from crn in db.CRNs
                                            where crn.GroupName == groupName
                                            select crn.CRNID).ToList();
            }


            if (crnId.HasValue)
            {
                titleOrders = titleOrders.Where(x => x.CrnId == crnId.Value);
            }
            else
            {
                titleOrders = titleOrders.Where(x => allowedCrnIdsForThisUser.Contains(x.CrnId));
            }

            if (isRush.HasValue)
            {
                titleOrders = titleOrders.Where(x => x.IsRushOrder == isRush.Value);
            }

            AuditTimeEntry timeEntry = db.AuditTimeEntries.FirstOrDefault(x => x.IsInprogress && x.UserId == userInfo.USERID);

            if (timeEntry != null)
            {
                ViewBag.InprogressAuditId = timeEntry.AuditId;
            }

            List<int> allowedTaskIts = db.AuditTasks.Select(x => x.Id).ToList();

            List<Rush> rushes = new List<Rush>();
            rushes.Add(new Rush(false, "No"));
            rushes.Add(new Rush(true, "Yes"));

            ViewBag.TaskId = new SelectList(db.AuditTasks.Where(x => allowedTaskIts.Contains(x.Id)), "Id", "Name", taskId ?? 1);
            ViewBag.RequestTypeId = new SelectList(db.AuditRequestTypes, "Id", "Name", requestTypeId ?? 0);
            ViewBag.CrnId = new SelectList(db.CRNs.Where(x => x.IsVisibleInMainApplication == true), "CRNID", "CRNDISPLAYNAME", crnId ?? 0);
            ViewBag.IsRush = new SelectList(rushes, "Key", "Value", requestTypeId);
            ViewBag.UserInfos = new SelectList(db.USERINFOes.OrderBy(x => x.USERNAME).ToList(), "USERID", "USERNAME");
            var offices = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).ToList();
            ViewBag.Crns = offices.Select(x => new { x.CRNID, x.CRNNAME, x.GroupName }).ToList();
            ViewBag.OfficeGroups = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).Select(x => x.GroupName).Distinct().ToList();
            ViewBag.GroupName = groupName;

            var items = titleOrders.ToList();

            foreach (var item in items.Where(x => x.UserAssigned.HasValue))
            {
                var userInfoObj = db.USERINFOes.FirstOrDefault(x => x.USERID == item.UserAssigned.Value);

                if (userInfoObj != null)
                {
                    item.UserAssignedName = userInfoObj.USERNAME;
                }
            }

            return View(items);
        }

        [HttpPost]
        public int AssignUserToOrder(int? userId, List<int> titleOrderIds)
        {
            titleOrderIds.ForEach(orderId =>
            {
                Audit titleOrder = db.Audits.FirstOrDefault(x => x.Id == orderId);
                titleOrder.UserAssigned = userId;
            });

            return db.SaveChanges();
        }


        private List<AuditError> GetErrors(int AuditId)
        {
            List<AuditError> AuditErrors = new List<AuditError>();
            var tasks = db.AuditTasks.ToDictionary(x => x.Id, y => y.Name);
            var errorCategories = db.AuditErrorCategories.ToDictionary(x => x.Id, y => y.Name);
            var errorTypes = db.AuditErrorTypes.Select(x => new { x.Id, Name = x.Name, x.IsCritical }).ToList();

            var listOfErrors = db.AuditErrorJsons.Where(x => x.AuditId == AuditId);

            foreach (var AuditError in listOfErrors)
            {
                var errorDeObject = JsonConvert.DeserializeObject<List<AuditError>>(AuditError.OrderErrorJson);

                if (errorDeObject != null)
                {
                    foreach (var item in errorDeObject)
                    {
                        item.TaskName = tasks[AuditError.TaskId];
                        item.SelectedCategoryName = errorCategories[item.SelectedCategory];
                        item.ErrorTypes = new List<AuditErrorTypeLite>();
                        foreach (var selectedType in item.SelectedType)
                        {
                            var errorTypeItem = errorTypes.FirstOrDefault(x => x.Id == selectedType);

                            if (errorTypeItem != null)
                            {
                                item.ErrorTypes.Add(new AuditErrorTypeLite() { Id = selectedType, Name = errorTypeItem.Name, IsCriticalText = errorTypeItem.IsCritical ? "C" : "NC" });
                            }
                        }
                    }

                    AuditErrors.AddRange(errorDeObject);
                }
            }

            return AuditErrors;
        }

        private void sendErrorEmail(int AuditId, int taskId, string orderErrorJson, int timeEntryId)
        {
            var lastTimeEntry = db.AuditTimeEntries.Where(x => x.AuditId == AuditId && x.Id != timeEntryId).OrderByDescending(x => x.Id).FirstOrDefault();

            if (lastTimeEntry != null)
            {
                var userInfo = db.USERINFOes.FirstOrDefault(x => x.USERID == lastTimeEntry.UserId);

                if (userInfo != null)
                {
                    string holdEmailTo = !string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("ERROREMAIL-TO")) ? ConfigurationManager.AppSettings.Get("ERROREMAIL-TO") : "prelims@firsttitlebpo.com";
                    string holdEmailFrom = !string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("ERROREMAIL-FROM")) ? ConfigurationManager.AppSettings.Get("ERROREMAIL-FROM") : "notifications@ftbpo.com";

                    if (!string.IsNullOrEmpty(holdEmailTo) && !string.IsNullOrEmpty(holdEmailFrom))
                    {
                        var tasks = db.AuditTasks.ToDictionary(x => x.Id, y => y.Name);
                        var errorCategories = db.AuditErrorCategories.ToDictionary(x => x.Id, y => y.Name);
                        var errorTypes = db.AuditErrorTypes.Select(x => new { x.Id, Name = x.Name, x.IsCritical }).ToList();

                        var errorDeObject = JsonConvert.DeserializeObject<List<AuditError>>(orderErrorJson);

                        if (errorDeObject != null)
                        {
                            foreach (var item in errorDeObject)
                            {
                                item.TaskName = tasks[lastTimeEntry.AuditTaskId];
                                item.SelectedCategoryName = errorCategories[item.SelectedCategory];
                                item.ErrorTypes = new List<AuditErrorTypeLite>();
                                foreach (var selectedType in item.SelectedType)
                                {
                                    var errorTypeItem = errorTypes.FirstOrDefault(x => x.Id == selectedType);

                                    if (errorTypeItem != null)
                                    {
                                        item.ErrorTypes.Add(new AuditErrorTypeLite() { Id = selectedType, Name = errorTypeItem.Name, IsCriticalText = errorTypeItem.IsCritical ? "C" : "NC" });
                                    }
                                }
                            }
                        }

                        string emailContent = GenerateErrorReportHtml(errorDeObject);
                        string subject = string.Format("FNT Update - Errors in - Order No: {0}, Task: {1}", lastTimeEntry.Audit.OrderNo, tasks[lastTimeEntry.AuditTaskId]);

                        sendMail(holdEmailTo, holdEmailFrom, subject, emailContent, null, userInfo.EMAIL, true);
                    }

                }
            }
        }

        public string GenerateErrorReportHtml(List<AuditError> AuditErrors)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<h4>FNT Update Error Report</h4>");

            // Group by TaskName
            var groupedErrors = AuditErrors
                .GroupBy(e => e.TaskName)
                .Select(group => new
                {
                    TaskName = group.Key,
                    Errors = group.ToList()
                })
                .ToList();

            sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse; width: 100%;'>");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th>Task Name</th>");
            sb.AppendLine("<th>Selected Category</th>");
            sb.AppendLine("<th>Error Types</th>");
            sb.AppendLine("<th>Comments</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            foreach (var group in groupedErrors)
            {
                // Task Name as a separate row
                sb.AppendLine($"<tr><td colspan='5' style='background-color: #f9f9f9; font-weight: bold;'>{group.TaskName}</td></tr>");

                // Error details for the task
                foreach (var error in group.Errors)
                {
                    sb.AppendLine("<tr>");
                    sb.AppendLine("<td></td>"); // Empty cell for spacing
                    sb.AppendLine($"<td>{error.SelectedCategoryName}</td>");

                    // List of Error Types
                    sb.AppendLine("<td>");
                    foreach (var errorType in error.ErrorTypes)
                    {
                        sb.AppendLine($"{errorType.Name} ({errorType.IsCriticalText})<br />");
                    }
                    sb.AppendLine("</td>");

                    sb.AppendLine($"<td>{error.Comments}</td>");
                    sb.AppendLine("</tr>");
                }
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");

            return sb.ToString();
        }

        private void sendMail(string mailTo, string from, string subject, string bodyContent, string mailCC = null)
        {
            var emailMessage = new MailMessage();

            emailMessage.From = new MailAddress(from);

            foreach (var emailAddressItem in mailTo.Split(';'))
            {
                emailMessage.To.Add(new MailAddress(emailAddressItem));
            }

            if (!string.IsNullOrEmpty(mailCC))
            {
                foreach (var emailAddressItem in mailCC.Split(';'))
                {
                    emailMessage.CC.Add(new MailAddress(emailAddressItem));
                }
            }

            var smtpServerConfig = ConfigurationManager.AppSettings.Get("SMTP-SERVERNAME");
            var smtpUserConfig = ConfigurationManager.AppSettings.Get("SMTP-USERNAME");
            var smtpPasswordConfig = ConfigurationManager.AppSettings.Get("SMTP-PASSWORD");

            string smtpServer = !string.IsNullOrEmpty(smtpServerConfig) ? smtpServerConfig : "";
            string smtpUser = !string.IsNullOrEmpty(smtpUserConfig) ? smtpUserConfig : "";
            string smtpPassword = !string.IsNullOrEmpty(smtpPasswordConfig) ? smtpPasswordConfig : "";

            var smtpEmailClient = new SmtpClient(smtpServer);
            //smtpEmailClient.UseDefaultCredentials = true;
            smtpEmailClient.Port = 587;
            smtpEmailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpEmailClient.EnableSsl = true;
            smtpEmailClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);

            emailMessage.Subject = subject;
            emailMessage.Body = bodyContent;

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                smtpEmailClient.Send(emailMessage);
            }
            catch (Exception exe) { }
        }

        public JsonResult GetErrorCategories()
        {
            JsonResult jsonResult = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            jsonResult.Data = db.AuditErrorCategories.Select(x => new { x.Id, x.Name }).ToList();

            return jsonResult;
        }

        public JsonResult GetErrorTypes(int categoryId)
        {
            JsonResult jsonResult = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            jsonResult.Data = db.AuditErrorTypes
                            .Where(t => t.AuditErrorCategoryId == categoryId)
                            .Select(t => new { t.Id, t.Name, t.IsCritical })
                            .ToList();

            return jsonResult;
        }

        private List<AuditCheckModel> GetAuditChecks(string orderNumber, int taskId, int crnId)
        {
            var instructions = new List<AuditCheckModel>();
            var allAuditCheckItems = new List<AuditCheck>();

            var onlyOfficeAuditChecks = db.AuditChecks.Where(x => x.AuditCheckCrns.Any(ct => ct.CrnId == crnId) && !x.AuditCheckTasks.Any()).ToList();
            var onlyTaskAuditChecks = db.AuditChecks.Where(x => !x.AuditCheckCrns.Any() && x.AuditCheckTasks.Any(ct => ct.TaskId == taskId)).ToList();
            var officeTaskAuditChecks = db.AuditChecks.Where(x => x.AuditCheckCrns.Any(ct => ct.CrnId == crnId) && x.AuditCheckTasks.Any(ct => ct.TaskId == taskId)).ToList();
            var allAuditChecks = db.AuditChecks.Where(x => !x.AuditCheckCrns.Any() && !x.AuditCheckTasks.Any()).ToList();

            if (onlyOfficeAuditChecks.Any())
                allAuditCheckItems.AddRange(onlyOfficeAuditChecks);

            if (onlyTaskAuditChecks.Any())
                allAuditCheckItems.AddRange(onlyTaskAuditChecks);

            if (officeTaskAuditChecks.Any())
                allAuditCheckItems.AddRange(officeTaskAuditChecks);

            if (allAuditChecks.Any())
                allAuditCheckItems.AddRange(allAuditChecks);

            List<AuditCheck> notMatchedAuditChecks = new List<AuditCheck>();
            foreach (var orderNumberAuditCheck in allAuditCheckItems.Where(x => !string.IsNullOrEmpty(x.OrderNumberSeries)))
            {
                var stringPatterns = orderNumberAuditCheck.OrderNumberSeries.Split(',');
                var isMatch = false;
                foreach (var pattern in stringPatterns)
                {
                    if (pattern.StartsWith("*") && orderNumber.EndsWith(pattern.Substring(1)))
                    {
                        isMatch = true;
                    }
                    else if (pattern.EndsWith("*") && orderNumber.StartsWith(pattern.Substring(0, pattern.Length - 1)))
                    {
                        isMatch = true;
                    }
                }

                if (!isMatch)
                {
                    notMatchedAuditChecks.Add(orderNumberAuditCheck);
                }
            }

            foreach (var itemToRemore in notMatchedAuditChecks)
            {
                allAuditCheckItems.Remove(itemToRemore);
            }

            foreach (var item in allAuditCheckItems)
            {
                var checkModel = new AuditCheckModel();

                checkModel.Name = item.Name;

                if (!string.IsNullOrEmpty(item.AuditCheckValues))
                {
                    checkModel.AuditCheckValues = item.AuditCheckValues.Split('|').ToList();
                    checkModel.IsDropdownList = item.AuditCheckValues.Contains("|");
                }
                else
                {
                    checkModel.IsDropdownList = false;
                }
                checkModel.AuditCheckId = item.Id;

                instructions.Add(checkModel);
            }

            return instructions;
        }

        public ActionResult UserReport(DateTime? startDateTime, DateTime? endDateTime, string location, string groupName, int? crnId, string selectedOffices)
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<int> crnIds = new List<int>();

            if (!string.IsNullOrEmpty(selectedOffices))
            {
                foreach (var checkOfficeId in selectedOffices.Split(',').ToList())
                {
                    crnIds.Add(Convert.ToInt32(checkOfficeId));
                }
            }
            else if (string.IsNullOrEmpty(selectedOffices))
            {
                crnIds = (from crn in db.CRNs
                          join userCrn in db.UserCrns on crn.CRNID equals userCrn.CrnId
                          where userCrn.UserId == userInfo.USERID
                          select crn.CRNID).ToList();

            }

            List<UserReportViewModel> userReportItems = new List<UserReportViewModel>();

            if (startDateTime.HasValue && endDateTime.HasValue)
            {
                var listUsers = db.USERINFOes.Where(x => x.ISDELETED == 0 || x.ISDELETED == null);

                if (!string.IsNullOrEmpty(location))
                {
                    listUsers = listUsers.Where(x => x.LOCATION.Equals(location));
                }


                foreach (var user in listUsers.ToList())
                {
                    UserReportViewModel userReportViewModel = new UserReportViewModel();

                    userReportViewModel.UserName = user.USERNAME;
                    userReportViewModel.Location = user.LOCATION;

                    userReportViewModel.LVUpdatesCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 1 && x.Audit.RequestTypeId == 1 && x.UserId == user.USERID);
                    userReportViewModel.LVDatedownsCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 1 && x.Audit.RequestTypeId == 6 && x.UserId == user.USERID);
                    userReportViewModel.LVOtherCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 1 && x.Audit.RequestTypeId == 7 && x.UserId == user.USERID);

                    userReportViewModel.LVCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 1 && x.UserId == user.USERID);

                    userReportViewModel.PIUpdatesCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 2 && x.Audit.RequestTypeId == 1 && x.UserId == user.USERID);
                    userReportViewModel.PIDatedownsCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 2 && x.Audit.RequestTypeId == 6 && x.UserId == user.USERID);
                    userReportViewModel.PIOtherCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 2 && x.Audit.RequestTypeId == 7 && x.UserId == user.USERID);

                    userReportViewModel.PICount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 2 && x.UserId == user.USERID);

                    userReportViewModel.GIUpdatesCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 3 && x.Audit.RequestTypeId == 1 && x.UserId == user.USERID);
                    userReportViewModel.GIDatedownsCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 3 && x.Audit.RequestTypeId == 6 && x.UserId == user.USERID);
                    userReportViewModel.GIOtherCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 3 && x.Audit.RequestTypeId == 7 && x.UserId == user.USERID);

                    userReportViewModel.GICount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 3 && x.UserId == user.USERID);

                    userReportViewModel.StarterUpdatesCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 4 && x.Audit.RequestTypeId == 1 && x.UserId == user.USERID);
                    userReportViewModel.StarterDatedownsCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 4 && x.Audit.RequestTypeId == 6 && x.UserId == user.USERID);
                    userReportViewModel.StarterOtherCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 4 && x.Audit.RequestTypeId == 7 && x.UserId == user.USERID);

                    userReportViewModel.StarterCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 4 && x.UserId == user.USERID);

                    userReportViewModel.NotesUpdatesCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 5 && x.Audit.RequestTypeId == 1 && x.UserId == user.USERID);
                    userReportViewModel.NotesDatedownsCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 5 && x.Audit.RequestTypeId == 6 && x.UserId == user.USERID);
                    userReportViewModel.NotesOtherCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 5 && x.Audit.RequestTypeId == 7 && x.UserId == user.USERID);

                    userReportViewModel.NotesCount = GetAuditTimeEntryQuerable(crnIds).Count(x => x.StartTime > startDateTime && x.EndTime < endDateTime && x.AuditTaskId == 5 && x.UserId == user.USERID);

                    userReportItems.Add(userReportViewModel);
                }
            }

            SetOfficeFields(userInfo);
            ViewBag.Offices = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).ToList();

            return View(userReportItems);
        }

        private IQueryable<AuditTimeEntry> GetAuditTimeEntryQuerable(List<int> crnIds)
        {
            var querableRecordsSet = db.AuditTimeEntries.AsQueryable();

            if (crnIds.Any())
            {
                querableRecordsSet = querableRecordsSet.Where(x => crnIds.Contains(x.Audit.CrnId));
            }

            return querableRecordsSet;
        }


        public ActionResult HourTasks()
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];

            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            SetOfficeFields(userInfo);
            ViewBag.Offices = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).ToList();
            ViewBag.TaskId = new SelectList(db.AuditTasks, "Id", "Name");
            return View();
        }

        public JsonResult GetHourlyTaskdOrders(DateTime? startDateTime, DateTime? endDateTime, string location, int? taskId, string groupName, int? crnId, string selectedOffices)
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];

            List<int> crnIds = new List<int>();

            if (!string.IsNullOrEmpty(selectedOffices))
            {
                foreach (var checkOfficeId in selectedOffices.Split(',').ToList())
                {
                    crnIds.Add(Convert.ToInt32(checkOfficeId));
                }
            }
            else if (string.IsNullOrEmpty(selectedOffices))
            {
                crnIds = (from crn in db.CRNs
                          join userCrn in db.UserCrns on crn.CRNID equals userCrn.CrnId
                          where userCrn.UserId == userInfo.USERID
                          select crn.CRNID).ToList();

            }

            if (!startDateTime.HasValue && !endDateTime.HasValue && string.IsNullOrEmpty(location))
            {
                return null;
            }

            if ((endDateTime.Value - startDateTime.Value).Days > 12)
            {
                return null;
            }

            DateTime loopDate = startDateTime.Value;

            var allTasks = taskId.HasValue ? db.AuditTasks.Where(x => x.Id == taskId.Value).ToList() : db.AuditTasks.ToList();
            var allTypes = db.AuditRequestTypes.ToList();
            var userIds = location == "ALL" ? db.USERINFOes.Select(x => x.USERID).ToList() : db.USERINFOes.Where(x => x.LOCATION.Equals(location)).Select(x => x.USERID).ToList();

            var allTimeEntriesQuerable = db.AuditTimeEntries.Where(y => y.EndTime > loopDate && y.EndTime < endDateTime.Value && userIds.Contains(y.UserId));

            if (crnIds.Any())
            {
                allTimeEntriesQuerable = allTimeEntriesQuerable.Where(x => crnIds.Contains(x.Audit.CrnId));
            }

            var allTimeEntries = allTimeEntriesQuerable.ToList();

            List<string> rowItems = new List<string>();
            foreach (var task in allTasks)
            {
                List<string> propertyTokens = new List<string>();

                propertyTokens.Add(string.Format("\"Task\":\"{0}\"", task.Name));
                propertyTokens.Add(string.Format("\"Location\":\"{0}\"", location));

                foreach (var type in allTypes)
                {
                    propertyTokens.Add(string.Format("\"Product Type\":\"{1}\"", type.Name, type.Name));
                    while (loopDate < endDateTime.Value)
                    {
                        var addOneHour = loopDate.AddHours(1);

                        var count = allTimeEntries.Count(y => y.EndTime > loopDate && y.EndTime < addOneHour && y.AuditTaskId == task.Id && y.Audit.RequestTypeId == type.Id);

                        propertyTokens.Add(string.Format("\"{0}\":\"{1}\"", loopDate.ToString("hh''tt") + "-" + addOneHour.ToString("hh''tt"), count));
                        loopDate = addOneHour;
                    }

                    var taskTotal = allTimeEntries.Count(y => y.EndTime > startDateTime.Value && y.EndTime < loopDate && y.AuditTaskId == task.Id && y.Audit.RequestTypeId == type.Id);
                    propertyTokens.Add(string.Format("\"{0}\":\"{1}\"", "Total", taskTotal));
                    loopDate = startDateTime.Value;

                    rowItems.Add("{" + string.Join(",", propertyTokens) + "}");
                }
            }

            return new JsonResult()
            {
                Data = "[" + string.Join(",", rowItems) + "]",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
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

    public class AuditUiFilter
    {
        public int? CrnId
        {
            get; set;
        }

        public int? TaskId
        {
            get; set;
        }

        public int? RequestTypeId
        {
            get; set;
        }
    }

    public class RejectOrderInfo
    {
        public int SerialNumber { get; set; }

        public string OrderNo { get; set; }

        public string OfficeName { get; set; }

        public string OrderType { get; set; }

        public string RecievedDateTime { get; set; }

        public string RejectedBy { get; set; }

        public string RejectedDateTime { get; set; }

        public string Reason { get; set; }
    }

    public class OrderCompletedInfo
    {
        public int SerialNumber { get; set; }
        public string OrderNo { get; set; }
        public string OfficeName { get; set; }
        public string OrderType { get; set; }
        public string UploadedBy { get; set; }
        public string RecievedDateTime { get; set; }
        public string UploadDateTime { get; set; }
    }

    public class OrderUploadViewModel
    {
        public List<OrderUploadModel> Items { get; set; }

        public bool AnyErrors { get; set; }
    }

    public class OrderUploadModel
    {
        public string Message { get; set; }
        public string OrderNumber { get; set; }
        public string Customer { get; set; }
        public string SenderEmailAddress { get; set; }
        public string ProductType { get; set; }
        public string RequestBy { get; set; }
        public string ClientInstructions { get; set; }
        public string County { get; set; }
        public string ReceivedDateTime { get; set; }
        public string FileName { get; set; }
    }

    public class RushOrderReportViewModel
    {
        public string OfficeName { get; set; }
        public int CrnId { get; set; }
        public int Count { get; set; }
    }
    public class Rush
    {
        public Rush(bool key, string value)
        {
            Key = key;
            Value = value;
        }

        public bool Key { get; set; }
        public string Value { get; set; }
    }

    public class AuditError
    {
        public int SelectedCategory { get; set; }
        public string SelectedCategoryName { get; set; }
        public List<AuditErrorTypeLite> ErrorTypes { get; set; }
        public List<int> SelectedType { get; set; }
        public string Comments { get; set; }
        public bool IsCritical { get; set; }

        public string TaskName { get; set; }
    }

    public class AuditErrorTypeLite
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string IsCriticalText { get; set; }
    }
}
