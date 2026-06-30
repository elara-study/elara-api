using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.EducationalConfig
{
    public class HomeworkConfiguration : IEntityTypeConfiguration<Homework>
    {
        public void Configure(EntityTypeBuilder<Homework> builder)
        {
            builder.ToTable("Homework");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.Content)
                .HasMaxLength(5000);

            builder.Property(l => l.EstimatedDurationMinutes)
                .HasDefaultValue(30);

            builder.Property(l => l.Description)
                .HasMaxLength(2000);

            builder.Property(l => l.DueDate)
                .IsRequired();

            builder.Property(l => l.MaxScore)
                .HasDefaultValue(100);

            builder.Property(l => l.IsRequired)
                .HasDefaultValue(true);

            builder.Property(l => l.DifficultyLevel)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(DifficultyLevel.Easy)
                .HasSentinel(default(DifficultyLevel));

            builder.Property(l => l.IsAIGenerated)
                .HasDefaultValue(false);

            builder.Property(l => l.ModuleId)
                .IsRequired();

            builder.HasIndex(l => l.ModuleId)
                .HasDatabaseName("IX_Homework_ModuleId");

            builder.HasOne(l => l.Module)
                .WithMany(t => t.Homeworks)
                .HasForeignKey(l => l.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
