using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
using WebAppERP.Services;
using System.Collections.Generic;

namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SalesOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AccountingService _accountingService;

        public SalesOrdersController(ApplicationDbContext context, AccountingService accountingService)
        {
            _context = context;
            _accountingService = accountingService;
            ViewData["ActiveMenuGroup"] = "Sales";
        }

        // GET: SalesOrders (Đã nâng cấp: Sắp xếp, Lọc, Phân trang)
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var salesOrders = from s in _context.SalesOrders.Include(s => s.Customer) select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                salesOrders = salesOrders.Where(s => s.Customer.Name.Contains(searchString));
            }

            salesOrders = sortOrder switch
            {
                "date_desc" => salesOrders.OrderByDescending(s => s.OrderDate),
                _ => salesOrders.OrderBy(s => s.OrderDate),
            };

            int pageSize = 10;
            return View(await PaginatedList<SalesOrder>.CreateAsync(salesOrders.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: SalesOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var salesOrder = await _context.SalesOrders
                .Include(s => s.Customer)
                .Include(so => so.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (salesOrder == null) return NotFound();

            // Tải danh sách sản phẩm để bán
            ViewBag.Products = new SelectList(
                _context.Products.Where(p => p.Type == ProductType.FinishedGood), // Không cần kiểm tra tồn kho ở đây nữa
                "Id", "Name"
            );
            ViewBag.Colors = new SelectList(await _context.Colors.OrderBy(c => c.Name).ToListAsync(), "Name", "Name");
            return View(salesOrder);
        }

        // GET: SalesOrders/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Customers = new SelectList(await _context.Customers.OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: SalesOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,ShippingAddress")] SalesOrder salesOrder)
        {
            salesOrder.OrderDate = DateTime.Now;
            salesOrder.Status = OrderStatus.Pending;
            salesOrder.TotalAmount = 0;

            if (ModelState.IsValid)
            {
                _context.Add(salesOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = salesOrder.Id });
            }

            ViewData["CustomerId"] = new SelectList(await _context.Customers.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", salesOrder.CustomerId);
            return View(salesOrder);
        }

        // POST: /SalesOrders/AddProductToOrder (Đã refactor, không trừ kho)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProductToOrder(int salesOrderId, int productId, int quantity, string color, decimal gsmOrGlm, decimal fabricWidth, decimal meterage, int quantityPerBag)
        {
            var salesOrder = await _context.SalesOrders.FindAsync(salesOrderId);
            if (salesOrder == null) return NotFound();

            if (salesOrder.Status != OrderStatus.Pending)
            {
                return Json(new { success = false, message = "Không thể thêm sản phẩm vào đơn hàng đã được xử lý." });
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null || quantity <= 0)
            {
                return Json(new { success = false, message = "Sản phẩm không hợp lệ hoặc số lượng không đúng." });
            }

            var orderDetail = new SalesOrderDetail
            {
                SalesOrderId = salesOrderId,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price,
                Color = color,
                GsmOrGlm = gsmOrGlm,
                FabricWidth = fabricWidth,
                Meterage = meterage,
                QuantityPerBag = quantityPerBag
            };
            _context.SalesOrderDetails.Add(orderDetail);

            salesOrder.TotalAmount += (orderDetail.Quantity * orderDetail.UnitPrice);

            await _context.SaveChangesAsync();

            var updatedOrder = await _context.SalesOrders
                .Include(so => so.OrderDetails).ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.Id == salesOrderId);

            return PartialView("_OrderDetailsTable", updatedOrder);
        }

        // POST: /SalesOrders/DeleteProductFromOrder (Đã refactor, không hoàn kho)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductFromOrder(int salesOrderDetailId, int salesOrderId)
        {
            var salesOrder = await _context.SalesOrders.FindAsync(salesOrderId);
            if (salesOrder == null) return NotFound();

            if (salesOrder.Status != OrderStatus.Pending)
            {
                return BadRequest("Không thể xóa sản phẩm khỏi đơn hàng đã được xử lý.");
            }

            var orderDetail = await _context.SalesOrderDetails
                .Include(od => od.Product)
                .FirstOrDefaultAsync(od => od.Id == salesOrderDetailId);
            if (orderDetail == null) return NotFound();

            salesOrder.TotalAmount -= (orderDetail.Quantity * orderDetail.UnitPrice);

            _context.SalesOrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();

            var updatedOrder = await _context.SalesOrders
                .Include(so => so.OrderDetails).ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(so => so.Id == salesOrderId);

            return PartialView("_OrderDetailsTable", updatedOrder);
        }

        // POST: SalesOrders/CreateShipmentFromOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShipmentFromOrder(int salesOrderId)
        {
            var salesOrder = await _context.SalesOrders
                .Include(so => so.OrderDetails)
                .FirstOrDefaultAsync(so => so.Id == salesOrderId);

            if (salesOrder == null || salesOrder.Status != OrderStatus.Pending)
            {
                TempData["ErrorMessage"] = "Đơn hàng không hợp lệ hoặc đã được xử lý.";
                return RedirectToAction(nameof(Details), new { id = salesOrderId });
            }

            if (await _context.Shipments.AnyAsync(s => s.SalesOrderId == salesOrderId))
            {
                TempData["WarningMessage"] = "Đã tồn tại phiếu xuất kho cho đơn hàng này.";
                var existingShipment = await _context.Shipments.FirstAsync(s => s.SalesOrderId == salesOrderId);
                return RedirectToAction("Details", "Shipments", new { id = existingShipment.Id });
            }

            var shipment = new Shipment
            {
                SalesOrderId = salesOrderId,
                CreationDate = DateTime.Now,
                Status = ShipmentStatus.Pending
            };

            foreach (var detail in salesOrder.OrderDetails)
            {
                shipment.ShipmentDetails.Add(new ShipmentDetail
                {
                    ProductId = detail.ProductId,
                    QuantityToShip = detail.Quantity
                });
            }

            _context.Shipments.Add(shipment);
            salesOrder.Status = OrderStatus.Processing; // Chuyển trạng thái đơn hàng sang "Đang xử lý"
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã tạo phiếu xuất kho thành công. Chuyển đến trang chi tiết phiếu xuất.";
            return RedirectToAction("Details", "Shipments", new { id = shipment.Id });
        }

        // POST: SalesOrders/CompleteOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            var salesOrder = await _context.SalesOrders.Include(s => s.Customer).FirstOrDefaultAsync(s => s.Id == id);

            // Chỉ cho phép hoàn thành đơn hàng đã giao
            if (salesOrder == null || salesOrder.Status != OrderStatus.Shipped)
            {
                TempData["ErrorMessage"] = "Chỉ có thể hoàn thành đơn hàng sau khi đã xuất kho.";
                return RedirectToAction(nameof(Details), new { id });
            }

            salesOrder.Status = OrderStatus.Completed;
            await _accountingService.CreateJournalEntryForSalesOrder(salesOrder);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đơn hàng đã hoàn thành và hạch toán thành công!";
            return RedirectToAction(nameof(Details), new { id = salesOrder.Id });
        }

        // GET: SalesOrders/PrintInvoice/5
        public async Task<IActionResult> PrintInvoice(int? id)
        {
            if (id == null) return NotFound();

            var salesOrder = await _context.SalesOrders
                .Include(s => s.Customer)
                .Include(p => p.OrderDetails)
                .ThenInclude(pd => pd.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (salesOrder == null) return NotFound();

            return new ViewAsPdf("SalesOrderInvoice", salesOrder)
            {
                FileName = $"Invoice-{salesOrder.Id}.pdf"
            };
        }

        private bool SalesOrderExists(int id)
        {
            return _context.SalesOrders.Any(e => e.Id == id);
        }

        // Các action GET/POST cho Edit và Delete có thể giữ lại vì chúng chỉ thao tác
        // trên thông tin chính của đơn hàng, không ảnh hưởng đến kho.
        #region Edit and Delete Actions

        // GET: SalesOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var salesOrder = await _context.SalesOrders.FindAsync(id);
            if (salesOrder == null) return NotFound();
            if (salesOrder.Status != OrderStatus.Pending)
            {
                TempData["ErrorMessage"] = "Không thể sửa đơn hàng đã được xử lý.";
                return RedirectToAction(nameof(Details), new { id });
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", salesOrder.CustomerId);
            return View(salesOrder);
        }

        // POST: SalesOrders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,ShippingAddress,Status,OrderDate,TotalAmount")] SalesOrder salesOrder)
        {
            if (id != salesOrder.Id) return NotFound();
            var orderToUpdate = await _context.SalesOrders.FindAsync(id);
            if (orderToUpdate == null) return NotFound();

            if (await TryUpdateModelAsync<SalesOrder>(
                orderToUpdate,
                "",
                so => so.CustomerId, so => so.ShippingAddress, so => so.Status))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesOrderExists(orderToUpdate.Id)) return NotFound();
                    else throw;
                }
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", salesOrder.CustomerId);
            return View(salesOrder);
        }

        // GET: SalesOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var salesOrder = await _context.SalesOrders
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesOrder == null) return NotFound();
            return View(salesOrder);
        }

        // POST: SalesOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salesOrder = await _context.SalesOrders.FindAsync(id);
            if (salesOrder.Status != OrderStatus.Pending)
            {
                TempData["ErrorMessage"] = "Không thể xóa đơn hàng đã được xử lý.";
                return RedirectToAction(nameof(Index));
            }
            _context.SalesOrders.Remove(salesOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
