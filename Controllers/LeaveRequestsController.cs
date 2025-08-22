using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebAppERP.Data;
using WebAppERP.Models;
using Microsoft.AspNetCore.Identity; // Thêm using
namespace WebAppERP.Controllers
{
    [Authorize] // Yêu cầu đăng nhập
    public class LeaveRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LeaveRequestsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: LeaveRequests (Hiển thị các đơn của tôi)
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == currentUser.Id);

            if (employee == null) return View("NoProfileFound");

            var leaveRequests = await _context.LeaveRequests
                .Where(lr => lr.RequestingEmployeeId == employee.Id)
                .OrderByDescending(lr => lr.RequestDate)
                .ToListAsync();

            return View(leaveRequests);
        }

        // GET: LeaveRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LeaveRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartDate,EndDate,Reason")] LeaveRequest leaveRequest)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == currentUser.Id);

            if (employee == null)
            {
                // Xử lý lỗi nếu user không có hồ sơ nhân viên
                ModelState.AddModelError(string.Empty, "Bạn phải có hồ sơ nhân viên để tạo đơn.");
                return View(leaveRequest);
            }

            if (ModelState.IsValid)
            {
                leaveRequest.RequestingEmployeeId = employee.Id;
                leaveRequest.RequestDate = DateTime.Now;
                leaveRequest.Status = LeaveRequestStatus.Pending; // Mặc định là chờ duyệt
                _context.Add(leaveRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leaveRequest);
        }
    }
}
