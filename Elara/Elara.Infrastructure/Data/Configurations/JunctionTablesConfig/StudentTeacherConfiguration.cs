using Elara.Domain.Entities.JunctionTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Infrastructure.Data.Configurations.JunctionTablesConfig
{
    public class StudentTeacherConfiguration : IEntityTypeConfiguration<StudentTeacher>
    {
        public void Configure(EntityTypeBuilder<StudentTeacher> builder)
        {
            builder.ToTable("StudentTeachers");

            builder.HasKey(st => st.Id);

            // Composite Index
            builder.HasIndex(st => new { st.StudentId, st.TeacherId })
                .IsUnique()
                .HasDatabaseName("IX_StudentTeachers_StudentId_TeacherId");

            // Relationships
            builder.HasOne(st => st.Student)
                .WithMany(s => s.StudentTeachers)
                .HasForeignKey(st => st.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(st => st.Teacher)
                .WithMany(t => t.StudentTeachers)
                .HasForeignKey(st => st.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
