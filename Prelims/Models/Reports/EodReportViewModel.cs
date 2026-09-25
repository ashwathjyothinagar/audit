using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Prelims.Models.Reports
{
    public class EodReportViewModel
    {
        [DisplayName("Sl No")]
        public int SerialNumber { get; set; }

        [DisplayName("Order Number")]
        public string OrderNo { get; set; }

        [DisplayName("Office Name")]
        public string OfficeName { get; set; }

        [DisplayName("Product Type")]
        public string ProductType { get; set; }

        [DisplayName("Requested By")]
        public string RequestedBy { get; set; }

        [DisplayName("County")]
        public string County { get; set; }

        [DisplayName("Instruction")]
        public string Instruction { get; set; }

        [DisplayName("APN No")]
        public string APNNo { get; set; }

        [DisplayName("Address")]
        public string Address { get; set; }

        [DisplayName("Recieved Date Time")]
        public string RecievedDateTime { get; set; }

        [DisplayName("Completed By")]
        public string CompletedBy { get; set; }

        [DisplayName("Completed Date Time")]
        public string CompletedDateTime { get; set; }

        [DisplayName("Did you change Effective Date?")]
        public string EffectiveDateChanged { get; set; }
        
        [DisplayName("Any change in vesting?")]
        public string AnyChangeInVesting { get; set; }

        [DisplayName("Did you update Tax information?")]
        public string AnyUpdateOnTaxInformation { get; set; }

        [DisplayName("Did you update any new PI Docs?")]
        public string AnyUpdateOnNewPIDocs { get; set; }

        [DisplayName("Did you update any new GI Docs?")]
        public string AnyUpdateOnNewGIDocs { get; set; }


        [DisplayName("Did you review 24 month chain of Title?")]
        public string DidYouReviewTwentyFourMonthChainOfTitle { get; set; }

        [DisplayName("Did you upload prelim & SP to smart view?")]
        public string DidYouUploadPrelimAndSPToSmartView { get; set; }

        [DisplayName("Did you send completion email to client?")]
        public string DidYouSendCompletionEmailToClient { get; set; }

        [DisplayName("Requested Office")]
        public string CWLTTitleOfficeName { get; set; }

        [DisplayName("Requested By")]
        public string CWLTTitleOfficerName { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }
        
    }
}