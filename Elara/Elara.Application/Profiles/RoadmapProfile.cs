using AutoMapper;
using Elara.Application.Features.Users.Teachers.Commands.CreateRoadmap;
using Elara.Domain.Entities.Educational;

namespace Elara.Application.Profiles
{
    public class RoadmapProfile : Profile
    {
        public RoadmapProfile()
        {
            CreateMap<Roadmap, CreateRoadmapResponse>()
                .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => (int)src.Grade));
        }
    }
}
