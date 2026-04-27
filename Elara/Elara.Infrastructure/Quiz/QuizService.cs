using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Entities.Submissions;
using System.Text.Json;


namespace Elara.Infrastructure.Quiz
{
    public class QuizService : IQuizService
    {
        private readonly IGeminiService _geminiService;
        private readonly IStudentRepository _studentRepository;

        public QuizService(IGeminiService geminiService, IStudentRepository studentRepository)
        {
            _geminiService = geminiService;
            _studentRepository = studentRepository;
        }

        public async Task<int> CalculateTotalXpAsync(QuizSession session, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(session.Answers
                .Where(a => a.IsCorrect == true)
                .Sum(a => a.XpAwarded));
        }

        public async Task<(bool IsCorrect, int Score, string Feedback)> GradeEssayAnswerAsync(string questionText, string studentAnswer, CancellationToken cancellationToken = default)
        {
            var prompt = $@"Grade this student answer for the following question. 
            Question: {questionText}
            Student Answer: {studentAnswer}
            Respond only in JSON format: {{ ""isCorrect"": bool, ""score"": int (0-100), ""feedback"": ""string"" }}";

            try
            {
                var response = await _geminiService.GenerateResponseAsync(prompt, "", [], cancellationToken);
                var json = response.Replace("```json", "").Replace("```", "").Trim();
                var result = JsonSerializer.Deserialize<EssayGradingResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return (result?.IsCorrect ?? false, result?.Score ?? 0, result?.Feedback ?? "");
            }
            catch
            {
                return (false, 0, "AI Grading unavailable");
            }
        }

        public async Task<ElaraInsightDto> GenerateQuizInsightAsync(QuizSession session, CancellationToken cancellationToken = default)
        {
            var performanceData = session.Answers.Select(a => new { a.Question.Text, a.IsCorrect }).ToList();
            var prompt = $@"Analyze this student quiz performance and provide recommendations.
            Performance: {JsonSerializer.Serialize(performanceData)}
            Respond only in JSON format: {{ ""message"": ""general feedback"", ""weakTopics"": [""topic1""], ""recommendation"": ""what to study"" }}";

            try
            {
                var response = await _geminiService.GenerateResponseAsync(prompt, "", [], cancellationToken);
                var json = response.Replace("```json", "").Replace("```", "").Trim();
                return JsonSerializer.Deserialize<ElaraInsightDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
                       ?? new ElaraInsightDto { Message = "Great job!" };
            }
            catch
            {
                return new ElaraInsightDto { Message = "Keep practicing!" };
            }
        }

        public async Task UpdateStudentProgressAsync(Guid studentId, int xpEarned, CancellationToken cancellationToken = default)
        {
            var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
            if (student == null) return;

            student.TotalXP += xpEarned;
            
            student.Level = (student.TotalXP / 1000) + 1;

            if (student.LastActivityDate.HasValue && (DateTime.UtcNow - student.LastActivityDate.Value).TotalDays <= 1)
            {
                student.CurrentStreak++;
            }
            else
            {
                student.CurrentStreak = 1;
            }
            student.LastActivityDate = DateTime.UtcNow;

            await _studentRepository.UpdateAsync(student, cancellationToken);
        }

        public async Task<int> GenerateQuizAsync(int lessonId, int questionCount, string difficulty, List<string> questionTypes, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(1); 
        }

        private class EssayGradingResult
        {
            public bool IsCorrect { get; set; }
            public int Score { get; set; }
            public string Feedback { get; set; } = "";
        }
    }
}
