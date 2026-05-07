using Elara.Domain.Entities.Administrative;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.AdministrativeConfig
{
    public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
    {
        public void Configure(EntityTypeBuilder<DeviceToken> builder)
        {
            builder.ToTable("DeviceTokens");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Token)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(d => d.LastUsedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Unique index: one token per user (prevents duplicate registrations)
            builder.HasIndex(d => new { d.UserId, d.Token })
                .IsUnique()
                .HasDatabaseName("IX_DeviceTokens_UserId_Token");
        }
    }
}
