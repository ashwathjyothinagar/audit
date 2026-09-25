using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prelims.Models.Reports
{
    public class HoldOrderReportViewModel
    {
        public string OfficeName { get; set; }
        public int CrnId { get; set; }
        public int Count { get; set; }
    }
}