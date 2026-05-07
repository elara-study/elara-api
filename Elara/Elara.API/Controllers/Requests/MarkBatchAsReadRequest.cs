using System.ComponentModel.DataAnnotations;

namespace Elara.API.Controllers.Requests
{
    public class MarkBatchAsReadRequest
    {
        [Required]
        public List<Guid> NotificationIds { get; set; } = new();
    }
}
