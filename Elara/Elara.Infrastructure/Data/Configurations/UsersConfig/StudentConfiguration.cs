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
                .HasDefaultValue(LearningLevel.Beginner)
                .HasSentinel(default(LearningLevel));

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Student>(s => s.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(s => s.GradeLevel)
                .HasDatabaseName("IX_Students_GradeLevel");

            builder.HasIndex(s => s.TotalXP)
                .HasDatabaseName("IX_Students_TotalXP");

            // XP & Gamification defaults
            builder.Property(s => s.TotalXP).HasDefaultValue(0);

            builder.Property(s => s.CurrentStreak).HasDefaultValue(0);
        }
    }
}
