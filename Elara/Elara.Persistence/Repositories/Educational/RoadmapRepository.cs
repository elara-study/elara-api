using Elara.Application.Contracts.Persistence.Educational;
using Elara.Domain.Entities.Educational;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Persistence.Repositories.Educational
{
    public class RoadmapRepository : BaseRepository<Roadmap, int>, IRoadmapRepository
    {
        public RoadmapRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<Roadmap?> GetRoadmapWithDetailsAsync(int roadmapId, CancellationToken cancellationToken = default)
        {
            return await _context.Roadmaps
                .Where(r => r.Id == roadmapId && !r.IsDeleted)
                .Include(r => r.Subject)
                .Include(r => r.Modules)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Roadmap?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default)
        {
            return await _context.Roadmaps
                .Where(r => r.PublicId == publicId && !r.IsDeleted)
                .Include(r => r.Subject)
                .Include(r => r.Modules)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
