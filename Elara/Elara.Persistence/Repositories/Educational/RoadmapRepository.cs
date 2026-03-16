using Elara.Application.Contracts.Persistence.Educational;
using Elara.Domain.Entities.Educational;
using Elara.Infrastructure.Data;

namespace Elara.Persistence.Repositories.Educational
{
    public class RoadmapRepository : BaseRepository<Roadmap, int>, IRoadmapRepository
    {
        public RoadmapRepository(AppDbContext context) : base(context)
        {
        }
    }
}
