using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Features.Users.Teachers.Queries.GetAnnouncements;
using Elara.Domain.Entities.Administrative;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elara.Persistence.Repositories.Administrative
{
    public class AnnouncementRepository : BaseRepository<Announcement, int>, IAnnouncementRepository
    {
        public AnnouncementRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Announcement>> GetByClassPublicIdAsync(Guid classPublicId, CancellationToken cancellationToken = default)
        {
            return await _context.Announcements
                .AsNoTracking()
                .Where(a => a.Class.PublicId == classPublicId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<GetAnnouncementsResponse>> GetByClassPublicIdProjectedAsync(Guid classPublicId, CancellationToken cancellationToken = default)
        {
            return await _context.Announcements
                .AsNoTracking()
                .Where(a => a.Class.PublicId == classPublicId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new GetAnnouncementsResponse
                {
                    Id = a.PublicId,
                    Title = a.Title,
                    Content = a.Content,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
