// File: Models/MaterialRequisition.cs
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    public enum RequisitionStatus { Pending, Approved, Issued }

    [Table("MaterialRequisitions")]
    public class MaterialRequisition
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Cho Lệnh sản xuất")]
        public int WorkOrderId { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }

        [Required]
        [Display(Name = "Phân xưởng Yêu cầu")]
        public int WorkshopId { get; set; }
        public virtual Workshop Workshop { get; set; }

        [Required]
        [Display(Name = "Ngày yêu cầu")]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Người yêu cầu")]
        public string RequestedById { get; set; }
        public virtual IdentityUser RequestedBy { get; set; }

        [Required]
        public RequisitionStatus Status { get; set; } = RequisitionStatus.Pending;

        public virtual ICollection<MaterialRequisitionDetail> Details { get; set; } = new List<MaterialRequisitionDetail>();
    }
}