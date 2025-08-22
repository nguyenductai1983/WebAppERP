using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebAppERP.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Họ và Tên")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Chức vụ là bắt buộc")]
        [StringLength(50)]
        [Display(Name = "Chức vụ")]
        public string Position { get; set; } = null!;

        [Display(Name = "Mức Lương")]
        [DisplayFormat(DataFormatString = "{0:N0} VND")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(250)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Số CCCD là bắt buộc")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "CCCD phải có đúng 12 số")]
        [Display(Name = "Căn cước công dân")]
        public string CitizenId { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày vào công ty")]
        public System.DateTime HireDate { get; set; }

        [Required(ErrorMessage = "Bộ phận là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Bộ phận")]
        public string Department { get; set; } = null!;

        // Foreign Key và Navigation Property
        public string  UserId { get; set; } = null!;
        public virtual IdentityUser User { get; set; } = null!;
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = null!;
    }
}