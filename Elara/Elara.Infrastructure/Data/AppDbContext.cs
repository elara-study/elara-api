using Elara.Domain.Entities;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        #region User DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Parent> Parents { get; set; }
        #endregion

        #region Educational DbSets
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<EducationalVideo> EducationalVideos { get; set; }
        #endregion

        #region Submission DbSets
        public DbSet<StudentSubmission> StudentSubmissions { get; set; }
        public DbSet<Hint> Hints { get; set; }
        #endregion

        #region Administrative DbSets
        public DbSet<Class> Classes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        #endregion

        #region Junction Tables DbSets
        public DbSet<StudentTeacher> StudentTeachers { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<ClassTeacher> ClassTeachers { get; set; }
        public DbSet<StudentAchievement> StudentAchievements { get; set; }
        public DbSet<LessonVideo> LessonVideos { get; set; }
        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Configure TPH (Table Per Hierarchy) for User inheritance
             ConfigureUserInheritance(modelBuilder);

            // Apply soft delete query filter
            ApplySoftDeleteQueryFilter(modelBuilder);

        }

        private void ConfigureUserInheritance(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Student>("Student")
                .HasValue<Teacher>("Teacher")
                .HasValue<Parent>("Parent")
                .HasValue<User>("User");

            modelBuilder.Entity<User>()
              .HasIndex("UserType")
              .HasDatabaseName("IX_Users_UserType");
        }

        private void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // For User inheritance, apply filter only on the root type
                    if (entityType.ClrType == typeof(User))
                    {
                        var parameter = Expression.Parameter(entityType.ClrType, "e");
                        var body = Expression.Equal(
                            Expression.Property(parameter, nameof(BaseEntity.IsDeleted)),
                            Expression.Constant(false)
                        );
                        modelBuilder.Entity(entityType.ClrType)
                            .HasQueryFilter(Expression.Lambda(body, parameter));
                    }

                    // For other entities not inheriting from User, apply normally
                    else if (!typeof(User).IsAssignableFrom(entityType.ClrType))
                    {
                        var parameter = Expression.Parameter(entityType.ClrType, "e");
                        var body = Expression.Equal(
                            Expression.Property(parameter, nameof(BaseEntity.IsDeleted)),
                            Expression.Constant(false)
                        );
                        modelBuilder.Entity(entityType.ClrType)
                            .HasQueryFilter(Expression.Lambda(body, parameter));
                    }
                }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Auto-set audit fields
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        // Soft delete
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
