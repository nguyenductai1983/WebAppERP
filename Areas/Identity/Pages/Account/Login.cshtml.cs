using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebAppERP.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        // Thêm UserManager để có thể tìm kiếm người dùng
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager,
                          ILogger<LoginModel> logger,
                          UserManager<IdentityUser> userManager) // Thêm UserManager vào constructor
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = null!;

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = null!;

        public string ReturnUrl { get; set; } = null!;

        [TempData]
        public string ErrorMessage { get; set; } = null!;

        public class InputModel
        {
            [Required]
            [Display(Name = "Email hoặc Username")] // Sửa lại nhãn hiển thị
            public string Email { get; set; } = null!;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; } = null!;

            [Display(Name = "Ghi nhớ đăng nhập")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        // --- THAY THẾ TOÀN BỘ PHƯƠNG THỨC OnPostAsync BẰNG PHIÊN BẢN NÀY ---
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Logic để tìm user bằng email hoặc username
                var userName = Input.Email;
                // Nếu người dùng nhập chuỗi có chứa "@", ta giả định đó là email
                if (userName.Contains("@"))
                {
                    var user = await _userManager.FindByEmailAsync(userName);
                    if (user != null)
                    {
                        // Nếu tìm thấy, lấy username thật sự của họ để đăng nhập
                        userName = user.UserName;
                    }
                }

                // Luôn thực hiện đăng nhập bằng username
                var result = await _signInManager.PasswordSignInAsync(userName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
                    return Page();
                }
            }

            // Nếu có lỗi, quay lại trang đăng nhập
            return Page();
        }
    }
}
