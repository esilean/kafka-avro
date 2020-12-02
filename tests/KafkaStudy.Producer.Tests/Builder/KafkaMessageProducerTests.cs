using Confluent.Kafka;
using KafkaStudy.Common.Tests.Fake;
using KafkaStudy.Producer.Builder;
using KafkaStudy.Producer.Builder.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KafkaStudy.Producer.Tests.Builder
{
    public class KafkaMessageProducerTests
    {
        [Fact(DisplayName = "Produce - Deve produzir uma mensagem com o tópico correto")]
        public async Task Produce_ShouldProduceMessageWithCorrectTopic()
        {
            //Arrange
            const string expectedTopic = "fake-messages";
            var stubMessageProducerBuilder = new Mock<IKafkaProducerBuilder<FakeAvro>>();

            var stubLogger = Mock.Of<ILogger<KafkaMessageProducer<FakeAvro>>>();
            var mockProducer = new Mock<IProducer<string, FakeAvro>>();
            stubMessageProducerBuilder
                .Setup(x => x.Build())
                .Returns(mockProducer.Object);
            var fakeMessage = new FakeMessage("fake-property-value");

            //Act
            var sut = new KafkaMessageProducer<FakeAvro>(stubLogger, stubMessageProducerBuilder.Object);
            await sut.ProduceAsync(fakeMessage, "key", CancellationToken.None);

            //Arrange
            mockProducer.Verify(x => x.ProduceAsync(expectedTopic,
                It.IsAny<Message<string, FakeAvro>>(),
                It.IsAny<CancellationToken>()));
        }

        [Fact(DisplayName = "Produce - Deve produzir uma mensagem com a key correta")]
        public async Task Produce_ShouldProduceMessageWithCorrectKey()
        {
            //Arrange
            const string expectedMessageKey = "some-key-id";
            var stubMessageProducerBuilder = new Mock<IKafkaProducerBuilder<FakeAvro>>();
            var stubLogger = Mock.Of<ILogger<KafkaMessageProducer<FakeAvro>>>();
            var mockProducer = new Mock<IProducer<string, FakeAvro>>();
            stubMessageProducerBuilder
                .Setup(x => x.Build())
                .Returns(mockProducer.Object);
            var fakeMessage = new FakeMessage("fake-property-value");

            //Act
            var sut = new KafkaMessageProducer<FakeAvro>(stubLogger, stubMessageProducerBuilder.Object);
            await sut.ProduceAsync(fakeMessage, expectedMessageKey, CancellationToken.None);

            //Assert
            mockProducer.Verify(x => x.ProduceAsync(It.IsAny<string>(),
                It.Is<Message<string, FakeAvro>>(i => i.Key == expectedMessageKey),
                It.IsAny<CancellationToken>()));
        }

        [Fact(DisplayName = "Produce - Deve produzir uma mensagem com o type no header")]
        public async Task Produce_ShouldProduceMessageTypeAsHeader()
        {
            //Arrange
            var stubMessageProducerBuilder = new Mock<IKafkaProducerBuilder<FakeAvro>>();
            var stubLogger = Mock.Of<ILogger<KafkaMessageProducer<FakeAvro>>>();
            var mockProducer = new Mock<IProducer<string, FakeAvro>>();
            stubMessageProducerBuilder
                .Setup(x => x.Build())
                .Returns(mockProducer.Object);
            var expectedMessageType = typeof(FakeMessage).AssemblyQualifiedName;
            var fakeMessage = new FakeMessage("some-property-value");

            //Act
            var sut = new KafkaMessageProducer<FakeAvro>(stubLogger, stubMessageProducerBuilder.Object);
            await sut.ProduceAsync(fakeMessage, "key", CancellationToken.None);

            //Assert
            mockProducer.Verify(x => x.ProduceAsync(It.IsAny<string>(),
                It.Is<Message<string, FakeAvro>>(i =>
                    Encoding.UTF8.GetString(i.Headers.GetLastBytes("message-type")) == expectedMessageType),
                It.IsAny<CancellationToken>()));
        }

        [Fact(DisplayName = "Produce - Deve usar um único builder para mais de uma pub")]
        public async Task Produce_ShouldUseASingleProducerForMultipleRequests()
        {
            //Arrange
            var mockMessageProducerBuilder = new Mock<IKafkaProducerBuilder<FakeAvro>>();
            var stubLogger = Mock.Of<ILogger<KafkaMessageProducer<FakeAvro>>>();
            var mockProducer = new Mock<IProducer<string, FakeAvro>>();
            mockMessageProducerBuilder
                .Setup(x => x.Build())
                .Returns(mockProducer.Object);
            var fakeMessage = new FakeMessage("some-property-value");

            //Act
            var sut = new KafkaMessageProducer<FakeAvro>(stubLogger, mockMessageProducerBuilder.Object);
            await sut.ProduceAsync(fakeMessage, "key", CancellationToken.None);
            await sut.ProduceAsync(fakeMessage, "key", CancellationToken.None);
            await sut.ProduceAsync(fakeMessage, "key", CancellationToken.None);

            //Assert
            mockMessageProducerBuilder.Verify(x => x.Build(), Times.Once);
        }

        [Fact(DisplayName = "Dispose - Deve realizar o dispose se producer cancelado")]
        public async Task Dispose_ShouldDisposeProducerIfProduceHasBeenCalled()
        {
            //Arrange
            var stubMessageProducerBuilder = new Mock<IKafkaProducerBuilder<FakeAvro>>();
            var stubLogger = Mock.Of<ILogger<KafkaMessageProducer<FakeAvro>>>();
            var mockProducer = new Mock<IProducer<string, FakeAvro>>();
            stubMessageProducerBuilder
                .Setup(x => x.Build())
                .Returns(mockProducer.Object);
            var fakeMessage = new FakeMessage("some-property-value");

            //Act
            var sut = new KafkaMessageProducer<FakeAvro>(stubLogger, stubMessageProducerBuilder.Object);
            await sut.ProduceAsync(fakeMessage, "key", CancellationToken.None);
            sut.Dispose();

            //Asset
            mockProducer.Verify(x => x.Dispose());
        }

        [Fact(DisplayName = "Dispose - Deve ignorar dispose se producer nao cancelado")]
        public void Dispose_ShouldNotDisposeProducerIfProduceHasNotBeenCalled()
        {
            var stubMessageProducerBuilder = new Mock<IKafkaProducerBuilder<FakeAvro>>();
            var stubLogger = Mock.Of<ILogger<KafkaMessageProducer<FakeAvro>>>();
            var mockProducer = new Mock<IProducer<string, FakeAvro>>();
            stubMessageProducerBuilder
                .Setup(x => x.Build())
                .Returns(mockProducer.Object);

            var sut = new KafkaMessageProducer<FakeAvro>(stubLogger, stubMessageProducerBuilder.Object);
            sut.Dispose();

            mockProducer.Verify(x => x.Dispose(), Times.Never);
        }
    }
}
