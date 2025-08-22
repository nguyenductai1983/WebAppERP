using System.ComponentModel.DataAnnotations;

namespace WebAppERP.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string UserId { get; set; } = null!;

        public string Email { get; set; }= null!;

        [Required]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} và tối đa {1} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không trùng khớp.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}