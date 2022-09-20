namespace NotificationService.Models
{
    public class OrderStatusChangedModel
    {
        public string OrderNumber { get; set; }
        public string Comment { get; set; }
        public int Status { get; set; }
    }
}
