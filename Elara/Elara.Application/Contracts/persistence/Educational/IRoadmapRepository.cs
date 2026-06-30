using Elara.Domain.Entities.Educational;

namespace Elara.Application.Contracts.Persistence.Educational
{
    public interface IRoadmapRepository : IAsyncRepository<Roadmap, int>
    {
        Task<Roadmap?> GetRoadmapWithDetailsAsync(int roadmapId, CancellationToken cancellationToken = default);
        Task<Roadmap?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default);
    }
}
