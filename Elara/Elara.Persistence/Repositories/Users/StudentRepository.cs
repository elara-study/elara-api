using Elara.Application.Contracts.Persistence.Users;
using Elara.Application.Models.Users;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Entities.Users;
using Elara.Domain.Enums;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static Elara.Application.Common.Constants.DailyGoalConstants;


namespace Elara.Persistence.Repositories.Users
{
    public class StudentRepository : BaseRepository<Student, Guid>, IStudentRepository
    {
        public StudentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Guid?> GetStudentIdByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var query = from u in _context.Users
                        join s in _context.Students on u.Id equals s.Id
                        where u.Username == username
                        select u.Id;

            var result = await query.Cast<Guid?>().FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task<IReadOnlyList<Student>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentParents
                .Where(sp => sp.ParentId == parentId)
                .Select(sp => sp.Student)
                .Where(s => !s.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Guid>> GetParentIdsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentParents
                .Where(sp => sp.StudentId == studentId)
                .Select(sp => sp.ParentId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsParentOfStudentAsync(Guid parentId, Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentParents
                .AnyAsync(sp => sp.ParentId == parentId && sp.StudentId == studentId, cancellationToken);
        }

        public async Task<IReadOnlyList<Guid>> GetTeacherIdsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Elara.Domain.Entities.JunctionTables.StudentTeacher>()
                .Where(st => st.StudentId == studentId)
                .Select(st => st.TeacherId)
                .ToListAsync(cancellationToken);
        }
            
        public async Task<Student?> GetStudentWithAchievementsAsync(Guid studentId, CancellationToken cancellationToken)
        {
            return await _context.Students
                .Include(s => s.StudentAchievements)
                .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
        }

        public async Task<StudentProfileReadModel?> GetStudentProfileAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            var studentProfile = await (
                from student in _context.Students
                join user in _context.Users on student.Id equals user.Id
                where student.Id == studentId && !student.IsDeleted
                select new StudentProfileReadModel
                {
                    StudentId = student.Id,
                    Username = user.Username,
                    FullName = string.IsNullOrWhiteSpace(user.Name) ? user.Username : user.Name,
                    AvatarUrl = user.ImageUrl,
                    GradeLevel = student.GradeLevel,
                    TotalXP = student.TotalXP,
                    CurrentStreak = student.CurrentStreak
                }).FirstOrDefaultAsync(cancellationToken);

            if (studentProfile == null)
            {
                return null;
            }

            studentProfile.Parents = await (
                from link in _context.StudentParents
                join parentEntity in _context.Parents on link.ParentId equals parentEntity.Id
                join parentUser in _context.Users on parentEntity.Id equals parentUser.Id
                where link.StudentId == studentId && !parentEntity.IsDeleted
                orderby parentUser.Name, parentUser.Username
                select new ParentProfileReadModel
                {
                    Id = parentEntity.Id,
                    FullName = string.IsNullOrWhiteSpace(parentUser.Name) ? parentUser.Username : parentUser.Name,
                    AvatarUrl = parentUser.ImageUrl
                }).ToListAsync(cancellationToken);

            studentProfile.RecentAchievements = await _context.Set<StudentAchievement>()
                .Where(sa => sa.StudentId == studentId)
                .OrderByDescending(sa => sa.EarnedAt)
                .Take(3)
                .Select(sa => new StudentAchievementReadModel
                {
                    Id = sa.AchievementId,
                    Title = sa.Achievement.Title
                })
                .ToListAsync(cancellationToken);

            return studentProfile;
        }

        public async Task<IReadOnlyList<Student>> GetTopStudentsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var validatedPage = Math.Max(1, page);
            var validatedPageSize = Math.Max(1, pageSize);

            return await _context.Students
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.TotalXP)
                .ThenBy(s => s.CreatedAt)
                .Skip((validatedPage - 1) * validatedPageSize)
                .Take(validatedPageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetStudentRankAsync(Guid studentId, int studentTotalXp, DateTime studentCreatedAt, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Where(s => s.IsActive
                     && s.Id != studentId
                     && (s.TotalXP > studentTotalXp
                     || (s.TotalXP == studentTotalXp 
                     && s.CreatedAt < studentCreatedAt)))
                .CountAsync(cancellationToken) + 1;
        }

        public async Task<Dictionary<Guid, string>> GetStudentNamesAsync(IEnumerable<Guid> studentIds, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Where(u => studentIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => string.IsNullOrWhiteSpace(u.Name) ? u.Username : u.Name, cancellationToken);
        }

        public async Task<int> GetMasteredSubjectsCountAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.QuizSessions
                .Where(s => s.StudentId == studentId && s.Status == QuizSessionStatus.Completed)
                .Select(s => s.Assignment.Topic.SubjectId)
                .Distinct()
                .CountAsync(cancellationToken);
        }

        public async Task<int> GetPerfectDaysStreakAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            var sevenDaysAgo = DateTime.UtcNow.Date.AddDays(-7);
            
