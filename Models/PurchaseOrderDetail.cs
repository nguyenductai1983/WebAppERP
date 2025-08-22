using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebAppERP.Models
{
    public class PurchaseOrderDetail
    {
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Đơn giá mua")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }
        [StringLength(100)]
        [Display(Name = "Số lô NCC")]
        public string SupplierLotNumber { get; set; }
    }
}