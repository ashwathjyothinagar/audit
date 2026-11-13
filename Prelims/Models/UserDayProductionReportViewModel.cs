using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Prelims.Models
{
    public class UserDayProductionReportViewModel
    {
        [Display(Name = "App Name")]
        public string App { get; set; }

        [Display(Name = "User ID")]
        public int UserId { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNo { get; set; }

        [Display(Name = "Received Date & Time")]
        public DateTime ReceivedDateTime { get; set; }

        [Display(Name = "CRN Name")]
        public string CRNName { get; set; }

        [Display(Name = "Task Name")]
        public string TaskName { get; set; }

        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        [Display(Name = "Time Taken (Min)")]
        public int TimeTakenMin { get; set; }

        [Display(Name = "No Of Exceptions")]
        public string NoOfExceptions { get; set; }

        [Display(Name = "Status Name")]
        public string StatusName { get; set; }
    }

    public class AuditTaskSummaryModel
    {
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Taxes (Hours)")]
        public string TaxesHours { get; set; }

        [Display(Name = "L&V (Hours)")]
        public string LVHours { get; set; }

        [Display(Name = "PI (Hours)")]
        public string PIHours { get; set; }

        [Display(Name = "GI (Hours)")]
        public string GIHours { get; set; }

        [Display(Name = "Starter (Hours)")]
        public string StarterHours { get; set; }

        [Display(Name = "Audit Proofing (Hours)")]
        public string ProofingHours { get; set; }

        [Display(Name = "Total (Hours)")]
        public string TotalHours { get; set; }
    }

    public class AuditTaskCountSummaryModel
    {
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Taxes (Count)")]
        public int TaxesCount { get; set; }

        [Display(Name = "L&V (Count)")]
        public int LVCount { get; set; }

        [Display(Name = "PI (Count)")]
        public int PICount { get; set; }

        [Display(Name = "GI (Count)")]
        public int GICount { get; set; }

        [Display(Name = "Starter (Count)")]
        public int StarterCount { get; set; }

        [Display(Name = "Audit Proofing (Count)")]
        public int ProofingCount { get; set; }

        [Display(Name = "Total (Count)")]
        public int TotalCount { get; set; }
    }

    public class UserBreakEntryModel
    {
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Display(Name = "End Time")]
        public DateTime? EndTime { get; set; }

        [Display(Name = "Planned Duration (Min)")]
        public int PlannedDurationMinutes { get; set; }

        [Display(Name = "Actual Duration (Min)")]
        public int? ActualDurationMinutes { get; set; }

        [Display(Name = "Break Reason")]
        public string BreakReason { get; set; }
    }

    public class UserDayProductionReportDetailViewModel
    {
        [Display(Name = "Report Start Date & Time")]
        public DateTime StartDateTime { get; set; }

        public List<UserDayProductionReportViewModel> ProductionDetails { get; set; }

        public List<AuditTaskSummaryModel> AuditTimeSummary { get; set; }
        public List<AuditTaskCountSummaryModel> AuditCountSummary { get; set; }
        public List<UserBreakEntryModel> UserBreakEntries { get; set; }

        public UserDayProductionReportDetailViewModel()
        {
            ProductionDetails = new List<UserDayProductionReportViewModel>();
            AuditTimeSummary = new List<AuditTaskSummaryModel>();
            AuditCountSummary = new List<AuditTaskCountSummaryModel>();
            UserBreakEntries = new List<UserBreakEntryModel>();
        }
    }
}
