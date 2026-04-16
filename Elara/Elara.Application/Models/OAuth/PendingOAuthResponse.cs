namespace Elara.Application.Models.OAuth
{
    public class PendingOAuthResponse
    {
        public string Status       { get; set; } = "pending_registration";
        public string PendingToken { get; set; } = string.Empty;
    }
}
