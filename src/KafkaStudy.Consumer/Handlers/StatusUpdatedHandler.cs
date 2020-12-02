using KafkaStudy.Common.Messages;
using KafkaStudy.Consumer.Notification;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaStudy.Consumer.Handlers
{
    public class StatusUpdatedHandler : INotificationHandler<MessageNotification<StatusUpdatedMessage>>
    {
        private readonly ILogger<StatusUpdatedHandler> _logger;

        public StatusUpdatedHandler(ILogger<StatusUpdatedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(MessageNotification<StatusUpdatedMessage> notification, CancellationToken cancellationToken)
        {
            var message = notification.Message;

            _logger.LogInformation($"Message received with Id: {message.Id} and Status: {message.Status}");

            return Task.CompletedTask;
        }
    }
}
