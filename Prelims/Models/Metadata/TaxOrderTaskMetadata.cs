using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Prelims.Models.Metadata
{
    public class AuditTaskMetadata
    {
        [Display(Name = "Task")]
        [Required(ErrorMessage = "Task is required")]
        public string Name { get; set; }
    }
}