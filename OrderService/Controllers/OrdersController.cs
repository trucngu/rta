using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using System.Text.Json;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ILogger<OrdersController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderModel model)
        {
            using (var producer = new ProducerBuilder<string, string>(
                new ProducerConfig
                {
                    BootstrapServers = "localhost:9092"
                })
                .Build())
            {
                await producer.ProduceAsync("order_recieved_event", new Message<string, string>
                {
                    Key = "new_order_recieved_event",
                    Value = JsonSerializer.Serialize(new
                    {
                        OrderDateTime = DateTime.Now,
                        OrderNumber = Guid.NewGuid(),
                        Product = model.Product,
                        Customer = model.Customer,
                        Price = model.Price,
                        Description = model.Description,
                        Status = OrderStatus.New,
                    })
                });
            }
            return Ok();
        }

        [HttpPut("{orderNumber}/status")]
        public async Task<IActionResult> ChangeOrderStatus([FromRoute]string orderNumber, OrderStatusModel model)
        {
            using (var producer = new ProducerBuilder<string, string>(
                new ProducerConfig
                {
                    BootstrapServers = "localhost:9092"
                })
                .Build())
            {
                await producer.ProduceAsync("order_status_changed_event", new Message<string, string>
                {
                    Key = "order_status_changed_event",
                    Value = JsonSerializer.Serialize(new
                    {
                        OrderNumber = orderNumber,
                        Status = model.Status,
                        Comment = model.Comment
                    })
                });
            }
            return Ok();
        }
    }
}