using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elara.Domain.Entities.Submissions
{
    public class StudentSubmission : BaseEntity
    {
        public string? Content { get; set; }
        public double Score { get; set; } = 0;
        public string? TeacherFeedback { get; set; }
        public string? AIFeedback { get; set; }

        // Foreign Keys
        public Guid StudentId { get; set; }
        public int AssignmentId { get; set; }

        // Navigation Properties
        public virtual Student Student { get; set; } = null!;
        public virtual Assignment Assignment { get; set; } = null!;
        public virtual ICollection<StudentSubmissionAnswer> Answers { get; set; } = new List<StudentSubmissionAnswer>();

        // Validation
        public bool ValidateScore()
        {
            return Score >= 0 && Score <= Assignment.MaxScore;
        }

    }
}
