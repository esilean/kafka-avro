using Confluent.Kafka;
using KafkaStudy.Common;
using KafkaStudy.Producer.Infra;
using KafkaStudy.Producer.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace KafkaStudy.Producer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var producerConfig = new ProducerConfig();
            var section = Configuration.GetSection("Kafka");
            section.Bind(producerConfig);

            services.AddHealthChecks()
                .AddKafka(producerConfig, timeout: TimeSpan.FromSeconds(5));

            services.AddOptions<KafkaOptions>().Bind(section);

            services.AddHostedService<StatusUpdatedWorker>();
            services.AddHostedService<OrderCreatedWorker>();

            services.AddKafkaProducer();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");

                endpoints.MapControllers();
            });
        }
    }
}
