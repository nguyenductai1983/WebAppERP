// File: Services/ProductionStrategies/IProductionStrategy.cs
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
using WebAppERP.ViewModels; // Thêm using

namespace WebAppERP.Services.ProductionStrategies
{
    public interface IProductionStrategy
    {
        string StageName { get; }
        Task OnOutputCreatedAsync(ApplicationDbContext context, IInventoryService inventoryService, ProductionLog log, ProductionOutputViewModel output);
        //Task OnOutputCreatedAsync(ApplicationDbContext context, ProductionLog log, ProductionOutputViewModel output);
        Task OnLogDeletedAsync(ApplicationDbContext context, ProductionLog log);
    }
}