using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NotificationService.Models;
using System.Text.Json;

namespace NotificationService
{
    public class OrderRecievedWorker : BackgroundService
    {
        private readonly ILogger<OrderRecievedWorker> logger;
        private readonly IHubContext<NotificationHub> hub;
        private readonly ConsumerConfig consumerConfig;

        public OrderRecievedWorker(
            IOptions<ConsumerConfig> consumerOptions,
            ILogger<OrderRecievedWorker> logger, 
            IHubContext<NotificationHub> hub)
        {
            this.consumerConfig = consumerOptions.Value;
            this.logger = logger;
            this.hub = hub;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(() => DoWork(stoppingToken)).Start();
            return Task.CompletedTask;
        }

        private void DoWork(CancellationToken stoppingToken)
        {
            try
            {
                using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
                {
                    consumer.Subscribe("order_recieved_event");

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        logger.LogInformation("Waiting data from order_recieved_event topic");
                        var result = consumer.Consume(default(CancellationToken));
                        logger.LogInformation("Received data from order_recieved_event topic");

                        hub.Clients.All.SendAsync("order_recieved_event", JsonSerializer.Deserialize<OrderReceivedModel>(result.Message.Value));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }
    }
}
