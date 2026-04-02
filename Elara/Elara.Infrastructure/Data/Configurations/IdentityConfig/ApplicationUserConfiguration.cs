using Elara.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.IdentityConfig
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Table name
            builder.ToTable("Users");

            // Properties
            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.ImageUrl)
                .HasMaxLength(500);

            builder.Property(u => u.ImagePublicId)
                .HasMaxLength(255);

            // Indexes
            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            builder.HasIndex(u => u.UserName)
                .IsUnique()
                .HasDatabaseName("IX_Users_UserName");
        }
    }
}

