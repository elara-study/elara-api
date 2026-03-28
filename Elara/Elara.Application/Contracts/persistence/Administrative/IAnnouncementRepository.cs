using Elara.Application.Features.Users.Teachers.Queries.GetAnnouncements;
using Elara.Domain.Entities.Administrative;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elara.Application.Contracts.Persistence.Administrative
{
    public interface IAnnouncementRepository : IAsyncRepository<Announcement, int>
    {
        Task<List<Announcement>> GetByClassPublicIdAsync(Guid classPublicId, CancellationToken cancellationToken = default);
        Task<List<GetAnnouncementsResponse>> GetByClassPublicIdProjectedAsync(Guid classPublicId, CancellationToken cancellationToken = default);
    }
}
