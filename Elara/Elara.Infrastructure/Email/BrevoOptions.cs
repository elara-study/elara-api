namespace Elara.Infrastructure.Email
{
    public class BrevoOptions
    {
        public const string SectionName = "Brevo";
        public string ApiKey { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
    }
}