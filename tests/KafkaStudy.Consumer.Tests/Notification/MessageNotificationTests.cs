using KafkaStudy.Common.Tests.Fake;
using KafkaStudy.Consumer.Notification;
using Xunit;

namespace KafkaStudy.Consumer.Tests.Notification
{
    public class MessageNotificationTests
    {
        [Fact(DisplayName = "Constructor - Deve validar notificacao")]
        public void Constructor_ReturnsMessageNotification()
        {
            //Arrange
            var fakeMessage = new FakeMessage("some-property");

            //Act
            var sut = new MessageNotification<FakeMessage>(fakeMessage);

            //Assert
            Assert.Equal(fakeMessage, sut.Message);
        }
    }
}
