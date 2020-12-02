using Confluent.Kafka;

namespace KafkaStudy.Producer.Builder.Interfaces
{
    public interface IKafkaProducerBuilder<TAvro>
    {
        IProducer<string, TAvro> Build();
    }
}
