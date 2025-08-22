using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebAppERP.Models
{
    public class SalesOrderDetail
    {
        public int Id { get; set; }

        // Foreign Key đến đơn hàng chính
        public int SalesOrderId { get; set; }
        public virtual SalesOrder SalesOrder { get; set; }

        // Foreign Key đến sản phẩm
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Đơn giá")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; } // Lưu lại giá tại thời điểm bán
                                               // --- Các thuộc tính động từ đơn hàng ---
        [StringLength(50)]
        [Display(Name = "Màu sắc")]
        public string Color { get; set; }

        [Display(Name = "GSM / GLM")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal GsmOrGlm { get; set; }

        [Display(Name = "Lamination")]
        public bool HasLamination { get; set; }

        [Display(Name = "Khổ vải (m)")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal FabricWidth { get; set; }

        [Display(Name = "Số mét")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Meterage { get; set; }

        [Display(Name = "Số lượng / bao")]
        public int QuantityPerBag { get; set; }

        [StringLength(200)]
        [Display(Name = "Qui cách")]
        public string Specification { get; set; }
    }
}