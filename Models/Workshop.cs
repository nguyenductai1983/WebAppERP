using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    [Table("Workshops")]
    public class Workshop
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Tên Phân xưởng")]
        public string Name { get; set; }

        [StringLength(250)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
    }
}