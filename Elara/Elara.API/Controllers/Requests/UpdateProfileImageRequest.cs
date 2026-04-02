using Microsoft.AspNetCore.Http;

namespace Elara.API.Controllers.Requests
{
    public class UpdateProfileImageRequest
    {
        public IFormFile? File { get; set; }
    }
}
