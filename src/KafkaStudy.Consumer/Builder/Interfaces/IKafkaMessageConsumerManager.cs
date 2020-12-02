using System.Threading;

namespace KafkaStudy.Consumer.Builder.Interfaces
{
    public interface IKafkaMessageConsumerManager
    {
        void StartConsumers(CancellationToken cancellationToken);
    }
}
