using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppERP.Models
{
    public class ProductionPlan
    {
        public int Id { get; set; }

        [Display(Name = "Ngày lập kế hoạch")]
        public DateTime PlanDate { get; set; }

        // Liên kết đến nhu cầu (tùy chọn)
        [Display(Name = "Từ Đơn bán hàng")]
        public int? SalesOrderId { get; set; }
        public virtual SalesOrder SalesOrder { get; set; } = null!;

        // Liên kết đến lệnh sản xuất được tạo ra
        [Display(Name = "Lệnh sản xuất")]
        public int WorkOrderId { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
    }
}