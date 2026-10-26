using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Prelims.Controllers;

namespace Prelims
{
    [MetadataType(typeof(Prelims.Models.Metadata.AuditMetadata))]
    public partial class Audit
    {
        [NotMapped]
        public string UserAssignedName { get; set; }

        [NotMapped]
        public List<AuditError> Errors { get; set; }

        [NotMapped]
        public int? MoreThanCodeValue { get; set; }

        [NotMapped]
        public int? CodesCount
        {
            get { return MoreThanCodeValue; }
            set { MoreThanCodeValue = value; }
        }

        [NotMapped]
        public DateTime? FFAssignedDate { get; set; }

        [NotMapped]
        public DateTime? TOT { get; set; }

        [NotMapped]
        public string TimeTaken { get; set; }
    }
}
