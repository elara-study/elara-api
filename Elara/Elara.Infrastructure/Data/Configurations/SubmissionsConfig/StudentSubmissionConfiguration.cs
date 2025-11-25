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

            builder.Property(s => s.AttemptsCount)
                .HasDefaultValue(1);

            builder.Property(s => s.SubmittedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(s => s.StudentId)
                .HasDatabaseName("IX_StudentSubmissions_StudentId");

            builder.HasIndex(s => s.AssignmentId)
                .HasDatabaseName("IX_StudentSubmissions_AssignmentId");

            builder.HasIndex(s => new { s.StudentId, s.AssignmentId })
                .HasDatabaseName("IX_StudentSubmissions_StudentId_AssignmentId");

            builder.HasIndex(s => s.SubmittedAt)
                .HasDatabaseName("IX_StudentSubmissions_SubmittedAt");

            // Relationships
            builder.HasOne(s => s.Student)
                .WithMany(st => st.Submissions)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(s => s.AssignmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
