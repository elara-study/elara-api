using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.DeleteModuleResource
{
    public class DeleteModuleResourceCommand : IRequest<bool>
    {
        public int ResourceId { get; set; }
    }
}
