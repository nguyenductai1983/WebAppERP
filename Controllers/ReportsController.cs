using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;
namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin, ProductionStaff")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action này sẽ là trang "Trung tâm Ghi nhận Sản lượng"
        public async Task<IActionResult> Index()
        {
            // BƯỚC 2: THAY THẾ LOGIC LỌC DƯỚI ĐÂY
            var loggableStages = await _context.ProductionStages
                // Dòng cũ gây lỗi: .Where(ps => !string.IsNullOrEmpty(ps.LogControllerName))
                .Where(ps => ps.ProcessingMethod != ProductionProcessingMethod.None) // <-- Sửa thành dòng này
                .OrderBy(ps => ps.Sequence)
                .ToListAsync();

            ViewData["ActiveMenuGroup"] = "Operations";

            return View(loggableStages);
        }
    }
}