// File: ViewModels/ProductionLogViewModel.cs (Phiên bản nâng cao)
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebAppERP.Models;

namespace WebAppERP.ViewModels
{
    // Lớp chứa thông tin cho MỘT dòng nguyên vật liệu đầu vào
    public class MaterialInputViewModel
    {
        public int WorkOrderBOMId { get; set; }
        public string ComponentName { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal RequiredQuantity { get; set; }
        public decimal AlreadyConsumedQuantity { get; set; }
        public bool IsLotTracked { get; set; }
        public SelectList AvailableLots { get; set; }

        // Dữ liệu người dùng nhập
        [Display(Name = "Chọn lô")]
        public int? SelectedLotId { get; set; }

        [Display(Name = "Lượng tiêu thụ")]
        [Range(0, double.MaxValue)]
        public decimal QuantityToConsume { get; set; }
    }

    // Lớp MỚI chứa thông tin cho MỘT dòng sản phẩm đầu ra
    public class ProductionOutputViewModel
    {
        [Required]
        [Display(Name = "Sản lượng")]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
        // ==> THÊM THUỘC TÍNH NÀY
        [Display(Name = "Số Lượng Lô/Xe cần tạo")]
        [Range(1, 100, ErrorMessage = "Số lượng lô phải từ 1 đến 100.")]
        public int NumberOfLotsToCreate { get; set; } = 1;
        // Dictionary để chứa các thông số kỹ thuật đặc thù (NetWeight, Length, Width...)
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }

    // ViewModel chính được thiết kế lại
    public class ProductionLogViewModel
    {
        // Thông tin chung
        public int WorkOrderRoutingId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn nhân viên.")]
        public string OperatorId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn máy.")]
        public int MachineId { get; set; }
        public string Notes { get; set; }

        // ==> THAY ĐỔI LỚN: Giờ đây là DANH SÁCH đầu ra và đầu vào
        public List<ProductionOutputViewModel> Outputs { get; set; } = new List<ProductionOutputViewModel>();
        public List<MaterialInputViewModel> Inputs { get; set; } = new List<MaterialInputViewModel>();

        // Thuộc tính để truyền dữ liệu sang View
        public WorkOrderRouting RoutingInfo { get; set; }
    }
}