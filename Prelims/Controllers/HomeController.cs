using Prelims.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using Prelims.Models.Reports;

namespace Prelims.Controllers
{
    [Authorize(Roles ="Admin")]
    public class HomeController : Controller
    {
        private FntEntities db = new FntEntities();

        public ActionResult Index()
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            HomeViewModel homeViewModel = new HomeViewModel();

            // 1. Fetch Inprogress Audit Time Entries
            var inprogressEntries = (from tote in db.AuditTimeEntries.AsNoTracking()
                                     join a in db.Audits.AsNoTracking() on tote.AuditId equals a.Id
                                     join cr in db.CRNs.AsNoTracking() on a.CrnId equals cr.CRNID
                                     join art in db.AuditRequestTypes.AsNoTracking() on a.RequestTypeId equals art.Id
                                     join atask in db.AuditTasks.AsNoTracking() on a.TaskId equals atask.Id
                                     join astatus in db.AuditStatuses.AsNoTracking() on a.StatusId equals astatus.Id into astatusJoin
                                     from astatus in astatusJoin.DefaultIfEmpty()
                                     join ui in db.USERINFOes.AsNoTracking() on tote.UserId equals ui.USERID
                                     where tote.IsInprogress
                                     select new
                                     {
                                         AuditId = a.Id,
                                         TitleOrderId = a.TitleOrderId,
                                         OrderNo = a.OrderNo,
                                         CountyName = a.County,
                                         CRNName = cr.CRNNAME,
                                         RequestTypeName = art.Name,
                                         TaskName = atask.Name,
                                         IsRushOrder = a.IsRushOrder,
                                         StatusName = astatus != null ? astatus.Name : "",
                                         ReceivedDateTime = a.DateCreated,
                                         UserName = ui.USERNAME,
                                         Location = ui.LOCATION,
                                         StartTime = tote.StartTime
                                     }).ToList();

            var titleOrderIds = inprogressEntries.Where(x => x.TitleOrderId.HasValue && x.TitleOrderId.Value > 0)
                                                 .Select(x => x.TitleOrderId.Value).Distinct().ToList();
            var orderNos = inprogressEntries.Where(x => !string.IsNullOrEmpty(x.OrderNo))
                                            .Select(x => x.OrderNo).Distinct().ToList();

            var byId = new Dictionary<int, TitleOrderInfoDto>();
            var byOrderNo = new Dictionary<string, TitleOrderInfoDto>(StringComparer.OrdinalIgnoreCase);

            if (titleOrderIds.Count > 0)
            {
                try
                {
                    for (int i = 0; i < titleOrderIds.Count; i += 500)
                    {
                        var chunk = titleOrderIds.Skip(i).Take(500).ToList();
                        string idsStr = string.Join(",", chunk);
                        var results = db.Database.SqlQuery<TitleOrderInfoDto>(
                            $"SELECT Id, OrderNo, MoreThanCodeValue, FFAssignedDate, TOT FROM TitleOrders WHERE Id IN ({idsStr})"
                        ).ToList();
                        foreach (var r in results)
                        {
                            if (!byId.ContainsKey(r.Id)) byId[r.Id] = r;
                            if (!string.IsNullOrEmpty(r.OrderNo) && !byOrderNo.ContainsKey(r.OrderNo)) byOrderNo[r.OrderNo] = r;
                        }
                    }
                }
                catch { }
            }

            var remainingOrderNos = orderNos.Where(no => !byOrderNo.ContainsKey(no)).Distinct().ToList();
            if (remainingOrderNos.Count > 0)
            {
                try
                {
                    for (int i = 0; i < remainingOrderNos.Count; i += 500)
                    {
                        var chunk = remainingOrderNos.Skip(i).Take(500).Select(x => "'" + x.Replace("'", "''") + "'").ToList();
                        string ordersStr = string.Join(",", chunk);
                        var results = db.Database.SqlQuery<TitleOrderInfoDto>(
                            $"SELECT Id, OrderNo, MoreThanCodeValue, FFAssignedDate, TOT FROM TitleOrders WHERE OrderNo IN ({ordersStr})"
                        ).ToList();
                        foreach (var r in results)
                        {
                            if (!byId.ContainsKey(r.Id)) byId[r.Id] = r;
                            if (!string.IsNullOrEmpty(r.OrderNo) && !byOrderNo.ContainsKey(r.OrderNo)) byOrderNo[r.OrderNo] = r;
                        }
                    }
                }
                catch { }
            }

