using Elara.Application.Contracts.Persistence;
using Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview;
using Elara.Domain.Entities.Educational;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.EditProblem
{
    public class EditProblemCommandHandler : IRequestHandler<EditProblemCommand, HomeworkProblemDto>
    {
        private readonly IAsyncRepository<Problem, int> _problemRepository;

        public EditProblemCommandHandler(IAsyncRepository<Problem, int> problemRepository)
        {
            _problemRepository = problemRepository;
        }

        public async Task<HomeworkProblemDto> Handle(EditProblemCommand request, CancellationToken cancellationToken)
        {
            var problem = await _problemRepository.GetByIdAsync(request.ProblemId, cancellationToken);
            if (problem == null)
            {
                throw new KeyNotFoundException($"Problem with id {request.ProblemId} not found.");
            }

            problem.Text = request.Text;

            await _problemRepository.UpdateAsync(problem, cancellationToken);

            return new HomeworkProblemDto
            {
                ProblemId = problem.Id,
                Description = problem.Text
            };
        }
    }
}
