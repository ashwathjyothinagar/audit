using Prelims.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prelims.Models
{
    public class HomeViewModel
    {
        public List<Audit> InprogressAudits { get; set; }

        public List<LiveStatusReportViewModel> LiveStatusItems { get; set; }

        public List<MyOrder> MyOrders { get; set; }
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
    }
}