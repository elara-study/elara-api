using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Quiz;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.StartQuiz
{
    public class StartQuizSessionCommandHandler : IRequestHandler<StartQuizSessionCommand, QuizSessionDto>
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IAsyncRepository<Assignment, int> _assignmentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public StartQuizSessionCommandHandler(
            IQuizRepository quizRepository,
            IAsyncRepository<Assignment, int> assignmentRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _quizRepository = quizRepository;
            _assignmentRepository = assignmentRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<QuizSessionDto> Handle(StartQuizSessionCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
                throw new Exception("User not authenticated");
            //check if the assignment exists
            var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
            if (assignment == null)
                throw new Exception("Quiz not found");
            
            // create new quiz session
            var session = new QuizSession
            {
                StudentId = userId.Value,
                AssignmentId = request.AssignmentId,
                StartedAt = DateTime.UtcNow,
                Status = Domain.Enums.QuizSessionStatus.InProgress
            };

            session = await _quizRepository.AddAsync(session, cancellationToken);
            var fullSession = await _quizRepository.GetSessionWithDetailsAsync(session.Id, cancellationToken);
            
            return _mapper.Map<QuizSessionDto>(fullSession);
        }
    }
}
