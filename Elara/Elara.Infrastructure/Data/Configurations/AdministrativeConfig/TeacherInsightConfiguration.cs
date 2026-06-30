using Elara.Domain.Entities.Administrative;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.AdministrativeConfig
{
    public class TeacherInsightConfiguration : IEntityTypeConfiguration<TeacherInsight>
    {
        public void Configure(EntityTypeBuilder<TeacherInsight> builder)
        {
            builder.ToTable("TeacherInsights");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.PublicId)
                .IsRequired()
                .HasDefaultValueSql("gen_random_uuid()");

            builder.HasIndex(t => t.PublicId)
                .IsUnique()
                .HasDatabaseName("IX_TeacherInsights_PublicId");

            builder.Property(t => t.Content)
                .IsRequired();

            builder.HasOne(t => t.Student)
                .WithMany()
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Teacher)
                .WithMany()
                .HasForeignKey(t => t.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
