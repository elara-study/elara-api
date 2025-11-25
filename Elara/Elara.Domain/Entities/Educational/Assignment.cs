using Elara.Domain.Entities.Submissions;
using Elara.Domain.Entities.Users;
using Elara.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Domain.Entities.Educational
{
    public class Assignment : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(0, 1000)]
        public int MaxScore { get; set; } = 100;

        public bool IsRequired { get; set; } = true;

        public bool IsAIGenerated { get; set; } = false;

        [Required]
        public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Easy;


        // Foreign Key
        [Required]
        public int TopicId { get; set; }

        public int? LessonId { get; set; }

        [Required]
        public int TeacherId { get; set; }


        // Navigation Properties
        [ForeignKey(nameof(TopicId))]
        public virtual Topic Topic { get; set; } = null!;

        [ForeignKey(nameof(LessonId))]
        public virtual Lesson? Lesson { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher Teacher { get; set; } = null!;

        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public virtual ICollection<StudentSubmission> Submissions { get; set; } = new List<StudentSubmission>();


        // Validation Method
        public bool IsValidDueDate()
        {
            return DueDate > CreatedAt;
        }
    }
}
