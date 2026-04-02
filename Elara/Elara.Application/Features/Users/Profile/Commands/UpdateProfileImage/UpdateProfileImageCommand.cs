using MediatR;

namespace Elara.Application.Features.Users.Profile.Commands.UpdateProfileImage
{
    public class UpdateProfileImageCommand : IRequest<UpdateProfileImageResponse>
    {
        public UpdateProfileImageCommand(Guid userId, byte[] fileBytes, string fileName, string contentType)
        {
            UserId = userId;
            FileBytes = fileBytes;
            FileName = fileName;
            ContentType = contentType;
        }

        public Guid UserId { get; }
        public byte[] FileBytes { get; }
        public string FileName { get; }
        public string ContentType { get; }
    }
}
