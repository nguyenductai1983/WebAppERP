using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebAppERP.Models;

namespace WebAppERP.ViewModels
{
    // Lớp này đại diện cho MỘT DÒNG vật tư trên form xuất kho
    public class MaterialIssueDetailViewModel
    {
        public int WorkOrderBOMId { get; set; }

        [Display(Name = "Vật tư")]
        public string ProductName { get; set; }

        public string UnitOfMeasure { get; set; }

        [Display(Name = "SL Yêu cầu")]
        public decimal QuantityRequired { get; set; }

        [Display(Name = "Tồn kho")]
        public decimal CurrentStock { get; set; }

        public bool IsLotTracked { get; set; }

        // Dữ liệu Kho sẽ nhập vào
        [Display(Name = "Chọn lô xuất")]
        public int? SelectedLotId { get; set; }
        public SelectList AvailableLots { get; set; }

        [Display(Name = "Số lượng thực xuất")]
        [Range(0, double.MaxValue)]
        public decimal QuantityToIssue { get; set; }
    }

    // ViewModel chính cho toàn bộ trang
    public class MaterialIssueViewModel
    {
        public int MaterialRequisitionId { get; set; }
        public MaterialRequisition Requisition { get; set; } // Để hiển thị thông tin phiếu yêu cầu
        public List<MaterialIssueDetailViewModel> Details { get; set; } = new List<MaterialIssueDetailViewModel>();
    }
}
