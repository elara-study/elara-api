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

            var page = Math.Max(1, request.Page);
            var pageSize = Math.Max(1, Math.Min(request.PageSize, 50));

            // Calculate top students
            var topStudentsEntities = await _studentRepository.GetTopStudentsAsync(page, pageSize, cancellationToken);
            var studentIds = topStudentsEntities.Select(s => s.Id).ToList();

            if (!studentIds.Contains(userId))
            {
                studentIds.Add(userId);
            }

            var studentNamesMap = await _studentRepository.GetStudentNamesAsync(studentIds, cancellationToken);

            var leaderboard = new List<LeaderboardStudentDto>();
            int currentRank = ((page - 1) * pageSize) + 1;
            bool currentUserInTop = false;

            foreach (var student in topStudentsEntities)
            {
                bool isCurrentUser = student.Id == userId;
                if (isCurrentUser) currentUserInTop = true;

                string realName = studentNamesMap.TryGetValue(student.Id, out var name) ? name : "Unknown Student";

                leaderboard.Add(new LeaderboardStudentDto
                {
                    Rank = currentRank++,
                    UserId = student.Id.ToString(),
                    Name = isCurrentUser ? "You" : realName, 
                    Xp = student.TotalXP,
                    AvatarUrl = null,
                    IsCurrentUser = isCurrentUser
                });
            }

            // Find current user's rank
            if (!currentUserInTop && page == 1)
            {
                var currentUserEntity = await _studentRepository.GetByIdAsync(userId, cancellationToken);
                if (currentUserEntity != null)
                {
                    var userRank = await _studentRepository.GetStudentRankAsync(userId, currentUserEntity.TotalXP, currentUserEntity.CreatedAt, cancellationToken);

                    leaderboard.Add(new LeaderboardStudentDto
                    {
                        Rank = userRank,
                        UserId = userId.ToString(),
                        Name = "You",
                        Xp = currentUserEntity.TotalXP,
                        AvatarUrl = null,
                        IsCurrentUser = true
                    });
                }
            }

            return new LeaderboardDto
            {
                Leaderboard = leaderboard 
            };
        }
    }
}
