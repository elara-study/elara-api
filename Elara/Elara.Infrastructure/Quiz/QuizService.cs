using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Entities.Users;
using Elara.Application.Common.Interfaces;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Enums;
using System.Text.Json;

namespace Elara.Infrastructure.Quiz
{
    public class QuizService : IQuizService
    {
        private readonly IGeminiService _geminiService;
        private readonly IAsyncRepository<Lesson, int> _lessonRepository;
        private readonly IAsyncRepository<Assignment, int> _assignmentRepository;
        private readonly IAsyncRepository<Student, Guid> _studentRepository;
        private readonly ICurrentUserService _currentUserService;

        public QuizService(
            IGeminiService geminiService, 
            IAsyncRepository<Lesson, int> lessonRepository, 
            IAsyncRepository<Assignment, int> assignmentRepository,
            IAsyncRepository<Student, Guid> studentRepository,
            ICurrentUserService currentUserService)
        {
            _geminiService = geminiService;
            _lessonRepository = lessonRepository;
            _assignmentRepository = assignmentRepository;
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<int> GenerateQuizAsync(int lessonId, int questionCount, string difficulty, List<string> questionTypes, CancellationToken cancellationToken)
        {
            var lesson = await _lessonRepository.GetByIdAsync(lessonId, cancellationToken);
            if (lesson == null) throw new Exception("Lesson not found");

            var currentUserId = _currentUserService.UserId ?? throw new Exception("User must be logged in to generate a quiz.");

            string prompt = $@"Generate a quiz for the lesson titled '{lesson.Title}'.
            Content: {lesson.Content}
            Strict Requirement: Generate EXACTLY {questionCount} questions. No more, no less.
            Difficulty: {difficulty}
            Types: {string.Join(", ", questionTypes)}
            
            Return ONLY a JSON object with this structure:
            {{
              ""title"": ""{lesson.Title} Quiz"",
              ""questions"": [
                {{
                  ""text"": ""Question text"",
                  ""type"": ""MCQ or Essay"",
                  ""hint"": ""Optional hint"",
                  ""options"": [
                    {{ ""text"": ""Option 1"", ""isCorrect"": true }},
                    {{ ""text"": ""Option 2"", ""isCorrect"": false }}
                  ]
                }}
              ]
            }}";

            try
            {
                var response = await _geminiService.GenerateResponseAsync(prompt, "", [], cancellationToken);
                var json = response.Replace("```json", "").Replace("```", "").Trim();
                var aiQuiz = JsonSerializer.Deserialize<AIQuizResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (aiQuiz == null || aiQuiz.Questions == null) throw new Exception("Failed to parse AI response");

                var finalQuestions = aiQuiz.Questions.Take(questionCount).ToList();
                int actualCount = finalQuestions.Count;
                int marksPerQuestion = 10;

                var assignment = new Assignment
                {
                    Title = aiQuiz.Title ?? $"{lesson.Title} Quiz",
                    LessonId = lessonId,
                    TopicId = lesson.TopicId,
                    IsAIGenerated = true,
                    DifficultyLevel = Enum.Parse<DifficultyLevel>(difficulty, ignoreCase: true),
                    DueDate = DateTime.UtcNow.AddDays(7),
                    MaxScore = actualCount * marksPerQuestion,
                    Questions = new List<Question>()
                };

                foreach (var q in finalQuestions)
                {
                    var question = new Question
                    {
                        Text = q.Text,
                        QuestionType = Enum.Parse<QuestionType>(q.Type, ignoreCase: true),
                        Marks = marksPerQuestion,
                        IsAIGenerated = true,
                        Options = q.Options?.Select(o => new QuestionOption { Text = o.Text, IsCorrect = o.IsCorrect }).ToList() ?? new List<QuestionOption>(),
                        Hints = new List<Hint> { new Hint { Content = q.Hint ?? "", StudentId = currentUserId } }
                    };
                    assignment.Questions.Add(question);
                }

                var createdAssignment = await _assignmentRepository.AddAsync(assignment, cancellationToken);
                return createdAssignment.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"AI Quiz Generation failed: {ex.Message}");
            }
        }

        public async Task<(bool IsCorrect, int Score, string Feedback)> GradeEssayAnswerAsync(string questionText, string studentAnswer, CancellationToken cancellationToken)
        {
            string prompt = $@"You are a teacher grading an essay question.
            Question: {questionText}
            Student Answer: {studentAnswer}
            
            Grade the answer based on accuracy (0-100 score). 
            Return ONLY a JSON object:
            {{
              ""isCorrect"": true/false,
              ""score"": 85,
              ""feedback"": ""A short feedback message""
            }}";

            try
            {
                var response = await _geminiService.GenerateResponseAsync(prompt, "", [], cancellationToken);
                var json = response.Replace("```json", "").Replace("```", "").Trim();
                var result = JsonSerializer.Deserialize<AIGradingResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return (result?.IsCorrect ?? false, result?.Score ?? 0, result?.Feedback ?? "N/A");
            }
            catch
            {
                return (true, 50, "Automatic passing grade (AI grading failed).");
            }
        }

        public async Task<ElaraInsightDto> GenerateQuizInsightAsync(QuizSession session, CancellationToken cancellationToken)
        {
            double accuracy = session.Assignment.Questions.Count > 0 
                ? (double)session.CorrectAnswers / session.Assignment.Questions.Count * 100 : 0;

            if (accuracy >= 80)
            {
                return new ElaraInsightDto { 
                    Message = "Excellent work! You have mastered this lesson.",
                    Recommendation = "You are ready for the next topic.",
                    WeakTopics = new List<string>()
                };
            }
            else if (accuracy >= 50)
            {
                return new ElaraInsightDto { 
                    Message = "Good job, but there's room for improvement.",
                    Recommendation = "Try reviewing the parts you got wrong and retake the quiz.",
                    WeakTopics = new List<string> { "Key concepts of " + session.Assignment.Title }
                };
            }
            else
            {
                return new ElaraInsightDto { 
                    Message = "Don't give up! This topic needs more focus.",
                    Recommendation = "I recommend re-watching the lesson video and focusing on the examples.",
                    WeakTopics = new List<string> { session.Assignment.Title }
                };
            }
        }

        public async Task<int> CalculateTotalXpAsync(QuizSession session, CancellationToken cancellationToken)
        {
            return session.CorrectAnswers * 10;
        }

        public async Task UpdateStudentProgressAsync(Guid studentId, int xpEarned, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
            if (student != null)
            {
                student.TotalXP += xpEarned;
                student.Level = (student.TotalXP / 100) + 1;
                student.LastActivityDate = DateTime.UtcNow;
                student.CurrentStreak += 1;
                await _studentRepository.UpdateAsync(student, cancellationToken);
            }
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
            public List<AIOption> Options { get; set; } = new();
        }

        private class AIOption
        {
            public string Text { get; set; } = string.Empty;
            public bool IsCorrect { get; set; }
        }

        private class AIGradingResult
        {
            public bool IsCorrect { get; set; }
            public int Score { get; set; }
            public string Feedback { get; set; } = string.Empty;
        }
    }
}
