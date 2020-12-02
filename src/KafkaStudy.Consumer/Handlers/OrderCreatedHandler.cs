using KafkaStudy.Common.Messages;
using KafkaStudy.Consumer.Notification;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaStudy.Consumer.Handlers
{
    public class OrderCreatedHandler : INotificationHandler<MessageNotification<OrderCreatedMessage>>
    {
        private readonly ILogger<OrderCreatedHandler> _logger;

        public OrderCreatedHandler(ILogger<OrderCreatedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(MessageNotification<OrderCreatedMessage> notification, CancellationToken cancellationToken)
        {
            var message = notification.Message;

            _logger.LogInformation($"Message received with Id: {message.Id} and CustomerName: {message.CustomerName}");

            return Task.CompletedTask;
        }
    }
}
