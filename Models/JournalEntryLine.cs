using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    public enum DebitOrCredit
    {
        Debit,  // Nợ
        Credit  // Có
    }

    public class JournalEntryLine
    {
        public int Id { get; set; }

        public int JournalEntryId { get; set; }
        public virtual JournalEntry JournalEntry { get; set; }

        [Required]
        [Display(Name = "Tài khoản")]
        public int AccountId { get; set; }
        public virtual Account Account { get; set; }

        [Required]
        [Display(Name = "Nợ/Có")]
        public DebitOrCredit DebitOrCredit { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Số tiền")]
        public decimal Amount { get; set; }
    }
}