using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    [Table("MaterialConsumptionLogs")]
    public class MaterialConsumptionLog
    {
        [Key]
        public int Id { get; set; }

        // Liên kết đến lần sản xuất đã ghi nhận
        [Required]
        public int ProductionLogId { get; set; }
        public virtual ProductionLog ProductionLog { get; set; }

        // Liên kết đến YÊU CẦU vật tư cụ thể trong Lệnh sản xuất
        [Required]
        [Display(Name = "Yêu cầu NVL")]
        public int WorkOrderBOMId { get; set; }
        public virtual WorkOrderBOM WorkOrderBOM { get; set; }

        // ID của lô Bán thành phẩm đã dùng (ví dụ: Yarn.ID, Textile.ID).
        // Sẽ là NULL nếu xuất dùng Nguyên liệu thô (không quản lý theo lô).
        [Display(Name = "Lô BTP đã dùng")]
        public int? ConsumedLotId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "Số lượng xuất dùng lần này")]
        public decimal QuantityConsumed { get; set; }
    }
}