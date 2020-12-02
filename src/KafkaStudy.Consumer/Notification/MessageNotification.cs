using KafkaStudy.Common.Interfaces;
using MediatR;

namespace KafkaStudy.Consumer.Notification
{
    public class MessageNotification<TMessage> : INotification
                                    where TMessage : IMessage
    {
        public MessageNotification(TMessage message)
        {
            Message = message;
        }

        public TMessage Message { get; }
    }
}
