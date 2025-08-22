// File: Models/ShipmentDetail.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    [Table("ShipmentDetails")]
    public class ShipmentDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; }

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        [Display(Name = "Số lượng xuất")]
        public decimal QuantityToShip { get; set; }
    }
}