using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Prelims.Models.Metadata
{
    public class AuditRequestTypeMetadata
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(300)]
        public string Name { get; set; }
    }
}