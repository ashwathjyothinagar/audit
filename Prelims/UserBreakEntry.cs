using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prelims
{
    [Table("UserBreakEntries")]
    public partial class UserBreakEntry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int PlannedDurationMinutes { get; set; }
        public int? ActualDurationMinutes { get; set; }
        public string BreakReason { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual USERINFO USERINFO { get; set; }
    }
}
