using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    public enum OrderStatus
    {
        Pending,    // Mới tạo
        Processing, // Đang xử lý
        Shipped,    // Đã giao hàng
        Completed,  // Hoàn thành
        Cancelled   // Đã hủy
    }

    public class SalesOrder
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; }

        // << XÓA BỎ TRƯỜNG CustomerName THỪA >>
        // [Required]
        // [StringLength(150)]
        // [Display(Name = "Tên khách hàng")]
        // public string CustomerName { get; set; } = null!;

        [StringLength(250)]
        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; } = null!;

        [Display(Name = "Tổng tiền")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Trạng thái")]
        public OrderStatus Status { get; set; }

        [Required] // Yêu cầu phải có CustomerId
        [Display(Name = "Khách hàng")]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!;

        public virtual ICollection<SalesOrderDetail> OrderDetails { get; set; } = new List<SalesOrderDetail>();
    }
}