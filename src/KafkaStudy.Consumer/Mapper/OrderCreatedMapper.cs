using AutoMapper;
using KafkaStudy.Common.Avro;
using KafkaStudy.Common.Messages;

namespace KafkaStudy.Consumer.Mapper
{
    public class OrderCreatedMapper : Profile
    {
        public OrderCreatedMapper()
        {
            CreateMap<OrderCreatedAvroMessage, OrderCreatedMessage>();
        }

    }
}
