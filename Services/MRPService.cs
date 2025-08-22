// File: Services/MRPService.cs
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
using WebAppERP.ViewModels;

namespace WebAppERP.Services
{
    public class MRPService : IMRPService
    {
        private readonly ApplicationDbContext _context;

        public MRPService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MaterialRequirementViewModel>> CalculateRequirementsAsync()
        {
            // Bước 1: Tính toán tổng nhu cầu còn lại từ các Lệnh sản xuất đang hoạt động.
            // Nhóm theo ID của Nguyên vật liệu.
            var demandQuery = _context.WorkOrderBOMs
                .Where(b => (b.WorkOrder.Status == WorkOrderStatus.InProgress || b.WorkOrder.Status == WorkOrderStatus.New)
                            && b.Component.Type == ProductType.RawMaterial)
                .GroupBy(b => b.ComponentId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    // Nhu cầu còn lại = Tổng yêu cầu - Lượng đã xuất dùng
                    TotalDemand = g.Sum(b => b.RequiredQuantity - b.ConsumedQuantity)
                });

            // Chuyển kết quả về dạng Dictionary để tra cứu nhanh
            var demandDictionary = await demandQuery
                .Where(d => d.TotalDemand > 0) // Chỉ lấy các NVL có nhu cầu thực sự
                .ToDictionaryAsync(d => d.ProductId, d => d.TotalDemand);

            // Nếu không có nhu cầu nào, trả về danh sách rỗng
            if (!demandDictionary.Any())
            {
                return new List<MaterialRequirementViewModel>();
            }

            var neededProductIds = demandDictionary.Keys.ToList();

            // Bước 2: Lấy thông tin chi tiết (bao gồm tồn kho) của các NVL cần thiết.
            var productDetails = await _context.Products
                .Where(p => neededProductIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            // Bước 3: Lấy thông tin hàng đang trên đường về từ các Đơn mua hàng chưa hoàn thành.
            var onOrderQuery = _context.PurchaseOrderDetails
                .Where(d => d.PurchaseOrder.Status != PurchaseOrderStatus.Completed && neededProductIds.Contains(d.ProductId))
                .GroupBy(d => d.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalOnOrder = g.Sum(d => d.Quantity)
                });

            var onOrderDictionary = await onOrderQuery.ToDictionaryAsync(o => o.ProductId, o => o.TotalOnOrder);

            // Bước 4: Kết hợp tất cả dữ liệu trong bộ nhớ để tính toán kết quả cuối cùng.
            var viewModelList = new List<MaterialRequirementViewModel>();

            foreach (var productId in neededProductIds)
            {
                var product = productDetails[productId];
                var demand = demandDictionary[productId];
                onOrderDictionary.TryGetValue(productId, out int onOrderQty);

                var shortage = demand - (product.Quantity + onOrderQty);

                viewModelList.Add(new MaterialRequirementViewModel
                {
                    ProductId = product.Id,
                    ProductSku = product.Sku,
                    ProductName = product.Name,
                    UnitOfMeasure = product.UnitOfMeasure,
                    TotalRequiredQuantity = demand,
                    CurrentStockQuantity = product.Quantity,
                    OnOrderQuantity = onOrderQty,
                    ShortageQuantity = shortage,
                    SuggestedPurchaseQuantity = shortage > 0 ? shortage : 0
                });
            }

            return viewModelList.OrderBy(v => v.ProductName).ToList();
        }
    }
}
