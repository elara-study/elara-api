using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Quiz;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using MediatR;
using Elara.Application.Responses;

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
            var question = await _quizRepository.GetQuestionWithDetailsAsync(request.Answer.QuestionId, cancellationToken);
            if (question == null) throw new Exception("Question not found");

            var session = await _quizRepository.GetSessionWithAnswersAsync(request.SessionId, cancellationToken);
            if (session == null) throw new Exception("Session not found");

            bool isCorrect = false;
            int xpAwarded = 0;
            int? correctOptionId = null;

            if (question.QuestionType == QuestionType.MultipleChoice)
            {
                var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                correctOptionId = correctOption?.Id;
                isCorrect = request.Answer.SelectedOptionId == correctOptionId;
                xpAwarded = isCorrect ? (int)question.Marks : 0;
            }
            else if (question.QuestionType == QuestionType.Essay)
            {
                var gradingResult = await _quizService.GradeEssayAnswerAsync(
                    question.Text, 
                    request.Answer.AnswerContent ?? "", 
                    cancellationToken);
                
                isCorrect = gradingResult.IsCorrect;
                xpAwarded = (int)(question.Marks * (gradingResult.Score / 100.0));
            }

            // Find or create answer
            var answer = session.Answers.FirstOrDefault(a => a.QuestionId == request.Answer.QuestionId);
            if (answer == null)
            {
                answer = new QuizAnswer
                {
                    QuizSessionId = request.SessionId,
                    QuestionId = request.Answer.QuestionId,
                    QuestionType = question.QuestionType
                };
                session.Answers.Add(answer);
            }

            answer.SelectedOptionId = question.QuestionType == QuestionType.Essay ? null : request.Answer.SelectedOptionId;
            answer.AnswerContent = request.Answer.AnswerContent;
            answer.IsCorrect = isCorrect;
            answer.XpAwarded = xpAwarded;
            answer.HintUsed = request.Answer.HintUsed;

            await _quizRepository.UpdateAsync(session, cancellationToken);

            var response = new SubmitAnswerResponse
            {
                QuestionId = question.Id,
                IsCorrect = isCorrect,
                CorrectOptionId = correctOptionId,
                XpAwarded = xpAwarded
            };

            return new BaseResponse<SubmitAnswerResponse>
            {
                Data = response,
                Message = "Answer submitted successfully."
            };
        }
    }
}
