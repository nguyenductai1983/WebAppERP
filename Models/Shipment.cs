// File: Models/Shipment.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace WebAppERP.Models
{
    public enum ShipmentStatus
    {
        [Display(Name = "Chờ xuất kho")]
        Pending,
        [Display(Name = "Đã xuất kho")]
        Shipped
    }

    [Table("Shipments")]
    public class Shipment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SalesOrderId { get; set; }
        public virtual SalesOrder SalesOrder { get; set; }

        [Required]
        [Display(Name = "Ngày tạo phiếu")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Ngày thực xuất")]
        public DateTime? ShippedDate { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public ShipmentStatus Status { get; set; }

        [Display(Name = "Người xuất kho")]
        public string ShippedById { get; set; }
        public virtual IdentityUser ShippedBy { get; set; }

        public virtual ICollection<ShipmentDetail> ShipmentDetails { get; set; } = new List<ShipmentDetail>();
    }
}