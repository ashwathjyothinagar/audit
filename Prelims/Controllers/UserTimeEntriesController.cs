using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Prelims.Controllers
{
    [Authorize(Roles = "Admin")]
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

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
