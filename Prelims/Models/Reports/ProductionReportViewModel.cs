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

        [DisplayName("L&V done by")]
        public string ProcessingDoneBy { get; set; }

        [DisplayName("Start Time")]
        public string ProcessingStartTime { get; set; }

        [DisplayName("End Time")]
        public string ProcessingEndTime { get; set; }

        [DisplayName("Time Taken")]
        public string ProcessingTimeTaken { get; set; }

        [DisplayName("PI done by")]
        public string QCDoneBy { get; set; }

        [DisplayName("Start Time")]
        public string QCStartTime { get; set; }

        [DisplayName("End Time")]
        public string QCEndTime { get; set; }

        [DisplayName("PI Comments")]
        public string QCComments { get; set; }

        [DisplayName("Error Found")]
        public string AnyCriticalErrors { get; set; }

        [DisplayName("Errors")]
        public List<AuditError> AuditErrors { get; set; }

        [DisplayName("Time Taken")]
        public string QCTimeTaken { get; set; }

        [DisplayName("GI done by")]
        public string DeliveryDoneBy { get; set; }

        [DisplayName("Start Time")]
        public string DeliveryStartTime { get; set; }

        [DisplayName("End Time")]
        public string DeliveryEndTime { get; set; }

        [DisplayName("Time Taken")]
        public string DeliveryTimeTaken { get; set; }

        [DisplayName("Starter done by")]
        public string StarterDoneBy { get; set; }

        [DisplayName("Start Time")]
        public string StarterStartTime { get; set; }

        [DisplayName("End Time")]
        public string StarterEndTime { get; set; }

        [DisplayName("Time Taken")]
        public string StarterTimeTaken { get; set; }

        [DisplayName("Notes done by")]
        public string NotesDoneBy { get; set; }

        [DisplayName("Start Time")]
        public string NotesStartTime { get; set; }

        [DisplayName("End Time")]
        public string NotesEndTime { get; set; }

        [DisplayName("Time Taken")]
        public string NotesTimeTaken { get; set; }
    }
}


//L&V
//PI
//GI
//Starter
//Notes