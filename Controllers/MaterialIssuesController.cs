// File: Controllers/MaterialIssuesController.cs
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
using WebAppERP.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppERP.Services;

namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin, WarehouseStaff")]
    public class MaterialIssuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IInventoryService _inventoryService;

        public MaterialIssuesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IInventoryService inventoryService)
        {
            _context = context;
            _userManager = userManager;
            _inventoryService = inventoryService;
            ViewData["ActiveMenuGroup"] = "Warehouse";
        }

        // GET: MaterialIssues (Hiển thị các YÊU CẦU đang chờ xử lý)
        public async Task<IActionResult> Index()
        {
            var pendingRequisitions = await _context.MaterialRequisitions
                .Where(r => r.Status == RequisitionStatus.Pending)
                .Include(r => r.WorkOrder).ThenInclude(wo => wo.Product)
                .Include(r => r.Workshop)
                .OrderBy(r => r.RequestDate)
                .ToListAsync();

            return View(pendingRequisitions);
        }

        // GET: MaterialIssues/Details/5 (id ở đây là RequisitionId)
        // Action này được viết lại hoàn toàn để chuẩn bị cho việc xuất kho
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var requisition = await _context.MaterialRequisitions
                .Include(r => r.WorkOrder).ThenInclude(wo => wo.Product)
                .Include(r => r.Details).ThenInclude(rd => rd.WorkOrderBOM).ThenInclude(wb => wb.Component)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (requisition == null || requisition.Status != RequisitionStatus.Pending)
            {
                // Thông báo lỗi này là chính xác khi phiếu yêu cầu không tồn tại hoặc đã xử lý
                return NotFound("Không tìm thấy phiếu yêu cầu hoặc phiếu đã được xử lý.");
            }

            var viewModel = new MaterialIssueViewModel
            {
                MaterialRequisitionId = requisition.Id,
                Requisition = requisition // Gán thông tin phiếu yêu cầu để hiển thị
            };

            // Lấy danh sách các lô BTP có sẵn để tối ưu
            var availableYarnLots = await _context.Yarns.Where(y => y.StockQuantity > 0).ToListAsync();

            foreach (var reqDetail in requisition.Details)
            {
                var component = reqDetail.WorkOrderBOM.Component;
                var detailVM = new MaterialIssueDetailViewModel
                {
                    WorkOrderBOMId = reqDetail.WorkOrderBOMId,
                    ProductName = component.Name,
                    UnitOfMeasure = component.UnitOfMeasure,
                    QuantityRequired = reqDetail.QuantityRequested,
                    CurrentStock = component.Quantity, // Lấy tồn kho hiện tại của NVL
                    IsLotTracked = component.Type != ProductType.RawMaterial,
                    QuantityToIssue = reqDetail.QuantityRequested // Gợi ý số lượng xuất bằng số lượng yêu cầu
                };

                if (detailVM.IsLotTracked)
                {
                    if (component.LotEntityName == "Yarn")
                    {
                        // Đối với BTP, tồn kho là tổng tồn của các lô
                        detailVM.CurrentStock = availableYarnLots.Where(y => y.ProductId == component.Id).Sum(y => y.StockQuantity);
                        detailVM.AvailableLots = new SelectList(
                            availableYarnLots
                                .Where(y => y.ProductId == component.Id)
                                .Select(y => new { y.ID, Text = $"Lô Sợi #{y.ID} (Tồn: {y.StockQuantity})" }),
                            "ID", "Text");
                    }
                    // Thêm logic cho các loại BTP khác (Vải, Tráng...)
                }
                viewModel.Details.Add(detailVM);
            }

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Issue(MaterialIssueViewModel model)
        {
            var requisition = await _context.MaterialRequisitions
                .FirstOrDefaultAsync(r => r.Id == model.MaterialRequisitionId);

            if (requisition == null || requisition.Status != RequisitionStatus.Pending)
            {
                TempData["ErrorMessage"] = "Phiếu yêu cầu không hợp lệ hoặc đã được xử lý.";
                return RedirectToAction(nameof(Index));
            }

            var currentUser = await _userManager.GetUserAsync(User);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Tạo Phiếu Xuất kho (MaterialIssue)
                var materialIssue = new MaterialIssue
                {
                    WorkOrderId = requisition.WorkOrderId,
                    RequestDate = requisition.RequestDate,
                    IssuedDate = DateTime.Now,
                    IssuedById = currentUser.Id,
                    Status = IssueStatus.Issued
                };
                _context.MaterialIssues.Add(materialIssue);
                await _context.SaveChangesAsync();

                // 2. Lặp qua các chi tiết để tạo MaterialIssueDetail và trừ kho
                foreach (var detailVM in model.Details.Where(d => d.QuantityToIssue > 0))
                {
                    var bomItem = await _context.WorkOrderBOMs.FindAsync(detailVM.WorkOrderBOMId);
                    if (bomItem == null)
                    {
                        throw new InvalidOperationException($"Lỗi dữ liệu BOM ID {detailVM.WorkOrderBOMId}");
                    }

                    // =================================================================
                    // ==> SỬA LỖI: CẬP NHẬT SỐ LƯỢNG ĐÃ XUẤT DÙNG <==
                    bomItem.ConsumedQuantity += detailVM.QuantityToIssue;
                    // =================================================================

                    var issueDetail = new MaterialIssueDetail
                    {
                        MaterialIssueId = materialIssue.Id,
                        WorkOrderBOMId = detailVM.WorkOrderBOMId,
                        QuantityIssued = detailVM.QuantityToIssue,
                        LotId = detailVM.SelectedLotId
                    };
                    _context.MaterialIssueDetails.Add(issueDetail);

                    await _inventoryService.IssueForProductionAsync(
                        bomItem.ComponentId,
                        detailVM.QuantityToIssue,
                        detailVM.SelectedLotId,
                        currentUser.Id,
                        $"WO-{requisition.WorkOrderId}"
                    );
                }

                // 3. Cập nhật trạng thái của Phiếu Yêu cầu gốc
                requisition.Status = RequisitionStatus.Issued;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Đã xuất kho nguyên vật liệu thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Lỗi xuất kho: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = model.MaterialRequisitionId });
            }
        }
    }
}
