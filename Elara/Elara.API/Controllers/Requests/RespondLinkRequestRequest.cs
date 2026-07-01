using System.ComponentModel.DataAnnotations;

namespace Elara.API.Controllers.Requests
{
    public class RespondLinkRequestRequest
    {
        [Required]
        public string action { get; set; } = string.Empty;
    }
}
