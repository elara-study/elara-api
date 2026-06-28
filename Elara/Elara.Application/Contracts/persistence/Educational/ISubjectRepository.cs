using Elara.Domain.Entities.Educational;

namespace Elara.Application.Contracts.Persistence.Educational
{
    public interface ISubjectRepository : IAsyncRepository<Subject, int>
    {
        Task<Subject?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
