using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Administrative
{
    public class Achievement : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public AchievementType AchievementType { get; set; }

        [Range(0, 1000)]
        public int Points { get; set; } = 10;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(100)]
        public string? ImagePublicId { get; set; }

        public int TargetValue { get; set; }

        // Navigation Properties
        public virtual ICollection<StudentAchievement> StudentAchievements { get; set; } = new List<StudentAchievement>();
    }
}
