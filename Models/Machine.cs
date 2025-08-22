using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    [Table("Machine")]
    public class Machine
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên / Số hiệu Máy")]
        public string Name { get; set; } = null!;
        // << THÊM CÁC THUỘC TÍNH MỚI NÀY >>
        [Required(ErrorMessage = "Vui lòng chọn phân xưởng.")]
        [Display(Name = "Phân xưởng")]
        public int WorkshopId { get; set; }
        public virtual Workshop Workshop { get; set; }
        // ------------------------------------
        // Thay thế enum bằng Foreign Key và Navigation Property
        [Required]
        [ForeignKey("MachineType")]
        [Display(Name = "Loại Máy")]
        public int MachineTypeId { get; set; }
        public virtual MachineType MachineType { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = null!;
    }
}
