using KafkaStudy.Common.Avro;
using KafkaStudy.Producer.Builder;
using KafkaStudy.Producer.Builder.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaStudy.Producer.Infra
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddKafkaProducer(this IServiceCollection services)
        {
            //status-updated
            services.AddSingleton<IKafkaProducerBuilder<StatusUpdatedAvroMessage>, KafkaProducerBuilder<StatusUpdatedAvroMessage>>();
            services.AddSingleton<IMessageProducer<StatusUpdatedAvroMessage>, KafkaMessageProducer<StatusUpdatedAvroMessage>>();

            //order-created
            services.AddSingleton<IKafkaProducerBuilder<OrderCreatedAvroMessage>, KafkaProducerBuilder<OrderCreatedAvroMessage>>();
            services.AddSingleton<IMessageProducer<OrderCreatedAvroMessage>, KafkaMessageProducer<OrderCreatedAvroMessage>>();

            return services;
        }
    }
}
