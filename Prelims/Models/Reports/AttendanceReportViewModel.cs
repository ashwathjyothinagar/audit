using System;
using System.ComponentModel;

namespace Prelims.Models.Reports
{
    public class AttendanceReportViewModel
    {
        public int UserId { get; set; }

        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }

        [DisplayName("Employee Code")]
        public string EmployeeCode { get; set; }

        [DisplayName("Project Manager")]
        public string ProjectManager { get; set; }

        [DisplayName("Date")]
        public string Date { get; set; }

        [DisplayName("Shift")]
        public string Shift { get; set; }

        [DisplayName("Shift Timing (IST)")]
        public string ShiftTiming { get; set; }

        [DisplayName("Login Time")]
        public string LoginTime { get; set; }

        [DisplayName("Order Start Time")]
        public string OrderStartTime { get; set; }

        [DisplayName("Total Break Duration")]
        public string TotalBreakDuration { get; set; }

        [DisplayName("Logout Time")]
        public string LogoutTime { get; set; }

        [DisplayName("Total Orders")]
        public int TotalOrders { get; set; }

        [DisplayName("Comments")]
        public string Comments { get; set; }
    }
}
