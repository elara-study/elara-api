using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Identity;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService; 
        public ChangePasswordCommandHandler(
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            await _identityService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
        }
    }
}