            var inprogressList = new List<AuditTimeEntryViewModel>();
            foreach (var e in inprogressEntries)
            {
                TitleOrderInfoDto titleInfo = null;
                if (e.TitleOrderId.HasValue && byId.TryGetValue(e.TitleOrderId.Value, out titleInfo))
                {
                }
                else if (!string.IsNullOrEmpty(e.OrderNo) && byOrderNo.TryGetValue(e.OrderNo, out titleInfo))
                {
                }

                var diff = DateTime.Now - e.StartTime;
                if (diff.TotalSeconds < 0) diff = TimeSpan.Zero;
                string timeTakenStr = $"{(int)diff.TotalHours:D2}:{diff.Minutes:D2}";

                inprogressList.Add(new AuditTimeEntryViewModel
                {
                    Id = e.AuditId,
                    OrderNo = e.OrderNo,
                    StartTime = e.StartTime,
                    CountyName = e.CountyName,
                    CRNName = e.CRNName,
                    RequestTypeName = e.RequestTypeName,
                    TaskName = e.TaskName,
                    IsRushOrder = e.IsRushOrder,
                    StatusName = e.StatusName,
                    ReceivedDateTime = e.ReceivedDateTime,
                    FFAssignedDate = titleInfo != null ? titleInfo.FFAssignedDate : null,
                    TOT = titleInfo != null ? titleInfo.TOT : null,
                    UserName = e.UserName,
                    CodesCount = titleInfo != null ? titleInfo.MoreThanCodeValue : null,
                    TimeTaken = timeTakenStr,
                    Location = string.IsNullOrWhiteSpace(e.Location) ? "CHENNAI" : e.Location.ToUpper().Trim()
                });
            }

            homeViewModel.InprogressAudits = inprogressList.OrderByDescending(x => x.TimeTaken).ToList();
            homeViewModel.IsAllowedToViewInprogressOrders = userInfo.IsAllowedToViewInprogressOrders || IsAuditFLM(userInfo) || userInfo.ISREVIEWER;


            List<LiveStatusReportViewModel> liveStatusItems = new List<LiveStatusReportViewModel>();

            var groupByCrn = (from k in db.Audits.Where(x => x.StatusId == 3 && x.StatusId != 4 && x.StatusId != 5)
                              group k by k.CrnId into grp
                              select new { key = grp.Key, Count = grp.Count() }).ToList();



