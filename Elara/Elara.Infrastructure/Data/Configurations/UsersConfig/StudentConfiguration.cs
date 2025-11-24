using Elara.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            .HasDefaultValue("Beginner");

            builder.Property(s => s.DailyHintsUsed)
               .HasDefaultValue(0);

            // Relationship with Parent
            builder.HasOne(s => s.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(s => s.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(s => s.GradeLevel)
                .HasDatabaseName("IX_Students_GradeLevel");

            builder.HasIndex(s => s.ParentId)
                .HasDatabaseName("IX_Students_ParentId");
        }
    }
}
