using AutoMapper;
using Consumer.Api.KafkaConsumerConfig;

namespace Consumer.Api.AutoMapperProfiles
{
    public class ConsumerConfigProfile : Profile
    {
        public ConsumerConfigProfile()
        {
            CreateMap<ConsumerConfigOptions, Confluent.Kafka.ConsumerConfig>();
        }
    }
}