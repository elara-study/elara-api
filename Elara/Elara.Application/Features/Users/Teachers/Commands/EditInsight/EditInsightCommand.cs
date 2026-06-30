using Elara.Application.Features.Users.Teachers.Commands.AddInsight;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.EditInsight
{
    public class EditInsightCommand : IRequest<AddInsightResponse>
    {
        public Guid InsightId { get; }
        public string Content { get; }

        public EditInsightCommand(Guid insightId, string content)
        {
            InsightId = insightId;
            Content = content;
        }
    }
}
