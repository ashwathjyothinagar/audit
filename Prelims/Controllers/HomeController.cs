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
                            $"SELECT Id, OrderNo, MoreThanCodeValue, FFAssignedDate, TOT, IsRushOrder FROM TitleOrders WHERE Id IN ({idsStr})"
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
                            $"SELECT Id, OrderNo, MoreThanCodeValue, FFAssignedDate, TOT, IsRushOrder FROM TitleOrders WHERE OrderNo IN ({ordersStr})"
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
                    IsRushOrder = e.IsRushOrder || (titleInfo != null && titleInfo.IsRushOrder.HasValue && titleInfo.IsRushOrder.Value),
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

            homeViewModel.InprogressAudits = inprogressList.OrderByDescending(x => x.IsRushOrder).ThenByDescending(x => x.TimeTaken).ToList();
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

        public ActionResult ShowActiveUsers()
        {
            List<UserStatusReportModel> userStatus = new List<UserStatusReportModel>();

            var userTimeEntries = db.UserTimeEntries.Where(x => !x.EndTime.HasValue).ToList();
            var listOfActiveUsers = userTimeEntries.Select(x => x.UserId).ToList();
            var userInfos = db.USERINFOes.Where(x => listOfActiveUsers.Contains(x.USERID)).ToList();

            var inprogressEntries = db.AuditTimeEntries
                .Include(x => x.Audit)
                .Include(x => x.Audit.CRN)
                .Include(x => x.AuditTask)
                .Where(x => x.IsInprogress)
                .ToList();

            var today = DateTime.Today;

            foreach (var userInfo in userInfos)
            {
                var userTimeEntry = userTimeEntries.FirstOrDefault(x => x.UserId == userInfo.USERID);
                var statusItem = new UserStatusReportModel()
                {
                    UserName = userInfo.USERNAME,
                    Status = "Available",
                    Location = userInfo.LOCATION,
                };

                if (userTimeEntry != null)
                {
                    statusItem.LoggedInTime = userTimeEntry.StartTime;
                    statusItem.UserTimeEntryId = userTimeEntry.Id;
                }

                // Check if user is currently on break
                UserBreakEntry activeBreak = null;
                try
                {
                    activeBreak = db.UserBreakEntries
                        .FirstOrDefault(b => b.UserId == userInfo.USERID && b.EndTime == null);
                }
                catch { }

                if (activeBreak != null)
                {
                    statusItem.Status = "On Break";
                    statusItem.IsOnBreak = true;
                    statusItem.BreakStartTime = activeBreak.StartTime;
                    statusItem.BreakReason = activeBreak.BreakReason;

                    var currentBreakDuration = DateTime.Now - activeBreak.StartTime;
                    statusItem.CurrentBreakDuration = (int)Math.Ceiling(currentBreakDuration.TotalMinutes);
                }
                else
                {
                    // Check for regular work status in Audit
                    var timeEntry = inprogressEntries.FirstOrDefault(x => x.UserId == userInfo.USERID);
                    if (timeEntry != null && timeEntry.Audit != null)
                    {
                        statusItem.Status = "Order Inprocess: " + timeEntry.Audit.OrderNo;
                        statusItem.Office = timeEntry.Audit.CRN != null ? timeEntry.Audit.CRN.CRNNAME : "";
                        statusItem.Task = timeEntry.AuditTask != null ? timeEntry.AuditTask.Name : "";
                    }
                    else
                    {
                        var lastTimeEntry = db.AuditTimeEntries
                            .Include(x => x.Audit)
                            .Include(x => x.Audit.CRN)
                            .Include(x => x.AuditTask)
                            .Where(x => x.UserId == userInfo.USERID && !x.IsInprogress)
                            .OrderByDescending(x => x.Id)
                            .FirstOrDefault();

                        if (lastTimeEntry != null && lastTimeEntry.Audit != null)
                        {
                            statusItem.Status = "Available";
                            statusItem.LastOrderCompletedDate = lastTimeEntry.EndTime;
                            statusItem.Office = lastTimeEntry.Audit.CRN != null ? lastTimeEntry.Audit.CRN.CRNNAME : "";
                            statusItem.Task = lastTimeEntry.AuditTask != null ? lastTimeEntry.AuditTask.Name : "";
                        }
                    }
                }

                // Calculate today's break statistics for this user
                try
                {
                    var todayBreaks = db.UserBreakEntries
                        .Where(b => b.UserId == userInfo.USERID &&
                                   DbFunctions.TruncateTime(b.StartTime) == today &&
                                   b.EndTime.HasValue)
                        .ToList();

                    statusItem.TodayBreakCount = todayBreaks.Count;
                    statusItem.TodayTotalBreakMinutes = todayBreaks
                        .Where(b => b.ActualDurationMinutes.HasValue)
                        .Sum(b => b.ActualDurationMinutes.Value);

                    if (activeBreak != null)
                    {
                        statusItem.TodayBreakCount += 1;
                    }
                }
                catch { }

                userStatus.Add(statusItem);
            }

            return View(userStatus);
        }

        public JsonResult GetActiveUsersRealTime()
        {
            try
            {
                var data = GetActiveUsersDataForApi();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private object GetActiveUsersDataForApi()
        {
            List<UserStatusReportModel> userStatus = new List<UserStatusReportModel>();

            var userTimeEntries = db.UserTimeEntries.Where(x => !x.EndTime.HasValue).ToList();
            var listOfActiveUsers = userTimeEntries.Select(x => x.UserId).ToList();
            var userInfos = db.USERINFOes.Where(x => listOfActiveUsers.Contains(x.USERID)).ToList();

            var inprogressEntries = db.AuditTimeEntries
                .Include(x => x.Audit)
                .Include(x => x.Audit.CRN)
                .Include(x => x.AuditTask)
                .Where(x => x.IsInprogress)
                .ToList();

            var today = DateTime.Today;

            foreach (var userInfo in userInfos)
            {
                var userTimeEntry = userTimeEntries.FirstOrDefault(x => x.UserId == userInfo.USERID);
                var statusItem = new UserStatusReportModel()
                {
                    UserName = userInfo.USERNAME,
                    Status = "Available",
                    Location = userInfo.LOCATION,
                };

                if (userTimeEntry != null)
                {
                    statusItem.LoggedInTime = userTimeEntry.StartTime;
                    statusItem.UserTimeEntryId = userTimeEntry.Id;
                }

                UserBreakEntry activeBreak = null;
                try
                {
                    activeBreak = db.UserBreakEntries
                        .FirstOrDefault(b => b.UserId == userInfo.USERID && b.EndTime == null);
                }
                catch { }

                if (activeBreak != null)
                {
                    statusItem.Status = "On Break";
                    statusItem.IsOnBreak = true;
                    statusItem.BreakStartTime = activeBreak.StartTime;
                    statusItem.BreakReason = activeBreak.BreakReason;

                    var currentBreakDuration = DateTime.Now - activeBreak.StartTime;
                    statusItem.CurrentBreakDuration = (int)Math.Ceiling(currentBreakDuration.TotalMinutes);
                }
                else
                {
                    var timeEntry = inprogressEntries.FirstOrDefault(x => x.UserId == userInfo.USERID);
                    if (timeEntry != null && timeEntry.Audit != null)
                    {
                        statusItem.Status = "Order Inprocess: " + timeEntry.Audit.OrderNo;
                        statusItem.Office = timeEntry.Audit.CRN != null ? timeEntry.Audit.CRN.CRNNAME : "";
                        statusItem.Task = timeEntry.AuditTask != null ? timeEntry.AuditTask.Name : "";
                    }
                    else
                    {
                        var lastTimeEntry = db.AuditTimeEntries
                            .Include(x => x.Audit)
                            .Include(x => x.Audit.CRN)
                            .Include(x => x.AuditTask)
                            .Where(x => x.UserId == userInfo.USERID && !x.IsInprogress)
                            .OrderByDescending(x => x.Id)
                            .FirstOrDefault();

                        if (lastTimeEntry != null && lastTimeEntry.Audit != null)
                        {
                            statusItem.Status = "Available";
                            statusItem.LastOrderCompletedDate = lastTimeEntry.EndTime;
                            statusItem.Office = lastTimeEntry.Audit.CRN != null ? lastTimeEntry.Audit.CRN.CRNNAME : "";
                            statusItem.Task = lastTimeEntry.AuditTask != null ? lastTimeEntry.AuditTask.Name : "";
                        }
                    }
                }

                try
                {
                    var todayBreaks = db.UserBreakEntries
                        .Where(b => b.UserId == userInfo.USERID &&
                                   DbFunctions.TruncateTime(b.StartTime) == today &&
                                   b.EndTime.HasValue)
                        .ToList();

                    statusItem.TodayBreakCount = todayBreaks.Count;
                    statusItem.TodayTotalBreakMinutes = todayBreaks
                        .Where(b => b.ActualDurationMinutes.HasValue)
                        .Sum(b => b.ActualDurationMinutes.Value);

                    if (activeBreak != null)
                    {
                        statusItem.TodayBreakCount += 1;
                    }
                }
                catch { }

                userStatus.Add(statusItem);
            }

            var chennaiList = userStatus
                .Where(x => x.Location == "CHENNAI")
                .Select(u => new
                {
                    u.UserName,
                    u.Status,
                    u.Location,
                    LoggedInTime = u.LoggedInTime.HasValue ? u.LoggedInTime.Value.ToString("o") : null,
                    u.UserTimeEntryId,
                    u.Office,
                    u.Task,
                    LastOrderCompletedDate = u.LastOrderCompletedDate.HasValue ? u.LastOrderCompletedDate.Value.ToString("o") : null,
                    u.IsOnBreak,
                    BreakStartTime = u.BreakStartTime.HasValue ? u.BreakStartTime.Value.ToString("o") : null,
                    u.BreakReason,
                    u.CurrentBreakDuration,
                    u.TodayBreakCount,
                    u.TodayTotalBreakMinutes
                })
                .ToList();

            var hospetList = userStatus
                .Where(x => x.Location == "HOSPET")
                .Select(u => new
                {
                    u.UserName,
                    u.Status,
                    u.Location,
                    LoggedInTime = u.LoggedInTime.HasValue ? u.LoggedInTime.Value.ToString("o") : null,
                    u.UserTimeEntryId,
                    u.Office,
                    u.Task,
                    LastOrderCompletedDate = u.LastOrderCompletedDate.HasValue ? u.LastOrderCompletedDate.Value.ToString("o") : null,
                    u.IsOnBreak,
                    BreakStartTime = u.BreakStartTime.HasValue ? u.BreakStartTime.Value.ToString("o") : null,
                    u.BreakReason,
                    u.CurrentBreakDuration,
                    u.TodayBreakCount,
                    u.TodayTotalBreakMinutes
                })
                .ToList();

            return new
            {
                chennai = chennaiList,
                hospet = hospetList,
                timestamp = DateTime.Now.ToString("o")
            };
        }

        public JsonResult GetShifts()
        {
            JsonResult jsonResult = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            try
            {
                jsonResult.Data = db.Shifts.Select(x => new { ShiftId = x.Id, ShiftName = x.ShiftName }).ToList();
            }
            catch
            {
                jsonResult.Data = new List<object>();
            }
            return jsonResult;
        }

        public ActionResult WorkTimeTracker()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetWorkTimeTracker(DateTime? fromDate, DateTime? toDate, string userIds, int page = 1, int pageSize = 15)
        {
            if (!fromDate.HasValue || !toDate.HasValue || string.IsNullOrWhiteSpace(userIds))
            {
                return Json(new { error = "Missing required filters" }, JsonRequestBehavior.AllowGet);
            }

            DateTime from = fromDate.Value.Date;
            DateTime to = toDate.Value.Date;

            var ids = userIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s =>
            {
                int v; int.TryParse(s, out v); return v;
            }).Where(x => x > 0).ToList();

            var users = db.USERINFOes.Where(u => ids.Contains(u.USERID)).ToList();

            var results = new List<object>();

            Func<double, string> fmtHMS = secs =>
            {
                var s = (long)secs;
                var h = s / 3600;
                var m = (s % 3600) / 60;
                return string.Format("{0:D2}:{1:D2}", h, m);
            };

            foreach (var u in users)
            {
                for (DateTime d = from; d <= to; d = d.AddDays(1))
                {
                    var timeEntries = db.UserTimeEntries
                        .Where(te => te.UserId == u.USERID && te.EndTime.HasValue
                                     && DbFunctions.TruncateTime(te.StartTime) == d)
                        .ToList();

                    double totalLoggedSeconds = 0;
                    foreach (var te in timeEntries)
                    {
                        var end = te.EndTime.Value;
                        var start = te.StartTime;
                        totalLoggedSeconds += (end - start).TotalSeconds;
                    }

                    int breakMinutes = 0;
                    int breakCount = 0;
                    try
                    {
                        var breaks = db.UserBreakEntries
                            .Where(b => b.UserId == u.USERID && DbFunctions.TruncateTime(b.StartTime) == d && b.EndTime.HasValue)
                            .ToList();

                        foreach (var b in breaks)
                        {
                            if (b.ActualDurationMinutes.HasValue)
                                breakMinutes += b.ActualDurationMinutes.Value;
                            else
                                breakMinutes += b.PlannedDurationMinutes;
                        }
                        breakCount = breaks.Count;
                    }
                    catch { }

                    double loggedWithBreakSeconds = totalLoggedSeconds;
                    double loggedNetSeconds = Math.Max(0, totalLoggedSeconds - (breakMinutes * 60));

                    string reportingManager = "";
                    if (u.ReportingManagerId.HasValue)
                    {
                        var mgr = db.USERINFOes.Find(u.ReportingManagerId.Value);
                        reportingManager = mgr != null ? (mgr.FULLNAME ?? mgr.USERNAME) : "";
                    }

                    results.Add(new
                    {
                        Date = d.ToString("yyyy-MM-dd"),
                        EmployeeCode = u.USERNAME,
                        FullName = u.FULLNAME ?? u.USERNAME,
                        Email = u.EMAIL,
                        ReportingManager = reportingManager,
                        TotalLoggedHours = fmtHMS(totalLoggedSeconds),
                        BreakDurationMinutes = breakMinutes,
                        BreakCount = breakCount,
                        LoggedWithBreak = fmtHMS(loggedWithBreakSeconds),
                        LoggedNet = fmtHMS(loggedNetSeconds),
                        UserId = u.USERID
                    });
                }
            }

            int total = results.Count;
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 15;
            var paged = results.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Json(new { total = total, page = page, pageSize = pageSize, data = paged }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileResult ExportWorkTimeTracker(DateTime? fromDate, DateTime? toDate, string userIds)
        {
            if (!fromDate.HasValue || !toDate.HasValue || string.IsNullOrWhiteSpace(userIds))
            {
                throw new HttpException(400, "Missing required filters");
            }

            DateTime from = fromDate.Value.Date;
            DateTime to = toDate.Value.Date;

            var ids = userIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s =>
            {
                int v; int.TryParse(s, out v); return v;
            }).Where(x => x > 0).ToList();

            var users = db.USERINFOes.Where(u => ids.Contains(u.USERID)).ToList();

            Func<double, string> fmtHMS = secs =>
            {
                var s = (long)secs;
                var h = s / 3600;
                var m = (s % 3600) / 60;
                return string.Format("{0:D2}:{1:D2}", h, m);
            };

            var rows = new List<string>();
            rows.Add("Date,EmployeeCode,FullName,Email,ReportingManager,TotalLoggedHours,BreakDurationMinutes,BreakCount,BreakDetails,LoggedWithBreak,LoggedNet");

            foreach (var u in users)
            {
                for (DateTime d = from; d <= to; d = d.AddDays(1))
                {
                    var timeEntries = db.UserTimeEntries
                        .Where(te => te.UserId == u.USERID && te.EndTime.HasValue
                                     && DbFunctions.TruncateTime(te.StartTime) == d)
                        .ToList();

                    double totalLoggedSeconds = 0;
                    foreach (var te in timeEntries)
                    {
                        var end = te.EndTime.Value;
                        var start = te.StartTime;
                        totalLoggedSeconds += (end - start).TotalSeconds;
                    }

                    int breakMinutes = 0;
                    var breakDetailsList = new List<string>();
                    int breakCount = 0;
                    try
                    {
                        var breaks = db.UserBreakEntries
                            .Where(b => b.UserId == u.USERID && DbFunctions.TruncateTime(b.StartTime) == d && b.EndTime.HasValue)
                            .OrderBy(b => b.StartTime)
                            .ToList();

                        foreach (var b in breaks)
                        {
                            int dur = b.ActualDurationMinutes.HasValue ? b.ActualDurationMinutes.Value : b.PlannedDurationMinutes;
                            breakMinutes += dur;

                            var startStr = b.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                            var endStr = b.EndTime.HasValue ? b.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                            var reason = string.IsNullOrWhiteSpace(b.BreakReason) ? "(No reason)" : b.BreakReason.Replace('\r', ' ').Replace('\n', ' ');

                            var detail = string.Format("{0} - {1} | {2} Minutes | {3}", startStr, endStr, dur, reason);
                            breakDetailsList.Add(detail);
                        }
                        breakCount = breaks.Count;
                    }
                    catch { }

                    double loggedWithBreakSeconds = totalLoggedSeconds;
                    double loggedNetSeconds = Math.Max(0, totalLoggedSeconds - (breakMinutes * 60));

                    string reportingManager = "";
                    if (u.ReportingManagerId.HasValue)
                    {
                        var mgr = db.USERINFOes.Find(u.ReportingManagerId.Value);
                        reportingManager = mgr != null ? (mgr.FULLNAME ?? mgr.USERNAME) : "";
                    }

                    Func<string, string> esc = v =>
                    {
                        if (v == null) return "";
                        var needsQuote = v.Contains(",") || v.Contains("\"") || v.Contains('\n') || v.Contains('\r');
                        var safe = v.Replace("\"", "\"\"");
                        return needsQuote ? '"' + safe + '"' : safe;
                    };

                    var breakDetailsJoined = breakDetailsList.Count > 0 ? string.Join("\r\n", breakDetailsList) : string.Empty;

                    var line = string.Join(",", new string[] {
                        esc(d.ToString("yyyy-MM-dd")),
                        esc(u.USERNAME),
                        esc(u.FULLNAME ?? u.USERNAME),
                        esc(u.EMAIL),
                        esc(reportingManager),
                        esc(fmtHMS(totalLoggedSeconds)),
                        esc(breakMinutes.ToString()),
                        esc(breakCount.ToString()),
                        esc(breakDetailsJoined),
                        esc(fmtHMS(loggedWithBreakSeconds)),
                        esc(fmtHMS(loggedNetSeconds))
                    });

                    rows.Add(line);
                }
            }

            var csvBody = string.Join("\r\n", rows);
            var bom = new byte[] { 0xEF, 0xBB, 0xBF };
            var bodyBytes = System.Text.Encoding.UTF8.GetBytes(csvBody);
            var bytes = new byte[bom.Length + bodyBytes.Length];
            Buffer.BlockCopy(bom, 0, bytes, 0, bom.Length);
            Buffer.BlockCopy(bodyBytes, 0, bytes, bom.Length, bodyBytes.Length);

            var fileName = "WorkTimeTracker_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
            return File(bytes, "text/csv", fileName);
        }

        [HttpGet]
        public JsonResult GetUserBreakDetails(int userId, DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                return Json(new { error = "Missing date filters" }, JsonRequestBehavior.AllowGet);
            }

            DateTime from = fromDate.Value.Date;
            DateTime to = toDate.Value.Date;

            try
            {
                var breakEntities = db.UserBreakEntries
                    .Where(b => b.UserId == userId && DbFunctions.TruncateTime(b.StartTime) >= from && DbFunctions.TruncateTime(b.StartTime) <= to)
                    .OrderBy(b => b.StartTime)
                    .ToList();

                var result = breakEntities.Select(b => new
                {
                    StartTime = b.StartTime.ToString("o"),
                    EndTime = b.EndTime.HasValue ? b.EndTime.Value.ToString("o") : null,
                    PlannedDurationMinutes = b.PlannedDurationMinutes,
                    ActualDurationMinutes = b.ActualDurationMinutes,
                    BreakReason = b.BreakReason,
                    CreatedDate = b.CreatedDate.ToString("o")
                }).ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new object[0], JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetUsers()
        {
            var users = db.USERINFOes
                .Where(u => !u.ISDELETED.HasValue || u.ISDELETED.Value != 1)
                .Where(u => u.USERNAME != "Admin")
                .OrderBy(u => u.USERNAME)
                .Select(u => new
                {
                    id = u.USERID,
                    name = u.USERNAME,
                    location = u.LOCATION,
                    reportingManagerId = u.ReportingManagerId
                }).ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetReportingManagers()
        {
            var managerIds = db.USERINFOes
                .Where(u => u.ReportingManagerId.HasValue)
                .Select(u => u.ReportingManagerId.Value)
                .Distinct()
                .ToList();

            var managers = db.USERINFOes
                .Where(u => managerIds.Contains(u.USERID))
                .OrderBy(u => u.USERNAME)
                .Select(u => new
                {
                    id = u.USERID,
                    name = u.USERNAME
                }).ToList();

            return Json(managers, JsonRequestBehavior.AllowGet);
        }
    }
}
