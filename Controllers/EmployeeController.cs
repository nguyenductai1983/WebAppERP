using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppERP.Data;
using WebAppERP.Models;
using Microsoft.AspNetCore.Identity; // Thêm using
//using Microsoft.EntityFrameworkCore; // Thêm using
//using System.Linq; // Thêm using
namespace WebAppERP
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; // <-- Tiêm UserManager
        public EmployeesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager; // <-- Khởi tạo
            ViewData["ActiveMenuGroup"] = "HR";
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees.Include(n => n.User);            
            return View(await applicationDbContext.ToListAsync());
            //return View(await _context.Employees.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Thêm .Include(n => n.User) để lấy thông tin của User liên quan
            var Employee = await _context.Employees
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Employee == null)
            {
                return NotFound();
            }

            return View(Employee);
        }
        public async Task<IActionResult> Create()
        {
            // Lấy danh sách ID của các user đã được gán cho nhân viên
            var assignedUserIds = await _context.Employees
                                                .Where(e => e.UserId != null)
                                                .Select(e => e.UserId)
                                                .ToListAsync();

            // Lấy danh sách các user chưa được gán
            var availableUsers = await _userManager.Users
                                                   .Where(u => !assignedUserIds.Contains(u.Id))
                                                   .ToListAsync();

            // Gửi danh sách user sang View
            ViewBag.AvailableUsers = availableUsers;

            return View();
        }
        // GET: Employees/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Position,Salary,Address,CitizenId,HireDate,Department,UserId")] Employee Employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Employee);
        }

        // GET: Employees/Edit/5        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Employee = await _context.Employees.FindAsync(id);
            if (Employee == null)
            {
                return NotFound();
            }

            // --- BỔ SUNG LOGIC LẤY DANH SÁCH USER ---

            // Lấy ID của các user đã được gán cho những nhân viên KHÁC
            var assignedUserIds = await _context.Employees
                                                .Where(e => e.UserId != null && e.Id != id) // Quan trọng: loại trừ nhân viên hiện tại
                                                .Select(e => e.UserId)
                                                .ToListAsync();

            // Lấy danh sách user có sẵn (bao gồm cả user hiện tại của nhân viên này)
            var availableUsers = await _userManager.Users
                                                   .Where(u => !assignedUserIds.Contains(u.Id))
                                                   .ToListAsync();

            ViewBag.AvailableUsers = availableUsers;

            return View(Employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Position,Salary,Address,CitizenId,HireDate,Department,UserId")] Employee Employee)
        {
            if (id != Employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(Employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(Employee);
        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Employee == null)
            {
                return NotFound();
            }

            return View(Employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Employee = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(Employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
