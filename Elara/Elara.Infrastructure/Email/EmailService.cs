using brevo_csharp.Api;
using brevo_csharp.Model;
using Microsoft.Extensions.Options;
using Configuration = brevo_csharp.Client.Configuration;

namespace Elara.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly BrevoOptions _options;

        public EmailService(IOptions<BrevoOptions> options)
        {
            _options = options.Value;
        }

        public async System.Threading.Tasks.Task SendPasswordResetEmailAsync(string toEmail, string userName, string otp, CancellationToken cancellationToken = default)
        {
            var subject = "Password Reset OTP";
            var body = $@"
                <h2>Password Reset OTP</h2>
                <p>Hi {userName},</p>
                <p>Your OTP code is:</p>
                <h1 style='letter-spacing: 8px'>{otp}</h1>
                <p>This code expires in <strong>10 minutes</strong>. Do not share it with anyone.</p>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async System.Threading.Tasks.Task SendEmailVerificationAsync(string toEmail, string otp, CancellationToken cancellationToken = default)
        {
            var subject = "Verify Your Email";
            var body = $@"
                <h2>Email Verification</h2>
                <p>Your verification code is:</p>
                <h1 style='letter-spacing: 8px'>{otp}</h1>
                <p>This code expires in <strong>10 minutes</strong>. Do not share it with anyone.</p>";

            await SendEmailAsync(toEmail, subject, body);
        }

        private async System.Threading.Tasks.Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var config = new Configuration();
            config.ApiKey["api-key"] = _options.ApiKey;
                                                                        
            var apiInstance = new TransactionalEmailsApi(config);

            var sendSmtpEmail = new SendSmtpEmail(
                sender: new SendSmtpEmailSender(_options.FromName, _options.FromEmail),
                to: new List<SendSmtpEmailTo> { new SendSmtpEmailTo(toEmail) },
                subject: subject,
                htmlContent: htmlBody
            );

            await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
        }
    }
}