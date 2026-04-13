public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string userName, string otp, CancellationToken cancellationToken = default);
}