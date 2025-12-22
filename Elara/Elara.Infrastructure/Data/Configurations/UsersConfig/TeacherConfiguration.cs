using Elara.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.UsersConfig
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {

            // Relationship with Subject
            builder.HasOne(t => t.Subject)
                .WithMany(s => s.Teachers)
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(t => t.User)
                .WithOne()
                .HasForeignKey<Teacher>(t=>t.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t=>t.ClassTeachers)
                .WithOne(t=>t.Teacher)
                .HasForeignKey(s => s.TeacherId);

            builder.HasMany(t => t.Assignments)
                .WithOne(t => t.Teacher)
                .HasForeignKey(s => s.TeacherId);

            builder.HasMany(t => t.StudentTeachers)
                .WithOne(t => t.Teacher)
                .HasForeignKey(s => s.TeacherId);
            
            // Indexes
            builder.HasIndex(t => t.SubjectId)
                .HasDatabaseName("IX_Teachers_SubjectId");
        }
    }
}
