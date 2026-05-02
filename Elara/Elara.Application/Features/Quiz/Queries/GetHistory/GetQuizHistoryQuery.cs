using Elara.Application.Features.Quiz.DTOs;
using MediatR;

namespace Elara.Application.Features.Quiz.Queries.GetHistory
{
    public class GetQuizHistoryQuery : IRequest<QuizHistoryListDto>
    {
        public int? LessonId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
