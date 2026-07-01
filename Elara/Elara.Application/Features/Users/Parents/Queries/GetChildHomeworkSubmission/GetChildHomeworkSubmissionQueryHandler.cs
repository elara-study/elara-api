using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using MediatR;

namespace Elara.Application.Features.Users.Parents.Queries.GetChildHomeworkSubmission
{
    public class GetChildHomeworkSubmissionQueryHandler : IRequestHandler<GetChildHomeworkSubmissionQuery, ChildHomeworkSubmissionDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IAsyncRepository<Homework, int> _homeworkRepository;
        private readonly IAsyncRepository<StudentSubmission, int> _submissionRepository;
        private readonly IAsyncRepository<StudentSubmissionAnswer, int> _submissionAnswerRepository;
        private readonly IAsyncRepository<Problem, int> _problemRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetChildHomeworkSubmissionQueryHandler(
            IStudentRepository studentRepository,
            IAsyncRepository<Homework, int> homeworkRepository,
            IAsyncRepository<StudentSubmission, int> submissionRepository,
            IAsyncRepository<StudentSubmissionAnswer, int> submissionAnswerRepository,
            IAsyncRepository<Problem, int> problemRepository,
            ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _homeworkRepository = homeworkRepository;
            _submissionRepository = submissionRepository;
            _submissionAnswerRepository = submissionAnswerRepository;
            _problemRepository = problemRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ChildHomeworkSubmissionDto> Handle(GetChildHomeworkSubmissionQuery request, CancellationToken cancellationToken)
        {
            var parentId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Parent is not authenticated.");

            // Verify relationship
            var isParent = await _studentRepository.IsParentOfStudentAsync(parentId, request.ChildId, cancellationToken);
            if (!isParent)
            {
                throw new UnauthorizedAccessException("You are not authorized to access this child's homework submission.");
            }

            var homework = await _homeworkRepository.GetByIdAsync(request.HomeworkId, cancellationToken);
            if (homework == null)
            {
                throw new KeyNotFoundException("Homework not found.");
            }

            var submissions = await _submissionRepository.FindAsync(
                s => s.StudentId == request.ChildId && s.HomeworkId == request.HomeworkId && !s.IsDeleted,
                cancellationToken);
            var submission = submissions.FirstOrDefault();
            if (submission == null)
            {
                throw new KeyNotFoundException("No submission found for this student and homework.");
            }

            var submissionAnswers = await _submissionAnswerRepository.FindAsync(
                sa => sa.StudentSubmissionId == submission.Id && !sa.IsDeleted,
                cancellationToken);

            var problems = await _problemRepository.FindAsync(
                p => p.HomeworkId == request.HomeworkId && !p.IsDeleted,
                cancellationToken);
            var problemsMap = problems.ToDictionary(p => p.Id);

            var response = new ChildHomeworkSubmissionDto
            {
                total_score = $"{(int)Math.Round(submission.Score)} / {homework.MaxScore}"
            };

            int idx = 1;
            foreach (var ans in submissionAnswers)
            {
                problemsMap.TryGetValue(ans.ProblemId, out var prob);

                var isImageUrl = !string.IsNullOrWhiteSpace(ans.ImageUrl);

                response.answers.Add(new SubmissionAnswerItemDto
                {
                    problem_label = $"ANSWER {idx++}",
                    is_correct = ans.IsCorrect ?? false,
                    question_text = prob?.Text ?? "Unknown Question",
                    student_answer = new StudentAnswerDetailDto
                    {
                        type = isImageUrl ? "image" : "text",
                        url = isImageUrl ? ans.ImageUrl : null,
                        text = isImageUrl ? null : (ans.TextAnswer ?? string.Empty)
                    }
                });
            }

            return response;
        }
    }
}
