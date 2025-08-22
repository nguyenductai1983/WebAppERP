// File: Services/ProductionStrategies/FabricProductionStrategy.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
using WebAppERP.ViewModels;

namespace WebAppERP.Services.ProductionStrategies
{
    public class FabricProductionStrategy : IProductionStrategy
    {
        public string StageName => "Dệt";

        // THAY ĐỔI CHỮ KÝ CỦA PHƯƠNG THỨC NÀY
        public async Task OnOutputCreatedAsync(ApplicationDbContext context, IInventoryService inventoryService, ProductionLog log, ProductionOutputViewModel output)
        {
            var attributes = output.Attributes;
            attributes.TryGetValue("Length", out var lengthStr);
            attributes.TryGetValue("Width", out var widthStr);

            // Tạo ra các cây Vải (Textile) mới dựa trên sản lượng
            for (int i = 0; i < output.Quantity; i++)
            {
                // Bước 1: Tạo bản ghi Lô Vải (Textile) với tồn kho bằng 0
                var newTextileLot = new Textile
                {
                    ProductId = log.WorkOrderRouting.WorkOrder.ProductId,
                    OperatorId = log.OperatorId,
                    MachineId = log.MachineId,
                    WorkOrderId = log.WorkOrderRouting.WorkOrderId,
                    InitialLength = decimal.TryParse(lengthStr, out var length) ? length : 0,
                    ActualWidth = decimal.TryParse(widthStr, out var width) ? width : 0,
                    StockQuantity = 0, // <-- Quan trọng: Bắt đầu bằng 0
                    Status = StockItemStatus.InStock,
                    ProductionLogId = log.Id
                };
                context.Textiles.Add(newTextileLot);
                await context.SaveChangesAsync(); // Lưu để có ID

                // Bước 2: Gọi InventoryService để ghi nhận nhập kho
                await inventoryService.ReceiveFromProductionAsync(
                    newTextileLot.ProductId,
                    1, // Mỗi cây vải là 1 đơn vị
                    newTextileLot.ID, // ID của lô vừa tạo
                    log.OperatorId,
                    $"WO-{log.WorkOrderRouting.WorkOrderId}"
                );
            }
        }

        public async Task OnLogDeletedAsync(ApplicationDbContext context, ProductionLog log)
        {
            var textileLots = await context.Textiles
                .Where(t => t.ProductionLogId == log.Id)
                .ToListAsync();

            if (textileLots.Any())
            {
                // Cần logic hoàn trả tồn kho ở đây nếu cần
                context.Textiles.RemoveRange(textileLots);
            }
        }
    }
}