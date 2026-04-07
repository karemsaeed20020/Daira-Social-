using AutoMapper;
using Daira.Application.Response.LikeModule;
using Daira.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daira.Application.Mapping
{
    public class LikeMappingProfile : Profile
    {
        public LikeMappingProfile()
        {
            CreateMap<LikeResponse, Like>().ReverseMap();
        }
    }
}
