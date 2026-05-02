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

            builder.Property(a => a.QuestionType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.AnswerContent)
                .HasMaxLength(5000);

            builder.Property(a => a.IsCorrect)
                .IsRequired(false);

            builder.Property(a => a.XpAwarded)
                .HasDefaultValue(0);

            builder.Property(a => a.HintUsed)
                .HasDefaultValue(false);

            // Indexes
            builder.HasIndex(a => a.QuizSessionId)
                .HasDatabaseName("IX_QuizAnswers_QuizSessionId");

            builder.HasIndex(a => a.QuestionId)
                .HasDatabaseName("IX_QuizAnswers_QuestionId");

            // Relationships
            builder.HasOne(a => a.QuizSession)
                .WithMany(s => s.Answers)
                .HasForeignKey(a => a.QuizSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.SelectedOption)
                .WithMany()
                .HasForeignKey(a => a.SelectedOptionId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
