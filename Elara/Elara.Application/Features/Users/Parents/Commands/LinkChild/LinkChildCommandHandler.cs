using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Entities.Users;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Parents.Commands.LinkChild
{
    public class LinkChildCommandHandler : IRequestHandler<LinkChildCommand, bool>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IAsyncRepository<StudentParent, int> _studentParentRepository;
        private readonly IAsyncRepository<Parent, Guid> _parentRepository;
        private readonly ICurrentUserService _currentUserService;

        public LinkChildCommandHandler(
            IStudentRepository studentRepository,
            IAsyncRepository<StudentParent, int> studentParentRepository,
            IAsyncRepository<Parent, Guid> parentRepository,
            ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _studentParentRepository = studentParentRepository;
            _parentRepository = parentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(LinkChildCommand request, CancellationToken cancellationToken)
        {
            var parentId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Parent is not authenticated.");

            var parent = await _parentRepository.GetByIdAsync(parentId, cancellationToken);
            if (parent == null)
            {
                parent = new Parent
                {
                    Id = parentId,
                    IsDeleted = false
                };
                await _parentRepository.AddAsync(parent, cancellationToken);
            }

            if (string.IsNullOrWhiteSpace(request.child_username))
            {
                throw new ArgumentException("Child username is required.");
            }

            var cleanUsername = request.child_username.Trim();
            if (cleanUsername.StartsWith("@"))
            {
                cleanUsername = cleanUsername.Substring(1);
            }

            var childId = await _studentRepository.GetStudentIdByUsernameAsync(cleanUsername, cancellationToken);
            if (!childId.HasValue)
            {
                throw new KeyNotFoundException($"Student with username '{request.child_username}' was not found.");
            }

            var links = await _studentParentRepository.FindAsync(
                sp => sp.ParentId == parentId && sp.StudentId == childId.Value, 
                cancellationToken);

            var existingLink = links.FirstOrDefault();

            if (existingLink != null)
            {
                if (existingLink.Status == StudentParentRelationStatus.Accepted)
                {
                    throw new InvalidOperationException("This child account is already linked to your account.");
                }
                
                if (existingLink.Status == StudentParentRelationStatus.Pending)
                {
                    return true;
                }

                existingLink.Status = StudentParentRelationStatus.Pending;
                existingLink.InitiatedById = parentId;
                await _studentParentRepository.UpdateAsync(existingLink, cancellationToken);
            }
            else
            {
                var newLink = new StudentParent
                {
                    StudentId = childId.Value,
                    ParentId = parentId,
                    Status = StudentParentRelationStatus.Pending,
                    InitiatedById = parentId
                };

                await _studentParentRepository.AddAsync(newLink, cancellationToken);
            }

            return true;
        }
    }
}
