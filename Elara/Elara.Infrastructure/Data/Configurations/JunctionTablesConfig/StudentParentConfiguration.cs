using Elara.Domain.Entities.JunctionTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.JunctionTablesConfig
{
    public class StudentParentConfiguration : IEntityTypeConfiguration<StudentParent>
    {
        public void Configure(EntityTypeBuilder<StudentParent> builder)
        {
            builder.ToTable("StudentParents");

            builder.HasKey(sp => sp.Id);

            builder.HasIndex(sp => new { sp.StudentId, sp.ParentId })
                .IsUnique()
                .HasDatabaseName("IX_StudentParents_StudentId_ParentId");

            builder.HasIndex(sp => sp.StudentId)
                .HasDatabaseName("IX_StudentParents_StudentId");

            builder.HasIndex(sp => sp.ParentId)
                .HasDatabaseName("IX_StudentParents_ParentId");

            builder.HasOne(sp => sp.Student)
                .WithMany(s => s.StudentParents)
                .HasForeignKey(sp => sp.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sp => sp.Parent)
                .WithMany(p => p.StudentParents)
                .HasForeignKey(sp => sp.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
