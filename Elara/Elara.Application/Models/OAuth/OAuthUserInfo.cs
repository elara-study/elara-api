namespace Elara.Application.Models.OAuth
{
    public class OAuthUserInfo
    {
        public string ProviderUserId { get; set; } = string.Empty;
        public string Email          { get; set; } = string.Empty;
        public string Name           { get; set; } = string.Empty;
    }
}
