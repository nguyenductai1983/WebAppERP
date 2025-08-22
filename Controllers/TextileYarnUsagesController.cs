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
    public class TextileYarnUsagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TextileYarnUsagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TextileYarnUsages
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TextileYarnUsages.Include(t => t.Textile).Include(t => t.Yarn);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TextileYarnUsages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textileYarnUsage = await _context.TextileYarnUsages
                .Include(t => t.Textile)
                .Include(t => t.Yarn)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (textileYarnUsage == null)
            {
                return NotFound();
            }

            return View(textileYarnUsage);
        }

        // GET: TextileYarnUsages/Create
        public IActionResult Create()
        {
            ViewData["TextileId"] = new SelectList(_context.Textiles, "ID", "Code");
            ViewData["YarnId"] = new SelectList(_context.Yarns, "ID", "OperatorId");
            return View();
        }

        // POST: TextileYarnUsages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,TextileId,YarnId,QuantityUsed,UsageDate")] TextileYarnUsage textileYarnUsage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(textileYarnUsage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TextileId"] = new SelectList(_context.Textiles, "ID", "Code", textileYarnUsage.TextileId);
            ViewData["YarnId"] = new SelectList(_context.Yarns, "ID", "OperatorId", textileYarnUsage.YarnId);
            return View(textileYarnUsage);
        }

        // GET: TextileYarnUsages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textileYarnUsage = await _context.TextileYarnUsages.FindAsync(id);
            if (textileYarnUsage == null)
            {
                return NotFound();
            }
            ViewData["TextileId"] = new SelectList(_context.Textiles, "ID", "Code", textileYarnUsage.TextileId);
            ViewData["YarnId"] = new SelectList(_context.Yarns, "ID", "OperatorId", textileYarnUsage.YarnId);
            return View(textileYarnUsage);
        }

        // POST: TextileYarnUsages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TextileId,YarnId,QuantityUsed,UsageDate")] TextileYarnUsage textileYarnUsage)
        {
            if (id != textileYarnUsage.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(textileYarnUsage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TextileYarnUsageExists(textileYarnUsage.ID))
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
            ViewData["TextileId"] = new SelectList(_context.Textiles, "ID", "Code", textileYarnUsage.TextileId);
            ViewData["YarnId"] = new SelectList(_context.Yarns, "ID", "OperatorId", textileYarnUsage.YarnId);
            return View(textileYarnUsage);
        }

        // GET: TextileYarnUsages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textileYarnUsage = await _context.TextileYarnUsages
                .Include(t => t.Textile)
                .Include(t => t.Yarn)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (textileYarnUsage == null)
            {
                return NotFound();
            }

            return View(textileYarnUsage);
        }

        // POST: TextileYarnUsages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var textileYarnUsage = await _context.TextileYarnUsages.FindAsync(id);
            _context.TextileYarnUsages.Remove(textileYarnUsage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TextileYarnUsageExists(int id)
        {
            return _context.TextileYarnUsages.Any(e => e.ID == id);
        }
        // Thêm action mới này vào TextilesController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddYarnUsage(int textileId, int yarnId, decimal quantityUsed)
        {
            var yarn = await _context.Yarns.FindAsync(yarnId);
            if (yarn == null || quantityUsed <= 0 || yarn.StockQuantity < quantityUsed)
            {
                TempData["ErrorMessage"] = "Lô sợi không hợp lệ hoặc không đủ tồn kho.";
                return RedirectToAction(nameof(Details), new { id = textileId });
            }

            // 1. Tạo bản ghi ghi nhận việc sử dụng
            var usage = new TextileYarnUsage
            {
                TextileId = textileId,
                YarnId = yarnId,
                QuantityUsed = quantityUsed,
                UsageDate = DateTime.Now
            };
            _context.TextileYarnUsages.Add(usage);

            // 2. Trừ tồn kho Sợi
            yarn.StockQuantity -= quantityUsed;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã ghi nhận tiêu thụ Sợi thành công!";
            return RedirectToAction(nameof(Details), new { id = textileId });
        }
    }
}