            foreach (var crn in db.CRNs.Where(x => x.IsVisibleInMainApplication == true))
            {
                LiveStatusReportViewModel liveStatusViewModel = new LiveStatusReportViewModel();

                liveStatusViewModel.OfficeName = crn.CRNNAME;
                liveStatusViewModel.GroupName = crn.GroupName;

                var grpItem = groupByCrn.FirstOrDefault(x => x.key == crn.CRNID);
                if (grpItem != null)
                {
                    liveStatusViewModel.HoldCount = grpItem.Count;
                }else
                {
                    liveStatusViewModel.HoldCount = 0;
                }

                liveStatusViewModel.TaxesBothCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 6 && x.RequestTypeId == 4);
                liveStatusViewModel.TaxesBuyerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 6 && x.RequestTypeId == 2);
                liveStatusViewModel.TaxesSellerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 6 && x.RequestTypeId == 1);
                liveStatusViewModel.TaxesCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 6);

                liveStatusViewModel.LVBothCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId==crn.CRNID && x.TaskId == 1 && x.RequestTypeId == 4);
                liveStatusViewModel.LVBuyerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 1 && x.RequestTypeId == 2);
                liveStatusViewModel.LVSellerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 1 && x.RequestTypeId == 1);
                liveStatusViewModel.LVCount= db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 1);

                liveStatusViewModel.PIBothCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 2 && x.RequestTypeId == 4);
                liveStatusViewModel.PIBuyerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 2 && x.RequestTypeId == 2);
                liveStatusViewModel.PISellerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 2 && x.RequestTypeId == 1);
                liveStatusViewModel.PICount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 2);

                liveStatusViewModel.GIBothCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 3 && x.RequestTypeId == 4);
                liveStatusViewModel.GIBuyerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 3 && x.RequestTypeId == 2);
                liveStatusViewModel.GISellerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 3 && x.RequestTypeId == 1);
                liveStatusViewModel.GICount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 3);

                liveStatusViewModel.StarterBothCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 4 && x.RequestTypeId == 4);
                liveStatusViewModel.StarterBuyerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 4 && x.RequestTypeId == 2);
                liveStatusViewModel.StarterSellerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 4 && x.RequestTypeId == 1);
                liveStatusViewModel.StarterCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 4);

                liveStatusViewModel.NotesBothCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 5 && x.RequestTypeId == 4);
                liveStatusViewModel.NotesBuyerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 5 && x.RequestTypeId == 2);
                liveStatusViewModel.NotesSellerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 5 && x.RequestTypeId == 1);
                liveStatusViewModel.NotesCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 5);

                liveStatusItems.Add(liveStatusViewModel);
            }
            homeViewModel.LiveStatusItems = liveStatusItems;

            var inprogressTitleOrderIds = db.AuditTimeEntries.Where(x => x.IsInprogress).Select(x => x.AuditId).ToList();

            int? userID = userInfo.USERID;
            var myOrderList = (from k in db.Audits
                               where k.StatusId != 3 && k.StatusId != 4 && k.StatusId != 5 && !inprogressTitleOrderIds.Any(x => x == k.Id) && k.UserAssigned == userID
                               select new MyOrder
                               {
                                   COUNTYNAME = k.County,
                                   CRNDISPLAYNAME = k.CRN.CRNDISPLAYNAME,
                                   TitleOrderRequestTypeName = k.AuditRequestType.Name,
                                   IsRushOrder = k.IsRushOrder,
                                   TitleOrderTaskName = k.AuditTask.Name,
                                   ReceivedDateTime = k.DateCreated,
                                   OrderNo = k.OrderNo,
                                   Id = k.Id
                               }).OrderByDescending(x => x.IsRushOrder).ToList();

            homeViewModel.MyOrders = myOrderList.ToList();

            AuditTimeEntry timeEntry = db.AuditTimeEntries.FirstOrDefault(x => x.IsInprogress && x.UserId == userInfo.USERID);

            if (timeEntry != null)
            {
                ViewBag.InprogressTitleOrderId = timeEntry.AuditId;
            }

            // Login/Logout active time for dashboard (from UserTimeEntries table)
            var activeSession = db.UserTimeEntries.AsNoTracking()
                .Where(x => x.UserId == userInfo.USERID && !x.EndTime.HasValue)
                .OrderByDescending(x => x.StartTime)
                .FirstOrDefault();

            if (activeSession != null)
            {
                ViewBag.IsLoggedIn = true;
                ViewBag.ActiveTime = DateTime.Now - activeSession.StartTime;
                ViewBag.ActiveEntryId = activeSession.Id;
            }
            else
            {
                ViewBag.IsLoggedIn = false;
            }

            return View(homeViewModel);
        }

        private bool IsAuditFLM(USERINFO userInfo)
        {
            if (userInfo == null || !userInfo.TitleOrderRoleId.HasValue) return false;
            try
            {
                string sql = @"
                    SELECT COUNT(1)
                    FROM TitleOrderRoles r
                    INNER JOIN TitleOrderRoleTasks rt ON r.Id = rt.RoleId
                    WHERE r.Id = @p0 
                      AND r.IsAllowedToViewInprogressOrders = 1 
                      AND rt.TaskId = 3";
                int count = db.Database.SqlQuery<int>(sql, userInfo.TitleOrderRoleId.Value).FirstOrDefault();
                return count > 0;
            }
            catch
            {
                var allowedRoleIds = new HashSet<int> { 4, 23, 24, 26, 28, 35, 37, 40, 42, 43, 44, 46 };
                return allowedRoleIds.Contains(userInfo.TitleOrderRoleId.Value);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
