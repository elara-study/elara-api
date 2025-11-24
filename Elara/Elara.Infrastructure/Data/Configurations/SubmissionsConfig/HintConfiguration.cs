using Elara.Domain.Entities.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Infrastructure.Data.Configurations.SubmissionsConfig
{
    public class HintConfiguration : IEntityTypeConfiguration<Hint>
    {
        public void Configure(EntityTypeBuilder<Hint> builder)
        {
            builder.ToTable("Hints");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Content)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(h => h.IsAIGenerated)
                .HasDefaultValue(false);

            builder.Property(h => h.HintLevel)
                .HasDefaultValue(1);

            // Indexes
            builder.HasIndex(h => h.QuestionId)
                .HasDatabaseName("IX_Hints_QuestionId");

            builder.HasIndex(h => h.StudentId)
                .HasDatabaseName("IX_Hints_StudentId");

            builder.HasIndex(h => new { h.QuestionId, h.StudentId })
                .HasDatabaseName("IX_Hints_QuestionId_StudentId");

            // Relationships
            builder.HasOne(h => h.Question)
                .WithMany(q => q.Hints)
                .HasForeignKey(h => h.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(h => h.Student)
                .WithMany(s => s.Hints)
                .HasForeignKey(h => h.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
