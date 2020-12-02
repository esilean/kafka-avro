using KafkaStudy.Common;
using KafkaStudy.Consumer.Infra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KafkaStudy.Consumer
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
                    services.AddHostedService<Worker>();

                    services.AddOptions<KafkaOptions>()
                        .Bind(hostContext.Configuration.GetSection("Kafka"));

                    services.AddKafkaConsumer(typeof(Program));
                });
    }
}
