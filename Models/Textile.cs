using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    // Tương ứng với bảng VAI
    [Table("Textile")]
    public class Textile
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [ForeignKey("TextileType")]
        [Display(Name = "Loại Dệt")]
        public int TextileTypeId { get; set; }
        public virtual TextileType TextileType { get; set; } = null!;
        [Required]
        [ForeignKey("Machine")]
        [Display(Name = "Máy dệt")]
        public int MachineId { get; set; }
        public virtual Machine Machine { get; set; } = null!;

        [Required]
        [ForeignKey("Operator")]
        [Display(Name = "Nhân viên Vận hành")]
        public string OperatorId { get; set; } = null!;
        public virtual IdentityUser Operator { get; set; } = null!;
        [Required]
        [ForeignKey("Product")]
        [Display(Name = "Sản phẩm Sợi")] // Tên hiển thị cũng thay đổi cho rõ ràng
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        [Required]
        [StringLength(50)]
        [Display(Name = "Mã cây vải")]
        public string Code { get; set; } = null!;

        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Khổ thực tế (m)")]
        public decimal ActualWidth { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Số mét ban đầu")]
        public decimal InitialLength { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Trọng lượng GW")]
        public decimal GrossWeight { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Trọng lượng NW")]
        public decimal NetWeight { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "Tồn kho")]
        public decimal StockQuantity { get; set; }
        [Display(Name = "Chất lượng")]
        public bool Quality { get; set; } // Dùng bool (true/false) cho Đạt/Không đạt
                                          // Thêm trường để truy vết nguồn gốc Sợi đã dùng
        [Display(Name = "Lô Sợi Nguồn")]
        public int? SourceYarnId { get; set; }
        public int? ProductionLogId { get; set; }
        public virtual ProductionLog ProductionLog { get; set; }
        public virtual Yarn SourceYarn { get; set; }
        public int? WorkOrderId { get; set; } // int? cho phép giá trị null
        public virtual WorkOrder WorkOrder { get; set; }
        public StockItemStatus Status { get; set; } = StockItemStatus.InStock;
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
