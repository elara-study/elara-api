using Elara.Application.Contracts.Identity;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IIdentityService _identityService;

        public ChangePasswordCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            await _identityService.ChangePasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword);
        }
    }
}