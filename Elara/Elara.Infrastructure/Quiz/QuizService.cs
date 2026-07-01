using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Users;
using Elara.Domain.Entities.Submissions;
using Elara.Application.Common.Interfaces;
using Elara.Application.Features.Quiz.DTOs;
using System.Text.Json;

namespace Elara.Infrastructure.Quiz
{
    public class QuizService : IQuizService
    {
        private readonly IGeminiService _geminiService;
        private readonly IAsyncRepository<Module, int> _moduleRepository;
        private readonly IAsyncRepository<Student, Guid> _studentRepository;
        private readonly ICurrentUserService _currentUserService;

        public QuizService(
            IGeminiService geminiService,
            IAsyncRepository<Module, int> moduleRepository,
            IAsyncRepository<Student, Guid> studentRepository,
            ICurrentUserService currentUserService)
        {
            _geminiService = geminiService;
            _moduleRepository = moduleRepository;
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<GenerateQuizResult> GenerateQuizAsync(int moduleId, int questionCount, string difficulty, List<string> questionTypes, CancellationToken cancellationToken)
        {
            var module = await _moduleRepository.GetByIdAsync(moduleId, cancellationToken);
            if (module == null) throw new Exception("Module not found");

            var content = module.Content ?? module.Description ?? module.Title;
            string prompt = $@"Generate a quiz for the module titled '{module.Title}'.
            Content: {content}
            Strict Requirement: Generate EXACTLY {questionCount} questions. No more, no less.
            Difficulty: {difficulty}
            Types: {string.Join(", ", questionTypes)}

            Return ONLY a JSON object with this structure:
            {{
              ""title"": ""{module.Title} Quiz"",
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
                var quizTitle = aiQuiz.Title ?? $"{module.Title} Quiz";

                var questionsForStorage = finalQuestions.Select((q, i) => new AIQuestion
                {
                    Text = q.Text,
                    Type = q.Type,
                    Hint = q.Hint ?? "",
                    Options = q.Options?.Select(o => new AIOption { Text = o.Text, IsCorrect = o.IsCorrect }).ToList()
                }).ToList();

                var quizData = new AIQuizResult
                {
                    Title = quizTitle,
                    Questions = questionsForStorage
                };

                var questionsJson = JsonSerializer.Serialize(quizData, new JsonSerializerOptions { WriteIndented = false });

                return new GenerateQuizResult
                {
                    QuestionsJson = questionsJson,
                    QuizTitle = quizTitle,
                    TotalQuestions = finalQuestions.Count
                };
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

        public async Task<string> GenerateQuizInsightAsync(string questionsJson, ICollection<QuizAnswer> answers, string quizTitle, CancellationToken cancellationToken)
        {
            var quizData = JsonSerializer.Deserialize<AIQuizResult>(questionsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (quizData?.Questions == null || answers == null || answers.Count == 0)
                return "Complete the quiz to receive personalized insights.";

            var answerLookup = answers.ToDictionary(a => a.QuestionText, a => a, StringComparer.OrdinalIgnoreCase);

            var resultsLines = new List<string>();
            foreach (var q in quizData.Questions)
            {
                var studentGotItRight = answerLookup.TryGetValue(q.Text, out var answer) && answer.IsCorrect == true;
                var mark = studentGotItRight ? "[✓]" : "[✗]";
                var correctText = q.Options?.FirstOrDefault(o => o.IsCorrect)?.Text ?? "";
                resultsLines.Add($"{mark} Q: {q.Text}");
                if (!studentGotItRight)
                    resultsLines.Add($"   Correct answer: {correctText}");
            }

            var resultsText = string.Join("\n", resultsLines);
            string prompt = $@"The student completed the quiz '{quizTitle}'. Here are their results:

{resultsText}

Give a VERY concise insight (2-3 sentences max) identifying the student's weak points based on what they got wrong. Suggest what topics they should review. Be encouraging and specific. Do NOT use markdown or bullet points.";

            try
            {
                var insight = await _geminiService.GenerateResponseAsync(prompt, "", [], cancellationToken);
                return insight.Replace("```", "").Trim();
            }
            catch
            {
                int correctCount = answers.Count(a => a.IsCorrect == true);
                int totalCount = quizData.Questions.Count;
                double accuracy = totalCount > 0 ? (double)correctCount / totalCount * 100 : 0;
                if (accuracy >= 80)
                    return "Excellent work! You have mastered this lesson.";
                else if (accuracy >= 50)
                    return "Good job, but there's room for improvement. Review the topics you missed.";
                else
                    return "This topic needs more focus. Try reviewing the material and retaking the quiz.";
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

                var today = DateTime.UtcNow.Date;
                if (student.LastActivityDate.HasValue)
                {
                    var lastActiveDate = student.LastActivityDate.Value.Date;
                    var daysDifference = (today - lastActiveDate).Days;

                    if (daysDifference == 1)
                    {
                        student.CurrentStreak += 1;
                    }
                    else if (daysDifference > 1)
                    {
                        student.CurrentStreak = 1;
                    }
                }
                else
                {
                    student.CurrentStreak = 1;
                }

                student.LastActivityDate = DateTime.UtcNow;
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
            public List<AIOption>? Options { get; set; }
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
