using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
using WebAppERP.Services;

namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin, ProductionStaff")]
    public class WorkOrdersController : Controller
    {
        private readonly IWorkOrderService _workOrderService;
        private readonly ApplicationDbContext _context;

        public WorkOrdersController(IWorkOrderService workOrderService, ApplicationDbContext context)
        {
            _workOrderService = workOrderService;
            _context = context;
            ViewData["ActiveMenuGroup"] = "Production";
        }

        // ... (Các action Index, Details, Create, Edit, etc. giữ nguyên) ...
        #region Other Actions
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber, int? productionStageId)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null) { pageNumber = 1; }
            else { searchString = currentFilter; }
            var workOrders = from s in _context.WorkOrders
                     .Include(w => w.Product)
                     .Include(w => w.ProductionStage)
                     .Include(w => w.WorkOrderRoutings)
                     .ThenInclude(r => r.ProductionStage)
                             select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                workOrders = workOrders.Where(s => s.Product.Name.Contains(searchString) || s.Description.Contains(searchString));
            }

            if (productionStageId.HasValue)
            {
                workOrders = workOrders.Where(wo => wo.Type != WorkOrderType.Master && wo.ProductionStageId == productionStageId.Value && wo.Status != WorkOrderStatus.Completed);
                var stage = await _context.ProductionStages.FindAsync(productionStageId.Value);
                ViewData["StageFilter"] = stage?.Name;
            }

            workOrders = sortOrder switch
            {
                "date_desc" => workOrders.OrderByDescending(s => s.CreationDate),
                _ => workOrders.OrderBy(s => s.CreationDate),
            };

            int pageSize = 10;
            return View(await PaginatedList<WorkOrder>.CreateAsync(workOrders.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var workOrder = await _context.WorkOrders
                .Include(w => w.Product)
                .Include(w => w.ProductionStage)
                .Include(w => w.WorkOrderRoutings)
                .ThenInclude(r => r.ProductionStage)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workOrder == null) return NotFound();

            ViewBag.ChildWorkOrders = await _context.WorkOrders
                .Where(wo => wo.ParentWorkOrderId == id)
                .Include(wo => wo.Product)
                .Include(wo => wo.ProductionStage)
                .OrderBy(wo => wo.ProductionStage != null ? wo.ProductionStage.Sequence : 0)
                .ThenBy(wo => wo.Id)
                .ToListAsync();

            return View(workOrder);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["ProductId"] = new SelectList(
                await _context.Products.Where(p => p.Type == ProductType.FinishedGood).OrderBy(p => p.Name).ToListAsync(),
                "Id", "Name"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,QuantityToProduce,Description,DueDate")] WorkOrder workOrder)
        {
            ModelState.Remove("Product");

            if (ModelState.IsValid)
            {
                try
                {
                    var createdWorkOrder = await _workOrderService.CreateMasterWorkOrderAsync(workOrder);
                    TempData["SuccessMessage"] = $"Đã tạo LSX Tổng #{createdWorkOrder.Id} và các công đoạn liên quan thành công.";
                    return RedirectToAction(nameof(Details), new { id = createdWorkOrder.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Đã có lỗi xảy ra: {ex.Message}");
                }
            }

            ViewData["ProductId"] = new SelectList(_context.Products.Where(p => p.Type == ProductType.FinishedGood), "Id", "Name", workOrder.ProductId);
            return View(workOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReleaseWorkOrder(int id)
        {
            try
            {
                await _workOrderService.ReleaseWorkOrderAsync(id);
                TempData["SuccessMessage"] = "Đã ban hành thành công. Toàn bộ chuỗi Lệnh sản xuất đã được tự động hoạch định và khởi động.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message} {ex.InnerException}";
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartProduction(int id)
        {
            var workOrder = await _context.WorkOrders.FindAsync(id);
            if (workOrder == null) return NotFound();

            try
            {
                await _workOrderService.StartProductionAsync(id);
                TempData["SuccessMessage"] = $"Đã bắt đầu sản xuất LSX #{id} thành công!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra khi bắt đầu sản xuất.";
            }

            var redirectId = workOrder.ParentWorkOrderId ?? workOrder.Id;
            return RedirectToAction(nameof(Details), new { id = redirectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProduction(int id)
        {
            var workOrder = await _context.WorkOrders.FindAsync(id);
            if (workOrder == null) return NotFound();

            try
            {
                await _workOrderService.CompleteProductionAsync(id);
                TempData["SuccessMessage"] = "Hoàn thành sản xuất! Tồn kho và chi phí đã được cập nhật.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra khi hoàn thành sản xuất.";
            }

            return RedirectToAction(nameof(Details), new { id = workOrder.ParentWorkOrderId ?? id });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var workOrder = await _context.WorkOrders.Include(wo => wo.Product).FirstOrDefaultAsync(wo => wo.Id == id);
            if (workOrder == null) return NotFound();
            if (workOrder.Status != WorkOrderStatus.New)
            {
                TempData["ErrorMessage"] = "Không thể chỉnh sửa Lệnh sản xuất đã được ban hành hoặc đang sản xuất.";
                return RedirectToAction(nameof(Details), new { id });
            }
            ViewData["ProductId"] = new SelectList(_context.Products.Where(p => p.Type == workOrder.Product.Type), "Id", "Name", workOrder.ProductId);
            return View(workOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,ProductId,QuantityToProduce,DueDate")] WorkOrder workOrder)
        {
            if (id != workOrder.Id) return NotFound();

            var orderToUpdate = await _context.WorkOrders.FindAsync(id);
            if (orderToUpdate == null || orderToUpdate.Status != WorkOrderStatus.New)
            {
                TempData["ErrorMessage"] = "Không thể chỉnh sửa Lệnh sản xuất này.";
                return RedirectToAction(nameof(Index));
            }

            if (await TryUpdateModelAsync<WorkOrder>(
                orderToUpdate,
                "",
                wo => wo.Description, wo => wo.ProductId, wo => wo.QuantityToProduce, wo => wo.DueDate))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã cập nhật Lệnh sản xuất #{id} thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.WorkOrders.Any(e => e.Id == workOrder.Id)) return NotFound();
                    else throw;
                }
            }
            ViewData["ProductId"] = new SelectList(_context.Products.Where(p => p.Type == orderToUpdate.Product.Type), "Id", "Name", workOrder.ProductId);
            return View(orderToUpdate);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var workOrder = await _context.WorkOrders
                .Include(w => w.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workOrder == null) return NotFound();

            if (workOrder.Status == WorkOrderStatus.Completed)
            {
                TempData["ErrorMessage"] = "Không thể xóa Lệnh sản xuất đã hoàn thành.";
                return RedirectToAction(nameof(Details), new { id });
            }
            if (workOrder.Type == WorkOrderType.Master && await _context.WorkOrders.AnyAsync(wo => wo.ParentWorkOrderId == id))
            {
                TempData["ErrorMessage"] = "Không thể xóa Lệnh sản xuất Gốc khi đã có các Lệnh sản xuất Công đoạn con. Vui lòng xóa các lệnh con trước.";
                return RedirectToAction(nameof(Details), new { id });
            }
            return View(workOrder);
        }
        #endregion

        // POST: WorkOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _workOrderService.DeleteWorkOrderRecursiveAsync(id);
                TempData["SuccessMessage"] = $"Đã xóa thành công Lệnh sản xuất #{id} và các lệnh con liên quan.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Bắt lỗi nghiệp vụ cụ thể từ Service và hiển thị cho người dùng
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                // Bắt các lỗi không lường trước khác (ví dụ: lỗi database)
                // Ghi log lỗi `ex` ở đây để debug
                TempData["ErrorMessage"] = $"Đã có lỗi hệ thống xảy ra khi xóa Lệnh sản xuất. {ex.Message}" ;
            }

            // Nếu có lỗi, quay lại trang chi tiết
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
