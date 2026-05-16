using Elara.Domain.Entities.Users;

namespace Elara.Application.Contracts.Persistence.Users
{
    public interface IStudentRepository : IAsyncRepository<Student, Guid>
    {
        Task<Guid?> GetStudentIdByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Student>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Guid>> GetTeacherIdsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Student>> GetTopStudentsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<int> GetStudentRankAsync(Guid studentId, int studentTotalXp, DateTime studentCreatedAt, CancellationToken cancellationToken = default);
        Task<Student?> GetStudentWithAchievementsAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<Dictionary<Guid, string>> GetStudentNamesAsync(IEnumerable<Guid> studentIds, CancellationToken cancellationToken = default);
    }
}
