using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prelims.Models
{
    public class UserReportViewModel
    {

        public string UserName { get; set; }
        public string Location { get; set; }

        public int ProcessingCount { get; set; }
        public int ProcessingUpdatesCount { get; set; }
        public int ProcessingDatedownsCount { get; set; }
        public int ProcessingOtherCount { get; set; }

        public int QCCount { get; set; }
        public int QCUpdatesCount { get; set; }
        public int QCDatedownsCount { get; set; }
        public int QCOtherCount { get; set; }

        public int DeliveryCount { get; set; }
        public int DeliveryUpdatesCount { get; set; }
        public int DeliveryDatedownsCount { get; set; }
        public int DeliveryOtherCount { get; set; }

        public int AuditCount { get; set; }
        public int AuditUpdatesCount { get; set; }
        public int AuditDatedownsCount { get; set; }
        public int AuditOtherCount { get; set; }
    }
}