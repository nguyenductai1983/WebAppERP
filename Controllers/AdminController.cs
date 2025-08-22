using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebAppERP.Models; // Thêm using cho ViewModel
using System.Linq; // Thêm using cho LINQ
namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin")] // Chỉ Admin mới được vào controller này
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Hiển thị danh sách người dùng
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
        // GET: /Admin/ManageRoles?userId=...
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var allRoles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = allRoles.Select(role => new RoleViewModel
                {
                    RoleName = role.Name,
                    IsSelected = userRoles.Contains(role.Name)
                }).ToList()
            };

            return View(model);
        }

        // POST: /Admin/ManageRoles
        [HttpPost]
        public async Task<IActionResult> ManageRoles(ManageUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName);

            // Thêm các role mới được chọn
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded) return View(model); // Lỗi, trả về view cũ

            // Xóa các role đã bị bỏ chọn
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded) return View(model); // Lỗi, trả về view cũ

            return RedirectToAction("Index");
        }
        // GET: Admin/ResetPassword/some-user-id
        public async Task<IActionResult> ResetPassword(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ResetPasswordViewModel
            {
                UserId = user.Id,
                Email = user.Email
            };

            return View(model);
        }

        // POST: Admin/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                // Should not happen if the form is used correctly
                return NotFound();
            }

            // Xóa token cũ và tạo token mới để reset mật khẩu
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

            if (result.Succeeded)
            {
                // Thêm một thông báo thành công để hiển thị trên trang danh sách
                TempData["SuccessMessage"] = $"Đã đặt lại mật khẩu cho người dùng {user.Email} thành công.";
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, thêm lỗi vào ModelState và hiển thị lại form
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

    }
}