using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.SubmissionsConfig
{
    public class QuizAnswerConfiguration : IEntityTypeConfiguration<QuizAnswer>
    {
        public void Configure(EntityTypeBuilder<QuizAnswer> builder)
        {
            builder.ToTable("QuizAnswers");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.QuestionText)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(a => a.QuestionType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.StudentAnswer)
                .HasMaxLength(5000);

            builder.Property(a => a.CorrectAnswer)
                .HasMaxLength(2000);

            builder.Property(a => a.IsCorrect)
                .IsRequired(false);

            builder.Property(a => a.XpAwarded)
                .HasDefaultValue(0);

            builder.Property(a => a.HintUsed)
                .HasDefaultValue(false);

            builder.HasIndex(a => a.QuizSessionId)
                .HasDatabaseName("IX_QuizAnswers_QuizSessionId");

            builder.HasOne(a => a.QuizSession)
                .WithMany(s => s.Answers)
                .HasForeignKey(a => a.QuizSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
