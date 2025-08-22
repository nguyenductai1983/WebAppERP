using System.ComponentModel.DataAnnotations;

namespace WebAppERP.Models
{
    public class Color
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên màu")]
        public string Name { get; set; }

        [Display(Name = "Mã màu (Hex)")]
        public string Code { get; set; } // Ví dụ: #FF0000
    }
}