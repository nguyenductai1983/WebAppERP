using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;

namespace WebAppERP.Controllers
{
    // Dùng abstract để class này không thể tự tạo ra mà chỉ có thể được kế thừa
    public abstract class BaseProductionLogController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public BaseProductionLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // PHƯƠNG THỨC DÙNG CHUNG ĐỂ CẬP NHẬT TIẾN ĐỘ VÀ TỒN KHO
        // PHƯƠNG THỨC DÙNG CHUNG ĐÃ ĐƯỢC NÂNG CẤP
        protected async Task UpdateRoutingAndInventoryAsync(int routingId, int quantityChange)
        {
            var routing = await _context.WorkOrderRoutings.FindAsync(routingId);
            if (routing == null) return;

            var workOrder = await _context.WorkOrders
                .Include(wo => wo.Product)
                .FirstOrDefaultAsync(wo => wo.Id == routing.WorkOrderId);
            if (workOrder == null) return;

            // 1. Cập nhật tồn kho: Cộng hoặc trừ lượng chênh lệch
            workOrder.Product.Quantity += quantityChange;

            // 2. Cập nhật tiến độ công đoạn
            routing.QuantityProduced += quantityChange;

            // 3. Cập nhật trạng thái công đoạn
            if (routing.QuantityProduced >= routing.QuantityToProduce)
            {
                routing.Status = RoutingStatus.Completed;
            }
            else if (routing.QuantityProduced > 0)
            {
                routing.Status = RoutingStatus.InProgress;
            }
            else // Nếu sản lượng trở về 0
            {
                routing.Status = RoutingStatus.NotStarted;
            }
        }
    }
}