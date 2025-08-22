using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    [Table("TextileYarnUsage")]
    public class TextileYarnUsage
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [ForeignKey("Textile")]
        [Display(Name = "Cây Vải thành phẩm")]
        public int TextileId { get; set; }
        public virtual Textile Textile { get; set; } = null!;

        [Required]
        [ForeignKey("Yarn")]
        [Display(Name = "Lô Sợi đã dùng")]
        public int YarnId { get; set; }
        public virtual Yarn Yarn { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Số lượng Sợi đã dùng (kg)")]
        public decimal QuantityUsed { get; set; }

        [Display(Name = "Ngày sử dụng")]
        public DateTime UsageDate { get; set; }
    }
}