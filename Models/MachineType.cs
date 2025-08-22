using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    [Table("MachineType")]
    public class MachineType
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên Loại Máy")]
        public string Name { get; set; } = null!;
        // ==========================================================
        // ==> THÊM CÁC THUỘC TÍNH MỚI NÀY
        // ==========================================================
        [Required(ErrorMessage = "Vui lòng chọn công đoạn sản xuất.")]
        [Display(Name = "Sử dụng cho Công đoạn")]
        public int ProductionStageId { get; set; }
        public virtual ProductionStage ProductionStage { get; set; }
        // ==========================================================
    }
}
