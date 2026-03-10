using Elara.Domain.Entities.Administrative;
using Elara.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.AdministrativeConfig
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Reports");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.GeneratedDate)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(r => r.AverageScore)
                .HasDefaultValue(0)
                .HasPrecision(5, 2);

            builder.Property(r => r.ImprovementTips)
                .HasMaxLength(2000);

            builder.Property(r => r.WeakAreas)
                .HasMaxLength(2000);

            builder.Property(r => r.StrengthAreas)
                .HasMaxLength(2000);

            builder.Property(r => r.Summary)
                .HasMaxLength(1000);

            builder.Property(r => r.TotalAssignmentsCompleted)
                .HasDefaultValue(0);

            builder.Property(r => r.TotalHintsUsed)
                .HasDefaultValue(0);

            builder.Property(r => r.CompletionRate)
                .HasDefaultValue(0)
                .HasPrecision(5, 2);

            // Indexes
            builder.HasIndex(r => r.StudentId)
                .HasDatabaseName("IX_Reports_StudentId");

            builder.HasIndex(r => r.GeneratedDate)
                .HasDatabaseName("IX_Reports_GeneratedDate");

            builder.HasIndex(r => new { r.StudentId, r.GeneratedDate })
                .HasDatabaseName("IX_Reports_StudentId_GeneratedDate");

            // Relationships
            builder.HasOne<ApplicationUser>()
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
