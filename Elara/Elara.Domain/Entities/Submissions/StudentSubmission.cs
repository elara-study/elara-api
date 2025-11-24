using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Domain.Entities.Submissions
{
    public class StudentSubmission : BaseEntity
    {
        [MaxLength(10000)]
        public string? Content { get; set; }

        [Range(0, int.MaxValue)]
        public int Score { get; set; } = 0;

        [MaxLength(2000)]
        public string? TeacherFeedback { get; set; }

        [MaxLength(2000)]
        public string? AIFeedback { get; set; }

        [Range(0, 10)]
        public int AttemptsCount { get; set; } = 1;

        [Required]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = null!;

        [ForeignKey(nameof(AssignmentId))]
        public virtual Assignment Assignment { get; set; } = null!;


        // Validation
        public bool ValidateScore()
        {
            return Score >= 0 && Score <= Assignment.MaxScore;
        }

    }
}
