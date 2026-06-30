using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elara.Infrastructure.Data.Configurations.EducationalConfig
{
    public class ProblemConfiguration : IEntityTypeConfiguration<Problem>
    {
        public void Configure(EntityTypeBuilder<Problem> builder)
        {
            builder.ToTable("Problems");

            builder.HasKey(q => q.Id);

            builder.Property(q => q.Text)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(q => q.QuestionType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(q => q.DifficultyLevel)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(DifficultyLevel.Easy)
                .HasSentinel(default(DifficultyLevel));

            builder.Property(q => q.IsAIGenerated)
                .HasDefaultValue(false);

            builder.Property(q => q.HasVideoSupport)
                .HasDefaultValue(false);

            builder.Property(q => q.Marks)
                .HasDefaultValue(10)
                .HasPrecision(5, 2);

            builder.Property(q => q.HomeworkId)
                .IsRequired();

            builder.HasIndex(q => q.HomeworkId)
                .HasDatabaseName("IX_Problems_HomeworkId");

            builder.HasIndex(q => q.QuestionType)
                .HasDatabaseName("IX_Problems_QuestionType");

            builder.HasIndex(q => q.DifficultyLevel)
                .HasDatabaseName("IX_Problems_DifficultyLevel");

            builder.HasOne(q => q.Homework)
                .WithMany(l => l.Problems)
                .HasForeignKey(q => q.HomeworkId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
