using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Quiz;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using MediatR;
using Elara.Application.Responses;

namespace Elara.Application.Features.Quiz.Commands.SubmitAnswer
{
    public class SubmitQuizAnswerCommandHandler : IRequestHandler<SubmitQuizAnswerCommand, BaseResponse<SubmitAnswerResponse>>
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IAsyncRepository<Question, int> _questionRepository;
        private readonly IQuizService _quizService;

        public SubmitQuizAnswerCommandHandler(
            IQuizRepository quizRepository,
            IAsyncRepository<Question, int> questionRepository,
            IQuizService quizService)
        {
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _quizService = quizService;
        }

        public async Task<BaseResponse<SubmitAnswerResponse>> Handle(SubmitQuizAnswerCommand request, CancellationToken cancellationToken)
        {
            var question = await _questionRepository.GetByIdAsync(request.Answer.QuestionId, cancellationToken);
            if (question == null) throw new Exception("Question not found");

            var isCorrect = false;
            var xpAwarded = 0;
            int? correctOptionId = null;

            if (question.QuestionType == QuestionType.MultipleChoice)
            {
                //automatic correction of MCQ
                var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                correctOptionId = correctOption?.Id;
                isCorrect = request.Answer.SelectedOptionId == correctOptionId;
                
                if (isCorrect) xpAwarded = (int)question.Marks;
            }
            else if (question.QuestionType == QuestionType.Essay)
            {
                //correction of essay using the AI service
                var gradingResult = await _quizService.GradeEssayAnswerAsync(
                    question.Text, 
                    request.Answer.AnswerContent ?? "", 
                    cancellationToken);
                
                isCorrect = gradingResult.IsCorrect;
                xpAwarded = (int)(question.Marks * (gradingResult.Score / 100.0));
            }

            var quizAnswer = new QuizAnswer
            {
                QuizSessionId = request.SessionId,
                QuestionId = request.Answer.QuestionId,
                QuestionType = question.QuestionType,
                SelectedOptionId = request.Answer.SelectedOptionId,
                AnswerContent = request.Answer.AnswerContent,
                IsCorrect = isCorrect,
                XpAwarded = xpAwarded,
                HintUsed = request.Answer.HintUsed
            };

            var session = await _quizRepository.GetByIdAsync(request.SessionId, cancellationToken);
            if (session == null) throw new Exception("Session not found");
            
            session.Answers.Add(quizAnswer);
            await _quizRepository.UpdateAsync(session, cancellationToken);

            var data = new SubmitAnswerResponse
            {
                QuestionId = question.Id,
                IsCorrect = isCorrect,
                CorrectOptionId = correctOptionId,
                XpAwarded = xpAwarded
            };

            return new BaseResponse<SubmitAnswerResponse>
            {
                Message = "Answer submitted successfully.",
                Data = data
            };
        }
    }
}
