using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.DeleteTopicResource
{
    public class DeleteTopicResourceCommandHandler : IRequestHandler<DeleteTopicResourceCommand, bool>
    {
        private readonly IAsyncRepository<TopicResource, int> _resourceRepository;

        public DeleteTopicResourceCommandHandler(IAsyncRepository<TopicResource, int> resourceRepository)
        {
            _resourceRepository = resourceRepository;
        }

        public async Task<bool> Handle(DeleteTopicResourceCommand request, CancellationToken cancellationToken)
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
