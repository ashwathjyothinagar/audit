using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prelims.Models
{
    public class AuditProductionModel
    {
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        public string Updates { get; set; }

        public int AuditTaskId { get; set; }
        public int OwnerSearchCount { get; set; }
        public bool IsDirectHitUploaded { get; set; }
        
        [UIHint("YesNo")]
        public bool? EffectiveDateChanged { get; set; }
        [UIHint("YesNo")]
        public bool? AnyChangeInVesting { get; set; }
        [UIHint("YesNo")]
        public bool? AnyUpdateOnTaxInformation { get; set; }
        [UIHint("YesNo")]
        public bool? AnyUpdateOnNewPIDocs { get; set; }
        [UIHint("YesNo")]
        public bool? AnyUpdateOnNewGIDocs { get; set; }

        [UIHint("YesNo")]
        public bool? DidYouReviewTwentyFourMonthChainOfTitle { get; set; }

        [UIHint("YesNo")]
        public bool? DidYouUploadPrelimAndSPToSmartView { get; set; }
        [UIHint("YesNo")]
        public bool? DidYouSendCompletionEmailToClient { get; set; }

        [UIHint("YesNo")]
        public bool? AnyPostingFoundInPIGI { get; set; }


        public string OwnerName { get; set; }
        public bool IsQcRequired { get; set; }

        public AuditTimeEntry TimeEntry { get; set; }
        public Audit Audit { get; set; }
        public int AuditId { get; set; }
        public int TaxTimeEntryId { get; set; }

        public List<AuditTimeEntry> TimeEntries { get; set; }
        public List<AuditUpdate> AuditUpdates { get; set; }

        public string OrderErrorJson { get; set; }
        public string AnyOrderErrors { get; set; }

        public string CheckText { get; set; }
        public List<AuditCheckModel> Checks { get; set; }

        public string CWLTTitleOfficeName { get; set; }
        public string CWLTTitleOfficerName { get; set; }

        public Nullable<System.DateTime> AmendmentDate { get; set; }
    }

    public class AuditCheckModel
    {
        public int AuditCheckId { get; set; }
        public string Name { get; set; }
        public List<string> AuditCheckValues { get; set; }
        public string SelectedValue { get; set; }
        public bool IsDropdownList { get; set; }
    }
}