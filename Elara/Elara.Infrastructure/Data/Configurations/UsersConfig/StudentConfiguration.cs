using Elara.Domain.Entities.Users;
using Elara.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.UsersConfig
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.Property(s => s.GradeLevel)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(s => s.LearningLevel)
            .HasMaxLength(50)
            .HasDefaultValue(LearningLevel.Beginner);

            builder.HasOne(S=>S.student)
                .WithOne()
                .HasForeignKey<Student>(s=>s.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
           
            builder.HasMany(s => s.StudentClasses)
                .WithOne(s => s.Student)
                .HasForeignKey(s => s.StudentId);

            builder.HasMany(s => s.StudentTeachers)
                .WithOne(s => s.Student)
                .HasForeignKey(s => s.StudentId);

            builder.HasMany(s => s.StudentAchievements)
                .WithOne(s => s.Student)
                .HasForeignKey(s => s.StudentId);

            // Indexes
            builder.HasIndex(s => s.GradeLevel)
                .HasDatabaseName("IX_Students_GradeLevel");
        }
    }
}
