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

        [DisplayName("Check Effective Date")]
        public string EffectiveDateChanged { get; set; }
        
        [DisplayName("Vesting same Trust and LLC etc codes and typo in Prelim")]
        public string AnyChangeInVesting { get; set; }

        [DisplayName("Check All Tax shown in Prelim with correct code")]
        public string AnyUpdateOnTaxInformation { get; set; }

        [DisplayName("Retrive DOT and all document Check codes and check typos in prelim")]
        public string AnyUpdateOnNewPIDocs { get; set; }

        [DisplayName("Check all GI matters Typo in Prelim")]
        public string AnyUpdateOnNewGIDocs { get; set; }

        [DisplayName("Check 24 month code and check typos in prelim")]
        public string DidYouReviewTwentyFourMonthChainOfTitle { get; set; }

        [DisplayName("Check Fee Type")]
        public string CheckFeeType { get; set; }

        [DisplayName("Check Policy Type")]
        public string CheckPolicyType { get; set; }

        [DisplayName("Did you check the client instructions")]
        public string DidYouUploadPrelimAndSPToSmartView { get; set; }

        [DisplayName("Did you check the Ops Team instructions")]
        public string DidYouSendCompletionEmailToClient { get; set; }

        [DisplayName("Comments")]
        public string Comments { get; set; }

        [DisplayName("Requested Office")]
        public string CWLTTitleOfficeName { get; set; }

        [DisplayName("Requested By")]
        public string CWLTTitleOfficerName { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }
        
    }
}