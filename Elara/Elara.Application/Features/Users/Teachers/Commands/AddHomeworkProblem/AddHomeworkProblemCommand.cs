using Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.AddHomeworkProblem
{
    public class AddHomeworkProblemCommand : IRequest<HomeworkProblemDto>
    {
        public AddHomeworkProblemCommand(Guid moduleId, string description)
        {
            ModuleId = moduleId;
            Description = description;
        }

        public Guid ModuleId { get; }
        public string Description { get; }
    }
}
