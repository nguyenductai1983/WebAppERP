// File: Services/ProductionStrategies/YarnProductionStrategy.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
using WebAppERP.ViewModels;
using WebAppERP.ViewModels.ProductionAttributes; // <-- Quan trọng

namespace WebAppERP.Services.ProductionStrategies
{
    public class YarnProductionStrategy : IProductionStrategy
    {
        public string StageName => "Đùn Sợi";
        // File: Services/ProductionStrategies/YarnProductionStrategy.cs
        public async Task OnOutputCreatedAsync(ApplicationDbContext context, IInventoryService inventoryService, ProductionLog log, ProductionOutputViewModel output)
        {
            var attributes = output.Attributes;
            attributes.TryGetValue("NetWeight", out var netWeightStr);
            attributes.TryGetValue("GrossWeight", out var grossWeightStr);
            attributes.TryGetValue("SpoolCount", out var spoolCountStr);

            decimal netWeight = decimal.TryParse(netWeightStr, out var n) ? n : 0;

            // Bước 1: Tạo bản ghi Lô Sợi (Yarn) nhưng chưa có tồn kho
            var newYarnLot = new Yarn
            {
                ProductId = log.WorkOrderRouting.WorkOrder.ProductId,
                OperatorId = log.OperatorId,
                MachineId = log.MachineId,
                WorkOrderId = log.WorkOrderRouting.WorkOrderId,
                NetWeight = netWeight,
                GrossWeight = decimal.TryParse(grossWeightStr, out var g) ? g : 0,
                SpoolCount = int.TryParse(spoolCountStr, out var s) ? s : 0,
                StockQuantity = 0, // <-- Quan trọng: Bắt đầu bằng 0
                Status = StockItemStatus.InStock,
                ProductionLogId = log.Id
            };

            context.Yarns.Add(newYarnLot);
            // Lưu lại để newYarnLot có ID
            await context.SaveChangesAsync();

            // Bước 2: Gọi InventoryService để ghi nhận nhập kho
            // Service sẽ tự động cập nhật StockQuantity và tạo InventoryTransaction
            await inventoryService.ReceiveFromProductionAsync(
                newYarnLot.ProductId,
                netWeight, // Số lượng nhập kho chính là NetWeight
                newYarnLot.ID, // ID của lô vừa tạo
                log.OperatorId,
                $"WO-{log.WorkOrderRouting.WorkOrderId}"
            );
        }

        public async Task OnLogCreatedAsync(ApplicationDbContext context, ProductionLog log, ProductionLogViewModel viewModel)
        {
            // 1. DESERIALIZE JSON ĐỂ LẤY DỮ LIỆU ĐẶC THÙ
            if (string.IsNullOrEmpty(log.AttributesJson))
                throw new InvalidOperationException("Thiếu thông tin chi tiết (NetWeight, SpoolCount...) để tạo lô Sợi.");

            var attributes = JsonSerializer.Deserialize<YarnLogAttributes>(log.AttributesJson);

            // 2. SỬ DỤNG DỮ LIỆU ĐÃ DESERIALIZE ĐỂ TẠO LÔ SỢI
            var newYarnLot = new Yarn
            {
                ProductId = log.WorkOrderRouting.WorkOrder.ProductId,
                OperatorId = viewModel.OperatorId,
                MachineId = viewModel.MachineId,
                WorkOrderId = log.WorkOrderRouting.WorkOrderId,
                SpoolCount = attributes.SpoolCount,
                GrossWeight = attributes.GrossWeight,
                NetWeight = attributes.NetWeight,
                StockQuantity = attributes.NetWeight, // <== SỬA LỖI Ở ĐÂY
                Status = StockItemStatus.InStock,
                Created_at = DateTime.Now,
                ProductionLogId = log.Id
            };
            context.Yarns.Add(newYarnLot);

            // Trả về Task hoàn thành
            await Task.CompletedTask;
        }

        public async Task OnLogDeletedAsync(ApplicationDbContext context, ProductionLog log)
        {
            var yarnLot = await context.Yarns.FirstOrDefaultAsync(y => y.ProductionLogId == log.Id);
            if (yarnLot != null)
            {
                context.Yarns.Remove(yarnLot);
            }
        }
    }
}