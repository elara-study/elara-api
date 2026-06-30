using Elara.Application.Features.Quiz.DTOs;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.GenerateQuiz
{
    public class GenerateQuizCommand : IRequest<GeneratedQuizDto>
    {
        public Guid GroupId { get; set; }
        public Guid ModuleId { get; set; }
        public int QuestionCount { get; set; } = 10;
        public string DifficultyLevel { get; set; } = "Medium";
        public List<string> QuestionTypes { get; set; } = new();
    }
}
