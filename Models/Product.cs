using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    // Thêm enum này để phân loại sản phẩm
    public enum ProductType
    {
        [Display(Name = "Nguyên phụ liệu")]
        RawMaterial, // Mua vào, không có BOM

        [Display(Name = "Bán thành phẩm")]
        SemiFinishedGood, // Sản xuất ra, vừa là đầu ra của công đoạn trước, vừa là đầu vào của công đoạn sau. CÓ BOM.

        [Display(Name = "Thành phẩm")]
        FinishedGood // Sản xuất ra để bán. CÓ BOM.
    }
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Loại sản phẩm")]
        public ProductType Type { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [StringLength(50)]
        [Display(Name = "Mã SKU")]
        public string Sku { get; set; } // Stock Keeping Unit

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Giá bán")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Số lượng tồn kho")]
        [Column(TypeName = "decimal(18, 4)")] 
        public decimal Quantity { get; set; }  

        // --- THÊM CÁC THUỘC TÍNH CHI PHÍ ---
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Giá vốn / Giá mua")]
        public decimal Cost { get; set; } // Dùng cho Nguyên vật liệu

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Chi phí nhân công chuẩn")]
        public decimal StandardLaborCost { get; set; } // Dùng cho Thành phẩm

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Chi phí chung chuẩn")]
        public decimal StandardOverheadCost { get; set; } // Dùng cho Thành phẩm
        [StringLength(50)]
        [Display(Name = "Đơn vị tính")]
        public string UnitOfMeasure { get; set; } // Ví dụ: "m", "kg", "cái", "cuộn"

        [StringLength(50)]
        [Display(Name = "Màu sắc")]
        public string Color { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Khổ vải (m)")]
        public decimal? Width { get; set; } // Dùng decimal? để cho phép null với các NPL không phải vải

        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "GSM (g/m²)")]
        public decimal? Gsm { get; set; } // Grams per Square Meter
        [StringLength(50)]
        [Display(Name = "Tên định danh Bảng Lô")]
        public string LotEntityName { get; set; }

        [Display(Name = "Công đoạn sản xuất chính")]
        public int? DefaultProductionStageId { get; set; }
        public virtual ProductionStage DefaultProductionStage { get; set; }
    }
}