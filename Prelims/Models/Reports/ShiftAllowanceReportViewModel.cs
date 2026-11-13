using System.Collections.Generic;

namespace Prelims.Models.Reports
{
    public class ShiftAllowanceReportViewModel
    {
        public List<ShiftAllowanceReportRow> Rows { get; set; }

        public ShiftAllowanceReportViewModel()
        {
            Rows = new List<ShiftAllowanceReportRow>();
        }
    }

    public class ShiftAllowanceReportRow
    {
        public int SerialNumber { get; set; }
        public int UserId { get; set; }
        public string EmpCode { get; set; }
        public string Name { get; set; }
        public string Process { get; set; }
        public string Location { get; set; }

        public int TotalWorkingDays { get; set; }
        public int LeavesAvailed { get; set; }
        public int DayShiftCount { get; set; }
        public int NightShiftCount { get; set; }
        public int NSACount { get; set; }
        public string ProdPercent { get; set; }

        // Detail lists for popup modals
        public List<string> LeaveDates { get; set; }
        public List<string> DayShiftDates { get; set; }
        public List<string> NightShiftDates { get; set; }

        public ShiftAllowanceReportRow()
        {
            LeaveDates = new List<string>();
            DayShiftDates = new List<string>();
            NightShiftDates = new List<string>();
        }
    }
}
