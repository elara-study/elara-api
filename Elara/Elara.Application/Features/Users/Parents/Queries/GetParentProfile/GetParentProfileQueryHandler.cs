using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Parents.Queries.GetParentProfile
{
    public class GetParentProfileQueryHandler : IRequestHandler<GetParentProfileQuery, ParentProfileDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly IAsyncRepository<QuizSession, int> _quizSessionRepository;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetParentProfileQueryHandler(
            IStudentRepository studentRepository,
            IClassRepository classRepository,
            IAsyncRepository<QuizSession, int> quizSessionRepository,
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _quizSessionRepository = quizSessionRepository;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<ParentProfileDto> Handle(GetParentProfileQuery request, CancellationToken cancellationToken)
        {
            var parentId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Parent is not authenticated.");

            var parentUser = await _identityService.GetUserByIdAsync(parentId);

            var username = parentUser.Username;
            if (!string.IsNullOrEmpty(username) && !username.StartsWith("@"))
            {
                username = "@" + username;
            }

            var response = new ParentProfileDto
            {
                user = new ParentUserDto
                {
                    name = parentUser.Name,
                    username = username,
                    phone = parentUser.PhoneNumber ?? string.Empty,
                    avatar_url = parentUser.ImageUrl ?? string.Empty
                }
            };

            var children = await _studentRepository.GetByParentIdAsync(parentId, cancellationToken);
            if (!children.Any())
            {
                response.stats = new ParentStatsDto { avg_completion = 0, avg_attendance = 0 };
                return response;
            }

            var childIds = children.Select(c => c.Id).ToList();
            var profiles = await _identityService.GetUserProfilesByIdsAsync(childIds, cancellationToken);
            var images = await _identityService.GetUserImagesByIdsAsync(childIds, cancellationToken);

            var totalCompletionPercentage = 0;
            var totalAttendancePercentage = 0;

            foreach (var child in children)
            {
                var profile = profiles.GetValueOrDefault(child.Id);
                var imageUrl = images.GetValueOrDefault(child.Id, string.Empty);
                var name = profile?.Name ?? child.Id.ToString();

                var studentClasses = await _classRepository.GetStudentGroupsByStudentIdAsync(child.Id, cancellationToken);
                var totalLessons = studentClasses.Sum(c => c.Stats.Lessons.Total);

                var lessonsCompleted = await _quizSessionRepository.CountAsync(
                    s => s.StudentId == child.Id && s.Status == QuizSessionStatus.Completed && !s.IsDeleted,
                    cancellationToken);

                var completionPercentage = totalLessons > 0
                    ? (int)Math.Round((double)lessonsCompleted / totalLessons * 100)
                    : 0;

                totalCompletionPercentage += completionPercentage;

                var attendance = child.CurrentStreak > 0
                    ? Math.Min(100, 90 + child.CurrentStreak)
                    : 95;

                totalAttendancePercentage += attendance;

                response.children.Add(new ParentChildDto
                {
                    id = child.Id.ToString(),
                    name = name,
                    avatar_url = imageUrl
                });
            }

            response.stats = new ParentStatsDto
            {
                avg_completion = (int)Math.Round((double)totalCompletionPercentage / children.Count),
                avg_attendance = (int)Math.Round((double)totalAttendancePercentage / children.Count)
            };

            return response;
        }
    }
}
