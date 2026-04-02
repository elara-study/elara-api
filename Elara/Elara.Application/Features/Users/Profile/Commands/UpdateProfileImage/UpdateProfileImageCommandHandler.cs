using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Identity;
using MediatR;

namespace Elara.Application.Features.Users.Profile.Commands.UpdateProfileImage
{
    public class UpdateProfileImageCommandHandler : IRequestHandler<UpdateProfileImageCommand, UpdateProfileImageResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly IImageStorageService _imageStorageService;

        public UpdateProfileImageCommandHandler(IIdentityService identityService, IImageStorageService imageStorageService)
        {
            _identityService = identityService;
            _imageStorageService = imageStorageService;
        }

        public async Task<UpdateProfileImageResponse> Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
        {
            var existingImage = await _identityService.GetUserImageDataAsync(request.UserId);

            if (!string.IsNullOrWhiteSpace(existingImage.ImagePublicId))
            {
                await _imageStorageService.DeleteAsync(existingImage.ImagePublicId, cancellationToken);
            }

            using var stream = new MemoryStream(request.FileBytes);
            var uploadResult = await _imageStorageService.UploadAsync(stream, request.FileName, request.ContentType, cancellationToken);

            await _identityService.UpdateUserImageAsync(request.UserId, uploadResult.Url, uploadResult.PublicId);

            return new UpdateProfileImageResponse
            {
                ImageUrl = uploadResult.Url
            };
        }
    }
}
