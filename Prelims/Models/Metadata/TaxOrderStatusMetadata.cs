using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace Prelims.Models.Metadata
{
    public class AuditStatusMetadata
    {
        [Display(Name = "Status")]
        [Required(ErrorMessage = "Status is required")]
        public string Name { get; set; }
    }
}