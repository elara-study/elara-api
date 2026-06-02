using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Educational
{
    public class TopicResource : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Url]
        public string Url { get; set; } = string.Empty;

        public ResourceType ResourceType { get; set; }

        public string? ThumbnailUrl { get; set; }
        public string? SizeOrDurationText { get; set; }

        // Foreign Key
        public int TopicId { get; set; }

        // Navigation Properties
        public virtual Topic Topic { get; set; } = null!;
    }
}
