using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    [Table("MaterialIssueDetails")]
    public class MaterialIssueDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MaterialIssueId { get; set; }
        public virtual MaterialIssue MaterialIssue { get; set; }

        [Required]
        [Display(Name = "Yêu cầu NVL từ BOM")]
        public int WorkOrderBOMId { get; set; }
        public virtual WorkOrderBOM WorkOrderBOM { get; set; }

        [Display(Name = "Lô BTP xuất kho")]
        public int? LotId { get; set; } // ID của lô Yarn, Textile... được xuất

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "Số lượng thực xuất")]
        public decimal QuantityIssued { get; set; }
    }
}