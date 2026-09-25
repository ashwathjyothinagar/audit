using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prelims.Models.Reports
{
    public class TaskWisePendingReportViewModel
    {
        public string OfficeName { get; set; }
        public string GroupName { get; set; }
        
        public int CrnId { get; set; }
        public int LVCount { get; set; }

        public int PICount { get; set; }
        public int GICount { get; set; }
        public int StarterCount { get; set; }
        public int NotesCount { get; set; }
        public int Total { get; set; }
        public int HoldCount { get; set; }
    }
}