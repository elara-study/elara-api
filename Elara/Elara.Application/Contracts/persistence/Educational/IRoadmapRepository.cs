using Elara.Domain.Entities.Educational;

namespace Elara.Application.Contracts.Persistence.Educational
{
    public interface IRoadmapRepository : IAsyncRepository<Roadmap, int>
    {
    }
}
