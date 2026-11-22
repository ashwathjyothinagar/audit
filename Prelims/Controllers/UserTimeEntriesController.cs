using Prelims.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Prelims.Controllers
{
    public class UserTimeEntriesController : Controller
    {
        private readonly FntEntities _context = new FntEntities();

        public UserTimeEntriesController()
        {
        }

        // Start tracking time (Dashboard Login)
        public ActionResult Start()
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var now = DateTime.Now;
            var nineHoursAgo = now.AddHours(-9);

            // Find active entry (EndTime == null)
            var activeEntry = _context.UserTimeEntries
                .FirstOrDefault(entry =>
                    entry.UserId == userInfo.USERID &&
                    entry.EndTime == null);

            if (activeEntry != null)
            {
                // If the active shift started more than 9 hours ago → auto-end it
                if (activeEntry.StartTime < nineHoursAgo)
                {
                    activeEntry.EndTime = activeEntry.StartTime.AddHours(9);
                    activeEntry.TotalSeconds = (int)(activeEntry.EndTime.Value - activeEntry.StartTime).TotalSeconds;

                    // Create a new shift
                    var newEntry = new UserTimeEntry
                    {
                        UserId = userInfo.USERID,
                        StartTime = now
                    };
                    _context.UserTimeEntries.Add(newEntry);
                }
                // else: Active and within 9 hours → do nothing (continue)
            }
            else
            {
                // No active entry → create one
                var newEntry = new UserTimeEntry
                {
                    UserId = userInfo.USERID,
                    StartTime = now
                };
                _context.UserTimeEntries.Add(newEntry);
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        // End tracking time (Dashboard Logout)
        public ActionResult End(int entryId)
        {
            var entry = _context.UserTimeEntries.Find(entryId);
            if (entry == null || entry.EndTime != null)
            {
                return RedirectToAction("Index", "Home");
            }

            entry.EndTime = DateTime.Now;
            entry.TotalSeconds = (int)(entry.EndTime.Value - entry.StartTime).TotalSeconds;
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        // POST: Start break
        [HttpPost]
        public JsonResult BreakIn(int breakDurationMinutes, string breakReason = "")
        {
            try
            {
                USERINFO userInfo = (USERINFO)Session["UserInfo"];
                if (userInfo == null)
                {
                    return Json(new { success = false, message = "User not authenticated." });
                }

                var activeBreak = _context.Database.SqlQuery<UserBreakEntryDto>(
                    @"SELECT TOP 1 Id, UserId, StartTime, EndTime, PlannedDurationMinutes, ActualDurationMinutes, BreakReason, CreatedDate
                      FROM UserBreakEntries
                      WHERE UserId = @p0 AND EndTime IS NULL",
                    userInfo.USERID).FirstOrDefault();

                if (activeBreak != null)
                {
                    return Json(new { success = false, message = "You are already on a break." });
                }

                var now = DateTime.Now;
                string reason = string.IsNullOrEmpty(breakReason) ? GetBreakReasonFromDuration(breakDurationMinutes) : breakReason;

                _context.Database.ExecuteSqlCommand(
                    @"INSERT INTO UserBreakEntries (UserId, StartTime, PlannedDurationMinutes, BreakReason, CreatedDate)
                      VALUES (@p0, @p1, @p2, @p3, @p4)",
                    userInfo.USERID, now, breakDurationMinutes, reason, now);

                return Json(new
                {
                    success = true,
                    message = "Break started successfully.",
                    breakStartTime = now.ToString("yyyy-MM-ddTHH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        // POST: End break
        [HttpPost]
        public JsonResult BreakOut()
        {
            try
            {
                USERINFO userInfo = (USERINFO)Session["UserInfo"];
                if (userInfo == null)
                {
                    return Json(new { success = false, message = "User not authenticated." });
                }

                var activeBreak = _context.Database.SqlQuery<UserBreakEntryDto>(
                    @"SELECT TOP 1 Id, UserId, StartTime, EndTime, PlannedDurationMinutes, ActualDurationMinutes, BreakReason, CreatedDate
                      FROM UserBreakEntries
                      WHERE UserId = @p0 AND EndTime IS NULL",
                    userInfo.USERID).FirstOrDefault();

                if (activeBreak == null)
                {
                    return Json(new { success = false, message = "No active break found." });
                }

                var endTime = DateTime.Now;
                var durationInMinutes = (int)Math.Ceiling((endTime - activeBreak.StartTime).TotalMinutes);
                int actualDuration = Math.Max(durationInMinutes, 1);

                _context.Database.ExecuteSqlCommand(
                    @"UPDATE UserBreakEntries SET EndTime = @p0, ActualDurationMinutes = @p1 WHERE Id = @p2",
                    endTime, actualDuration, activeBreak.Id);

                return Json(new
                {
                    success = true,
                    message = "Break ended successfully.",
                    actualDuration = actualDuration
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        // GET: Check break status
        [HttpGet]
        public JsonResult GetBreakStatus()
        {
            try
            {
                USERINFO userInfo = (USERINFO)Session["UserInfo"];
                if (userInfo == null)
                {
                    return Json(new { isOnBreak = false }, JsonRequestBehavior.AllowGet);
                }

                var activeBreak = _context.Database.SqlQuery<UserBreakEntryDto>(
                    @"SELECT TOP 1 Id, UserId, StartTime, EndTime, PlannedDurationMinutes, ActualDurationMinutes, BreakReason, CreatedDate
                      FROM UserBreakEntries
                      WHERE UserId = @p0 AND EndTime IS NULL",
                    userInfo.USERID).FirstOrDefault();

                if (activeBreak != null)
                {
                    return Json(new
                    {
                        isOnBreak = true,
                        breakStartTime = activeBreak.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                        plannedDuration = activeBreak.PlannedDurationMinutes,
                        breakReason = activeBreak.BreakReason
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { isOnBreak = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isOnBreak = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string GetBreakReasonFromDuration(int durationMinutes)
        {
            switch (durationMinutes)
            {
                case 5:
                    return "Quick Break";
                case 15:
                    return "Coffee Break";
                case 30:
                    return "Lunch Break";
                default:
                    return "Personal Break";
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
