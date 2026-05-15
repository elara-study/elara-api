using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Rewards
{
    public class AchievementEvaluationService : IAchievementEvaluationService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IAsyncRepository<Achievement, int> _achievementRepository;
        private readonly IAsyncRepository<QuizSession, int> _quizSessionRepository;

        public AchievementEvaluationService(
            IStudentRepository studentRepository,
            IAsyncRepository<Achievement, int> achievementRepository,
            IAsyncRepository<QuizSession, int> quizSessionRepository)
        {
            _studentRepository = studentRepository;
            _achievementRepository = achievementRepository;
            _quizSessionRepository = quizSessionRepository;
        }

        public async Task EvaluateStudentAchievementsAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            // Eagerly load the student WITH their existing achievements
            var student = await _studentRepository.GetStudentWithAchievementsAsync(studentId, cancellationToken);
            if (student == null) return;

            var existingAchievements = student.StudentAchievements?.ToList() ?? new System.Collections.Generic.List<StudentAchievement>();

            var allAchievements = await _achievementRepository.ListAllAsync(cancellationToken);
            var completedSessionsCount = await _quizSessionRepository.AsQueryable()
                .CountAsync(s => s.StudentId == studentId && s.Status == QuizSessionStatus.Completed, cancellationToken);
            
            var hasPerfectScore = await _quizSessionRepository.AsQueryable()
                .AnyAsync(s => s.StudentId == studentId && 
                               s.Status == QuizSessionStatus.Completed && 
                               s.WrongAnswers == 0 && s.CorrectAnswers > 0,
                cancellationToken);

            bool newAchievementEarned = false;

            foreach (var achievement in allAchievements)
            {
                if (existingAchievements.Any(ea => ea.AchievementId == achievement.Id))
                    continue; // Already earned

                bool qualifies = false;

                switch (achievement.AchievementType)
                {
                    case AchievementType.TotalXP:
                        qualifies = student.TotalXP >= achievement.TargetValue;
                        break;
                    case AchievementType.Streak:
                        qualifies = student.CurrentStreak >= achievement.TargetValue;
                        break;
                    case AchievementType.LessonsCompleted:
                        qualifies = completedSessionsCount >= achievement.TargetValue;
                        break;
                    case AchievementType.SpecificQuizScore:
                        qualifies = hasPerfectScore;
                        break;
                }

                if (qualifies)
                {
                    if (student.StudentAchievements == null)
                        student.StudentAchievements = new System.Collections.Generic.List<StudentAchievement>();

                    student.StudentAchievements.Add(new StudentAchievement
                    {
                        AchievementId = achievement.Id,
                        StudentId = student.Id,
                        EarnedAt = DateTime.UtcNow
                    });

                    newAchievementEarned = true;
                }
            }

            if (newAchievementEarned)
            {
                await _studentRepository.UpdateAsync(student, cancellationToken);
            }
        }
    }
}
