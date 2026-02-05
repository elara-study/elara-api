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
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.ToTable("Subjects");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Description)
                .HasMaxLength(500);

            builder.Property(s => s.GradeLevel)
                .IsRequired()
                .HasConversion<string>();

            // Indexes
            builder.HasIndex(s => s.Name)
                .HasDatabaseName("IX_Subjects_Name");

            builder.HasIndex(s => s.GradeLevel)
                .HasDatabaseName("IX_Subjects_GradeLevel");
        }
    }
}
