using AutoMapper;
using Daira.Application.DTOs.CommentModule;
using Daira.Domain.Entities;

namespace Daira.Application.Mapping
{
    public class CommentMappingProfile : Profile
    {
        public CommentMappingProfile()
        {
            CreateMap<Comment, CommentResponse>().ReverseMap();
        }
    }
}
