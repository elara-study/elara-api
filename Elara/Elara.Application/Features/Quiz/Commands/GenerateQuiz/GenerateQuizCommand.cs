using Elara.Application.Features.Quiz.DTOs;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.GenerateQuiz
{
    public class GenerateQuizCommand : IRequest<GeneratedQuizDto>
    {
        public int LessonId { get; set; }
        public int QuestionCount { get; set; } = 10;
        public string DifficultyLevel { get; set; } = "Medium";
        public List<string> QuestionTypes { get; set; } = new();
    }
}
