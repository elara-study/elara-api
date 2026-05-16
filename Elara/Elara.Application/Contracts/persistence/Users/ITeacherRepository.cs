using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Users;

namespace Elara.Application.Contracts.Persistence.Users
{
    public interface ITeacherRepository : IAsyncRepository<Teacher,Guid>
    {
        Task<(Teacher teacher, Roadmap? roadmap)> GetTeacherWithSubjectAndRoadmapAsync(Guid teacherId, string? roadmapName = null, CancellationToken cancellationToken = default);
        Task<Teacher?> GetTeacherWithStudentsAsync(Guid teacherId, CancellationToken cancellationToken = default);
    }
}