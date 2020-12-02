using KafkaStudy.Common.Avro;
using KafkaStudy.Common.Interfaces;

namespace KafkaStudy.Common.Messages
{
    [MessageTopic(Topics.STATUS_UPDATED)]
    public class StatusUpdatedMessage : StatusUpdatedAvroMessage, IMessage
    {
        private StatusUpdatedMessage(string id, string status)
        {
            Id = id;
            Status = status;
        }

        public class Factory
        {
            public static StatusUpdatedMessage Create(string id, string status)
            {
                return new StatusUpdatedMessage(id, status);
            }
        }
    }
}
