using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebAppERP.Models;

namespace WebAppERP.ViewModels
{
    // Lớp này chứa thông tin cho một dòng NVL trên phiếu
    public class MaterialRequisitionDetailViewModel
    {
        public int WorkOrderBOMId { get; set; }
        public string ProductName { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal QuantityRequired { get; set; } // Tổng lượng cần cho LSX
        public decimal QuantityAlreadyIssued { get; set; } // Tổng lượng đã xuất kho

        [Display(Name = "SL Yêu cầu lần này")]
        [Range(0, double.MaxValue)]
        public decimal QuantityToRequest { get; set; } // Lượng yêu cầu cho lần này
    }

    // ViewModel chính cho toàn bộ trang
    public class MaterialRequisitionViewModel
    {
        public int WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; set; }
        public int WorkshopId { get; set; }

        // Danh sách các NVL cần cho LSX
        public List<MaterialRequisitionDetailViewModel> Details { get; set; } = new List<MaterialRequisitionDetailViewModel>();
    }
}