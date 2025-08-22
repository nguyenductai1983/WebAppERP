using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    // << THÊM ENUM MỚI NÀY ĐỂ PHÂN LOẠI LỆNH SẢN XUẤT >>
    public enum WorkOrderType
    {
        [Display(Name = "LSX Tổng")]
        Master,
        [Display(Name = "LSX Công đoạn")]
        Stage,
        [Display(Name = "LSX Bán thành phẩm")] // << THÊM LOẠI MỚI NÀY
        SubAssembly
    }

    public enum WorkOrderStatus
    {
        [Display(Name = "Mới tạo")]
        New,
        [Display(Name = "Đang sản xuất")]
        InProgress,
        [Display(Name = "Hoàn thành")]
        Completed,
        [Display(Name = "Đã hủy")]
        Cancelled
    }

    public class WorkOrder
    {
        public WorkOrder()
        {
            this.WorkOrderBOMs = new HashSet<WorkOrderBOM>();
            this.WorkOrderRoutings = new HashSet<WorkOrderRouting>();
        }

        public int Id { get; set; }

        [StringLength(250)]
        [Display(Name = "Diễn giải / Tên LSX")]
        public string Description { get; set; }

        // === CÁC THUỘC TÍNH MỚI ĐỂ PHÂN LOẠI VÀ LIÊN KẾT ===
        [Display(Name = "Loại LSX")]
        public WorkOrderType Type { get; set; } = WorkOrderType.Master; // Mặc định là LSX Tổng

        [Display(Name = "Thuộc Công đoạn")]
        public int? ProductionStageId { get; set; } // Dùng cho LSX Công đoạn
        public virtual ProductionStage ProductionStage { get; set; }
        // ===================================================

        [Required]
        [Display(Name = "Sản phẩm")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [Display(Name = "Số lượng cần sản xuất")]
        public int QuantityToProduce { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Ngày dự kiến hoàn thành")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Trạng thái")]
        public WorkOrderStatus Status { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Chi phí NVL thực tế")]
        public decimal ActualMaterialCost { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Chi phí nhân công thực tế")]
        public decimal ActualLaborCost { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Chi phí chung thực tế")]
        public decimal ActualOverheadCost { get; set; }

        [NotMapped]
        [Display(Name = "Tổng chi phí sản xuất")]
        public decimal TotalCost => ActualMaterialCost + ActualLaborCost + ActualOverheadCost;

        [Display(Name = "Lệnh sản xuất cha")]
        public int? ParentWorkOrderId { get; set; }
        public virtual WorkOrder ParentWorkOrder { get; set; }

        public virtual ICollection<WorkOrderBOM> WorkOrderBOMs { get; set; }
        public virtual ICollection<WorkOrderRouting> WorkOrderRoutings { get; set; }
    }
}