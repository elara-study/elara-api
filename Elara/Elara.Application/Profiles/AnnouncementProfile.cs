using AutoMapper;
using Elara.Application.Features.Users.Teachers.Commands.AddAnnouncement;
using Elara.Domain.Entities.Administrative;

namespace Elara.Application.Profiles
{
    public class AnnouncementProfile : Profile
    {
        public AnnouncementProfile()
        {
            CreateMap<AddAnnouncementCommand, Announcement>()
                .ForMember(dest => dest.ClassId, opt => opt.Ignore());

            CreateMap<Announcement, AddAnnouncementResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PublicId));
        }
    }
}
