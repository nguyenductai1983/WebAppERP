using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    [Table("TextileType")]
    public class TextileType
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên Loại Vải")]
        public string Name { get; set; } = null!;

        [Display(Name = "Ghi Chú")]
        public string Note { get; set; } = null!;
    }
}
