using Elara.Domain.Entities.Educational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.EducationalConfig
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.ToTable("Modules");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.Content)
                .HasMaxLength(5000);

            builder.Property(t => t.SubjectId)
                .IsRequired();

            builder.HasIndex(t => t.Title)
                .HasDatabaseName("IX_Modules_Title");

            builder.HasIndex(t => t.SubjectId)
                .HasDatabaseName("IX_Modules_SubjectId");

            builder.HasOne(t => t.Subject)
                .WithMany(s => s.Modules)
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
