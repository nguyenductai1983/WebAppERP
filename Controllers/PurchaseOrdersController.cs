using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppERP.Data;
using WebAppERP.Models;
using Rotativa.AspNetCore;
using WebAppERP.Services;
using Microsoft.AspNetCore.Identity;
namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PurchaseOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AccountingService _accountingService; // Khai báo service
        private readonly IInventoryService _inventoryService;
        private readonly UserManager<IdentityUser> _userManager;
        public PurchaseOrdersController(
       ApplicationDbContext context,
       AccountingService accountingService,
       UserManager<IdentityUser> userManager,
       IInventoryService inventoryService) // <-- THÊM THAM SỐ NÀY
        {
            _context = context;
            _accountingService = accountingService;
            _userManager = userManager;
            _inventoryService = inventoryService; // <-- KHỞI TẠO NÓ
        }

        // GET: PurchaseOrders
        public async Task<IActionResult> Index()
        {
            var purchaseOrders = _context.PurchaseOrders.Include(p => p.Supplier).OrderByDescending(p => p.OrderDate);
            return View(await purchaseOrders.ToListAsync());
        }

        // GET: PurchaseOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.OrderDetails)
                    .ThenInclude(pd => pd.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (purchaseOrder == null) return NotFound();

            // Gửi danh sách sản phẩm sang View để thêm vào đơn
            ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Create
        public async Task<IActionResult> Create()
        {
            // Gửi danh sách nhà cung cấp sang View
            ViewBag.Suppliers = new SelectList(await _context.Suppliers.ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: PurchaseOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SupplierId,ExpectedDeliveryDate")] PurchaseOrder purchaseOrder)
        {
            purchaseOrder.OrderDate = System.DateTime.Now;
            purchaseOrder.Status = PurchaseOrderStatus.Draft;

            if (ModelState.IsValid)
            {
                _context.Add(purchaseOrder);
                await _context.SaveChangesAsync();
                // Chuyển hướng đến trang Details để bắt đầu thêm sản phẩm
                return RedirectToAction(nameof(Details), new { id = purchaseOrder.Id });
            }
            ViewBag.Suppliers = new SelectList(await _context.Suppliers.ToListAsync(), "Id", "Name", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // THÊM tham số supplierLotNumber vào đây
        public async Task<IActionResult> AddProductToPurchaseOrder(int purchaseOrderId, int productId, int quantity, decimal unitPrice, string supplierLotNumber)
        {
            if (quantity <= 0 || unitPrice < 0)
            {
                TempData["ErrorMessage"] = "Số lượng và đơn giá phải hợp lệ.";
                return RedirectToAction(nameof(Details), new { id = purchaseOrderId });
            }

            var orderDetail = new PurchaseOrderDetail
            {
                PurchaseOrderId = purchaseOrderId,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                SupplierLotNumber = supplierLotNumber // <-- Gán giá trị mới
            };
            _context.PurchaseOrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = purchaseOrderId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveOrder(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders
                                              .Include(po => po.Supplier)
                                              .Include(po => po.OrderDetails)
                                              // Không cần .ThenInclude(pod => pod.Product) nữa vì service sẽ tự xử lý
                                              .FirstOrDefaultAsync(po => po.Id == id);

            if (purchaseOrder == null || purchaseOrder.Status == PurchaseOrderStatus.Completed)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge(); // Yêu cầu đăng nhập lại nếu mất session
            }

            // BẮT ĐẦU GIAO DỊCH ĐỂ ĐẢM BẢO TÍNH TOÀN VẸN
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Vòng lặp để gọi InventoryService cho mỗi dòng hàng
                foreach (var detail in purchaseOrder.OrderDetails)
                {
                    await _inventoryService.ReceiveFromPurchaseOrderAsync(detail, currentUser.Id, $"PO-{purchaseOrder.Id}");
                }

                // Đánh dấu đơn hàng là đã hoàn thành
                purchaseOrder.Status = PurchaseOrderStatus.Completed;

                // GỌI SERVICE ĐỂ TỰ ĐỘNG HẠCH TOÁN
                await _accountingService.CreateJournalEntryForPurchaseOrder(purchaseOrder);

                // Lưu thay đổi trạng thái của đơn hàng
                await _context.SaveChangesAsync();

                // Nếu tất cả thành công, commit transaction
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Đã nhận hàng, cập nhật kho và hạch toán thành công!";
                return RedirectToAction(nameof(Details), new { id = purchaseOrder.Id });
            }
            catch (Exception ex)
            {
                // Nếu có bất kỳ lỗi nào, rollback tất cả
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Đã có lỗi xảy ra: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = purchaseOrder.Id });
            }
        }
        // GET: PurchaseOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,ExpectedDeliveryDate,SupplierId,Status")] PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseOrderExists(purchaseOrder.Id))
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
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            _context.PurchaseOrders.Remove(purchaseOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: PurchaseOrders/PrintReceipt/5
        public async Task<IActionResult> PrintReceipt(int? id)
        {
            if (id == null) return NotFound();

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.OrderDetails)
                .ThenInclude(pd => pd.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (purchaseOrder == null) return NotFound();

            return new ViewAsPdf("PurchaseOrderReceipt", purchaseOrder)
            {
                FileName = $"PurchaseReceipt-{purchaseOrder.Id}.pdf"
            };
        }
        private bool PurchaseOrderExists(int id)
        {
            return _context.PurchaseOrders.Any(e => e.Id == id);
        }

    }
}
