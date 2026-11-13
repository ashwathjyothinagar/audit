using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Prelims.Models;
using Prelims.Models.Reports;

namespace Prelims.Controllers
{
    public partial class AuditsController
    {
        #region Production Metrics
        [Authorize]
        public ActionResult ProductionMetrics()
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }
            SetOfficeFields(userInfo);
            ViewBag.Offices = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).ToList();
            ViewBag.Locations = db.USERINFOes.Where(x => x.LOCATION != null && x.LOCATION != "").Select(x => x.LOCATION).Distinct().OrderBy(x => x).ToList();
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetProductionMetricsData(string startDate, string endDate, string reportType, string selectedOffices, int? userId, string selectedLocation, string startTime, string endTime)
        {
            try
            {
                DateTime dtStart = string.IsNullOrEmpty(startDate) ? DateTime.Today : DateTime.ParseExact(startDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime dtEnd = string.IsNullOrEmpty(endDate) ? DateTime.Today.AddDays(1) : DateTime.ParseExact(endDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).AddDays(1);

                if (!string.IsNullOrEmpty(startTime))
                {
                    TimeSpan tsStart;
                    if (TimeSpan.TryParse(startTime, out tsStart))
                    {
                        dtStart = dtStart.Date.Add(tsStart);
                    }
                }
                if (!string.IsNullOrEmpty(endTime))
                {
                    TimeSpan tsEnd;
                    if (TimeSpan.TryParse(endTime, out tsEnd))
                    {
                        DateTime endDateParsed = string.IsNullOrEmpty(endDate) ? DateTime.Today : DateTime.ParseExact(endDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        dtEnd = endDateParsed.Date.Add(tsEnd);
                    }
                }

                int workingDays = 0;
                {
                    DateTime countDate = dtStart.Date;
                    DateTime countEnd = dtEnd.Date;
                    if (dtEnd.TimeOfDay == TimeSpan.Zero && countEnd > countDate)
                        countEnd = countEnd.AddDays(-1);
                    while (countDate <= countEnd)
                    {
                        if (countDate.DayOfWeek != DayOfWeek.Saturday && countDate.DayOfWeek != DayOfWeek.Sunday)
                            workingDays++;
                        countDate = countDate.AddDays(1);
                    }
                }
                if (workingDays < 1) workingDays = 1;

                if (string.IsNullOrEmpty(reportType) || reportType.Equals("Daily", StringComparison.OrdinalIgnoreCase))
                {
                    workingDays = 1;
                }

                List<int> officeIds = new List<int>();
                if (!string.IsNullOrEmpty(selectedOffices))
                {
                    officeIds = selectedOffices.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(int.Parse)
                                               .ToList();
                }

                // 1. Get targets from JSON
                var jsonPath = Server.MapPath("~/App_Data/ProductionMetrics.json");
                var jsonContent = System.IO.File.ReadAllText(jsonPath);
                var targets = JsonConvert.DeserializeObject<List<ProductionMetricItem>>(jsonContent);

                // 2. Query AuditTimeEntries
                var timeEntriesQuery = db.AuditTimeEntries
                    .Include(x => x.Audit)
                    .Include(x => x.Audit.AuditRequestType)
                    .Include(x => x.AuditTask)
                    .Include(x => x.USERINFO)
                    .Where(x => (x.EndTime != null && x.EndTime >= dtStart && x.EndTime < dtEnd)
                             || (x.EndTime == null && x.StartTime >= dtStart && x.StartTime < dtEnd)
                             || (x.StartTime >= dtStart && x.StartTime < dtEnd));

                if (userId.HasValue)
                {
                    var uid = userId.Value;
                    timeEntriesQuery = timeEntriesQuery.Where(x => x.UserId == uid);
                }

                if (officeIds.Any())
                {
                    timeEntriesQuery = timeEntriesQuery.Where(x => x.Audit != null && officeIds.Contains(x.Audit.CrnId));
                }

                if (!string.IsNullOrEmpty(selectedLocation))
                {
                    timeEntriesQuery = timeEntriesQuery.Where(x => x.USERINFO != null && x.USERINFO.LOCATION == selectedLocation);
                }

                var timeEntries = timeEntriesQuery.ToList();

                var resultModel = new ProductionMetricsViewModel();
                var crns = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).ToList();
                var requestTypeMap = db.AuditRequestTypes.ToDictionary(r => r.Id, r => r.Name);

                Func<string, int, string> getJsonCrnName = (groupName, reqTypeId) => {
                    if (string.IsNullOrEmpty(groupName)) return "FNF";
                    var g = groupName.ToUpper();
                    if (g == "FIDELITY NATIONAL FINANCIAL") return "FNF";
                    if (g == "MONARCH TITLE COMPANY") return "MTC";
                    if (g == "CHICAGO TITLE COMPANY") return "CTT";
                    if (g == "ORANGE COAST TITLE") return "OCT";
                    if (g == "COMMONWEALTH") return "CWLT";
                    if (g == "TICOR TITLE") return "TICOR";
                    if (g == "STEWART TITLE") return "STEWART";
                    if (g == "LAWYERS TITLE COMPANY")
                    {
                        if (requestTypeMap.ContainsKey(reqTypeId))
                        {
                            var reqName = requestTypeMap[reqTypeId].ToUpper();
                            if (reqName.Contains("REFI") || reqName.Contains("RE-FINANCE"))
                                return "LTIC Refi";
                        }
                        return "LTIC Resale";
                    }
                    return groupName;
                };

                // Dynamic columns based on Audit tasks
                var taskOrder = new List<string> { "Taxes", "L&V", "PI", "GI", "Starter", "Audit Proofing" };
                var allAuditTasks = db.AuditTasks.OrderBy(x => x.SortOrder).Select(x => x.Name).ToList();
                foreach (var t in taskOrder)
                {
                    if (allAuditTasks.Contains(t))
                        resultModel.DynamicTaskColumns.Add(t);
                }
                foreach (var t in allAuditTasks)
                {
                    if (!resultModel.DynamicTaskColumns.Contains(t))
                        resultModel.DynamicTaskColumns.Add(t);
                }

                Func<string, string> normalizeTaskName = (name) =>
                {
                    if (string.IsNullOrEmpty(name)) return name;
                    var t = name.Trim();
                    if (string.Equals(t, "Notes", StringComparison.OrdinalIgnoreCase)) return "Audit Proofing";
                    return t;
                };

                var groupedByUser = timeEntries.GroupBy(t => t.UserId);
                int slNo = 1;

                foreach (var userGroup in groupedByUser)
                {
                    var uInfo = userGroup.First().USERINFO;
                    if (uInfo == null) continue;

                    var row = new ProductionMetricsUserRow
                    {
                        SerialNumber = slNo++,
                        UserId = uInfo.USERID,
                        Name = ((uInfo.FIRSTNAME ?? "") + " " + (uInfo.LASTNAME ?? "")).Trim(),
                        Location = uInfo.LOCATION,
                        TaskCounts = new Dictionary<string, ProductionMetricsTaskDetail>()
                    };
                    if (string.IsNullOrEmpty(row.Name)) row.Name = uInfo.USERNAME;

                    var taskAggregates = new Dictionary<string, ProductionMetricsTaskDetail>();
                    var orderCrnTaskMap = new Dictionary<string, List<OrderWithOffice>>();

                    var entriesByOrder = userGroup
                        .Where(t => t.Audit != null && !string.IsNullOrEmpty(t.Audit.OrderNo))
                        .GroupBy(t => t.Audit.OrderNo);

                    foreach (var orderGroup in entriesByOrder)
                    {
                        var processedTasks = new HashSet<string>();

                        foreach (var entry in orderGroup)
                        {
                            if (entry.AuditTask == null) continue;

                            var crn = crns.FirstOrDefault(c => c.CRNID == entry.Audit.CrnId);
                            string jsonCrnName = crn != null ? getJsonCrnName(crn.GroupName, entry.Audit.RequestTypeId) : "FNF";
                            string normalizedTaskName = normalizeTaskName(entry.AuditTask.Name);

                            string taskKey = jsonCrnName + " - " + normalizedTaskName;
                            if (processedTasks.Contains(taskKey)) continue;
                            processedTasks.Add(taskKey);

                            if (!taskAggregates.ContainsKey(normalizedTaskName))
                                taskAggregates[normalizedTaskName] = new ProductionMetricsTaskDetail { Count = 0, OrderNumbers = new List<OrderWithOffice>() };

                            var orderWithOffice = new OrderWithOffice
                            {
                                OrderNumber = orderGroup.Key,
                                OfficeName = crn != null ? crn.CRNNAME : jsonCrnName,
                                ProductType = entry.Audit.AuditRequestType != null ? entry.Audit.AuditRequestType.Name : ""
                            };

                            if (!taskAggregates[normalizedTaskName].OrderNumbers.Any(o => o.OrderNumber == orderGroup.Key))
                            {
                                taskAggregates[normalizedTaskName].OrderNumbers.Add(orderWithOffice);
                                taskAggregates[normalizedTaskName].Count = taskAggregates[normalizedTaskName].OrderNumbers.Count;
                            }

                            if (!orderCrnTaskMap.ContainsKey(taskKey))
                                orderCrnTaskMap[taskKey] = new List<OrderWithOffice>();

                            if (!orderCrnTaskMap[taskKey].Any(o => o.OrderNumber == orderGroup.Key))
                            {
                                orderCrnTaskMap[taskKey].Add(orderWithOffice);
                            }
                        }
                    }

                    foreach (var kvp in taskAggregates)
                    {
                        row.TaskCounts[kvp.Key] = kvp.Value;
                    }

                    bool isMonthly = !string.IsNullOrEmpty(reportType) && reportType.Equals("Monthly", StringComparison.OrdinalIgnoreCase);
                    int userWorkingDays = workingDays;

                    if (isMonthly)
                    {
                        DateTime realEndDate = dtEnd.Date;
                        if (dtEnd.TimeOfDay == TimeSpan.Zero && realEndDate > dtStart.Date)
                            realEndDate = realEndDate.AddDays(-1);

                        try
                        {
                            int shiftCount = db.ShiftAssignments
                                .Where(sa => sa.UserId == uInfo.USERID && sa.ShiftDate >= dtStart.Date && sa.ShiftDate <= realEndDate)
                                .Select(sa => sa.ShiftDate)
                                .Distinct()
                                .Count();

                            if (shiftCount > 0)
                            {
                                userWorkingDays = Math.Min(shiftCount, workingDays);
                            }
                        }
                        catch { }

                        decimal sumOfTaskAchievements = 0;
                        bool hasValidTasks = false;

                        foreach (var kvp in taskAggregates)
                        {
                            string taskName = kvp.Key;
                            int actualCount = kvp.Value.Count;

                            int dailyTarget = 0;
                            foreach (var t in targets.Where(x => string.Equals(x.TaskName, taskName, StringComparison.OrdinalIgnoreCase) && x.TargetMinutes > 0))
                            {
                                if (t.TargetMinutes > dailyTarget)
                                    dailyTarget = t.TargetMinutes;
                            }
                            if (dailyTarget == 0) dailyTarget = 40;

                            decimal taskMTDTarget = (decimal)dailyTarget * userWorkingDays;
                            decimal taskAchievedPct = (actualCount / taskMTDTarget) * 100.0m;
                            sumOfTaskAchievements += taskAchievedPct;
                            hasValidTasks = true;
                        }

                        if (hasValidTasks)
                        {
                            row.AchievedPercent = sumOfTaskAchievements;
                            row.AchievedPercentDisplay = Math.Round(row.AchievedPercent, 0, MidpointRounding.AwayFromZero).ToString("0") + "%";
                            resultModel.UserRows.Add(row);
                        }
                    }
                    else
                    {
                        decimal totalTimeUsed = 0;
                        bool hasValidTasks = false;

                        foreach (var kvp in orderCrnTaskMap)
                        {
                            var parts = kvp.Key.Split(new[] { " - " }, StringSplitOptions.None);
                            if (parts.Length < 2) continue;
                            var targetInfo = targets.FirstOrDefault(x => string.Equals(x.CrnName, parts[0], StringComparison.OrdinalIgnoreCase) && string.Equals(x.TaskName, parts[1], StringComparison.OrdinalIgnoreCase));
                            int timePerFile = targetInfo != null && targetInfo.TimePerFile > 0 ? targetInfo.TimePerFile : 12;

                            totalTimeUsed += (decimal)kvp.Value.Count * timePerFile;
                            hasValidTasks = true;
                        }

                        if (hasValidTasks)
                        {
                            decimal totalAvailableTime = 480.0m * workingDays;
                            row.AchievedPercent = totalTimeUsed / totalAvailableTime * 100.0m;
                            row.AchievedPercentDisplay = Math.Round(row.AchievedPercent, 0, MidpointRounding.AwayFromZero).ToString("0") + "%";
                            resultModel.UserRows.Add(row);
                        }
                    }
                }

                return Json(new { success = true, data = resultModel });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region User Day Production
        public ActionResult UserDayProduction(bool? showUserAndDate)
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var today = DateTime.Now.Date;
            try
            {
                var shiftAssignment = db.ShiftAssignments.AsNoTracking().FirstOrDefault(x => x.UserId == userInfo.USERID && x.ShiftDate == today);
                if (shiftAssignment != null && shiftAssignment.Shift != null)
                {
                    ViewBag.ShiftName = shiftAssignment.Shift.ShiftName;
                }
                else
                {
                    ViewBag.ShiftName = "Morning";
                }
            }
            catch
            {
                ViewBag.ShiftName = "Morning";
            }

            ViewBag.UserInfos = new SelectList(db.USERINFOes.Where(x => !x.ISDELETED.HasValue || x.ISDELETED.Value != 1).OrderBy(x => x.USERNAME).ToList(), "USERID", "USERNAME");
            ViewBag.ShowUserAndDate = showUserAndDate;

            return View();
        }

        public JsonResult SendDialyProducitonReport(string shift, DateTime? reportDate, int? userId)
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null)
            {
                return Json(new { error = "Unauthorized" }, JsonRequestBehavior.AllowGet);
            }

            int targetUserId = userId.HasValue ? userId.Value : userInfo.USERID;
            DateTime targetDate = reportDate.HasValue ? reportDate.Value.Date : DateTime.Now.Date;

            var targetUserInfo = db.USERINFOes.Find(targetUserId) ?? userInfo;

            if (!string.IsNullOrEmpty(shift))
            {
                var data = GetTaskSummary(targetUserId, shift, targetDate);
                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"<h2>Report Start Date: {data.StartDateTime:yyyy/MM/dd hh:mm tt}</h2>");

                Func<string, int> parseToMinutes = (s) =>
                {
                    if (string.IsNullOrWhiteSpace(s)) return 0;
                    TimeSpan ts;
                    if (TimeSpan.TryParse(s, out ts)) return (int)ts.TotalMinutes;
                    var match = System.Text.RegularExpressions.Regex.Match(s, @"(?<h>\d+):(?<m>\d+)");
                    int h;
                    int m;
                    if (match.Success && int.TryParse(match.Groups["h"].Value, out h) && int.TryParse(match.Groups["m"].Value, out m)) return h * 60 + m;
                    double dbl;
                    if (double.TryParse(s, out dbl)) return (int)(dbl * 60);
                    return 0;
                };

                int totalMinutes = 0;
                if (data.AuditTimeSummary != null)
                {
                    totalMinutes = data.AuditTimeSummary.Sum(x => parseToMinutes(x.TotalHours));
                }

                int breakMinutes = 0;
                if (data.UserBreakEntries != null)
                {
                    foreach (var b in data.UserBreakEntries)
                    {
                        if (b.ActualDurationMinutes.HasValue)
                            breakMinutes += b.ActualDurationMinutes.Value;
                        else
                            breakMinutes += b.PlannedDurationMinutes;
                    }
                }

                int productiveMinutes = Math.Max(0, totalMinutes - breakMinutes);

                Func<int, string> fmt = (mins) =>
                {
                    var h = mins / 60;
                    var m = mins % 60;
                    return string.Format("{0:D2}:{1:D2}", h, m);
                };

                sb.AppendLine("<div style='margin-bottom:12px;padding:10px;border:1px solid #ddd;background:#f9f9f9;border-radius:4px;'>");
                sb.AppendLine($"<strong>Total Hours:</strong> {HttpUtility.HtmlEncode(fmt(totalMinutes))} &nbsp;&nbsp; ");
                sb.AppendLine($"<strong>Break:</strong> {HttpUtility.HtmlEncode(fmt(breakMinutes))} &nbsp;&nbsp; ");
                sb.AppendLine($"<strong>Productive Hours:</strong> {HttpUtility.HtmlEncode(fmt(productiveMinutes))}");
                sb.AppendLine("</div>");

                sb.Append("<br/><h3>Audit - Hours Summary</h3>")
                  .Append(GenerateHtmlTable(data.AuditTimeSummary));
                sb.Append("<br/><h3>Audit - Count Summary</h3>")
                  .Append(GenerateHtmlTable(data.AuditCountSummary));

                sb.Append("<h3>Audit - Detailed Tasks</h3>")
                  .Append(GenerateHtmlTable(data.ProductionDetails.OrderBy(x => x.StartTime).ToList()));

                sb.Append("<br/><h3>Break Details</h3>")
                  .Append(GenerateHtmlTable(data.UserBreakEntries));

                string recordsHtmlContent = sb.ToString();

                string emailTo = !string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("DAILYPRODUCTIONEMAIL-TO")) ? ConfigurationManager.AppSettings.Get("DAILYPRODUCTIONEMAIL-TO") : "managers@firstfocusbpo.com";
                string emailFrom = !string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("DAILYPRODUCTIONEMAIL-FROM")) ? ConfigurationManager.AppSettings.Get("DAILYPRODUCTIONEMAIL-FROM") : "notifications@ftbpo.com";

                string subject = string.Format("User {0} - Audit Production Report", targetUserInfo.USERNAME);

                try
                {
                    sendMail(emailTo, emailFrom, subject, recordsHtmlContent, targetUserInfo.EMAIL, true);
                }
                catch { }
            }

            return new JsonResult() { Data = "Sent", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public UserDayProductionReportDetailViewModel GetTaskSummary(int userId, string shift, DateTime reportDate)
        {
            var response = new UserDayProductionReportDetailViewModel
            {
                StartDateTime = reportDate,
                ProductionDetails = new List<UserDayProductionReportViewModel>(),
                AuditTimeSummary = new List<AuditTaskSummaryModel>(),
                AuditCountSummary = new List<AuditTaskCountSummaryModel>(),
                UserBreakEntries = new List<UserBreakEntryModel>()
            };

            var user = db.USERINFOes.Find(userId);
            if (user == null) return response;

            DateTime shiftStart = reportDate.Date.AddHours(6);
            DateTime shiftEnd = reportDate.Date.AddHours(15);

            try
            {
                var shiftObj = db.Shifts.FirstOrDefault(s => s.ShiftName == shift);
                if (shiftObj != null)
                {
                    shiftStart = reportDate.Date.Add(shiftObj.StartTime);
                    var dur = shiftObj.EndTime - shiftObj.StartTime;
                    if (dur <= TimeSpan.Zero) dur += TimeSpan.FromDays(1);
                    shiftEnd = shiftStart.Add(dur);
                }
                else if (shift.IndexOf("Afternoon", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    shiftStart = reportDate.Date.AddHours(14);
                    shiftEnd = reportDate.Date.AddHours(23);
                }
                else if (shift.IndexOf("Night", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    shiftStart = reportDate.Date.AddHours(22);
                    shiftEnd = reportDate.Date.AddDays(1).AddHours(7);
                }
                else if (shift.IndexOf("General", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    shiftStart = reportDate.Date.AddHours(9);
                    shiftEnd = reportDate.Date.AddHours(18);
                }
            }
            catch { }

            response.StartDateTime = shiftStart;

            var timeEntries = db.AuditTimeEntries
                .Include(x => x.Audit)
                .Include(x => x.Audit.CRN)
                .Include(x => x.AuditTask)
                .Include(x => x.Audit.AuditStatus)
                .Where(x => x.UserId == userId &&
                            ((x.StartTime >= shiftStart && x.StartTime < shiftEnd) ||
                             (x.EndTime.HasValue && x.EndTime >= shiftStart && x.EndTime <= shiftEnd)))
                .ToList();

            try
            {
                var breaks = db.UserBreakEntries
                    .Where(b => b.UserId == userId &&
                                b.StartTime >= shiftStart &&
                                b.StartTime < shiftEnd)
                    .OrderBy(b => b.StartTime)
                    .ToList();

                foreach (var b in breaks)
                {
                    response.UserBreakEntries.Add(new UserBreakEntryModel
                    {
                        StartTime = b.StartTime,
                        EndTime = b.EndTime,
                        PlannedDurationMinutes = b.PlannedDurationMinutes,
                        ActualDurationMinutes = b.ActualDurationMinutes,
                        BreakReason = b.BreakReason
                    });
                }
            }
            catch { }

            foreach (var te in timeEntries)
            {
                int durationMin = 0;
                if (te.EndTime.HasValue)
                {
                    var span = te.EndTime.Value - te.StartTime;
                    durationMin = (int)Math.Max(0, span.TotalMinutes);
                }

                response.ProductionDetails.Add(new UserDayProductionReportViewModel
                {
                    App = "Audit",
                    UserId = userId,
                    UserName = user.USERNAME,
                    OrderNo = te.Audit != null ? te.Audit.OrderNo : "",
                    ReceivedDateTime = te.Audit != null && te.Audit.DateCreated.HasValue ? te.Audit.DateCreated.Value : DateTime.MinValue,
                    CRNName = te.Audit != null && te.Audit.CRN != null ? te.Audit.CRN.CRNNAME : "",
                    TaskName = te.AuditTask != null ? te.AuditTask.Name : "",
                    StartTime = te.StartTime,
                    EndTime = te.EndTime.HasValue ? te.EndTime.Value : DateTime.MinValue,
                    TimeTakenMin = durationMin,
                    StatusName = te.Audit != null && te.Audit.AuditStatus != null ? te.Audit.AuditStatus.Name : ""
                });
            }

            Func<int, int> getTaskSeconds = (taskId) =>
            {
                return (int)timeEntries
                    .Where(x => x.AuditTaskId == taskId && x.EndTime.HasValue)
                    .Sum(x => (x.EndTime.Value - x.StartTime).TotalSeconds);
            };

            Func<int, string> formatSeconds = (secs) =>
            {
                int h = secs / 3600;
                int m = (secs % 3600) / 60;
                return $"{h:D2}:{m:D2}";
            };

            int taxesSec = getTaskSeconds(6);
            int lvSec = getTaskSeconds(1);
            int piSec = getTaskSeconds(2);
            int giSec = getTaskSeconds(3);
            int starterSec = getTaskSeconds(4);
            int proofingSec = getTaskSeconds(5);
            int totalSec = taxesSec + lvSec + piSec + giSec + starterSec + proofingSec;

            response.AuditTimeSummary.Add(new AuditTaskSummaryModel
            {
                UserName = user.USERNAME,
                Location = user.LOCATION ?? "",
                TaxesHours = formatSeconds(taxesSec),
                LVHours = formatSeconds(lvSec),
                PIHours = formatSeconds(piSec),
                GIHours = formatSeconds(giSec),
                StarterHours = formatSeconds(starterSec),
                ProofingHours = formatSeconds(proofingSec),
                TotalHours = formatSeconds(totalSec)
            });

            Func<int, int> getTaskCount = (taskId) =>
            {
                return timeEntries.Where(x => x.AuditTaskId == taskId).Select(x => x.AuditId).Distinct().Count();
            };

            int taxesCnt = getTaskCount(6);
            int lvCnt = getTaskCount(1);
            int piCnt = getTaskCount(2);
            int giCnt = getTaskCount(3);
            int starterCnt = getTaskCount(4);
            int proofingCnt = getTaskCount(5);
            int totalCnt = taxesCnt + lvCnt + piCnt + giCnt + starterCnt + proofingCnt;

            response.AuditCountSummary.Add(new AuditTaskCountSummaryModel
            {
                UserName = user.USERNAME,
                Location = user.LOCATION ?? "",
                TaxesCount = taxesCnt,
                LVCount = lvCnt,
                PICount = piCnt,
                GICount = giCnt,
                StarterCount = starterCnt,
                ProofingCount = proofingCnt,
                TotalCount = totalCnt
            });

            return response;
        }

        public string GenerateHtmlTable<T>(List<T> data)
        {
            var html = new StringBuilder();
            var props = typeof(T).GetProperties();

            html.Append("<table border='1' cellpadding='4' cellspacing='0'><thead><tr style=\"background: #1a472a; color: white;\">");
            foreach (var prop in props)
            {
                var displayAttr = prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>();
                var name = displayAttr != null ? displayAttr.Name : prop.Name;
                html.Append($"<th>{name}</th>");
            }
            html.Append("</tr></thead><tbody>");

            if (data != null)
            {
                foreach (var item in data)
                {
                    html.Append("<tr>");
                    foreach (var prop in props)
                    {
                        var val = prop.GetValue(item)?.ToString() ?? string.Empty;
                        html.Append($"<td>{HttpUtility.HtmlEncode(val)}</td>");
                    }
                    html.Append("</tr>");
                }
            }

            html.Append("</tbody></table>");
            return html.ToString();
        }
        #endregion

        #region Pending Orders
        public ActionResult PendingOrdersReport()
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            SetOfficeFields(userInfo);
            ViewBag.Offices = db.CRNs.Where(x => x.IsVisibleInMainApplication == true).ToList();

            return View();
        }

        private List<int> GetPendingOrdersOfficeIds(USERINFO userInfo, string selectedOffices)
        {
            if (!string.IsNullOrEmpty(selectedOffices))
            {
                return selectedOffices.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(s => { int id; return int.TryParse(s, out id) ? id : 0; })
                                      .Where(id => id > 0)
                                      .ToList();
            }

            return (from crn in db.CRNs
                    join userCrn in db.UserCrns on crn.CRNID equals userCrn.CrnId
                    where userCrn.UserId == userInfo.USERID
                    select crn.CRNID).ToList();
        }

        public JsonResult GetPendingOrders(string selectedOffices, string selectedStatuses)
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Json(new object[0], JsonRequestBehavior.AllowGet);
            }

            List<int> officeIds = GetPendingOrdersOfficeIds(userInfo, selectedOffices);
            var statusFilters = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(selectedStatuses))
            {
                foreach (var s in selectedStatuses.Split(','))
                    if (!string.IsNullOrWhiteSpace(s)) statusFilters.Add(s.Trim());
            }

            var query = db.Audits
                .Where(x => x.StatusId != 4 && x.StatusId != 5)
                .Include(x => x.CRN)
                .Include(x => x.AuditRequestType)
                .Include(x => x.AuditTask)
                .Include(x => x.AuditStatus)
                .Include(x => x.AuditTimeEntries.Select(t => t.USERINFO))
                .Include(x => x.AuditHolds.Select(h => h.USERINFO));

            if (officeIds.Any())
            {
                query = query.Where(x => officeIds.Contains(x.CrnId));
            }

            var users = db.USERINFOes.ToDictionary(x => x.USERID, x => x.USERNAME);
            var audits = query.OrderBy(x => x.DateCreated).ToList();

            var result = new List<object>();
            foreach (var order in audits)
            {
                string statusLabel = "Pending";
                if (order.StatusId == 3)
                {
                    statusLabel = "On Hold";
                }
                else if (order.AuditTimeEntries.Any(t => t.IsInprogress))
                {
                    statusLabel = "In Progress";
                }

                if (statusFilters.Count > 0 && !statusFilters.Contains(statusLabel))
                {
                    continue;
                }

                string userName = "";
                var timeEntry = order.AuditTimeEntries.FirstOrDefault(t => t.IsInprogress);
                if (timeEntry != null && timeEntry.USERINFO != null)
                {
                    userName = timeEntry.USERINFO.USERNAME;
                }
                else if (order.StatusId == 3)
                {
                    var hold = order.AuditHolds.OrderByDescending(h => h.HoldDateTime).FirstOrDefault();
                    if (hold != null && hold.USERINFO != null)
                    {
                        userName = hold.USERINFO.USERNAME;
                    }
                }
                else if (order.UserAssigned.HasValue && users.ContainsKey(order.UserAssigned.Value))
                {
                    userName = users[order.UserAssigned.Value];
                }

                result.Add(new
                {
                    id = order.Id,
                    orderNo = order.OrderNo,
                    officeName = order.CRN != null ? order.CRN.CRNNAME : "",
                    receivedDate = order.DateCreated.HasValue ? order.DateCreated.Value.ToString("MM/dd/yyyy HH:mm") : "",
                    productType = order.AuditRequestType != null ? order.AuditRequestType.Name : "",
                    requestedBy = order.RequestedByEmailId ?? "",
                    task = order.AuditTask != null ? order.AuditTask.Name : "",
                    assignedUser = userName,
                    status = statusLabel
                });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void ExportPendingOrdersReport(string selectedOffices, string selectedStatuses)
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null) return;

            List<int> officeIds = GetPendingOrdersOfficeIds(userInfo, selectedOffices);
            var statusFilters = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(selectedStatuses))
            {
                foreach (var s in selectedStatuses.Split(','))
                    if (!string.IsNullOrWhiteSpace(s)) statusFilters.Add(s.Trim());
            }

            var query = db.Audits
                .Where(x => x.StatusId != 4 && x.StatusId != 5)
                .Include(x => x.CRN)
                .Include(x => x.AuditRequestType)
                .Include(x => x.AuditTask)
                .Include(x => x.AuditStatus)
                .Include(x => x.AuditTimeEntries.Select(t => t.USERINFO))
                .Include(x => x.AuditHolds.Select(h => h.USERINFO));

            if (officeIds.Any())
            {
                query = query.Where(x => officeIds.Contains(x.CrnId));
            }

            var users = db.USERINFOes.ToDictionary(x => x.USERID, x => x.USERNAME);
            var audits = query.OrderBy(x => x.DateCreated).ToList();

            var reportItems = new List<PendingOrdersReportViewModel>();
            int slNo = 1;
            foreach (var order in audits)
            {
                string statusLabel = "Pending";
                if (order.StatusId == 3) statusLabel = "On Hold";
                else if (order.AuditTimeEntries.Any(t => t.IsInprogress)) statusLabel = "In Progress";

                if (statusFilters.Count > 0 && !statusFilters.Contains(statusLabel)) continue;

                string userName = "";
                var timeEntry = order.AuditTimeEntries.FirstOrDefault(t => t.IsInprogress);
                if (timeEntry != null && timeEntry.USERINFO != null)
                {
                    userName = timeEntry.USERINFO.USERNAME;
                }
                else if (order.StatusId == 3)
                {
                    var hold = order.AuditHolds.OrderByDescending(h => h.HoldDateTime).FirstOrDefault();
                    if (hold != null && hold.USERINFO != null) userName = hold.USERINFO.USERNAME;
                }
                else if (order.UserAssigned.HasValue && users.ContainsKey(order.UserAssigned.Value))
                {
                    userName = users[order.UserAssigned.Value];
                }

                reportItems.Add(new PendingOrdersReportViewModel
                {
                    SlNo = slNo++,
                    OrderNo = order.OrderNo,
                    OfficeName = order.CRN != null ? order.CRN.CRNNAME : "",
                    ReceivedDate = order.DateCreated.HasValue ? order.DateCreated.Value.ToString("MM/dd/yyyy HH:mm") : "",
                    ProductType = order.AuditRequestType != null ? order.AuditRequestType.Name : "",
                    RequestedBy = order.RequestedByEmailId ?? "",
                    Task = order.AuditTask != null ? order.AuditTask.Name : "",
                    User = userName,
                    Status = statusLabel
                });
            }

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=PendingOrdersReport.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            WriteHtmlTable<PendingOrdersReportViewModel>(reportItems, Response.Output);
            Response.End();
        }
        #endregion

        #region Login Logout (Attendance Report)
        public ActionResult AttendanceReport(
            DateTime? startDateTime,
            DateTime? endDateTime,
            string location,
            int? selectedUserId,
            string reportingManagerIds)
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null)
                return RedirectToAction("Login", "Account");

            SetOfficeFields(userInfo);

            if (!startDateTime.HasValue || !endDateTime.HasValue)
                return View(new List<AttendanceReportViewModel>());

            DateTime start = startDateTime.Value.Date;
            DateTime end = endDateTime.Value.Date;

            var listUsers = db.USERINFOes
                .Where(x => (x.ISDELETED == 0 || x.ISDELETED == null) && x.USERNAME != "Admin");

            if (!string.IsNullOrEmpty(location))
                listUsers = listUsers.Where(x => x.LOCATION == location);

            var managerIds = new List<int>();
            if (!string.IsNullOrEmpty(reportingManagerIds))
            {
                foreach (var idStr in reportingManagerIds.Split(','))
                {
                    int mgrId;
                    if (int.TryParse(idStr.Trim(), out mgrId) && mgrId > 0)
                        managerIds.Add(mgrId);
                }
            }
            if (managerIds.Count > 0)
                listUsers = listUsers.Where(x => x.ReportingManagerId.HasValue && managerIds.Contains(x.ReportingManagerId.Value));

            if (selectedUserId.HasValue && selectedUserId.Value > 0)
                listUsers = listUsers.Where(x => x.USERID == selectedUserId.Value);

            var users = listUsers.ToList();
            var userIds = users.Select(u => u.USERID).ToList();

            DateTime fetchStart = start.AddDays(-1);
            DateTime fetchEnd = end.AddDays(2);

            var userTimeEntries = db.UserTimeEntries.AsNoTracking()
                .Where(x => userIds.Contains(x.UserId)
                         && x.StartTime >= fetchStart
                         && x.StartTime < fetchEnd)
                .ToList();

            var breakEntries = new List<UserBreakEntry>();
            try
            {
                breakEntries = db.UserBreakEntries.AsNoTracking()
                    .Where(b => userIds.Contains(b.UserId)
                             && b.StartTime >= fetchStart
                             && b.StartTime < fetchEnd
                             && b.EndTime.HasValue)
                    .ToList();
            }
            catch { }

            var shiftAssignments = new List<ShiftAssignment>();
            try
            {
                shiftAssignments = db.ShiftAssignments.AsNoTracking()
                    .Include(sa => sa.Shift)
                    .Where(sa => userIds.Contains(sa.UserId)
                              && sa.ShiftDate >= start
                              && sa.ShiftDate <= end)
                    .ToList();
            }
            catch { }

            var orderTimeEntries = db.AuditTimeEntries.AsNoTracking()
                .Where(x => userIds.Contains(x.UserId)
                         && x.StartTime >= fetchStart
                         && x.StartTime < fetchEnd)
                .ToList();

            var result = new List<AttendanceReportViewModel>();

            for (DateTime curDate = start; curDate <= end; curDate = curDate.AddDays(1))
            {
                DateTime day = curDate;

                foreach (var user in users)
                {
                    var userShifts = shiftAssignments
                        .Where(sa => sa.UserId == user.USERID && sa.ShiftDate.Date == day)
                        .ToList();

                    var shiftsToProcess = new List<ShiftAssignment>();
                    if (userShifts.Any())
                    {
                        shiftsToProcess = userShifts;
                    }
                    else
                    {
                        shiftsToProcess.Add(new ShiftAssignment
                        {
                            UserId = user.USERID,
                            ShiftDate = day,
                            Shift = new Shift
                            {
                                ShiftName = "General",
                                StartTime = new TimeSpan(9, 0, 0),
                                EndTime = new TimeSpan(18, 0, 0)
                            }
                        });
                    }

                    foreach (var sa in shiftsToProcess)
                    {
                        var shift = sa.Shift;
                        if (shift == null) continue;

                        DateTime shiftStart = day.Add(shift.StartTime);
                        TimeSpan shiftDuration = shift.EndTime - shift.StartTime;
                        if (shiftDuration <= TimeSpan.Zero)
                            shiftDuration += TimeSpan.FromDays(1);
                        DateTime shiftEnd = shiftStart.Add(shiftDuration);

                        DateTime windowStart = shiftStart.AddHours(-4);
                        DateTime windowEnd = shiftEnd.AddHours(4);

                        var relevantLogins = userTimeEntries
                            .Where(x => x.UserId == user.USERID
                                     && x.StartTime >= windowStart
                                     && x.StartTime < windowEnd)
                            .OrderBy(x => x.StartTime)
                            .ToList();

                        var relevantBreaks = breakEntries
                            .Where(b => b.UserId == user.USERID
                                     && b.StartTime >= windowStart
                                     && b.StartTime < windowEnd)
                            .ToList();

                        var relevantOrders = orderTimeEntries
                            .Where(x => x.UserId == user.USERID
                                     && x.StartTime >= windowStart
                                     && x.StartTime < windowEnd)
                            .OrderBy(x => x.StartTime)
                            .ToList();

                        string loginTimeStr = "-";
                        string logoutTimeStr = "-";

                        if (relevantLogins.Any())
                        {
                            var firstLogin = relevantLogins.First();
                            loginTimeStr = firstLogin.StartTime.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture);

                            var lastLoginWithEnd = relevantLogins
                                .Where(x => x.EndTime.HasValue)
                                .OrderByDescending(x => x.EndTime.Value)
                                .FirstOrDefault();

                            if (lastLoginWithEnd != null && lastLoginWithEnd.EndTime.HasValue)
                            {
                                logoutTimeStr = lastLoginWithEnd.EndTime.Value.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                logoutTimeStr = "Active";
                            }
                        }

                        string orderStartTimeStr = "-";
                        if (relevantOrders.Any())
                        {
                            orderStartTimeStr = relevantOrders.First().StartTime.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture);
                        }

                        int totalBreakMinutes = 0;
                        foreach (var b in relevantBreaks)
                        {
                            totalBreakMinutes += b.ActualDurationMinutes.HasValue ? b.ActualDurationMinutes.Value : b.PlannedDurationMinutes;
                        }
                        string breakDurationStr = $"{totalBreakMinutes / 60:D2}:{totalBreakMinutes % 60:D2}";

                        int totalOrders = relevantOrders.Count(x => !x.IsInprogress && x.EndTime.HasValue);

                        string mgrName = "";
                        if (user.ReportingManagerId.HasValue)
                        {
                            var mgr = db.USERINFOes.Find(user.ReportingManagerId.Value);
                            mgrName = mgr != null ? (mgr.FULLNAME ?? mgr.USERNAME) : "";
                        }

                        result.Add(new AttendanceReportViewModel
                        {
                            UserId = user.USERID,
                            EmployeeCode = user.EmployeeCode ?? user.USERNAME,
                            EmployeeName = ((user.FIRSTNAME ?? "") + " " + (user.LASTNAME ?? "")).Trim(),
                            ProjectManager = mgrName,
                            Date = day.ToString("MM/dd/yyyy"),
                            Shift = shift.ShiftName,
                            ShiftTiming = $"{DateTime.Today.Add(shift.StartTime):hh:mm tt} - {DateTime.Today.Add(shift.EndTime):hh:mm tt}",
                            LoginTime = loginTimeStr,
                            OrderStartTime = orderStartTimeStr,
                            TotalBreakDuration = breakDurationStr,
                            LogoutTime = logoutTimeStr,
                            TotalOrders = totalOrders,
                            Comments = ""
                        });
                    }
                }
            }

            return View(result);
        }

        public void ExportAttendanceReport(
            DateTime? startDateTime,
            DateTime? endDateTime,
            string location,
            int? selectedUserId,
            string reportingManagerIds)
        {
            var viewResult = AttendanceReport(startDateTime, endDateTime, location, selectedUserId, reportingManagerIds) as ViewResult;
            var list = (viewResult?.Model as List<AttendanceReportViewModel>) ?? new List<AttendanceReportViewModel>();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=LoginLogoutReport.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            WriteHtmlTable<AttendanceReportViewModel>(list, Response.Output);
            Response.End();
        }
        #endregion

        #region Shift Allowance Report
        [Authorize]
        public ActionResult ShiftAllowanceReport()
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null)
                return RedirectToAction("Login", "Account");

            SetOfficeFields(userInfo);
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetShiftAllowanceReportData(
            DateTime? startDateTime,
            DateTime? endDateTime,
            string location,
            int? selectedUserId)
        {
            try
            {
                if (!startDateTime.HasValue || !endDateTime.HasValue)
                    return Json(new { success = false, message = "Date range is required." }, JsonRequestBehavior.AllowGet);

                var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime startIst = TimeZoneInfo.ConvertTime(startDateTime.Value, istZone);
                DateTime endIst = TimeZoneInfo.ConvertTime(endDateTime.Value, istZone);

                DateTime dtStart = startIst.Date;
                DateTime dtEnd = endIst.Date;

                var usersQuery = db.USERINFOes
                    .Where(x => (x.ISDELETED == 0 || x.ISDELETED == null) && x.USERNAME != "Admin");

                if (!string.IsNullOrEmpty(location))
                    usersQuery = usersQuery.Where(x => x.LOCATION == location);

                if (selectedUserId.HasValue && selectedUserId.Value > 0)
                    usersQuery = usersQuery.Where(x => x.USERID == selectedUserId.Value);

                var users = usersQuery.ToList();
                var userIds = users.Select(u => u.USERID).ToList();

                var shiftAssignments = new List<ShiftAssignment>();
                try
                {
                    shiftAssignments = db.ShiftAssignments.AsNoTracking()
                        .Include(sa => sa.Shift)
                        .Where(sa => userIds.Contains(sa.UserId)
                                  && sa.ShiftDate >= dtStart
                                  && sa.ShiftDate <= dtEnd)
                        .ToList();
                }
                catch { }

                DateTime dtEndExclusive = dtEnd.AddDays(1);

                var userTimeEntries = db.UserTimeEntries.AsNoTracking()
                    .Where(x => userIds.Contains(x.UserId)
                             && x.StartTime >= dtStart
                             && x.StartTime < dtEndExclusive)
                    .ToList();

                var orderTimeEntries = db.AuditTimeEntries.AsNoTracking()
                    .Include(x => x.AuditTask)
                    .Where(x => userIds.Contains(x.UserId)
                             && x.StartTime >= dtStart
                             && x.StartTime < dtEndExclusive)
                    .ToList();

                var jsonPath = Server.MapPath("~/App_Data/ProductionMetrics.json");
                var jsonContent = System.IO.File.ReadAllText(jsonPath);
                var targets = JsonConvert.DeserializeObject<List<ProductionMetricItem>>(jsonContent);

                var result = new ShiftAllowanceReportViewModel();
                int slNo = 1;

                foreach (var user in users)
                {
                    var userShifts = shiftAssignments
                        .Where(sa => sa.UserId == user.USERID)
                        .ToList();

                    if (!userShifts.Any()) continue;

                    var row = new ShiftAllowanceReportRow
                    {
                        SerialNumber = slNo++,
                        UserId = user.USERID,
                        EmpCode = user.EmployeeCode ?? user.USERNAME,
                        Name = ((user.FIRSTNAME ?? "") + " " + (user.LASTNAME ?? "")).Trim(),
                        Process = user.PrimaryOperationName ?? "Audit",
                        Location = user.LOCATION ?? ""
                    };
                    if (string.IsNullOrEmpty(row.Name)) row.Name = user.USERNAME;

                    var distinctShifts = userShifts
                        .Where(sa => sa.Shift != null && sa.Shift.ShiftName != "Off")
                        .GroupBy(sa => sa.ShiftDate.Date)
                        .Select(g => g.OrderByDescending(x => x.Id).First())
                        .ToList();

                    foreach (var sa in distinctShifts)
                    {
                        if (sa.ShiftDate.DayOfWeek == DayOfWeek.Saturday || sa.ShiftDate.DayOfWeek == DayOfWeek.Sunday) continue;

                        DateTime shiftDateIst = TimeZoneInfo.ConvertTime(sa.ShiftDate, istZone);
                        DateTime shiftDate = shiftDateIst.Date;
                        string dateLabel = shiftDate.ToString("dd-MMM-yyyy") + " IST";
                        bool isNight = sa.Shift.ShiftName.IndexOf("Night", StringComparison.OrdinalIgnoreCase) >= 0;

                        DateTime shiftStart = shiftDate.Add(sa.Shift.StartTime);
                        TimeSpan duration = sa.Shift.EndTime - sa.Shift.StartTime;
                        if (duration <= TimeSpan.Zero) duration += TimeSpan.FromDays(1);
                        DateTime shiftEnd = shiftStart.Add(duration);

                        bool hasLogin = userTimeEntries
                            .Any(x => x.UserId == user.USERID
                                   && x.StartTime >= shiftStart.AddHours(-5)
                                   && x.StartTime < shiftEnd.AddHours(2));

                        bool hasOrder = orderTimeEntries
                            .Any(x => x.UserId == user.USERID
                                   && x.StartTime >= shiftStart.AddHours(-5)
                                   && x.StartTime < shiftEnd.AddHours(2));

                        bool isPresent = hasLogin || hasOrder;

                        if (!isPresent)
                        {
                            row.LeavesAvailed++;
                            row.LeaveDates.Add(dateLabel + " (" + sa.Shift.ShiftName + ")");
                        }
                        else
                        {
                            row.TotalWorkingDays++;

                            if (isNight)
                            {
                                row.NightShiftCount++;
                                row.NSACount++;
                                row.NightShiftDates.Add(dateLabel + " (" + sa.Shift.ShiftName + ")");
                            }
                            else
                            {
                                row.DayShiftCount++;
                                row.DayShiftDates.Add(dateLabel + " (" + sa.Shift.ShiftName + ")");
                            }
                        }
                    }

                    if (row.TotalWorkingDays > 0)
                    {
                        var userOrders = orderTimeEntries
                            .Where(x => x.UserId == user.USERID && x.EndTime.HasValue)
                            .ToList();

                        decimal totalTimeUsed = 0;
                        foreach (var entry in userOrders)
                        {
                            string taskName = entry.AuditTask != null ? entry.AuditTask.Name : "";
                            var targetInfo = targets.FirstOrDefault(t => string.Equals(t.TaskName, taskName, StringComparison.OrdinalIgnoreCase));
                            int timePerFile = targetInfo != null && targetInfo.TimePerFile > 0 ? targetInfo.TimePerFile : 12;
                            totalTimeUsed += timePerFile;
                        }

                        decimal totalAvailableTime = 480.0m * row.TotalWorkingDays;
                        decimal prodPct = totalAvailableTime > 0 ? (totalTimeUsed / totalAvailableTime) * 100.0m : 0;
                        row.ProdPercent = Math.Round(prodPct, 0, MidpointRounding.AwayFromZero).ToString("0") + "%";
                    }
                    else
                    {
                        row.ProdPercent = "0%";
                    }

                    result.Rows.Add(row);
                }

                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region Attendance Helpers
        public JsonResult GetUsersForAttendance()
        {
            var allUsers = db.USERINFOes.ToList();
            var users = allUsers
                .Where(x => x.ISDELETED == 0 || x.ISDELETED == null)
                .OrderBy(x => x.USERNAME)
                .Select(x => new
                {
                    id = x.USERID,
                    name = x.USERNAME,
                    location = x.LOCATION ?? "",
                    reportingManagerId = x.ReportingManagerId,
                    reportingManagerName = x.ReportingManagerId.HasValue ? allUsers.FirstOrDefault(u => u.USERID == x.ReportingManagerId.Value)?.USERNAME : ""
                })
                .ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReportingManagers()
        {
            var managerIds = db.USERINFOes
                .Where(u => u.ReportingManagerId.HasValue)
                .Select(u => u.ReportingManagerId.Value)
                .Distinct()
                .ToList();

            var reportingManagers = db.USERINFOes
                .Where(u => managerIds.Contains(u.USERID))
                .OrderBy(u => u.USERNAME)
                .Select(u => new
                {
                    id = u.USERID,
                    name = u.USERNAME
                })
                .ToList();

            return Json(reportingManagers, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}