namespace Elara.Application.Models.Auth
{
    public class AuthUserData
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}