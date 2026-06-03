namespace Elara.Application.Models.Auth
{
    public class RegisterUserData
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Role { get; set; } = string.Empty;
        public int? SubjectId { get; set; }
        public int? Grade { get; set; }
    }
}
