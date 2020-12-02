using System.Threading;

namespace KafkaStudy.Consumer.Builder.Interfaces
{
    public interface IKafkaTopicMessageConsumer
    {
        void StartConsuming(string topic, CancellationToken cancellationToken);
    }
}
