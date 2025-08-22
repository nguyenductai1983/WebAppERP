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
    public class CoatedTextilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoatedTextilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CoatedTextiles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CoatedTextiles.Include(c => c.Machine).Include(c => c.Operator);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CoatedTextiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coatedTextile = await _context.CoatedTextiles
                .Include(c => c.Machine)
                .Include(c => c.Operator)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (coatedTextile == null)
            {
                return NotFound();
            }

            return View(coatedTextile);
        }

        // GET: CoatedTextiles/Create
        public IActionResult Create()
        {
            ViewData["MachineId"] = new SelectList(_context.Machines, "ID", "Name");
            ViewData["OperatorId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: CoatedTextiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MachineId,OperatorId,Code,InitialLength,GrossWeight,NetWeight,Quality,Created_at,Updated_at")] CoatedTextile coatedTextile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coatedTextile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
         //   coatedTextile.OperatorId = _userManager.GetUserId(User);
            ViewData["MachineId"] = new SelectList(_context.Machines, "ID", "Name", coatedTextile.MachineId);
            ViewData["OperatorId"] = new SelectList(_context.Users, "Id", "Id", coatedTextile.OperatorId);
            return View(coatedTextile);
        }

        // GET: CoatedTextiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coatedTextile = await _context.CoatedTextiles.FindAsync(id);
            if (coatedTextile == null)
            {
                return NotFound();
            }
            ViewData["MachineId"] = new SelectList(_context.Machines, "ID", "Name", coatedTextile.MachineId);
            ViewData["OperatorId"] = new SelectList(_context.Users, "Id", "Id", coatedTextile.OperatorId);
            return View(coatedTextile);
        }

        // POST: CoatedTextiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,MachineId,OperatorId,Code,InitialLength,GrossWeight,NetWeight,Quality,Created_at,Updated_at")] CoatedTextile coatedTextile)
        {
            if (id != coatedTextile.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coatedTextile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoatedTextileExists(coatedTextile.ID))
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
            ViewData["MachineId"] = new SelectList(_context.Machines, "ID", "Name", coatedTextile.MachineId);
            ViewData["OperatorId"] = new SelectList(_context.Users, "Id", "Id", coatedTextile.OperatorId);
            return View(coatedTextile);
        }

        // GET: CoatedTextiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coatedTextile = await _context.CoatedTextiles
                .Include(c => c.Machine)
                .Include(c => c.Operator)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (coatedTextile == null)
            {
                return NotFound();
            }

            return View(coatedTextile);
        }

        // POST: CoatedTextiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coatedTextile = await _context.CoatedTextiles.FindAsync(id);
            _context.CoatedTextiles.Remove(coatedTextile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoatedTextileExists(int id)
        {
            return _context.CoatedTextiles.Any(e => e.ID == id);
        }
    }
}
