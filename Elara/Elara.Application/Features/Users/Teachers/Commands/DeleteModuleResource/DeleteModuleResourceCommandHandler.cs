using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.DeleteModuleResource
{
    public class DeleteModuleResourceCommandHandler : IRequestHandler<DeleteModuleResourceCommand, bool>
    {
        private readonly IAsyncRepository<ModuleResource, int> _resourceRepository;

        public DeleteModuleResourceCommandHandler(IAsyncRepository<ModuleResource, int> resourceRepository)
        {
            _resourceRepository = resourceRepository;
        }

        public async Task<bool> Handle(DeleteModuleResourceCommand request, CancellationToken cancellationToken)
        {
            var resource = await _resourceRepository.GetByIdAsync(request.ResourceId, cancellationToken);
            if (resource == null)
            {
                throw new KeyNotFoundException($"Resource with id {request.ResourceId} not found.");
            }

            await _resourceRepository.DeleteAsync(resource, cancellationToken);
            return true;
        }
    }
}
