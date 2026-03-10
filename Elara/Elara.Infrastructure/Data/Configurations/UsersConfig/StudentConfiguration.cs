using Elara.Domain.Entities.Users;
using Elara.Domain.Enums;
using Elara.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.UsersConfig
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(s => s.Id);
            
            builder.Property(s => s.Id)
                .ValueGeneratedNever();

            builder.Property(s => s.GradeLevel)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(s => s.LearningLevel)
                .HasMaxLength(50)
                .HasDefaultValue(LearningLevel.Beginner);

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Student>(s => s.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Parent
            builder.HasOne(s => s.Parent)
                .WithMany(p => p.Childrens)
                .HasForeignKey(s => s.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(s => s.GradeLevel)
                .HasDatabaseName("IX_Students_GradeLevel");
        }
    }
}
