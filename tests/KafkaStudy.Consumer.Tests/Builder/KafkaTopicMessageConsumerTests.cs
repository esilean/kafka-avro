using AutoMapper;
using Confluent.Kafka;
using KafkaStudy.Common.Tests.Fake;
using KafkaStudy.Consumer.Builder;
using KafkaStudy.Consumer.Builder.Interfaces;
using KafkaStudy.Consumer.Notification;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KafkaStudy.Consumer.Tests.Builder
{
    public class KafkaTopicMessageConsumerTests
    {
        [Fact(DisplayName = "Start - Deve consumir topico correto")]
        public void Start_ConsumingSubscribesToCorrectTopic()
        {
            //Arrange
            const string expectedTopic = "fake-messages";
            var stubLogger = Mock.Of<ILogger<KafkaTopicMessageConsumer<FakeAvro, FakeMessage>>>();
            var stubMediator = Mock.Of<IMediator>();
            var stubMapper = Mock.Of<IMapper>();
            var serviceProvider = BuildServiceProvider(stubMediator);
            var stubMessageConsumerBuilder = new Mock<IKafkaConsumerBuilder<FakeAvro>>();
            var mockConsumer = new Mock<IConsumer<string, FakeAvro>>();
            stubMessageConsumerBuilder
                .Setup(x => x.Build())
                .Returns(mockConsumer.Object);
            // throw exception to avoid infinite loop
            mockConsumer
                .Setup(x => x.Consume(It.IsAny<CancellationToken>()))
                .Throws<OperationCanceledException>();

            //Act
            var sut = new KafkaTopicMessageConsumer<FakeAvro, FakeMessage>(stubLogger, stubMessageConsumerBuilder.Object, serviceProvider, stubMapper);
            sut.StartConsuming(expectedTopic, CancellationToken.None);

            //Assert
            mockConsumer.Verify(x => x.Subscribe(expectedTopic));
        }

        [Fact(DisplayName = "Start - Deve consumir as msg do consumer")]
        public void Start_ConsumingConsumesMessageFromConsumer()
        {
            //Arrange
            var stubLogger = Mock.Of<ILogger<KafkaTopicMessageConsumer<FakeAvro, FakeMessage>>>();
            var stubMediator = Mock.Of<IMediator>();
            var stubMapper = Mock.Of<IMapper>();
            var serviceProvider = BuildServiceProvider(stubMediator);
            var stubMessageConsumerBuilder = new Mock<IKafkaConsumerBuilder<FakeAvro>>();
            var mockConsumer = new Mock<IConsumer<string, FakeAvro>>();
            stubMessageConsumerBuilder
                .Setup(x => x.Build())
                .Returns(mockConsumer.Object);
            // throw exception to avoid infinite loop
            mockConsumer
                .Setup(x => x.Consume(It.IsAny<CancellationToken>()))
                .Throws<OperationCanceledException>();

            //Act
            var sut = new KafkaTopicMessageConsumer<FakeAvro, FakeMessage>(stubLogger, stubMessageConsumerBuilder.Object, serviceProvider, stubMapper);
            sut.StartConsuming("fake-messages", CancellationToken.None);

            //Assert
            mockConsumer.Verify(x => x.Consume(It.IsAny<CancellationToken>()));
        }

        [Fact(DisplayName = "Start - Deve fechar o consumer quando cancelado")]
        public void Start_ConsumingClosesConsumerWhenCancelled()
        {
            var stubLogger = Mock.Of<ILogger<KafkaTopicMessageConsumer<FakeAvro, FakeMessage>>>();
            var stubMediator = Mock.Of<IMediator>();
            var stubMapper = Mock.Of<IMapper>();
            var serviceProvider = BuildServiceProvider(stubMediator);
            var stubMessageConsumerBuilder = new Mock<IKafkaConsumerBuilder<FakeAvro>>();
            var mockConsumer = new Mock<IConsumer<string, FakeAvro>>();
            stubMessageConsumerBuilder
                .Setup(x => x.Build())
                .Returns(mockConsumer.Object);
            // throw exception to avoid infinite loop
            mockConsumer
                .Setup(x => x.Consume(It.IsAny<CancellationToken>()))
                .Throws<OperationCanceledException>();

            var sut = new KafkaTopicMessageConsumer<FakeAvro, FakeMessage>(stubLogger, stubMessageConsumerBuilder.Object, serviceProvider, stubMapper);
            sut.StartConsuming("fake-messages", CancellationToken.None);

            mockConsumer.Verify(x => x.Close());
        }

        [Fact(DisplayName = "Start - Deve fechar o consumer quando cancelado")]
        public void Start_ConsumingPublishesConsumedMessageToMediator()
        {
            //Arrange
            var fakeMessage = new FakeMessage("some-property-value");
            var cancellationTokenSource = new CancellationTokenSource();
            var mockMediator = new Mock<IMediator>();
            var stubMapper = Mock.Of<IMapper>();
            var serviceProvider = BuildServiceProvider(mockMediator.Object);
            var stubLogger = Mock.Of<ILogger<KafkaTopicMessageConsumer<FakeAvro, FakeMessage>>>();
            var stubConsumer = new Mock<IConsumer<string, FakeAvro>>();
            stubConsumer
                .Setup(x => x.Consume(It.IsAny<CancellationToken>()))
                .Returns(BuildFakeConsumeResult(fakeMessage));
            var stubMessageConsumerBuilder = new Mock<IKafkaConsumerBuilder<FakeAvro>>();
            stubMessageConsumerBuilder
                .Setup(x => x.Build())
                .Returns(stubConsumer.Object);

            //Act
            var sut = new KafkaTopicMessageConsumer<FakeAvro, FakeMessage>(stubLogger, stubMessageConsumerBuilder.Object, serviceProvider, stubMapper);
            //Running inside another thread to use "cancellationTokenSource.Cancel()"
            Task.Run(() => sut.StartConsuming("fake-messages", cancellationTokenSource.Token));
            Task.Delay(500).Wait();
            cancellationTokenSource.Cancel();

            //Assert
            mockMediator.Verify(x =>
                x.Publish(
                    It.Is<object>(i => i.GetType() == typeof(MessageNotification<FakeMessage>)),
                    It.IsAny<CancellationToken>()));
        }

        private static ServiceProvider BuildServiceProvider(IMediator mediator)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped(s => mediator);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        private static ConsumeResult<string, FakeAvro> BuildFakeConsumeResult(FakeMessage fakeMessage)
        {
            return new ConsumeResult<string, FakeAvro>
            {
                Message = new Message<string, FakeAvro>
                {
                    Value = fakeMessage,
                    Headers = new Headers
                    {
                        { "message-type", Encoding.UTF8.GetBytes(fakeMessage.GetType().AssemblyQualifiedName) }
                    }
                }
            };
        }
    }
}
