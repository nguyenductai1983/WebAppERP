using System.ComponentModel.DataAnnotations;

namespace WebAppERP.ViewModels
{
    public class CreateCoatingLogViewModel
    {
        public int Id { get; set; }
        public int WorkOrderRoutingId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập sản lượng.")]
        [Display(Name = "Sản lượng Ghi nhận (Số mét)")]
        [Range(1, int.MaxValue, ErrorMessage = "Sản lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhân viên.")]
        [Display(Name = "Nhân viên")]
        public string OperatorId { get; set; }

        [Display(Name = "Máy móc")]
        public int MachineId { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }

        // --- Các thuộc tính đặc thù cho công đoạn Tráng ---
        [Required(ErrorMessage = "Vui lòng nhập định lượng keo.")]
        [Display(Name = "Định lượng keo (g/m²)")]
        public decimal GlueGrammage { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nhiệt độ sấy.")]
        [Display(Name = "Nhiệt độ sấy (°C)")]
        public int DryingTemperature { get; set; }
    }
}