using Confluent.Kafka;
using KafkaStudy.Common;
using KafkaStudy.Producer.Builder.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaStudy.Producer.Builder
{
    public class KafkaMessageProducer<TAvro> : IMessageProducer<TAvro>, IDisposable
    {
        private readonly Lazy<IProducer<string, TAvro>> _cachedProducer;
        private readonly ILogger<KafkaMessageProducer<TAvro>> _logger;

        public KafkaMessageProducer(ILogger<KafkaMessageProducer<TAvro>> logger, IKafkaProducerBuilder<TAvro> kafkaProducerBuilder)
        {
            _cachedProducer = new Lazy<IProducer<string, TAvro>>(() => kafkaProducerBuilder.Build());
            _logger = logger;
        }

        public void Dispose()
        {
            if (_cachedProducer.IsValueCreated) _cachedProducer.Value.Dispose();
        }

        public async Task ProduceAsync(TAvro message, string key, CancellationToken cancellationToken)
        {

            var topic = Attribute.GetCustomAttributes(message.GetType())
                .OfType<MessageTopicAttribute>()
                .Single()
                .Topic;

            var messageType = message.GetType().AssemblyQualifiedName;
            var producedMessage = new Message<string, TAvro>
            {
                Key = key,
                Value = message,
                Headers = new Headers
                {
                    { "message-type", Encoding.UTF8.GetBytes(messageType) }
                }
            };

            await _cachedProducer.Value.ProduceAsync(topic, producedMessage, cancellationToken)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine($"Error: {task.Exception.Message}");
                    }
                    else
                    {
                        _logger.LogInformation($"Message wrote to offset: {task.Result?.Offset}");
                    }
                });

        }
    }
}
