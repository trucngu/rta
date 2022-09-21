using Confluent.Kafka;
using NotificationService;
using Serilog;
using System.Text.Json;

public class Program
{
    public static int Main(string[] args)
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:HH:mm:ss.fff} | Thead {ThreadId} | {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

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
                .AddHostedService<OrderStatusChangedWorker>()
                .AddHostedService<OrderRecievedWorker>()
                ;
            var app = builder.Build();

            app.UseCors(cors);

            app.MapHub<NotificationHub>("/hubs/notification");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

            return 0;

        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}