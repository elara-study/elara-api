namespace Elara.Application.Models.OAuth
{
    public class OAuthUserData
    {
        public string Provider       { get; set; } = string.Empty;
        public string ProviderUserId { get; set; } = string.Empty;
        public string Email          { get; set; } = string.Empty;
    }
}
