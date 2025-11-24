using Elara.Domain.Entities.JunctionTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Infrastructure.Data.Configurations.JunctionTablesConfig
{
    public class LessonVideoConfiguration : IEntityTypeConfiguration<LessonVideo>
    {
        public void Configure(EntityTypeBuilder<LessonVideo> builder)
        {
            builder.ToTable("LessonVideos");

            builder.HasKey(lv => lv.Id);

            // Composite Index
            builder.HasIndex(lv => new { lv.LessonId, lv.VideoId })
                .IsUnique()
                .HasDatabaseName("IX_LessonVideos_LessonId_VideoId");

            builder.HasIndex(lv => lv.LessonId)
                .HasDatabaseName("IX_LessonVideos_LessonId");

            builder.HasIndex(lv => lv.VideoId)
                .HasDatabaseName("IX_LessonVideos_VideoId");

            // Relationships
            builder.HasOne(lv => lv.Lesson)
                .WithMany(l => l.LessonVideos)
                .HasForeignKey(lv => lv.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(lv => lv.Video)
                .WithMany(v => v.LessonVideos)
                .HasForeignKey(lv => lv.VideoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
