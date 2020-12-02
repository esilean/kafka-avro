using AutoMapper;
using KafkaStudy.Consumer.Builder.Interfaces;
using KafkaStudy.Consumer.Notification;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;

namespace KafkaStudy.Consumer.Builder
{
    public class KafkaTopicMessageConsumer<TAvro, TResponse> : IKafkaTopicMessageConsumer
    {
        private readonly IKafkaConsumerBuilder<TAvro> _kafkaConsumerBuilder;
        private readonly ILogger<KafkaTopicMessageConsumer<TAvro, TResponse>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        public KafkaTopicMessageConsumer(ILogger<KafkaTopicMessageConsumer<TAvro, TResponse>> logger,
                                         IKafkaConsumerBuilder<TAvro> kafkaConsumerBuilder,
                                         IServiceProvider serviceProvider,
                                         IMapper mapper)
        {
            _logger = logger;
            _kafkaConsumerBuilder = kafkaConsumerBuilder;
            _serviceProvider = serviceProvider;
            _mapper = mapper;
        }

        public void StartConsuming(string topic, CancellationToken cancellationToken)
        {
            using (var consumer = _kafkaConsumerBuilder.Build())
            {

                _logger.LogInformation($"Starting consumer for {topic}");
                consumer.Subscribe(topic.ToString());

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var consumeResult = consumer.Consume(cancellationToken);

                        var messageTypeEncoded = consumeResult.Message.Headers.GetLastBytes("message-type");
                        var messageTypeHeader = Encoding.UTF8.GetString(messageTypeEncoded);
                        var messageType = Type.GetType(messageTypeHeader);

                        var message = _mapper.Map<TResponse>(consumeResult.Message.Value);
                        var messageNotificationType = typeof(MessageNotification<>).MakeGenericType(messageType);
                        var messageNotification = Activator.CreateInstance(messageNotificationType, message);

                        using var scope = _serviceProvider.CreateScope();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        mediator.Publish(messageNotification, cancellationToken).GetAwaiter().GetResult();
                    }
                }
                catch (OperationCanceledException)
                {
                    // do nothing on cancellation
                }
                finally
                {
                    consumer.Close();
                }
            }
        }

    }
}
