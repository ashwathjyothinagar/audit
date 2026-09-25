using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prelims.Models
{
    public class UserReportViewModel
    {

        public string UserName { get; set; }
        public string Location { get; set; }

        public int LVCount { get; set; }
        public int LVUpdatesCount { get; set; }
        public int LVDatedownsCount { get; set; }
        public int LVOtherCount { get; set; }

        public int PICount { get; set; }
        public int PIUpdatesCount { get; set; }
        public int PIDatedownsCount { get; set; }
        public int PIOtherCount { get; set; }

        public int GICount { get; set; }
        public int GIUpdatesCount { get; set; }
        public int GIDatedownsCount { get; set; }
        public int GIOtherCount { get; set; }

        public int StarterCount { get; set; }
        public int StarterUpdatesCount { get; set; }
        public int StarterDatedownsCount { get; set; }
        public int StarterOtherCount { get; set; }

        public int NotesCount { get; set; }
        public int NotesUpdatesCount { get; set; }
        public int NotesDatedownsCount { get; set; }
        public int NotesOtherCount { get; set; }

        public int AuditCount { get; set; }
        public int AuditUpdatesCount { get; set; }
        public int AuditDatedownsCount { get; set; }
        public int AuditOtherCount { get; set; }
    }
}