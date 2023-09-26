namespace Consumer.Api.KafkaConsumerConfig
{
    public class ConsumerConfigOptions
    {
        public required string TopicName { get; set; }

        public required bool EnableAutoCommit { get; set; }

        public required string BootstrapServers { get; set; }

        public required string GroupId { get; set; }
    }
}