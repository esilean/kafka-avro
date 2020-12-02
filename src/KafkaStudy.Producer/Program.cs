using KafkaStudy.Common;
using KafkaStudy.Producer.Infra;
using KafkaStudy.Producer.Workers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KafkaStudy.Producer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<StatusUpdatedWorker>();
                    services.AddHostedService<OrderCreatedWorker>();

                    services.AddOptions<KafkaOptions>()
                        .Bind(hostContext.Configuration.GetSection("Kafka"));

                    services.AddKafkaProducer();
                });
    }
}
