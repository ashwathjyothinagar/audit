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

        [DisplayName("Recieved Date Time")]
        public string RecievedDateTime { get; set; }

        [DisplayName("Taxes done by")]
        public string TaxesDoneBy { get; set; }

        [DisplayName("Taxes Start Time")]
        public string TaxesStartTime { get; set; }

        [DisplayName("Taxes End Time")]
        public string TaxesEndTime { get; set; }

        [DisplayName("Taxes Time Taken")]
        public string TaxesTimeTaken { get; set; }

        [DisplayName("Taxes Comments")]
        public string TaxesComments { get; set; }

        [DisplayName("L&V done by")]
        public string ProcessingDoneBy { get; set; }

        [DisplayName("L&V Start Time")]
        public string ProcessingStartTime { get; set; }

        [DisplayName("L&V End Time")]
        public string ProcessingEndTime { get; set; }

        [DisplayName("L&V Time Taken")]
        public string ProcessingTimeTaken { get; set; }

        [DisplayName("L&V Comments")]
        public string ProcessingComments { get; set; }

        [DisplayName("PI done by")]
        public string QCDoneBy { get; set; }

        [DisplayName("PI Start Time")]
        public string QCStartTime { get; set; }

        [DisplayName("PI End Time")]
        public string QCEndTime { get; set; }

        [DisplayName("PI Comments")]
        public string QCComments { get; set; }

        [DisplayName("Error Found")]
        public string AnyCriticalErrors { get; set; }

        [DisplayName("Errors")]
        public List<AuditError> AuditErrors { get; set; }

        [DisplayName("PI Time Taken")]
        public string QCTimeTaken { get; set; }

        [DisplayName("GI done by")]
        public string DeliveryDoneBy { get; set; }

        [DisplayName("GI Start Time")]
        public string DeliveryStartTime { get; set; }

        [DisplayName("GI End Time")]
        public string DeliveryEndTime { get; set; }

        [DisplayName("GI Time Taken")]
        public string DeliveryTimeTaken { get; set; }

        [DisplayName("GI Comments")]
        public string DeliveryComments { get; set; }

        [DisplayName("Starter done by")]
        public string StarterDoneBy { get; set; }

        [DisplayName("Starter Start Time")]
        public string StarterStartTime { get; set; }

        [DisplayName("Starter End Time")]
        public string StarterEndTime { get; set; }

        [DisplayName("Starter Time Taken")]
        public string StarterTimeTaken { get; set; }

        [DisplayName("Starter Comments")]
        public string StarterComments { get; set; }

        [DisplayName("Notes done by")]
        public string NotesDoneBy { get; set; }

        [DisplayName("Notes Start Time")]
        public string NotesStartTime { get; set; }

        [DisplayName("Notes End Time")]
        public string NotesEndTime { get; set; }

        [DisplayName("Notes Time Taken")]
        public string NotesTimeTaken { get; set; }

        [DisplayName("Notes Comments")]
        public string NotesComments { get; set; }
    }
}