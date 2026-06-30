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
                .HasDefaultValue(QuizSessionStatus.InProgress)
                .HasSentinel(default(QuizSessionStatus));

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

            builder.Property(s => s.QuestionsJson)
                .HasColumnType("text");

            builder.Property(s => s.ElaraInsight)
                .HasColumnType("text");

            builder.Property(s => s.StudentId)
                .IsRequired();

            builder.HasIndex(s => s.StudentId)
                .HasDatabaseName("IX_QuizSessions_StudentId");

            builder.HasIndex(s => new { s.StudentId, s.Status })
                .HasDatabaseName("IX_QuizSessions_StudentId_Status");

            builder.HasOne(s => s.Student)
                .WithMany(st => st.QuizSessions)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Homework)
                .WithMany()
                .HasForeignKey(s => s.HomeworkId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(s => s.Module)
                .WithMany()
                .HasForeignKey(s => s.ModuleId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
