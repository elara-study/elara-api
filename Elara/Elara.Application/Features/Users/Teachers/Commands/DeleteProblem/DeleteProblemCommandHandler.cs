using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.DeleteProblem
{
    public class DeleteProblemCommandHandler : IRequestHandler<DeleteProblemCommand>
    {
        private readonly IAsyncRepository<Problem, int> _problemRepository;

        public DeleteProblemCommandHandler(IAsyncRepository<Problem, int> problemRepository)
        {
            _problemRepository = problemRepository;
        }

        public async Task Handle(DeleteProblemCommand request, CancellationToken cancellationToken)
        {
            var problem = await _problemRepository.GetByIdAsync(request.ProblemId, cancellationToken);
            if (problem == null)
            {
                throw new KeyNotFoundException($"Problem with id {request.ProblemId} not found.");
            }

            await _problemRepository.DeleteAsync(problem, cancellationToken);
        }
    }
}
