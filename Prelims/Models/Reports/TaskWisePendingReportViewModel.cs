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
        public int ProcessingCount { get; set; }

        public int QCCount { get; set; }
        public int DeliveryCount { get; set; }
        public int Total { get; set; }
        public int HoldCount { get; set; }
    }
}