using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    public enum PaymentMethod
    {
        [Display(Name = "Tiền mặt")]
        Cash,
        [Display(Name = "Chuyển khoản")]
        BankTransfer,
        [Display(Name = "Thẻ")]
        Card
    }

    [Table("Payment")]
    public class Payment
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Ngày thanh toán")]
        public DateTime PaymentDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Số tiền")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Phương thức")]
        public PaymentMethod Method { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Note { get; set; } = null!;

        // --- Liên kết đến các đơn hàng ---
        // Một thanh toán có thể thuộc về một đơn bán hàng...
        [ForeignKey("SalesOrder")]
        public int? SalesOrderId { get; set; }
        public virtual SalesOrder SalesOrder { get; set; } = null!;

        // ...hoặc một đơn mua hàng
        [ForeignKey("PurchaseOrder")]
        public int? PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
    }
}
