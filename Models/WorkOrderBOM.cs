using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    public class WorkOrderBOM
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Lệnh sản xuất")]
        public int WorkOrderId { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }

        [Required]
        [Display(Name = "Nguyên phụ liệu")]
        public int ComponentId { get; set; }
        public virtual Product Component { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "Định mức sử dụng")]
        public decimal RequiredQuantity { get; set; }
        // ==> THÊM THUỘC TÍNH MỚI NÀY
        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "Số lượng đã xuất dùng")]
        public decimal ConsumedQuantity { get; set; } = 0;
    }
}