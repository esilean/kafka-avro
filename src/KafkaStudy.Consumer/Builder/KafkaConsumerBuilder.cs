using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaStudy.Common;
using KafkaStudy.Consumer.Builder.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace KafkaStudy.Consumer.Builder
{
    public class KafkaConsumerBuilder<TAvro> : IKafkaConsumerBuilder<TAvro>
    {
        private readonly KafkaOptions _kafkaOptions;
        private readonly ILogger<KafkaConsumerBuilder<TAvro>> _logger;

        public KafkaConsumerBuilder(ILogger<KafkaConsumerBuilder<TAvro>> logger, IOptions<KafkaOptions> kafkaOptions)
        {
            _kafkaOptions = kafkaOptions?.Value ?? throw new ArgumentNullException(nameof(kafkaOptions));
            _logger = logger;
        }

        public IConsumer<string, TAvro> Build()
        {
            var consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = _kafkaOptions.BootstrapServers,
                GroupId = _kafkaOptions.ConsumerGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                // Note: you can specify more than one schema registry url using the
                // schema.registry.url property for redundancy (comma separated list). 
                // The property name is not plural to follow the convention set by
                // the Java implementation.
                Url = _kafkaOptions.SchemaRegistryUrl
            };

            var avroSerializerConfig = new AvroSerializerConfig
            {
                // optional Avro serializer properties:
                BufferBytes = 100
            };

            var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

            var consumerBuilder =
                                new ConsumerBuilder<string, TAvro>(consumerConfig)
                                    .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                                    .SetValueDeserializer(new AvroDeserializer<TAvro>(schemaRegistry).AsSyncOverAsync())
                                    .SetErrorHandler((_, e) =>
                                    {
                                        _logger.LogError(e.Reason);
                                    });

            return consumerBuilder.Build();
        }
    }
}
