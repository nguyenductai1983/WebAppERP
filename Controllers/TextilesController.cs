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
    public class TextilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TextilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Textiles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Textiles.Include(t => t.Machine).Include(t => t.Operator).Include(t => t.TextileType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Textiles/Details/5
        // Sửa lại action Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var textile = await _context.Textiles
                .Include(t => t.TextileType)
                .Include(t => t.Machine)
                .Include(t => t.Operator)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (textile == null) return NotFound();

            // Lấy danh sách Sợi đã sử dụng cho cây Vải này
            ViewBag.UsedYarns = await _context.TextileYarnUsages
                .Where(u => u.TextileId == id)
                .Include(u => u.Yarn)
                .ThenInclude(y => y.Product)
                .ToListAsync();

            // Lấy danh sách Sợi còn tồn kho để thêm vào
            ViewData["YarnId"] = new SelectList(_context.Yarns.Where(y => y.StockQuantity > 0), "ID", "ID"); // Hiển thị ID của lô sợi

            return View(textile);
        }
        // GET: Textiles/Create
        // GET: Textiles/Create
        public async Task<IActionResult> Create()
        {
            // Lấy danh sách máy dệt
            ViewData["MachineId"] = new SelectList(
                await _context.Machines.Include(m => m.MachineType)
                              .Where(m => m.MachineType.Name == "Máy dệt")
                              .ToListAsync(),
                "ID", "Name");

            // Lấy danh sách công nhân
            ViewData["OperatorId"] = new SelectList(await _context.Users.ToListAsync(), "Id", "UserName");

            // Lấy danh sách loại vải
            ViewData["TextileTypeId"] = new SelectList(await _context.TextileTypes.ToListAsync(), "ID", "Name");

            // === SỬA LẠI KHỐI CODE BÊN DƯỚI ===
            // Thêm .Include(y => y.Product) để tải thông tin Sản phẩm Sợi
            var availableYarns = await _context.Yarns
                .Include(y => y.Product) // Tải kèm thông tin Product
                .Where(y => y.StockQuantity > 0)
                .ToListAsync();

            // Tạo SelectList từ danh sách đã có đầy đủ dữ liệu
            ViewData["SourceYarnId"] = new SelectList(availableYarns, "ID", "Product.Name");

            return View();
        }

        // POST: Textiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Textiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,TextileTypeId,MachineId,OperatorId,Code,ActualWidth,InitialLength,GrossWeight,NetWeight,Quality,SourceYarnId")] Textile textile)
        {
            // Giả định: Trọng lượng NW (NetWeight) của Vải chính là lượng Sợi đã tiêu thụ
            decimal quantityYarnUsed = textile.NetWeight;

            if (ModelState.IsValid)
            {
                // === LOGIC NGHIỆP VỤ MỚI ===
                if (textile.SourceYarnId.HasValue && quantityYarnUsed > 0)
                {
                    var sourceYarn = await _context.Yarns.FindAsync(textile.SourceYarnId.Value);
                    if (sourceYarn == null)
                    {
                        ModelState.AddModelError("SourceYarnId", "Lô sợi nguồn không tồn tại.");
                    }
                    else if (sourceYarn.StockQuantity < quantityYarnUsed)
                    {
                        ModelState.AddModelError("SourceYarnId", $"Tồn kho lô sợi {sourceYarn.ID} không đủ (còn {sourceYarn.StockQuantity} kg, cần {quantityYarnUsed} kg).");
                    }
                    else
                    {
                        // Trừ tồn kho của lô sợi đã dùng
                        sourceYarn.StockQuantity -= quantityYarnUsed;
                    }
                }

                if (ModelState.ErrorCount == 0)
                {
                    _context.Add(textile);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            // Nếu có lỗi, tải lại các dropdown
            ViewData["MachineId"] = new SelectList(_context.Machines.Where(m => m.MachineType.Name == "Máy dệt"), "ID", "Name", textile.MachineId);
            ViewData["OperatorId"] = new SelectList(_context.Users, "Id", "UserName", textile.OperatorId);
            ViewData["TextileTypeId"] = new SelectList(_context.TextileTypes, "ID", "Name", textile.TextileTypeId);
            ViewData["SourceYarnId"] = new SelectList(_context.Yarns.Where(y => y.StockQuantity > 0), "ID", "Product.Name", textile.SourceYarnId);

            return View(textile);
        }

        // GET: Textiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textile = await _context.Textiles.FindAsync(id);
            if (textile == null)
            {
                return NotFound();
            }
            ViewData["MachineId"] = new SelectList(_context.Machines, "ID", "Name", textile.MachineId);
            ViewData["OperatorId"] = new SelectList(_context.Users, "Id", "Id", textile.OperatorId);
            ViewData["TextileTypeId"] = new SelectList(_context.TextileTypes, "ID", "Name", textile.TextileTypeId);
            return View(textile);
        }

        // POST: Textiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TextileTypeId,MachineId,OperatorId,Code,ActualWidth,InitialLength,GrossWeight,NetWeight,Quality,Created_at,Updated_at")] Textile textile)
        {
            if (id != textile.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(textile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TextileExists(textile.ID))
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
            ViewData["MachineId"] = new SelectList(_context.Machines, "ID", "Name", textile.MachineId);
            ViewData["OperatorId"] = new SelectList(_context.Users, "Id", "Id", textile.OperatorId);
            ViewData["TextileTypeId"] = new SelectList(_context.TextileTypes, "ID", "Name", textile.TextileTypeId);
            return View(textile);
        }

        // GET: Textiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textile = await _context.Textiles
                .Include(t => t.Machine)
                .Include(t => t.Operator)
                .Include(t => t.TextileType)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (textile == null)
            {
                return NotFound();
            }

            return View(textile);
        }

        // POST: Textiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var textile = await _context.Textiles.FindAsync(id);
            _context.Textiles.Remove(textile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TextileExists(int id)
        {
            return _context.Textiles.Any(e => e.ID == id);
        }
    }
}
