using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.JunctionTables;
using MediatR;


namespace Elara.Application.Features.Users.Parents.Commands.RemoveChild
{
    public class RemoveChildCommandHandler : IRequestHandler<RemoveChildCommand, bool>
    {
        private readonly IAsyncRepository<StudentParent, int> _studentParentRepository;
        private readonly ICurrentUserService _currentUserService;

        public RemoveChildCommandHandler(
            IAsyncRepository<StudentParent, int> studentParentRepository,
            ICurrentUserService currentUserService)
        {
            _studentParentRepository = studentParentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(RemoveChildCommand request, CancellationToken cancellationToken)
        {
            var parentId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Parent is not authenticated.");

            var links = await _studentParentRepository.FindAsync(
                sp => sp.ParentId == parentId && sp.StudentId == request.ChildId && !sp.IsDeleted,
                cancellationToken);

            var link = links.FirstOrDefault();
            if (link == null)
            {
                throw new KeyNotFoundException("No active link found between your account and this child.");
            }

            link.IsDeleted = true;
            await _studentParentRepository.UpdateAsync(link, cancellationToken);

            return true;
        }
    }
}
