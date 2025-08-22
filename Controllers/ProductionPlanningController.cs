using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
using WebAppERP.Services;
namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductionPlanningController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BomCalculationService _bomService; 
        public ProductionPlanningController(ApplicationDbContext context, BomCalculationService bomService)
        {
            _context = context;
            _bomService = bomService; // Khởi tạo
            ViewData["ActiveMenuGroup"] = "Production";
        }

        // GET: ProductionPlanning
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả các dòng chi tiết đơn hàng chưa được xử lý
            // Đây chính là "Nhu cầu" (Demand) của bạn
            var openSalesOrderDetails = await _context.SalesOrderDetails
                .Include(d => d.SalesOrder)
                .Include(d => d.Product)
                .Where(d => d.SalesOrder.Status == OrderStatus.Pending || d.SalesOrder.Status == OrderStatus.Processing)
                .ToListAsync();

            // Lấy các lệnh sản xuất đang hoạt động để biết "Lịch trình" hiện tại
            ViewBag.ActiveWorkOrders = await _context.WorkOrders
                .Include(w => w.Product)
                .Where(w => w.Status != WorkOrderStatus.Completed && w.Status != WorkOrderStatus.Cancelled)
                .ToListAsync();

            return View(openSalesOrderDetails);
        }

        // POST: ProductionPlanning/CreateWorkOrderFromDemand
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWorkOrderFromDemand(int productId, int quantity, int salesOrderId)
        {
            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Số lượng sản xuất phải lớn hơn 0.";
                return RedirectToAction(nameof(Index));
            }

            // 1. Tạo Lệnh sản xuất mới
            var workOrder = new WorkOrder
            {
                ProductId = productId,
                QuantityToProduce = quantity,
                CreationDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7), // Giả định sản xuất trong 7 ngày
                Status = WorkOrderStatus.New
            };
            _context.WorkOrders.Add(workOrder);
            await _context.SaveChangesAsync(); // Lưu để lấy Id

            // 2. Tạo Kế hoạch sản xuất để liên kết
            var plan = new ProductionPlan
            {
                PlanDate = DateTime.Now,
                SalesOrderId = salesOrderId,
                WorkOrderId = workOrder.Id
            };
            _context.ProductionPlans.Add(plan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã tạo thành công Lệnh sản xuất #{workOrder.Id} từ Đơn hàng #{salesOrderId}.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWorkOrderFromDemand(int salesOrderDetailId)
        {
            var orderDetail = await _context.SalesOrderDetails
                                            .Include(d => d.Product)
                                            .FirstOrDefaultAsync(d => d.Id == salesOrderDetailId);

            if (orderDetail == null) return NotFound();

            // 1. Tạo Lệnh sản xuất mới
            var workOrder = new WorkOrder
            {
                ProductId = orderDetail.ProductId,
                QuantityToProduce = orderDetail.Quantity,
                CreationDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                Status = WorkOrderStatus.New,
                // (Tùy chọn) Sao chép các thuộc tính động sang WorkOrder
            };
            _context.WorkOrders.Add(workOrder);
            await _context.SaveChangesAsync(); // Lưu để workOrder có Id

            // 2. GỌI SERVICE để tính toán BOM động
            var calculatedBoms = _bomService.CalculateAndGenerateBom(orderDetail, workOrder.Id);

            // 3. Lưu kết quả BOM đã tính toán vào bảng WorkOrderBOMs
            if (calculatedBoms.Any())
            {
                _context.WorkOrderBOMs.AddRange(calculatedBoms);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"Đã tạo Lệnh sản xuất #{workOrder.Id} và tính toán BOM thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}