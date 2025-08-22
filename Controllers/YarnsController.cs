using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;

namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class YarnsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public YarnsController(ApplicationDbContext context)
        {
            _context = context;
            ViewData["ActiveMenuGroup"] = "Material";
        }

        // GET: Yarns
        public async Task<IActionResult> Index()
        {
            // THAY ĐỔI 1: Tải kèm thông tin của Product và Machine
            var yarns = _context.Yarns.Include(y => y.Product).Include(y => y.Machine);
            return View(await yarns.ToListAsync());
        }

        // GET: Yarns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var yarn = await _context.Yarns
                .Include(y => y.Product) // << THAY ĐỔI TẠI ĐÂY
                .Include(y => y.Machine)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (yarn == null) return NotFound();
            return View(yarn);
        }

        // Sửa lại Action Create (GET) để nhận workOrderId
        public async Task<IActionResult> Create(int? workOrderId)
        {
            if (workOrderId.HasValue)
            {
                // Gửi workOrderId sang View để tự động điền
                ViewData["WorkOrderId"] = workOrderId.Value;
            }
            await PopulateDropdowns();
            return View();
        }
        // POST: Yarns/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ProductId,OperatorId,MachineId,SpoolCount,GrossWeight,NetWeight,SpoolWeight,StockQuantity,WorkOrderId")] Yarn yarn)
        {
            ModelState.Remove("Product");
            ModelState.Remove("Machine");
            ModelState.Remove("Operator");
            ModelState.Remove("WorkOrder");

            if (ModelState.IsValid)
            {
                _context.Add(yarn);
                await _context.SaveChangesAsync();

                // << TOÀN BỘ LOGIC CẬP NHẬT WORKORDER ĐÃ ĐƯỢC XÓA BỎ KHỎI ĐÂY >>
                // Nhiệm vụ của Action này giờ chỉ đơn thuần là tạo mới một bản ghi Yarn.

                TempData["SuccessMessage"] = "Đã tạo mới lô sợi thành công.";
                return RedirectToAction(nameof(Index));
            }

            // Nếu model không hợp lệ, tải lại danh sách dropdown
            await PopulateDropdowns(yarn);
            return View(yarn);
        }
        // GET: Yarns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var yarn = await _context.Yarns.FindAsync(id);
            if (yarn == null) return NotFound();
            await PopulateDropdowns(yarn);
            return View(yarn);
        }

        // POST: Yarns/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ProductId,OperatorId,MachineId,SpoolCount,GrossWeight,NetWeight,SpoolWeight,StockQuantity,WorkOrderId")] Yarn yarn)
        {
            if (id != yarn.ID)
            {
                return NotFound();
            }

            ModelState.Remove("Product");
            ModelState.Remove("Machine");
            ModelState.Remove("Operator");
            ModelState.Remove("WorkOrder");

            if (ModelState.IsValid)
            {
                try
                {
                    // Logic bây giờ chỉ đơn thuần là cập nhật lại bản ghi Yarn
                    // Toàn bộ phần code phức tạp về trừ/cộng sản lượng vào WorkOrder cũ đã được xóa bỏ
                    _context.Update(yarn);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!YarnExists(yarn.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Đã cập nhật thông tin lô sợi thành công.";
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, tải lại dropdowns và hiển thị lại form
            await PopulateDropdowns(yarn);
            return View(yarn);
        }
        // Thêm phương thức helper này vào cuối controller YarnsController
        private void UpdateWorkOrderStatus(WorkOrder workOrder)
        {            
                workOrder.Status = WorkOrderStatus.New;
            
        }

        // GET: Yarns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var yarn = await _context.Yarns
                .Include(y => y.Product) // << THAY ĐỔI TẠI ĐÂY
                .Include(y => y.Machine)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (yarn == null) return NotFound();
            return View(yarn);
        }

        // POST: Yarns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var yarn = await _context.Yarns.FindAsync(id);
            if (yarn != null)
            {
                _context.Yarns.Remove(yarn);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // THAY ĐỔI 3: Cập nhật hàm PopulateDropdowns để lấy danh sách "Sản phẩm Sợi"
        // ...
        private async Task PopulateDropdowns(Yarn yarn = null)
        {
            // Lấy danh sách sản phẩm sợi
            var yarnProducts = await _context.Products
                .Where(p => p.Type == ProductType.SemiFinishedGood)
                .OrderBy(p => p.Name)
                .ToListAsync();
            ViewData["ProductId"] = new SelectList(yarnProducts, "Id", "Name", yarn?.ProductId);

            // Lấy danh sách máy tạo sợi
            var yarnMachines = await _context.Machines
                .Include(m => m.MachineType)
                .Where(m => m.MachineType != null && m.MachineType.Name.ToUpper() == "MÁY TẠO SỢI")
                .ToListAsync();
            ViewData["MachineId"] = new SelectList(yarnMachines, "ID", "Name", yarn?.MachineId);

            // Lấy danh sách nhân viên
            ViewData["OperatorId"] = new SelectList(_context.Users, "Id", "UserName", yarn?.OperatorId);

            // Lấy danh sách Lệnh sản xuất đang hoạt động
            var activeWorkOrders = await _context.WorkOrders
                .Include(wo => wo.Product)
                .Where(wo => wo.Product.Type == ProductType.SemiFinishedGood &&
                             (wo.Status == WorkOrderStatus.New || wo.Status == WorkOrderStatus.InProgress))
                .OrderBy(wo => wo.Id)
                .Select(wo => new {
                    Id = wo.Id,
                    DisplayText = $"#{wo.Id} - {wo.Product.Name}"
                })
                .ToListAsync();

            var selectedWorkOrderId = yarn?.WorkOrderId;
            // Gán vào ViewData - Phần này đã đúng
            ViewData["WorkOrderId"] = new SelectList(activeWorkOrders, "Id", "DisplayText", selectedWorkOrderId);
        }
        // ...

        private bool YarnExists(int id)
        {
            return _context.Yarns.Any(e => e.ID == id);
        }
    }
}