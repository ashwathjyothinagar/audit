using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prelims
{
    public partial class AuditCheck
    {
        [NotMapped]
        public string SelectedOffices { get; set; }

        [NotMapped]
        public List<int> TaskIds { get; set; }


        [NotMapped]
        public string SelectedOfficeNames { get; set; }

        [NotMapped]
        public string TaskNames { get; set; }
    }
}