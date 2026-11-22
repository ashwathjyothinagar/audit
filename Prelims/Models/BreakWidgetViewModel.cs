using System;

namespace Prelims.Models
{
    public class BreakWidgetViewModel
    {
        public bool IsLoggedIn { get; set; }
        public TimeSpan ActiveTime { get; set; }
        public int? ActiveEntryId { get; set; }
        public int TotalBreakCount { get; set; }
        public int TotalBreakMinutes { get; set; }
    }

    public class UserBreakEntryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int PlannedDurationMinutes { get; set; }
        public int? ActualDurationMinutes { get; set; }
        public string BreakReason { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
