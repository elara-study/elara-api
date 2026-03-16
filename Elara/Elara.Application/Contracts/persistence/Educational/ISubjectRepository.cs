using Elara.Domain.Entities.Educational;

namespace Elara.Application.Contracts.Persistence.Educational
{
    public interface ISubjectRepository : IAsyncRepository<Subject, int>
    {
        Task<bool> ExistsAsync(int subjectId, CancellationToken cancellationToken = default);
    }
}
