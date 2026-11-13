using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prelims
{
    [Table("Shifts")]
    public partial class Shift
    {
        public Shift()
        {
            this.ShiftAssignments = new HashSet<ShiftAssignment>();
        }

        public int Id { get; set; }
        public string ShiftName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<ShiftAssignment> ShiftAssignments { get; set; }
    }
}
