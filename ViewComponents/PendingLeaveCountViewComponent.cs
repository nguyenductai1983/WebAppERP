using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;

namespace WebAppERP.ViewComponents
{
    public class PendingLeaveCountViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public PendingLeaveCountViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Chỉ đếm nếu người dùng là Admin
            if (User.IsInRole("Admin"))
            {
                var pendingCount = await _context.LeaveRequests
                    .CountAsync(lr => lr.Status == LeaveRequestStatus.Pending && lr.EndDate >= System.DateTime.Today);

                return View(pendingCount);
            }

            // Nếu không phải Admin, trả về 0
            return View(0);
        }
    }
}