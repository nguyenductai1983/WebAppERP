// File: Services/IInventoryService.cs
using System.Threading.Tasks;
using WebAppERP.Models;

namespace WebAppERP.Services
{
    public interface IInventoryService
    {
        // Nhập kho từ đơn mua hàng
        Task ReceiveFromPurchaseOrderAsync(PurchaseOrderDetail detail, string userId, string reference);

        // Xuất kho cho sản xuất (NVL & BTP)
        Task IssueForProductionAsync(int productId, decimal quantity, int? lotId, string userId, string reference);

        // Nhập kho từ sản xuất (BTP & TP)
        Task ReceiveFromProductionAsync(int productId, decimal quantity, int? lotId, string userId, string reference);

        // Xuất kho bán hàng
        Task ShipForSalesOrderAsync(SalesOrderDetail detail, string userId, string reference);

        // Điều chỉnh tồn kho thủ công
        Task AdjustStockAsync(int productId, decimal newQuantity, string userId, string reason);
    }
}