// Services/IWorkOrderService.cs
using System.Threading.Tasks;
using WebAppERP.Models;

namespace WebAppERP.Services
{
    public interface IWorkOrderService
    {
        Task<WorkOrder> CreateMasterWorkOrderAsync(WorkOrder workOrder);
        // Chuyển signature của các action phức tạp từ controller sang đây
        Task ReleaseWorkOrderAsync(int workOrderId);
        Task StartProductionAsync(int workOrderId);
        Task CompleteProductionAsync(int workOrderId);
        Task DeleteWorkOrderRecursiveAsync(int workOrderId);
    }
}