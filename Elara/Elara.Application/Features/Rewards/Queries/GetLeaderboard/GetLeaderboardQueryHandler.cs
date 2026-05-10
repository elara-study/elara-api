using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Application.Features.Rewards.DTOs;
using MediatR;

namespace Elara.Application.Features.Rewards.Queries.GetLeaderboard
{
    public class GetLeaderboardQueryHandler : IRequestHandler<GetLeaderboardQuery, LeaderboardDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetLeaderboardQueryHandler(
            IStudentRepository studentRepository,
            ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<LeaderboardDto> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId ?? throw new Exception("User must be authenticated");

            // Calculate top students
            var topStudentsEntities = await _studentRepository.GetTopStudentsAsync(request.Page, request.PageSize, cancellationToken);
            var studentIds = topStudentsEntities.Select(s => s.Id).ToList();
            
            var studentNamesMap = await _studentRepository.GetStudentNamesAsync(studentIds, cancellationToken);

            var topStudentsDto = new List<LeaderboardStudentDto>();
            int currentRank = ((request.Page - 1) * request.PageSize) + 1;

            foreach (var student in topStudentsEntities)
            {
                bool isCurrentUser = student.Id == userId;
                string realName = studentNamesMap.TryGetValue(student.Id, out var name) ? name : "Unknown Student";

                topStudentsDto.Add(new LeaderboardStudentDto
                {
                    Rank = currentRank++,
                    Name = isCurrentUser ? "You" : realName, 
                    Xp = student.TotalXP,
                    Level = student.Level,
                    IsCurrentUser = isCurrentUser,
                    PhotoUrl = null
                });
            }

            // Find current user's rank
            var currentUserEntity = topStudentsEntities.FirstOrDefault(s => s.Id == userId) 
                ?? await _studentRepository.GetByIdAsync(userId, cancellationToken);
                
            if (currentUserEntity == null) throw new Exception("Current user not found");

            var userRank = await _studentRepository.GetStudentRankAsync(userId, currentUserEntity.TotalXP, currentUserEntity.CreatedAt, cancellationToken);

            var currentUserRankDto = new LeaderboardStudentDto
            {
                Rank = userRank,
                Name = "You",
                Xp = currentUserEntity.TotalXP,
                Level = currentUserEntity.Level,
                IsCurrentUser = true,
                PhotoUrl = null
            };

            return new LeaderboardDto
            {
                TopStudents = topStudentsDto,
                CurrentUserRank = currentUserRankDto
            };
        }
    }
}
