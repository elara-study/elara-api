using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Students.Queries.GetStudentHome
{
    public class GetStudentHomeQueryHandler : IRequestHandler<GetStudentHomeQuery, StudentHomeDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetStudentHomeQueryHandler(
            IStudentRepository studentRepository,
            ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<StudentHomeDto> Handle(GetStudentHomeQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");

            var student = await _studentRepository.GetByIdAsync(userId, cancellationToken);
            if (student == null) throw new Exception("Student not found");

            var namesDict = await _studentRepository.GetStudentNamesAsync(new[] { userId }, cancellationToken);
            var studentName = namesDict.GetValueOrDefault(userId, string.Empty);

            var response = new StudentHomeDto
            {
                StudentName = studentName,
                Gamification = new HomeGamificationDto
                {
                    Streak = student.CurrentStreak,
                    TotalXp = student.TotalXP
                }
            };

            // Get Recent Activity
            var recentSession = await _studentRepository.GetLatestQuizSessionWithTopicAsync(userId, cancellationToken);

            if (recentSession != null)
            {
                int topicId = recentSession.TopicId;
                
                int totalLessons = await _studentRepository.GetTotalLessonsInTopicAsync(topicId, cancellationToken);
                int completedLessons = await _studentRepository.GetCompletedLessonsInTopicAsync(userId, topicId, cancellationToken);

                int progressPercentage = totalLessons > 0 ? (int)Math.Round((double)completedLessons / totalLessons * 100) : 0;
                
                string courseName = recentSession.TopicTitle ?? recentSession.AssignmentTitle ?? "Recent Activity";

                response.RecentActivity = new HomeRecentActivityDto
                {
                    CourseId = topicId,
                    CourseName = courseName,
                    CurrentLessonNumber = completedLessons + 1 > totalLessons ? totalLessons : completedLessons + 1,
                    TotalLessons = totalLessons,
                    ProgressPercentage = progressPercentage,
                    ActionUrl = $"/topics/{topicId}"
                };
            }

            // Calculate Daily Goals Progress
            var todayStart = DateTime.UtcNow.Date;

            // Define hardcoded goals
            var activeGoals = new[]
            {
                new { Id = 1, Title = "Complete 3 lessons", GoalType = AchievementType.LessonsCompletedInOneDay, TargetValue = 3, XpReward = 50, IconType = "flag" },
                new { Id = 2, Title = "Score 80% on a quiz", GoalType = AchievementType.SpecificQuizScore, TargetValue = 80, XpReward = 30, IconType = "trophy" },
                new { Id = 3, Title = "Practice for 15 mins", GoalType = AchievementType.PracticeTime, TargetValue = 15, XpReward = 25, IconType = "clock" }
            };

            var sessionsToday = await _studentRepository.GetTodayQuizSessionsAsync(userId, todayStart, cancellationToken);

            var completedLessonsTodayCount = sessionsToday.Count(s => s.Status == QuizSessionStatus.Completed);
            
            var has80PercentScoreToday = sessionsToday.Any(s => 
                s.Status == QuizSessionStatus.Completed && 
                (s.CorrectAnswers + s.WrongAnswers + s.UnansweredCount > 0) && 
                ((double)s.CorrectAnswers / (s.CorrectAnswers + s.WrongAnswers + s.UnansweredCount) >= 0.8));

            var totalPracticeMinutesToday = (int)sessionsToday
                .Where(s => s.CompletedAt.HasValue)
                .Sum(s => (s.CompletedAt!.Value - s.StartedAt).TotalMinutes);

            int completedGoalsCount = 0;

            foreach (var goal in activeGoals)
            {
                int currentValue = 0;
                switch (goal.GoalType)
                {
                    case AchievementType.LessonsCompletedInOneDay:
                        currentValue = completedLessonsTodayCount;
                        break;
                    case AchievementType.SpecificQuizScore:
                        currentValue = has80PercentScoreToday ? goal.TargetValue : 0;
                        break;
                    case AchievementType.PracticeTime:
                        currentValue = totalPracticeMinutesToday; 
                        break;
                    default:
                        currentValue = 0;
                        break;
                }

                bool isCompleted = currentValue >= goal.TargetValue;
                if (isCompleted) completedGoalsCount++;

                response.DailyGoals.Goals.Add(new HomeGoalItemDto
                {
                    Id = goal.Id,
                    Title = goal.Title,
                    TargetValue = goal.TargetValue,
                    CurrentValue = Math.Min(currentValue, goal.TargetValue),
                    XpReward = goal.XpReward,
                    IconType = goal.IconType,
                    IsCompleted = isCompleted
                });
            }

            response.DailyGoals.CompletedCount = completedGoalsCount;
            response.DailyGoals.TotalCount = activeGoals.Length;

            // Get Enrolled Groups
            var enrolledClasses = await _studentRepository.GetStudentEnrolledClassesAsync(userId, cancellationToken);

            foreach (var c in enrolledClasses)
            {
                if (c != null)
                {
                    response.MyGroups.Add(new HomeGroupDto
                    {
                        Id = c.PublicId,
                        Name = c.ClassName,
                        AvatarUrl = null
                    });
                }
            }

            return response;
        }
    }
}
