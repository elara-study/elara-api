using Elara.Domain.Entities.Educational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.EducationalConfig
{
    public class RoadmapConfiguration : IEntityTypeConfiguration<Roadmap>
    {
        public void Configure(EntityTypeBuilder<Roadmap> builder)
        {

            builder.ToTable("Roadmaps");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Grade)
                .IsRequired()
                .HasConversion<int>(); 

            builder.Property(r => r.Description)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(r => r.TeacherId)
                .IsRequired();

            builder.Property(r => r.SubjectId)
                .IsRequired();

            builder.Property(r => r.CreatedAt)
       .IsRequired()
       .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(r => r.UpdatedAt)
                .IsRequired(false);

            builder.Property(r => r.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(r => r.DeletedAt)
                .IsRequired(false);

         
            builder.HasOne(r => r.Teacher)
                .WithMany(t => t.Roadmaps)
                .HasForeignKey(r => r.TeacherId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(r => r.Subject)
                .WithMany(s=>s.Roadmaps)
                .HasForeignKey(r => r.SubjectId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.HasMany(r => r.Topics)
                .WithOne(t=>t.Roadmap)  
                .HasForeignKey(t=>t.RoadmapId) 
                .OnDelete(DeleteBehavior.Cascade); 

            builder.HasMany(r => r.Classes)
                .WithOne(c => c.Roadmap)
                .HasForeignKey(c => c.RoadmapId)
                .OnDelete(DeleteBehavior.SetNull); 

            builder.HasIndex(r => r.TeacherId)
                .HasDatabaseName("IX_Roadmaps_TeacherId");

            builder.HasIndex(r => r.SubjectId)
                .HasDatabaseName("IX_Roadmaps_SubjectId");

            builder.HasIndex(r => new { r.TeacherId, r.Name })
                .HasDatabaseName("IX_Roadmaps_TeacherId_Name")
                .IsUnique(); 

            builder.HasIndex(r => new { r.Grade, r.SubjectId })
                .HasDatabaseName("IX_Roadmaps_Grade_SubjectId");

            builder.HasIndex(r => r.IsDeleted)
                .HasDatabaseName("IX_Roadmaps_IsDeleted");

            // Query filter for soft delete
            builder.HasQueryFilter(r => !r.IsDeleted);
        }
    }
}