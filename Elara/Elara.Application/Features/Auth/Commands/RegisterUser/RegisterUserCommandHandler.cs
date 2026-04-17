using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Models.Auth;
using Elara.Domain.Entities.Administrative;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthUserData>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        private readonly IOtpRepository _otpRepository;
        private readonly IEmailService _emailService;

        public RegisterUserCommandHandler(
          IIdentityService identityService,
          ITokenService tokenService,
          IOtpRepository otpRepository,
          IEmailService emailService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
            _otpRepository = otpRepository;
            _emailService = emailService;
        }

        public async Task<AuthUserData> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var registeredUser = await _identityService.RegisterAsync(new RegisterUserData
            {
                Email = request.Email,
                Password = request.Password,
                Name = request.Name,
                DateOfBirth = request.DateOfBirth,
                Role = request.Role,
                SubjectId = request.SubjectId
            });


            // Generate OTP for email verification
            var otp = GenerateOtp();
            var otpHash = ComputeHash(otp);

            await _otpRepository.AddAsync(new OtpCode
            {
                UserId = registeredUser.UserId,
                OtpHash = otpHash,
                Type = OtpType.EmailVerification,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                Attempts = 0,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            await _otpRepository.SaveChangesAsync(cancellationToken);
            await _emailService.SendEmailVerificationAsync(request.Email, otp, cancellationToken);
        
            return registeredUser;
        }

        private static string GenerateOtp() =>
            Random.Shared.Next(100000, 999999).ToString();

        private static string ComputeHash(string value)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(value);
            var hash = System.Security.Cryptography.SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
