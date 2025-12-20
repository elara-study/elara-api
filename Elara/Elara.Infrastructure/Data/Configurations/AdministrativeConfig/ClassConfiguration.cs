using Elara.Domain.Entities.Administrative;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.AdministrativeConfig
{
    public class ClassConfiguration : IEntityTypeConfiguration<Class>
    {
        public void Configure(EntityTypeBuilder<Class> builder)
        {
            builder.ToTable("Classes");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.ClassName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Level)
                .IsRequired()
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(c => c.ClassName)
                .HasDatabaseName("IX_Classes_ClassName");

            builder.HasIndex(c => c.Level)
                .HasDatabaseName("IX_Classes_Level");

            builder.HasIndex(c => c.IsActive)
                .HasDatabaseName("IX_Classes_IsActive");
        }
    }
}
