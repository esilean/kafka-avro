using KafkaStudy.Consumer.Builder.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KafkaStudy.Consumer.Tests
{
    public class WorkerTests
    {
        [Fact(DisplayName = "ExecuteAsync - Deve iniciar os consumers")]
        public async Task ExecuteAsync_ShouldStartKafkaMessageConsumers()
        {
            var stubLogger = Mock.Of<ILogger<Worker>>();
            var mockKafkaMessageConsumerStarter = new Mock<IKafkaMessageConsumerManager>();
            var cancellationTokenSource = new CancellationTokenSource();

            var sut = new Worker(stubLogger, mockKafkaMessageConsumerStarter.Object);
            await sut.StartAsync(cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();

            mockKafkaMessageConsumerStarter.Verify(x => x.StartConsumers(It.IsAny<CancellationToken>()));
        }
    }
}
