using STP.Profile.Domain.DTO.Order;
using STP.Profile.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using STP.Profile.Domain.DTO.Position;
using STP.Common.Extensions;
using STP.Profile.Domain.DTO.TraderInfo;

namespace STP.Profile.Infrastructure
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderEntity, GetOrderDTO>()
                .ForMember(dto => dto.Timestamp, opt => opt.ConvertUsing(new UtcToUnixConverter()));

            CreateMap<PositionEntity, GetPositionDTO>()
                .ForMember(dto => dto.Timestamp, opt => opt.ConvertUsing(new UtcToUnixConverter()));

            CreateMap<PostOrderDTO, OrderEntity>();

            CreateMap<TraderInfoEntity, BaseTraderInfoDTO>();

            CreateMap<TraderInfoEntity, GetTraderInfoDTO>()
                .IncludeBase<TraderInfoEntity, BaseTraderInfoDTO>()
                .ForMember(dto => dto.LastChanged, opt => opt.ConvertUsing(new UtcToUnixConverter()));
        }
    }
}
