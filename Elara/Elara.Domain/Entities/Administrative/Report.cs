using Elara.Domain.Entities.IdentityEntites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elara.Domain.Entities.Administrative
{
    public class Report : BaseEntity
    {
        [Required]
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;

        [Range(0, 100)]
        public double AverageScore { get; set; } = 0;

        [MaxLength(2000)]
        public string? ImprovementTips { get; set; }

        [MaxLength(2000)]
        public string? WeakAreas { get; set; } 
        [MaxLength(2000)]
        public string? StrengthAreas { get; set; }

        [MaxLength(1000)]
        public string? Summary { get; set; }

        public int TotalAssignmentsCompleted { get; set; } = 0;

        public int TotalHintsUsed { get; set; } = 0;

        [Range(0, 100)]
        public double CompletionRate { get; set; } = 0;

        // Foreign Key
        [Required]
        public Guid StudentId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(StudentId))]
        public virtual ApplicationUser Student { get; set; } = null!;
    }
}
