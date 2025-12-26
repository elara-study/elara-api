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
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");

            builder.HasKey(q => q.Id);

            builder.Property(q => q.Text)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(q => q.QuestionType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(q => q.DifficultyLevel)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(DifficultyLevel.Easy);

            builder.Property(q => q.IsAIGenerated)
                .HasDefaultValue(false);

            builder.Property(q => q.HasVideoSupport)
                .HasDefaultValue(false);

            builder.Property(q => q.Marks)
                .HasDefaultValue(10)
                .HasPrecision(5, 2);

            builder.Property(q => q.AssignmentId)
                .IsRequired();


            // Indexes
            builder.HasIndex(q => q.AssignmentId)
                .HasDatabaseName("IX_Questions_AssignmentId");

            builder.HasIndex(q => q.QuestionType)
                .HasDatabaseName("IX_Questions_QuestionType");

            builder.HasIndex(q => q.DifficultyLevel)
                .HasDatabaseName("IX_Questions_DifficultyLevel");

            // Relationships

            builder.HasOne(q => q.Assignment)
                .WithMany(a => a.Questions)
                .HasForeignKey(q => q.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
