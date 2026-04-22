using System.ComponentModel.DataAnnotations;

namespace Elara.API.Controllers.Requests
{
    public class AddStudentRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;
    }
}
