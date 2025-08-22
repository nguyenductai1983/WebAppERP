namespace WebAppERP.Models
{
    public class DashboardViewModel
    {
        // Dành cho Admin
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int PendingLeaveRequests { get; set; }
        public int TotalSalesOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        // Dành cho nhân viên
        public int MyPendingLeaveRequests { get; set; }
    }
}