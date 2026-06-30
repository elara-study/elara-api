using Elara.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Administrative
{
    public class TeacherInsight : BaseEntity
    {
        [Required]
        public Guid PublicId { get; set; } = Guid.NewGuid();

        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public virtual Student Student { get; set; } = null!;
        public virtual Teacher Teacher { get; set; } = null!;
    }
}
