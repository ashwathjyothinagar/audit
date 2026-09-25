using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prelims.Models.Reports
{
    public class LiveStatusReportViewModel
    {
        public string OfficeName { get; set; }
        public string GroupName { get; set; }

        public int ProcessingCount { get; set; }
        public int ProcessingSellerCount { get; set; }
        public int ProcessingBuyerCount { get; set; }
        public int ProcessingBothCount { get; set; }

        public int QCCount { get; set; }
        public int QCSellerCount { get; set; }
        public int QCBuyerCount { get; set; }
        public int QCBothCount { get; set; }

        public int DeliveryCount { get; set; }
        public int DeliverySellerCount { get; set; }
        public int DeliveryBuyerCount { get; set; }
        public int DeliveryBothCount { get; set; }

        public int HoldCount { get; set; }
    }
}