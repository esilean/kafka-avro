using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaStudy.Common;
using KafkaStudy.Producer.Builder.Interfaces;
using Microsoft.Extensions.Options;
using System;

namespace KafkaStudy.Producer.Builder
{
    public class KafkaProducerBuilder<TAvro> : IKafkaProducerBuilder<TAvro>
    {
        private readonly KafkaOptions _kafkaOptions;

        public KafkaProducerBuilder(IOptions<KafkaOptions> producerOptions)
        {
            _kafkaOptions = producerOptions?.Value ??
                            throw new ArgumentNullException(nameof(producerOptions));
        }

        public IProducer<string, TAvro> Build()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaOptions.BootstrapServers,
                //producer mais seguro
                Acks = _kafkaOptions.Acks,
                EnableIdempotence = _kafkaOptions.EnableIdempotence,
                MessageSendMaxRetries = _kafkaOptions.MessageSendMaxRetries,
                MaxInFlight = _kafkaOptions.MaxInFlight,

                //melhorar taxa de transferencia
                CompressionType = _kafkaOptions.CompressionType,
                LingerMs = _kafkaOptions.LingerMs,
                BatchSize = _kafkaOptions.BatchSizeKB * 1024
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

            var producerBuilder =
                            new ProducerBuilder<string, TAvro>(config)
                            .SetKeySerializer(new AvroSerializer<string>(schemaRegistry, avroSerializerConfig))
                            .SetValueSerializer(new AvroSerializer<TAvro>(schemaRegistry, avroSerializerConfig));



            return producerBuilder.Build();
        }

    }
}
