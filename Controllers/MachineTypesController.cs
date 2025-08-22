using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppERP.Data;
using WebAppERP.Models;
using Microsoft.AspNetCore.Http;
namespace WebAppERP.Controllers
{
    public class MachineTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MachineTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MachineTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.MachineTypes.ToListAsync());
        }

        // GET: MachineTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineType = await _context.MachineTypes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (machineType == null)
            {
                return NotFound();
            }

            return View(machineType);
        }

        // GET: MachineTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MachineTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] MachineType machineType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(machineType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(machineType);
        }

        // GET: MachineTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineType = await _context.MachineTypes.FindAsync(id);
            if (machineType == null)
            {
                return NotFound();
            }
            return View(machineType);
        }

        // POST: MachineTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] MachineType machineType)
        {
            if (id != machineType.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(machineType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MachineTypeExists(machineType.ID))
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
            return View(machineType);
        }

        // GET: MachineTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineType = await _context.MachineTypes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (machineType == null)
            {
                return NotFound();
            }

            return View(machineType);
        }

        // POST: MachineTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var machineType = await _context.MachineTypes.FindAsync(id);
            _context.MachineTypes.Remove(machineType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
      
        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] MachineType machineType)
        {
            // Kiểm tra dữ liệu đầu vào
            if (machineType == null
                || string.IsNullOrWhiteSpace(machineType.Name)
                || machineType.ProductionStageId <= 0) // <-- KIỂM TRA THÊM CHO CÔNG ĐOẠN
            {
                return Json(new { success = false, message = "Tên loại máy và công đoạn không được để trống." });
            }

            if (await _context.MachineTypes.AnyAsync(m => m.Name == machineType.Name))
            {
                return Json(new { success = false, message = "Tên loại máy này đã tồn tại." });
            }

            // Dữ liệu đã hợp lệ, thêm vào database
            // machineType đã chứa cả Name và ProductionStageId từ Ajax gửi lên
            _context.Add(machineType);
            await _context.SaveChangesAsync();

            // Trả về danh sách đã cập nhật
            var machineTypes = await _context.MachineTypes.OrderBy(m => m.Name).Select(m => new { id = m.ID, name = m.Name }).ToListAsync();

            return Json(new
            {
                success = true,
                data = machineTypes,
                newId = machineType.ID
            });
        }
        private bool MachineTypeExists(int id)
        {
            return _context.MachineTypes.Any(e => e.ID == id);
        }
    }
}
