using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppERP.Data;
using WebAppERP.Models;
using Microsoft.AspNetCore.Http;
namespace WebAppERP.Controllers
{
    // Chỉ Admin mới có quyền quản lý dữ liệu gốc này
    [Authorize(Roles = "Admin")]
    public class WorkshopsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkshopsController(ApplicationDbContext context)
        {
            _context = context;
            // Đặt menu group để sidebar tự động mở rộng
            ViewData["ActiveMenuGroup"] = "MasterData"; // Hoặc "Production" tùy cách bạn tổ chức
        }

        // GET: Workshops
        public async Task<IActionResult> Index()
        {
            return View(await _context.Workshops.ToListAsync());
        }

        // GET: Workshops/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workshop = await _context.Workshops
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workshop == null)
            {
                return NotFound();
            }

            return View(workshop);
        }

        // GET: Workshops/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workshops/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Workshop workshop)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workshop);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workshop);
        }

        // GET: Workshops/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workshop = await _context.Workshops.FindAsync(id);
            if (workshop == null)
            {
                return NotFound();
            }
            return View(workshop);
        }

        // POST: Workshops/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Workshop workshop)
        {
            if (id != workshop.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workshop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkshopExists(workshop.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(workshop);
        }

        // GET: Workshops/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workshop = await _context.Workshops
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workshop == null)
            {
                return NotFound();
            }

            return View(workshop);
        }

        // POST: Workshops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workshop = await _context.Workshops.FindAsync(id);
            _context.Workshops.Remove(workshop);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // Thêm Action này vào trong WorkshopsController
        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] Workshop workshop)
        {
            if (workshop == null || string.IsNullOrWhiteSpace(workshop.Name))
            {
                return Json(new { success = false, message = "Tên phân xưởng không được để trống." });
            }

            if (await _context.Workshops.AnyAsync(m => m.Name == workshop.Name))
            {
                return Json(new { success = false, message = "Tên phân xưởng này đã tồn tại." });
            }

            // Gán các giá trị và lưu
            _context.Add(workshop);
            await _context.SaveChangesAsync();

            // Sau khi lưu, tải lại toàn bộ danh sách phân xưởng
            var workshops = await _context.Workshops.OrderBy(m => m.Name).Select(m => new { id = m.Id, name = m.Name }).ToListAsync();

            return Json(new
            {
                success = true,
                message = "Thêm thành công!",
                data = workshops,
                newId = workshop.Id // Trả về ID của phân xưởng vừa tạo
            });
        }
        private bool WorkshopExists(int id)
        {
            return _context.Workshops.Any(e => e.Id == id);
        }
    }
}