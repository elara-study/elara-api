namespace Elara.Domain.Entities.Administrative
{
    public enum OtpType
    {
        PasswordReset = 1,
        EmailVerification = 2
    }
    public class OtpCode
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string OtpHash { get; set; } = string.Empty;
        public OtpType Type { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public int Attempts { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}