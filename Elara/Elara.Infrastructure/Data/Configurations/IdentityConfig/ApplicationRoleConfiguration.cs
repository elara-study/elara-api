using Elara.Domain.Entities.IdentityEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.IdentityConfig
{
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            // Table name
            builder.ToTable("Roles");

            // Indexes
            builder.HasIndex(r => r.Name)
                .IsUnique()
                .HasDatabaseName("IX_Roles_Name");
        }
    }
}

