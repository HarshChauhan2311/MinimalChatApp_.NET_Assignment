

using AutoMapper;
using MinimalChatApp.DTO;
using MinimalChatApp.Entity;

namespace MinimalChatApp.DAL.Data
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<UserDto, ApplicationUser>();
        }
    }
}
