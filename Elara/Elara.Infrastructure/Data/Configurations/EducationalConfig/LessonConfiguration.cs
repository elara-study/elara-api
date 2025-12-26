using Elara.Domain.Entities.Educational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Infrastructure.Data.Configurations.EducationalConfig
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.ToTable("Lessons");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.Content)
                .HasMaxLength(5000);

            builder.Property(l => l.EstimatedDurationMinutes)
                .HasDefaultValue(30);

            builder.Property(l => l.TopicId)
                .IsRequired();

            // Indexes
            builder.HasIndex(l => l.TopicId)
                .HasDatabaseName("IX_Lessons_TopicId");

            // Relationships
            builder.HasOne(l => l.Topic)
                .WithMany(t => t.Lessons)
                .HasForeignKey(l => l.TopicId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
