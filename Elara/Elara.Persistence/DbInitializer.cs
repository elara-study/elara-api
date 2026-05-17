using Elara.Domain.Entities.Administrative;
using Elara.Domain.Enums;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (!await context.Lessons.AnyAsync())
            {
                await Elara.Scripts.DataPopulator.Run(context);
            }

            await SeedAchievementsAsync(context);
        }

        private static async Task SeedAchievementsAsync(AppDbContext context)
        {
            if (await context.Achievements.AnyAsync()) return;

            var achievements = new List<Achievement>
            {
                new Achievement
                {
                    Title = "First Steps",
                    Description = "Complete your first lesson",
                    AchievementType = AchievementType.LessonsCompleted,
                    TargetValue = 1,
                    Points = 10
                },
                new Achievement
                {
                    Title = "Quick Learner",
                    Description = "Complete 5 lessons in one day",
                    AchievementType = AchievementType.LessonsCompletedInOneDay,
                    TargetValue = 5,
                    Points = 20
                },
                new Achievement
                {
                    Title = "Streak Master",
                    Description = "Maintain a 7-day streak",
                    AchievementType = AchievementType.Streak,
                    TargetValue = 7,
                    Points = 30
                },
                new Achievement
                {
                    Title = "Quiz Champion",
                    Description = "Score 100% on any quiz",
                    AchievementType = AchievementType.SpecificQuizScore,
                    TargetValue = 100,
                    Points = 50
                },
                new Achievement
                {
                    Title = "Bookworm",
                    Description = "Complete 50 lessons",
                    AchievementType = AchievementType.LessonsCompleted,
                    TargetValue = 50,
                    Points = 100
                },
                new Achievement
                {
                    Title = "Legend",
                    Description = "Earn 10,000 XP",
                    AchievementType = AchievementType.TotalXP,
                    TargetValue = 10000,
                    Points = 200
                },
                new Achievement
                {
                    Title = "Genius",
                    Description = "Master all subjects",
                    AchievementType = AchievementType.SubjectsMastered,
                    TargetValue = 5,
                    Points = 500
                },
                new Achievement
                {
                    Title = "Perfect Week",
                    Description = "Complete all daily goals for a week",
                    AchievementType = AchievementType.PerfectWeek,
                    TargetValue = 7,
                    Points = 150
                }
            };

            await context.Achievements.AddRangeAsync(achievements);
            await context.SaveChangesAsync();
        }
    }
}
