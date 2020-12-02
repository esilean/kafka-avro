using Xunit;

namespace KafkaStudy.Common.Tests
{
    public class MessageTopicAttributeTests
    {
        [Fact(DisplayName = "Constructor - Deve retornar o valor do topic")]
        public void Constructor_ReturnsMessageTopicAttribute()
        {
            const string topic = "sample-topic";

            var sut = new MessageTopicAttribute(topic);

            Assert.Equal(topic, sut.Topic);
        }
    }
}
