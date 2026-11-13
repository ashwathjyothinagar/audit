using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prelims
{
    [Table("ShiftAssignments")]
    public partial class ShiftAssignment
    {
        public int Id { get; set; }
        public int ShiftScheduleId { get; set; }
        public int UserId { get; set; }
        public int ShiftId { get; set; }
        public DateTime ShiftDate { get; set; }
        public bool? IsProductionEmailSent { get; set; }

        public virtual Shift Shift { get; set; }
        public virtual USERINFO USERINFO { get; set; }
    }
}
