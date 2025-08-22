using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
using WebAppERP.Services;
using WebAppERP.ViewModels;

namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin, ProductionStaff, PurchaseStaff")]
    public class MRPController : Controller
    {
        private readonly IMRPService _mrpService;
        private readonly ApplicationDbContext _context;

        public MRPController(IMRPService mrpService, ApplicationDbContext context)
        {
            _mrpService = mrpService;
            _context = context;
            ViewData["ActiveMenuGroup"] = "Production";
        }

        // GET: Hiển thị báo cáo MRP
        public async Task<IActionResult> Index()
        {
            var viewModel = await _mrpService.CalculateRequirementsAsync();
            return View(viewModel);
        }

        // POST: Tạo đơn mua hàng từ các đề xuất đã chọn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePurchaseOrderFromMRP(List<MaterialRequirementViewModel> materials)
        {
            var selectedMaterials = materials.Where(m => m.IsSelected && m.SuggestedPurchaseQuantity > 0).ToList();

            if (!selectedMaterials.Any())
            {
                TempData["WarningMessage"] = "Vui lòng chọn ít nhất một nguyên vật liệu để tạo đơn hàng.";
                return RedirectToAction(nameof(Index));
            }

            // Giả định tạo một đơn hàng nháp không có nhà cung cấp
            // Trong thực tế, có thể nhóm theo nhà cung cấp gợi ý
            var newPurchaseOrder = new PurchaseOrder
            {
                OrderDate = DateTime.Now,
                Status = PurchaseOrderStatus.Draft,
                SupplierId = 1 // Tạm thời gán NCC mặc định, cần thay đổi
            };

            foreach (var material in selectedMaterials)
            {
                var product = await _context.Products.FindAsync(material.ProductId);
                newPurchaseOrder.OrderDetails.Add(new PurchaseOrderDetail
                {
                    ProductId = material.ProductId,
                    Quantity = (int)Math.Ceiling(material.SuggestedPurchaseQuantity),
                    UnitPrice = product.Cost // Lấy giá mua gần nhất từ sản phẩm
                });
            }

            _context.PurchaseOrders.Add(newPurchaseOrder);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã tạo thành công Đơn mua hàng nháp #{newPurchaseOrder.Id}. Bộ phận Mua hàng sẽ xử lý.";
            return RedirectToAction("Details", "PurchaseOrders", new { id = newPurchaseOrder.Id });
        }
    }
}
