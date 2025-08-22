// File: Services/ProductionService.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
using WebAppERP.ViewModels;
using WebAppERP.Services;
using WebAppERP.Services.ProductionStrategies; // <-- Quan trọng

namespace WebAppERP.Services
{
    public class ProductionService : IProductionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductionStrategyResolver _strategyResolver;
        private readonly IInventoryService _inventoryService; // <-- KHAI BÁO THÊM DÒNG NÀY

        public ProductionService(
           ApplicationDbContext context,
           ProductionStrategyResolver strategyResolver,
           IInventoryService inventoryService) // <-- THÊM THAM SỐ NÀY
        {
            _context = context;
            _strategyResolver = strategyResolver;
            _inventoryService = inventoryService; // <-- KHỞI TẠO NÓ
        }
        public async Task LogProductionAsync(ProductionLogViewModel model)
        {
            // Bắt đầu một giao dịch an toàn để đảm bảo tất cả các thao tác thành công hoặc thất bại cùng nhau
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Lấy thông tin công đoạn (routing) cần thiết
                var routing = await _context.WorkOrderRoutings
                    .Include(r => r.WorkOrder)
                    .Include(r => r.ProductionStage)
                    .FirstOrDefaultAsync(r => r.Id == model.WorkOrderRoutingId);

                if (routing == null)
                {
                    throw new InvalidOperationException("Công đoạn sản xuất không hợp lệ.");
                }

                // Tính tổng sản lượng đầu ra từ tất cả các dòng output
                var totalOutputQuantity = model.Outputs.Sum(o => o.Quantity);
                if (totalOutputQuantity <= 0)
                {
                    throw new InvalidOperationException("Sản lượng đầu ra phải lớn hơn 0.");
                }

                // 2. Tạo một bản ghi ProductionLog chung cho toàn bộ sự kiện
                var mainLog = new ProductionLog
                {
                    WorkOrderRoutingId = model.WorkOrderRoutingId,
                    Quantity = totalOutputQuantity,
                    OperatorId = model.OperatorId,
                    MachineId = model.MachineId,
                    Notes = model.Notes,
                    LogDate = DateTime.Now
                };
                _context.Add(mainLog);

                // 3. Xử lý tạo các sản phẩm ĐẦU RA (dùng Strategy Pattern)
                var outputStrategy = _strategyResolver.GetStrategy(routing.ProductionStage.Name);
                if (outputStrategy != null)
                {
                    foreach (var output in model.Outputs.Where(o => o.Quantity > 0))
                    {
                        // Giao việc tạo sản phẩm (Yarn, Textile...) cho chiến lược tương ứng
                        await outputStrategy.OnOutputCreatedAsync(_context, _inventoryService, mainLog, output);
                    }
                }


                var currentUser = await _context.Users.FindAsync(model.OperatorId); // Lấy thông tin user

                // 4. Xử lý tiêu hao NVL ĐẦU VÀO
                foreach (var input in model.Inputs.Where(i => i.QuantityToConsume > 0))
                {
                    var bomItem = await _context.WorkOrderBOMs
                        .Include(b => b.Component)
                        .FirstOrDefaultAsync(b => b.Id == input.WorkOrderBOMId);
                    if (bomItem == null) throw new InvalidOperationException($"Không tìm thấy yêu cầu NVL #{input.WorkOrderBOMId}.");

                    // Cập nhật số lượng đã tiêu hao trên WorkOrderBOM
                    bomItem.ConsumedQuantity += input.QuantityToConsume;

                    // TẠO BẢN GHI TIÊU THỤ (phần này vẫn giữ)
                    var consumptionLog = new MaterialConsumptionLog
                    {
                        ProductionLog = mainLog,
                        WorkOrderBOMId = input.WorkOrderBOMId,
                        ConsumedLotId = input.SelectedLotId,
                        QuantityConsumed = input.QuantityToConsume
                    };
                    _context.MaterialConsumptionLogs.Add(consumptionLog);

                    // =================================================================
                    // ==> THAY THẾ LOGIC TRỪ KHO CŨ BẰNG LỆNH GỌI SERVICE <==
                    await _inventoryService.IssueForProductionAsync(
                        bomItem.ComponentId,
                        input.QuantityToConsume,
                        input.SelectedLotId,
                        currentUser.Id,
                        $"WO-{routing.WorkOrderId}"
                    );
                    // =================================================================
                }

                // 5. Cập nhật tiến độ của WorkOrderRouting
                await UpdateRoutingProgressAsync(routing.Id, totalOutputQuantity);

                // 6. Lưu tất cả thay đổi vào database
                await _context.SaveChangesAsync();
                // 7. Hoàn tất giao dịch
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // Nếu có bất kỳ lỗi nào, hủy bỏ tất cả thay đổi
                await transaction.RollbackAsync();
                throw; // Ném lỗi ra để Controller có thể bắt và hiển thị
            }
        }
        public async Task DeleteProductionLogAsync(int logId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var productionLog = await _context.ProductionLogs
                    .Include(p => p.WorkOrderRouting).ThenInclude(r => r.ProductionStage)
                    .FirstOrDefaultAsync(p => p.Id == logId);

                if (productionLog == null) throw new InvalidOperationException("Không tìm thấy bản ghi.");

                // 1. Hoàn trả tiến độ sản xuất
                await UpdateRoutingProgressAsync(productionLog.WorkOrderRoutingId, -productionLog.Quantity);

                // 2. Hoàn trả NVL đã tiêu thụ
                var consumptions = await _context.MaterialConsumptionLogs
                    .Include(c => c.WorkOrderBOM).ThenInclude(b => b.Component)
                    .Where(c => c.ProductionLogId == logId)
                    .ToListAsync();

                foreach (var cons in consumptions)
                {
                    // 2a. Cập nhật lại số lượng đã tiêu hao trên BOM
                    cons.WorkOrderBOM.ConsumedQuantity -= cons.QuantityConsumed;

                    // 2b. Hoàn trả tồn kho
                    if (cons.WorkOrderBOM.Component.Type == ProductType.RawMaterial)
                    {
                        var productToUpdate = await _context.Products.FindAsync(cons.WorkOrderBOM.ComponentId);
                        productToUpdate.Quantity += cons.QuantityConsumed;
                    }
                    else // Bán thành phẩm
                    {
                        if (cons.ConsumedLotId.HasValue)
                        {
                            if (cons.WorkOrderBOM.Component.Name.Contains("Sợi"))
                            {
                                var yarnLot = await _context.Yarns.FindAsync(cons.ConsumedLotId.Value);
                                yarnLot.StockQuantity += cons.QuantityConsumed;
                                yarnLot.Status = StockItemStatus.InStock;
                            }
                            // else if (cons.WorkOrderBOM.Component.Name.Contains("Vải")) { ... }
                        }
                    }
                }
                _context.MaterialConsumptionLogs.RemoveRange(consumptions);

                // 3. Xóa sản phẩm đầu ra đã được tạo bởi log này (dùng Strategy)
                var outputStrategy = _strategyResolver.GetStrategy(productionLog.WorkOrderRouting.ProductionStage.Name);
                if (outputStrategy != null)
                {
                    await outputStrategy.OnLogDeletedAsync(_context, productionLog);
                }

                // 4. Xóa bản ghi Log
                _context.ProductionLogs.Remove(productionLog);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public Task UpdateProductionLogAsync(int logId, ProductionLogViewModel model)
        {
            // Logic cập nhật sẽ phức tạp, về cơ bản là một nghiệp vụ Delete và sau đó là Create mới
            // Tạm thời để trống
            throw new NotImplementedException();
        }

        // Phương thức helper để cập nhật tiến độ, giờ không cần cập nhật tồn kho nữa
        private async Task UpdateRoutingProgressAsync(int routingId, int quantityChange)
        {
            var routing = await _context.WorkOrderRoutings.FindAsync(routingId);
            if (routing == null) return;

            routing.QuantityProduced += quantityChange;

            if (routing.QuantityProduced >= routing.QuantityToProduce)
            {
                routing.Status = RoutingStatus.Completed;
            }
            else if (routing.QuantityProduced > 0)
            {
                routing.Status = RoutingStatus.InProgress;
            }
            else
            {
                routing.Status = RoutingStatus.NotStarted;
            }
        }
    }
}