using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Quiz;
using Elara.Application.Features.Quiz.DTOs;
using MediatR;

namespace Elara.Application.Features.Quiz.Queries.GetHistory
{
    public class GetQuizHistoryQueryHandler : IRequestHandler<GetQuizHistoryQuery, QuizHistoryListDto>
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetQuizHistoryQueryHandler(
            IQuizRepository quizRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _quizRepository = quizRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<QuizHistoryListDto> Handle(GetQuizHistoryQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) throw new Exception("User not authenticated");

            var (sessions, totalCount) = await _quizRepository.GetStudentQuizHistoryAsync(
                userId.Value,
                request.ModuleId,
                request.Page,
                request.PageSize,
                cancellationToken);

            var sessionDtos = _mapper.Map<List<QuizHistoryDto>>(sessions);

            return new QuizHistoryListDto
            {
                Sessions = sessionDtos,
                TotalCount = totalCount
            };
        }
    }
}
