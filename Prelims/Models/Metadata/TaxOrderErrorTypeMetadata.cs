using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Prelims.Models.Metadata
{
    public class AuditErrorTypeMetadata
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(500)]
        public string Name { get; set; }

        [Display(Name = "Error Category")]
        [Required(ErrorMessage = "Error Category is required")]
        public int AuditErrorCategoryId { get; set; }

        [Display(Name = "Is Critical")]
        [Required(ErrorMessage = "Is Critical is required")]
        public bool IsCritical { get; set; }
    }

    public class AuditErrorCategoryMetadata
    {
        [Display(Name = "Category Name")]
        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(500)]
        public string Name { get; set; }
    }
}