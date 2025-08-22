using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppERP.Data;
using WebAppERP.Models;

namespace WebAppERP.Controllers
{
    public class MachinesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MachinesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Machines
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Machines.Include(m => m.MachineType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Machines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machine = await _context.Machines
                .Include(m => m.MachineType)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (machine == null)
            {
                return NotFound();
            }

            return View(machine);
        }

        // Sửa lại Action này để tải thêm danh sách Phân xưởng
        // GET: Machines/Create
        public async Task<IActionResult> Create()
        {
            ViewData["MachineTypeId"] = new SelectList(await _context.MachineTypes.OrderBy(m => m.Name).ToListAsync(), "ID", "Name");
            // THÊM DÒNG NÀY: Tải danh sách Phân xưởng
            ViewData["WorkshopId"] = new SelectList(await _context.Workshops.OrderBy(w => w.Name).ToListAsync(), "Id", "Name");
            ViewData["ProductionStageId"] = new SelectList(await _context.ProductionStages.OrderBy(ps => ps.Sequence).ToListAsync(), "Id", "Name");

            return View();
        }

        // Sửa lại Action này để đảm bảo Bind có WorkshopId
        // POST: Machines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,MachineTypeId,WorkshopId,Status")] Machine machine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(machine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Nếu có lỗi, tải lại cả hai danh sách
            ViewData["MachineTypeId"] = new SelectList(await _context.MachineTypes.OrderBy(m => m.Name).ToListAsync(), "ID", "Name", machine.MachineTypeId);
            ViewData["WorkshopId"] = new SelectList(await _context.Workshops.OrderBy(w => w.Name).ToListAsync(), "Id", "Name", machine.WorkshopId);
            return View(machine);
        }

        // Tương tự, cập nhật Action Edit (GET) để tải danh sách Phân xưởng
        // GET: Machines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var machine = await _context.Machines.FindAsync(id);
            if (machine == null) return NotFound();

            ViewData["MachineTypeId"] = new SelectList(await _context.MachineTypes.OrderBy(m => m.Name).ToListAsync(), "ID", "Name", machine.MachineTypeId);
            ViewData["WorkshopId"] = new SelectList(await _context.Workshops.OrderBy(w => w.Name).ToListAsync(), "Id", "Name", machine.WorkshopId);
            return View(machine);
        }

        // POST: Machines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,MachineTypeId,Status")] Machine machine)
        {
            if (id != machine.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(machine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MachineExists(machine.ID))
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
            ViewData["MachineTypeId"] = new SelectList(_context.MachineTypes, "ID", "Name", machine.MachineTypeId);
            return View(machine);
        }

        // GET: Machines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machine = await _context.Machines
                .Include(m => m.MachineType)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (machine == null)
            {
                return NotFound();
            }

            return View(machine);
        }

        // POST: Machines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var machine = await _context.Machines.FindAsync(id);
            _context.Machines.Remove(machine);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MachineExists(int id)
        {
            return _context.Machines.Any(e => e.ID == id);
        }
    }
}
