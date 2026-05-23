using Elara.Application.Models.Users;
using Elara.Domain.Entities.Users;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Entities.Administrative;

namespace Elara.Application.Contracts.Persistence.Users
{
    public interface IStudentRepository : IAsyncRepository<Student, Guid>
    {
        Task<StudentProfileReadModel?> GetStudentProfileAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<Guid?> GetStudentIdByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Student>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Guid>> GetParentIdsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<bool> IsParentOfStudentAsync(Guid parentId, Guid studentId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Guid>> GetTeacherIdsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Student>> GetTopStudentsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<int> GetStudentRankAsync(Guid studentId, int studentTotalXp, DateTime studentCreatedAt, CancellationToken cancellationToken = default);
        Task<Student?> GetStudentWithAchievementsAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<Dictionary<Guid, string>> GetStudentNamesAsync(IEnumerable<Guid> studentIds, CancellationToken cancellationToken = default);
        Task<int> GetMasteredSubjectsCountAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<int> GetPerfectDaysStreakAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<LatestQuizSessionReadModel?> GetLatestQuizSessionWithTopicAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<int> GetTotalLessonsInTopicAsync(int topicId, CancellationToken cancellationToken = default);
        Task<int> GetCompletedLessonsInTopicAsync(Guid studentId, int topicId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TodayQuizSessionReadModel>> GetTodayQuizSessionsAsync(Guid studentId, DateTime todayStart, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<EnrolledClassReadModel>> GetStudentEnrolledClassesAsync(Guid studentId, CancellationToken cancellationToken = default);
    }
}
