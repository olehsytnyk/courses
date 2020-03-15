using AutoMapper;
using STP.Markets.Application;
using STP.Markets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STP.Markets.API {
    public class MarketsProfile : Profile {
        public MarketsProfile() {
            CreateMap<Market, MarketDto>().ReverseMap();
            CreateMap<Market, MarketInnerDto>().ReverseMap();

            CreateMap<Watchlist, WatchlistDto>();
            CreateMap<Watchlist, WatchlistPatchDto>();
            CreateMap<Watchlist, WatchlistPostDto>();

            CreateMap<Watchlist, WatchlistWithMarketsDto>()
                .IncludeAllDerived()
                .ForMember(w => w.MarketsId,
                    config => config.MapFrom(
                        src => src.MarketWatchlists.Select(mw => mw.MarketId)
                        )
                    );


            CreateMap<WatchlistBaseDto, Watchlist>();

            CreateMap<WatchlistWithMarketsDto, Watchlist>().IncludeBase<WatchlistBaseDto, Watchlist>()
                .ForMember(w => w.MarketWatchlists, conf => conf.MapFrom(opt => opt.MarketsId.Select(id => new MarketWatchlist { MarketId = id})));

            CreateMap<WatchlistPostDto, Watchlist>().IncludeBase<WatchlistWithMarketsDto, Watchlist>();
            CreateMap<WatchlistDto, Watchlist>().IncludeBase<WatchlistWithMarketsDto, Watchlist>();
            CreateMap<WatchlistDtoI, WatchlistDto>();
            CreateMap<WatchlistDto, WatchlistDtoI>();
            CreateMap<WatchlistPatchDto, Watchlist>().IncludeBase<WatchlistBaseDto, Watchlist>();
        }
    }
}