            var recentSessions = await _context.QuizSessions
                .Where(s => s.StudentId == studentId && s.Status == QuizSessionStatus.Completed && s.CompletedAt >= sevenDaysAgo)
                .ToListAsync(cancellationToken);

            var sessionsByDate = recentSessions
                .Where(s => s.CompletedAt.HasValue)
                .GroupBy(s => s.CompletedAt.Value.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            int streak = 0;
            var checkDate = DateTime.UtcNow.Date;

            // Check if today is a perfect day
            bool isTodayPerfect = false;
            if (sessionsByDate.TryGetValue(checkDate, out var todaySessions))
            {
                bool has3Lessons = todaySessions.Count >= Elara.Application.Common.Constants.DailyGoalConstants.LessonsTarget;
                bool has80PercentScore = todaySessions.Any(s => 
                {
                    var totalQuestions = s.CorrectAnswers + s.WrongAnswers + s.UnansweredCount;
                    return totalQuestions > 0 && ((double)s.CorrectAnswers / totalQuestions) >= Elara.Application.Common.Constants.DailyGoalConstants.ScoreTarget;
                });
                var totalDurationMinutes = todaySessions.Sum(s => (s.CompletedAt!.Value - s.StartedAt).TotalMinutes);
                bool has15MinsPractice = totalDurationMinutes >= Elara.Application.Common.Constants.DailyGoalConstants.PracticeMinutesTarget;

                isTodayPerfect = has3Lessons && has80PercentScore && has15MinsPractice;
            }

            if (isTodayPerfect)
            {
                streak++;
            }

            var remainingDaysToCheck = isTodayPerfect ? 6 : 7;

            // Always check previous days starting from yesterday
            checkDate = DateTime.UtcNow.Date.AddDays(-1);

            for (int i = 0; i < remainingDaysToCheck; i++)
                if (sessionsByDate.TryGetValue(checkDate, out var daySessions))
                {
                    bool has3Lessons = daySessions.Count >= LessonsTarget;
                    
                    bool has80PercentScore = daySessions.Any(s => 
                    {
                        var totalQuestions = s.CorrectAnswers + s.WrongAnswers + s.UnansweredCount;
                        return totalQuestions > 0 && ((double)s.CorrectAnswers / totalQuestions) >= ScoreTarget;
                    });
                    
                    var totalDurationMinutes = daySessions.Sum(s => (s.CompletedAt!.Value - s.StartedAt).TotalMinutes);
                    bool has15MinsPractice = totalDurationMinutes >= PracticeMinutesTarget;

                    if (has3Lessons && has80PercentScore && has15MinsPractice)
                    {
                        streak++;
                        checkDate = checkDate.AddDays(-1);
                    }
                    else
                    {
                        break; // Streak broken
                    }
                }
                else
                {
                    break; // Streak broken
                }
            }

            return streak;
        }

        public async Task<LatestQuizSessionReadModel?> GetLatestQuizSessionWithTopicAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.QuizSessions
                .Where(s => s.StudentId == studentId && !s.IsDeleted)
                .OrderByDescending(s => s.StartedAt)
                .Select(s => new LatestQuizSessionReadModel
                {
                    TopicId = s.Assignment.TopicId,
                    TopicTitle = s.Assignment.Topic.Title,
                    AssignmentTitle = s.Assignment.Title
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> GetTotalLessonsInTopicAsync(int topicId, CancellationToken cancellationToken = default)
        {
            return await _context.Lessons
                .CountAsync(l => l.TopicId == topicId && !l.IsDeleted, cancellationToken);
        }

        public async Task<int> GetCompletedLessonsInTopicAsync(Guid studentId, int topicId, CancellationToken cancellationToken = default)
        {
            return await _context.QuizSessions
                .Where(s => s.StudentId == studentId 
                         && s.Status == QuizSessionStatus.Completed 
                         && s.Assignment.TopicId == topicId
                         && s.Assignment.LessonId != null
                         && !s.IsDeleted)
                .Select(s => s.Assignment.LessonId)
                .Distinct()
                .CountAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TodayQuizSessionReadModel>> GetTodayQuizSessionsAsync(Guid studentId, DateTime todayStart, CancellationToken cancellationToken = default)
        {
            return await _context.QuizSessions
                .Where(s => s.StudentId == studentId && s.StartedAt >= todayStart && !s.IsDeleted)
                .Select(s => new TodayQuizSessionReadModel
                {
                    Status = s.Status,
                    CorrectAnswers = s.CorrectAnswers,
                    WrongAnswers = s.WrongAnswers,
                    UnansweredCount = s.UnansweredCount,
                    StartedAt = s.StartedAt,
                    CompletedAt = s.CompletedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<EnrolledClassReadModel>> GetStudentEnrolledClassesAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Where(sc => sc.StudentId == studentId && sc.IsActive && !sc.IsDeleted && !sc.Class.IsDeleted)
                .Select(sc => new EnrolledClassReadModel
                {
                    PublicId = sc.Class.PublicId,
                    ClassName = sc.Class.ClassName
                })
                .ToListAsync(cancellationToken);
        }
    }
}
