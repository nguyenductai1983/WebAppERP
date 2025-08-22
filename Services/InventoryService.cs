// File: Services/InventoryService.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;

namespace WebAppERP.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Public Methods

        public async Task ReceiveFromPurchaseOrderAsync(PurchaseOrderDetail detail, string userId, string reference)
        {
            await CreateTransactionAsync(
                detail.ProductId,
                detail.Quantity,
                null,
                TransactionType.PurchaseReceipt,
                userId,
                reference
            );
        }

        public async Task IssueForProductionAsync(int productId, decimal quantity, int? lotId, string userId, string reference)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Số lượng xuất kho phải lớn hơn 0.");
            }
            await CreateTransactionAsync(
                productId,
                -quantity,
                lotId,
                TransactionType.IssueForProduction,
                userId,
                reference
            );
        }

        public async Task ReceiveFromProductionAsync(int productId, decimal quantity, int? lotId, string userId, string reference)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Số lượng nhập kho phải lớn hơn 0.");
            }
            await CreateTransactionAsync(
                productId,
                quantity,
                lotId,
                TransactionType.ReceiptFromProduction,
                userId,
                reference
            );
        }

        public async Task ShipForSalesOrderAsync(SalesOrderDetail detail, string userId, string reference)
        {
            await CreateTransactionAsync(
                detail.ProductId,
                -detail.Quantity,
                null,
                TransactionType.SalesShipment,
                userId,
                reference
            );
        }

        public async Task AdjustStockAsync(int productId, decimal newQuantity, string userId, string reason)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException($"Không tìm thấy sản phẩm với ID {productId}.");
            }

            decimal quantityChange = newQuantity - product.Quantity;
            if (quantityChange == 0) return;

            var transactionType = quantityChange > 0
                ? TransactionType.AdjustmentIncrease
                : TransactionType.AdjustmentDecrease;

            await CreateTransactionAsync(
                productId,
                quantityChange,
                null,
                transactionType,
                userId,
                reason
            );
        }

        #endregion

        #region Private Core Method

        // =================================================================
        // ==> THAY ĐỔI CHÍNH: GỠ BỎ "using var dbTransaction..." <==
        // =================================================================
        private async Task CreateTransactionAsync(int productId, decimal quantityChange, int? lotId, TransactionType transactionType, string userId, string reference)
        {
            // Không cần bắt đầu transaction mới ở đây nữa
            // vì phương thức này sẽ tham gia vào transaction do Controller khởi tạo.
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Không tìm thấy sản phẩm với ID {productId}.");
                }

                if (product.Quantity + quantityChange < 0)
                {
                    throw new InvalidOperationException($"Không đủ tồn kho cho sản phẩm '{product.Name}'. Tồn kho hiện tại: {product.Quantity}, cần xuất: {-quantityChange}.");
                }

                product.Quantity += quantityChange;

                var transaction = new InventoryTransaction
                {
                    ProductId = productId,
                    Type = transactionType,
                    QuantityChange = quantityChange,
                    QuantityAfterTransaction = product.Quantity,
                    LotId = lotId,
                    TransactionDate = DateTime.Now,
                    UserId = userId,
                    Reference = reference
                };

                _context.InventoryTransactions.Add(transaction);

                // Lưu ý: SaveChangesAsync() ở đây sẽ không commit transaction ngay
                // mà chỉ ghi các thay đổi vào transaction đang hoạt động của Controller.
                // Transaction chỉ được commit khi Controller gọi transaction.CommitAsync().
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Không cần rollback ở đây, Controller sẽ xử lý việc đó.
                throw;
            }
        }
        // =================================================================

        #endregion
    }
}
