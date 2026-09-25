using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prelims.Models.Reports
{
    public class TaskWisePendingReportDetailViewModel
    {
        public List<Audit> Audits { get; set; }

        public bool IsHoldOrders { get; set; }
    }


}