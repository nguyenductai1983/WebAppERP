using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebAppERP.Data; // Thay bằng namespace Data của bạn
using WebAppERP.Models; // Thay bằng namespace Models của bạn

namespace WebAppERP.Controllers
{
    [Authorize] // Yêu cầu phải đăng nhập để truy cập
    public class ProfileController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: /Profile
        public async Task<IActionResult> Index()
        {
            // 1. Lấy thông tin người dùng đang đăng nhập
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            // 2. Dùng UserId để tìm hồ sơ nhân viên tương ứng
            var employeeProfile = await _context.Employees
                                                .FirstOrDefaultAsync(e => e.UserId == currentUser.Id);

            // 3. Xử lý trường hợp người dùng không có hồ sơ nhân viên
            if (employeeProfile == null)
            {
                // Có thể chuyển hướng đến một trang thông báo
                // hoặc hiển thị một view đặc biệt
                return View("NoProfileFound");
            }

            // 4. Trả về View với dữ liệu hồ sơ nhân viên
            return View(employeeProfile);
        }
    }
}