using AutoMapper;
using STP.Common.Extensions;
using STP.Identity.Domain.DTOs.User;
using STP.Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Identity.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<CreateUserDto, User>();

            CreateMap<UserDto, UserDtoI>();

            CreateMap<User, UserDto>()
                .ForMember(d => d.DateOfBirth, w => w.MapFrom(u => u.DateOfBirth == null ? (long?)null : UtcToUnixConverter.Convert((DateTime)u.DateOfBirth)));

            CreateMap<UpdateUserDto, User>()
                .ForMember(d => d.DateOfBirth, w => w.MapFrom(u => u.DateOfBirth == null ? (DateTime?)null : UnixToUtcConverter.Convert((long)u.DateOfBirth)));

            CreateMap<User, UpdateUserDto>()
                .ForMember(d => d.DateOfBirth, w => w.MapFrom(u => u.DateOfBirth == null ? (long?)null : UtcToUnixConverter.Convert((DateTime)u.DateOfBirth)));
        }
    }
}
