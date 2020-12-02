using KafkaStudy.Common.Tests.Fake;
using KafkaStudy.Consumer.Builder;
using KafkaStudy.Consumer.Notification;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading;
using Xunit;

namespace KafkaStudy.Consumer.Tests.Builder
{
    public class KafkaMessageConsumerManagerTests
    {

        [Fact(DisplayName = "Start - Deve iniciar consumers unicos por topico", Skip = "NEED HELP")]
        public void Start_ConsumersShouldStartSingleConsumerPerMessage()
        {
            var mockTopicFakeMessageConsumer = new Mock<KafkaTopicMessageConsumer<FakeMessage>>();
            var mockTopicOtherFakeMessageConsumer = new Mock<KafkaTopicMessageConsumer<OtherFakeMessage>>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(mockTopicFakeMessageConsumer.Object);
            serviceCollection.AddSingleton(mockTopicOtherFakeMessageConsumer.Object);
            serviceCollection.AddTransient(s => Mock.Of<INotificationHandler<MessageNotification<FakeMessage>>>());
            serviceCollection.AddTransient(s => Mock.Of<INotificationHandler<MessageNotification<OtherFakeMessage>>>());

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var sut = new KafkaMessageConsumerManager(serviceProvider, serviceCollection);
            sut.StartConsumers(CancellationToken.None);

            mockTopicFakeMessageConsumer.Verify(x => x.StartConsuming("fake-messages", It.IsAny<CancellationToken>()),
                Times.Once);
            mockTopicOtherFakeMessageConsumer.Verify(x => x.StartConsuming("other-fake-messages", It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
