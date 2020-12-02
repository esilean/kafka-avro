using KafkaStudy.Common;
using KafkaStudy.Common.Interfaces;
using KafkaStudy.Consumer.Builder.Interfaces;
using KafkaStudy.Consumer.Notification;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static KafkaStudy.Consumer.Infra.ServiceExtensions;

namespace KafkaStudy.Consumer.Builder
{
    public class KafkaMessageConsumerManager : IKafkaMessageConsumerManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceCollection _services;

        public KafkaMessageConsumerManager(IServiceProvider serviceProvider, IServiceCollection services)
        {
            _serviceProvider = serviceProvider;
            _services = services;
        }

        public void StartConsumers(CancellationToken cancellationToken)
        {
            var topicsWithNotificationHandlers = GetTopicsWithNotificationHandlers(_services);

            foreach (var topic in topicsWithNotificationHandlers)
            {
                var kafkaTopicMessageConsumer = _serviceProvider.GetService<ServiceResolver>().Invoke(topic);

                new Thread(() => kafkaTopicMessageConsumer.StartConsuming(topic, cancellationToken)).Start();
            }
        }

        private static IEnumerable<string> GetTopicsWithNotificationHandlers(IServiceCollection services)
        {
            var messageTypesWithNotificationHandlers = services
                .Where(s => s.ServiceType.IsGenericType &&
                            s.ServiceType.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
                .Select(s => s.ServiceType.GetGenericArguments()[0])
                .Where(s => s.IsGenericType &&
                            s.GetGenericTypeDefinition() == typeof(MessageNotification<>))
                .Select(s => s.GetGenericArguments()[0])
                .Where(s => typeof(IMessage).IsAssignableFrom(s))
                .Distinct();

            var messageList = messageTypesWithNotificationHandlers
                .SelectMany(t => Attribute.GetCustomAttributes(t))
                .OfType<MessageTopicAttribute>()
                .Select(t => t.Topic)
                .Distinct()
                .ToList();

            return messageList;
        }
    }
}
