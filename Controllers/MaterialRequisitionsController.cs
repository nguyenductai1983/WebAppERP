using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
using WebAppERP.ViewModels;

namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin, ProductionStaff")]
    public class MaterialRequisitionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MaterialRequisitionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ACTION MỚI: Trang quản lý trung tâm cho việc lĩnh vật tư
        public async Task<IActionResult> Manage(int workOrderId)
        {
            var workOrder = await _context.WorkOrders
                .Include(wo => wo.Product)
                .FirstOrDefaultAsync(wo => wo.Id == workOrderId);

            if (workOrder == null) return NotFound();

            ViewBag.PastRequisitions = await _context.MaterialRequisitions
                .Where(r => r.WorkOrderId == workOrderId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(workOrder);
        }

        // GET: Hiển thị form tạo phiếu lĩnh
        public async Task<IActionResult> Create(int workOrderId)
        {
            var workOrder = await _context.WorkOrders
                .Include(wo => wo.Product)
                .FirstOrDefaultAsync(wo => wo.Id == workOrderId);
            if (workOrder == null) return NotFound();

            var viewModel = new MaterialRequisitionViewModel
            {
                WorkOrderId = workOrderId,
                WorkOrder = workOrder
            };

            var bomRequirements = await _context.WorkOrderBOMs
                .Include(b => b.Component)
                .Where(b => b.WorkOrderId == workOrderId)
                .ToListAsync();

            var alreadyRequestedDetails = await _context.MaterialRequisitionDetails
                .Where(d => d.MaterialRequisition.WorkOrderId == workOrderId)
                .GroupBy(d => d.WorkOrderBOMId)
                .Select(g => new { WorkOrderBOMId = g.Key, TotalRequested = g.Sum(d => d.QuantityRequested) })
                .ToListAsync();

            foreach (var req in bomRequirements)
            {
                var requestedInfo = alreadyRequestedDetails.FirstOrDefault(d => d.WorkOrderBOMId == req.Id);
                decimal alreadyRequestedQty = requestedInfo?.TotalRequested ?? 0;
                decimal neededQty = req.RequiredQuantity - alreadyRequestedQty;

                if (neededQty > 0)
                {
                    viewModel.Details.Add(new MaterialRequisitionDetailViewModel
                    {
                        WorkOrderBOMId = req.Id,
                        ProductName = req.Component.Name,
                        UnitOfMeasure = req.Component.UnitOfMeasure,
                        QuantityRequired = req.RequiredQuantity,
                        QuantityAlreadyIssued = alreadyRequestedQty,
                        // =================================================================
                        // ==> THÊM DÒNG NÀY ĐỂ TỰ ĐỘNG ĐIỀN SỐ LƯỢNG <==
                        QuantityToRequest = neededQty
                        // =================================================================
                    });
                }
            }

            return View(viewModel);
        }


        // POST: Lưu phiếu lĩnh NVL mới (Giữ nguyên logic)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaterialRequisitionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var requisition = new MaterialRequisition
                {
                    WorkOrderId = model.WorkOrderId,
                    WorkshopId = 1, // Cần logic để lấy workshop thực tế
                    RequestDate = System.DateTime.Now,
                    RequestedById = currentUser.Id,
                    Status = RequisitionStatus.Pending
                };

                foreach (var detail in model.Details.Where(d => d.QuantityToRequest > 0))
                {
                    requisition.Details.Add(new MaterialRequisitionDetail
                    {
                        WorkOrderBOMId = detail.WorkOrderBOMId,
                        QuantityRequested = detail.QuantityToRequest
                    });
                }

                if (requisition.Details.Any())
                {
                    _context.MaterialRequisitions.Add(requisition);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Đã gửi yêu cầu lĩnh vật tư thành công.";
                    // Quay lại trang quản lý thay vì trang chi tiết LSX
                    return RedirectToAction(nameof(Manage), new { workOrderId = model.WorkOrderId });
                }
                else
                {
                    ModelState.AddModelError("", "Cần nhập số lượng yêu cầu cho ít nhất một vật tư.");
                }
            }

            model.WorkOrder = await _context.WorkOrders.Include(wo => wo.Product).FirstOrDefaultAsync(wo => wo.Id == model.WorkOrderId);
            return View(model);
        }
        // GET: MaterialRequisitions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialRequisition = await _context.MaterialRequisitions
                .Include(m => m.WorkOrder)
                    .ThenInclude(wo => wo.Product)
                .Include(m => m.RequestedBy)
                .Include(m => m.Details)
                    .ThenInclude(d => d.WorkOrderBOM)
                    .ThenInclude(wb => wb.Component)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (materialRequisition == null)
            {
                return NotFound();
            }

            return View(materialRequisition);
        }
        // GET: Hiển thị danh sách các phiếu đã được kho xuất, chờ sản xuất xác nhận
        public async Task<IActionResult> AwaitingConfirmation(int workOrderId)
        {
            var issuesToConfirm = await _context.MaterialIssues
                .Where(i => i.WorkOrderId == workOrderId && i.Status == IssueStatus.Issued)
                .Include(i => i.IssuedBy)
                .OrderByDescending(i => i.IssuedDate)
                .ToListAsync();

            ViewBag.WorkOrderId = workOrderId;
            return View(issuesToConfirm);
        }

        // POST: Xử lý việc xác nhận đã nhận hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmReceipt(int issueId, int workOrderId)
        {
            var issue = await _context.MaterialIssues.FindAsync(issueId);
            if (issue == null || issue.Status != IssueStatus.Issued)
            {
                TempData["ErrorMessage"] = "Phiếu xuất kho không hợp lệ hoặc đã được xác nhận.";
                return RedirectToAction(nameof(AwaitingConfirmation), new { workOrderId });
            }

            var currentUser = await _userManager.GetUserAsync(User);

            issue.Status = IssueStatus.ReceiptConfirmed;
            issue.ConfirmationDate = DateTime.Now;
            issue.ConfirmedById = currentUser.Id;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã xác nhận nhận vật tư thành công!";
            return RedirectToAction(nameof(AwaitingConfirmation), new { workOrderId });
        }
    }
}