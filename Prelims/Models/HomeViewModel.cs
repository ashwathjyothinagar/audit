using Prelims.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prelims.Models
{
    public class HomeViewModel
    {
        public List<AuditTimeEntryViewModel> InprogressAudits { get; set; }

        public List<LiveStatusReportViewModel> LiveStatusItems { get; set; }

        public List<MyOrder> MyOrders { get; set; }

        public bool IsAllowedToViewInprogressOrders { get; set; }
    }

    public class AuditTimeEntryViewModel
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public DateTime StartTime { get; set; }
        public string CountyName { get; set; }
        public string CRNName { get; set; }
        public string RequestTypeName { get; set; }
        public string TaskName { get; set; }
        public bool IsRushOrder { get; set; }
        public string StatusName { get; set; }
        public DateTime? ReceivedDateTime { get; set; }
        public DateTime? FFAssignedDate { get; set; }
        public DateTime? TOT { get; set; }
        public string UserName { get; set; }
        public int? CodesCount { get; set; }
        public string TimeTaken { get; set; }
        public string Location { get; set; }
    }

    public class MyOrder
    {
        public int Id { get; set; }
        public string COUNTYNAME { get; set; }
        public string OrderNo { get; set; }
        public string CRNDISPLAYNAME { get; set; }
        public string TitleOrderRequestTypeName { get; set; }
        public bool? IsRushOrder { get; set; }
        public string TitleOrderTaskName { get; set; }
        public DateTime? ReceivedDateTime { get; set; }
        public DateTime? FFAssignedDate { get; set; }
        public DateTime? TOT { get; set; }
    }
}
