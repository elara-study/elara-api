using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Domain.Entities.JunctionTables
{
    public class StudentAchievement : BaseEntity
    {
        // Foreign Keys
        public int StudentId { get; set; }
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
