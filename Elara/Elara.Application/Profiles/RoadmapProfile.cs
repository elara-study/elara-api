using AutoMapper;
using Elara.Application.Features.Users.Teachers.Commands.CreateRoadmap;
using Elara.Application.Features.Users.Teachers.Queries.GetTeacherRoadmaps;
using Elara.Domain.Entities.Educational;

namespace Elara.Application.Profiles
{
    public class RoadmapProfile : Profile
    {
        public RoadmapProfile()
        {
            CreateMap<Roadmap, CreateRoadmapResponse>()
                .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => (int)src.Grade));

            CreateMap<Roadmap, TeacherRoadmapListDto>()
                .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => (int)src.Grade))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Name : string.Empty))
                .ForMember(dest => dest.ModulesCount, opt => opt.MapFrom(src => src.Modules != null ? src.Modules.Count : 0));

            CreateMap<Roadmap, TeacherRoadmapDetailDto>()
                .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => (int)src.Grade))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Name : string.Empty));

            CreateMap<Module, RoadmapModuleDto>();
        }
    }
}
