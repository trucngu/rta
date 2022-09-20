namespace NotificationService.Models
{
    public class OrderReceivedModel
    {
        public string OrderNumber { get; set; }
        public string Product { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Customer { get; set; }
        public int Status { get; set; }
    }
}
