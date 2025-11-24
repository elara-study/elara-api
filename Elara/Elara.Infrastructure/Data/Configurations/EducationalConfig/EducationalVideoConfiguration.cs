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
    public class EducationalVideoConfiguration : IEntityTypeConfiguration<EducationalVideo>
    {
        public void Configure(EntityTypeBuilder<EducationalVideo> builder)
        {
            builder.ToTable("EducationalVideos");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(v => v.Description)
                .HasMaxLength(1000);

            builder.Property(v => v.Url)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(v => v.DurationInSeconds)
                .IsRequired()
                .HasDefaultValue(0);

            // Indexes
            builder.HasIndex(v => v.Title)
                .HasDatabaseName("IX_EducationalVideos_Title");
        }
    }
}
