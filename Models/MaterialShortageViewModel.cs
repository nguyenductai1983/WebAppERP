// File: ViewModels/MaterialShortageViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace WebAppERP.ViewModels
{
    public class MaterialShortageViewModel
    {
        public int ProductId { get; set; }

        [Display(Name = "Mã NVL")]
        public string Sku { get; set; }

        [Display(Name = "Tên Nguyên vật liệu")]
        public string ProductName { get; set; }

        [Display(Name = "Đơn vị tính")]
        public string UnitOfMeasure { get; set; }

        [Display(Name = "Tồn kho Hiện tại")]
        public decimal CurrentStock { get; set; }

        [Display(Name = "Lượng cần mua tối thiểu")]
        public decimal RequiredToOrder => CurrentStock < 0 ? -CurrentStock : 0;
    }
}