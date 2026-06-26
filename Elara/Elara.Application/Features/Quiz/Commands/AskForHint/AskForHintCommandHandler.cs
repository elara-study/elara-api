using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Quiz;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Entities.Submissions;
using System.Text.Json;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.AskForHint
{
    public class AskForHintCommandHandler : IRequestHandler<AskForHintCommand, HintDto>
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IGeminiService _geminiService;

        public AskForHintCommandHandler(
            IQuizRepository quizRepository,
            IGeminiService geminiService)
        {
            _quizRepository = quizRepository;
            _geminiService = geminiService;
        }

        public async Task<HintDto> Handle(AskForHintCommand request, CancellationToken cancellationToken)
        {
            var session = await _quizRepository.GetSessionWithDetailsAsync(request.SessionId, cancellationToken);
            if (session == null || string.IsNullOrEmpty(session.QuestionsJson))
                throw new Exception("Session or questions not found");

            var quizData = JsonSerializer.Deserialize<AIQuizResult>(session.QuestionsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (quizData?.Questions == null || request.QuestionNumber < 1 || request.QuestionNumber > quizData.Questions.Count)
                throw new Exception("Invalid question number");

            var question = quizData.Questions[request.QuestionNumber - 1];

            string hintContent;

            if (!string.IsNullOrEmpty(question.Hint))
            {
                hintContent = question.Hint;
            }
            else
            {
                var optionsText = string.Join(", ", question.Options?.Select(o => o.Text) ?? new List<string>());
                var prompt = $@"Provide a subtle educational hint for the following question.
                             Do NOT give the answer.
                             Question: {question.Text}
                             Options: {optionsText}
                             Requirement: The hint should be helpful but not direct. Keep it short.";

                hintContent = await _geminiService.GenerateResponseAsync(prompt, "", [], cancellationToken);
                hintContent = hintContent.Trim();
            }

            // Mark hint as used
            var existingAnswer = session.Answers.FirstOrDefault(a => a.QuestionText == question.Text);
            if (existingAnswer != null)
            {
                existingAnswer.HintUsed = true;
                await _quizRepository.UpdateAsync(session, cancellationToken);
            }

            return new HintDto
            {
                Content = hintContent,
                HintLevel = 1
            };
        }

        private class AIQuizResult
        {
            public string Title { get; set; } = string.Empty;
            public List<AIQuestion> Questions { get; set; } = new();
        }

        private class AIQuestion
        {
            public string Text { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Hint { get; set; } = string.Empty;
            public List<AIOption>? Options { get; set; }
        }

        private class AIOption
        {
            public string Text { get; set; } = string.Empty;
        }
    }
}
