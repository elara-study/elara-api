using Elara.Application.Models.Users;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Entities.Users;

namespace Elara.Application.Contracts.Persistence.Users
{
    public interface ITeacherRepository : IAsyncRepository<Teacher,Guid>
    {
        Task<(Teacher teacher, Roadmap? roadmap)> GetTeacherWithSubjectAndRoadmapAsync(Guid teacherId, string? roadmapName = null, CancellationToken cancellationToken = default);
        Task<Teacher?> GetTeacherWithStudentsAsync(Guid teacherId, CancellationToken cancellationToken = default);
        Task<TeacherProfileReadModel?> GetTeacherProfileAsync(Guid teacherId, CancellationToken cancellationToken = default);
        Task<List<Class>> GetClassesByTeacherAsync(Guid teacherId, CancellationToken cancellationToken = default);
        Task<List<Roadmap>> GetRoadmapsByTeacherAsync(Guid teacherId, CancellationToken cancellationToken = default);
        Task<double> GetAvgCompletionByTeacherAsync(Guid teacherId, CancellationToken cancellationToken = default);
        Task<List<StudentSubmission>> GetPendingSubmissionsAsync(Guid teacherId, int take, CancellationToken cancellationToken = default);
        Task<List<StudentSubmission>> GetRecentSubmissionsAsync(Guid teacherId, int take, CancellationToken cancellationToken = default);
    }
}