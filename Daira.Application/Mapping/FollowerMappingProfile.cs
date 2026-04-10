using AutoMapper;
using Daira.Application.Response.FollowerModule;
using Daira.Domain.Entities;

namespace Daira.Application.Mapping
{
    public class FollowerMappingProfile : Profile
    {
        public FollowerMappingProfile()
        {
            CreateMap<FollowerResponse, Follower>().ReverseMap();
        }
    }
}
