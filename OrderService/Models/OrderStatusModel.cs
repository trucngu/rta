namespace OrderService.Models
{
    public class OrderStatusModel
    {
        public OrderStatus Status { get; set; }
        public string Comment { get; set; }
    }

    public enum OrderStatus
    {
        New = 1,
        Confirm = 2,
        Reject = 3,
        Dispatched = 4
    }
}
