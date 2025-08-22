using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAppERP.Data;
using WebAppERP.Models;
using Microsoft.AspNetCore.Identity; // Thêm using
namespace WebAppERP.Controllers
{
    [Authorize(Roles = "Admin")] // Chỉ Admin được truy cập
    public class LeaveManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveManagementController(ApplicationDbContext context)
        {
            _context = context;
            ViewData["ActiveMenuGroup"] = "HR";
        }

        // GET: LeaveManagement (Danh sách tất cả các đơn)
        public async Task<IActionResult> Index()
        {
            var allRequests = await _context.LeaveRequests
                .Include(lr => lr.RequestingEmployee) // Lấy thông tin nhân viên gửi đơn
                .OrderByDescending(lr => lr.RequestDate)
                .ToListAsync();
            return View(allRequests);
        }

        // POST: LeaveManagement/Approve/5
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest != null)
            {
                leaveRequest.Status = LeaveRequestStatus.Approved;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: LeaveManagement/Deny/5
        [HttpPost]
        public async Task<IActionResult> Deny(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest != null)
            {
                leaveRequest.Status = LeaveRequestStatus.Denied;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        // Thêm 2 Action này vào trong LeaveManagementController

        // GET: LeaveManagement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tải kèm thông tin nhân viên để hiển thị tên
            var leaveRequest = await _context.LeaveRequests
                .Include(lr => lr.RequestingEmployee)
                .FirstOrDefaultAsync(lr => lr.Id == id);

            if (leaveRequest == null)
            {
                return NotFound();
            }
            return View(leaveRequest);
        }

        // POST: LeaveManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,Reason,Status,RequestDate,RequestingEmployeeId")] LeaveRequest leaveRequest)
        {
            if (id != leaveRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.LeaveRequests.Any(e => e.Id == leaveRequest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Đã cập nhật đơn nghỉ phép thành công.";
                return RedirectToAction(nameof(Index));
            }
            return View(leaveRequest);
        }
    }
}
