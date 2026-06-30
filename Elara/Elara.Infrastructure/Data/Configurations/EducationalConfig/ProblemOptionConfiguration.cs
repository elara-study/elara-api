using Elara.Domain.Entities.Educational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.EducationalConfig
{
    public class ProblemOptionConfiguration : IEntityTypeConfiguration<ProblemOption>
    {
        public void Configure(EntityTypeBuilder<ProblemOption> builder)
        {
            builder.ToTable("ProblemOptions");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Text)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(o => o.IsCorrect)
                .HasDefaultValue(false);

            builder.Property(o => o.ProblemId)
                .IsRequired();

            builder.HasIndex(o => o.ProblemId)
                .HasDatabaseName("IX_ProblemOptions_ProblemId");

            builder.HasOne(o => o.Problem)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.ProblemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
