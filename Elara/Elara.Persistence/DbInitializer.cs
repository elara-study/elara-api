using EducationalSubject = Elara.Domain.Entities.Educational.Subject;
using Elara.Domain.Enums;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Persistence
{
    public static class DbInitializer
    {
        public static async Task RunMigrationsAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!await context.Subjects.IgnoreQueryFilters().AnyAsync(s => s.Name == "Chemistry"))
            {
                Console.WriteLine("Seeding subjects...");
                var subjects = new List<EducationalSubject>
                {
                    new() { Id = 1, Name = "Chemistry", Description = "Chemistry", GradeLevel = GradeLevel.Grade12, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Id = 2, Name = "Physics", Description = "Physics", GradeLevel = GradeLevel.Grade12, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Id = 3, Name = "Biology", Description = "Biology", GradeLevel = GradeLevel.Grade12, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Id = 4, Name = "PureMathematics", Description = "PureMathematics", GradeLevel = GradeLevel.Grade12, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Id = 5, Name = "AppliedMathematics", Description = "AppliedMathematics", GradeLevel = GradeLevel.Grade12, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Id = 6, Name = "Arabic", Description = "Arabic", GradeLevel = GradeLevel.Grade12, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Id = 7, Name = "English", Description = "English", GradeLevel = GradeLevel.Grade12, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                };
                context.Subjects.AddRange(subjects);
                await context.SaveChangesAsync();
                Console.WriteLine("Subjects seeded successfully.");
            }
        }
    }
}
