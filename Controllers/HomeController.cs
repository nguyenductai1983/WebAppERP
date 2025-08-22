using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Models;
using WebAppERP.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace WebAppERP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel();

            if (User.IsInRole("Admin"))
            {
                // ... (phần code lấy dữ liệu cho Admin không đổi)
                viewModel.TotalUsers = await _context.Users.CountAsync();
                viewModel.TotalProducts = await _context.Products.CountAsync();
                viewModel.PendingLeaveRequests = await _context.LeaveRequests.CountAsync(lr => lr.Status == LeaveRequestStatus.Pending);
                viewModel.TotalSalesOrders = await _context.SalesOrders.CountAsync();
                viewModel.TotalRevenue = await _context.SalesOrders
                                                .Where(so => so.Status == OrderStatus.Completed)
                                                .SumAsync(so => so.TotalAmount);
            }

            // Lấy dữ liệu cho nhân viên
            var currentUser = await _userManager.GetUserAsync(User);

            // << THÊM KIỂM TRA NULL TẠI ĐÂY >>
            if (currentUser == null)
            {
                // Nếu không tìm thấy user, có thể session có vấn đề, yêu cầu đăng nhập lại
                return Challenge();
            }

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == currentUser.Id);
            if (employee != null)
            {
                viewModel.MyPendingLeaveRequests = await _context.LeaveRequests
                    .CountAsync(lr => lr.RequestingEmployeeId == employee.Id && lr.Status == LeaveRequestStatus.Pending);
            }

            return View(viewModel);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // Thêm action này vào HomeController.cs
        [HttpGet]
        public async Task<JsonResult> GetSalesDataForChart()
        {
            var salesData = await _context.SalesOrders
                .Where(so => so.Status == OrderStatus.Completed) // Chỉ lấy đơn đã hoàn thành
                .GroupBy(so => new { Year = so.OrderDate.Year, Month = so.OrderDate.Month })
                .Select(g => new
                {
                    Month = g.Key.Month + "/" + g.Key.Year,
                    Total = g.Sum(so => so.TotalAmount)
                })
                .OrderBy(d => d.Month)
                .ToListAsync();

            return Json(salesData);
        }
        // GET: /Home/GetTopSellingProducts
        [HttpGet]
        public async Task<JsonResult> GetTopSellingProducts(int top = 5)
        {
            var topProducts = await _context.SalesOrderDetails
                .GroupBy(od => od.Product.Name)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(top)
                .ToListAsync();

            return Json(topProducts);
        }

        // GET: /Home/GetInventoryStatus
        [HttpGet]
        public async Task<JsonResult> GetInventoryStatus()
        {
            var inventoryStatus = new
            {
                FinishedGoods = await _context.Products.CountAsync(p => p.Type == ProductType.FinishedGood),
                RawMaterials = await _context.Products.CountAsync(p => p.Type == ProductType.RawMaterial),
            };
            return Json(inventoryStatus);
        }
    }
}
