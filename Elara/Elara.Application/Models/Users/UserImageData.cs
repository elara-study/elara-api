namespace Elara.Application.Models.Users
{
    public class UserImageData
    {
        public Guid UserId { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImagePublicId { get; set; }
    }
}
