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
        private readonly IEmailService _emailService;

        public RegisterUserCommandHandler(
          IIdentityService identityService,
          ITokenService tokenService,
          IEmailService emailService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
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

            var otp = await _identityService.GenerateEmailVerificationOtpAsync(registeredUser.UserId);
            
            await _emailService.SendEmailVerificationAsync(request.Email, otp, cancellationToken);
        
            return registeredUser;
        }
    }
}
