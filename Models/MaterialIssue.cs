using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    public enum IssueStatus
    {
        [Display(Name = "Chờ xuất kho")]
        Pending, // Trạng thái ban đầu của Phiếu Xuất Kho (chưa dùng)

        [Display(Name = "Đã xuất kho")]
        Issued, // Kho đã xuất, chờ Sản xuất xác nhận

        [Display(Name = "Đã xác nhận nhận")]
        ReceiptConfirmed // Sản xuất đã xác nhận nhận hàng
    }
    [Table("MaterialIssues")]
    public class MaterialIssue
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Lệnh sản xuất")]
        public int WorkOrderId { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        [Required]
        [Display(Name = "Ngày yêu cầu")]
        public DateTime RequestDate { get; set; }
        [Display(Name = "Ngày thực xuất")]
        public DateTime? IssuedDate { get; set; }
        [Display(Name = "Người xuất kho")]
        public string IssuedById { get; set; }
        public virtual IdentityUser IssuedBy { get; set; }
        [Required]
        [Display(Name = "Trạng thái")]
        public IssueStatus Status { get; set; } = IssueStatus.Pending;
        [Display(Name = "Người xác nhận nhận")]
        public string ConfirmedById { get; set; }
        public virtual IdentityUser ConfirmedBy { get; set; }
        [Display(Name = "Ngày xác nhận nhận")]
        public DateTime? ConfirmationDate { get; set; }
        public virtual ICollection<MaterialIssueDetail> Details { get; set; } = new List<MaterialIssueDetail>();
    }
}