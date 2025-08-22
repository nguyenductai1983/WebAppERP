using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    [Table("ProductionLogs")]
    public class ProductionLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Công đoạn sản xuất")]
        public int WorkOrderRoutingId { get; set; }
        public virtual WorkOrderRouting WorkOrderRouting { get; set; }

        [Required]
        [Display(Name = "Sản lượng Ghi nhận")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Ngày Ghi nhận")]
        public DateTime LogDate { get; set; }

        [Required]
        [Display(Name = "Nhân viên")]
        public string OperatorId { get; set; }
        public virtual IdentityUser Operator { get; set; }

        [Display(Name = "Máy móc")]
        public int MachineId { get; set; }
        public virtual Machine Machine { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }

        // --- THÊM THUỘC TÍNH MỚI NÀY ---
        [Display(Name = "Thuộc tính Chi tiết")]
        public string AttributesJson { get; set; }
    }
}