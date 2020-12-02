using System.Threading;
using System.Threading.Tasks;

namespace KafkaStudy.Producer.Builder.Interfaces
{
    public interface IMessageProducer<TAvro>
    {
        Task ProduceAsync(TAvro message, string key, CancellationToken cancellationToken);
    }
}
