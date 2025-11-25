using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Domain.Entities.Users
{
    public class Student : User
    {
        [Required]
        public GradeLevel GradeLevel { get; set; }

        [MaxLength(50)]
        public string LearningLevel { get; set; } = "Beginner";

        [Range(0, 10)]
        public int DailyHintsUsed { get; set; } = 0;

        // Foreign Keys
        public int? ParentId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(ParentId))]
        public virtual Parent? Parent { get; set; }

        public virtual ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
        public virtual ICollection<StudentTeacher> StudentTeachers { get; set; } = new List<StudentTeacher>();
        public virtual ICollection<StudentSubmission> Submissions { get; set; } = new List<StudentSubmission>();
        public virtual ICollection<StudentAchievement> StudentAchievements { get; set; } = new List<StudentAchievement>();
        public virtual ICollection<Hint> Hints { get; set; } = new List<Hint>();
    }
}
