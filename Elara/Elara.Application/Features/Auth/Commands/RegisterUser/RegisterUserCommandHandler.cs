using Elara.Application.Models.Auth;
using Elara.Domain.Entities.IdentityEntites;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Elara.Application.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthUserData>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            ILogger<RegisterUserCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<AuthUserData> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            // Create new user
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                Name = request.Name,
                DateOfBirth = request.DateOfBirth,
                EmailConfirmed = false // TODO: implement email confirmation
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                List<string> errors = result.Errors.Select(e => e.Description).ToList();
                string errorMessage = string.Join("; ", errors);
                
                _logger.LogWarning("User registration failed for {Email}. Errors: {Errors}", 
                    request.Email, errorMessage);

                throw new InvalidOperationException($"Registration failed: {errorMessage}");
            }

            _logger.LogInformation("User {Email} registered successfully with ID {UserId}", 
                request.Email, user.Id);

            return new AuthUserData
            {
                UserId = user.Id,
                Email = user.Email!,
                Name = user.Name
            };
        }
    }
}

