using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppERP.Models
{
    public enum RoutingStatus
    {
        [Display(Name = "Chưa bắt đầu")]
        NotStarted,
        [Display(Name = "Đang thực hiện")]
        InProgress,
        [Display(Name = "Hoàn thành")]
        Completed
    }

    [Table("WorkOrderRoutings")]
    public class WorkOrderRouting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Lệnh sản xuất")]
        public int WorkOrderId { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }

        [Required]
        [Display(Name = "Công đoạn")]
        public int ProductionStageId { get; set; }
        public virtual ProductionStage ProductionStage { get; set; }

        [Display(Name = "Sản lượng Yêu cầu")]
        public int QuantityToProduce { get; set; }

        [Display(Name = "Sản lượng Đã sản xuất")]
        public int QuantityProduced { get; set; } = 0;

        [Display(Name = "Trạng thái Công đoạn")]
        public RoutingStatus Status { get; set; } = RoutingStatus.NotStarted;
    }
}