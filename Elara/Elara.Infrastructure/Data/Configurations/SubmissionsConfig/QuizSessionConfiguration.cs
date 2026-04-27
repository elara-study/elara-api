using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.SubmissionsConfig
{
    public class QuizSessionConfiguration : IEntityTypeConfiguration<QuizSession>
    {
        public void Configure(EntityTypeBuilder<QuizSession> builder)
        {
            builder.ToTable("QuizSessions");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(QuizSessionStatus.InProgress);

            builder.Property(s => s.StartedAt)
                .IsRequired();

            builder.Property(s => s.XpEarned)
                .HasDefaultValue(0);

            builder.Property(s => s.CorrectAnswers)
                .HasDefaultValue(0);

            builder.Property(s => s.WrongAnswers)
                .HasDefaultValue(0);

            builder.Property(s => s.UnansweredCount)
                .HasDefaultValue(0);

            builder.Property(s => s.ElaraInsight)
                .HasMaxLength(1000);

            builder.Property(s => s.WeakTopics)
                .HasMaxLength(500);

            builder.Property(s => s.InsightRecommendation)
                .HasMaxLength(500);

            builder.Property(s => s.StudentId)
                .IsRequired();

            builder.Property(s => s.AssignmentId)
                .IsRequired();

            // Indexes
            builder.HasIndex(s => s.StudentId)
                .HasDatabaseName("IX_QuizSessions_StudentId");

            builder.HasIndex(s => s.AssignmentId)
                .HasDatabaseName("IX_QuizSessions_AssignmentId");

            builder.HasIndex(s => new { s.StudentId, s.Status })
                .HasDatabaseName("IX_QuizSessions_StudentId_Status");

            // Relationships
            builder.HasOne(s => s.Student)
                .WithMany(st => st.QuizSessions)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Assignment)
                .WithMany()
                .HasForeignKey(s => s.AssignmentId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
