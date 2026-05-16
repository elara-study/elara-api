using Elara.Domain.Entities.Administrative;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.AdministrativeConfig
{
    public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
    {
        public void Configure(EntityTypeBuilder<Achievement> builder)
        {
            builder.ToTable("Achievements");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Description)
                .HasMaxLength(1000);

            builder.Property(a => a.AchievementType)
               .IsRequired()
               .HasConversion<string>();

            builder.Property(a => a.Points)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(a => a.ImageUrl)
                .HasMaxLength(500);

            builder.Property(a => a.ImagePublicId)
                .HasMaxLength(100);

            builder.Property(a => a.TargetValue)
                .IsRequired()
                .HasDefaultValue(0);

            // Indexes
            builder.HasIndex(a => a.Title)
                .HasDatabaseName("IX_Achievements_Title");

            builder.HasIndex(a => a.AchievementType)
                .HasDatabaseName("IX_Achievements_AchievementType");
        }

    }
}
