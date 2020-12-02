using KafkaStudy.Common.Avro;
using KafkaStudy.Common.Messages;
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
    public class KafkaTopicMessageConsumer<T> : IKafkaTopicMessageConsumer
    {
        private readonly IKafkaConsumerBuilder<T> _kafkaConsumerBuilder;
        private readonly ILogger<KafkaTopicMessageConsumer<T>> _logger;
        private readonly IServiceProvider _serviceProvider;

        public KafkaTopicMessageConsumer(ILogger<KafkaTopicMessageConsumer<T>> logger,
                                         IKafkaConsumerBuilder<T> kafkaConsumerBuilder,
                                         IServiceProvider serviceProvider)
        {
            _logger = logger;
            _kafkaConsumerBuilder = kafkaConsumerBuilder;
            _serviceProvider = serviceProvider;
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

                        object message = null;
                        if (messageType == typeof(StatusUpdatedMessage))
                        {
                            var statusUpdatedMessage = consumeResult.Message.Value as StatusUpdatedAvroMessage;
                            message = StatusUpdatedMessage.Factory.Create(statusUpdatedMessage.Id, statusUpdatedMessage.Status);
                        }
                        else if (messageType == typeof(OrderCreatedMessage))
                        {
                            var orderCreatedMessage = consumeResult.Message.Value as OrderCreatedAvroMessage;
                            message = OrderCreatedMessage.Factory.Create(
                                                                        orderCreatedMessage.Id,
                                                                        orderCreatedMessage.CustomerName,
                                                                        orderCreatedMessage.Age,
                                                                        orderCreatedMessage.Qty,
                                                                        orderCreatedMessage.CartValue);
                        }
                        else
                        {
                            // TODO Try to make it generic
                        }


                        var messageNotificationType = typeof(MessageNotification<>).MakeGenericType(messageType);
                        var messageNotification = Activator.CreateInstance(messageNotificationType, message);

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                            mediator.Publish(messageNotification, cancellationToken).GetAwaiter().GetResult();
                        }
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
