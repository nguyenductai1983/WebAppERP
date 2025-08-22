namespace WebAppERP.Models
{
    public class RequiredMaterialViewModel
    {
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        public decimal RequiredQuantity { get; set; }
        public decimal CurrentStock { get; set; }
    }
}