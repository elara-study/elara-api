using Elara.Domain.Entities.Submissions;
using Elara.Domain.Entities.Users;
using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Educational
{
    public class Assignment : BaseEntity
    {
        public string Title { get; set; } 
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }

        [Range(0, 1000)]
        public int MaxScore { get; set; } = 100;
        public bool IsRequired { get; set; } = true;
        public bool IsAIGenerated { get; set; } = false;
        public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Easy;
        public AssignmentType AssignmentType { get; set; } = AssignmentType.Quiz;

        // Foreign Key
        public int TopicId { get; set; }
        public int? LessonId { get; set; }
        public Guid? TeacherId { get; set; }


        // Navigation Properties
        public virtual Topic Topic { get; set; } = null!;
        public virtual Lesson? Lesson { get; set; }
        public virtual Teacher?Teacher { get; set; } 
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<StudentSubmission> Submissions { get; set; }


        // Validation Method
        public bool IsValidDueDate()
        {
            return DueDate > CreatedAt;
        }
    }
}
