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
    public class TopicConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.ToTable("Topics");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.Content)
                .HasMaxLength(5000);

            builder.Property(t => t.SubjectId)
                .IsRequired();

            // Indexes
            builder.HasIndex(t => t.Title)
                .HasDatabaseName("IX_Topics_Title");

            builder.HasIndex(t => t.SubjectId)
                .HasDatabaseName("IX_Topics_SubjectId");

            // Relationships
            builder.HasOne(t => t.Subject)
                .WithMany(s => s.Topics)
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
