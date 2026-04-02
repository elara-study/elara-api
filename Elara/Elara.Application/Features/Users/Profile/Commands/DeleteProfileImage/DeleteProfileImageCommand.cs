using MediatR;

namespace Elara.Application.Features.Users.Profile.Commands.DeleteProfileImage
{
    public class DeleteProfileImageCommand : IRequest<Guid>
    {
        public DeleteProfileImageCommand(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }
}
