using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Parents.Commands.RespondLinkRequest
{
    public class RespondLinkRequestCommandHandler : IRequestHandler<RespondLinkRequestCommand, bool>
    {
        private readonly IAsyncRepository<StudentParent, int> _studentParentRepository;
        private readonly ICurrentUserService _currentUserService;

        public RespondLinkRequestCommandHandler(
            IAsyncRepository<StudentParent, int> studentParentRepository,
            ICurrentUserService currentUserService)
        {
            _studentParentRepository = studentParentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(RespondLinkRequestCommand request, CancellationToken cancellationToken)
        {
            var parentId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Parent is not authenticated.");

            var link = await _studentParentRepository.GetByIdAsync(request.RequestId, cancellationToken);
            if (link == null || link.ParentId != parentId)
            {
                throw new KeyNotFoundException($"Link request with id {request.RequestId} not found.");
            }

            var cleanAction = request.Action?.Trim().ToLowerInvariant();
            if (cleanAction == "accept")
            {
                link.Status = StudentParentRelationStatus.Accepted;
            }
            else if (cleanAction == "decline")
            {
                link.Status = StudentParentRelationStatus.Declined;
            }
            else
            {
                throw new ArgumentException("Action must be either 'accept' or 'decline'.");
            }

            await _studentParentRepository.UpdateAsync(link, cancellationToken);
            return true;
        }
    }
}
