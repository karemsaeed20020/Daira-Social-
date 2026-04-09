using AutoMapper;
using Daira.Application.Response.FriendshipModule;
using Daira.Domain.Entities;

namespace Daira.Application.Mapping
{
    public class FriendshipMappingProfile : Profile
    {
        public FriendshipMappingProfile()
        {
            CreateMap<FriendshipResponse, Friendship>().ReverseMap();
        }
    }
}
