using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
using WebAppERP.ViewModels;

namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin, ProductionStaff")]
    public class MaterialPlanningController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaterialPlanningController(ApplicationDbContext context)
        {
            _context = context;
            ViewData["ActiveMenuGroup"] = "Production"; // Hoặc "Purchase"
        }

        // Action này sẽ hiển thị báo cáo thiếu hụt
        public async Task<IActionResult> ShortageReport()
        {
            var shortages = await _context.Products
                .Where(p => p.Type == ProductType.RawMaterial && p.Quantity < 0)
                .Select(p => new MaterialShortageViewModel
                {
                    ProductId = p.Id,
                    Sku = p.Sku,
                    ProductName = p.Name,
                    UnitOfMeasure = p.UnitOfMeasure,
                    CurrentStock = p.Quantity
                })
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            return View(shortages);
        }
    }
}