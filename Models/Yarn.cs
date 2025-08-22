using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    public enum StockItemStatus
    {
        [Display(Name = "Trong Kho")]
        InStock,
        [Display(Name = "Đã sử dụng")]
        Consumed,
        [Display(Name = "Tạm giữ")]
        OnHold
    }
    [Table("Yarn")]
    public class Yarn
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [ForeignKey("Product")]
        [Display(Name = "Sản phẩm Sợi")] // Tên hiển thị cũng thay đổi cho rõ ràng
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        [Required]
        [ForeignKey("Operator")]
        [Display(Name = "Nhân viên Vận hành")]
        public string OperatorId { get; set; } = null!;
        public virtual IdentityUser Operator { get; set; } = null!;
        // --- THÊM LIÊN KẾT VỚI MÁY MÓC ---
        [Required]
        [ForeignKey("Machine")]
        [Display(Name = "Máy sản xuất")]
        public int? MachineId { get; set; }
        public virtual Machine Machine { get; set; } = null!;
        // ------------------------------------

        [Display(Name = "Tổng Số Ống")]
        public int SpoolCount { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Trọng Lượng GW")]
        public decimal GrossWeight { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Trọng Lượng NW")]
        public decimal NetWeight { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Trọng Lượng Ống")]
        public decimal SpoolWeight { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Tồn kho (kg)")]
        public decimal StockQuantity { get; set; }
        [Display(Name = "Từ Lệnh sản xuất")]
        public int? WorkOrderId { get; set; } // int? cho phép giá trị null
        public virtual WorkOrder WorkOrder { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        [Display(Name = "Trạng thái Lô")]
        public StockItemStatus Status { get; set; } = StockItemStatus.InStock;
       public int? ProductionLogId { get; set; }
        public virtual ProductionLog ProductionLog { get; set; }
         
    }
}
