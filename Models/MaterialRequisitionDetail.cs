using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    [Table("MaterialRequisitionDetails")]
    public class MaterialRequisitionDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MaterialRequisitionId { get; set; }
        public virtual MaterialRequisition MaterialRequisition { get; set; }

        /// <summary>
        /// Liên kết trực tiếp đến dòng yêu cầu NVL trong BOM của Lệnh sản xuất.
        /// Đây là mối liên kết quan trọng nhất.
        /// </summary>
        [Required]
        [Display(Name = "Yêu cầu từ BOM")]
        public int WorkOrderBOMId { get; set; }
        public virtual WorkOrderBOM WorkOrderBOM { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "Số lượng yêu cầu")]
        public decimal QuantityRequested { get; set; }
    }
}