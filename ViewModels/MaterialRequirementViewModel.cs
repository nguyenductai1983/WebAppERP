using System.ComponentModel.DataAnnotations;
namespace WebAppERP.ViewModels
{
    public class MaterialRequirementViewModel
    {
        // Thông tin để hiển thị
        public int ProductId { get; set; }
        [Display(Name = "Mã NVL")]
        public string ProductSku { get; set; }
        [Display(Name = "Tên Nguyên vật liệu")]
        public string ProductName { get; set; }
        [Display(Name = "ĐVT")]
        public string UnitOfMeasure { get; set; }
        [Display(Name = "Tổng Nhu cầu")]
        public decimal TotalRequiredQuantity { get; set; }
        [Display(Name = "Tồn kho")]
        public decimal CurrentStockQuantity { get; set; }
        [Display(Name = "Đang mua")]
        public decimal OnOrderQuantity { get; set; }
        // SỬA LỖI: Chuyển các thuộc tính get-only thành thuộc tính có set
        [Display(Name = "Thiếu hụt")]
        public decimal ShortageQuantity { get; set; }
        [Display(Name = "SL Đề xuất Mua")]
        public decimal SuggestedPurchaseQuantity { get; set; }
        // Thuộc tính để xử lý form
        public bool IsSelected { get; set; }
        public int? PreferredSupplierId { get; set; }
    }
}
