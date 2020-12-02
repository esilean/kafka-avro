using KafkaStudy.Common.Avro;
using KafkaStudy.Common.Messages;
using KafkaStudy.Producer.Builder.Interfaces;
using KafkaStudy.Producer.Workers;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KafkaStudy.Producer.Tests
{
    public class StatusUpdatedWorkerTests
    {
        [Fact(DisplayName = "ExecuteAsync - Deve publicar uma mensagem de status atualizado")]
        public async Task ExecuteAsync_ShouldPublishMessage()
        {
            //Arrange
            var stubLogger = Mock.Of<ILogger<StatusUpdatedWorker>>();
            var mockStatusUpdatedProducer = new Mock<IMessageProducer<StatusUpdatedAvroMessage>>();
            var cancellationTokenSource = new CancellationTokenSource();

            var sut = new StatusUpdatedWorker(stubLogger, mockStatusUpdatedProducer.Object);

            //Act
            await sut.StartAsync(cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();

            //Assert
            mockStatusUpdatedProducer.Verify(x => x.ProduceAsync(It.IsAny<StatusUpdatedMessage>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
        }
    }
}
