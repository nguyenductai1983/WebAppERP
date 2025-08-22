using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    public enum TransactionType
    {
        [Display(Name = "Nhập mua hàng")]
        PurchaseReceipt,

        [Display(Name = "Xuất cho Sản xuất")]
        IssueForProduction,

        [Display(Name = "Nhập từ Sản xuất")]
        ReceiptFromProduction,

        [Display(Name = "Xuất bán hàng")]
        SalesShipment,

        [Display(Name = "Điều chỉnh Tăng")]
        AdjustmentIncrease,

        [Display(Name = "Điều chỉnh Giảm")]
        AdjustmentDecrease,

        [Display(Name = "Điều chuyển Kho")]
        Transfer
    }

    [Table("InventoryTransactions")]
    public class InventoryTransaction
    {
        [Key]
        public long Id { get; set; } // Dùng long để lưu được nhiều giao dịch

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [Display(Name = "Loại giao dịch")]
        public TransactionType Type { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "Số lượng thay đổi")]
        public decimal QuantityChange { get; set; } // Số dương là nhập, số âm là xuất

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "Tồn kho sau giao dịch")]
        public decimal QuantityAfterTransaction { get; set; }

        [Display(Name = "Mã Lô (BTP/TP)")]
        public int? LotId { get; set; } // ID của Yarn, Textile...

        [Required]
        [Display(Name = "Ngày giao dịch")]
        public DateTime TransactionDate { get; set; }

        [Display(Name = "Người thực hiện")]
        public string UserId { get; set; }
        public virtual IdentityUser User { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú/Tham chiếu")]
        public string Reference { get; set; } // Ví dụ: "PO-001", "WO-123", "SO-205"
    }
}