namespace Elara.Application.Common.Interfaces
{
    public interface IAchievementEvaluationService
    {
        Task EvaluateStudentAchievementsAsync(Guid studentId, CancellationToken cancellationToken = default);
    }
}
