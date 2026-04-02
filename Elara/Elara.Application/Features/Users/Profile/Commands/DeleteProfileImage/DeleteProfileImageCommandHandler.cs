using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Identity;
using MediatR;

namespace Elara.Application.Features.Users.Profile.Commands.DeleteProfileImage
{
    public class DeleteProfileImageCommandHandler : IRequestHandler<DeleteProfileImageCommand, Guid>
    {
        private readonly IIdentityService _identityService;
        private readonly IImageStorageService _imageStorageService;

        public DeleteProfileImageCommandHandler(IIdentityService identityService, IImageStorageService imageStorageService)
        {
            _identityService = identityService;
            _imageStorageService = imageStorageService;
        }

        public async Task<Guid> Handle(DeleteProfileImageCommand request, CancellationToken cancellationToken)
        {
            var existingImage = await _identityService.GetUserImageDataAsync(request.UserId);

            if (!string.IsNullOrWhiteSpace(existingImage.ImagePublicId))
            {
                await _imageStorageService.DeleteAsync(existingImage.ImagePublicId, cancellationToken);
            }

            await _identityService.UpdateUserImageAsync(request.UserId, null, null);
            return request.UserId;
        }
    }
}
