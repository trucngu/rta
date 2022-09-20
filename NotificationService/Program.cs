using Confluent.Kafka;
using NotificationService;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var cors = "cors_policy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(cors, p =>
    {
        p.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services
    .AddSignalR()
    .AddJsonProtocol(c =>
    {
        c.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        c.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services
    .Configure<ConsumerConfig>(builder.Configuration.GetSection("Kafka:Consumer"));
builder.Services
    .AddHostedService<OrderRecievedWorker>()
    .AddHostedService<OrderStatusChangedWorker>()
    ;
var app = builder.Build();

app.UseCors(cors);

app.MapHub<NotificationHub>("/hubs/notification");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
