using System.ComponentModel.DataAnnotations;

namespace WebAppERP.ViewModels
{
    // Lớp này đại diện cho một dòng nguyên liệu đầu vào trên form
    public class ConsumedMaterialInput
    {
        [Required(ErrorMessage = "Vui lòng chọn lô Sợi.")]
        [Display(Name = "Lô Sợi đã sử dụng")]
        public int ConsumedYarnId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng.")]
        [Display(Name = "Số lượng Sợi đã dùng (kg)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public decimal QuantityConsumed { get; set; }
    }
}