using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Prelims.Models.Metadata
{
    public class AuditMetadata
    {
        [StringLength(200)]
        [Display(Name = "Order No")]
        [Required(ErrorMessage ="Order No is required")]
        public string OrderNo { get; set; }

        [StringLength(300)]
        [Display(Name = "Owner Name")]
        [DataType(DataType.MultilineText)]
        public string OwnerNames { get; set; }

        [Required(ErrorMessage = "Request By is required")]
        public Nullable<int> SenderId { get; set; }

        [Required(ErrorMessage = "Office Name is required")]
        public int CrnId { get; set; }

        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Address")]
        public string PropertyAddress { get; set; }

        [StringLength(200)]
        [Display(Name = "APN No")]
        public string APNNo { get; set; }

        [Display(Name ="Request Type")]
        public int RequestTypeId { get; set; }

        [Display(Name = "Status")]
        public int StatusId { get; set; }

        [Display(Name = "Task")]
        public int TaskId { get; set; }

        [StringLength(200)]
        [Display(Name = "Requested By Email Id")]
        public string RequestedByEmailId { get; set; }

        [Display(Name = "Instructions")]
        [DataType(DataType.MultilineText)]
        public string ClientInstructions { get; set; }

        [Display(Name = "Rush Order")]
        public bool IsRushOrder { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Created")]
        public System.DateTime DateCreated { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "User Created")]
        public int UserCreated { get; set; }

        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> DateModified { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<int> UserModified { get; set; }

        public virtual AuditStatus AuditStatus { get; set; }
        public virtual AuditTask AuditTask { get; set; }
        public virtual AuditRequestType AuditRequestType { get; set; }
    }
}