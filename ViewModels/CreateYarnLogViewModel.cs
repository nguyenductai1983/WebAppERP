using System.ComponentModel.DataAnnotations;

namespace WebAppERP.ViewModels
{
    public class CreateYarnLogViewModel
    {
        public int Id { get; set; } // << THÊM DÒNG NÀY
        [Required(ErrorMessage = "Vui lòng chọn kiểu sợi.")] // << THÊM THUỘC TÍNH MỚI NÀY
        [Display(Name = "Kiểu sợi")]
        public int YarnTypeId { get; set; }
        // Thông tin ẩn cần thiết để biết đang ghi nhận cho công đoạn nào
        public int WorkOrderRoutingId { get; set; }

        // Thông tin chung
        [Required(ErrorMessage = "Vui lòng nhập sản lượng.")]
        [Display(Name = "Sản lượng Ghi nhận (Thành phẩm)")]
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

        // --- Các thuộc tính đặc thù cho công đoạn Sợi ---

        [Required(ErrorMessage = "Vui lòng nhập trọng lượng NW.")]
        [Display(Name = "Trọng lượng NW (kg)")]
        public decimal NetWeight { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập trọng lượng GW.")]
        [Display(Name = "Trọng lượng GW (kg)")]
        public decimal GrossWeight { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số ống sợi.")]
        [Display(Name = "Số ống sợi")]
        [Range(1, int.MaxValue, ErrorMessage = "Số ống phải lớn hơn 0.")]
        public int SpoolCount { get; set; }
    }
}