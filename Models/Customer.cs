using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAppERP.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Tên khách hàng")]
        public string Name { get; set; }

        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [StringLength(250)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        // Navigation property để xem tất cả đơn hàng của khách hàng này
        public virtual ICollection<SalesOrder> SalesOrders { get; set; }
    }
}