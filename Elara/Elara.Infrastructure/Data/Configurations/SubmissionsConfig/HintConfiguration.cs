using Elara.Domain.Entities.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

            builder.Property(h => h.StudentId)
                .IsRequired();

            builder.Property(h => h.ProblemId)
                .IsRequired();

            builder.HasIndex(h => h.ProblemId)
                .HasDatabaseName("IX_Hints_ProblemId");

            builder.HasIndex(h => h.StudentId)
                .HasDatabaseName("IX_Hints_StudentId");

            builder.HasIndex(h => new { h.ProblemId, h.StudentId })
                .HasDatabaseName("IX_Hints_ProblemId_StudentId");

            builder.HasOne(h => h.Problem)
                .WithMany(q => q.Hints)
                .HasForeignKey(h => h.ProblemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(h => h.Student)
                .WithMany()
                .HasForeignKey(h => h.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
