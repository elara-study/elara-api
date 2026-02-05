using Elara.Domain.Entities.IdentityEntites;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Enums;
namespace Elara.Domain.Entities.Users
{
    public class Student : BaseEntity<Guid>
    {
        public GradeLevel GradeLevel { get; set; }
        public LearningLevel LearningLevel { get; set; } = LearningLevel.Beginner;
        public bool IsActive { get; set; } = true;

        // Foreign Keys
        public Guid? ParentId { get; set; }

        // Navigation Properties
        public virtual Parent? Parent { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<StudentClass> StudentClasses { get; set; } 
        public virtual ICollection<StudentTeacher> StudentTeachers { get; set; }
        public virtual ICollection<StudentAchievement> StudentAchievements { get; set; } 
    }
}
