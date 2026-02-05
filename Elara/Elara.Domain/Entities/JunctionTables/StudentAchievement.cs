using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elara.Domain.Entities.JunctionTables
{
    public class StudentAchievement : BaseEntity
    {
        // Foreign Keys
        public Guid StudentId { get; set; }
        public int AchievementId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = null!;

        [ForeignKey(nameof(AchievementId))]
        public virtual Achievement Achievement { get; set; } = null!;

        // Additional Fields
        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
    }
}
