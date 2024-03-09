using AutoMapper;
using HutEntry.Data.Dtos;
using HutEntry.Models;

namespace HutEntry.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, User>();
        }
    }
}
