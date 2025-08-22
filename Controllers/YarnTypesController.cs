using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class YarnTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public YarnTypesController(ApplicationDbContext context)
        {
            _context = context;
            // Đặt menu group để sidebar tự động mở rộng
            ViewData["ActiveMenuGroup"] = "Material";
        }

        // GET: YarnTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.YarnTypes.ToListAsync());
        }

        // GET: YarnTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var yarnType = await _context.YarnTypes.FirstOrDefaultAsync(m => m.ID == id);
            if (yarnType == null) return NotFound();
            return View(yarnType);
        }

        // GET: YarnTypes/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products.Where(p => p.Type == ProductType.SemiFinishedGood), "Id", "Name");
            return View();
        }

        // POST: YarnTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Note")] YarnType yarnType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(yarnType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(yarnType);
        }

        // GET: YarnTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var yarnType = await _context.YarnTypes.FindAsync(id);
            if (yarnType == null) return NotFound();
            ViewData["ProductId"] = new SelectList(_context.Products.Where(p => p.Type == ProductType.SemiFinishedGood), "Id", "Name");

            return View(yarnType);
        }

        // POST: YarnTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Note")] YarnType yarnType)
        {
            if (id != yarnType.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(yarnType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!YarnTypeExists(yarnType.ID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(yarnType);
        }

        // GET: YarnTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var yarnType = await _context.YarnTypes.FirstOrDefaultAsync(m => m.ID == id);
            if (yarnType == null) return NotFound();
            return View(yarnType);
        }

        // POST: YarnTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var yarnType = await _context.YarnTypes.FindAsync(id);
            _context.YarnTypes.Remove(yarnType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // Thêm Action này vào trong YarnTypesController
        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] YarnType yarnType)
        {
            if (yarnType == null || string.IsNullOrWhiteSpace(yarnType.Name))
            {
                return Json(new { success = false, message = "Tên kiểu sợi không được để trống." });
            }

            if (await _context.YarnTypes.AnyAsync(m => m.Name == yarnType.Name))
            {
                return Json(new { success = false, message = "Tên kiểu sợi này đã tồn tại." });
            }

            if (ModelState.IsValid)
            {
                _context.Add(yarnType);
                await _context.SaveChangesAsync();

                var yarnTypes = await _context.YarnTypes.OrderBy(m => m.Name).Select(m => new { id = m.ID, name = m.Name }).ToListAsync();

                return Json(new
                {
                    success = true,
                    message = "Thêm thành công!",
                    data = yarnTypes,
                    newId = yarnType.ID
                });
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }
        private bool YarnTypeExists(int id)
        {
            return _context.YarnTypes.Any(e => e.ID == id);
        }
    }
}