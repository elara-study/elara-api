using Elara.Domain.Entities.Administrative;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.AdministrativeConfig
{
    public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
    {
        public void Configure(EntityTypeBuilder<NotificationPreference> builder)
        {
            builder.ToTable("NotificationPreferences");

            builder.HasKey(p => p.Id);

            // One preference record per user
            builder.HasIndex(p => p.UserId)
                .IsUnique()
                .HasDatabaseName("IX_NotificationPreferences_UserId");

            builder.Property(p => p.GroupUpdates).HasDefaultValue(true);
            builder.Property(p => p.StreakReminders).HasDefaultValue(true);
            builder.Property(p => p.HomeworkReminders).HasDefaultValue(true);
            builder.Property(p => p.NewLessons).HasDefaultValue(true);
            builder.Property(p => p.AiProgressReports).HasDefaultValue(true);
        }
    }
}
