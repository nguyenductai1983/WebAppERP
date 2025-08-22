using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    [Table("CoatedTextile")]
    public class CoatedTextile
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [ForeignKey("Machine")]
        [Display(Name = "Máy tráng")]
        public int MachineId { get; set; }
        public virtual Machine Machine { get; set; } = null!;

        [Required]
        [ForeignKey("Operator")]
        [Display(Name = "Nhân viên Vận hành")]
        public string OperatorId { get; set; } = null!;
        public virtual IdentityUser Operator { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Display(Name = "Mã cây vải tráng")]
        public string Code { get; set; } = null!; 

        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Số mét ban đầu")]
        public decimal InitialLength { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Trọng lượng GW")]
        public decimal GrossWeight { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Trọng lượng NW")]
        public decimal NetWeight { get; set; }

        [StringLength(50)]
        [Display(Name = "Chất lượng")]
        public string Quality { get; set; } = null!;

        [Display(Name = "Ngày tạo")]
        public DateTime Created_at { get; set; } = DateTime.Today;
        [Display(Name = "Ngày cập nhật")]
        public DateTime Updated_at { get; set; } = DateTime.Today;
    }
}
