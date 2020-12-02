using KafkaStudy.Common;
using KafkaStudy.Common.Tests.Fake;
using KafkaStudy.Consumer.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;

namespace KafkaStudy.Consumer.Tests.Builder
{
    public class KafkaConsumerBuilderTests
    {
        private readonly ILogger<KafkaConsumerBuilder<FakeAvro>> _subLogger;
        public KafkaConsumerBuilderTests()
        {
            _subLogger = Mock.Of<ILogger<KafkaConsumerBuilder<FakeAvro>>>();
        }

        [Fact(DisplayName = "Constructor - Deve criar um builder de consumer")]
        public void Constructor_ShouldCreateSampleConsumerBuilder()
        {
            //Arrange
            var kafkaOptions = Options.Create(new KafkaOptions());

            //Act
            var sut = new KafkaConsumerBuilder<FakeAvro>(_subLogger, kafkaOptions);

            //Assert
            Assert.IsType<KafkaConsumerBuilder<FakeAvro>>(sut);
        }

        [Fact(DisplayName = "Constructor - Deve lançar exceção se config null")]
        public void Constructor_ShouldThrowIfOptionsIsNull()
        {
            //Arrange
            IOptions<KafkaOptions> kafkaOptions = null;

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => new KafkaConsumerBuilder<FakeAvro>(_subLogger, kafkaOptions));
        }

        [Fact(DisplayName = "Build - Deve retornar um builder")]
        public void Build_ShouldReturnNonNullConsumer()
        {
            //Arrange
            var kafkaOptions = Options.Create(new KafkaOptions
            {
                BootstrapServers = "kafka-bootstrap",
                ConsumerGroupId = "test-group-id",
                SchemaRegistryUrl = "url"
            });

            //Act
            var sut = new KafkaConsumerBuilder<FakeAvro>(_subLogger, kafkaOptions);
            var consumer = sut.Build();

            //Assert
            Assert.NotNull(consumer);
        }
    }
}
