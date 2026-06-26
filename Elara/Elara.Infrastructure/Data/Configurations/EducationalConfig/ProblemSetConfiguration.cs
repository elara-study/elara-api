using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.EducationalConfig
{
    public class ProblemSetConfiguration : IEntityTypeConfiguration<ProblemSet>
    {
        public void Configure(EntityTypeBuilder<ProblemSet> builder)
        {
            builder.ToTable("ProblemSets");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Description)
                .HasMaxLength(2000);

            builder.Property(a => a.DueDate)
                .IsRequired();

            builder.Property(a => a.MaxScore)
                .HasDefaultValue(100);

            builder.Property(a => a.IsRequired)
                .HasDefaultValue(true);

            builder.Property(a => a.DifficultyLevel)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(DifficultyLevel.Easy)
                .HasSentinel(default(DifficultyLevel));

            builder.Property(a => a.ProblemSetType)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(ProblemSetType.Quiz)
                .HasSentinel(default(ProblemSetType));

            builder.Property(a => a.ModuleId)
                .IsRequired();

            builder.Property(a => a.HomeworkId);

            builder.Property(a => a.IsAIGenerated)
                .HasDefaultValue(false);

            builder.HasIndex(a => a.DueDate)
                .HasDatabaseName("IX_ProblemSets_DueDate");

            builder.HasIndex(a => a.ModuleId)
                .HasDatabaseName("IX_ProblemSets_ModuleId");

            builder.HasIndex(a => a.TeacherId)
                .HasDatabaseName("IX_ProblemSets_TeacherId");

            builder.HasOne(a => a.Module)
                .WithMany(t => t.ProblemSets)
                .HasForeignKey(a => a.ModuleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Homework)
                .WithMany(l => l.ProblemSets)
                .HasForeignKey(a => a.HomeworkId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(a => a.Teacher)
                .WithMany(t => t.ProblemSets)
                .HasForeignKey(a => a.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
