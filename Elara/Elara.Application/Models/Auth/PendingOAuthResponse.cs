namespace Elara.Application.Models.Auth
{
    public class PendingOAuthResponse
    {
        public string Status       { get; set; } = "pending_registration";
        public string PendingToken { get; set; } = string.Empty;
    }
}
