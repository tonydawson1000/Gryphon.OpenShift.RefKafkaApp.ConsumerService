using AutoMapper;
using Consumer.Api.Models;

namespace Consumer.Api.AutoMapperProfiles
{
    public class ConsumeResultProfile : Profile
    {
        public ConsumeResultProfile()
        {
            CreateMap<Confluent.Kafka.ConsumeResult<String, String>, MessageToShowModel>()
                .ForMember(x => x.Key, opt => opt.MapFrom(o => o.Message.Key))
                .ForMember(x => x.Value, opt => opt.MapFrom(o => o.Message.Value))
                .ForMember(x => x.Timestamp, opt => opt.MapFrom(o => o.Message.Timestamp.UtcDateTime));
        }
    }
}