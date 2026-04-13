namespace Elara.Domain.Entities.Administrative
{
    public class PasswordResetOtp
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string OtpHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public int Attempts { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}