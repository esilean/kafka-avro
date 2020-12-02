using AutoMapper;
using KafkaStudy.Common.Avro;
using KafkaStudy.Common.Messages;

namespace KafkaStudy.Consumer.Mapper
{
    public class StatusUpdatedMapper : Profile
    {
        public StatusUpdatedMapper()
        {
            CreateMap<StatusUpdatedAvroMessage, StatusUpdatedMessage>();
        }

    }
}
