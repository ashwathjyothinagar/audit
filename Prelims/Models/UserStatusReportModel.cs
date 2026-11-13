using System;

namespace Prelims.Models
{
    public class UserStatusReportModel
    {
        public string UserName { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public DateTime? LoggedInTime { get; set; }
        public int? UserTimeEntryId { get; set; }
        public string Office { get; set; }
        public string Task { get; set; }
        public DateTime? LastOrderCompletedDate { get; set; }

        // Break-related properties
        public bool IsOnBreak { get; set; }
        public DateTime? BreakStartTime { get; set; }
        public string BreakReason { get; set; }
        public int CurrentBreakDuration { get; set; } // Current break duration in minutes
        public int TodayBreakCount { get; set; } // Number of breaks taken today
        public int TodayTotalBreakMinutes { get; set; } // Total break minutes for today
    }
}
