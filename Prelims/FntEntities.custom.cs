using System.Data.Entity;

namespace Prelims
{
    public partial class FntEntities
    {
        public virtual DbSet<UserBreakEntry> UserBreakEntries { get; set; }
        public virtual DbSet<Shift> Shifts { get; set; }
        public virtual DbSet<ShiftAssignment> ShiftAssignments { get; set; }
    }
}
