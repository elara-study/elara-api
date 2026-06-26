using Elara.Domain.Entities.Educational;
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
