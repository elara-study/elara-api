using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Infrastructure.Data.Configurations.EducationalConfig
{
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.ToTable("Assignments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Description)
                .HasMaxLength(2000);

            builder.Property(a => a.DueDate)
                .IsRequired();

            builder.Property(a => a.MaxScore)
                .HasDefaultValue(100);

            builder.Property(a => a.IsRequired)
                .HasDefaultValue(true);

            builder.Property(a => a.DifficultyLevel)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(DifficultyLevel.Easy);

            builder.Property(a => a.TopicId)
                .IsRequired();

            // Indexes
            builder.HasIndex(a => a.DueDate)
                .HasDatabaseName("IX_Assignments_DueDate");

            builder.HasIndex(a => a.TopicId)
                .HasDatabaseName("IX_Assignments_TopicId");

            builder.HasIndex(a => a.TeacherId)
                .HasDatabaseName("IX_Assignments_TeacherId");

            // Relationships
            builder.HasOne(a => a.Topic)
                .WithMany(t => t.Assignments)
                .HasForeignKey(a => a.TopicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Lesson)
                .WithMany(l => l.Assignments)
                .HasForeignKey(a => a.LessonId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(a => a.Teacher)
                .WithMany(t => t.Assignments)
                .HasForeignKey(a => a.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
