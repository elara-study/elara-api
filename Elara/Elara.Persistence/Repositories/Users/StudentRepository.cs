using Elara.Application.Contracts.Persistence.Users;
using Elara.Application.Features.Users.Parents.Queries.GetParentChildren;
using Elara.Application.Models.Users;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Entities.Users;
using Elara.Domain.Entities.Submissions;
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
                .Where(sp => sp.ParentId == parentId && sp.Status == StudentParentRelationStatus.Accepted)
                .Select(sp => sp.Student)
                .Where(s => !s.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Guid>> GetParentIdsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentParents
                .Where(sp => sp.StudentId == studentId && sp.Status == StudentParentRelationStatus.Accepted)
                .Select(sp => sp.ParentId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsParentOfStudentAsync(Guid parentId, Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentParents
                .AnyAsync(sp => sp.ParentId == parentId && sp.StudentId == studentId && sp.Status == StudentParentRelationStatus.Accepted, cancellationToken);
        }

        public async Task<IReadOnlyList<Guid>> GetTeacherIdsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<StudentTeacher>()
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
                .Select(s => s.Module.SubjectId)
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

            bool isTodayPerfect = false;
            if (sessionsByDate.TryGetValue(checkDate, out var todaySessions))
            {
                bool has3Lessons = todaySessions.Count >= LessonsTarget;
                bool has80PercentScore = todaySessions.Any(s =>
                {
                    var totalQuestions = s.CorrectAnswers + s.WrongAnswers + s.UnansweredCount;
                    return totalQuestions > 0 && ((double)s.CorrectAnswers / totalQuestions) >= ScoreTarget;
                });
                var totalDurationMinutes = todaySessions.Sum(s => (s.CompletedAt!.Value - s.StartedAt).TotalMinutes);
                bool has15MinsPractice = totalDurationMinutes >= PracticeMinutesTarget;

                isTodayPerfect = has3Lessons && has80PercentScore && has15MinsPractice;
            }

            if (isTodayPerfect)
            {
                streak++;
            }

            var remainingDaysToCheck = isTodayPerfect ? 6 : 7;

            checkDate = DateTime.UtcNow.Date.AddDays(-1);

            for (int i = 0; i < remainingDaysToCheck; i++)
            {
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
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return streak;
        }

        public async Task<LatestQuizSessionReadModel?> GetLatestQuizSessionWithModuleAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.QuizSessions
                .Where(s => s.StudentId == studentId && s.ModuleId != null && !s.IsDeleted)
                .OrderByDescending(s => s.StartedAt)
                .Select(s => new LatestQuizSessionReadModel
                {
                    ModuleId = s.ModuleId,
                    ModuleTitle = s.Module.Title
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> GetTotalHomeworkInModuleAsync(int moduleId, CancellationToken cancellationToken = default)
        {
            return await _context.Homework
                .CountAsync(l => l.ModuleId == moduleId && !l.IsDeleted, cancellationToken);
        }

        public async Task<int> GetCompletedHomeworkInModuleAsync(Guid studentId, int moduleId, CancellationToken cancellationToken = default)
        {
            return await _context.QuizSessions
                .Where(s => s.StudentId == studentId
                         && s.Status == QuizSessionStatus.Completed
                         && s.ModuleId == moduleId
                         && !s.IsDeleted)
                .Select(s => s.ModuleId)
                .Distinct()
                .CountAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TodayQuizSessionReadModel>> GetTodayQuizSessionsAsync(Guid studentId, DateTime todayStart, CancellationToken cancellationToken = default)
        {
            var todayEnd = todayStart.Date.AddDays(1);

            return await _context.QuizSessions
                .Where(s => s.StudentId == studentId
                         && s.CompletedAt.HasValue
                         && s.CompletedAt.Value >= todayStart
                         && s.CompletedAt.Value < todayEnd
                         && !s.IsDeleted)
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

        public async Task<List<StudentParentWithStatsDto>> GetParentChildrenWithStatsAsync(string parentId, CancellationToken cancellationToken)
        {
            var parentGuid = Guid.Parse(parentId);

            return await _context.StudentParents
                .Where(sp => sp.ParentId == parentGuid && !sp.IsDeleted)
                .Select(sp => new StudentParentWithStatsDto
                {
                    Id = sp.Id,
                    StudentId = sp.StudentId,
                    Status = sp.Status,
                    CreatedAt = sp.CreatedAt,
                    GradeLevel = sp.Student.GradeLevel,
                    TotalXP = sp.Student.TotalXP,
                    CurrentStreak = sp.Student.CurrentStreak,
                    CompletedLessonsCount = sp.Student.QuizSessions.Count(s => s.Status == QuizSessionStatus.Completed && !s.IsDeleted)
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<Dictionary<Guid, double>> GetLatestCompletionRatesAsync(List<Guid> studentIds, CancellationToken cancellationToken)
        {
            if (studentIds == null || !studentIds.Any())
                return new Dictionary<Guid, double>();

            var reports = await _context.Reports
                .Where(r => studentIds.Contains(r.StudentId))
                .GroupBy(r => r.StudentId)
                .Select(g => new
                {
                    StudentId = g.Key,
                    CompletionRate = g.OrderByDescending(r => r.GeneratedDate)
                                      .Select(r => r.CompletionRate)
                                      .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            return reports.ToDictionary(r => r.StudentId, r => r.CompletionRate);
        }

        public async Task<Dictionary<Guid, List<ChildSubjectProgressDto>>> GetRealSubjectProgressForStudentsAsync(List<Guid> studentIds, CancellationToken cancellationToken)
        {
            if (studentIds == null || !studentIds.Any())
                return new Dictionary<Guid, List<ChildSubjectProgressDto>>();

            var progressData = await _context.Classes
                .Where(c => c.StudentClasses.Any(sc => studentIds.Contains(sc.StudentId) && sc.IsActive && !sc.IsDeleted) && !c.IsDeleted)
                .SelectMany(c => c.StudentClasses
                    .Where(sc => studentIds.Contains(sc.StudentId) && sc.IsActive && !sc.IsDeleted)
                    .Select(sc => new
                    {
                        StudentId = sc.StudentId,
                        SubjectName = c.Subject.Name,
                        TotalItems = c.Roadmap != null
                            ? c.Roadmap.Modules.SelectMany(m => m.Homeworks).Count()
                            : 0,
                        CompletedItems = c.Roadmap != null
                            ? _context.StudentSubmissions.Count(sub => 
                                sub.StudentId == sc.StudentId && 
                                !sub.IsDeleted &&
                                _context.Homework.Any(h => h.Id == sub.HomeworkId && h.Module.RoadmapId == c.RoadmapId))
                            : 0
                    }))
                .ToListAsync(cancellationToken);

            var grouped = progressData
                .GroupBy(x => x.StudentId)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(x => x.SubjectName)
                          .Select(sg => new ChildSubjectProgressDto
                          {
                              subject = sg.Key ?? "Unknown",
                              progress_percentage = sg.Sum(x => x.TotalItems) == 0 ? 0 :
                                  (int)Math.Round((double)sg.Sum(x => x.CompletedItems) / sg.Sum(x => x.TotalItems) * 100)
                          })
                          .ToList()
                );

            return grouped;
        }

        public async Task<Dictionary<Guid, ChildDashboardStatsDto>> GetChildrenDashboardStatsAsync(List<Guid> studentIds, CancellationToken cancellationToken)
        {
            if (studentIds == null || !studentIds.Any())
                return new Dictionary<Guid, ChildDashboardStatsDto>();

            var statsList = await _context.Students
                .Where(s => studentIds.Contains(s.Id))
                .Select(s => new
                {
                    StudentId = s.Id,
                    TotalLessons = s.StudentClasses
                        .Where(sc => sc.IsActive && !sc.IsDeleted && !sc.Class.IsDeleted)
                        .Select(sc => sc.Class.Roadmap == null 
                            ? 0 
                            : sc.Class.Roadmap.Modules.SelectMany(m => m.Homeworks).Count())
                        .Sum(),
                    CompletedLessons = s.QuizSessions
                        .Count(qs => qs.Status == QuizSessionStatus.Completed && !qs.IsDeleted)
                })
                .ToListAsync(cancellationToken);

            return statsList.ToDictionary(
                x => x.StudentId,
                x => new ChildDashboardStatsDto
                {
                    TotalLessons = x.TotalLessons,
                    CompletedLessons = x.CompletedLessons
                });
        }

        public async Task<List<StudentSubmission>> GetRecentSubmissionsForStudentsAsync(List<Guid> studentIds, int count, CancellationToken cancellationToken)
        {
            if (studentIds == null || !studentIds.Any())
                return new List<StudentSubmission>();

            return await _context.StudentSubmissions
                .AsNoTracking()
                .Where(s => studentIds.Contains(s.StudentId))
                .OrderByDescending(s => s.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<QuizSession>> GetRecentCompletedQuizSessionsForStudentsAsync(List<Guid> studentIds, int count, CancellationToken cancellationToken)
        {
            if (studentIds == null || !studentIds.Any())
                return new List<QuizSession>();

            return await _context.QuizSessions
                .AsNoTracking()
                .Where(qs => studentIds.Contains(qs.StudentId) && qs.Status == QuizSessionStatus.Completed && !qs.IsDeleted)
                .OrderByDescending(qs => qs.CompletedAt ?? qs.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);
        }
    }
}
