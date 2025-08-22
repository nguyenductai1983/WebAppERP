using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAppERP.Models;

namespace WebAppERP.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- DANH SÁCH CÁC BẢNG (DbSet) ---
        public DbSet<Account> Accounts { get; set; }
        public DbSet<BillOfMaterial> BillOfMaterials { get; set; }
        public DbSet<CoatedTextile> CoatedTextiles { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<MachineType> MachineTypes { get; set; }
        public DbSet<MaterialConsumptionLog> MaterialConsumptionLogs { get; set; }
        public DbSet<MaterialIssue> MaterialIssues { get; set; }
        public DbSet<MaterialIssueDetail> MaterialIssueDetails { get; set; }
        public DbSet<MaterialRequisition> MaterialRequisitions { get; set; }
        public DbSet<MaterialRequisitionDetail> MaterialRequisitionDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductionLog> ProductionLogs { get; set; }
        public DbSet<ProductionPlan> ProductionPlans { get; set; }
        public DbSet<ProductionStage> ProductionStages { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Textile> Textiles { get; set; }
        public DbSet<TextileType> TextileTypes { get; set; }
        public DbSet<TextileYarnUsage> TextileYarnUsages { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<WorkOrderBOM> WorkOrderBOMs { get; set; }
        public DbSet<WorkOrderRouting> WorkOrderRoutings { get; set; }
        public DbSet<Workshop> Workshops { get; set; }
        public DbSet<Yarn> Yarns { get; set; }
        public DbSet<YarnType> YarnTypes { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentDetail> ShipmentDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // QUAN TRỌNG: Luôn giữ dòng này ở đầu tiên
            base.OnModelCreating(modelBuilder);

            // === CẤU HÌNH CÁC MỐI QUAN HỆ (ĐÃ DỌN DẸP VÀ TỔNG HỢP) ===

            // --- Ngăn chặn xóa dây chuyền cho các bảng có liên kết đến người dùng (AspNetUsers) ---
            modelBuilder.Entity<Employee>().HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Yarn>().HasOne(y => y.Operator).WithMany().HasForeignKey(y => y.OperatorId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Textile>().HasOne(t => t.Operator).WithMany().HasForeignKey(t => t.OperatorId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CoatedTextile>().HasOne(ct => ct.Operator).WithMany().HasForeignKey(ct => ct.OperatorId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MaterialIssue>().HasOne(m => m.IssuedBy).WithMany().HasForeignKey(m => m.IssuedById).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MaterialRequisition>().HasOne(m => m.RequestedBy).WithMany().HasForeignKey(m => m.RequestedById).OnDelete(DeleteBehavior.Restrict);

            // --- Cấu hình cho BillOfMaterial (BOM) ---
            modelBuilder.Entity<BillOfMaterial>().HasOne(b => b.FinishedProduct).WithMany().HasForeignKey(b => b.FinishedProductId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BillOfMaterial>().HasOne(b => b.Component).WithMany().HasForeignKey(b => b.ComponentId).OnDelete(DeleteBehavior.Restrict);

            // --- Cấu hình cho WorkOrder và các bảng liên quan ---
            modelBuilder.Entity<WorkOrder>()
                .HasOne(wo => wo.ParentWorkOrder)
                .WithMany() // Không có collection điều hướng ngược lại
                .HasForeignKey(wo => wo.ParentWorkOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkOrderRouting>()
                .HasOne(r => r.WorkOrder)
                .WithMany(w => w.WorkOrderRoutings) // Có collection WorkOrderRoutings trong WorkOrder
                .HasForeignKey(r => r.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade); // Khi xóa WorkOrder, xóa luôn các bước công đoạn của nó

            modelBuilder.Entity<WorkOrderBOM>().HasOne(b => b.WorkOrder).WithMany(w => w.WorkOrderBOMs).HasForeignKey(b => b.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<WorkOrderBOM>().HasOne(b => b.Component).WithMany().HasForeignKey(b => b.ComponentId).OnDelete(DeleteBehavior.Restrict);

            // --- Cấu hình cho các bảng Ghi nhận Sản xuất ---
            modelBuilder.Entity<MaterialConsumptionLog>().HasOne(m => m.WorkOrderBOM).WithMany().HasForeignKey(m => m.WorkOrderBOMId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TextileYarnUsage>().HasOne(u => u.Textile).WithMany().HasForeignKey(u => u.TextileId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TextileYarnUsage>().HasOne(u => u.Yarn).WithMany().HasForeignKey(u => u.YarnId).OnDelete(DeleteBehavior.Restrict);

            // --- Cấu hình cho các bảng Xuất/Lĩnh NVL ---
            modelBuilder.Entity<MaterialRequisitionDetail>().HasOne(d => d.WorkOrderBOM).WithMany().HasForeignKey(d => d.WorkOrderBOMId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MaterialIssueDetail>().HasOne(d => d.WorkOrderBOM).WithMany().HasForeignKey(d => d.WorkOrderBOMId).OnDelete(DeleteBehavior.Restrict);

            // --- Cấu hình cho các bảng Dữ liệu gốc ---
            modelBuilder.Entity<MachineType>()
                .HasOne(mt => mt.ProductionStage)
                .WithMany()
                .HasForeignKey(mt => mt.ProductionStageId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkOrderRouting>()
                .HasOne(r => r.ProductionStage)
                .WithMany() // A stage can be in many routings
                .HasForeignKey(r => r.ProductionStageId)
                .OnDelete(DeleteBehavior.Restrict); // <-- This is the important fix
        }
    }
}