using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using Rotativa.AspNetCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;

namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
            ViewData["ActiveMenuGroup"] = "Product";
        }

        // ... (Các action Index, RawMaterialsIndex, Details, Create, Edit không thay đổi) ...
        #region Các Action không thay đổi
        // GET: Products
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;

            var products = from p in _context.Products select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString));
            }
            products = sortOrder switch
            {
                "name_desc" => products.OrderByDescending(p => p.Name),
                "Price" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                _ => products.OrderBy(p => p.Name),
            };
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(products.AsNoTracking().ToPagedList(pageNumber, pageSize));
        }

        // GET: Products/RawMaterialsIndex
        public IActionResult RawMaterialsIndex(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;

            var rawMaterials = from p in _context.Products
                               where p.Type == ProductType.RawMaterial
                               select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                rawMaterials = rawMaterials.Where(s => s.Name.Contains(searchString));
            }
            rawMaterials = sortOrder switch
            {
                "name_desc" => rawMaterials.OrderByDescending(p => p.Name),
                "Price" => rawMaterials.OrderBy(p => p.Price),
                "price_desc" => rawMaterials.OrderByDescending(p => p.Price),
                _ => rawMaterials.OrderBy(p => p.Name),
            };
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(rawMaterials.AsNoTracking().ToPagedList(pageNumber, pageSize));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            // Tải danh sách công đoạn và gửi sang View
            ViewData["ProductionStageId"] = new SelectList(await _context.ProductionStages.OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Name,Sku,Description,Price,Quantity,Cost,StandardLaborCost,StandardOverheadCost,UnitOfMeasure,Color,Width,Gsm,DefaultProductionStageId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã tạo sản phẩm mới thành công.";
                return RedirectToAction(nameof(Index));
            }
            // Nếu có lỗi, tải lại danh sách công đoạn để hiển thị lại form
            ViewData["ProductionStageId"] = new SelectList(await _context.ProductionStages.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", product.DefaultProductionStageId);
            return View(product);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // Tải danh sách công đoạn và gửi sang View
            ViewData["ProductionStageId"] = new SelectList(await _context.ProductionStages.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", product.DefaultProductionStageId);
            return View(product);
        }

        // POST: Products/Edit/5 (ĐÃ CẬP NHẬT)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Name,Sku,Description,Price,Quantity,Cost,StandardLaborCost,StandardOverheadCost,UnitOfMeasure,Color,Width,Gsm,DefaultProductionStageId")] Product product)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
                TempData["SuccessMessage"] = "Đã cập nhật sản phẩm thành công.";
                return RedirectToAction(nameof(Index));
            }
            // Nếu có lỗi, tải lại danh sách công đoạn
            ViewData["ProductionStageId"] = new SelectList(await _context.ProductionStages.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", product.DefaultProductionStageId);
            return View(product);
        }
        #endregion

        // GET: Products/ManageBOM/5
        public async Task<IActionResult> ManageBOM(int? id)
        {
            if (id == null) return NotFound();

            var finishedProduct = await _context.Products.FindAsync(id);

            // SỬA LỖI 1: Chấp nhận cả Thành phẩm và Bán thành phẩm
            if (finishedProduct == null || (finishedProduct.Type != ProductType.FinishedGood && finishedProduct.Type != ProductType.SemiFinishedGood))
            {
                return NotFound();
            }

            ViewBag.ComponentsInBOM = await _context.BillOfMaterials
                .Where(b => b.FinishedProductId == id)
                .Include(b => b.Component)
                .Include(b => b.ProductionStage) // Thêm Include cho Công đoạn
                .ToListAsync();

            ViewBag.ProductionStages = new SelectList(
                _context.ProductionStages.OrderBy(p => p.Sequence), "Id", "Name");

            ViewBag.AvailableComponents = new SelectList(
                _context.Products.Where(p => p.Type == ProductType.RawMaterial || p.Type == ProductType.SemiFinishedGood), "Id", "Name");

            return View(finishedProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComponentToBOM(
    int finishedProductId, int componentId, int productionStageId,
    decimal quantity, // Giá trị mặc định nếu không tính toán
    decimal? pieceLength, decimal? pieceWidth, int? piecesPerProduct, decimal? gsm)
        {
            // Tạo đối tượng BOM mới
            var bomItem = new BillOfMaterial
            {
                FinishedProductId = finishedProductId,
                ComponentId = componentId,
                ProductionStageId = productionStageId,
                PieceLength = pieceLength,
                PieceWidth = pieceWidth,
                PiecesPerProduct = piecesPerProduct,
                Gsm = gsm
            };

            // === LOGIC TÍNH TOÁN TỰ ĐỘNG ===
            // Ưu tiên tính theo GSM (theo diện tích)
            if (pieceLength.HasValue && pieceWidth.HasValue && piecesPerProduct.HasValue && gsm.HasValue)
            {
                // Công thức: (Dài * Rộng * Số mảnh * GSM) / 1000 = Số kg cần dùng
                bomItem.Quantity = (pieceLength.Value * pieceWidth.Value * piecesPerProduct.Value * gsm.Value) / 1000;
            }
            else
            {
                // Nếu không có đủ thông số, lấy giá trị Quantity người dùng nhập trực tiếp
                bomItem.Quantity = quantity;
            }

            _context.BillOfMaterials.Add(bomItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageBOM), new { id = finishedProductId });
        }
        // GET: Products/InventoryReport
        public async Task<IActionResult> InventoryReport()
        {
            var allProducts = await _context.Products.OrderBy(p => p.Name).ToListAsync();
            return new ViewAsPdf("InventoryReport", allProducts)
            {
                FileName = $"InventoryReport-{DateTime.Now:yyyy-MM-dd}.pdf"
            };
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        // POST: Products/DeleteComponentFromBOM/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComponentFromBOM(int bomId, int finishedProductId)
        {
            var bomItem = await _context.BillOfMaterials.FindAsync(bomId);
            if (bomItem != null)
            {
                _context.BillOfMaterials.Remove(bomItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageBOM), new { id = finishedProductId });
        }
    }   
}