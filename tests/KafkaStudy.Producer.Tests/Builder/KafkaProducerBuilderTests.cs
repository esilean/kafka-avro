using Confluent.Kafka;
using KafkaStudy.Common;
using KafkaStudy.Common.Tests.Fake;
using KafkaStudy.Producer.Builder;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace KafkaStudy.Producer.Tests.Builder
{
    public class KafkaProducerBuilderTests
    {
        [Fact(DisplayName = "Constructor - Deve criar um ProducerBuilder")]
        public void Constructor_ShouldCreateSampleProducerBuilder()
        {
            //Arrange
            var kafkaOptions = Options.Create(new KafkaOptions());

            //Act
            var sut = new KafkaProducerBuilder<FakeAvro>(kafkaOptions);

            //Assert
            Assert.IsType<KafkaProducerBuilder<FakeAvro>>(sut);
        }

        [Fact(DisplayName = "Constructor - Deve lançar exceção se config null")]
        public void Constructor_ShouldThrowIfOptionsIsNull()
        {
            //Arrange
            IOptions<KafkaOptions> kafkaOptions = null;

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => new KafkaProducerBuilder<FakeAvro>(kafkaOptions));
        }

        [Fact(DisplayName = "Build - Deve retornar um builder")]
        public void Build_ShouldReturnNonNullProducer()
        {
            //Arrange
            var options = new KafkaOptions
            {
                BootstrapServers = "broker",
                Acks = Acks.All,
                EnableIdempotence = true,
                MessageSendMaxRetries = 3,
                MaxInFlight = 5,
                CompressionType = CompressionType.Snappy,
                LingerMs = 10,
                BatchSizeKB = 1024,
                SchemaRegistryUrl = "url"
            };
            var kafkaOptions = Options.Create(options);
            var sut = new KafkaProducerBuilder<FakeAvro>(kafkaOptions);

            //Act
            var producer = sut.Build();

            //Assert
            Assert.NotNull(producer);
        }
    }
}
