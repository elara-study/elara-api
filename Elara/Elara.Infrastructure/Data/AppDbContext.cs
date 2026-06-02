using Elara.Domain.Entities;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Chat;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Entities.Users;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Elara.Infrastructure.Identity;

namespace Elara.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        #region User DbSets
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
        public DbSet<Roadmap> Roadmaps { get; set; }
        public DbSet<EducationalVideo> EducationalVideos { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<TopicResource> TopicResources { get; set; }
        #endregion

        #region Submission DbSets
        public DbSet<StudentSubmission> StudentSubmissions { get; set; }
        public DbSet<Hint> Hints { get; set; }
        public DbSet<QuizSession> QuizSessions { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }
        public DbSet<StudentSubmissionAnswer> StudentSubmissionAnswers { get; set; }
        #endregion

        #region Administrative DbSets
        public DbSet<Class> Classes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<NotificationPreference> NotificationPreferences { get; set; }
        public DbSet<DeviceToken> DeviceTokens { get; set; }
        #endregion

        #region Junction Tables DbSets
        public DbSet<StudentTeacher> StudentTeachers { get; set; }
        public DbSet<StudentParent> StudentParents { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<StudentAchievement> StudentAchievements { get; set; }
        public DbSet<LessonVideo> LessonVideos { get; set; }
        #endregion

        #region Chat DbSets
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatAnalysisReport> ChatAnalysisReports { get; set; }
        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Configure Identity table names
            ConfigureIdentityTables(modelBuilder);

            // Apply soft delete query filter
            ApplySoftDeleteQueryFilter(modelBuilder);

        }

        private void ConfigureIdentityTables(ModelBuilder modelBuilder)
        {
            // Rename Identity tables
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        }

        private void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
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

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Auto-set audit fields
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = DateTime.UtcNow;
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
