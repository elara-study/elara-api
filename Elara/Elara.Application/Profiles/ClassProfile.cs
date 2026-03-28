using AutoMapper;
using Elara.Application.Features.Users.Teachers.Queries.GetClassInfo;
using Elara.Application.Features.Users.Teachers.Queries.GetTeacherClasses;
using Elara.Domain.Entities.Administrative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Profiles
{
    public class ClassProfile:Profile
    {
        public ClassProfile()
        {
            CreateMap<Class, GetClassInfoResponse>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.PublicId)
                )
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => src.ClassName)
                )
                .ForMember(
                    dest => dest.Subject,
                    opt => opt.MapFrom(src => src.Subject.Name)
                )
                .ForMember(
                    dest => dest.Grade,
                    opt => opt.MapFrom(src => (int)src.Level)
                );
                


            CreateMap<Class, GetTeacherClassesResponse>()
              .ForMember(d => d.Id, o => o.MapFrom(s => s.PublicId))
              .ForMember(d => d.Name, o => o.MapFrom(s => s.ClassName))
              .ForMember(d => d.Subject, o => o.MapFrom(s => s.Subject.Name))
              .ForMember(d => d.Grade, o => o.MapFrom(s => (int)s.Level))
              .ForMember(d => d.StudentsCount,
                o => o.MapFrom(s => s.StudentClasses.Count));
        }
    }
}
