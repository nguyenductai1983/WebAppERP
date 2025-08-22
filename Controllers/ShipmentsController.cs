// File: Controllers/ShipmentsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Authorize(Roles = "Admin, WarehouseStaff")] // Giả sử có role WarehouseStaff
    public class ShipmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IInventoryService _inventoryService;

        public ShipmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IInventoryService inventoryService)
        {
            _context = context;
            _userManager = userManager;
            _inventoryService = inventoryService;
            ViewData["ActiveMenuGroup"] = "Warehouse";
        }

        // GET: Shipments (Hiển thị các phiếu chờ xuất kho)
        public async Task<IActionResult> Index()
        {
            var pendingShipments = await _context.Shipments
                .Include(s => s.SalesOrder).ThenInclude(so => so.Customer)
                .Where(s => s.Status == ShipmentStatus.Pending)
                .OrderBy(s => s.CreationDate)
                .ToListAsync();
            return View(pendingShipments);
        }

        // GET: Shipments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var shipment = await _context.Shipments
                .Include(s => s.SalesOrder).ThenInclude(so => so.Customer)
                .Include(s => s.ShipmentDetails).ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shipment == null) return NotFound();

            return View(shipment);
        }

        // POST: Shipments/ConfirmShipment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmShipment(int id)
        {
            var shipment = await _context.Shipments
                .Include(s => s.SalesOrder)
                .Include(s => s.ShipmentDetails)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shipment == null || shipment.Status != ShipmentStatus.Pending)
            {
                TempData["ErrorMessage"] = "Phiếu xuất kho không hợp lệ hoặc đã được xử lý.";
                return RedirectToAction(nameof(Index));
            }

            var currentUser = await _userManager.GetUserAsync(User);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Gọi InventoryService để trừ kho cho từng mặt hàng
                foreach (var detail in shipment.ShipmentDetails)
                {
                    // Chuyển đổi từ SalesOrderDetail sang một đối tượng tạm để truyền đi
                    var tempSalesDetail = new SalesOrderDetail { ProductId = detail.ProductId, Quantity = (int)detail.QuantityToShip };
                    await _inventoryService.ShipForSalesOrderAsync(tempSalesDetail, currentUser.Id, $"SO-{shipment.SalesOrderId}");
                }

                // Cập nhật trạng thái
                shipment.Status = ShipmentStatus.Shipped;
                shipment.ShippedDate = DateTime.Now;
                shipment.ShippedById = currentUser.Id;
                shipment.SalesOrder.Status = OrderStatus.Shipped; // Cập nhật trạng thái đơn hàng gốc

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Đã xác nhận xuất kho thành công!";
                return RedirectToAction(nameof(Details), new { id = shipment.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Lỗi xuất kho: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = shipment.Id });
            }
        }
    }
}