using System.ComponentModel.DataAnnotations;

namespace WebAppERP.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Tên nhà cung cấp")]
        public string Name { get; set; }

        [StringLength(150)]
        [Display(Name = "Người liên hệ")]
        public string ContactPerson { get; set; }

        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [StringLength(250)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
    }
}