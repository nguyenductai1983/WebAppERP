using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    // Tương ứng với bảng LOẠI SỢI
    [Table("YarnType")]
    public class YarnType
    {
        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        [Display(Name = "Tên Loại Sợi")]
        public string Name { get; set; } = null!;

        [Display(Name = "Ghi Chú")]
        public string Note { get; set; } = null!;
        //[Required]
        //[Display(Name = "Sản phẩm Tương ứng")]
        //public int ProductId { get; set; }
        //public virtual Product Product { get; set; } = null!;
    }
}
