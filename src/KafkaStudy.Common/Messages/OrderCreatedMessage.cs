using KafkaStudy.Common.Avro;
using KafkaStudy.Common.Interfaces;

namespace KafkaStudy.Common.Messages
{
    [MessageTopic(Topics.ORDER_CREATED)]
    public class OrderCreatedMessage : OrderCreatedAvroMessage, IMessage
    {
        private OrderCreatedMessage(string id, string customerName, int age, int qty, double cartValue)
        {
            Id = id;
            CustomerName = customerName;
            Age = age;
            Qty = qty;
            CartValue = cartValue;
        }

        public class Factory
        {
            public static OrderCreatedMessage Create(string id, string customerName, int age, int qty, double cartValue)
            {
                return new OrderCreatedMessage(id, customerName, age, qty, cartValue);
            }
        }
    }
}
