namespace Consumer.Api.Models
{
    public class MessageToShowModel
    {
        public required string Key { get; set; }
        public required string Value { get; set; }
        public required DateTime Timestamp { get; set; }

        public required string Topic { get; set; }
        public required int Partition { get; set; }
        public required long Offset { get; set; }
    }
}