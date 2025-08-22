using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    public enum AccountType
    {
        Asset,      // Tài sản
        Liability,  // Nợ phải trả
        Equity,     // Vốn chủ sở hữu
        Revenue,    // Doanh thu
        Expense     // Chi phí
    }

    public class Account
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Số hiệu Tài khoản")]
        public string AccountNumber { get; set; } = null!;// Ví dụ: 111, 156, 511

        [Required]
        [StringLength(200)]
        [Display(Name = "Tên Tài khoản")]
        public string Name { get; set; } = null!;// Ví dụ: Tiền mặt, Hàng hóa, Doanh thu bán hàng

        [Required]
        [Display(Name = "Loại Tài khoản")]
        public AccountType Type { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; } = null!;

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Số dư")]
        public decimal Balance { get; set; }
    }
}