using Confluent.Kafka;

namespace KafkaStudy.Consumer.Builder.Interfaces
{
    public interface IKafkaConsumerBuilder<T>
    {
        IConsumer<string, T> Build();
    }
}
