using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
using WebAppERP.Services;
using WebAppERP.ViewModels;

namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin, ProductionStaff")]
    public class ProductionLogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductionService _productionService;

        public ProductionLogController(ApplicationDbContext context, IProductionService productionService)
        {
            _context = context;
            _productionService = productionService;
        }

        // GET: ProductionLog/Log?routingId=5
        public async Task<IActionResult> Log(int routingId)
        {
            var routing = await _context.WorkOrderRoutings
                .Include(r => r.WorkOrder).ThenInclude(wo => wo.Product)
                .Include(r => r.ProductionStage)
                .FirstOrDefaultAsync(r => r.Id == routingId);

            if (routing == null) return NotFound("Không tìm thấy công đoạn sản xuất.");

            var viewModel = new ProductionLogViewModel
            {
                WorkOrderRoutingId = routingId,
                RoutingInfo = routing
            };
            viewModel.Outputs.Add(new ProductionOutputViewModel()); // Tự động thêm một dòng sản phẩm đầu ra

            // =====================================================================
            // ==> LOGIC MỚI: LẤY DANH SÁCH NVL TỪ PHIẾU XUẤT KHO
            // =====================================================================
            // Tìm tất cả các chi tiết đã được xuất kho cho Lệnh sản xuất này
            var issuedItems = await _context.MaterialIssueDetails
                .Include(d => d.MaterialIssue)
                .Include(d => d.WorkOrderBOM).ThenInclude(b => b.Component)
                 .Where(d => d.MaterialIssue.WorkOrderId == routing.WorkOrderId &&
                     d.MaterialIssue.Status == IssueStatus.Issued && // Chỉ lấy phiếu đã xuất
                     d.WorkOrderBOM.Component.DefaultProductionStageId == routing.ProductionStageId)
        .ToListAsync();

            // Tải trước danh sách các lô BTP có sẵn để tối ưu
            var availableYarnLots = await _context.Yarns.Where(y => y.StockQuantity > 0 && y.Status == StockItemStatus.InStock).ToListAsync();
            // ... tải các BTP khác nếu có

            foreach (var issuedItem in issuedItems)
            {
                var input = new MaterialInputViewModel
                {
                    WorkOrderBOMId = issuedItem.WorkOrderBOMId,
                    ComponentName = issuedItem.WorkOrderBOM.Component.Name,
                    UnitOfMeasure = issuedItem.WorkOrderBOM.Component.UnitOfMeasure,
                    // Hiển thị số lượng đã được kho xuất
                    QuantityToConsume = issuedItem.QuantityIssued,
                    IsLotTracked = issuedItem.WorkOrderBOM.Component.Type != ProductType.RawMaterial
                };

                // Nếu là BTP, tạo dropdown chỉ chứa lô đã được kho xuất
                if (input.IsLotTracked && issuedItem.LotId.HasValue)
                {
                    if (issuedItem.WorkOrderBOM.Component.LotEntityName == "Yarn")
                    {
                        var specificLot = availableYarnLots.FirstOrDefault(y => y.ID == issuedItem.LotId.Value);
                        if (specificLot != null)
                        {
                            input.AvailableLots = new SelectList(
                                new List<object> { new { specificLot.ID, Text = $"Lô Sợi #{specificLot.ID} (Đã xuất)" } },
                                "ID", "Text", specificLot.ID);
                            input.SelectedLotId = specificLot.ID; // Tự động chọn lô đã được xuất
                        }
                    }
                    // Thêm else if cho Textile...
                }
                viewModel.Inputs.Add(input);
            }

            await LoadGeneralViewData(viewModel);
            return View(viewModel);
        }

        // POST: ProductionLog/Log
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Log(ProductionLogViewModel model)
        {
            // Xóa các dòng input/output trống mà người dùng có thể đã thêm nhưng không điền
            model.Inputs.RemoveAll(i => i.QuantityToConsume == 0);
            model.Outputs.RemoveAll(o => o.Quantity == 0);

            if (ModelState.IsValid)
            {
                try
                {
                    await _productionService.LogProductionAsync(model);
                    TempData["SuccessMessage"] = "Đã ghi nhận sản lượng thành công.";
                    var routing = await _context.WorkOrderRoutings.FindAsync(model.WorkOrderRoutingId);
                    return RedirectToAction("Details", "WorkOrders", new { id = routing.WorkOrderId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Đã có lỗi xảy ra: {ex.Message}");
                }
            }

            // Nếu có lỗi, tải lại toàn bộ dữ liệu cần thiết cho View
            await ReloadFullViewModel(model);
            return View(model);
        }
        private async Task LoadGeneralViewData(ProductionLogViewModel model)
        {
            ViewData["OperatorId"] = new SelectList(await _context.Users.OrderBy(u => u.UserName).ToListAsync(), "Id", "UserName", model.OperatorId);
            ViewData["MachineId"] = new SelectList(await _context.Machines.OrderBy(m => m.Name).ToListAsync(), "ID", "Name", model.MachineId);
        }

        // Hàm helper để tải lại toàn bộ dữ liệu cho ViewModel khi có lỗi
        private async Task ReloadFullViewModel(ProductionLogViewModel model)
        {
            var routing = await _context.WorkOrderRoutings
                .Include(r => r.WorkOrder).ThenInclude(wo => wo.Product)
                .Include(r => r.ProductionStage)
                .FirstOrDefaultAsync(r => r.Id == model.WorkOrderRoutingId);
            model.RoutingInfo = routing;

            var bomRequirements = await _context.WorkOrderBOMs
                .Include(b => b.Component)
                .Where(b => b.WorkOrderId == routing.WorkOrderId &&
                             b.Component.DefaultProductionStageId == routing.ProductionStageId)
                .ToListAsync();

            var availableYarnLots = await _context.Yarns.Where(y => y.StockQuantity > 0).ToListAsync();

            // Gắn lại các thông tin hiển thị và danh sách dropdown cho các input
            foreach (var input in model.Inputs)
            {
                var req = bomRequirements.FirstOrDefault(r => r.Id == input.WorkOrderBOMId);
                input.ComponentName = req.Component.Name;
                // ... gán lại các thuộc tính hiển thị khác ...
                if (input.IsLotTracked)
                {
                    if (req.Component.Name.Contains("Sợi"))
                    {
                        input.AvailableLots = new SelectList(
                         availableYarnLots.Select(y => new { y.ID, Text = $"Lô Sợi #{y.ID} (Tồn: {y.StockQuantity})" }),
                         "ID", "Text", input.SelectedLotId);
                    }
                }
            }

            await LoadGeneralViewData(model);
        }
    }
}