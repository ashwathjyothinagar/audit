using Prelims.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Prelims.Models.Reports
{
    public class ProductionReportViewModel
    {
        [DisplayName("Sl No")]
        public int SerialNumber { get; set; }

        [DisplayName("Order Number")]
        public string OrderNo { get; set; }

        [DisplayName("Office Name")]
        public string OfficeName { get; set; }

        [DisplayName("Product Type")]
        public string ProductType { get; set; }

        [DisplayName("Sub Type")]
        public string SubType { get; set; }

        [DisplayName("Recieved Date Time")]
        public string RecievedDateTime { get; set; }

        // --- Exam Task (from TitleOrder Examining Task) ---
        [DisplayName("Exam done by")]
        public string ExamDoneBy { get; set; }

        [DisplayName("Start Time")]
        public string ExamStartTime { get; set; }

        [DisplayName("End Time")]
        public string ExamEndTime { get; set; }

        [DisplayName("Time Taken")]
        public string ExamTimeTaken { get; set; }

        [DisplayName("Error Found")]
        public string ExamErrorFound { get; set; }

        [DisplayName("Error Categorys")]
        public string ExamErrorCategories { get; set; }

        [DisplayName("Error Type")]
        public string ExamErrorType { get; set; }

        [DisplayName("Error done by")]
        public string ExamErrorDoneBy { get; set; }

        [DisplayName("Error Comments")]
        public string ExamErrorComments { get; set; }

        // --- L&V Task ---
        [DisplayName("L&V done by")]
        public string LVDoneBy { get; set; }

        [DisplayName("Start Time")]
        public string LVStartTime { get; set; }

        [DisplayName("End Time")]
        public string LVEndTime { get; set; }

        [DisplayName("Time Taken")]
        public string LVTimeTaken { get; set; }

        [DisplayName("Error Found")]
        public string LVErrorFound { get; set; }

        [DisplayName("Error Categorys")]
        public string LVErrorCategories { get; set; }

        [DisplayName("Error Type")]
        public string LVErrorType { get; set; }

        [DisplayName("Error done by")]
        public string LVErrorDoneBy { get; set; }

        [DisplayName("Error Comments")]
        public string LVErrorComments { get; set; }

        // --- PI Task ---
        [DisplayName("PI done by")]
        public string PIDoneBy { get; set; }

        [DisplayName("Start Time")]
        public string PIStartTime { get; set; }

        [DisplayName("End Time")]
        public string PIEndTime { get; set; }

        [DisplayName("Time Taken")]
        public string PITimeTaken { get; set; }

        [DisplayName("Error Found")]
        public string PIErrorFound { get; set; }

        [DisplayName("Error Categorys")]
        public string PIErrorCategories { get; set; }

        [DisplayName("Error Type")]
        public string PIErrorType { get; set; }

        [DisplayName("Error done by")]
        public string PIErrorDoneBy { get; set; }

        [DisplayName("Error Comments")]
        public string PIErrorComments { get; set; }

        // --- GI Task ---
        [DisplayName("GI done by")]
        public string GIDoneBy { get; set; }

        [DisplayName("Start Time")]
        public string GIStartTime { get; set; }

        [DisplayName("End Time")]
        public string GIEndTime { get; set; }

        [DisplayName("Time Taken")]
        public string GITimeTaken { get; set; }

        [DisplayName("Error Found")]
        public string GIErrorFound { get; set; }

        [DisplayName("Error Categorys")]
        public string GIErrorCategories { get; set; }

        [DisplayName("Error Type")]
        public string GIErrorType { get; set; }

        [DisplayName("Error done by")]
        public string GIErrorDoneBy { get; set; }

        [DisplayName("Error Comments")]
        public string GIErrorComments { get; set; }

        // --- Starter Code Task ---
        [DisplayName("Starter Code done by")]
        public string StarterCodeDoneBy { get; set; }

        [DisplayName("Start Time")]
        public string StarterCodeStartTime { get; set; }

        [DisplayName("End Time")]
        public string StarterCodeEndTime { get; set; }

        [DisplayName("Time Taken")]
        public string StarterCodeTimeTaken { get; set; }

        [DisplayName("Error Found")]
        public string StarterCodeErrorFound { get; set; }

        [DisplayName("Error Categorys")]
        public string StarterCodeErrorCategories { get; set; }

        [DisplayName("Error Type")]
        public string StarterCodeErrorType { get; set; }

        [DisplayName("Error done by")]
        public string StarterCodeErrorDoneBy { get; set; }

        [DisplayName("Error Comments")]
        public string StarterCodeErrorComments { get; set; }
    }
}