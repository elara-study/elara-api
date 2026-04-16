public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string userName, string otp, CancellationToken cancellationToken = default);
    Task SendEmailVerificationAsync(string toEmail, string otp, CancellationToken cancellationToken = default);

}