using Elara.Domain.Entities.Users;
using Elara.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.UsersConfig
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedNever();

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Teacher>(t => t.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Subject
            builder.HasOne(t => t.Subject)
                .WithMany(s => s.Teachers)
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(t => t.SubjectId)
                .HasDatabaseName("IX_Teachers_SubjectId");
        }
    }
}
