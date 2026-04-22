using Elara.Domain.Entities.Educational;
using GradeLevel = Elara.Domain.Enums.GradeLevel;
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

            builder.HasData(
                new Subject { Id = 1, Name = "Chemistry", Description = "Chemistry", GradeLevel = GradeLevel.Grade12, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new Subject { Id = 2, Name = "Physics", Description = "Physics", GradeLevel = GradeLevel.Grade12, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new Subject { Id = 3, Name = "Biology", Description = "Biology", GradeLevel = GradeLevel.Grade12, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new Subject { Id = 4, Name = "PureMathematics", Description = "PureMathematics", GradeLevel = GradeLevel.Grade12, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new Subject { Id = 5, Name = "AppliedMathematics", Description = "AppliedMathematics", GradeLevel = GradeLevel.Grade12, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new Subject { Id = 6, Name = "Arabic", Description = "Arabic", GradeLevel = GradeLevel.Grade12, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new Subject { Id = 7, Name = "English", Description = "English", GradeLevel = GradeLevel.Grade12, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false }
            );
        }
    }
}
