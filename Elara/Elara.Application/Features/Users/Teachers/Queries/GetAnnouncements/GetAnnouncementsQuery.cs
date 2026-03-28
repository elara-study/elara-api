using MediatR;
using System;
using System.Collections.Generic;

namespace Elara.Application.Features.Users.Teachers.Queries.GetAnnouncements
{
    public class GetAnnouncementsQuery : IRequest<List<GetAnnouncementsResponse>>
    {
        public Guid ClassId { get; set; }
    }
}
