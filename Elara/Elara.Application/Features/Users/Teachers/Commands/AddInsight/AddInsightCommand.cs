using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.AddInsight
{
    public class AddInsightCommand : IRequest<AddInsightResponse>
    {
        public Guid StudentId { get; }
        public string Content { get; }

        public AddInsightCommand(Guid studentId, string content)
        {
            StudentId = studentId;
            Content = content;
        }
    }
}
