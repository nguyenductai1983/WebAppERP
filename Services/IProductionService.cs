// File: Services/IProductionService.cs
using System.Threading.Tasks;
using WebAppERP.ViewModels;

namespace WebAppERP.Services
{
    public interface IProductionService
    {
        /// <summary>
        /// Ghi nhận sản lượng và tiêu hao NVL cho một công đoạn sản xuất một cách linh hoạt.
        /// </summary>
        Task LogProductionAsync(ProductionLogViewModel model);

        /// <summary>
        /// Cập nhật một bản ghi nhận sản lượng đã có.
        /// </summary>
        Task UpdateProductionLogAsync(int logId, ProductionLogViewModel model);

        /// <summary>
        /// Xóa một bản ghi nhận sản lượng và hoàn trả tất cả tồn kho liên quan.
        /// </summary>
        Task DeleteProductionLogAsync(int logId);
    }
}