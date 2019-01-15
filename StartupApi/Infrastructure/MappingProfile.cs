using System;
using AutoMapper;
using StartupApi.Controllers;
using StartupApi.Model;

namespace StartupApi.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppDataEntity, AppData>()
                .ForMember(dest => dest.Self, opt =>
                   opt.MapFrom(src => Link.To(nameof(AppDataController.GetDataById), new { appId = src.Id })));

            CreateMap<UserEntity, User>()
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src => Link.To(nameof(UsersController.GetUserById),
                new { userId = src.Id })));


        }
    }
}
