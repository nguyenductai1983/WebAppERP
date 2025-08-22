// File mới: Models/ProductionStage.cs
using System.ComponentModel.DataAnnotations;

namespace WebAppERP.Models
{
    public class ProductionStage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên Công đoạn")]
        public string Name { get; set; }        
        // << KHÔI PHỤC LẠI NHƯ BAN ĐẦU >>  
        [Display(Name = "Phân xưởng")]
        public int WorkshopId { get; set; }
        public virtual Workshop Workshop { get; set; }
        // ------------------------------------
        [Display(Name = "Thứ tự")]
        public int Sequence { get; set; } // Dùng để sắp xếp các công đoạn
        // << THÊM TRƯỜNG MỚI NÀY >>
        //[StringLength(100)]
        //[Display(Name = "Controller Ghi nhận Sản lượng")]
        //public string LogControllerName { get; set; } // Ví dụ: "YarnProduction", "FabricProduction"
        [Display(Name = "Loại hình xử lý")]
        public ProductionProcessingMethod ProcessingMethod { get; set; } = ProductionProcessingMethod.None;


    }
}