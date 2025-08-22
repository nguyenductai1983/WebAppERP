using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    public class BillOfMaterial
    {
        public int Id { get; set; }

        // Foreign Key đến Thành phẩm (cha)
        [Display(Name = "Thành phẩm")]
        public int FinishedProductId { get; set; }
        public virtual Product FinishedProduct { get; set; }

        // Foreign Key đến Công đoạn
        [Display(Name = "Sử dụng tại Công đoạn")]
        public int ProductionStageId { get; set; }
        public virtual ProductionStage ProductionStage { get; set; }

        // Foreign Key đến Nguyên phụ liệu (con)
        [Display(Name = "Nguyên phụ liệu")]
        public int ComponentId { get; set; }
        public virtual Product Component { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "Định mức sử dụng")]
        public decimal Quantity { get; set; } // Là "Số met cần dùng" nếu NPL là vải, là "Số lượng" nếu là nút, nhãn...

        // --- CÁC TRƯỜNG MỚI ĐỂ TÍNH TOÁN ---
        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "Chiều dài (m)")]
        public decimal? PieceLength { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "Chiều rộng (m)")]
        public decimal? PieceWidth { get; set; }

        [Display(Name = "Số lượng / SP")]
        public int? PiecesPerProduct { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "GSM (g/m²)")]
        public decimal? Gsm { get; set; } // Grams per Square Meter
        [StringLength(250)]
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }
    }
}