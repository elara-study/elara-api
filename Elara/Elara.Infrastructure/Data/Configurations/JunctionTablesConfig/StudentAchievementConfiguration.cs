using Elara.Domain.Entities.JunctionTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.JunctionTablesConfig
{
    public class StudentAchievementConfiguration : IEntityTypeConfiguration<StudentAchievement>
    {
        public void Configure(EntityTypeBuilder<StudentAchievement> builder)
        {
            builder.ToTable("StudentAchievements");

            builder.HasKey(sa => sa.Id);

            builder.Property(sa => sa.EarnedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");


            // Composite Index
            builder.HasIndex(sa => new { sa.StudentId, sa.AchievementId })
                .IsUnique()
                .HasDatabaseName("IX_StudentAchievements_StudentId_AchievementId");

            builder.HasIndex(sa => sa.StudentId)
                .HasDatabaseName("IX_StudentAchievements_StudentId");

            builder.HasIndex(sa => sa.AchievementId)
                .HasDatabaseName("IX_StudentAchievements_AchievementId");

            builder.HasIndex(sa => sa.EarnedAt)
                .HasDatabaseName("IX_StudentAchievements_EarnedAt");

            builder.HasIndex(sa => new { sa.StudentId, sa.EarnedAt })
                .IsDescending(false, true)
                .HasDatabaseName("IX_StudentAchievements_StudentId_EarnedAt");

            // Relationships
            builder.HasOne(sa => sa.Student)
                .WithMany(s => s.StudentAchievements)
                .HasForeignKey(sa => sa.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sa => sa.Achievement)
                .WithMany(a => a.StudentAchievements)
                .HasForeignKey(sa => sa.AchievementId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
