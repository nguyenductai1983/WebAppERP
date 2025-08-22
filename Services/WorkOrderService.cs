using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;

namespace WebAppERP.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly ApplicationDbContext _context;

        public WorkOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ... (Các phương thức CreateMasterWorkOrderAsync, ReleaseWorkOrderAsync, etc. giữ nguyên) ...
        #region Other Methods
        public async Task<WorkOrder> CreateMasterWorkOrderAsync(WorkOrder workOrder)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Gán giá trị mặc định và lưu LSX Tổng để có ID
                workOrder.Type = WorkOrderType.Master;
                workOrder.Status = WorkOrderStatus.New;
                workOrder.CreationDate = DateTime.Now;
                _context.Add(workOrder);
                await _context.SaveChangesAsync();

                // 2. Tìm tất cả công đoạn cần thiết dựa trên cây BOM đệ quy
                var allRequiredStageIds = new HashSet<int>();
                await FindAllRequiredStagesRecursiveAsync(workOrder.ProductId, allRequiredStageIds);

                var productionStages = await _context.ProductionStages
                    .Where(ps => allRequiredStageIds.Contains(ps.Id))
                    .OrderBy(ps => ps.Sequence)
                    .ToListAsync();

                // 3. Với mỗi công đoạn tìm được, tạo một bản ghi WorkOrderRouting
                foreach (var stage in productionStages)
                {
                    var routing = new WorkOrderRouting
                    {
                        WorkOrderId = workOrder.Id,
                        ProductionStageId = stage.Id,
                        QuantityToProduce = workOrder.QuantityToProduce,
                        Status = RoutingStatus.NotStarted
                    };
                    _context.WorkOrderRoutings.Add(routing);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return workOrder;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task ReleaseWorkOrderAsync(int masterWorkOrderId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var masterWorkOrder = await _context.WorkOrders
                    .Include(wo => wo.Product)
                    .FirstOrDefaultAsync(wo => wo.Id == masterWorkOrderId);

                if (masterWorkOrder == null || masterWorkOrder.Type != WorkOrderType.Master || masterWorkOrder.Status != WorkOrderStatus.New)
                {
                    throw new InvalidOperationException("Lệnh sản xuất không hợp lệ hoặc đã được ban hành.");
                }

                await CreateWorkOrderBOMsAsync(masterWorkOrder);
                await GenerateAndSaveChildrenAsync(masterWorkOrder);
                masterWorkOrder.Status = WorkOrderStatus.InProgress;
                await _context.SaveChangesAsync();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public Task StartProductionAsync(int workOrderId) { throw new NotImplementedException(); }
        public Task CompleteProductionAsync(int workOrderId) { throw new NotImplementedException(); }

        private async Task GenerateAndSaveChildrenAsync(WorkOrder parentWorkOrder)
        {
            var bomItems = await _context.BillOfMaterials
                .Include(b => b.Component)
                .Where(b => b.FinishedProductId == parentWorkOrder.ProductId && b.Component.Type == ProductType.SemiFinishedGood)
                .ToListAsync();

            foreach (var bomItem in bomItems)
            {
                decimal quantityNeeded = bomItem.Quantity * parentWorkOrder.QuantityToProduce;
                decimal physicalStock = bomItem.Component.Quantity;
                decimal plannedSupply = await _context.WorkOrders
                    .Where(wo => wo.ProductId == bomItem.ComponentId && wo.Status != WorkOrderStatus.Completed && wo.Status != WorkOrderStatus.Cancelled)
                    .SumAsync(wo => (decimal?)wo.QuantityToProduce) ?? 0;
                decimal totalAvailable = physicalStock + plannedSupply;

                if (totalAvailable < quantityNeeded)
                {
                    decimal quantityToProduce = quantityNeeded - totalAvailable;
                    var subWorkOrder = new WorkOrder
                    {
                        ProductId = bomItem.ComponentId,
                        QuantityToProduce = (int)Math.Ceiling(quantityToProduce),
                        Status = WorkOrderStatus.New,
                        Type = WorkOrderType.SubAssembly,
                        CreationDate = DateTime.Now,
                        DueDate = parentWorkOrder.CreationDate.AddDays(-1),
                        Description = $"[BTP: {bomItem.Component.Name}] -> {parentWorkOrder.Description}",
                        ParentWorkOrderId = parentWorkOrder.Id,
                        ProductionStageId = bomItem.Component.DefaultProductionStageId
                    };

                    if (!subWorkOrder.ProductionStageId.HasValue)
                    {
                        throw new InvalidOperationException($"Sản phẩm Bán thành phẩm '{bomItem.Component.Name}' chưa được thiết lập 'Công đoạn sản xuất chính'.");
                    }

                    _context.WorkOrders.Add(subWorkOrder);
                    await _context.SaveChangesAsync();

                    await CreateWorkOrderBOMsAsync(subWorkOrder);
                    var subRouting = new WorkOrderRouting
                    {
                        WorkOrderId = subWorkOrder.Id,
                        ProductionStageId = subWorkOrder.ProductionStageId.Value,
                        QuantityToProduce = subWorkOrder.QuantityToProduce,
                        Status = RoutingStatus.NotStarted
                    };
                    _context.WorkOrderRoutings.Add(subRouting);

                    await StartSingleWorkOrderAsync(subWorkOrder);
                    await _context.SaveChangesAsync();

                    await GenerateAndSaveChildrenAsync(subWorkOrder);
                }
            }
        }

        private async Task StartSingleWorkOrderAsync(WorkOrder workOrder)
        {
            var bomForCosting = await _context.WorkOrderBOMs
                .Where(b => b.WorkOrderId == workOrder.Id)
                .Include(b => b.Component)
                .ToListAsync();

            decimal totalMaterialCost = 0;
            foreach (var item in bomForCosting.Where(i => i.Component.Type == ProductType.RawMaterial))
            {
                var requiredQuantity = item.RequiredQuantity;
                totalMaterialCost += requiredQuantity * item.Component.Cost;
                var rawMaterialProduct = await _context.Products.FindAsync(item.ComponentId);
                if (rawMaterialProduct != null)
                {
                    rawMaterialProduct.Quantity -= requiredQuantity;
                }
            }
            workOrder.ActualMaterialCost = totalMaterialCost;
            workOrder.Status = WorkOrderStatus.InProgress;
        }

        private async Task CreateWorkOrderBOMsAsync(WorkOrder workOrder)
        {
            var originalBOMs = await _context.BillOfMaterials
                .Where(b => b.FinishedProductId == workOrder.ProductId)
                .ToListAsync();

            if (originalBOMs.Any())
            {
                foreach (var bomItem in originalBOMs)
                {
                    var workOrderBOM = new WorkOrderBOM
                    {
                        WorkOrderId = workOrder.Id,
                        ComponentId = bomItem.ComponentId,
                        RequiredQuantity = bomItem.Quantity * workOrder.QuantityToProduce,
                        ConsumedQuantity = 0
                    };
                    _context.WorkOrderBOMs.Add(workOrderBOM);
                }
            }
        }

        private async Task FindAllRequiredStagesRecursiveAsync(int productId, HashSet<int> collectedStageIds)
        {
            var bomItems = await _context.BillOfMaterials
                .Include(b => b.Component)
                .Where(b => b.FinishedProductId == productId)
                .ToListAsync();

            if (!bomItems.Any()) return;

            foreach (var item in bomItems)
            {
                collectedStageIds.Add(item.ProductionStageId);
                if (item.Component.Type == ProductType.SemiFinishedGood)
                {
                    await FindAllRequiredStagesRecursiveAsync(item.ComponentId, collectedStageIds);
                }
            }
        }

        private async Task<List<WorkOrder>> GetAllChildWorkOrdersRecursive(int parentId)
        {
            var children = await _context.WorkOrders
                .Where(wo => wo.ParentWorkOrderId == parentId)
                .ToListAsync();

            var allDescendants = new List<WorkOrder>(children);
            foreach (var child in children)
            {
                allDescendants.AddRange(await GetAllChildWorkOrdersRecursive(child.Id));
            }
            return allDescendants;
        }
        #endregion

        // =================================================================
        // ==> BẮT ĐẦU PHẦN CODE MỚI ĐỂ SỬA LỖI <==
        // =================================================================
        public async Task DeleteWorkOrderRecursiveAsync(int workOrderId)
        {
            var workOrdersToDelete = new List<WorkOrder>();
            await CollectWorkOrdersToDeleteAsync(workOrderId, workOrdersToDelete);

            if (!workOrdersToDelete.Any()) return;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Bước 1: Kiểm tra tất cả các quy tắc nghiệp vụ trước khi xóa bất cứ thứ gì.
                foreach (var wo in workOrdersToDelete)
                {
                    if (wo.Status != WorkOrderStatus.New)
                    {
                        throw new InvalidOperationException($"Không thể xóa Lệnh sản xuất #{wo.Id} vì đã được xử lý (trạng thái: {wo.Status}).");
                    }

                    if (await _context.MaterialIssues.AnyAsync(i => i.WorkOrderId == wo.Id))
                    {
                        throw new InvalidOperationException($"Không thể xóa LSX #{wo.Id} vì đã có Phiếu xuất kho liên quan.");
                    }

                    if (await _context.Yarns.AnyAsync(y => y.WorkOrderId == wo.Id) || await _context.Textiles.AnyAsync(t => t.WorkOrderId == wo.Id))
                    {
                        throw new InvalidOperationException($"Không thể xóa LSX #{wo.Id} vì đã có Bán thành phẩm (Sợi/Vải) được sản xuất từ lệnh này.");
                    }
                }

                // Bước 2: Nếu tất cả kiểm tra đều qua, tiến hành xóa.
                // EF Core sẽ xử lý thứ tự xóa dựa trên các mối quan hệ.
                // Cấu hình Cascade delete cho WorkOrderBOMs và WorkOrderRoutings sẽ tự động xóa chúng.
                _context.WorkOrders.RemoveRange(workOrdersToDelete);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // Ném lỗi ra để Controller bắt lại
            }
        }

        private async Task CollectWorkOrdersToDeleteAsync(int workOrderId, List<WorkOrder> list)
        {
            var workOrder = await _context.WorkOrders.FindAsync(workOrderId);
            if (workOrder != null)
            {
                // Tìm các con trước
                var childIds = await _context.WorkOrders
                    .Where(wo => wo.ParentWorkOrderId == workOrderId)
                    .Select(wo => wo.Id)
                    .ToListAsync();

                foreach (var childId in childIds)
                {
                    await CollectWorkOrdersToDeleteAsync(childId, list);
                }

                // Thêm cha vào danh sách sau khi đã thêm tất cả các con
                list.Add(workOrder);
            }
        }
        // =================================================================
        // ==> KẾT THÚC PHẦN CODE MỚI <==
        // =================================================================
    }
}
