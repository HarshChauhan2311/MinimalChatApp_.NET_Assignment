using AutoMapper;
using MinimalChatApp.MinimalChatApp.DTOs;
using MinimalChatApp.Models;

namespace MinimalChatApp.MinimalChatApp.Data
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}
