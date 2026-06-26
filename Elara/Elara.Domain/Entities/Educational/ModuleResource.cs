using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Educational
{
    public class ModuleResource : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Url]
        public string Url { get; set; } = string.Empty;
        public string? PublicId { get; set; }
        public ResourceType ResourceType { get; set; }

        public string? ThumbnailUrl { get; set; }
        public string? SizeOrDurationText { get; set; }

        public int ModuleId { get; set; }

        public virtual Module Module { get; set; } = null!;
    }
}
