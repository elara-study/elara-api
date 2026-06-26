using Elara.Domain.Entities.JunctionTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.JunctionTablesConfig
{
    public class HomeworkVideoConfiguration : IEntityTypeConfiguration<HomeworkVideo>
    {
        public void Configure(EntityTypeBuilder<HomeworkVideo> builder)
        {
            builder.ToTable("HomeworkVideos");

            builder.HasKey(lv => lv.Id);

            builder.HasIndex(lv => new { lv.HomeworkId, lv.VideoId })
                .IsUnique()
                .HasDatabaseName("IX_HomeworkVideos_HomeworkId_VideoId");

            builder.HasIndex(lv => lv.HomeworkId)
                .HasDatabaseName("IX_HomeworkVideos_HomeworkId");

            builder.HasIndex(lv => lv.VideoId)
                .HasDatabaseName("IX_HomeworkVideos_VideoId");

            builder.HasOne(lv => lv.Homework)
                .WithMany(l => l.HomeworkVideos)
                .HasForeignKey(lv => lv.HomeworkId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(lv => lv.Video)
                .WithMany(v => v.HomeworkVideos)
                .HasForeignKey(lv => lv.VideoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
