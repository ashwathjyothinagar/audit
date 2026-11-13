using System.ComponentModel;

namespace Prelims.Models.Reports
{
    public class PendingOrdersReportViewModel
    {
        [DisplayName("Sl No")]
        public int SlNo { get; set; }

        [DisplayName("Order No")]
        public string OrderNo { get; set; }

        [DisplayName("Office Name")]
        public string OfficeName { get; set; }

        [DisplayName("Received Date")]
        public string ReceivedDate { get; set; }

        [DisplayName("Product Type")]
        public string ProductType { get; set; }

        [DisplayName("Requested By")]
        public string RequestedBy { get; set; }

        [DisplayName("Task")]
        public string Task { get; set; }

        [DisplayName("User")]
        public string User { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }
    }
}
