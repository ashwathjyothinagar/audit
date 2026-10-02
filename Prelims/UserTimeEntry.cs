namespace Prelims
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserTimeEntry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public System.DateTime StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> TotalSeconds { get; set; }
    }
}
