using KafkaStudy.Common;
using KafkaStudy.Common.Avro;
using KafkaStudy.Common.Messages;
using System;
using System.Linq;
using Xunit;

namespace KafkaStudy.Producer.Tests.Messages
{
    public class StatusUpdatedMessageTests
    {
        private readonly string _id;
        private readonly string _status;

        public StatusUpdatedMessageTests()
        {
            _id = "id";
            _status = "status";
        }


        [Fact(DisplayName = "Factory - Deve criar um objeto de StatusUpdated")]
        public void Factory_ShouldCreateAValidMessage()
        {
            //Arrange
            //Act
            var message = StatusUpdatedMessage.Factory.Create(_id, _status);

            //Assert
            Assert.Equal(_id, message.Id);
            //Assert.Equal(_id, message.Key);
            Assert.Equal(_status, message.Status);
        }

        [Fact(DisplayName = "Factory - Deve existir um tópico relacionado a esta mensagem")]
        public void Factory_ShouldExistATopicInTheMessage()
        {
            //Arrange
            var message = StatusUpdatedMessage.Factory.Create(_id, _status);
            //Act
            var topic = Attribute.GetCustomAttributes(message.GetType())
                        .OfType<MessageTopicAttribute>()
                        .Single()
                        .Topic;

            //Assert
            Assert.Equal(Topics.STATUS_UPDATED, topic);
        }

        [Fact(DisplayName = "Factory - Deve ser do tipo AVRO")]
        public void Factory_ShouldBeAvroType()
        {
            //Arrange
            var message = StatusUpdatedMessage.Factory.Create(_id, _status);

            //Act
            var result = message.GetType().IsSubclassOf(typeof(StatusUpdatedAvroMessage));

            //Assert
            Assert.True(result);
        }
    }
}
