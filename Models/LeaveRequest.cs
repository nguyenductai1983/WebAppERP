using System;
using System.ComponentModel.DataAnnotations;
using WebAppERP.Models; // Namespace chứa model Employee

namespace WebAppERP.Models
{
    // Enum để quản lý trạng thái
    public enum LeaveRequestStatus
    {
        Pending,    // Đang chờ duyệt
        Approved,   // Đã duyệt
        Denied      // Bị từ chối
    }

    public class LeaveRequest
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Lý do")]
        public string Reason { get; set; }

        [Display(Name = "Trạng thái")]
        public LeaveRequestStatus Status { get; set; }

        [Display(Name = "Ngày yêu cầu")]
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; }

        // --- Foreign Key và Navigation Property ---
        // Liên kết đến nhân viên gửi đơn
        [Display(Name = "Nhân viên yêu cầu")]
        public int RequestingEmployeeId { get; set; }
        [Display(Name = "Người lập")]
        public virtual Employee RequestingEmployee { get; set; }
    }
}