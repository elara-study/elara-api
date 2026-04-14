using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Elara.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpOptions _options;
        public EmailService(IOptions<SmtpOptions> options)
        {
            _options = options.Value;
        }
        public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string otp, CancellationToken cancellationToken = default)
        {
            var subject = "Password Reset OTP";
            var body = $@"
                 <h2>Password Reset OTP</h2>
                 <p>Hi {userName},</p>
                 <p>Your OTP code is:</p>
                 <h1 style='letter-spacing: 8px'>{otp}</h1>
                 <p>This code expires in <strong>10 minutes</strong>. Do not share it with anyone.</p>";

            await SendEmailAsync(toEmail, subject, body, cancellationToken);
        }
        public async Task SendEmailVerificationAsync(string toEmail, string otp, CancellationToken cancellationToken = default)
        {
            var subject = "Verify Your Email";
            var body = $@"
                   <h2>Email Verification</h2>
                   <p>Your verification code is:</p>
                   <h1 style='letter-spacing: 8px'>{otp}</h1>
                   <p>This code expires in <strong>10 minutes</strong>. Do not share it with anyone.</p>";

            await SendEmailAsync(toEmail, subject, body, cancellationToken);
        }
        private async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken)
        {
            using var client = new SmtpClient(_options.Host, _options.Port)
            {
                Credentials = new NetworkCredential(_options.Username, _options.Password),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_options.FromEmail, _options.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(to);

            await client.SendMailAsync(message, cancellationToken);
        }
    }
}