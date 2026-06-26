using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Quiz;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using Elara.Application.Responses;
using System.Text.Json;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.SubmitAnswer
{
    public class SubmitQuizAnswerCommandHandler : IRequestHandler<SubmitQuizAnswerCommand, BaseResponse<SubmitAnswerResponse>>
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuizService _quizService;

        public SubmitQuizAnswerCommandHandler(IQuizRepository quizRepository, IQuizService quizService)
        {
            _quizRepository = quizRepository;
            _quizService = quizService;
        }

        public async Task<BaseResponse<SubmitAnswerResponse>> Handle(SubmitQuizAnswerCommand request, CancellationToken cancellationToken)
        {
            var session = await _quizRepository.GetSessionWithAnswersAsync(request.SessionId, cancellationToken);
            if (session == null) throw new Exception("Session not found");
            if (session.Status == QuizSessionStatus.Completed) throw new Exception("Quiz already completed");

            if (string.IsNullOrEmpty(session.QuestionsJson))
                throw new Exception("No questions found for this session");

            var quizData = JsonSerializer.Deserialize<AIQuizResult>(session.QuestionsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (quizData?.Questions == null || request.Answer.QuestionNumber < 1 || request.Answer.QuestionNumber > quizData.Questions.Count)
                throw new Exception("Invalid question number");

            var question = quizData.Questions[request.Answer.QuestionNumber - 1];
            var questionType = Enum.Parse<QuestionType>(question.Type, ignoreCase: true);

            bool isCorrect = false;
            int xpAwarded = 0;
            string correctAnswer = "";

            if (questionType == QuestionType.MCQ && question.Options != null)
            {
                var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                correctAnswer = correctOption?.Text ?? "";
                isCorrect = string.Equals(request.Answer.SelectedOptionText?.Trim(), correctAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
                xpAwarded = isCorrect ? 10 : 0;
            }
            else if (questionType == QuestionType.Essay)
            {
                var gradingResult = await _quizService.GradeEssayAnswerAsync(
                    question.Text,
                    request.Answer.AnswerContent ?? "",
                    cancellationToken);

                isCorrect = gradingResult.IsCorrect;
                xpAwarded = gradingResult.Score;
                correctAnswer = "See feedback";
            }

            var answer = new QuizAnswer
            {
                QuizSessionId = request.SessionId,
                QuestionText = question.Text,
                QuestionType = questionType,
                StudentAnswer = request.Answer.SelectedOptionText ?? request.Answer.AnswerContent ?? "",
                CorrectAnswer = correctAnswer,
                IsCorrect = isCorrect,
                XpAwarded = xpAwarded,
                HintUsed = request.Answer.HintUsed
            };

            session.Answers.Add(answer);

            // Update running totals
            session.CorrectAnswers = session.Answers.Count(a => a.IsCorrect == true);
            session.WrongAnswers = session.Answers.Count(a => a.IsCorrect == false);
            session.UnansweredCount = quizData.Questions.Count - session.Answers.Count;

            await _quizRepository.UpdateAsync(session, cancellationToken);

            var response = new SubmitAnswerResponse
            {
                QuestionNumber = request.Answer.QuestionNumber,
                IsCorrect = isCorrect,
                CorrectAnswerText = correctAnswer,
                XpAwarded = xpAwarded
            };

            return new BaseResponse<SubmitAnswerResponse>
            {
                Data = response,
                Message = "Answer submitted successfully."
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
            public bool IsCorrect { get; set; }
        }
    }
}
