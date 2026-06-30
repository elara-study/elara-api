using Elara.Domain.Entities.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations.SubmissionsConfig
{
    public class StudentSubmissionConfiguration : IEntityTypeConfiguration<StudentSubmission>
    {
        public void Configure(EntityTypeBuilder<StudentSubmission> builder)
        {
            builder.ToTable("StudentSubmissions");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Content)
                .HasMaxLength(10000);

            builder.Property(s => s.Score)
                .HasDefaultValue(0);

            builder.Property(s => s.TeacherFeedback)
                .HasMaxLength(2000);

            builder.Property(s => s.AIFeedback)
                .HasMaxLength(2000);

            builder.Property(s => s.StudentId)
                .IsRequired();

            builder.Property(s => s.HomeworkId)
                .IsRequired();

            builder.HasIndex(s => s.StudentId)
                .HasDatabaseName("IX_StudentSubmissions_StudentId");

            builder.HasIndex(s => s.HomeworkId)
                .HasDatabaseName("IX_StudentSubmissions_HomeworkId");

            builder.HasIndex(s => new { s.StudentId, s.HomeworkId })
                .HasDatabaseName("IX_StudentSubmissions_StudentId_HomeworkId");

            builder.HasOne(s => s.Homework)
                .WithMany()
                .HasForeignKey(s => s.HomeworkId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
