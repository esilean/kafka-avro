using Confluent.Kafka;

namespace KafkaStudy.Common
{
    public class KafkaOptions
    {
        public string BootstrapServers { get; set; }
        public string SchemaRegistryUrl { get; set; }

        //Producer
        public Acks Acks { get; set; }
        public bool EnableIdempotence { get; set; }
        public int MessageSendMaxRetries { get; set; }
        public int MaxInFlight { get; set; }
        public CompressionType CompressionType { get; set; }
        public int LingerMs { get; set; }
        public int BatchSizeKB { get; set; }

        //Consumer
        public string ConsumerGroupId { get; set; }
    }
}
