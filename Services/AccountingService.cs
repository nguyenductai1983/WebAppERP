using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAppERP.Data;
using WebAppERP.Models;

namespace WebAppERP.Services
{
    public class AccountingService
    {
        private readonly ApplicationDbContext _context;

        public AccountingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateJournalEntryForSalesOrder(SalesOrder order)
        {
            // Tìm các tài khoản đã định nghĩa trước
            var accountsReceivable = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == "131");
            var salesRevenueAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == "511");

            if (accountsReceivable == null || salesRevenueAccount == null)
            {
                // Nếu không tìm thấy tài khoản, không làm gì cả (hoặc ghi log lỗi)
                return;
            }

            // 1. Tạo bút toán chung (Journal Entry)
            var journalEntry = new JournalEntry
            {
                EntryDate = DateTime.Now,
                Description = $"Ghi nhận doanh thu cho đơn hàng #{order.Id} - Khách hàng: {order.Customer.Name}"
            };
            _context.JournalEntries.Add(journalEntry);
            await _context.SaveChangesAsync(); // Lưu để lấy Id

            // 2. Tạo dòng Nợ (Debit) cho tài khoản Phải thu
            var debitLine = new JournalEntryLine
            {
                JournalEntryId = journalEntry.Id,
                AccountId = accountsReceivable.Id,
                DebitOrCredit = DebitOrCredit.Debit,
                Amount = order.TotalAmount
            };
            _context.JournalEntryLines.Add(debitLine);

            // 3. Tạo dòng Có (Credit) cho tài khoản Doanh thu
            var creditLine = new JournalEntryLine
            {
                JournalEntryId = journalEntry.Id,
                AccountId = salesRevenueAccount.Id,
                DebitOrCredit = DebitOrCredit.Credit,
                Amount = order.TotalAmount
            };
            _context.JournalEntryLines.Add(creditLine);

            // 4. Cập nhật số dư các tài khoản
            accountsReceivable.Balance += order.TotalAmount;
            // Theo nguyên tắc kế toán, tài khoản doanh thu có kết cấu bên Có (Credit)
            // nên khi ghi Có, số dư của nó sẽ tăng.
            salesRevenueAccount.Balance += order.TotalAmount;

            await _context.SaveChangesAsync();
        }
        // Thêm phương thức này vào Services/AccountingService.cs
        public async Task CreateJournalEntryForPurchaseOrder(PurchaseOrder order)
        {
            // Tìm các tài khoản đã định nghĩa trước
            var inventoryAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == "152");
            var accountsPayable = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == "331");

            if (inventoryAccount == null || accountsPayable == null)
            {
                // Nếu không tìm thấy tài khoản, không làm gì cả (hoặc ghi log lỗi)
                return;
            }

            // Tính tổng giá trị hàng nhập kho
            decimal totalValue = order.OrderDetails.Sum(d => d.Quantity * d.UnitPrice);

            // 1. Tạo bút toán chung (Journal Entry)
            var journalEntry = new JournalEntry
            {
                EntryDate = DateTime.Now,
                Description = $"Ghi nhận nhập kho và công nợ cho Đơn mua hàng #{order.Id} - NCC: {order.Supplier?.Name}"
            };
            _context.JournalEntries.Add(journalEntry);
            await _context.SaveChangesAsync(); // Lưu để lấy Id

            // 2. Tạo dòng Nợ (Debit) cho tài khoản Hàng tồn kho (Tài sản tăng)
            var debitLine = new JournalEntryLine
            {
                JournalEntryId = journalEntry.Id,
                AccountId = inventoryAccount.Id,
                DebitOrCredit = DebitOrCredit.Debit,
                Amount = totalValue
            };
            _context.JournalEntryLines.Add(debitLine);

            // 3. Tạo dòng Có (Credit) cho tài khoản Phải trả người bán (Nợ phải trả tăng)
            var creditLine = new JournalEntryLine
            {
                JournalEntryId = journalEntry.Id,
                AccountId = accountsPayable.Id,
                DebitOrCredit = DebitOrCredit.Credit,
                Amount = totalValue
            };
            _context.JournalEntryLines.Add(creditLine);

            // 4. Cập nhật số dư các tài khoản
            inventoryAccount.Balance += totalValue;
            accountsPayable.Balance += totalValue; // Nợ phải trả tăng khi ghi Có

            await _context.SaveChangesAsync();
        }
        // Thêm vào AccountingService.cs
        public async Task CreateJournalEntryForPaymentReceived(Payment payment)
        {
            // Bút toán: Nợ TK Tiền (111/112), Có TK Phải thu (131)
            var cashAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == "111"); // Giả sử là tiền mặt
            var accountsReceivable = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == "131");

            if (cashAccount == null || accountsReceivable == null) return;

            var journalEntry = new JournalEntry { /* ... */ };
            // ... Tạo 2 dòng JournalEntryLine ...

            // Cập nhật số dư
            cashAccount.Balance += payment.Amount;
            accountsReceivable.Balance -= payment.Amount;

            await _context.SaveChangesAsync();
        }
    }
}