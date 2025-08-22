// File: Controllers/ProductionStagesController.cs (ĐÃ CẬP NHẬT)
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppERP.Data;
using WebAppERP.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductionStagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductionStagesController(ApplicationDbContext context)
        {
            _context = context;
            ViewData["ActiveMenuGroup"] = "MasterData";
        }

        // GET: ProductionStages
        public async Task<IActionResult> Index()
        {
            // Thêm Include để hiển thị tên Workshop
            var stages = await _context.ProductionStages.Include(ps => ps.Workshop).OrderBy(p => p.Sequence).ToListAsync();
            return View(stages);
        }

        // GET: ProductionStages/Create
        public async Task<IActionResult> Create()
        {
            ViewData["WorkshopId"] = new SelectList(await _context.Workshops.OrderBy(w => w.Name).ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: ProductionStages/Create (Cập nhật Bind)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Sequence,WorkshopId,ProcessingMethod")] ProductionStage productionStage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productionStage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkshopId"] = new SelectList(await _context.Workshops.OrderBy(w => w.Name).ToListAsync(), "Id", "Name", productionStage.WorkshopId);
            return View(productionStage);
        }

        // GET: ProductionStages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var productionStage = await _context.ProductionStages.FindAsync(id);
            if (productionStage == null) return NotFound();

            ViewData["WorkshopId"] = new SelectList(await _context.Workshops.OrderBy(w => w.Name).ToListAsync(), "Id", "Name", productionStage.WorkshopId);
            return View(productionStage);
        }

        // POST: ProductionStages/Edit/5 (Cập nhật Bind)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Sequence,WorkshopId,ProcessingMethod")] ProductionStage productionStage)
        {
            if (id != productionStage.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productionStage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ProductionStages.Any(e => e.Id == productionStage.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkshopId"] = new SelectList(await _context.Workshops.OrderBy(w => w.Name).ToListAsync(), "Id", "Name", productionStage.WorkshopId);
            return View(productionStage);
        }

        // ... (Các action Details, Delete không thay đổi) ...
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var productionStage = await _context.ProductionStages.Include(ps => ps.Workshop).FirstOrDefaultAsync(m => m.Id == id);
            if (productionStage == null) return NotFound();
            return View(productionStage);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var productionStage = await _context.ProductionStages.FirstOrDefaultAsync(m => m.Id == id);
            if (productionStage == null) return NotFound();
            return View(productionStage);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productionStage = await _context.ProductionStages.FindAsync(id);
            _context.ProductionStages.Remove(productionStage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}