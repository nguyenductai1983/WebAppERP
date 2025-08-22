using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace WebAppERP.ViewModels
{
    public class CreateFabricLogViewModel
    {
        public int Id { get; set; }
        public int WorkOrderRoutingId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập sản lượng.")]
        [Display(Name = "Sản lượng Ghi nhận (Số cây vải)")]
        [Range(1, int.MaxValue, ErrorMessage = "Sản lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhân viên.")]
        [Display(Name = "Nhân viên")]
        public string OperatorId { get; set; }

        [Display(Name = "Máy móc")]
        public int MachineId { get; set; }
        [Display(Name = "Sợi đã tiêu thụ")]
        public int ConsumedYarnId { get; set; }
        [Display(Name = "Số lượng tiêu thụ")]
        public int QuantityConsumed { get; set; }
        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }
        // --- THÔNG TIN ĐẦU VÀO (INPUT) ---
        public List<ConsumedMaterialInput> ConsumedYarns { get; set; } = new List<ConsumedMaterialInput>();
        // --- Các thuộc tính đặc thù cho công đoạn Dệt ---
        [Required(ErrorMessage = "Vui lòng nhập chiều dài.")]
        [Display(Name = "Chiều dài (mét)")]
        public decimal Length { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập khổ rộng.")]
        [Display(Name = "Khổ rộng thực tế (mét)")]
        public decimal Width { get; set; }
    }
}