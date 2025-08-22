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
    public class JournalEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JournalEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JournalEntries
        public async Task<IActionResult> Index()
        {
            return View(await _context.JournalEntries.ToListAsync());
        }

        // GET: JournalEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalEntry = await _context.JournalEntries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journalEntry == null)
            {
                return NotFound();
            }

            return View(journalEntry);
        }

        // GET: JournalEntries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JournalEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EntryDate,Description")] JournalEntry journalEntry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(journalEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(journalEntry);
        }

        // GET: JournalEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalEntry = await _context.JournalEntries.FindAsync(id);
            if (journalEntry == null)
            {
                return NotFound();
            }
            return View(journalEntry);
        }

        // POST: JournalEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EntryDate,Description")] JournalEntry journalEntry)
        {
            if (id != journalEntry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(journalEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JournalEntryExists(journalEntry.Id))
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
            return View(journalEntry);
        }

        // GET: JournalEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalEntry = await _context.JournalEntries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journalEntry == null)
            {
                return NotFound();
            }

            return View(journalEntry);
        }

        // POST: JournalEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var journalEntry = await _context.JournalEntries.FindAsync(id);
            _context.JournalEntries.Remove(journalEntry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JournalEntryExists(int id)
        {
            return _context.JournalEntries.Any(e => e.Id == id);
        }
        // Action ví dụ để thêm dòng và cập nhật số dư
        [HttpPost]
        public async Task<IActionResult> AddLine(int journalEntryId, int accountId, DebitOrCredit debitOrCredit, decimal amount)
        {
            // ... (Code tạo JournalEntryLine) ...

            var account = await _context.Accounts.FindAsync(accountId);
            if (debitOrCredit == DebitOrCredit.Debit)
            {
                account.Balance += amount; // Nợ làm tăng tài sản/chi phí
            }
            else
            {
                account.Balance -= amount; // Có làm giảm tài sản/chi phí (logic này cần được điều chỉnh tùy loại tài khoản)
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = journalEntryId });
        }
    }
}
