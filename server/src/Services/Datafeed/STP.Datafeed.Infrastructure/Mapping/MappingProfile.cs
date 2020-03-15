using AutoMapper;
using STP.Common.Extensions;
using STP.Common.Models;
using STP.Datafeed.Application.Models;

namespace STP.Datafeed.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MarketUpdateDto, MarketUpdate>()
                .ReverseMap();

            CreateMap<MarketInnerDto, Market>()
               .ForMember(m => m.MarketId, opt => opt.MapFrom(src => src.Id));

            /*  CreateMap<MarketUpdateDto, MarketUpdate>()
                 .ForMember(m => m.Timestamp, opt => opt.ConvertUsing(new UnixToUtcConverter(), src => src.Timestamp));*/

            CreateMap<MarketUpdate, MarketUpdateDto>()
              .ForMember(m => m.Timestamp, opt => opt.ConvertUsing(new UtcToUnixConverter(), src => src.Timestamp));

            CreateMap<MarketUpdate, MarketUpdateDto>()
            .ForMember(m => m.Timestamp, opt => opt.ConvertUsing(new UtcToUnixConverter(), src => src.Timestamp));
        }
    }
}
