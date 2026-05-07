using System.ComponentModel.DataAnnotations;

namespace Elara.API.Controllers.Requests
{
    public class RegisterDeviceTokenRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
