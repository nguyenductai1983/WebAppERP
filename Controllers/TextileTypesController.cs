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
    public class TextileTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TextileTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TextileTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.TextileTypes.ToListAsync());
        }

        // GET: TextileTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textileType = await _context.TextileTypes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (textileType == null)
            {
                return NotFound();
            }

            return View(textileType);
        }

        // GET: TextileTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TextileTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Note")] TextileType textileType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(textileType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(textileType);
        }

        // GET: TextileTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textileType = await _context.TextileTypes.FindAsync(id);
            if (textileType == null)
            {
                return NotFound();
            }
            return View(textileType);
        }

        // POST: TextileTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Note")] TextileType textileType)
        {
            if (id != textileType.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(textileType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TextileTypeExists(textileType.ID))
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
            return View(textileType);
        }

        // GET: TextileTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textileType = await _context.TextileTypes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (textileType == null)
            {
                return NotFound();
            }

            return View(textileType);
        }

        // POST: TextileTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var textileType = await _context.TextileTypes.FindAsync(id);
            _context.TextileTypes.Remove(textileType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TextileTypeExists(int id)
        {
            return _context.TextileTypes.Any(e => e.ID == id);
        }
    }
}
