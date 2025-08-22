using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAppERP.Models
{
    public enum PurchaseOrderStatus
    {
        Draft,      // Mới tạo (nháp)
        Submitted,  // Đã gửi cho nhà cung cấp
        Completed   // Đã nhận hàng
    }

    public class PurchaseOrder
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Ngày dự kiến nhận")]
        public DateTime? ExpectedDeliveryDate { get; set; }

        [Required]
        [Display(Name = "Nhà cung cấp")]
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        [Display(Name = "Trạng thái")]
        public PurchaseOrderStatus Status { get; set; }

        public virtual ICollection<PurchaseOrderDetail> OrderDetails { get; set; }
    }
}