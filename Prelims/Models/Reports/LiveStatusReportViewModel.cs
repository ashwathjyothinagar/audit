using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prelims.Models.Reports
{
    public class LiveStatusReportViewModel
    {
        public string OfficeName { get; set; }
        public string GroupName { get; set; }

        public int LVCount { get; set; }
        public int LVSellerCount { get; set; }
        public int LVBuyerCount { get; set; }
        public int LVBothCount { get; set; }

        public int PICount { get; set; }
        public int PISellerCount { get; set; }
        public int PIBuyerCount { get; set; }
        public int PIBothCount { get; set; }

        public int GICount { get; set; }
        public int GISellerCount { get; set; }
        public int GIBuyerCount { get; set; }
        public int GIBothCount { get; set; }

        public int StarterCount { get; set; }
        public int StarterSellerCount { get; set; }
        public int StarterBuyerCount { get; set; }
        public int StarterBothCount { get; set; }

        public int NotesCount { get; set; }
        public int NotesSellerCount { get; set; }
        public int NotesBuyerCount { get; set; }
        public int NotesBothCount { get; set; }

        public int HoldCount { get; set; }
    }
}